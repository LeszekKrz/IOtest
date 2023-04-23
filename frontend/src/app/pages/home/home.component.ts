import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
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
  subscriptions!: SubscriptionDto[];

  constructor(
    private playlistService: PlaylistService,
    private subscriptionService: SubscriptionService,
    private messageService: MessageService,
    private router: Router) {
      this.getRecommended();
  }

  getRecommended() {
    this.playlistService.getRecommended().subscribe(recommended => {
      this.videos = recommended.videos;
    });
  }

  getSubscriptions() {
    this.subscriptionService.getSubscriptions().subscribe(userSubs => {
      this.subscriptions = userSubs.subscriptions;
    });
  }

  public goToVideo(id: string): void {
    this.router.navigate(['videos/' + id]);
  }

  public goToCreator(id: string): void {
    this.router.navigate(['creator/' + id]);
  }
}
