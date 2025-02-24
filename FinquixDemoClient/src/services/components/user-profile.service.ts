import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../global/api.service';

@Injectable({
  providedIn: 'root'
})
export class UserProfileService {
  private readonly path = 'UserProfile';

  constructor(private apiService: ApiService) { }

  // Submit onboarding data
  submitOnboarding(data: any): Observable<any> {
    return this.apiService.post<any, any>(`${this.path}/submit-onboarding`, data);
  }

  // Get all user profiles
  getUserProfiles(): Observable<any[]> {
    return this.apiService.get<any[]>(`${this.path}`);
  }  

  // Get a single user profile by ID
  getUserProfileById(id: number): Observable<any> {
    return this.apiService.get<any>(`${this.path}/${id}`);
  }

  // Create a new user profile
  createUserProfile(data: any): Observable<any> {
    return this.apiService.post<any, any>(`${this.path}`, data);
  }
}
