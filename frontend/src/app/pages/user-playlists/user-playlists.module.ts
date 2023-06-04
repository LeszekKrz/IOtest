import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserPlaylistsComponent } from './user-playlists.component';
import { ButtonModule } from 'primeng/button';
import { FormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { InputSwitchModule } from 'primeng/inputswitch';
import { DialogModule } from 'primeng/dialog';
import { CardModule } from 'primeng/card';
import {ConfirmPopupModule} from 'primeng/confirmpopup';


@NgModule({
  declarations: [
    UserPlaylistsComponent
  ],
  exports: [
    UserPlaylistsComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    DialogModule,
    InputTextModule,
    InputSwitchModule,
    CardModule,
    ConfirmPopupModule,
  ]
})
export class UserPlaylistsModule { }
