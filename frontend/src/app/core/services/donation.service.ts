import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from "@angular/common/http";
import { getHttpOptionsWithAuthenticationHeader } from "../functions/get-http-options-with-authorization-header";
import { Observable } from 'rxjs';
import { getApiUrl } from '../functions/get-api-url';



@Injectable({
  providedIn: 'root'
})
export class DonationService {
  constructor(private httpClient: HttpClient) { }

  sendDonation(id: string, amount: number): Observable<void>{
    const httpOptions = {
        params: new HttpParams().set('id', id).set('amount',amount),
        headers: getHttpOptionsWithAuthenticationHeader().headers
      };
    return this.httpClient.post<void>(`${getApiUrl()}/donate/send`, null, httpOptions);
  }

  withdraw(amount: number): Observable<void>{
    const httpOptions = {
        params: new HttpParams().set('amount',amount),
        headers: getHttpOptionsWithAuthenticationHeader().headers
    };
    return this.httpClient.post<void>(`${getApiUrl()}/donate/withdraw`, null, httpOptions);
  }
}
