import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { VideoMedatadataDTO } from '../../add-video/models/video-metadata-dto';
import { environment } from 'src/environments/environment';
import { getHttpOptionsWithAuthenticationHeader } from 'src/app/core/functions/get-http-options-with-authorization-header';

@Injectable({
  providedIn: 'root'
})
export class UpdateVideoMetadataService {
  private readonly updateVideoMetadataPageWebAPIUrl: string = `${environment.webApiUrl}/video-metadata`;

  constructor(private httpClient: HttpClient) { }

  downloadFileImage(url: string): Observable<Blob> {
    return this.httpClient.get(url, {responseType: 'blob'});
  }

  updateVideoMetadata(videoMetadataDTO: VideoMedatadataDTO, videoId: string): Observable<void> {
    const putVideoMetadataWebAPIUrl: string = this.updateVideoMetadataPageWebAPIUrl;
    const httpOptions = {
      params: new HttpParams().set('id', videoId),
      headers: getHttpOptionsWithAuthenticationHeader().headers,
    };

    return this.httpClient.put<void>(putVideoMetadataWebAPIUrl, videoMetadataDTO, httpOptions);
  }
}
