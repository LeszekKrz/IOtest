import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegistrationComponent } from '../app/pages/registration/registration.component';
import { LoginComponent } from '../app/pages/login/login.component';
import { VideoComponent } from './pages/video/video.component';

const routes: Routes = [
  { path: 'register', component: RegistrationComponent },
  { path: 'videos/:videoId', component: VideoComponent },
  { path: 'login', component: LoginComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
