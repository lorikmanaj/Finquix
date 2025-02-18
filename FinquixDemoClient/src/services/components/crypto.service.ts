import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../global/api.service';

@Injectable({
  providedIn: 'root'
})
export class CryptoService {
  private readonly path = 'Crypto';

  constructor(private apiService: ApiService) { }

  getCryptoQuotes(): Observable<any[]> {
    return this.apiService.get<any[]>(`${this.path}/quotes`);
  }

  getCryptoHistory(symbol: string): Observable<any> {
    return this.apiService.get<any>(`${this.path}/history/${symbol}`);
  }
}
