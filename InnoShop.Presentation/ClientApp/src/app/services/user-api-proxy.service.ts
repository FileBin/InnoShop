import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { AuthStateProviderService } from './auth-state-provider.service';

@Injectable({
  providedIn: 'root'
})
export class UserApiProxyService {

  constructor(private http: HttpClient,
    @Inject('USER_API_URL') private userApiUrl: string,
    private authStateProvider: AuthStateProviderService) { }

    login(dto: LoginDto) : Observable<void> {
      return this.http.post<LoginResultDto>(`${this.userApiUrl}/login`, dto)
        .pipe(
          map(result => {
            this.authStateProvider.setToken(result.token);
          }),
        );
    }

    register(dto: RegisterDto) : Observable<void> {
      return this.http.post<void>(`${this.userApiUrl}/register`, dto);
    }

    forgotPassword(email: string) : Observable<void> {
      return this.http.post<void>(`${this.userApiUrl}/forgot_password`, email);
    }

    resetPassword(email: ResetPasswordDto) : Observable<void> {
      return this.http.post<void>(`${this.userApiUrl}/reset_password`, email);
    }

    userInfo() : Observable<UserInfoDto> {
      return this.http.get<UserInfoDto>(`${this.userApiUrl}/info`);
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
  token: string;
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

