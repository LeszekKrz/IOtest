import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';
import { PlaylistComponent } from './playlist.component';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { InputSwitchModule } from 'primeng/inputswitch';
import { FormsModule } from '@angular/forms';
import { ReportButtonModule } from 'src/app/core/components/report-button/report-button.module';



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
    InputSwitchModule,
    ReportButtonModule
  ]
})
export class PlaylistModule { }
