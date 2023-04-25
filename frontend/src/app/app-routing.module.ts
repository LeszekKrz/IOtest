import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegistrationComponent } from '../app/pages/registration/registration.component';
import { CreatorComponent } from './pages/creator/creator.component';
import { LoginComponent } from '../app/pages/login/login.component';
import { VideoComponent } from './pages/video/video.component';
import { AddVideoComponent } from './pages/add-video/add-video.component';
import { HomeComponent } from './pages/home/home.component';
import { PlaylistComponent } from './pages/playlist/playlist.component';
import { UserPlaylistsComponent } from './pages/user-playlists/user-playlists.component';
import { SubscriptionsVideosComponent } from './pages/subscriptions-videos/subscriptions-videos.component'

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'register', component: RegistrationComponent },
  { path: 'creator/:id', component: CreatorComponent },
  { path: 'videos/:videoId', component: VideoComponent },
  { path: 'login', component: LoginComponent },
  { path: 'add-video', component: AddVideoComponent },
  { path: 'playlist/:id', component: PlaylistComponent },
  { path: 'playlists', component: UserPlaylistsComponent },
  { path: 'subscriptions-videos', component: SubscriptionsVideosComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
