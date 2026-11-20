import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { CourseResponse, SaveCourseRequest, CourseFilter } from './course.models';

import { PagedResult } from '../../../shared/models/paged-result.model';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class CourseService {
  private http = inject(HttpClient);

  private apiUrl = `${environment.apiUrl}/courses`;

  // GET: Retrieve paginated courses with filters
  getCourses(
    pageNumber: number = 1,
    pageSize: number = 10,
    filters?: CourseFilter,
  ): Observable<PagedResult<CourseResponse>> {
    let params = new HttpParams().set('pageNumber', pageNumber).set('pageSize', pageSize);

    if (filters) {
      if (filters.search) {
        params = params.set('search', filters.search);
      }

      if (filters.isActive !== null && filters.isActive !== undefined) {
        params = params.set('isActive', filters.isActive);
      }

      if (filters.availability) {
        params = params.set('availability', filters.availability);
      }

      if (filters.minFees !== null && filters.minFees !== undefined) {
        params = params.set('minFees', filters.minFees);
      }

      if (filters.maxFees !== null && filters.maxFees !== undefined) {
        params = params.set('maxFees', filters.maxFees);
      }

      if (filters.minCapacity !== null && filters.minCapacity !== undefined) {
        params = params.set('minCapacity', filters.minCapacity);
      }

      if (filters.maxCapacity !== null && filters.maxCapacity !== undefined) {
        params = params.set('maxCapacity', filters.maxCapacity);
      }
    }

    return this.http.get<PagedResult<CourseResponse>>(this.apiUrl, { params });
  }

  // GET: Retrieve a single course
  getCourseById(id: number): Observable<CourseResponse> {
    return this.http.get<CourseResponse>(`${this.apiUrl}/${id}`);
  }

  // POST: Create a course
  createCourse(request: SaveCourseRequest): Observable<CourseResponse> {
    return this.http.post<CourseResponse>(this.apiUrl, request);
  }

  // PUT: Update a course
  updateCourse(id: number, request: SaveCourseRequest): Observable<CourseResponse> {
    return this.http.put<CourseResponse>(`${this.apiUrl}/${id}`, request);
  }

  // DELETE: Deactivate a course
  deactivateCourse(id: number): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.apiUrl}/${id}`);
  }
}
