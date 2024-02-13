import { BrowserModule } from '@angular/platform-browser';
import { APP_ID, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { LoginFormComponent } from './login-form/login-form.component';
import { JwtInterceptor } from './middleware/jwt.interceptor';
import { RegisterFormComponent } from './register-form/register-form.component';
import { UserInfoComponent } from './user-info/user-info.component';
import { ForgotPasswordComponent } from './forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './reset-password/reset-password.component';
import { ResendEmailComponent } from './resend-email/resend-email.component';
import { SearchComponent } from './search/search.component';

import {MatSliderModule} from '@angular/material/slider'; 

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    LoginFormComponent,
    RegisterFormComponent,
    UserInfoComponent,
    ForgotPasswordComponent,
    ResetPasswordComponent,
    ResendEmailComponent,
    SearchComponent,
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    MatSliderModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'counter', component: CounterComponent },
      { path: 'login', component: LoginFormComponent },
      { path: 'register', component: RegisterFormComponent },
      { path: 'forgot_password', component: ForgotPasswordComponent },
      { path: 'passwordreset', component: ResetPasswordComponent },
      { path: 'resend', component: ResendEmailComponent },
    ])
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptor,
      multi: true,
    },
    { provide: APP_ID,  useValue: 'serverApp' }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
