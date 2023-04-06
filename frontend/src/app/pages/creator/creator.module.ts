import { NgModule } from '@angular/core';
import { CreatorComponent } from './creator.component';
import { PanelModule } from 'primeng/panel';
import { TabViewModule } from 'primeng/tabview';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';

@NgModule({
  declarations: [
    CreatorComponent
  ],
  exports: [
    CreatorComponent
  ],
  imports: [
    PanelModule,
    TabViewModule,
    CommonModule,
    ButtonModule
  ]
})
export class CreatorModule { }
