import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, inject, signal } from '@angular/core';

import { CancellationResponse, CancellationStatus } from '../../cancellation.models';
import { CancellationService } from '../../cancellation.service';

@Component({
  selector: 'app-my-cancellations',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './my-cancellations.html',
  styleUrl: './my-cancellations.css',
})
export class MyCancellations implements OnInit {
  private readonly cancellationService = inject(CancellationService);

  requests = signal<CancellationResponse[]>([]);
  isLoading = signal(false);
  errorMessage = signal('');
  pageNumber = signal(1);
  pageSize = signal(10);
  totalRecords = signal(0);
  totalPages = signal(0);

  readonly CancellationStatus = CancellationStatus;

  ngOnInit(): void {
    this.loadRequests();
  }

  loadRequests(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.cancellationService
      .getMyRequests(this.pageNumber(), this.pageSize())
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
            'Unable to load your cancellation requests.',
          );
          this.isLoading.set(false);
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
