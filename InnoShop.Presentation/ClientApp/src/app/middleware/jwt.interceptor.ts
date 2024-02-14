import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthStateProviderService } from '../services/auth-state-provider.service';

import { jwtDecode } from 'jwt-decode';
import { UserApiProxyService } from '../services/user-api-proxy.service';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private authStateProvider: AuthStateProviderService,
    private userApi: UserApiProxyService) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    let access_token = this.authStateProvider.access_token;

    if (access_token && !request.headers.has('Authorization')) {
      try {
        let exp = jwtDecode(access_token).exp;
        if (exp) {
          if (exp * 1000 < (new Date().getTime() + 300_000)) {
            this.userApi.refreshToken()
              .subscribe({ error: err => { console.log(`refreshing failed\n err was ${err}`); } });
          }
        }

      } catch (err: any) {
        console.log(`parsing token failed\n err was ${err}`);
      }
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${access_token}`
        }
      });
    }
    return next.handle(request);
  }
}
