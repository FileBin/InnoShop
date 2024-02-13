import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class ProductApiProxyService {
  constructor(private http: HttpClient,
    @Inject('PRODUCT_API_URL') private productApiUrl: string) { }


  serialize = function (obj: any) {
    var str = [];
    for (var p in obj)
      if (obj.hasOwnProperty(p)) {
        str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
      }
    return str.join("&");
  }

  create(dto: CreateProductDto): Observable<string> {
    return this.http.post(`${this.productApiUrl}/create`, dto, { responseType: 'text' });
  }

  update(id: string, dto: UpdateProductDto): Observable<void> {
    return this.http.put<void>(`${this.productApiUrl}/${id}`, dto);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.productApiUrl}/${id}`);
  }

  get(id: any): Observable<Product> {
    return this.http.get<Product>(`${this.productApiUrl}/${id}`);
  }

  search(dto: SearchDto): Observable<SearchResultDto> {
    return this.http.get<SearchResultDto>(`${this.productApiUrl}/search?${this.serialize(dto)}`);
  }
}

export interface CreateProductDto {
  title: string,
  description: string,
  price: number,
}

export enum AvailabilityStatus {
  Draft = "Draft",
  Published = "Published",
  Sold = "Sold",
}

export interface UpdateProductDto {
  title?: string,
  description?: string,
  price?: number,
  status?: AvailabilityStatus;
}

export interface Product extends CreateProductDto {
  id: string;
  userId: string;
  status: AvailabilityStatus;
  creationTimestamp: string;
  lastUpdateTimestamp: string;
}

export interface SearchFilters {
  contains?: string;
  price_from?: number;
  price_to?: number;

  type?: SortingType;
  order?: SortingOrder;
}

export interface SearchDto extends SearchFilters {
  from: number;
  to: number;
}

export interface SearchResultDto {
  products: Product[],
  queryCount: number,
}

export enum SortingType {
  Ascending = 'Ascending',
  Descending = 'Descending',
}

export enum SortingOrder {
  AtoZ = 'AtoZ',
  ByDate = 'ByDate',
  ByPrice = 'ByPrice',
}
