import { HttpErrorResponse } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { UserApiProxyService } from '../services/user-api-proxy.service';

@Component({
  selector: 'app-resend-email',
  templateUrl: './resend-email.component.html',
  styleUrls: ['./resend-email.component.css']
})
export class ResendEmailComponent {
  message = ""
  done = false

  form = new FormGroup({
    email: new FormControl(''),
  });
  loading = false;
  submitted = false;

  constructor(private formBuilder: FormBuilder,
              private userApi: UserApiProxyService) { }

  ngOnInit() {
    this.form = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
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


    this.userApi.resend(this.f.email.value!)
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
        this.done = true;
      }
    })
    .add(
      () => this.loading = false
    );
  }
}
