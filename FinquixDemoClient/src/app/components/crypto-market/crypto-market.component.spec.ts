import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CryptoMarketComponent } from './crypto-market.component';

describe('CryptoMarketComponent', () => {
  let component: CryptoMarketComponent;
  let fixture: ComponentFixture<CryptoMarketComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CryptoMarketComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(CryptoMarketComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
