import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { LoginDto, UserApiProxyService } from '../services/user-api-proxy.service';

@Component({
  selector: 'app-login-form',
  templateUrl: './login-form.component.html',
  styleUrls: ['./login-form.component.css']
})
export class LoginFormComponent {
  message = ""
  form = new FormGroup({
    username: new FormControl(''),
    password: new FormControl(''),
  });
  loading = false;
  submitted = false;

  constructor(private formBuilder: FormBuilder,
              private userApi: UserApiProxyService) { }

  ngOnInit() {
    this.form = this.formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
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
      login: this.f.username.value!,
      password: this.f.password.value!,
    };

    this.userApi.login(dto)
    .subscribe({ 
      error: (err: HttpErrorResponse) => {
        let detail = err.error['detail'];
        if(detail) {
          this.message = detail;
          return;
        }
        this.message = err.message;
      },
      complete: () => {
        this.message = "";
      }
    })
    .add(
      () => this.loading = false
    );

    /*invoke("login", {
      username: this.f.username.value as string,
      password: this.f.password.value as string
    })
      .then(() => invoke("hide_login_window"))
      .catch(msg => this.message = (msg as string) ?? "")
      .finally(() => this.loading = false)*/
  }


}
