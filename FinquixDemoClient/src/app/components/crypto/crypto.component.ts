import { Component } from '@angular/core';
import { CryptoService } from '../../../services/components/crypto.service';
import { NgIf, NgFor, CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-crypto',
  standalone: true,
  imports: [NgIf, NgFor, FormsModule, CommonModule],
  templateUrl: './crypto.component.html',
  styleUrl: './crypto.component.css'
})
export class CryptoComponent {
  cryptoQuotes: any[] = [];
  selectedCrypto: string = 'BTCUSD';
  cryptoHistory: any = null;

  constructor(private cryptoService: CryptoService) { }

  ngOnInit(): void {
    this.loadCryptoQuotes();
  }

  loadCryptoQuotes(): void {
    this.cryptoService.getCryptoQuotes().subscribe(
      (data) => {
        this.cryptoQuotes = data;
      },
      (error) => {
        console.error('Error fetching crypto quotes', error);
      }
    );
  }

  loadCryptoHistory(): void {
    this.cryptoService.getCryptoHistory(this.selectedCrypto).subscribe(
      (data) => {
        this.cryptoHistory = data;
      },
      (error) => {
        console.error(`Error fetching history for ${this.selectedCrypto}`, error);
      }
    );
  }
}
