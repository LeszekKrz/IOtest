import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './login.component';
import { ButtonModule } from 'primeng/button';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { CardModule } from 'primeng/card';
import { ReactiveFormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { RouterModule } from '@angular/router';
import { AutoFocusModule } from 'primeng/autofocus';



@NgModule({
  declarations: [
    LoginComponent
  ],
  exports: [
    LoginComponent
  ],
  imports: [
    CommonModule,
    ButtonModule,
    ProgressSpinnerModule,
    CardModule,
    ReactiveFormsModule,
    InputTextModule,
    ButtonModule,
    RouterModule,
    AutoFocusModule
  ]
})
export class LoginModule { }
