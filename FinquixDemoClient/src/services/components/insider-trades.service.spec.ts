import { TestBed } from '@angular/core/testing';

import { InsiderTradesService } from './insider-trades.service';

describe('InsiderTradesService', () => {
  let service: InsiderTradesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(InsiderTradesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
