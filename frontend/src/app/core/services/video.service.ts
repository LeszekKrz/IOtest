import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { VideoListDto } from "../models/video-list-dto";
import { VideoMetadataDto } from "../models/video-metadata-dto";
import { getHttpOptionsWithAuthenticationHeader } from "../functions/get-http-options-with-authorization-header";
import { getApiUrl } from "../functions/get-api-url";

@Injectable({
  providedIn: 'root'
})
export class VideoService {
  constructor(private httpClient: HttpClient) {}

  getUserVideos(id: string): Observable<VideoListDto> {
    let params = new HttpParams().set('id', id);
    const httpOptions = {
      params: params,
      headers: getHttpOptionsWithAuthenticationHeader().headers
    };

    return this.httpClient.get<VideoListDto>(`${getApiUrl()}/user/videos`, httpOptions);
  }

  getVideoMetadata(id: string): Observable<VideoMetadataDto> {
    let params = new HttpParams().set('id', id);
    const httpOptions = {
      params: params,
      headers: getHttpOptionsWithAuthenticationHeader().headers
    };

    return this.httpClient.get<VideoMetadataDto>(`${getApiUrl()}/video-metadata`, httpOptions);
  }

  deleteVideo(id: string): Observable<void> {
    let params = new HttpParams().set('id', id);
    const httpOptions = {
      params: params,
      headers: getHttpOptionsWithAuthenticationHeader().headers
    };

    return this.httpClient.delete<void>(`${getApiUrl()}/video`, httpOptions);
  }
}
