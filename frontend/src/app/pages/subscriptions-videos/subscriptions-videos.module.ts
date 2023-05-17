import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SubscriptionsVideosComponent } from './subscriptions-videos.component';
import { CardModule } from 'primeng/card';
import { ProgressSpinnerModule } from 'primeng/progressspinner';



@NgModule({
  declarations: [
    SubscriptionsVideosComponent
  ],
  imports: [
    CommonModule,
    CardModule,
    ProgressSpinnerModule,
  ]
})
export class SubscriptionsVideosModule { }
