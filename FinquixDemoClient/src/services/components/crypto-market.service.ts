import { Injectable } from '@angular/core';
import { Observable, timer, switchMap } from 'rxjs';
import { ApiService } from '../global/api.service';

@Injectable({
  providedIn: 'root'
})
export class CryptoMarketService {
  private readonly path = 'CryptoMarket';

  constructor(private apiService: ApiService) { }

  getCryptoAssets(): Observable<any[]> {
    return this.apiService.get<any[]>(`${this.path}`);
  }

  simulateMarketUpdate(): Observable<any[]> {
    return this.apiService.post<any[], null>(`${this.path}/simulate-update`, null);
  }

  getLiveCryptoUpdates(): Observable<any[]> {
    return timer(0, 30000).pipe(
      switchMap(() => this.getCryptoAssets())
    );
  }
}
