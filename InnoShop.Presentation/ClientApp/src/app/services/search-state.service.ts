import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Product, ProductApiProxyService, SearchDto, SearchFilters } from './product-api-proxy.service';

@Injectable({
  providedIn: 'root'
})
export class SearchStateService {
  public page$ = new BehaviorSubject<number>(0);
  public items$ = new BehaviorSubject<Product[]>([]);
  public page_size$ = new BehaviorSubject<number>(20);
  public items_size$ = new BehaviorSubject<number>(1);

  public filters$ = new BehaviorSubject<SearchFilters>({});

  public search() {
    let from = this.page$.value * this.page_size$.value;
    let to = this.page_size$.value + from - 1;

    var filters = this.filters$.value;

    if(filters.contains?.length == 0) {
      delete filters.contains;
    }

    var dto : SearchDto = filters as SearchDto;

    dto.from = from;
    dto.to = to;

    this.productApi.search(dto)
    .subscribe(result => {
      this.items$.next(result.products);
      this.items_size$.next(result.queryCount);
    });
  }

  constructor(private productApi: ProductApiProxyService) { }
}
