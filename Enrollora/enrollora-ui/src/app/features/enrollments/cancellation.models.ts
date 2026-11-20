// CancellationStatus mirrors backend enum: Pending=0, Approved=1, Rejected=2
export enum CancellationStatus {
  Pending = 0,
  Approved = 1,
  Rejected = 2,
}

export interface CancellationResponse {
  cancellationRequestId: number;
  enrollmentId: number;
  studentId: number;
  studentName: string;
  studentEmail: string;
  courseName: string;
  reason: string;
  status: CancellationStatus;
  requestedAt: string;
  reviewedAt: string | null;
  adminRemarks: string | null;
}

export interface CreateCancellationRequest {
  reason: string;
}

export interface ReviewCancellationRequest {
  adminRemarks?: string;
}

export interface CancellationFilter {
  search?: string;
  courseId?: number | null;
  status?: CancellationStatus | null;
  fromDate?: string;
  toDate?: string;
}
