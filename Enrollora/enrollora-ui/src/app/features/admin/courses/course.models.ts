import { PagedResult } from '../../../shared/models/paged-result.model';

export interface CourseResponse {
  courseId: number;
  courseName: string;
  duration: string;
  fees: number;
  capacity: number;
  enrolledCount: number;
  availableSeats: number;
  isActive: boolean;
}

export interface SaveCourseRequest {
  courseName: string;
  duration: string;
  fees: number;
  capacity: number;
  isActive: boolean;
}

export interface CourseFilter {
  search?: string;
  isActive?: boolean | null;
  availability?: string;
  minFees?: number | null;
  maxFees?: number | null;
  minCapacity?: number | null;
  maxCapacity?: number | null;
}

export type CoursePagedResult = PagedResult<CourseResponse>;
