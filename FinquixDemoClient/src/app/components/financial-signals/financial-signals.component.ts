import { Component, OnInit } from '@angular/core';
import { StockMarketService } from '../../../services/components/stock-market.service';
import { NgFor, NgClass, DatePipe } from '@angular/common';

@Component({
    selector: 'app-financial-signals',
    imports: [NgFor, NgClass, DatePipe],
    templateUrl: './financial-signals.component.html',
    styleUrl: './financial-signals.component.css'
})
export class FinancialSignalsComponent implements OnInit {
  stockAssets: any[] = [];

  constructor(private stockMarketService: StockMarketService) { }

  ngOnInit(): void {
    this.fetchStockData();
  }

  fetchStockData(): void {
    this.stockMarketService.getStockAssets().subscribe({
      next: (data) => {
        this.stockAssets = [...data.map(stock => ({
          symbol: stock.ticker,
          name: stock.companyName,
          previousPrice: parseFloat(stock.previousPrice).toFixed(2),
          currentPrice: parseFloat(stock.currentPrice).toFixed(2),
          changePercent: parseFloat(stock.changePercent).toFixed(2),
          predictedChange: parseFloat(stock.predictedChange ?? 0).toFixed(2),
          lastUpdated: stock.signalDate
        }))];
      },
      error: (error) => console.error('Error fetching stock data:', error)
    });
  }

  simulateMarketUpdate(): void {
    this.stockMarketService.simulateMarketUpdate().subscribe({
      next: () => {
        console.log('Market update simulated successfully!');
        this.fetchStockData();
      },
      error: (error) => console.error('Error updating market:', error)
    });
  }
}
