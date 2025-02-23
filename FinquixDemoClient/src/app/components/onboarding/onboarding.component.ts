import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSelectModule } from '@angular/material/select';
import { Router } from '@angular/router';

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
export class OnboardingComponent {
  onboardingForm: FormGroup;

  constructor(private fb: FormBuilder, private router: Router) {
    this.onboardingForm = this.fb.group({
      name: [''],
      age: [''],
      employmentStatus: [''],
      dependents: [''],
      income: [''],
      fixedExpenses: [''],
      variableExpenses: [''],
      savingsRate: [''],
      riskTolerance: ['Medium']
    });
  }

  submit() {
    console.log('User Data:', this.onboardingForm.value);
    this.router.navigate(['/dashboard']);
  }
}
