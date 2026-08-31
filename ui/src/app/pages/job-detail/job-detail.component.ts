import { DatePipe } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { Job } from '../../models/job.model';
import { JobService } from '../../services/job.service';

@Component({
  selector: 'app-job-detail',
  imports: [ReactiveFormsModule, RouterLink, DatePipe],
  templateUrl: './job-detail.component.html',
  styleUrl: './job-detail.component.scss',
})
export class JobDetailComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly jobService = inject(JobService);
  private readonly route = inject(ActivatedRoute);

  protected readonly job = signal<Job | null>(null);
  protected readonly loading = signal(true);
  protected readonly loadError = signal('');
  protected readonly saving = signal(false);
  protected readonly saveError = signal('');
  protected readonly saveSuccess = signal('');

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

  readonly criticalLevels = ['High', 'Medium', 'Low'];
  readonly statusOptions = ['Unscheduled', 'Scheduled', 'Completed'];

  readonly form = this.fb.group({
    description: ['', [Validators.required, Validators.maxLength(2000)]],
    condition: ['', Validators.maxLength(1000)],
    miles: [null as number | null, [Validators.min(0), Validators.max(9999999)]],
    critical: [''],
    status: ['Unscheduled', Validators.required],
    scheduledDate: [''],
    completedDate: [''],
    registration: ['', Validators.maxLength(20)],
    make: ['', Validators.maxLength(100)],
    model: ['', Validators.maxLength(100)],
    customerName: ['', Validators.maxLength(200)],
    assignedTo: [''],
  });

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!id) {
      this.loadError.set('Invalid job ID.');
      this.loading.set(false);
      return;
    }

    this.jobService.getJob(id).subscribe({
      next: (job) => {
        this.job.set(job);
        this.form.patchValue({
          description: job.description,
          condition: job.condition ?? '',
          miles: job.miles ?? null,
          critical: job.critical ?? '',
          status: this.normalizeStatus(job.status),
          scheduledDate: this.toDateTimeLocalValue(job.scheduledDate),
          completedDate: this.toDateTimeLocalValue(job.completedDate),
          registration: job.registration ?? '',
          make: job.make ?? '',
          model: job.model ?? '',
          customerName: job.customerName ?? '',
          assignedTo: job.assignedTo ?? '',
        });
        this.loading.set(false);
      },
      error: () => {
        this.loadError.set('Unable to load this job. It may have been removed.');
        this.loading.set(false);
      },
    });
  }

  protected submit(): void {
    const currentJob = this.job();
    if (!currentJob || this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    this.saveError.set('');
    this.saveSuccess.set('');

    const value = this.form.getRawValue();

    this.jobService
      .updateJob(currentJob.id, {
        description: value.description ?? '',
        condition: value.condition || undefined,
        miles: value.miles ?? undefined,
        critical: value.critical || undefined,
        status: value.status ?? 'Unscheduled',
        scheduledDate: this.fromDateTimeLocalValue(value.scheduledDate ?? ''),
        completedDate: this.fromDateTimeLocalValue(value.completedDate ?? ''),
        registration: value.registration || undefined,
        make: value.make || undefined,
        model: value.model || undefined,
        customerName: value.customerName || undefined,
        assignedTo: value.assignedTo || undefined,
      })
      .subscribe({
        next: (job) => {
          this.job.set(job);
          this.saveSuccess.set('Job updated successfully.');
          this.saving.set(false);
        },
        error: () => {
          this.saveError.set('Failed to save changes. Please check the API is running.');
          this.saving.set(false);
        },
      });
  }

  private normalizeStatus(status: string): string {
    const match = this.statusOptions.find(
      (option) => option.toLowerCase() === status.toLowerCase()
    );
    return match ?? 'Unscheduled';
  }

  private toDateTimeLocalValue(iso?: string): string {
    if (!iso) {
      return '';
    }

    const date = new Date(iso);
    const pad = (value: number) => value.toString().padStart(2, '0');
    return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
  }

  private fromDateTimeLocalValue(value: string): string | undefined {
    if (!value) {
      return undefined;
    }

    return new Date(value).toISOString();
  }
}
