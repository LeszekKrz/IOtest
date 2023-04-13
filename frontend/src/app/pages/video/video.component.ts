import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserDTO } from 'src/app/core/models/user-dto';
import { VideoMetadataDto } from 'src/app/core/models/video-metadata-dto';
import { UserService } from 'src/app/core/services/user.service';
import { VideoService } from 'src/app/core/services/video.service';
import { environment } from 'src/environments/environment';
import { switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-video',
  templateUrl: './video.component.html',
  styleUrls: ['./video.component.scss']
})
export class VideoComponent  {
  videoUrl: string = `${environment.webApiUrl}/video/54b12988-c373-4746-b687-a1ad1b883ccb`;
  videoId: string = "54b12988-c373-4746-b687-a1ad1b883ccb";
  videoMetadata!: VideoMetadataDto;
  author!: UserDTO;

  constructor(
    private route: ActivatedRoute,
    private userService: UserService,
    private router: Router,
    private videoService: VideoService
  ) {
    // this.videoUrl = this.route.snapshot.paramMap.get('videoId')!;
  
    videoService
      .getVideoMetadata(this.videoId)
      .pipe(
        switchMap(videoMetadata => {
          this.videoMetadata = videoMetadata;
          return this.userService.getUser(this.videoMetadata.authorId);
        })
      )
      .subscribe(user => {
        this.author = user;
      });
  }
}
