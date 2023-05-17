import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { Subscription } from 'rxjs/internal/Subscription';
import { getTimeAgo } from 'src/app/core/functions/get-time-ago';
import { getToken } from 'src/app/core/functions/get-token';
import { SubscriptionDto } from 'src/app/core/models/subscribtion-dto';
import { VideoFromPlaylistDto } from 'src/app/core/models/video-from-playlist-dto';
import { PlaylistService } from 'src/app/core/services/playlist.service';
import { SubscriptionService } from 'src/app/core/services/subscription.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {
  videos!: VideoFromPlaylistDto[];
  userSubscriptions!: SubscriptionDto[];
  subscriptions: Subscription[] = [];

  constructor(
    private playlistService: PlaylistService,
    private subscriptionService: SubscriptionService,
    private messageService: MessageService,
    private router: Router) {
      this.getRecommended();
      this.getSubscriptions();
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => {
      subscription.unsubscribe();
    });
  }

  getRecommended() {
    this.subscriptions.push(this.playlistService
      .getRecommended()
      .subscribe(recommended => {
        this.videos = recommended.videos;
    }));
  }

  getSubscriptions() {
    this.subscriptions.push(this.subscriptionService
      .getSubscriptions()
      .subscribe(userSubs => {
        this.userSubscriptions = userSubs.subscriptions;
    }));
  }

  public goToVideo(id: string): void {
    this.router.navigate(['videos/' + id]);
  }

  public goToCreator(id: string): void {
    this.router.navigate(['creator/' + id]);
  }

  public getTimeAgo(video: VideoFromPlaylistDto): string {
    return getTimeAgo(video.uploadDate);
  }
}
