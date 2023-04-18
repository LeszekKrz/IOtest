import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { VideoComponent } from './video.component';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';



@NgModule({
  declarations: [
    VideoComponent,
  ],
  imports: [
    CommonModule,
    ButtonModule,
    CardModule
  ]
})
export class VideoModule { }
