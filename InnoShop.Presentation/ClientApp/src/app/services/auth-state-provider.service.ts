import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthStateProviderService {

  constructor() { }

  setToken(token: string) {
    localStorage.setItem('access_token', token);
  }


  removeToken() {
    localStorage.removeItem('access_token');
  }

  isAuthorized(): boolean {
    if(localStorage.getItem('access_token')) {
      return true;
    }
    return false;
  }
}
