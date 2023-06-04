import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { userSubscriptionListDto } from '../models/user-subscription-list-dto';
import { getHttpOptionsWithAuthenticationHeader } from "../functions/get-http-options-with-authorization-header";
import { getApiUrl } from '../functions/get-api-url';


@Injectable({
  providedIn: 'root'
})
export class SubscriptionService {
  constructor(private httpClient: HttpClient) { }

  getSubscriptions(): Observable<userSubscriptionListDto> {
    return this.httpClient.get<userSubscriptionListDto>(`${getApiUrl()}/subscriptions`, getHttpOptionsWithAuthenticationHeader());
  }

  postSubscription(subId: string): Observable<void> {
    const httpOptions = {
      params: new HttpParams().set('creatorId', subId),
      headers: getHttpOptionsWithAuthenticationHeader().headers
    };

    return this.httpClient.post<void>(`${getApiUrl()}/subscriptions`, null, httpOptions);
  }

  deleteSubscription(subId: string): Observable<void> {
    const httpOptions = {
      params:  new HttpParams().set('subId', subId),
      headers: getHttpOptionsWithAuthenticationHeader().headers
    };

    return this.httpClient.delete<void>(`${getApiUrl()}/subscriptions`, httpOptions);
  }
}
