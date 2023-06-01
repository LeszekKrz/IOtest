import { Component } from '@angular/core';
import { Observable, Subscription, finalize, of, switchMap, tap } from 'rxjs';
import { PlaylistService } from 'src/app/core/services/playlist.service';
import { ConfirmationService, MessageService } from 'primeng/api';
import { UserPlaylistsDto } from 'src/app/core/models/user-playlists-dto';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user-playlists',
  templateUrl: './user-playlists.component.html',
  styleUrls: ['./user-playlists.component.scss']
})
export class UserPlaylistsComponent {
  showDialog = false;
  inputText = '';
  inputSwitch = false;
  userPlaylists!: UserPlaylistsDto[];
  isProgressSpinnerVisible = false;
  subscriptions: Subscription[] = [];

  constructor(
    private playlistService: PlaylistService,
    private messageService: MessageService,
    private router: Router,
    private confirmationService: ConfirmationService) {
      this.getOwnPlaylists();
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

  goToPlaylist(id: string) {
    this.router.navigate(['playlist', id]);
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

  showDeletePlaylistPopUp(playlist: UserPlaylistsDto, target: EventTarget): void {
    this.confirmationService.confirm({
      target: target,
      message:  'Are you sure you want to delete this playlist?',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        const deletePlaylist$ = this.playlistService.deletePlaylist(playlist.id).pipe(
          finalize(() => {
            const index = this.userPlaylists.findIndex(userPlaylist => userPlaylist.id === playlist.id);
            if (index > -1) {
              this.userPlaylists.splice(index, 1);
            }
          })
        )

        this.subscriptions.push(this.doWithLoading(deletePlaylist$).subscribe());
      }
    });
  }
}
