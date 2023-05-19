import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { VideoListDto } from 'src/app/core/models/video-list-dto';
import { environment } from 'src/environments/environment';
import { getHttpOptionsWithAuthenticationHeader } from '../../../core/functions/get-http-options-with-authorization-header';

@Injectable({
  providedIn: 'root'
})
export class SubscriptionsVideosService {
  private readonly videoPageWebAPIUrl: string = `${environment.webApiUrl}/user/videos/subscribed`;

  constructor(private httpClient: HttpClient) { }

  getVideosFromSubscriptions(): Observable<VideoListDto> {
    return this.httpClient.get<VideoListDto>(this.videoPageWebAPIUrl, getHttpOptionsWithAuthenticationHeader());
  }
}
