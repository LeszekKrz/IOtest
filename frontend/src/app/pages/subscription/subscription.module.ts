import { NgModule } from '@angular/core';
import { SubscriptionComponent } from './subscription.component';
import { PanelModule } from 'primeng/panel';
import { TabViewModule } from 'primeng/tabview';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';

@NgModule({
  declarations: [
    SubscriptionComponent
  ],
  exports: [
    SubscriptionComponent
  ],
  imports: [
    PanelModule,
    TabViewModule,
    CommonModule,
    ButtonModule
  ]
})
export class SubscriptionModule { }