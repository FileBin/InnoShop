import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { RegisterDto, UserApiProxyService } from '../services/user-api-proxy.service';
import { HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register-form',
  templateUrl: './register-form.component.html',
  styleUrls: ['./register-form.component.css']
})
export class RegisterFormComponent {
  message = ""

  registered = false;
  
  form = new FormGroup({
    email: new FormControl(''),
    username: new FormControl(''),
    password: new FormControl(''),
  });
  loading = false;
  submitted = false;

  constructor(private formBuilder: FormBuilder,
              private userApi: UserApiProxyService,
              private router: Router) { }

  ngOnInit() {
    this.form = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      username: ['', [Validators.required, Validators.minLength(3)]],
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

    let dto: RegisterDto = {
      email: this.f.email.value!,
      username: this.f.username.value!,
      password: this.f.password.value!,
    };

    this.userApi.register(dto)
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
        this.registered = true;
      }
    })
    .add(
      () => this.loading = false
    );
  }
}
