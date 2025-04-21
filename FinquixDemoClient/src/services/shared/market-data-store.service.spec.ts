import { TestBed } from '@angular/core/testing';

import { MarketDataStoreService } from './market-data-store.service';

describe('MarketDataStoreService', () => {
  let service: MarketDataStoreService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MarketDataStoreService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
