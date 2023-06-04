import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { getHttpOptionsWithAuthenticationHeader } from 'src/app/core/functions/get-http-options-with-authorization-header';
import { Observable } from 'rxjs';
import { CommentsDTO } from '../models/comments-dto';
import { CommentDTO } from '../models/comment-dto';
import { getApiUrl } from 'src/app/core/functions/get-api-url';

@Injectable({
  providedIn: 'root'
})
export class CommentsService {
  constructor(private httpClient: HttpClient) { }

  getAllComments(videoId: string): Observable<CommentsDTO> {
    const httpOptions = {
      params: new HttpParams().set('id', videoId),
      headers: getHttpOptionsWithAuthenticationHeader().headers,
    };

    return this.httpClient.get<CommentsDTO>(`${getApiUrl()}/comment`, httpOptions);
  }

  addComment(content: string, videoId: string): Observable<void> {
    const httpOptions = {
      params: new HttpParams().set('id', videoId),
      headers: getHttpOptionsWithAuthenticationHeader().headers,
    };

    return this.httpClient.post<void>(`${getApiUrl()}/comment`, content, httpOptions);
  }

  deleteComment(id: string): Observable<void> {
    const httpOptions = {
      params: new HttpParams().set('id', id),
      headers: getHttpOptionsWithAuthenticationHeader().headers,
    };

    return this.httpClient.delete<void>(`${getApiUrl()}/comment`, httpOptions);
  }

  getAllCommentResponses(commentId: string): Observable<CommentsDTO> {
    const httpOptions = {
      params: new HttpParams().set('id', commentId),
      headers: getHttpOptionsWithAuthenticationHeader().headers,
    };

    return this.httpClient.get<CommentsDTO>(`${getApiUrl()}/comment/response`, httpOptions);
  }

  addCommentResponse(content: string, commentId: string): Observable<void> {
    const httpOptions = {
      params: new HttpParams().set('id', commentId),
      headers: getHttpOptionsWithAuthenticationHeader().headers,
    };

    return this.httpClient.post<void>(`${getApiUrl()}/comment/response`, content, httpOptions);
  }

  deleteCommentResponse(id: string): Observable<void> {
    const httpOptions = {
      params: new HttpParams().set('id', id),
      headers: getHttpOptionsWithAuthenticationHeader().headers,
    };

    return this.httpClient.delete<void>(`${getApiUrl()}/comment/response`, httpOptions);
  }

  getCommentByIdAsync(commentId: string): Observable<CommentDTO> {
    const httpOptions = {
      params: new HttpParams().set('id', commentId),
      headers: getHttpOptionsWithAuthenticationHeader().headers,
    };

    return this.httpClient.get<CommentDTO>(`${getApiUrl()}/comment/commentById`, httpOptions);
  }

  getCommentResponseByIdAsync(commentId: string): Observable<CommentDTO> {
    const httpOptions = {
      params: new HttpParams().set('id', commentId),
      headers: getHttpOptionsWithAuthenticationHeader().headers,
    };

    return this.httpClient.get<CommentDTO>(`${getApiUrl()}/comment/response/commentById`, httpOptions);
  }
}
