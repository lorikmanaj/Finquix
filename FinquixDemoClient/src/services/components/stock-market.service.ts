import { Injectable } from '@angular/core';
import { Observable, timer, switchMap } from 'rxjs';
import { ApiService } from '../global/api.service';

@Injectable({
  providedIn: 'root'
})
export class StockMarketService {
  private readonly path = 'StockMarket';

  constructor(private apiService: ApiService) { }

  // getStockAssets(): Observable<any[]> {
  //   return timer(0, 30000).pipe(
  //     switchMap(() => this.apiService.get<any[]>(`${this.path}`))
  //   );
  // }
  getStockAssets(): Observable<any[]> {
    return this.apiService.get<any[]>(`${this.path}`);
  }

  simulateMarketUpdate(): Observable<any> {
    return this.apiService.post<any, any>(`${this.path}/simulate-update`, {});
  }
}
