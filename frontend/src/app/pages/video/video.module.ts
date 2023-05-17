import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { VideoComponent } from './video.component';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { CommentsModule } from './components/comments/comments.module';
import { MenuModule } from 'primeng/menu';



@NgModule({
  declarations: [
    VideoComponent
  ],
  imports: [
    CommonModule,
    ButtonModule,
    CardModule,
    CommentsModule,
    MenuModule,
  ]
})
export class VideoModule { }
