import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UpdateVideoMetadataComponent } from './update-video-metadata.component';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { CardModule } from 'primeng/card';
import { FileUploadModule } from 'primeng/fileupload';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { AutoFocusModule } from 'primeng/autofocus';
import { RippleModule } from 'primeng/ripple';
import { ChipsModule } from 'primeng/chips';



@NgModule({
  declarations: [
    UpdateVideoMetadataComponent
  ],
  imports: [
    CommonModule,
    ProgressSpinnerModule,
    CardModule,
    FileUploadModule,
    InputTextModule,
    InputTextareaModule,
    ReactiveFormsModule,
    ButtonModule,
    DropdownModule,
    AutoFocusModule,
    RippleModule,
    ChipsModule,
    FormsModule,
  ]
})
export class UpdateVideoMetadataModule { }
