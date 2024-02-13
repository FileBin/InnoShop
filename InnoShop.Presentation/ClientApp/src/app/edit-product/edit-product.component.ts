import { Component, EventEmitter, Input, Output } from '@angular/core';

import { FormBuilder, FormControl, FormGroup, FormGroupDirective, NgForm, Validators } from '@angular/forms';
import { AvailabilityStatus, CreateProductDto, Product, ProductApiProxyService, UpdateProductDto } from '../services/product-api-proxy.service';
import { ErrorStateMatcher } from '@angular/material/core';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { ProductStateService } from '../services/product-state.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-edit-product',
  templateUrl: './edit-product.component.html',
  styleUrl: './edit-product.component.css'
})
export class EditProductComponent {
  readonly TITLE_MAX = 256;
  readonly DESC_MAX = 1024;

  matcher = new MyErrorStateMatcher();
  submitted = false;
  loading = false;
  message = "";

  isCreated$ = new BehaviorSubject<boolean>(false);

  @Output() update: EventEmitter<void> = new EventEmitter();

  form = new FormGroup({
    availability: new FormControl(AvailabilityStatus.Draft),
    title: new FormControl(''),
    desc: new FormControl(''),
    price: new FormControl(0),
  });

  constructor(
    private formBuilder: FormBuilder,
    private productApi: ProductApiProxyService,
    private product: ProductStateService,
    private router: Router,
    private activatedRoute: ActivatedRoute) {

    product.state$.subscribe(x => this.init(x));
    product.state$.subscribe(x => this.isCreated$.next(x !== null));
  }

  ngOnInit() {
    this.init(this.product.state$.value)
  }

  init(product: Product | null) {
    let status = product?.status ?? AvailabilityStatus.Draft;
    let title = product?.title ?? '';
    let desc = product?.description ?? '';
    let price = product?.price ?? 0;


    var isCreated = product !== null;

    this.form = this.formBuilder.group({
      availability: [status, isCreated ? [Validators.required] : []],
      title: [title, [Validators.required, Validators.minLength(3), Validators.maxLength(this.TITLE_MAX)]],
      desc: [desc, [Validators.maxLength(this.DESC_MAX)]],
      price: [price, [Validators.required, Validators.min(0)]]
    });
  }

  get f() { return this.form.controls; }

  onSubmit() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.form.invalid) {
      return;
    }

    this.loading = true;

    let event: Observable<void> | null;

    let prevState = this.product.state$.value;

    let dto: CreateProductDto = {
      title: this.f.title.value!,
      description: this.f.desc.value!,
      price: this.f.price.value!,
    };

    let created_id = '';

    if (!this.isCreated$.value) {
      event = this.productApi.create(dto).pipe(map(x => { created_id = x }));
    } else {
      let updateDto: UpdateProductDto = {};
      if (dto.title != prevState?.title) {
        updateDto.title = dto.title;
      }

      if (dto.description != prevState?.description) {
        updateDto.description = dto.description;
      }

      if (dto.price != prevState?.price) {
        updateDto.price = dto.price;
      }

      updateDto.status = this.f.availability.value ?? undefined;

      event = this.productApi.update(prevState!.id, dto);
    }
    event?.subscribe({
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
        if (created_id.length > 0) {
          this.router.navigate(
            [],
            {
              relativeTo: this.activatedRoute,
              queryParams: { id: created_id, action: 'edit' },
            }
          );
        }
        this.update.emit();
      }
    })
      .add(() => this.loading = false);
  }
}

export class MyErrorStateMatcher implements ErrorStateMatcher {
  isErrorState(control: FormControl | null, form: FormGroupDirective | NgForm | null): boolean {
    const isSubmitted = form && form.submitted;
    return !!(control && control.invalid && (control.dirty || control.touched || isSubmitted));
  }
}


