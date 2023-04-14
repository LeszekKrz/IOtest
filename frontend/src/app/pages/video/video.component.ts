import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserDTO } from 'src/app/core/models/user-dto';
import { VideoMetadataDto } from 'src/app/core/models/video-metadata-dto';
import { UserService } from 'src/app/core/services/user.service';
import { VideoService } from 'src/app/core/services/video.service';
import { environment } from 'src/environments/environment';
import { switchMap } from 'rxjs/operators';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-video',
  templateUrl: './video.component.html',
  styleUrls: ['./video.component.scss']
})
export class VideoComponent  {
  videoId: string = "54b12988-c373-4746-b687-a1ad1b883ccb";
  videoUrl: string = `${environment.webApiUrl}/video/${this.videoId}`;
  videoMetadata!: VideoMetadataDto;
  author!: UserDTO;
  videos: VideoMetadataDto[] = [];

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

          // Use forkJoin to make two requests in parallel: one for the user, and one for the user's videos
          return forkJoin({
            user: this.userService.getUser(this.videoMetadata.authorId),
            userVideos: this.videoService.getUserVideos(this.videoMetadata.authorId)
          });
        })
      )
      .subscribe(({ user, userVideos }) => {
        this.author = user;
        this.videos = userVideos.videos;
      });
  }

  public goToUserProfile(id: string): void {
    this.router.navigate(['creator/' + id]);
  }

  public goToVideo(id: string): void {
    this.router.navigate(['videos/' + id]);
  }
}
