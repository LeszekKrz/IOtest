import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { userSubscriptionListDto } from '../models/user-subscription-list-dto';
import { getHttpOptionsWithAuthenticationHeader } from "../functions/get-http-options-with-authorization-header";


@Injectable({
  providedIn: 'root'
})
export class SubscriptionService {
  private readonly subscriptionsWebAPIUrl: string = `${environment.webApiUrl}/subscriptions`;

  constructor(private httpClient: HttpClient) { }

  getSubscriptions(): Observable<userSubscriptionListDto> {
    return this.httpClient.get<userSubscriptionListDto>(this.subscriptionsWebAPIUrl, getHttpOptionsWithAuthenticationHeader());
  }

  postSubscription(subId: string): Observable<void> {
    const httpOptions = {
      params: new HttpParams().set('creatorId', subId),
      headers: getHttpOptionsWithAuthenticationHeader().headers
    };

    return this.httpClient.post<void>(this.subscriptionsWebAPIUrl, null, httpOptions);
  }

  deleteSubscription(subId: string): Observable<void> {
    const httpOptions = {
      params:  new HttpParams().set('subId', subId),
      headers: getHttpOptionsWithAuthenticationHeader().headers
    };

    return this.httpClient.delete<void>(this.subscriptionsWebAPIUrl, httpOptions);
  }
}
