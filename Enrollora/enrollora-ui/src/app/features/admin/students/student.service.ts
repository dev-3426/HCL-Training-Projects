import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';

import { StudentFilter, StudentResponse } from './student.models';
import { PagedResult } from '../../../shared/models/paged-result.model';
import { ApiMessage } from '../../../core/auth/auth.models';
import { UpdateStudentRequest } from './student.models';

@Injectable({
  providedIn: 'root',
})
export class StudentService {
  private http = inject(HttpClient);

  private apiUrl = `${environment.apiUrl}/students`;

  getStudents(
    pageNumber = 1,
    pageSize = 10,
    filters?: StudentFilter,
  ): Observable<PagedResult<StudentResponse>> {
    let params = new HttpParams().set('pageNumber', pageNumber).set('pageSize', pageSize);

    if (filters) {
      if (filters.search) params = params.set('search', filters.search);

      if (filters.status !== null && filters.status !== undefined)
        params = params.set('status', filters.status);

      if (filters.department) params = params.set('department', filters.department);

      if (filters.minAge !== null && filters.minAge !== undefined)
        params = params.set('minAge', filters.minAge);

      if (filters.maxAge !== null && filters.maxAge !== undefined)
        params = params.set('maxAge', filters.maxAge);

      if (filters.minCgpa !== null && filters.minCgpa !== undefined)
        params = params.set('minCgpa', filters.minCgpa);

      if (filters.maxCgpa !== null && filters.maxCgpa !== undefined)
        params = params.set('maxCgpa', filters.maxCgpa);
    }

    return this.http.get<PagedResult<StudentResponse>>(this.apiUrl, { params });
  }

  getStudentById(id: number): Observable<StudentResponse> {
    return this.http.get<StudentResponse>(`${this.apiUrl}/${id}`);
  }

  approveStudent(id: number): Observable<ApiMessage> {
    return this.http.put<ApiMessage>(`${this.apiUrl}/${id}/approve`, {});
  }

  rejectStudent(id: number): Observable<ApiMessage> {
    return this.http.put<ApiMessage>(`${this.apiUrl}/${id}/reject`, {});
  }

  updateStudent(id: number, studentData: UpdateStudentRequest): Observable<StudentResponse> {
    return this.http.put<StudentResponse>(`${this.apiUrl}/${id}`, studentData);
  }

  deleteStudent(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
