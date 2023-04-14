import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { getToken } from 'src/app/core/functions/get-token';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-video',
  templateUrl: './video.component.html',
  styleUrls: ['./video.component.scss']
})
export class VideoComponent  {
  videoUrl: string = `${environment.webApiUrl}/video/${this.route.snapshot.params['videoId']}?access_token=${getToken()}`;

  constructor(private route: ActivatedRoute) {}
}
