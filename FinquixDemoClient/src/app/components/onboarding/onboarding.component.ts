import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, FormsModule, Validators, FormArray } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSelectModule } from '@angular/material/select';
import { ActivatedRoute, Router } from '@angular/router';
import { UserProfileService } from '../../../services/components/user-profile.service';
import { NgFor, NgIf } from '@angular/common';

@Component({
  selector: 'app-onboarding',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    FormsModule,
    NgIf,
    NgFor,
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

  predefinedGoals = [
    { value: 'DebtRepayment', label: 'Pay Off Debt' },
    { value: 'HousePurchase', label: 'Buy a House' },
    { value: 'Retirement', label: 'Retirement' },
    { value: 'SaveForEducation', label: 'Save for Education' }
  ];

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
      investmentBehavior: [''],
      financialGoals: this.fb.array([]),
      financialData: this.fb.group({
        income: [''],
        fixedExpenses: [''],
        variableExpenses: [''],
        savings: [''],
        debt: [''],
        emergencyFund: [''],
        monthlyBudget: [{ value: '', disabled: true }],
        totalNetWorth: [{ value: '', disabled: true }]
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
            this.updateComputedFields();
          },
          (error) => console.error('Failed to load user profile:', error)
        );
      }
    });

    this.onboardingForm.get('financialData')?.valueChanges.subscribe(() => {
      this.updateComputedFields();
    });
  }

  updateComputedFields() {
    const financialData = this.onboardingForm.get('financialData')?.value;
    const income = Number(financialData.income) || 0;
    const fixedExpenses = Number(financialData.fixedExpenses) || 0;
    const variableExpenses = Number(financialData.variableExpenses) || 0;
    const savings = Number(financialData.savings) || 0;
    const investments = Number(financialData.investments) || 0;
    const debt = Number(financialData.debt) || 0;

    const monthlyBudget = income - (fixedExpenses + variableExpenses);
    const totalNetWorth = savings + investments - debt;

    this.onboardingForm.get('financialData.monthlyBudget')?.setValue(monthlyBudget, { emitEvent: false });
    this.onboardingForm.get('financialData.totalNetWorth')?.setValue(totalNetWorth, { emitEvent: false });
  }

  addFinancialGoal() {
    const goalForm = this.fb.group({
      goalType: [''],
      customGoal: [''],
      estimatedValue: [''],
      monthlyContribution: [''],
      targetDate: [''],
      isActive: [false]
    });

    this.financialGoals.push(goalForm);
  }

  get financialGoals() {
    return this.onboardingForm.get('financialGoals') as FormArray;
  }

  submit() {
    const formData = this.onboardingForm.getRawValue();
    formData.financialGoals = formData.financialGoals.filter((g: { goalType: string; isActive: boolean }) => g.isActive);

    this.userProfileService.submitOnboarding(formData).subscribe({
      next: (response) => {
        console.log('Successfully submitted:', response);
        this.router.navigate(['/dashboard'], { queryParams: { userId: response.id } });
      },
      error: (error) => console.error('Submission failed:', error)
    });
  }
}