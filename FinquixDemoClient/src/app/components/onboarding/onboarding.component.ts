import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSelectModule } from '@angular/material/select';
import { ActivatedRoute, Router } from '@angular/router';
import { UserProfileService } from '../../../services/components/user-profile.service';

@Component({
  selector: 'app-onboarding',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    FormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCheckboxModule,
    MatSelectModule
  ],
  templateUrl: './onboarding.component.html',
  styleUrls: ['./onboarding.component.css']
})
export class OnboardingComponent implements OnInit {
  onboardingForm: FormGroup;
  userId: number | null = null;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private userProfileService: UserProfileService
  ) {
    this.onboardingForm = this.fb.group({
      name: [''],
      age: [''],
      employmentStatus: [''],
      dependents: [''],
      financialGoals: this.fb.group({
        debtRepayment: [false],
        housePurchase: [false],
        retirement: [false],
        saveForEducation: [false]
      }),
      financialData: this.fb.group({
        income: [''],
        fixedExpenses: [''],
        variableExpenses: [''],
        savingsRate: ['']
      }),
      riskTolerance: ['Medium']
    });
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.userId = params['userId'] || null;
      if (this.userId) {
        this.userProfileService.getUserProfileById(this.userId).subscribe(
          (data) => {
            this.onboardingForm.patchValue(data);
          },
          (error) => console.error('Failed to load user profile:', error)
        );
      }
    });
  }

  submit() {
    const formData = this.onboardingForm.value;

    // Transform financialGoals object into an array of goals
    formData.financialGoals = Object.entries(formData.financialGoals).map(([goalType, isActive]) => ({
      goalType,
      isActive
    }));

    console.log('Formatted Submission Data:', formData);

    this.userProfileService.submitOnboarding(formData).subscribe({
      next: (response) => {
        console.log('Successfully submitted:', response);
        this.router.navigate(['/dashboard'], { queryParams: { userId: response.id } });
      },
      error: (error) => {
        console.error('Submission failed:', error);
      }
    });
  }
}