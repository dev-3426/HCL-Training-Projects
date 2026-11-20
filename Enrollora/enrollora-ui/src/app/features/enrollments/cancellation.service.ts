import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { PagedResult } from '../../shared/models/paged-result.model';
import {
  CancellationFilter,
  CancellationResponse,
  CreateCancellationRequest,
  ReviewCancellationRequest,
} from './cancellation.models';

@Injectable({ providedIn: 'root' })
export class CancellationService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/cancellation-requests`;
  private readonly enrollmentsUrl = `${environment.apiUrl}/enrollments`;

  /** Admin: get all cancellation requests with optional filters and pagination */
  getAllRequests(
    pageNumber = 1,
    pageSize = 10,
    filters?: CancellationFilter,
  ): Observable<PagedResult<CancellationResponse>> {
    return this.http.get<PagedResult<CancellationResponse>>(this.baseUrl, {
      params: this.buildParams(pageNumber, pageSize, filters),
    });
  }

  /** Student: get own cancellation requests */
  getMyRequests(
    pageNumber = 1,
    pageSize = 10,
  ): Observable<PagedResult<CancellationResponse>> {
    return this.http.get<PagedResult<CancellationResponse>>(
      `${this.baseUrl}/my`,
      { params: new HttpParams().set('pageNumber', pageNumber).set('pageSize', pageSize) },
    );
  }

  /** Student: submit a cancellation request for an enrollment */
  createRequest(
    enrollmentId: number,
    body: CreateCancellationRequest,
  ): Observable<CancellationResponse> {
    return this.http.post<CancellationResponse>(
      `${this.enrollmentsUrl}/${enrollmentId}/cancellation-request`,
      body,
    );
  }

  /** Admin: approve a cancellation request */
  approveRequest(
    id: number,
    body: ReviewCancellationRequest,
  ): Observable<CancellationResponse> {
    return this.http.put<CancellationResponse>(
      `${this.baseUrl}/${id}/approve`,
      body,
    );
  }

  /** Admin: reject a cancellation request */
  rejectRequest(
    id: number,
    body: ReviewCancellationRequest,
  ): Observable<CancellationResponse> {
    return this.http.put<CancellationResponse>(
      `${this.baseUrl}/${id}/reject`,
      body,
    );
  }

  private buildParams(
    pageNumber: number,
    pageSize: number,
    filters?: CancellationFilter,
  ): HttpParams {
    let params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);

    if (filters?.search) params = params.set('search', filters.search);
    if (filters?.courseId != null) params = params.set('courseId', filters.courseId);
    if (filters?.status != null) params = params.set('status', filters.status);
    if (filters?.fromDate) params = params.set('fromDate', filters.fromDate);
    if (filters?.toDate) params = params.set('toDate', filters.toDate);

    return params;
  }
}
