import { Injectable } from '@angular/core';
import { environment } from "../../../environments/environment";
import { HttpClient, HttpParams } from "@angular/common/http";
import { getHttpOptionsWithAuthenticationHeader } from "../functions/get-http-options-with-authorization-header";
import { Observable } from 'rxjs';



@Injectable({
  providedIn: 'root'
})
export class DonationService {
  private readonly videoPageWebAPIUrl: string = `${environment.webApiUrl}`;

  constructor(private httpClient: HttpClient) { }

  sendDonation(id: string, amount: number): Observable<void>{
    const httpOptions = {
        params: new HttpParams().set('id', id).set('amount',amount),
        headers: getHttpOptionsWithAuthenticationHeader().headers
      };
    return this.httpClient.post<void>(`${this.videoPageWebAPIUrl}/donate/send`, null, httpOptions);
  }

  withdraw(amount: number): Observable<void>{
    const httpOptions = {
        params: new HttpParams().set('amount',amount),
        headers: getHttpOptionsWithAuthenticationHeader().headers
    };
    return this.httpClient.post<void>(`${this.videoPageWebAPIUrl}/donate/withdraw`, null, httpOptions);
  }
}
