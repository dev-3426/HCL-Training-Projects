import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

import { CourseService } from '../course.service';
import { CourseResponse, SaveCourseRequest } from '../course.models';

@Component({
  selector: 'app-course-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './course-list.html',
  styleUrl: './course-list.css',
})
export class CourseListComponent implements OnInit {
  private courseService = inject(CourseService);

  // Signals
  courses = signal<CourseResponse[]>([]);

  isLoading = signal(false);
  errorMessage = signal('');
  actionMessage = signal('');
  actionError = signal('');
  processingCourseId = signal<number | null>(null);

  // Modal signals
  showModal = signal(false);
  isEditMode = signal(false);
  editingCourseId = signal<number | null>(null);
  isSaving = signal(false);
  modalError = signal('');

  courseForm = new FormGroup({
    courseName: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(150)],
    }),
    duration: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(80)],
    }),
    fees: new FormControl(0, {
      nonNullable: true,
      validators: [Validators.required, Validators.min(0), Validators.max(999999999)],
    }),
    capacity: new FormControl(30, {
      nonNullable: true,
      validators: [Validators.required, Validators.min(1), Validators.max(100000)],
    }),
    isActive: new FormControl(true, { nonNullable: true }),
  });

  pageNumber = signal(1);
  pageSize = signal(10);

  totalRecords = signal(0);
  totalPages = signal(0);

  ngOnInit(): void {
    this.loadCourses();
  }

  // Load courses from backend
  loadCourses(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.courseService.getCourses(this.pageNumber(), this.pageSize()).subscribe({
      next: (response) => {
        this.courses.set(response.items);

        this.totalRecords.set(response.totalRecords);
        this.totalPages.set(response.totalPages);

        this.isLoading.set(false);
      },

      error: (error) => {
        console.error('Error loading courses:', error);

        this.errorMessage.set('Unable to load courses.');

        this.isLoading.set(false);
      },
    });
  }

  openCreateModal(): void {
    this.isEditMode.set(false);
    this.editingCourseId.set(null);
    this.modalError.set('');
    this.courseForm.reset({
      courseName: '',
      duration: '',
      fees: 0,
      capacity: 30,
      isActive: true,
    });
    this.showModal.set(true);
  }

  openEditModal(course: CourseResponse): void {
    this.isEditMode.set(true);
    this.editingCourseId.set(course.courseId);
    this.modalError.set('');
    this.courseForm.reset({
      courseName: course.courseName,
      duration: course.duration,
      fees: course.fees,
      capacity: course.capacity,
      isActive: course.isActive,
    });
    this.showModal.set(true);
  }

  closeModal(): void {
    if (this.isSaving()) return;
    this.showModal.set(false);
    this.editingCourseId.set(null);
    this.modalError.set('');
  }

  saveCourse(): void {
    if (this.courseForm.invalid) {
      this.courseForm.markAllAsTouched();
      return;
    }

    if (this.isSaving()) return;

    this.isSaving.set(true);
    this.modalError.set('');
    this.actionMessage.set('');
    this.actionError.set('');

    const raw = this.courseForm.getRawValue();
    const payload: SaveCourseRequest = {
      courseName: raw.courseName.trim(),
      duration: raw.duration.trim(),
      fees: Number(raw.fees),
      capacity: Number(raw.capacity),
      isActive: raw.isActive,
    };

    const courseId = this.editingCourseId();
    const request$ =
      this.isEditMode() && courseId !== null
        ? this.courseService.updateCourse(courseId, payload)
        : this.courseService.createCourse(payload);

    request$.subscribe({
      next: () => {
        this.isSaving.set(false);
        this.showModal.set(false);
        this.editingCourseId.set(null);
        this.actionMessage.set(
          this.isEditMode()
            ? 'Course updated successfully.'
            : 'Course created successfully.',
        );
        this.loadCourses();
      },
      error: (error: HttpErrorResponse) => {
        this.isSaving.set(false);
        this.modalError.set(
          this.extractError(error, 'Unable to save course. Please check your inputs.'),
        );
      },
    });
  }

  deactivateCourse(course: CourseResponse): void {
    if (!course.isActive || this.processingCourseId() !== null) return;

    if (
      !confirm(
        `Deactivate "${course.courseName}"? Historical enrollments will be preserved.`,
      )
    ) {
      return;
    }

    this.processingCourseId.set(course.courseId);
    this.actionMessage.set('');
    this.actionError.set('');

    this.courseService.deactivateCourse(course.courseId).subscribe({
      next: (res) => {
        this.processingCourseId.set(null);
        this.actionMessage.set(res.message || 'Course deactivated.');
        this.loadCourses();
      },
      error: (error: HttpErrorResponse) => {
        this.processingCourseId.set(null);
        this.actionError.set(this.extractError(error, 'Unable to deactivate course.'));
      },
    });
  }

  // Navigate to next page
  nextPage(): void {
    if (this.pageNumber() < this.totalPages()) {
      this.pageNumber.update((page) => page + 1);

      this.loadCourses();
    }
  }

  // Navigate to previous page
  previousPage(): void {
    if (this.pageNumber() > 1) {
      this.pageNumber.update((page) => page - 1);

      this.loadCourses();
    }
  }

  private extractError(error: HttpErrorResponse, fallback: string): string {
    const body = error.error;
    if (typeof body === 'string' && body.trim()) return body;
    if (body?.detail) return body.detail;
    if (body?.message) return body.message;
    if (body?.title) return body.title;
    if (body?.errors) {
      const msgs = Object.values(body.errors).flat().map(String);
      if (msgs.length > 0) return msgs.join(' ');
    }
    return fallback;
  }
}
