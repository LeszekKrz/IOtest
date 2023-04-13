import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { VideoComponent } from './video.component';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { TimeAgoPipe } from 'src/app/pipes/time-ago.pipe';



@NgModule({
  declarations: [
    VideoComponent,
    TimeAgoPipe
  ],
  imports: [
    CommonModule,
    ButtonModule,
    CardModule
  ]
})
export class VideoModule { }
