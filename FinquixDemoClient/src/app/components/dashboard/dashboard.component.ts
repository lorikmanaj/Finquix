import { Component, OnInit } from '@angular/core';
import { UserProfileService } from '../../../services/components/user-profile.service';
import { ActivatedRoute } from '@angular/router';
import { CommonModule, DecimalPipe, DatePipe, NgFor, NgIf, NgStyle } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    NgIf,
    NgFor,
    NgStyle,
    DecimalPipe,
    DatePipe,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
  providers: [DecimalPipe, DatePipe]
})
export class DashboardComponent implements OnInit {
  userProfile: any = null;
  savingsRate: string = '0%';
  activeGoals: any[] = [];
  inactiveGoals: any[] = [];

  constructor(
    private userProfileService: UserProfileService,
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const userId = params['userId'] || 1;
      this.loadUserProfile(userId);
    });
  }

  loadUserProfile(userId: number): void {
    this.userProfileService.getUserProfileById(userId).subscribe({
      next: (data) => {
        this.userProfile = data;
        this.processFinancialGoals();
        this.calculateSavingsRate();
      },
      error: (err) => console.error('Failed to load user profile:', err)
    });
  }

  processFinancialGoals(): void {
    if (!this.userProfile?.financialGoals) return;

    this.activeGoals = this.userProfile.financialGoals
      .filter((goal: any) => goal.isActive)
      .map(this.calculateGoalProgress);

    this.inactiveGoals = this.userProfile.financialGoals
      .filter((goal: any) => !goal.isActive)
      .map(this.calculateGoalProgress);
  }

  calculateGoalProgress(goal: any): any {
    const currentDate = new Date();
    const targetDate = new Date(goal.targetDate);
    const startDate = goal.startDate ? new Date(goal.startDate) : currentDate;

    const totalMonths = (targetDate.getFullYear() - startDate.getFullYear()) * 12
      + (targetDate.getMonth() - startDate.getMonth());

    const elapsedMonths = (currentDate.getFullYear() - startDate.getFullYear()) * 12
      + (currentDate.getMonth() - startDate.getMonth());

    const progressPercentage = Math.min(
      100,
      Math.max(0, (elapsedMonths / totalMonths) * 100)
    );

    const savedAmount = (goal.monthlyContribution || 0) * elapsedMonths;
    const remainingAmount = Math.max(0, (goal.estimatedValue || 0) - savedAmount);

    return {
      ...goal,
      progressPercentage,
      savedAmount,
      remainingAmount,
      isOnTrack: savedAmount >= ((goal.estimatedValue || 0) * (progressPercentage / 100))
    };
  }

  calculateSavingsRate(): void {
    const income = this.userProfile.financialData?.income || 0;
    const savings = this.userProfile.financialData?.savings || 0;
    this.savingsRate = income > 0 ? ((savings / income) * 100).toFixed(2) + '%' : '0%';
  }

  getGoalDisplayName(goal: any): string {
    return goal.goalType === 'custom' ? goal.customGoal :
      this.predefinedGoalLabels[goal.goalType] || goal.goalType;
  }

  predefinedGoalLabels: Record<string, string> = {
    'DebtRepayment': 'Pay Off Debt',
    'HousePurchase': 'Buy a House',
    'Retirement': 'Retirement',
    'SaveForEducation': 'Save for Education'
  };
}