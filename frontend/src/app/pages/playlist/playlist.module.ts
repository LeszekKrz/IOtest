import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';
import { PlaylistComponent } from './playlist.component';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { InputSwitchModule } from 'primeng/inputswitch';
import { FormsModule } from '@angular/forms';



@NgModule({
  declarations: [
    PlaylistComponent
  ],
  exports: [
    PlaylistComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    CardModule,
    ButtonModule,
    DialogModule,
    InputTextModule,
    InputSwitchModule
  ]
})
export class PlaylistModule { }
