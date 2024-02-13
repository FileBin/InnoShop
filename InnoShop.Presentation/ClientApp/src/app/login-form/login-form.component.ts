import { HttpErrorResponse } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { LoginDto, UserApiProxyService } from '../services/user-api-proxy.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login-form',
  templateUrl: './login-form.component.html',
  styleUrls: ['./login-form.component.css']
})
export class LoginFormComponent {
  message = ""
  form = new FormGroup({
    login: new FormControl(''),
    password: new FormControl(''),
  });
  loading = false;
  submitted = false;

  constructor(private formBuilder: FormBuilder,
              private userApi: UserApiProxyService,
              private router: Router) { }

  ngOnInit() {
    this.form = this.formBuilder.group({
      login: ['', [Validators.required, Validators.minLength(3)]],
      password: ['', [Validators.required, Validators.minLength(8)]],
    });
  }

  // convenience getter for easy access to form fields
  get f() { return this.form.controls; }

  onSubmit() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.form.invalid) {
      return;
    }

    this.loading = true;

    let dto: LoginDto = {
      login: this.f.login.value!,
      password: this.f.password.value!,
    };

    this.userApi.login(dto)
    .subscribe({ 
      error: (err: HttpErrorResponse) => {
        let detail = err.error?.detail;
        if(detail) {
          this.message = detail;
          return;
        }
        this.message = err.statusText;
      },
      complete: () => {
        this.message = "";
        this.router.navigate(['/']);
      }
    })
    .add(
      () => this.loading = false
    );
  }


}
