import { NgModule } from '@angular/core';
import { CreatorComponent } from './creator.component';
import { TabViewModule } from 'primeng/tabview';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';

@NgModule({
  declarations: [
    CreatorComponent
  ],
  exports: [
    CreatorComponent
  ],
  imports: [
    TabViewModule,
    CommonModule,
    ButtonModule,
    CardModule,
  ]
})
export class CreatorModule { }
