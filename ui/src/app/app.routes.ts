import { Routes } from '@angular/router';

import { MainLayoutComponent } from './layout/main-layout/main-layout.component';
import { CreateJobComponent } from './pages/create-job/create-job.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { JobDetailComponent } from './pages/job-detail/job-detail.component';
import { JobStatusComponent } from './pages/job-status/job-status.component';

export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: DashboardComponent },
      { path: 'jobs/create', component: CreateJobComponent },
      { path: 'jobs/status', component: JobStatusComponent },
      { path: 'jobs/:id', component: JobDetailComponent },
    ],
  },
  { path: '**', redirectTo: 'dashboard' },
];
