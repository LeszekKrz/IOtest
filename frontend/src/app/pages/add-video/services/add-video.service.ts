import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { VideoMetadataUpdateDTO } from '../models/video-metadata-dto';
import { Observable } from 'rxjs';
import { getHttpOptionsWithAuthenticationHeader } from 'src/app/core/functions/get-http-options-with-authorization-header';
import { VideoMetadataPostResponseDTO } from '../models/video-metadata-post-response-dto';
import { getApiUrl } from 'src/app/core/functions/get-api-url';

@Injectable({
  providedIn: 'root'
})
export class AddVideoService {
  constructor(private httpClient: HttpClient) { }

  postVideoMetadata(videoMetadataDTO: VideoMetadataUpdateDTO): Observable<VideoMetadataPostResponseDTO> {
    const postVideoMetadataWebAPIUrl: string = `${getApiUrl()}/video-metadata`;
    return this.httpClient.post<VideoMetadataPostResponseDTO>(postVideoMetadataWebAPIUrl, videoMetadataDTO, getHttpOptionsWithAuthenticationHeader());
  }

  uploadVideo(videoId: string, videoFile: FormData): Observable<void> {
    return this.httpClient.post<void>(
      `${getApiUrl()}/video/${videoId}`,
      videoFile,
      getHttpOptionsWithAuthenticationHeader());
  }
}
