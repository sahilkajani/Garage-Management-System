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
  scheduledDate?: string;
  completedDate?: string;
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

export interface UpdateJobRequest extends CreateJobRequest {
  status: string;
  scheduledDate?: string;
  completedDate?: string;
}
