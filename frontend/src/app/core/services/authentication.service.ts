import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, Subject } from "rxjs";
import { ErrorResponseDTO } from "../../core/models/error-response-dto";
import { environment } from "../../../environments/environment";
import { UserForRegistrationDTO } from "../../authentication/models/user-for-registration-dto";

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  private authenticationStateChangeSubject = new Subject<boolean>();
  public authenticationStateChanged = this.authenticationStateChangeSubject.asObservable();
  private readonly bankEmployeeRoleName = 'BankEmployee';
  private readonly roleUrl = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

  private readonly registrationPageWebAPIUrl: string = `${environment.webApiUrl}/registration-page`;

  sendAuthenticationStateChangedNotification = (isAuthenticated: boolean): void => {
    this.authenticationStateChangeSubject.next(isAuthenticated);
  }

  constructor(private httpClient: HttpClient) {}

  registerUser(userForRegistration: UserForRegistrationDTO): Observable<ErrorResponseDTO[]> {
    return this.httpClient.post<ErrorResponseDTO[]>(`${this.registrationPageWebAPIUrl}/register`, userForRegistration);
  }
}
