import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { RegistrationModule } from './pages/registration/registration.module';
import { ToastModule } from 'primeng/toast';
import { MenuModule } from './menu/menu.module';
import { MessageService } from 'primeng/api';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { HttpErrorInterceptor } from './core/interceptors/http-error-interceptor';
import { VideoModule } from './pages/video/video.module';
import { CreatorModule } from './pages/creator/creator.module';
import { LoginModule } from './pages/login/login.module';
import { SearchModule } from './pages/search/search.module';
import { UserModule } from './pages/user/user.module';
import { AddVideoModule } from './pages/add-video/add-video.module';
import { HomeModule } from './pages/home/home.module';
import { PlaylistModule } from './pages/playlist/playlist.module';
import { UserPlaylistsModule } from './pages/user-playlists/user-playlists.module';
import { CommentsModule } from './pages/video/components/comments/comments.module';
import { SubscriptionsVideosModule } from './pages/subscriptions-videos/subscriptions-videos.module';
import { UpdateVideoMetadataModule } from './pages/update-video-metadata/update-video-metadata.module';
import { TicketModule } from './pages/ticket/ticket.module';
import { ReportButtonModule } from './core/components/report-button/report-button.module';

@NgModule({
    declarations: [
        AppComponent,
    ],
    providers: [
        MessageService,
        {
            provide: HTTP_INTERCEPTORS,
            useClass: HttpErrorInterceptor,
            multi: true,
        },
    ],
    bootstrap: [AppComponent],
    imports: [
        BrowserModule,
        AppRoutingModule,
        RegistrationModule,
        LoginModule,
        ToastModule,
        MenuModule,
        BrowserAnimationsModule,
        VideoModule,
        CreatorModule,
        SearchModule,
        UserModule,
        AddVideoModule,
        HomeModule,
        PlaylistModule,
        UserPlaylistsModule,
        PlaylistModule,
        CommentsModule,
        SubscriptionsVideosModule,
        UpdateVideoMetadataModule,
        TicketModule,
        ReportButtonModule
    ]
})
export class AppModule { }
