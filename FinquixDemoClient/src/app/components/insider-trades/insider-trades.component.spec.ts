import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InsiderTradesComponent } from './insider-trades.component';

describe('InsiderTradesComponent', () => {
  let component: InsiderTradesComponent;
  let fixture: ComponentFixture<InsiderTradesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InsiderTradesComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(InsiderTradesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
