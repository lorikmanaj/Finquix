import { CommonModule } from '@angular/common';
import { Component, Inject, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectChange, MatSelectModule } from '@angular/material/select';

@Component({
    selector: 'app-goal-dialog',
    imports: [
        CommonModule,
        ReactiveFormsModule,
        MatFormFieldModule,
        MatInputModule,
        MatSelectModule,
        MatCheckboxModule,
        MatButtonModule,
        MatDialogModule
    ],
    templateUrl: './goal-dialog.component.html',
    styleUrl: './goal-dialog.component.css'
})
export class GoalDialogComponent implements OnInit {
  goalForm: FormGroup;
  showCustomInput = false;

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<GoalDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.goalForm = this.fb.group({
      goalType: ['', Validators.required],
      customGoal: [''],
      estimatedValue: ['', [Validators.required, Validators.min(0)]],
      monthlyContribution: ['', [Validators.required, Validators.min(0)]],
      targetDate: ['', Validators.required],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    if (this.data.isEdit && this.data.goal) {
      this.goalForm.patchValue(this.data.goal);
      this.showCustomInput = this.data.goal.goalType === 'custom';
    }
  }

  onGoalTypeChange(event: MatSelectChange): void {
    this.showCustomInput = event.value === 'custom';
    if (!this.showCustomInput) {
      this.goalForm.get('customGoal')?.setValue('');
    }
  }

  save(): void {
    if (this.goalForm.valid) {
      const formValue = this.goalForm.value;
      const goalData = {
        ...formValue,
        // If custom goal, use that as the display name
        ...(formValue.goalType === 'custom' && formValue.customGoal
          ? { goalType: 'custom', customGoal: formValue.customGoal }
          : {})
      };
      this.dialogRef.close(goalData);
    }
  }
}