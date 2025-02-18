import { NgFor } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { InsiderTradesService } from '../../../services/components/insider-trades.service';

@Component({
  selector: 'app-insider-trades',
  standalone: true,
  imports: [FormsModule, NgFor],
  templateUrl: './insider-trades.component.html',
  styleUrl: './insider-trades.component.css'
})
export class InsiderTradesComponent {
  trades: any[] = [];
  symbol: string = 'AAPL'; // Default to AAPL

  constructor(private insiderTradesService: InsiderTradesService) { }

  ngOnInit(): void {
    this.loadTrades();
  }

  loadTrades(): void {
    if (!this.symbol) return;

    this.insiderTradesService.getInsiderTrades(this.symbol).subscribe(
      (data) => {
        this.trades = data;
      },
      (error) => {
        console.error('Error fetching insider trades', error);
      }
    );
  }
}