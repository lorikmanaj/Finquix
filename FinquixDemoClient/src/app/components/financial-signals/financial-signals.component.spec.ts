import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FinancialSignalsComponent } from './financial-signals.component';

describe('FinancialSignalsComponent', () => {
  let component: FinancialSignalsComponent;
  let fixture: ComponentFixture<FinancialSignalsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FinancialSignalsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(FinancialSignalsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
