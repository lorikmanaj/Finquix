import { Component, OnInit } from '@angular/core';
import { CryptoMarketService } from '../../../services/components/crypto-market.service';
import { NgFor, NgClass, DecimalPipe, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
    selector: 'app-crypto-market',
    imports: [
        FormsModule,
        NgFor,
        NgClass,
        DecimalPipe,
        DatePipe
    ],
    templateUrl: './crypto-market.component.html',
    styleUrl: './crypto-market.component.css'
})
export class CryptoMarketComponent implements OnInit {
  cryptoAssets: any[] = [];

  constructor(private cryptoMarketService: CryptoMarketService) { }

  ngOnInit(): void {
    this.loadCryptoMarket();
  }

  loadCryptoMarket(): void {
    this.cryptoMarketService.getLiveCryptoUpdates().subscribe({
      next: (data) => {
        this.cryptoAssets = data.map(asset => ({
          ...asset,
          previousPrice: asset.previousPrice.toFixed(2),
          currentPrice: asset.currentPrice.toFixed(2),
          marketCap: asset.marketCap.toFixed(0),
          changePercent: asset.changePercent.toFixed(2),
        }));
      },
      error: (error) => console.error('Failed to fetch crypto data', error)
    });
  }

  simulateMarket(): void {
    this.cryptoMarketService.simulateMarketUpdate().subscribe({
      next: (response) => {
        console.log('Market update simulated:', response);
        this.loadCryptoMarket();
      },
      error: (error) => console.error('Failed to simulate market update', error)
    });
  }
}
