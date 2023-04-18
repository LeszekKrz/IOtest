import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { UserService } from 'src/app/core/services/user.service';
import { environment } from 'src/environments/environment';
import { VideoMedatadataDTO } from '../models/video-metadata-dto';
import { Observable } from 'rxjs';
import { getHttpOptionsWithAuthenticationHeader } from 'src/app/core/functions/get-http-options-with-authorization-header';
import { VideoMetadataPostResponseDTO } from '../models/video-metadata-post-response-dto';

@Injectable({
  providedIn: 'root'
})
export class AddVideoService {
  private readonly addVideoPageWebAPIUrl: string = `${environment.webApiUrl}`;

  constructor(private httpClient: HttpClient) { }

  postVideoMetadata(videoMetadataDTO: VideoMedatadataDTO): Observable<VideoMetadataPostResponseDTO> {
    const postVideoMetadataWebAPIUrl: string = `${this.addVideoPageWebAPIUrl}/video-metadata`;
    return this.httpClient.post<VideoMetadataPostResponseDTO>(postVideoMetadataWebAPIUrl, videoMetadataDTO, getHttpOptionsWithAuthenticationHeader());
  }

  uploadVideo(videoId: string, videoFile: FormData): Observable<void> {
    return this.httpClient.post<void>(
      `${this.addVideoPageWebAPIUrl}/video/${videoId}`,
      videoFile,
      getHttpOptionsWithAuthenticationHeader());
  }
}
