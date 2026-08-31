import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { JobService } from '../../services/job.service';

@Component({
  selector: 'app-create-job',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './create-job.component.html',
  styleUrl: './create-job.component.scss',
})
export class CreateJobComponent {
  private readonly fb = inject(FormBuilder);
  private readonly jobService = inject(JobService);
  private readonly router = inject(Router);

  submitting = false;
  errorMessage = '';
  successMessage = '';

  readonly serviceAdvisors = [
    'Emma Richardson',
    "James O'Connor",
    'Priya Sharma',
    'Michael Torres',
    'Sophie Williams',
    'David Chen',
    'Rachel Murphy',
    'Tom Anderson',
  ];

  readonly form = this.fb.group({
    description: ['', [Validators.required, Validators.maxLength(2000)]],
    registration: ['', Validators.maxLength(20)],
    make: ['', Validators.maxLength(100)],
    model: ['', Validators.maxLength(100)],
    customerName: ['', Validators.maxLength(200)],
    assignedTo: [''],
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    const value = this.form.getRawValue();

    this.jobService
      .createJob({
        description: value.description ?? '',
        registration: value.registration || undefined,
        make: value.make || undefined,
        model: value.model || undefined,
        customerName: value.customerName || undefined,
        assignedTo: value.assignedTo || undefined,
      })
      .subscribe({
        next: () => {
          this.successMessage = 'Job created successfully.';
          this.form.reset();
          this.submitting = false;
          setTimeout(() => this.router.navigate(['/dashboard']), 1200);
        },
        error: () => {
          this.errorMessage = 'Failed to create job. Please check the API is running.';
          this.submitting = false;
        },
      });
  }
}
