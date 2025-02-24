import { Component, OnInit } from '@angular/core';
import { FinancialSignalsService } from '../../../services/components/financial-signals.service';
import { FormsModule } from '@angular/forms';
import { NgFor, NgClass, DecimalPipe } from '@angular/common';
import { timer, switchMap } from 'rxjs';

@Component({
  selector: 'app-financial-signals',
  standalone: true,
  imports: [
    FormsModule,
    NgFor,
    NgClass, // ✅ Import ngClass
    DecimalPipe // ✅ Import DecimalPipe for number formatting
  ],
  templateUrl: './financial-signals.component.html',
  styleUrl: './financial-signals.component.css'
})
export class FinancialSignalsComponent implements OnInit {
  signals: any[] = [];

  constructor(private financialSignalsService: FinancialSignalsService) { }

  ngOnInit(): void {
    // ✅ Poll every 30 seconds for real-time updates
    timer(0, 30000).pipe(
      switchMap(() => this.financialSignalsService.getFinancialSignals())
    ).subscribe(
      (data) => {
        this.signals = data;
      },
      (error) => {
        console.error('Error fetching financial signals', error);
      }
    );
  }
}