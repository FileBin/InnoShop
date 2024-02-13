import { Component, Inject, OnInit, Renderer2 } from '@angular/core';
import { GlobalStateService } from '../services/global-state.service';
import { FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Product, SearchFilters, SortingOrder, SortingType } from '../services/product-api-proxy.service';
import { SearchStateService } from '../services/search-state.service';
import { PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css'],
})
export class SearchComponent implements OnInit {
  length = 0;
  pageIndex = 0;
  pageSize = 25;

  expandSidebar = false;
  numbers: number[] = []

  items: Product[] = []

  pageSizeOptions = [10, 25, 50];

  sortOrder = new FormControl('recent');
  minPrice = new FormControl(0);
  maxPrice = new FormControl(100);

  constructor(private state: GlobalStateService, private searchState: SearchStateService) {
    this.numbers = Array(24).fill(0).map((_, i) => i + 1);

    this.sortOrder.valueChanges.subscribe(x => this.updateStatus(x));
    this.minPrice.valueChanges.subscribe(x => this.updateMinPrice(x));
    this.maxPrice.valueChanges.subscribe(x => this.updateMaxPrice(x));


    state.filterExpand$.subscribe({
      next: x => this.expandSidebar = x
    })

    searchState.items_size$.subscribe(x => this.length = x);
    searchState.page$.subscribe(x => this.pageIndex = x);
    searchState.page_size$.subscribe(x => this.pageSize = x);
    searchState.items$.subscribe(x => this.items = x);
  }

  ngOnInit(): void {
    this.searchState.search();
  }

  handlePageEvent(e: PageEvent) {
    this.searchState.page_size$.next(e.pageSize);
    this.searchState.page$.next(e.pageIndex);
    this.searchState.search();
  }

  updateStatus(str: string | null) {
    if (str === null) return;
    let type = SortingType.Descending;
    let order = SortingOrder.ByDate;

    switch (str) {
      case 'oldest':
        type = SortingType.Ascending;
        order = SortingOrder.ByDate;
        break;
      case 'AtoZ':
        type = SortingType.Ascending;
        order = SortingOrder.AtoZ;
        break;

      case 'ZtoA':
        type = SortingType.Descending;
        order = SortingOrder.AtoZ;
        break;

      case 'price-up':
        type = SortingType.Ascending;
        order = SortingOrder.ByPrice;
        break;

      case 'price-down':
        type = SortingType.Descending;
        order = SortingOrder.ByPrice;
        break;

      default:
      case 'recent':
        break;
    }

    var filters = this.searchState.filters$.value;

    filters.type = type;
    filters.order = order;

    this.searchState.filters$.next(filters);
  }

  updateMinPrice(price: number | null) {
    if (price === null) return;
    var filters = this.searchState.filters$.value;

    filters.price_from = price;

    this.searchState.filters$.next(filters);
  }

  updateMaxPrice(price: number | null) {
    if (price === null) return;
    var filters = this.searchState.filters$.value;

    filters.price_to = price;

    this.searchState.filters$.next(filters);
  }
}
