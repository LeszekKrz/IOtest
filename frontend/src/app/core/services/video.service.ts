import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, Subject } from "rxjs";
import { environment } from "../../../environments/environment";
import { VideoListDto } from "../models/video-list-dto";
import { VideoMetadataDto } from "../models/video-metadata-dto";
import { getHttpOptionsWithAuthenticationHeader } from "../functions/get-http-options-with-authorization-header";

@Injectable({
  providedIn: 'root'
})
export class VideoService {
  private readonly videoPageWebAPIUrl: string = `${environment.webApiUrl}`;

  constructor(private httpClient: HttpClient) {}

  getUserVideos(id: string): Observable<VideoListDto> {
    let params = new HttpParams().set('id', id);
    const httpOptions = {
      params: params,
      headers: getHttpOptionsWithAuthenticationHeader().headers
    };

    return this.httpClient.get<VideoListDto>(`${this.videoPageWebAPIUrl}/user/videos`, httpOptions);
  }

  getVideoMetadata(id: string): Observable<VideoMetadataDto> {
    let params = new HttpParams().set('id', id);
    const httpOptions = {
      params: params,
      headers: getHttpOptionsWithAuthenticationHeader().headers
    };

    return this.httpClient.get<VideoMetadataDto>(`${this.videoPageWebAPIUrl}/video-metadata`, httpOptions);
  }
}
