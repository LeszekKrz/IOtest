import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CommentsComponent } from './comments.component';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { RippleModule } from 'primeng/ripple';
import { AvatarModule } from 'primeng/avatar';
import { MenuModule } from 'primeng/menu';
import { ProgressSpinnerModule } from 'primeng/progressspinner';

@NgModule({
  declarations: [
    CommentsComponent
  ],
  imports: [
    CommonModule,
    InputTextareaModule,
    ReactiveFormsModule,
    ButtonModule,
    RippleModule,
    AvatarModule,
    MenuModule,
    ProgressSpinnerModule,
  ],
  exports: [
    CommentsComponent,
  ]
})
export class CommentsModule { }
