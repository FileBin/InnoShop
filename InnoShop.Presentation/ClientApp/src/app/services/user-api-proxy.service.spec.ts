import { TestBed } from '@angular/core/testing';

import { UserApiProxyService } from './user-api-proxy.service';

describe('UserApiProxyService', () => {
  let service: UserApiProxyService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UserApiProxyService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
