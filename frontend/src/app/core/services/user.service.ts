import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, Subject } from "rxjs";
import { ErrorResponseDTO } from "../../core/models/error-response-dto";
import { environment } from "../../../environments/environment";
import { UserForRegistrationDTO } from "../../authentication/models/user-for-registration-dto";

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private authenticationStateChangeSubject = new Subject<boolean>();
  public authenticationStateChanged = this.authenticationStateChangeSubject.asObservable();

  private readonly registrationPageWebAPIUrl: string = `${environment.webApiUrl}`;

  constructor(private httpClient: HttpClient) {}

  registerUser(userForRegistration: UserForRegistrationDTO): Observable<void> {
    return this.httpClient.post<void>(`${this.registrationPageWebAPIUrl}/register`, userForRegistration);
  }
}
