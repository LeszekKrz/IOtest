import { Component } from '@angular/core';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-video',
  templateUrl: './video.component.html',
  styleUrls: ['./video.component.scss']
})
export class VideoComponent  {
  videoUrl: string = `${environment.webApiUrl}/video/54b12988-c373-4746-b687-a1ad1b883ccb`;
}
