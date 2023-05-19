import { Component } from '@angular/core';
import { OnDestroy, OnInit } from '@angular/core';
import { UserService } from 'src/app/core/services/user.service';
import { UserDTO } from 'src/app/core/models/user-dto';
import { UpdateUserDTO } from 'src/app/core/models/update-user-dto';
import { ActivatedRoute, Router } from '@angular/router';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { finalize, Observable, of, Subscription, switchMap, tap } from 'rxjs';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss']
})
export class UserComponent implements OnInit, OnDestroy {
  id!: string | null;
  user!: UserDTO;
  userForm!: FormGroup;
  subscriptions: Subscription[] = [];
  isProgressSpinnerVisible = false;

  constructor(
    private route: ActivatedRoute, 
    private userService: UserService,
    private router: Router,
    private formBuilder: FormBuilder,
    private messageService: MessageService,
  ) {
    this.id = null;
    this.userService.getUser(this.id).pipe(tap(res => this.redo(res))).subscribe(user => this.user = user)!;
  }

  private redo(userDTO: UserDTO):void {
    this.id = userDTO.id;
    this.userForm.get('name')?.setValue(userDTO.name);
    this.userForm.get('surname')?.setValue(userDTO.surname);
    this.userForm.get('nickname')?.setValue(userDTO.nickname);
    this.userForm.get('userType')?.setValue(userDTO.userType == 'Simple' ? false : true);
    this.userForm.get('email')?.setValue(userDTO.email);
    this.userForm.get('accountBallance')?.setValue(userDTO.accountBalance);
  }

  ngOnInit(): void {
    this.userForm = this.formBuilder.group({
      name: new FormControl('', Validators.required),
      surname: new FormControl('', Validators.required),
      nickname: new FormControl('', Validators.required),
      // avatar TODO
      userType: new FormControl<boolean>(false),
      email: new FormControl({value: '', disabled: true}),
      accountBallance: new FormControl({value: '', disabled: true}),
    });
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => {
      subscription.unsubscribe();
    });
  }

  onSubmit(): void {
    if (this.userForm.invalid) {
      this.userForm.markAllAsTouched();
      return;
    }
 
    const userToPut: UpdateUserDTO = {
      name: this.userForm.get('name')!.value,
      surname: this.userForm.get('surname')!.value,
      nickname: this.userForm.get('nickname')!.value,
      userType: this.userForm.get('userType')!.value ? 'Creator' : 'Simple',
      avatarImage: null,
    };

    const edit$ = this.userService.editUser(this.id!, userToPut).pipe(
      tap(() => {
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Your account info has been updated.'
        })
      })
    );
    this.subscriptions.push(this.doWithLoading(edit$).subscribe({
       complete: () => {
         this.router.navigate(['']);
       }
    }));
  }

  isInputInvalidAndTouchedOrDirty(inputName: string): boolean {
    const control = this.userForm.get(inputName)!;
    return this.isInputTouchedOrDirty(control) && control.invalid;
  }

  isInputTouchedOrDirtyAndEmpty(inputName: string): boolean {
    const control = this.userForm.get(inputName)!;
    return this.isInputTouchedOrDirty(control) && control.hasError('required');
  }

  private isInputTouchedOrDirty(control: AbstractControl): boolean {
    return control.touched || control.dirty;
  }

  private doWithLoading(observable$: Observable<any>): Observable<any> {
    return of(this.isProgressSpinnerVisible = true).pipe(
      switchMap(() => observable$),
      finalize(() => this.isProgressSpinnerVisible = false)
    );
  }
}
