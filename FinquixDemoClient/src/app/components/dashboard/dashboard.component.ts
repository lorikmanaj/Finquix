import { Component, OnInit } from '@angular/core';
import { UserProfileService } from '../../../services/components/user-profile.service';
import { ActivatedRoute } from '@angular/router';
import { CommonModule, DecimalPipe, DatePipe, NgFor, NgIf } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { OnboardingService } from '../../../services/components/onboarding.service';
import { GoalDialogComponent } from '../goal-dialog/goal-dialog.component';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    NgIf,
    NgFor,
    DecimalPipe,
    DatePipe,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule,
    MatDialogModule,
    MatSnackBarModule
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
  userId: number = 1;

  predefinedGoalLabels: Record<string, string> = {
    'DebtRepayment': 'Pay Off Debt',
    'HousePurchase': 'Buy a House',
    'Retirement': 'Retirement',
    'SaveForEducation': 'Save for Education'
  };

  constructor(
    private userProfileService: UserProfileService,
    private onboardingService: OnboardingService,
    private route: ActivatedRoute,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.userId = params['userId'] || 1;
      this.loadUserProfile(this.userId);
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

  openAddGoalDialog(): void {
    const dialogRef = this.dialog.open(GoalDialogComponent, {
      width: '500px',
      data: {
        predefinedGoals: Object.entries(this.predefinedGoalLabels).map(([value, label]) => ({ value, label })),
        isEdit: false
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.addGoal(result);
      }
    });
  }

  openEditGoalDialog(goal: any): void {
    const dialogRef = this.dialog.open(GoalDialogComponent, {
      width: '500px',
      data: {
        goal: { ...goal },
        predefinedGoals: Object.entries(this.predefinedGoalLabels).map(([value, label]) => ({ value, label })),
        isEdit: true
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.updateGoal(goal, result);
      }
    });
  }

  addGoal(newGoal: any): void {
    this.onboardingService.addFinancialGoal(this.userId, newGoal).subscribe({
      next: () => {
        this.snackBar.open('Goal added successfully!', 'Close', { duration: 3000 });
        this.loadUserProfile(this.userId);
      },
      error: (err) => {
        console.error('Error adding goal:', err);
        this.snackBar.open('Failed to add goal', 'Close', { duration: 3000 });
      }
    });
  }

  updateGoal(oldGoal: any, updatedGoal: any): void {
    const goals = this.userProfile.financialGoals.map((g: any) =>
      g === oldGoal ? updatedGoal : g
    );

    this.onboardingService.updateFinancialGoals(this.userId, goals).subscribe({
      next: () => {
        this.snackBar.open('Goal updated successfully!', 'Close', { duration: 3000 });
        this.loadUserProfile(this.userId);
      },
      error: (err) => {
        console.error('Error updating goal:', err);
        this.snackBar.open('Failed to update goal', 'Close', { duration: 3000 });
      }
    });
  }

  toggleGoalStatus(goal: any): void {
    const updatedGoal = { ...goal, isActive: !goal.isActive };
    this.updateGoal(goal, updatedGoal);
  }

  deleteGoal(goal: any): void {
    if (confirm('Are you sure you want to delete this goal?')) {
      const goals = this.userProfile.financialGoals.filter((g: any) => g !== goal);

      this.onboardingService.updateFinancialGoals(this.userId, goals).subscribe({
        next: () => {
          this.snackBar.open('Goal deleted successfully!', 'Close', { duration: 3000 });
          this.loadUserProfile(this.userId);
        },
        error: (err) => {
          console.error('Error deleting goal:', err);
          this.snackBar.open('Failed to delete goal', 'Close', { duration: 3000 });
        }
      });
    }
  }
}