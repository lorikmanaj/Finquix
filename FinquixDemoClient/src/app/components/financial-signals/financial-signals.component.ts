import { Component, OnInit } from '@angular/core';
import { FinancialSignalsService } from '../../../services/components/financial-signals.service';
import { FormsModule } from '@angular/forms';
import { NgFor, NgClass, DecimalPipe, DatePipe } from '@angular/common';
import { timer, switchMap } from 'rxjs';
import { StockMarketService } from '../../../services/components/stock-market.service';

@Component({
  selector: 'app-financial-signals',
  standalone: true,
  imports: [
    FormsModule,
    NgFor,
    NgClass, // ✅ Import ngClass
    DecimalPipe, // ✅ Import DecimalPipe for number formatting
    DatePipe
  ],
  templateUrl: './financial-signals.component.html',
  styleUrl: './financial-signals.component.css'
})
export class FinancialSignalsComponent {
  stockAssets: any[] = [];

  constructor(private stockMarketService: StockMarketService) { }

  ngOnInit(): void {
    this.stockMarketService.getStockAssets().subscribe(
      (data) => this.stockAssets = data,
      (error) => console.error('Error fetching stock data:', error)
    );
  }

  simulateMarketUpdate(): void {
    this.stockMarketService.simulateMarketUpdate().subscribe(
      (data) => this.stockAssets = data,
      (error) => console.error('Error updating market:', error)
    );
  }
}