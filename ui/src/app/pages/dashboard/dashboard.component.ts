import { Component, computed, inject, OnInit, signal } from '@angular/core';

import { Job } from '../../models/job.model';
import { JobService } from '../../services/job.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent implements OnInit {
  private readonly jobService = inject(JobService);

  protected readonly jobs = signal<Job[]>([]);
  protected readonly loading = signal(true);
  protected readonly error = signal<string | null>(null);

  protected readonly unscheduledCount = computed(
    () => this.jobs().filter((job) => this.normalizeStatus(job.status) === 'Unscheduled').length
  );

  protected readonly scheduledCount = computed(
    () => this.jobs().filter((job) => this.normalizeStatus(job.status) === 'Scheduled').length
  );

  protected readonly completedCount = computed(
    () => this.jobs().filter((job) => this.normalizeStatus(job.status) === 'Completed').length
  );

  ngOnInit(): void {
    this.jobService.getJobs().subscribe({
      next: (jobs) => {
        this.jobs.set(jobs);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Unable to load dashboard metrics. Please check the API is running.');
        this.loading.set(false);
      },
    });
  }

  private normalizeStatus(status: string): string {
    if (status.toLowerCase() === 'unassigned') {
      return 'Unscheduled';
    }

    return status;
  }
}
