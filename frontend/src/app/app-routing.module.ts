import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegistrationComponent } from '../app/pages/registration/registration.component';
import { CreatorComponent } from './pages/creator/creator.component';
import { LoginComponent } from '../app/pages/login/login.component';
import { VideoComponent } from './pages/video/video.component';
import { SearchComponent } from './pages/search/search.component';
import { UserComponent } from './pages/user/user.component';
import { AddVideoComponent } from './pages/add-video/add-video.component';
import { HomeComponent } from './pages/home/home.component';
import { PlaylistComponent } from './pages/playlist/playlist.component';
import { UserPlaylistsComponent } from './pages/user-playlists/user-playlists.component';
import { SubscriptionsVideosComponent } from './pages/subscriptions-videos/subscriptions-videos.component'
import { AuthGuard } from './core/guard/auth.guard';
import { UpdateVideoMetadataComponent } from './pages/update-video-metadata/update-video-metadata.component';

const routes: Routes = [
  { path: '', component: HomeComponent, canActivate: [AuthGuard] },
  { path: 'register', component: RegistrationComponent },
  { path: 'creator/:id', component: CreatorComponent, canActivate: [AuthGuard]  },
  { path: 'videos/:videoId', component: VideoComponent, canActivate: [AuthGuard]  },
  { path: 'login', component: LoginComponent },
  { path: 'search', component: SearchComponent, canActivate: [AuthGuard]  },
  { path: 'user', component: UserComponent, canActivate: [AuthGuard]  } ,
  { path: 'add-video', component: AddVideoComponent, canActivate: [AuthGuard]  },
  { path: 'playlist/:id', component: PlaylistComponent, canActivate: [AuthGuard]  },
  { path: 'playlists', component: UserPlaylistsComponent, canActivate: [AuthGuard]  },
  { path: 'subscriptions-videos', component: SubscriptionsVideosComponent, canActivate: [AuthGuard]  },
  { path: 'update-video-metadata/:videoId', component: UpdateVideoMetadataComponent, canActivate: [AuthGuard] },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
