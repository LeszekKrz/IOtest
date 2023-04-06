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
        }
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
        CreatorModule
    ]
})
export class AppModule { }
