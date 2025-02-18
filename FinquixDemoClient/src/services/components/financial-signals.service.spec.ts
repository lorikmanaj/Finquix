import { TestBed } from '@angular/core/testing';

import { FinancialSignalsService } from './financial-signals.service';

describe('FinancialSignalsService', () => {
  let service: FinancialSignalsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(FinancialSignalsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
