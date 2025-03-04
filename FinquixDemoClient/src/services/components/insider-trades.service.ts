import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../global/api.service';

@Injectable({
  providedIn: 'root'
})
export class InsiderTradesService {
  private readonly path = 'InsiderTrades';

  constructor(private apiService: ApiService) { }

  getInsiderTrades(symbol: string): Observable<any[]> {
    return this.apiService.get<any[]>(`${this.path}/${symbol}`);
  }
}
