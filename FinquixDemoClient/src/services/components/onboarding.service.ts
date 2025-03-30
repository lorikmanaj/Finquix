import { Injectable } from '@angular/core';
import { ApiService } from '../global/api.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class OnboardingService {
  private readonly path = 'UserProfile';

  constructor(private apiService: ApiService) { }

  updateFinancialGoals(userId: number, goals: any[]): Observable<any> {
    return this.apiService.put<any, any>(`${this.path}/${userId}/goals`, { financialGoals: goals });
  }

  addFinancialGoal(userId: number, goal: any): Observable<any> {
    return this.apiService.post<any, any>(`${this.path}/${userId}/goals`, goal);
  }
}
