import { Component } from '@angular/core';
import { Observable, Subscription, finalize, of, switchMap, tap } from 'rxjs';
import { PlaylistService } from 'src/app/core/services/playlist.service';
import { MessageService } from 'primeng/api';
import { UserPlaylistsDto } from 'src/app/core/models/user-playlists-dto';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-choose-playlist',
  templateUrl: './choose-playlist.component.html',
  styleUrls: ['./choose-playlist.component.scss']
})
export class ChoosePlaylistComponent {
  showDialog = false;
  inputText = '';
  inputSwitch = false;
  userPlaylists!: UserPlaylistsDto[];
  isProgressSpinnerVisible = false;
  subscriptions: Subscription[] = [];
  videoId!: string;


  constructor(
    private playlistService: PlaylistService,
    private messageService: MessageService,
    private router: Router,
    private route : ActivatedRoute) { 
      this.getOwnPlaylists();
      this.videoId = this.route.snapshot.paramMap.get('videoId')!;
    }

  getOwnPlaylists() {
    this.playlistService.getOwnPlaylists().subscribe(
      (userPlaylists) => {
        this.userPlaylists = userPlaylists;
      }
    );
  }

  private doWithLoading(observable$: Observable<any>): Observable<any> {
    return of(this.isProgressSpinnerVisible = true).pipe(
      switchMap(() => observable$),
      finalize(() => this.isProgressSpinnerVisible = false)
    );
  }

  choosePlaylist(id: string)
  {
    const playlist$ = this.playlistService.addToPlaylist(id, this.videoId).pipe(
        tap(() => {
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: 'Video added to playlist'
          })
          this.getOwnPlaylists();
        })
      );
      this.subscriptions.push(this.doWithLoading(playlist$).subscribe()); 
    this.router.navigate(['videos/' + this.videoId]);
  }

  openDialog() {
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

    const playlist$ = this.playlistService.postPlalist(postPlaylistDto).pipe(
      tap(() => {
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Operation successful.'
        })
        this.getOwnPlaylists();
      })
    );
    this.subscriptions.push(this.doWithLoading(playlist$).subscribe());
  }
}
