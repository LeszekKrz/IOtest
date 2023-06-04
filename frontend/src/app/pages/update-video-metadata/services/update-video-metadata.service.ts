import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { VideoMetadataUpdateDTO } from '../../add-video/models/video-metadata-dto';
import { getHttpOptionsWithAuthenticationHeader } from 'src/app/core/functions/get-http-options-with-authorization-header';
import { getApiUrl } from 'src/app/core/functions/get-api-url';

@Injectable({
  providedIn: 'root'
})
export class UpdateVideoMetadataService {
  constructor(private httpClient: HttpClient) { }

  downloadFileImage(url: string): Observable<Blob> {
    return this.httpClient.get(url, {responseType: 'blob'});
  }

  updateVideoMetadata(videoMetadataDTO: VideoMetadataUpdateDTO, videoId: string): Observable<void> {
    const putVideoMetadataWebAPIUrl: string = `${getApiUrl()}/video-metadata}`;
    const httpOptions = {
      params: new HttpParams().set('id', videoId),
      headers: getHttpOptionsWithAuthenticationHeader().headers,
    };

    return this.httpClient.put<void>(putVideoMetadataWebAPIUrl, videoMetadataDTO, httpOptions);
  }
}
