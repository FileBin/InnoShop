import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Product } from './product-api-proxy.service';

@Injectable({
  providedIn: 'root'
})
export class ProductStateService {
  public state$= new BehaviorSubject<Product | null>(null);
  constructor() { }
}
