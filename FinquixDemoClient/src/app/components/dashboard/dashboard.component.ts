import { Component, OnInit } from '@angular/core';
import { UserProfileService } from '../../../services/components/user-profile.service';
import { ActivatedRoute } from '@angular/router';
import { CommonModule, DecimalPipe, DatePipe, NgFor, NgIf, NgStyle } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, NgIf, NgFor, NgStyle, DecimalPipe, DatePipe],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
  providers: [DecimalPipe, DatePipe]
})
export class DashboardComponent implements OnInit {
  userProfile: any = null;
  savingsRate: string = '0%';

  constructor(private userProfileService: UserProfileService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const userId = params['userId'] || 1; // Default to 1 if not provided
      this.userProfileService.getUserProfileById(userId).subscribe({
        next: (data) => {
          this.userProfile = {
            ...data,
            financialData: {
              ...data.financialData
            }
          };

          const income = this.userProfile.financialData?.income || 0;
          const savings = this.userProfile.financialData?.savings || 0;
          this.savingsRate = income > 0 ? ((savings / income) * 100).toFixed(2) + '%' : '0%';
        },
        error: (err) => console.error('Failed to load user profile:', err)
      });
    });
  }
}
