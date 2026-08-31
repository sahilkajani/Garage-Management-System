import { DatePipe } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { Job } from '../../models/job.model';
import { JobService } from '../../services/job.service';

@Component({
  selector: 'app-job-status',
  imports: [DatePipe, RouterLink],
  templateUrl: './job-status.component.html',
  styleUrl: './job-status.component.scss',
})
export class JobStatusComponent implements OnInit {
  private readonly jobService = inject(JobService);

  protected readonly jobs = signal<Job[]>([]);
  protected readonly loading = signal(true);
  protected readonly error = signal<string | null>(null);

  ngOnInit(): void {
    this.loadJobs();
  }

  protected vehicleLabel(job: Job): string {
    const parts = [job.registration, job.make, job.model].filter(Boolean);
    return parts.length > 0 ? parts.join(' · ') : '—';
  }

  protected statusClass(status: string): string {
    return status.toLowerCase().replace(/\s+/g, '-');
  }

  private loadJobs(): void {
    this.loading.set(true);
    this.error.set(null);

    this.jobService.getJobs().subscribe({
      next: (jobs) => {
        this.jobs.set(jobs);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Unable to load jobs. Please check the API is running.');
        this.loading.set(false);
      },
    });
  }
}
