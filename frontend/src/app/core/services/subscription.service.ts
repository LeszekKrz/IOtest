import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, Subject } from "rxjs";
import { environment } from "../../../environments/environment";
import { SubbscriptionListDto } from "../models/subscription-list-dto";

@Injectable({
  providedIn: 'root'
})
export class SubscriptionService {
  private readonly videoPageWebAPIUrl: string = `${environment.webApiUrl}`;

  constructor(private httpClient: HttpClient) {}

  getUserVideos(id: string): Observable<SubbscriptionListDto> {
    let params = new HttpParams().set('id', id);
    
    return this.httpClient.get<SubbscriptionListDto>(`${this.videoPageWebAPIUrl}/subscriptions`, { params: params });
  }
}