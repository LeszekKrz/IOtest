import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription, forkJoin } from 'rxjs';
import { getTimeAgo } from 'src/app/core/functions/get-time-ago';
import { UserDTO } from 'src/app/core/models/user-dto';
import { VideoMetadataDto } from 'src/app/core/models/video-metadata-dto';
import { UserService } from 'src/app/core/services/user.service';
import { VideoService } from 'src/app/core/services/video.service';

@Component({
  selector: 'app-creator',
  templateUrl: './creator.component.html',
  styleUrls: ['./creator.component.scss']
})
export class CreatorComponent implements OnInit, OnDestroy {
  id!: string;
  user!: UserDTO;
  videos!: VideoMetadataDto[];
  subscriptions: Subscription[] = [];

  constructor(
    private route: ActivatedRoute,
    private userService: UserService,
    private router: Router,
    private videoService: VideoService
    ) {
    this.id = this.route.snapshot.paramMap.get('id')!;
  }

  ngOnInit(): void {
    const getUser$ = this.userService.getUser(this.id);
    const getVideos$ = this.videoService.getUserVideos(this.id);

    this.subscriptions.push(forkJoin([getUser$, getVideos$]).subscribe(([user, videosList]) => {
      this.user = user;
      this.videos = videosList.videos;
    }));
  }

  public goToVideo(id: string): void {
    this.router.navigate(['videos/' + id]);
  }

  public goToUserProfile(id: string): void {
    this.router.navigate(['creator/' + id]);
  }

  public getTimeAgo(video: VideoMetadataDto): string {
    return getTimeAgo(video.uploadDate);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => {
      subscription.unsubscribe();
    });
  }
}
