import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserPlaylistsComponent } from './user-playlists.component';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { FormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { InputSwitchModule } from 'primeng/inputswitch';
import { DialogModule } from 'primeng/dialog';



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
    InputSwitchModule
  ]
})
export class UserPlaylistsModule { }
