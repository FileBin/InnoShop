import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Product, ProductApiProxyService } from '../services/product-api-proxy.service';
import { AuthStateProviderService } from '../services/auth-state-provider.service';
import { BehaviorSubject, Subject } from 'rxjs';
import { ProductStateService } from '../services/product-state.service';

@Component({
  selector: 'app-product-page',
  templateUrl: './product-page.component.html',
  styleUrl: './product-page.component.css'
})
export class ProductPageComponent {
  id = ''
  isLoaded$ = new BehaviorSubject<boolean>(false);

  isView = true;

  constructor(private route: ActivatedRoute,
    private authStateProvider: AuthStateProviderService,
    private productApi: ProductApiProxyService,
    private product: ProductStateService) {
    this.route.queryParams.subscribe(params => {
      this.isView = true;
      var action = params['action'];
      if (action == 'create') {
        this.isView = false;
        this.product.state$.next(null);
        this.isLoaded$.next(true);
        return;
      }

      var id = params['id'];
      if (action == 'edit') {
        this.isView = false;
      }
      if (id) {
        this.id = id;
        this.update();
      }
    });
  }

  update() {
    this.productApi.get(this.id).subscribe({
      next:
        p => {
          this.product.state$.next(p);
        },
      complete: () => {
        this.isLoaded$.next(true);
      }
    });
  }

  isLoggedIn() {
    return this.authStateProvider.isAuthorized();
  }
}
