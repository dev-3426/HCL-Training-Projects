import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { CourseResponse } from '../../admin/courses/course.models';
import { CourseService } from '../../admin/courses/course.service';
import {EnrollmentResponse, EnrollmentStatus,} from '../../enrollments/enrollment.models';
import { EnrollmentService } from '../../enrollments/enrollment.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  styleUrl: './dashboard.css',
  templateUrl: './dashboard.html',
})
export class Dashboard implements OnInit {
  private readonly courseService = inject(CourseService);
  private readonly enrollmentService = inject(EnrollmentService);

  availableCourses = signal<CourseResponse[]>([]);
  recentEnrollments = signal<EnrollmentResponse[]>([]);

  activeEnrollmentCount = signal(0);
  cancelledEnrollmentCount = signal(0);
  totalEnrollmentCount = signal(0);
  availableCourseCount = signal(0);
  openSeatCount = signal(0);

  isLoading = signal(true);
  courseError = signal('');
  enrollmentError = signal('');

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.isLoading.set(true);
    this.courseError.set('');
    this.enrollmentError.set('');

    let courseRequestComplete = false;
    let enrollmentRequestComplete = false;

    const markComplete = (): void => {
      if (courseRequestComplete && enrollmentRequestComplete) {
        this.isLoading.set(false);
      }
    };

    this.courseService
      .getCourses(1, 6, { isActive: true, availability: 'available' })
      .subscribe({
        next: (response) => {
          this.availableCourses.set(response.items);
          this.availableCourseCount.set(response.totalRecords);
          this.openSeatCount.set(
            response.items.reduce((total, course) => total + course.availableSeats, 0),
          );
          courseRequestComplete = true;
          markComplete();
        },
        error: (error: HttpErrorResponse) => {
          this.courseError.set(this.getErrorMessage(error, 'Unable to load available courses.'));
          courseRequestComplete = true;
          markComplete();
        },
      });

    this.enrollmentService
      .getMyEnrollments(1, 6)
      .subscribe({
        next: (response) => {
          const enrollments = response.items;
          this.recentEnrollments.set(enrollments);
          this.totalEnrollmentCount.set(response.totalRecords);
          let activeStatsComplete = false;
          let cancelledStatsComplete = false;

          const markStatsComplete = (): void => {
            if (activeStatsComplete && cancelledStatsComplete) {
              enrollmentRequestComplete = true;
              markComplete();
            }
          };

          this.enrollmentService
            .getMyEnrollments(1, 1, { status: EnrollmentStatus.Active })
            .subscribe({
              next: (activeResponse) => {
                this.activeEnrollmentCount.set(activeResponse.totalRecords);
                activeStatsComplete = true;
                markStatsComplete();
              },
              error: () => {
                this.activeEnrollmentCount.set(0);
                activeStatsComplete = true;
                markStatsComplete();
              },
            });

          this.enrollmentService
            .getMyEnrollments(1, 1, { status: EnrollmentStatus.Cancelled })
            .subscribe({
              next: (cancelledResponse) => {
                this.cancelledEnrollmentCount.set(cancelledResponse.totalRecords);
                cancelledStatsComplete = true;
                markStatsComplete();
              },
              error: () => {
                this.cancelledEnrollmentCount.set(0);
                cancelledStatsComplete = true;
                markStatsComplete();
              },
            });
        },
        error: (error: HttpErrorResponse) => {
          this.enrollmentError.set(
            this.getErrorMessage(error, 'Unable to load your enrollments.'),
          );
          enrollmentRequestComplete = true;
          markComplete();
        },
      });
  }

statusLabel(status: number): string {
  return status === EnrollmentStatus.Active? 'Active': 'Cancelled';
}

  private getErrorMessage(error: HttpErrorResponse, fallback: string): string {
    const body = error.error;
    if (typeof body === 'string' && body.trim()) {
      return body;
    }
    if (body?.detail) {
      return body.detail;
    }
    if (body?.message) {
      return body.message;
    }
    if (body?.title) {
      return body.title;
    }
    return fallback;
  }
}
