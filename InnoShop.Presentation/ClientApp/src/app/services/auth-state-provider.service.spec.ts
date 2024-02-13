import { TestBed } from '@angular/core/testing';

import { AuthStateProviderService } from './auth-state-provider.service';

describe('AuthStateProviderService', () => {
  let service: AuthStateProviderService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AuthStateProviderService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
