import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from "@angular/common/http";
import { PostPlaylistDto } from '../models/post-playlist-dto';
import { getHttpOptionsWithAuthenticationHeader } from "../functions/get-http-options-with-authorization-header";
import { UserPlaylistsDto } from '../models/user-playlists-dto';
import { Observable } from 'rxjs';
import { PlaylistVideosDto } from '../models/playlist-videos-dto';
import { getApiUrl } from '../functions/get-api-url';



@Injectable({
  providedIn: 'root'
})
export class PlaylistService {
  constructor(private httpClient: HttpClient) { }

  postPlalist(postPlaylist: PostPlaylistDto): Observable<void>{
    return this.httpClient.post<void>(`${getApiUrl()}/playlist/details`, postPlaylist, getHttpOptionsWithAuthenticationHeader());
  }

  addToPlaylist(playlistId: string, videoId: string): Observable<void>
  {
    return this.httpClient.post<void>(`${getApiUrl()}/playlist/${playlistId}/${videoId}`, null, getHttpOptionsWithAuthenticationHeader());
  }

  updatePlalist(id: string, updatePlaylist: PostPlaylistDto): Observable<void>{
    const httpOptions = {
      params: new HttpParams().set('id', id),
      headers: getHttpOptionsWithAuthenticationHeader().headers
    };

    return this.httpClient.put<void>(`${getApiUrl()}/playlist/details`, updatePlaylist, httpOptions);
  }

  getOwnPlaylists(): Observable<UserPlaylistsDto[]>{
    return this.httpClient.get<UserPlaylistsDto[]>(`${getApiUrl()}/playlist/user`, getHttpOptionsWithAuthenticationHeader());
  }

  getUserPlaylists(id: string): Observable<UserPlaylistsDto[]>{
    const httpOptions = {
      params: new HttpParams().set('id', id),
      headers: getHttpOptionsWithAuthenticationHeader().headers
    };
    return this.httpClient.get<UserPlaylistsDto[]>(`${getApiUrl()}/playlist/user`, httpOptions);
  }

  getPlaylistVideos(playlistId: string): Observable<PlaylistVideosDto>{
    const httpOptions = {
      params: new HttpParams().set('id', playlistId),
      headers: getHttpOptionsWithAuthenticationHeader().headers
    };

    return this.httpClient.get<PlaylistVideosDto>(`${getApiUrl()}/playlist/video`, httpOptions);
  }

  getRecommended(): Observable<PlaylistVideosDto>{
    return this.httpClient.get<PlaylistVideosDto>(`${getApiUrl()}/playlist/recommended`, getHttpOptionsWithAuthenticationHeader());
  }

  deletePlaylist(playlistId: string): Observable<void> {
    const url = `${getApiUrl()}/playlist/details`;
    const httpOptions = {
      params: new HttpParams().set('id', playlistId),
      headers: getHttpOptionsWithAuthenticationHeader().headers
    };

    return this.httpClient.delete<void>(url, httpOptions);
  }

  deleteVideoFromPlaylist(playlistId: string, videoId: string): Observable<void> {
    const url = `${getApiUrl()}/playlist/${playlistId}/${videoId}`;

    return this.httpClient.delete<void>(url, getHttpOptionsWithAuthenticationHeader());
  }
}
