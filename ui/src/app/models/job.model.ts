export interface Job {
  id: number;
  description: string;
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
  registration?: string;
  make?: string;
  model?: string;
  customerName?: string;
  assignedTo?: string;
}
