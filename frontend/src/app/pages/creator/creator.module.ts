import { NgModule } from '@angular/core';
import { CreatorComponent } from './creator.component';
import { TabViewModule } from 'primeng/tabview';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { ReportButtonModule } from 'src/app/core/components/report-button/report-button.module';

@NgModule({
  declarations: [
    CreatorComponent
  ],
  exports: [
    CreatorComponent
  ],
  imports: [
    TabViewModule,
    CommonModule,
    ButtonModule,
    CardModule,
    ReportButtonModule
  ]
})
export class CreatorModule { }
