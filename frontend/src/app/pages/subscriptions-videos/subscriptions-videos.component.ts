import { Component, OnDestroy, OnInit } from '@angular/core';
import { VideoMetadataDto } from 'src/app/core/models/video-metadata-dto';
import { SubscriptionsVideosService } from './services/subscriptions-videos.service';
import { Observable, Subscription, finalize, of, switchMap, tap } from 'rxjs';
import { getTimeAgo } from 'src/app/core/functions/get-time-ago';
import { Router } from '@angular/router';

@Component({
  selector: 'app-subscriptions',
  templateUrl: './subscriptions-videos.component.html',
  styleUrls: ['./subscriptions-videos.component.scss']
})
export class SubscriptionsVideosComponent implements OnInit, OnDestroy {
  videos!: VideoMetadataDto[];
  subscriptions: Subscription[] = [];
  isProgressSpinnerVisible = false;

  constructor(
    private subscriptionsVideosService: SubscriptionsVideosService,
    private router: Router) { }

  ngOnInit(): void {
    const getSubscriptionsVideos$ = this.subscriptionsVideosService.getVideosFromSubscriptions();
    this.subscriptions.push(getSubscriptionsVideos$.subscribe(videosList => this.videos = videosList.videos));
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => {
      subscription.unsubscribe();
    });
  }

  getTimeAgo(video: VideoMetadataDto): string {
    return getTimeAgo(video.uploadDate);
  }

  public goToVideo(id: string): void {
    this.router.navigate(['videos/' + id]);
  }

  public goToCreator(id: string): void {
    this.router.navigate(['creator/' + id]);
  }
}
