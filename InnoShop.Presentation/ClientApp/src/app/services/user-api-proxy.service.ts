import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable, catchError, map, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserApiProxyService {

  constructor(private http: HttpClient,
    @Inject('USER_API_URL') private userApiUrl: string) { }

  login(dto: LoginDto) : Observable<void> {
    return this.http.post<LoginResultDto>(`${this.userApiUrl}/login`, dto)
      .pipe(
        map(result => {
          localStorage.setItem('access_token', JSON.stringify(result.token))
        }),
      );
  }
}

export interface LoginDto {
  login: string;
  password: string;
}

export interface LoginResultDto {
  token: string;
}
