import { Component, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { finalize, Observable, of, Subscription, switchMap, tap } from 'rxjs';
import { UserForRegistrationDTO } from '../../authentication/models/user-for-registration-dto';
import { UserService } from '../../core/services/user.service';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.scss']
})
export class RegistrationComponent implements OnInit, OnDestroy {
  registerForm!: FormGroup;
  subscriptions: Subscription[] = [];
  isPasswordVisible = false;
  isConfirmPasswordVisible = false;
  isProgressSpinnerVisible = false;
  containsLowerCaseLetterRegex = /[a-z]/;
  constainsUpperCaseLetterRegex = /[A-Z]/;
  containsNumberRegex: RegExp = /\d/;
  constainsSpecialCharacterRegex = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/;
  passwordMinLength = 8;
  passwordInputType = "password";
  passwordEyeIcon = "pi-eye";
  confirmPasswordInputType = "password";
  confirmPasswordEyeIcon = "pi-eye";
  image: string = "";

  constructor(private formBuilder: FormBuilder,
    private userService: UserService,
    private messageService: MessageService,
    private router: Router) { }

  ngOnInit(): void {
    this.registerForm = this.formBuilder.group({
      name: new FormControl('', Validators.required),
      surname: new FormControl('', Validators.required),
      nickname: new FormControl('', Validators.required),
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [
        Validators.required,
        Validators.minLength(this.passwordMinLength),
        Validators.pattern(this.containsLowerCaseLetterRegex),
        Validators.pattern(this.constainsUpperCaseLetterRegex),
        Validators.pattern(this.containsNumberRegex),
        Validators.pattern(this.constainsSpecialCharacterRegex)
      ]),
      confirmPassword: new FormControl('', [Validators.required]),
      userType: new FormControl<boolean>(false),
    }, {
      validators: [
        this.passwordsNotMatching,
      ]
    });
  }

  private passwordsNotMatching: ValidatorFn = (group: AbstractControl): ValidationErrors | null => {
    return group.get('password')!.value === group.get('confirmPassword')!.value
      ? null
      : {passwordsNotMatching: true};
  }

  private doWithLoading(observable$: Observable<any>): Observable<any> {
    return of(this.isProgressSpinnerVisible = true).pipe(
      switchMap(() => observable$),
      finalize(() => this.isProgressSpinnerVisible = false)
    );
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => {
      subscription.unsubscribe();
    });
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    const userForRegistration: UserForRegistrationDTO = {
      name: this.registerForm.get('name')!.value,
      surname: this.registerForm.get('surname')!.value,
      nickname: this.registerForm.get('nickname')!.value,
      email: this.registerForm.get('email')!.value,
      password: this.registerForm.get('password')!.value,
      userType: this.registerForm.get('userType')!.value ? 'Creator' : 'Simple',
      avatarImage: this.image,
    };

    const register$ = this.userService.registerUser(userForRegistration).pipe(
      tap(() => {
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Registration successful.'
        })
      })
    );
    this.subscriptions.push(this.doWithLoading(register$).subscribe({
      // complete: () => {
      //   this.router.navigate(['login']);
      // }
    }));
  }

  hideShowPassword(): void {
    if (this.passwordInputType == 'password') {
      this.passwordInputType = 'text';
      this.passwordEyeIcon = 'pi-eye-slash';
    }
    else {
      this.passwordInputType = 'password';
      this.passwordEyeIcon = 'pi-eye';
    }
  }

  hideShowConfirmPassword(): void {
    if (this.confirmPasswordInputType == 'password') {
      this.confirmPasswordInputType = 'text';
      this.confirmPasswordEyeIcon = 'pi-eye-slash';
    }
    else {
      this.confirmPasswordInputType = 'password';
      this.confirmPasswordEyeIcon = 'pi-eye';
    }
  }

  isInputInvalidAndTouchedOrDirty(inputName: string): boolean {
    const control = this.registerForm.get(inputName)!;
    return this.isInputTouchedOrDirty(control) && control.invalid;
  }

  isInputTouchedOrDirtyAndEmpty(inputName: string): boolean {
    const control = this.registerForm.get(inputName)!;
    return this.isInputTouchedOrDirty(control) && control.hasError('required');
  }

  isEmailInputTouchedOrDirtyAndNotValidEmail(): boolean {
    const control = this.registerForm.get('email')!;
    return this.isInputTouchedOrDirty(control) && control.hasError('email');
  }

  isPasswordInputTouchedOrDirtyAndTooShort(): boolean {
    const control = this.registerForm.get('password')!;
    return this.isInputTouchedOrDirty(control) && control.hasError('minlength');
  }

  isPasswordInputTouchedOrDirtyAndDoesntContainLowerCaseLetter(): boolean {
    const control = this.registerForm.get('password')!;
    return this.isInputTouchedOrDirty(control)
      && control.hasError('pattern')
      && control.errors!['pattern'].requiredPattern === this.containsLowerCaseLetterRegex.toString();
  }

  isPasswordInputTouchedOrDirtyAndDoesntContainUpperCaseLetter(): boolean {
    const control = this.registerForm.get('password')!;
    return this.isInputTouchedOrDirty(control)
      && control.hasError('pattern')
      && control.errors!['pattern'].requiredPattern === this.constainsUpperCaseLetterRegex.toString();
  }

  isPasswordInputTouchedOrDirtyAndDoesntContainNumber(): boolean {
    const control = this.registerForm.get('password')!;
    return this.isInputTouchedOrDirty(control)
      && control.hasError('pattern')
      && control.errors!['pattern'].requiredPattern === this.containsNumberRegex.toString();
  }

  isPasswordInputTouchedOrDirtyAndDoesntContainSpecialCharacter(): boolean {
    const control = this.registerForm.get('password')!;
    return this.isInputTouchedOrDirty(control)
      && control.hasError('pattern')
      && control.errors!['pattern'].requiredPattern === this.constainsSpecialCharacterRegex.toString();
  }

  isConfirmPasswordInputInvalidAndTouchedOrDirty(): boolean {
    const control = this.registerForm.get('confirmPassword')!;
    return this.isInputTouchedOrDirty(control) && (control.invalid || this.registerForm.hasError('passwordsNotMatching'));
  }

  arePasswordAndConfirmPasswordTouchedOrDirtyAndNotMatching(): boolean {
    const passwordControl = this.registerForm.get('password')!;
    const confirmPasswordControl = this.registerForm.get('confirmPassword')!;

    return this.isInputTouchedOrDirty(passwordControl)
      && this.isInputTouchedOrDirty(confirmPasswordControl)
      && this.registerForm.hasError('passwordsNotMatching');
  }

  private isInputTouchedOrDirty(control: AbstractControl): boolean {
    return control.touched || control.dirty;
  }

  handleOnRemove(){
    this.image = "";
  }

  handleFileSelect(event: any){
    var files = event.files;
    var file = files[0];

  if (files && file) {
    var reader = new FileReader();

    reader.onload = this._handleReaderLoaded.bind(this);

    reader.readAsBinaryString(file);
    }
  }

  _handleReaderLoaded(event: any) {
    var binaryString = event.target.result;
    this.image = btoa(binaryString);
    console.log(btoa(binaryString));
  }
}