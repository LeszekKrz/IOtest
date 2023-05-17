import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { getHttpOptionsWithAuthenticationHeader } from 'src/app/core/functions/get-http-options-with-authorization-header';
import { Observable } from 'rxjs';
import { CommentsDTO } from '../models/comments-dto';

@Injectable({
  providedIn: 'root'
})
export class CommentsService {
  private readonly commentWebAPIUrl = `${environment.webApiUrl}/comment`;
  private readonly commentResponseWebAPIUrl = `${this.commentWebAPIUrl}/response`;

  constructor(private httpClient: HttpClient) { }

  getAllComments(videoId: string): Observable<CommentsDTO> {
    const httpOptions = {
      params: new HttpParams().set('id', videoId),
      headers: getHttpOptionsWithAuthenticationHeader().headers,
    };

    return this.httpClient.get<CommentsDTO>(`${this.commentWebAPIUrl}`, httpOptions);
  }

  addComment(content: string, videoId: string): Observable<void> {
    const httpOptions = {
      params: new HttpParams().set('id', videoId),
      headers: getHttpOptionsWithAuthenticationHeader().headers,
    };

    return this.httpClient.post<void>(`${this.commentWebAPIUrl}`, content, httpOptions);
  }

  deleteComment(id: string): Observable<void> {
    const httpOptions = {
      params: new HttpParams().set('id', id),
      headers: getHttpOptionsWithAuthenticationHeader().headers,
    };

    return this.httpClient.delete<void>(`${this.commentWebAPIUrl}`, httpOptions);
  }

  getAllCommentResponses(commentId: string): Observable<CommentsDTO> {
    const httpOptions = {
      params: new HttpParams().set('id', commentId),
      headers: getHttpOptionsWithAuthenticationHeader().headers,
    };

    return this.httpClient.get<CommentsDTO>(`${this.commentResponseWebAPIUrl}`, httpOptions);
  }

  addCommentResponse(content: string, commentId: string): Observable<void> {
    const httpOptions = {
      params: new HttpParams().set('id', commentId),
      headers: getHttpOptionsWithAuthenticationHeader().headers,
    };

    return this.httpClient.post<void>(`${this.commentResponseWebAPIUrl}`, content, httpOptions);
  }

  deleteCommentResponse(id: string): Observable<void> {
    const httpOptions = {
      params: new HttpParams().set('id', id),
      headers: getHttpOptionsWithAuthenticationHeader().headers,
    };

    return this.httpClient.delete<void>(`${this.commentResponseWebAPIUrl}`, httpOptions);
  }
}
