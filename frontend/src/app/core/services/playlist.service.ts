import { Injectable } from '@angular/core';
import { environment } from "../../../environments/environment";
import { HttpClient, HttpParams } from "@angular/common/http";
import { PostPlaylistDto } from '../models/post-playlist-dto';
import { getHttpOptionsWithAuthenticationHeader } from "../functions/get-http-options-with-authorization-header";
import { UserPlaylistsDto } from '../models/user-playlists-dto';
import { Observable } from 'rxjs';
import { PlaylistVideosDto } from '../models/playlist-videos-dto';



@Injectable({
  providedIn: 'root'
})
export class PlaylistService {
  private readonly videoPageWebAPIUrl: string = `${environment.webApiUrl}`;

  constructor(private httpClient: HttpClient) { }

  postPlalist(postPlaylist: PostPlaylistDto): Observable<void>{
    return this.httpClient.post<void>(`${this.videoPageWebAPIUrl}/playlist/details`, postPlaylist, getHttpOptionsWithAuthenticationHeader());
  }

  addToPlaylist(playlistId: string, videoId: string): Observable<void>
  {
    return this.httpClient.post<void>(`${this.videoPageWebAPIUrl}/playlist/${playlistId}/${videoId}`,getHttpOptionsWithAuthenticationHeader());
  }
  
  updatePlalist(id: string, updatePlaylist: PostPlaylistDto): Observable<void>{
    const httpOptions = {
      params: new HttpParams().set('id', id),
      headers: getHttpOptionsWithAuthenticationHeader().headers
    };

    return this.httpClient.put<void>(`${this.videoPageWebAPIUrl}/playlist/details`, updatePlaylist, httpOptions);
  }

  getOwnPlaylists(): Observable<UserPlaylistsDto[]>{
    return this.httpClient.get<UserPlaylistsDto[]>(`${this.videoPageWebAPIUrl}/playlist/user`, getHttpOptionsWithAuthenticationHeader());
  }

  getPlaylistVideos(playlistId: string): Observable<PlaylistVideosDto>{
    const httpOptions = {
      params: new HttpParams().set('id', playlistId),
      headers: getHttpOptionsWithAuthenticationHeader().headers
    };

    return this.httpClient.get<PlaylistVideosDto>(`${this.videoPageWebAPIUrl}/playlist/video`, httpOptions);
  }

  getRecommended(): Observable<PlaylistVideosDto>{
    return this.httpClient.get<PlaylistVideosDto>(`${this.videoPageWebAPIUrl}/playlist/recommended`, getHttpOptionsWithAuthenticationHeader());
  }
}
