import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AddVideoComponent } from './add-video.component';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { AutoFocusModule } from 'primeng/autofocus';
import { FileUploadModule } from 'primeng/fileupload';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { RippleModule } from 'primeng/ripple';
import { ChipsModule } from 'primeng/chips';



@NgModule({
  declarations: [
    AddVideoComponent
  ],
  imports: [
    CommonModule,
    ProgressSpinnerModule,
    FormsModule,
    CardModule,
    ButtonModule,
    ReactiveFormsModule,
    InputTextModule,
    DropdownModule,
    AutoFocusModule,
    FileUploadModule,
    InputTextareaModule,
    RippleModule,
    ChipsModule,
  ]
})
export class AddVideoModule { }
