import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegistrationComponent } from '../app/pages/registration/registration.component';
import { CreatorComponent } from './pages/creator/creator.component';
import { LoginComponent } from '../app/pages/login/login.component';
import { VideoComponent } from './pages/video/video.component';
import { HomeComponent } from './pages/home/home.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'register', component: RegistrationComponent },
  { path: 'creator/:id', component: CreatorComponent },
  { path: 'videos/:videoId', component: VideoComponent },
  { path: 'login', component: LoginComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
