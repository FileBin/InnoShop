import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthStateProviderService {

  constructor() { }

  get access_token() {
    return localStorage.getItem('access_token');
  }

  set access_token(token: string | null) {
    if (token === null) {
      this.removeTokens();
      return;
    }

    localStorage.setItem('access_token', token);
  }

  get refresh_token() {
    return localStorage.getItem('refresh_token');
  }

  set refresh_token(token: string | null) {
    if (token === null) {
      this.removeTokens();
      return;
    }

    localStorage.setItem('refresh_token', token);
  }

  removeTokens() {
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
  }

  isAuthorized(): boolean {
    if (this.refresh_token !== null) {
      return true;
    }
    return false;
  }
}
