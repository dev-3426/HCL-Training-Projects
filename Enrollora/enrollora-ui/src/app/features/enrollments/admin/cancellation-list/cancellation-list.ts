import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import {
  CancellationFilter,
  CancellationResponse,
  CancellationStatus,
  ReviewCancellationRequest,
} from '../../cancellation.models';
import { CancellationService } from '../../cancellation.service';

@Component({
  selector: 'app-cancellation-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './cancellation-list.html',
  styleUrl: './cancellation-list.css',
})
export class CancellationListComponent implements OnInit {
  private readonly cancellationService = inject(CancellationService);
  private readonly destroyRef = inject(DestroyRef);
  private closeTimer: ReturnType<typeof setTimeout> | null = null;

  constructor() {
    this.destroyRef.onDestroy(() => {
      if (this.closeTimer) {
        clearTimeout(this.closeTimer);
      }
    });
  }

  requests = signal<CancellationResponse[]>([]);
  isLoading = signal(false);
  errorMessage = signal('');
  pageNumber = signal(1);
  pageSize = signal(10);
  totalRecords = signal(0);
  totalPages = signal(0);

  // Filters
  searchTerm = signal('');
  statusFilter = signal<CancellationStatus | null>(null);

  // Review Modal
  showReviewModal = signal(false);
  selectedRequest = signal<CancellationResponse | null>(null);
  reviewAction = signal<'approve' | 'reject'>('approve');
  adminRemarks = signal('');
  isReviewing = signal(false);
  reviewError = signal('');
  reviewSuccess = signal('');

  readonly CancellationStatus = CancellationStatus;
  readonly statusOptions = [
    { label: 'All Statuses', value: null },
    { label: 'Pending', value: CancellationStatus.Pending },
    { label: 'Approved', value: CancellationStatus.Approved },
    { label: 'Rejected', value: CancellationStatus.Rejected },
  ];

  ngOnInit(): void {
    this.loadRequests();
  }

  loadRequests(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    const filters: CancellationFilter = {
      search: this.searchTerm() || undefined,
      status: this.statusFilter(),
    };

    this.cancellationService
      .getAllRequests(this.pageNumber(), this.pageSize(), filters)
      .subscribe({
        next: (response) => {
          this.requests.set(response.items);
          this.totalRecords.set(response.totalRecords);
          this.totalPages.set(response.totalPages);
          this.isLoading.set(false);
        },
        error: (error: HttpErrorResponse) => {
          const body = error.error;
          this.errorMessage.set(
            body?.detail || body?.message || body?.title ||
            'Unable to load cancellation requests.',
          );
          this.isLoading.set(false);
        },
      });
  }

  applyFilters(): void {
    this.pageNumber.set(1);
    this.loadRequests();
  }

  clearFilters(): void {
    this.searchTerm.set('');
    this.statusFilter.set(null);
    this.pageNumber.set(1);
    this.loadRequests();
  }

  openReviewModal(request: CancellationResponse, action: 'approve' | 'reject'): void {
    this.selectedRequest.set(request);
    this.reviewAction.set(action);
    this.adminRemarks.set('');
    this.reviewError.set('');
    this.reviewSuccess.set('');
    this.showReviewModal.set(true);
  }

  closeReviewModal(): void {
    this.showReviewModal.set(false);
    this.selectedRequest.set(null);
  }

  submitReview(): void {
    if (this.isReviewing() || this.reviewSuccess()) return;

    const request = this.selectedRequest();
    if (!request) return;

    this.isReviewing.set(true);
    this.reviewError.set('');

    const body: ReviewCancellationRequest = {
      adminRemarks: this.adminRemarks().trim() || undefined,
    };

    const action$ = this.reviewAction() === 'approve'
      ? this.cancellationService.approveRequest(request.cancellationRequestId, body)
      : this.cancellationService.rejectRequest(request.cancellationRequestId, body);

    action$.subscribe({
      next: () => {
        this.isReviewing.set(false);
        this.reviewSuccess.set(
          this.reviewAction() === 'approve'
            ? 'Request approved successfully!'
            : 'Request rejected successfully.',
        );
        this.closeTimer = setTimeout(() => {
          this.closeReviewModal();
          this.loadRequests();
        }, 1400);
      },
      error: (error: HttpErrorResponse) => {
        const body = error.error;
        this.reviewError.set(
          body?.detail || body?.message || body?.title ||
          'Failed to process this request.',
        );
        this.isReviewing.set(false);
      },
    });
  }

  statusLabel(status: CancellationStatus): string {
    switch (status) {
      case CancellationStatus.Pending:  return 'Pending';
      case CancellationStatus.Approved: return 'Approved';
      case CancellationStatus.Rejected: return 'Rejected';
      default: return 'Unknown';
    }
  }

  statusClass(status: CancellationStatus): string {
    switch (status) {
      case CancellationStatus.Pending:  return 'pending';
      case CancellationStatus.Approved: return 'approved';
      case CancellationStatus.Rejected: return 'rejected';
      default: return '';
    }
  }

  previousPage(): void {
    if (this.pageNumber() > 1) {
      this.pageNumber.update((p) => p - 1);
      this.loadRequests();
    }
  }

  nextPage(): void {
    if (this.pageNumber() < this.totalPages()) {
      this.pageNumber.update((p) => p + 1);
      this.loadRequests();
    }
  }
}
