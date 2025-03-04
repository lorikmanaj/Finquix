import { TestBed } from '@angular/core/testing';

import { CryptoMarketService } from './crypto-market.service';

describe('CryptoMarketService', () => {
  let service: CryptoMarketService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CryptoMarketService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
