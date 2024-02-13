import { Component } from '@angular/core';
import { ProductStateService } from '../services/product-state.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-view-product',
  templateUrl: './view-product.component.html',
  styleUrl: './view-product.component.css'
})
export class ViewProductComponent {

  constructor(private product: ProductStateService,
    private router: Router,
    private activatedRoute: ActivatedRoute) {
    product.state$.subscribe(x => {
      this.title = x?.title ?? '';
      this.desc = x?.description ?? '';
      this.price = x?.price ?? 0;
      this.isEditable = x?.isEditable ?? false;
    });
  }


  navigateEdit() {
    this.router.navigate(
      [],
      {
        relativeTo: this.activatedRoute,
        queryParams: { action: 'edit' },
        queryParamsHandling: 'merge',
      }
    );
  }

  title = '';
  desc = '';
  price = 5.00;
  isEditable = true;

}
