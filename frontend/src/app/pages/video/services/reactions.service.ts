import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { getHttpOptionsWithAuthenticationHeader } from 'src/app/core/functions/get-http-options-with-authorization-header';
import { AddReactionDTO } from '../models/add-reaction-dto';
import { ReactionsDTO } from '../models/reactions-dto';
import { getApiUrl } from 'src/app/core/functions/get-api-url';

@Injectable({
  providedIn: 'root'
})
export class ReactionsService {
  constructor(private httpClient: HttpClient) { }

  addOrUpdateReaction(videoId: string, addReactionDTO: AddReactionDTO): Observable<void> {
    const httpOptions = {
      params: new HttpParams().set('id', videoId),
      headers: getHttpOptionsWithAuthenticationHeader().headers,
    };

    return this.httpClient.post<void>(`${getApiUrl()}/video-reaction`, addReactionDTO, httpOptions);
  }

  getReactions(videoId: string): Observable<ReactionsDTO> {
    const httpOptions = {
      params: new HttpParams().set('id', videoId),
      headers: getHttpOptionsWithAuthenticationHeader().headers,
    };

    return this.httpClient.get<ReactionsDTO>(`${getApiUrl()}/video-reaction`, httpOptions);
  }
}
