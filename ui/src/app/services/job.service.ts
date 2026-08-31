import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { CreateJobRequest, Job } from '../models/job.model';

@Injectable({ providedIn: 'root' })
export class JobService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/jobs`;

  getJobs(): Observable<Job[]> {
    return this.http.get<Job[]>(this.baseUrl);
  }

  createJob(request: CreateJobRequest): Observable<Job> {
    return this.http.post<Job>(this.baseUrl, request);
  }
}
