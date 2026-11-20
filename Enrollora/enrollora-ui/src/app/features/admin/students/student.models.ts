export interface StudentResponse {
  studentId: number;
  name: string;
  email: string;
  department: string;
  age: number;
  cgpa: number;
  status: number;
  registeredAt: string;
  reviewedAt: string | null;
}

export interface StudentFilter {
  search?: string;
  status?: number | null;
  department?: string;
  minAge?: number | null;
  maxAge?: number | null;
  minCgpa?: number | null;
  maxCgpa?: number | null;
}

export interface UpdateStudentRequest {
  name: string;
  email: string;
  department: string;
  age: number;
  cgpa: number;
}
