import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { getToken } from 'src/app/core/functions/get-token';
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
  videoId: string;
  videoUrl: string;
  videoMetadata!: VideoMetadataDto;
  author!: UserDTO;
  videos: VideoMetadataDto[] = [];

  constructor(
    private route: ActivatedRoute,
    private userService: UserService,
    private router: Router,
    private videoService: VideoService
  ) {
    this.videoId = this.route.snapshot.params['videoId'];
    this.videoUrl = `${environment.webApiUrl}/video/${this.videoId}?access_token=${getToken()}`;
  
    videoService
      .getVideoMetadata(this.videoId)
      .pipe(
        switchMap(videoMetadata => {
          this.videoMetadata = videoMetadata;

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

  public getTimeAgo(value: Date): string {
    value = new Date(value);
    const now = new Date();
    const timeDiffInSeconds = Math.floor((now.getTime() - value.getTime()) / 1000);
    const minutes = Math.floor(timeDiffInSeconds / 60);
    const hours = Math.floor(minutes / 60);
    const days = Math.floor(hours / 24);
    const weeks = Math.floor(days / 7);

    if (weeks > 0) {
      return weeks + (weeks === 1 ? ' week' : ' weeks');
    } else if (days > 0) {
      return days + (days === 1 ? ' day' : ' days');
    } else if (hours > 0) {
      return hours + (hours === 1 ? ' hour' : ' hours');
    } else {
      const roundedMinutes = Math.max(1, minutes);
      return roundedMinutes + (roundedMinutes === 1 ? ' minute' : ' minutes');
    }
  }
}
