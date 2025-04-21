import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class MarketDataStoreService {
  stockAssets: any[] = [];
  cryptoAssets: any[] = [];

  constructor() { }
}
