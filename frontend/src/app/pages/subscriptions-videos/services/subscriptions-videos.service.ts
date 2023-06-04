import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { VideoListDto } from 'src/app/core/models/video-list-dto';
import { getHttpOptionsWithAuthenticationHeader } from '../../../core/functions/get-http-options-with-authorization-header';
import { getApiUrl } from 'src/app/core/functions/get-api-url';

@Injectable({
  providedIn: 'root'
})
export class SubscriptionsVideosService {
  constructor(private httpClient: HttpClient) { }

  getVideosFromSubscriptions(): Observable<VideoListDto> {
    return this.httpClient.get<VideoListDto>(`${getApiUrl()}/user/videos/subscribed`, getHttpOptionsWithAuthenticationHeader());
  }
}
