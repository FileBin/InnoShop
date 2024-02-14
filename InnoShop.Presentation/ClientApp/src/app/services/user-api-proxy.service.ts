import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { AuthStateProviderService } from './auth-state-provider.service';

@Injectable({
  providedIn: 'root'
})
export class UserApiProxyService {
  headers = new HttpHeaders().set('Content-Type', 'application/json; charset=utf-8');

  constructor(private http: HttpClient,
    @Inject('USER_API_URL') private userApiUrl: string,
    private authStateProvider: AuthStateProviderService) {

  }

  login(dto: LoginDto): Observable<void> {
    return this.http.post<LoginResultDto>(`${this.userApiUrl}/login`, dto)
      .pipe(
        map(result => {
          this.authStateProvider.access_token = result.accessToken;
          this.authStateProvider.refresh_token = result.refreshToken;
        }),
      );
  }

  register(dto: RegisterDto): Observable<void> {
    return this.http.post<void>(`${this.userApiUrl}/register`, dto);
  }

  forgotPassword(email: string): Observable<void> {
    return this.http.post<void>(`${this.userApiUrl}/forgot_password`, JSON.stringify(email), { headers: this.headers });
  }

  resetPassword(email: ResetPasswordDto): Observable<void> {
    return this.http.post<void>(`${this.userApiUrl}/reset_password`, email);
  }

  userInfo(): Observable<UserInfoDto> {
    return this.http.get<UserInfoDto>(`${this.userApiUrl}/info`);
  }

  resend(email: string): Observable<void> {
    return this.http.post<void>(`${this.userApiUrl}/resend`, JSON.stringify(email), { headers: this.headers });
  }

  refreshToken(): Observable<void> {
    var headers = new HttpHeaders().set('Authorization', `Bearer ${this.authStateProvider.refresh_token}`);

    return this.http.get<LoginResultDto>(`${this.userApiUrl}/refresh_token`, { headers: headers })
      .pipe(
        map(result => {
          this.authStateProvider.access_token = result.accessToken;
          this.authStateProvider.refresh_token = result.refreshToken;
        }),
      );
  }
}

export interface RegisterDto {
  email: string;
  username: string;
  password: string;
}

export interface LoginDto {
  login: string;
  password: string;
}

export interface LoginResultDto {
  accessToken: string;
  refreshToken: string;
}

export interface ResetPasswordDto {
  userId: string;
  token: string;
  newPassword: string;
}

export interface UserInfoDto {
  email: string;
  username: string;
}

