import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ResetPasswordDto, UserApiProxyService } from '../services/user-api-proxy.service';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent {
  message = ""
  token = ""
  userId = ""

  valid = false
  form = new FormGroup({
    password: new FormControl(''),
  });
  loading = false;
  submitted = false;

  constructor(private formBuilder: FormBuilder,
    private userApi: UserApiProxyService,
    private router: Router,
    private route: ActivatedRoute) {
    this.route.queryParams.subscribe(params => {
      this.userId = params['userId'];
      this.token = params['token'];

      if(this.token.length > 0 && this.userId.length > 0) {
        this.valid= true;
      }
    });
  }

  ngOnInit() {
    this.form = this.formBuilder.group({
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

    var dto: ResetPasswordDto = {
      newPassword: this.f.password.value!,
      token: this.token,
      userId: this.userId,
    };

    this.userApi.resetPassword(dto)
      .subscribe({
        error: (err: HttpErrorResponse) => {
          let detail = err.error?.detail;
          if (detail) {
            this.message = detail;
            return;
          }
          this.message = err.statusText;
        },
        complete: () => {
          this.message = "";
          this.router.navigate(['/login']);
        }
      })
      .add(
        () => this.loading = false
      );
  }
}
