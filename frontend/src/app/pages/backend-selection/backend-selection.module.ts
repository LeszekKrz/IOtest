import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BackendSelectionComponent } from './backend-selection.component';
import { RadioButtonModule } from 'primeng/radiobutton';
import { FormsModule } from '@angular/forms';
import { CardModule } from 'primeng/card';



@NgModule({
  declarations: [
    BackendSelectionComponent
  ],
  imports: [
    CommonModule,
    RadioButtonModule,
    FormsModule,
    CardModule
  ]
})
export class BackendSelectionModule { }
