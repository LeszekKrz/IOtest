import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { userSubscriptionListDto } from '../models/user-subscription-list-dto';
import { getHttpOptionsWithAuthenticationHeader } from "../functions/get-http-options-with-authorization-header";


@Injectable({
  providedIn: 'root'
})
export class SubscriptionService {
  private readonly videoPageWebAPIUrl: string = `${environment.webApiUrl}`;

  constructor(private httpClient: HttpClient) { }

  getSubscriptions(): Observable<userSubscriptionListDto> {
    return this.httpClient.get<userSubscriptionListDto>(`${this.videoPageWebAPIUrl}/subscriptions`, getHttpOptionsWithAuthenticationHeader());
  }
}
