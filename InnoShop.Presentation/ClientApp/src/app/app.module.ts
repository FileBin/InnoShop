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
import { EditProductComponent } from './edit-product/edit-product.component';

import { MatSliderModule } from '@angular/material/slider';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatSidenavModule } from '@angular/material/sidenav';
import { ProductPageComponent } from './product-page/product-page.component';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import {MatPaginatorModule} from '@angular/material/paginator'; 

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
    EditProductComponent,
    ProductPageComponent,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,

    MatCardModule,
    MatButtonModule,
    MatSliderModule,
    MatSelectModule,
    MatFormFieldModule,
    MatInputModule,
    MatSidenavModule,
    MatPaginatorModule,

    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'counter', component: CounterComponent },
      { path: 'login', component: LoginFormComponent },
      { path: 'register', component: RegisterFormComponent },
      { path: 'forgot_password', component: ForgotPasswordComponent },
      { path: 'passwordreset', component: ResetPasswordComponent },
      { path: 'resend', component: ResendEmailComponent },
      { path: 'product', component: ProductPageComponent },
    ])
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptor,
      multi: true,
    },
    { provide: APP_ID, useValue: 'serverApp' }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
