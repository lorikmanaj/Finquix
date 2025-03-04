import { Component, OnInit } from '@angular/core';
import { CryptoMarketService } from '../../../services/components/crypto-market.service';
import { NgFor, NgClass, DecimalPipe, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-crypto-market',
  standalone: true,
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
    // ✅ Subscribe to live updates every 30 seconds
    this.cryptoMarketService.getLiveCryptoUpdates().subscribe(
      (data) => {
        this.cryptoAssets = data;
      },
      (error) => {
        console.error('Failed to fetch crypto data', error);
      }
    );
  }

  // ✅ Manual simulation trigger
  simulateMarket(): void {
    this.cryptoMarketService.simulateMarketUpdate().subscribe(
      (updatedData) => {
        this.cryptoAssets = updatedData;
        console.log('Market update simulated successfully!');
      },
      (error) => console.error('Failed to simulate market update', error)
    );
  }
}
