import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, Subject } from "rxjs";
import { environment } from "../../../environments/environment";
import { VideoListDto } from "../models/video-list-dto";

@Injectable({
  providedIn: 'root'
})
export class VideoService {
  private readonly videoPageWebAPIUrl: string = `${environment.webApiUrl}`;

  constructor(private httpClient: HttpClient) {}

  getUserVideos(id: string): Observable<VideoListDto> {
    let params = new HttpParams().set('id', id);
    
    return this.httpClient.get<VideoListDto>(`${this.videoPageWebAPIUrl}/user/videos`, { params: params });
  }
}
