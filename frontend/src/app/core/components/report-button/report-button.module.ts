import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DialogModule } from 'primeng/dialog';
import { ReportButtonComponent } from './report-button.component';
import { ButtonModule } from 'primeng/button';
import { FormsModule } from '@angular/forms';




@NgModule({
  declarations: [
    ReportButtonComponent
  ],
  exports: [
    ReportButtonComponent
  ],
  imports: [
    CommonModule,
    DialogModule,
    ButtonModule,
    FormsModule
  ]
})
export class ReportButtonModule { }
