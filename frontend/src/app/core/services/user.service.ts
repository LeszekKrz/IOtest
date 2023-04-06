import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, Subject } from "rxjs";
import { environment } from "../../../environments/environment";
import { UserForRegistrationDTO } from "../../authentication/models/user-for-registration-dto";
import { UserDTO } from "../models/user-dto";

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly registrationPageWebAPIUrl: string = `${environment.webApiUrl}`;

  constructor(private httpClient: HttpClient) {}

  registerUser(userForRegistration: UserForRegistrationDTO): Observable<void> {
    return this.httpClient.post<void>(`${this.registrationPageWebAPIUrl}/register`, userForRegistration);
  }

  getUser(id: string): Observable<UserDTO> {
    let params = new HttpParams().set('id', id);
    
    return this.httpClient.get<UserDTO>(`${this.registrationPageWebAPIUrl}/user`, { params: params });
  }
}
