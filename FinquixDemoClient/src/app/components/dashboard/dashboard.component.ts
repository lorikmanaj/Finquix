import { Component, OnInit } from '@angular/core';
import { UserProfileService } from '../../../services/components/user-profile.service';
import { NgFor, NgIf } from '@angular/common';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [NgIf, NgFor],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  userProfile: any = null;

  constructor(private userProfileService: UserProfileService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const userId = params['userId'] || 1; // Default to 1 if not provided
      this.userProfileService.getUserProfileById(userId).subscribe({
        next: (data) => {
          this.userProfile = data;
        },
        error: (err) => console.error('Failed to load user profile:', err)
      });
    });
  }
}
