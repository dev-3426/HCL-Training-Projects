import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { EnrollmentResponse } from '../../enrollment.models';
import { EnrollmentService } from '../../enrollment.service';
import { CancellationService } from '../../cancellation.service';

@Component({
  selector: 'app-my-enrollments',
  standalone: true,
  imports: [CommonModule, FormsModule],
  styleUrl: './my-enrollments.css',
  templateUrl: './my-enrollments.html',
})
export class MyEnrollments implements OnInit {
  private readonly enrollmentService = inject(EnrollmentService);
  private readonly cancellationService = inject(CancellationService);

  enrollments = signal<EnrollmentResponse[]>([]);
  isLoading = signal(false);
  errorMessage = signal('');
  pageNumber = signal(1);
  pageSize = signal(10);
  totalRecords = signal(0);
  totalPages = signal(0);

  // Modal state
  showModal = signal(false);
  selectedEnrollment = signal<EnrollmentResponse | null>(null);
  cancelReason = signal('');
  isSubmitting = signal(false);
  submitError = signal('');
  submitSuccess = signal('');

  ngOnInit(): void {
    this.loadEnrollments();
  }

  loadEnrollments(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.enrollmentService
      .getMyEnrollments(this.pageNumber(), this.pageSize())
      .subscribe({
        next: (response) => {
          this.enrollments.set(response.items);
          this.totalRecords.set(response.totalRecords);
          this.totalPages.set(response.totalPages);
          this.isLoading.set(false);
        },
        error: (error: HttpErrorResponse) => {
          const body = error.error;
          this.errorMessage.set(
            body?.detail || body?.message || body?.title ||
            'Unable to load your enrollments.',
          );
          this.isLoading.set(false);
        },
      });
  }

  statusLabel(status: number): string {
    return status === 0 ? 'Active' : 'Cancelled';
  }

  openCancelModal(enrollment: EnrollmentResponse): void {
    this.selectedEnrollment.set(enrollment);
    this.cancelReason.set('');
    this.submitError.set('');
    this.submitSuccess.set('');
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
    this.selectedEnrollment.set(null);
  }

  submitCancellation(): void {
    const reason = this.cancelReason().trim();
    if (!reason) {
      this.submitError.set('Please provide a reason for cancellation.');
      return;
    }

    const enrollment = this.selectedEnrollment();
    if (!enrollment) return;

    this.isSubmitting.set(true);
    this.submitError.set('');

    this.cancellationService
      .createRequest(enrollment.enrollmentId, { reason })
      .subscribe({
        next: () => {
          this.isSubmitting.set(false);
          this.submitSuccess.set('Cancellation request submitted successfully!');
          setTimeout(() => {
            this.closeModal();
            this.loadEnrollments();
          }, 1500);
        },
        error: (error: HttpErrorResponse) => {
          const body = error.error;
          this.submitError.set(
            body?.detail || body?.message || body?.title ||
            'Failed to submit cancellation request.',
          );
          this.isSubmitting.set(false);
        },
      });
  }

  previousPage(): void {
    if (this.pageNumber() > 1) {
      this.pageNumber.update((page) => page - 1);
      this.loadEnrollments();
    }
  }

  nextPage(): void {
    if (this.pageNumber() < this.totalPages()) {
      this.pageNumber.update((page) => page + 1);
      this.loadEnrollments();
    }
  }
}
