import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { getToken } from 'src/app/core/functions/get-token';
import { UserDTO } from 'src/app/core/models/user-dto';
import { VideoMetadataDto } from 'src/app/core/models/video-metadata-dto';
import { UserService } from 'src/app/core/services/user.service';
import { VideoService } from 'src/app/core/services/video.service';
import { SubscriptionService } from 'src/app/core/services/subscription.service';
import { environment } from 'src/environments/environment';
import { switchMap, tap } from 'rxjs/operators';
import { Subscription, forkJoin } from 'rxjs';
import { MenuItem } from 'primeng/api';
import { Location } from '@angular/common';
import { ReactionsDTO } from './models/reactions-dto';
import { ReactionsService } from './services/reactions.service';
import { AddReactionDTO } from './models/add-reaction-dto';
import { userSubscriptionListDto } from 'src/app/core/models/user-subscription-list-dto';

@Component({
  selector: 'app-video',
  templateUrl: './video.component.html',
  styleUrls: ['./video.component.scss']
})
export class VideoComponent implements OnInit, OnDestroy {
  videoId: string;
  videoUrl: string;
  videoMetadata!: VideoMetadataDto;
  author!: UserDTO;
  isAuthorSubscribed!: boolean;
  videos: VideoMetadataDto[] = [];
  reactions!: ReactionsDTO;
  subscriptions: Subscription[] = [];
  positiveReaction = 'Positive';
  negativeReaction = 'Negative';
  noneReaction = 'None';
  videoMenuModel: MenuItem[] = [
    {
      label: 'Delete',
      icon: 'pi pi-trash',
      command: () => this.deleteVideo(),
    },
    {
      label: 'Report',
      icon: 'pi pi-flag',
      command: () => this.reportVideo(),
    },
    {
      label: 'Edit metadata',
      icon: 'pi pi-file-edit',
      command: () => this.editMetadata(),
    },
  ];

  constructor(
    private route: ActivatedRoute,
    private userService: UserService,
    private router: Router,
    private videoService: VideoService,
    private location: Location,
    private reactionsService: ReactionsService,
    private subscriptionService: SubscriptionService
  ) {
    this.videoId = this.route.snapshot.params['videoId'];
    this.videoUrl = `${environment.webApiUrl}/video/${this.videoId}?access_token=${getToken()}`;
  }

  ngOnInit(): void {
    this.subscriptions.push(this.videoService
      .getVideoMetadata(this.videoId)
      .pipe(
        switchMap(videoMetadata => {
          this.videoMetadata = videoMetadata;

          return forkJoin({
            user: this.userService.getUser(this.videoMetadata.authorId),
            userVideos: this.videoService.getUserVideos(this.videoMetadata.authorId),
            subscriptionList: this.subscriptionService.getSubscriptions()
          });
        })
      )
      .subscribe(({ user, userVideos, subscriptionList }) => {
        this.author = user;
        this.videos = userVideos.videos;
        this.isAuthorSubscribed = this.isThisAuthorSubscribed(subscriptionList);
      }));

    this.getReactions();
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => {
      subscription.unsubscribe();
    });
  }

  private getReactions(): void {
    this.subscriptions.push(this.reactionsService.getReactions(this.videoId).subscribe(reactions => this.reactions = reactions));
  }

  private checkIfAuthorIsSubscribed(): void {
    this.subscriptions.push(this.subscriptionService.getSubscriptions().subscribe(subscriptions => 
        this.isAuthorSubscribed = this.isThisAuthorSubscribed(subscriptions)
      ));
  }

  private isThisAuthorSubscribed(subscriptionList: userSubscriptionListDto): boolean {
    for (let subscription of subscriptionList.subscriptions) {
      if (subscription.id === this.videoMetadata.authorId) {
        return true;
      }
    }
    return false;
  }

  private updateSubCount(): void {
    this.subscriptions.push(this.userService.getUser(this.author.id).subscribe(user => {
      this.author.subscriptionsCount = user.subscriptionsCount;
    }));
  }

  private deleteVideo(): void {
    const deleteVideo$ = this.videoService.deleteVideo(this.videoMetadata.id).pipe(
      tap(() => {
        this.location.back();
      }),
    );
    this.subscriptions.push(deleteVideo$.subscribe());
  }

  private reportVideo(): void {
    // REPORT VIDEO LOGIC HERE
  }

  private editMetadata(): void {
    this.router.navigate(['update-video-metadata/' + this.videoId]);
  }

  public goToUserProfile(id: string): void {
    this.router.navigate(['creator/' + id]);
  }

  public goToVideo(id: string): void {
    this.router.navigate(['videos/' + id]);
  }

  handlePositiveReactionOnClick(): void {
    const addReaction: AddReactionDTO = this.reactions.currentUserReaction === this.positiveReaction
      ? { value: this.noneReaction }
      : { value: this.positiveReaction };

    this.addOrUpdateReaction(addReaction);
  }

  handleNegativeReactionOnClick(): void {
    const addReaction: AddReactionDTO = this.reactions.currentUserReaction === this.negativeReaction
      ? { value: this.noneReaction }
      : { value: this.negativeReaction };

    this.addOrUpdateReaction(addReaction);
  }

  handleSubscribtionOnClick(): void {
    if (this.isAuthorSubscribed) {
      const checkIfAuthorIsSubscribed$ = this.subscriptionService.deleteSubscription(this.author.id).pipe(
        tap(() => {
          this.checkIfAuthorIsSubscribed();
        }),
      );

      this.subscriptions.push(checkIfAuthorIsSubscribed$.subscribe());
    } else {
      const checkIfAuthorIsSubscribed$ = this.subscriptionService.postSubscription(this.author.id).pipe(
        tap(() => {
          this.checkIfAuthorIsSubscribed();
        }),
      );

      this.subscriptions.push(checkIfAuthorIsSubscribed$.subscribe());
    }

    this.updateSubCount();
  }

  private addOrUpdateReaction(addReaction: AddReactionDTO): void {
    const getReactions$ = this.reactionsService.addOrUpdateReaction(this.videoId, addReaction).pipe(
      tap(() => {
        this.getReactions();
      }),
    );

    this.subscriptions.push(getReactions$.subscribe());
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
