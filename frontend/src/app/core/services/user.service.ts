import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, Subject } from "rxjs";
import { environment } from "../../../environments/environment";
import { UserForRegistrationDTO } from "../../authentication/models/user-for-registration-dto";
import { UserDTO } from "../models/user-dto";
import { UserForLoginDTO } from "src/app/authentication/models/user-login-dto";
import { UpdateUserDTO } from "../models/update-user-dto";
import { getHttpOptionsWithAuthenticationHeader } from "../functions/get-http-options-with-authorization-header";
import { AuthenticationResponseDTO } from "src/app/authentication/models/authentication-response-dto";

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly registrationPageWebAPIUrl: string = `${environment.webApiUrl}`;
  private authenticationStateChangeSubject = new Subject<boolean>();
  public authenticationStateChanged = this.authenticationStateChangeSubject.asObservable();

  constructor(private httpClient: HttpClient) {}

  sendAuthenticationStateChangedNotification = (isAuthenticated: boolean): void => {
    this.authenticationStateChangeSubject.next(isAuthenticated);
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('role');
    this.sendAuthenticationStateChangedNotification(false);
  }

  isUserAuthenticated(): boolean {
    const token = localStorage.getItem('token');
    return token != null;
  }

  registerUser(userForRegistration: UserForRegistrationDTO): Observable<void> {
    return this.httpClient.post<void>(`${this.registrationPageWebAPIUrl}/register`, userForRegistration);
  }

  getUser(id: string | null): Observable<UserDTO> {
    let params = new HttpParams()

    if (id !== null)
      params = params.set('id', id);

    const httpOptions = {
      params: params,
      headers: getHttpOptionsWithAuthenticationHeader().headers
    };

    return this.httpClient.get<UserDTO>(`${this.registrationPageWebAPIUrl}/user`, httpOptions);
  }

  editUser(id: string, updateUserDTO: UpdateUserDTO): Observable<void>{
    let params = new HttpParams().set('id', id);

    const httpOptions = {
      params: params,
      headers: getHttpOptionsWithAuthenticationHeader().headers
    };

    return this.httpClient.put<void>(`${this.registrationPageWebAPIUrl}/user`, updateUserDTO, httpOptions);
  }

  loginUser(userForLogin: UserForLoginDTO): Observable<AuthenticationResponseDTO> {
    return this.httpClient.post<AuthenticationResponseDTO>(`${this.registrationPageWebAPIUrl}/login`, userForLogin);
  }

  downloadFileImage(url: string): Observable<Blob> {
    return this.httpClient.get(url, {responseType: 'blob'});
  }
}
