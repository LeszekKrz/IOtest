import { Component, NgZone, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { finalize, map, Observable, of, Subscription, switchMap, tap } from 'rxjs';
import { Router } from '@angular/router';
import { UserForLoginDTO } from 'src/app/authentication/models/user-login-dto';
import { UserService } from 'src/app/core/services/user.service';
import { AuthenticationResponseDTO } from 'src/app/authentication/models/authentication-response-dto';
import { UserDTO } from 'src/app/core/models/user-dto';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnDestroy {
    loginForm: FormGroup = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', Validators.required)
  });
  subscriptions: Subscription[] = [];
  isProgressSpinnerVisible = false;
  passwordInputType = 'password';
  passwordEyeIcon = 'pi-eye';

  constructor(
    private userService: UserService,
    private messageService: MessageService,
    private router: Router) { }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => {
      subscription.unsubscribe();
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    const userForLogin: UserForLoginDTO = {
      email: this.loginForm.get('email')!.value,
      password: this.loginForm.get('password')!.value
    };

    const login$ = this.userService.loginUser(userForLogin).pipe(
      tap(() => {
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Login successful'
        });
      }),
      switchMap((authenticationResponse: AuthenticationResponseDTO) => {
        localStorage.setItem('token', authenticationResponse.token);
    
        return this.userService.getUser(null).pipe(
          map((userDTO: UserDTO) => ({ userDTO }))
        );
      })
    );
    
    this.subscriptions.push(
      this.doWithLoading(login$).subscribe({
        next: ({ userDTO }: { userDTO: UserDTO }) => {
          localStorage.setItem('role', userDTO.userType);
        },
        complete: () => {
          this.userService.sendAuthenticationStateChangedNotification(true);
          this.router.navigate(['']);
        }
      })
    );
  }

  private doWithLoading(observable$: Observable<any>): Observable<any> {
    return of(this.isProgressSpinnerVisible = true).pipe(
      switchMap(() => observable$),
      finalize(() => this.isProgressSpinnerVisible = false)
    );
  }

  hideShowPassword(): void {
    if (this.passwordInputType == "password") {
      this.passwordInputType = "text";
      this.passwordEyeIcon = "pi-eye-slash";
    }
    else {
      this.passwordInputType = "password";
      this.passwordEyeIcon = "pi-eye";
    }
  }

  isInputInvalidAndTouchedOrDirty(inputName: string): boolean {
    const control = this.loginForm.get(inputName)!;
    return this.isInputTouchedOrDirty(control) && control.invalid;
  }

  isInputTouchedOrDirtyAndEmpty(inputName: string): boolean {
    const control = this.loginForm.get(inputName)!;
    return this.isInputTouchedOrDirty(control) && control.hasError('required');
  }

  isEmailInputTouchedOrDirtyAndNotValidEmail(): boolean {
    const control = this.loginForm.get('email')!;
    return this.isInputTouchedOrDirty(control) && control.hasError('email');
  }

  private isInputTouchedOrDirty(control: AbstractControl): boolean {
    return control.touched || control.dirty;
  }
}