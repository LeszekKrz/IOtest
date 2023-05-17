import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { getToken } from '../functions/get-token';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private router: Router) { }

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {

    const isLoggedIn = getToken() !== '';

    // redirect to login page if not logged in and trying to access a restricted page
    const isLoginPage = state.url === '/login';
    const isRegisterPage = state.url === '/register';

    if (isLoggedIn || isLoginPage || isRegisterPage) {
      return true;
    }

    // not logged in so redirect to login page
    this.router.navigate(['login']);
    return false;
  }
}
