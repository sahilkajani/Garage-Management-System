export interface Job {
  id: number;
  description: string;
  condition?: string;
  miles?: number;
  critical?: string;
  registration?: string;
  make?: string;
  model?: string;
  customerName?: string;
  assignedTo?: string;
  status: string;
  createdAt: string;
}

export interface CreateJobRequest {
  description: string;
  condition?: string;
  miles?: number;
  critical?: string;
  registration?: string;
  make?: string;
  model?: string;
  customerName?: string;
  assignedTo?: string;
}

export type UpdateJobRequest = CreateJobRequest;
