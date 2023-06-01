import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MenuItem, MessageService } from 'primeng/api';
import { Observable, Subscription, finalize, of, switchMap, tap } from 'rxjs';
import { getTimeAgo } from 'src/app/core/functions/get-time-ago';
import { PlaylistVideosDto } from 'src/app/core/models/playlist-videos-dto';
import { VideoFromPlaylistDto } from 'src/app/core/models/video-from-playlist-dto';
import { PlaylistService } from 'src/app/core/services/playlist.service';

@Component({
  selector: 'app-playlist',
  templateUrl: './playlist.component.html',
  styleUrls: ['./playlist.component.scss']
})
export class PlaylistComponent {
  id!: string;
  playlist!: PlaylistVideosDto;
  showDialog = false;
  inputText = '';
  inputSwitch = false;
  isProgressSpinnerVisible = false;
  subscriptions: Subscription[] = [];

  constructor(
    private route: ActivatedRoute,
    private playlistService: PlaylistService,
    private messageService: MessageService,
    private router: Router) {
    this.id = this.route.snapshot.paramMap.get('id')!;

    this.getPlaylist();
  }

  public getPlaylist() {
    this.playlistService.getPlaylistVideos(this.id).subscribe(playlist => {
      this.playlist = playlist;
    });
  }

  public goToVideo(id: string): void {
    this.router.navigate(['videos/' + id]);
  }

  openDialog() {
    this.inputText = this.playlist.name;
    this.inputSwitch = this.playlist.visibility === 'Public';
    this.showDialog = true;
  }

  closeDialog() {
    this.showDialog = false;
  }

  submitForm() {
    const postPlaylistDto = {
      name: this.inputText,
      visibility: this.inputSwitch ? 'Public' : 'Private'
    };

    this.closeDialog();

    const playlist$ = this.playlistService.updatePlalist(this.id, postPlaylistDto).pipe(
      tap(() => {
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Operation successful.'
        })
        this.playlist.name = postPlaylistDto.name;
        this.playlist.visibility = postPlaylistDto.visibility;
      })
    );
    this.subscriptions.push(this.doWithLoading(playlist$).subscribe());
  }

  doWithLoading(observable$: Observable<any>): Observable<any> {
    return of(this.isProgressSpinnerVisible = true).pipe(
      switchMap(() => observable$),
      finalize(() => this.isProgressSpinnerVisible = false)
    );
  }

  public getTimeAgo(video: VideoFromPlaylistDto): string {
    return getTimeAgo(video.uploadDate);
  }

  getVideoMenuModel(video: VideoFromPlaylistDto): MenuItem[] {
    return [
      {
        label: 'Remove',
        icon: 'pi pi-trash',
        command: (event) => {
          this.removeVideo(video);
          event.originalEvent.stopPropagation();
        },
      },
    ];
  }

  removeVideo(video: VideoFromPlaylistDto): void {
    const removeVideo$ = this.playlistService.deleteVideoFromPlaylist(this.id, video.id).pipe(
      finalize(() => {
        const index = this.playlist.videos.findIndex(v => v.id === video.id);
        if (index > -1) {
          this.playlist.videos.splice(index, 1);
        }
      }),
    );

    this.subscriptions.push(this.doWithLoading(removeVideo$).subscribe());
  }
}
