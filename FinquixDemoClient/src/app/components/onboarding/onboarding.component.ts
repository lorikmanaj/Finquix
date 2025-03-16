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
import { NgFor } from '@angular/common';

@Component({
  selector: 'app-onboarding',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    FormsModule,
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
        monthlyBudget: [{ value: '', disabled: true }], // Computed, read-only
        totalNetWorth: [{ value: '', disabled: true }] // Computed, read-only
      }),
      riskTolerance: ['Medium']
    });

    // this.onboardingForm = this.fb.group({
    //   name: ['', Validators.required],
    //   age: ['', [Validators.required, Validators.min(18)]],
    //   employmentStatus: ['', Validators.required],
    //   dependents: ['', [Validators.required, Validators.min(0)]],
    //   investmentBehavior: ['Moderate', Validators.required],
    //   riskTolerance: ['Medium', Validators.required],

    //   financialGoals: this.fb.array([
    //     this.createFinancialGoal('DebtRepayment'),
    //     this.createFinancialGoal('HousePurchase'),
    //     this.createFinancialGoal('Retirement'),
    //     this.createFinancialGoal('EducationSavings')
    //   ]),

    //   financialData: this.fb.group({
    //     income: ['', [Validators.required, Validators.min(0)]],
    //     fixedExpenses: ['', [Validators.required, Validators.min(0)]],
    //     variableExpenses: ['', [Validators.required, Validators.min(0)]],
    //     savings: ['', [Validators.required, Validators.min(0)]],
    //     investments: ['', [Validators.required, Validators.min(0)]],
    //     debt: ['', [Validators.required, Validators.min(0)]],
    //     emergencyFund: ['', [Validators.required, Validators.min(0)]],
    //     monthlyBudget: [{ value: '', disabled: true }], // Computed, read-only
    //     totalNetWorth: [{ value: '', disabled: true }] // Computed, read-only
    //   })
    // });
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

  createFinancialGoal(goalType: string) {
    return this.fb.group({
      goalType: [goalType],
      isActive: [false],
      estimatedValue: ['', [Validators.required, Validators.min(1000)]],
      currentProgress: ['', [Validators.required, Validators.min(0)]],
      monthlyContribution: ['', [Validators.required, Validators.min(0)]],
      startDate: ['', Validators.required],
      targetDate: ['', Validators.required]
    });
  }

  addFinancialGoal() {
    const goalForm = this.fb.group({
      goalType: [''],
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