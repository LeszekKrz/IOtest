import { Component, ViewChild } from '@angular/core';
import { OnDestroy, OnInit } from '@angular/core';
import { UserService } from 'src/app/core/services/user.service';
import { UserDTO } from 'src/app/core/models/user-dto';
import { UpdateUserDTO } from 'src/app/core/models/update-user-dto';
import { Router } from '@angular/router';
import { AbstractControl, FormControl, FormGroup, Validators } from '@angular/forms';
import { finalize, Observable, of, Subscription, switchMap, tap } from 'rxjs';
import { MessageService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { DonationService } from 'src/app/core/services/donation.service';


@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss']
})
export class UserComponent implements OnInit, OnDestroy {
  id!: string;
  userForm: FormGroup = new FormGroup({
    name: new FormControl('', Validators.required),
    surname: new FormControl('', Validators.required),
    nickname: new FormControl('', Validators.required),
    avatarImage: new FormControl(''),
    userType: new FormControl<boolean>(false),
    email: new FormControl({value: '', disabled: true}),
    accountBallance: new FormControl({value: '', disabled: true}),
  });
  subscriptions: Subscription[] = [];
  isProgressSpinnerVisible = false;
  ableToWithdraw = false;
  showWithdrawDialog = false;
  withdrawAmount = 0;
  withdrawMax = 0;
  @ViewChild('avatarImageUpload') avatarImageUpload!: FileUpload;

  constructor(
    private userService: UserService,
    private router: Router,
    private messageService: MessageService,
    private donationService: DonationService
  ) { }

  private fillFormGroup(userDTO: UserDTO): void {
    this.userForm.patchValue({
      name: userDTO.name,
      surname: userDTO.surname,
      nickname: userDTO.nickname,
      userType: userDTO.userType === 'Creator',
      email: userDTO.email,
      accountBalance: userDTO.accountBalance,
    });
    this.ableToWithdraw = (userDTO.userType == 'Creator');
    if (userDTO.accountBalance != null)
    {
      this.withdrawMax = userDTO.accountBalance as number;
    }
  }

  ngOnInit(): void {
    const getUserData$ = this.userService.getUser(null).pipe(
      switchMap((userDTO: UserDTO) => {
        this.id = userDTO.id;
        this.fillFormGroup(userDTO);
        if (!userDTO.avatarImage) return of(null);
        return this.userService.downloadFileImage(userDTO.avatarImage).pipe(
          tap((blob: Blob) => {
            const file = new File([blob], 'avatarImage', { type: blob.type });

            let fileList: FileList = {
              0: file,
              length: 1,
              item: (_: number) => file,
            };

            let event = {
              type: 'change',
              target: {
                files: fileList,
              },
            };
            this.avatarImageUpload.onFileSelect(event);
          }),
        );
      }),
    );
    this.subscriptions.push(this.doWithLoading(getUserData$).subscribe());
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

    const avatarImageValue = this.userForm.get('avatarImage')!.value;

    const userToPut: UpdateUserDTO = {
      name: this.userForm.get('name')!.value,
      surname: this.userForm.get('surname')!.value,
      nickname: this.userForm.get('nickname')!.value,
      userType: this.userForm.get('userType')!.value ? 'Creator' : 'Simple',
      avatarImage: avatarImageValue === '' ? null : avatarImageValue,
    };

    const edit$ = this.userService.editUser(this.id, userToPut).pipe(
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

  handleOnAvatarImageSelect(event: { originalEvent: Event; files: File[] }): void {
    const avatarImageFile = event.files[0];
    if (avatarImageFile.type === 'image/png' || avatarImageFile.type === 'image/jpeg') {
      const reader = new FileReader();
      reader.readAsDataURL(avatarImageFile);
      reader.onload = () => {
        this.userForm.patchValue({avatarImage: reader.result as string});
      };
    }
  }

  handleOnAvatarImageRemove(): void {
    this.userForm.patchValue({avatarImage: ''});
    this.avatarImageUpload.clear();
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

  startWithdraw(): void {
    this.showWithdrawDialog = true;
  }

  isWithdrawalImpossible(): boolean {
    return this.withdrawAmount > this.withdrawMax;
  }

  withdraw(): void {
    if (!this.isWithdrawalImpossible())
    {

      const edit$ = this.donationService.withdraw(this.withdrawAmount).pipe(
        tap(() => {
          this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Money was withdrawn'
        })
      })
      );
      this.subscriptions.push(this.doWithLoading(edit$).subscribe({
        complete: () => {
          this.refreshInfo();
       }
      }));

      this.showWithdrawDialog = false;
      this.withdrawAmount = 0;
    }
  }

  refreshInfo(): void {
    const getUserData$ = this.userService.getUser(null).pipe(
      switchMap((userDTO: UserDTO) => {
        this.id = userDTO.id;
        this.fillFormGroup(userDTO);
        if (!userDTO.avatarImage) return of(null);
        return this.userService.downloadFileImage(userDTO.avatarImage).pipe(
          tap((blob: Blob) => {
            const file = new File([blob], 'avatarImage', { type: blob.type });

            let fileList: FileList = {
              0: file,
              length: 1,
              item: (_: number) => file,
            };

            let event = {
              type: 'change',
              target: {
                files: fileList,
              },
            };
            this.avatarImageUpload.onFileSelect(event);
          }),
        );
      }),
    );
    this.subscriptions.push(this.doWithLoading(getUserData$).subscribe());
  }
}
