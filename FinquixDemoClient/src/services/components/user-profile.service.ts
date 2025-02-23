import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../global/api.service';

@Injectable({
  providedIn: 'root'
})
export class UserProfileService {
  private readonly path = 'UserProfile'; // Fixed route to match controller

  constructor(private apiService: ApiService) { }

  submitOnboarding(data: any): Observable<any> {
    return this.apiService.post<any, any>(`${this.path}/submit-onboarding`, data);
  }

  getUserProfiles(): Observable<any[]> {
    return this.apiService.get<any[]>(`${this.path}`);
  }

  getUserProfileById(id: number): Observable<any> {
    return this.apiService.get<any>(`${this.path}/${id}`);
  }
}