import { Injectable } from '@angular/core';
import { Observable, switchMap, timer } from 'rxjs';
import { ApiService } from '../global/api.service';

@Injectable({
  providedIn: 'root'
})
export class FinancialSignalsService {
  private readonly path = 'FinancialSignals'; // Matches the backend route

  constructor(private apiService: ApiService) { }

  // getFinancialSignals(): Observable<any[]> {
  //   return this.apiService.get<any[]>(this.path);
  // }

  getFinancialSignals(): Observable<any[]> {
    return timer(0, 30000).pipe(
      switchMap(() => this.apiService.get<any[]>('FinancialSignals'))
    );
  }
}
