import { TestBed } from '@angular/core/testing';

import { ProductApiProxyService } from './product-api-proxy.service';

describe('ProductApiProxyService', () => {
  let service: ProductApiProxyService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ProductApiProxyService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
