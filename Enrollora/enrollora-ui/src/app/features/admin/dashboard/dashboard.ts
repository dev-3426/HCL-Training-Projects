import { DashboardService } from './dashboard.service';
import { DashboardSummary } from './dashboard.models';
import { Component, inject, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [DecimalPipe],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Dashboard implements OnInit {
  private dashboardService = inject(DashboardService);

  private cdr = inject(ChangeDetectorRef);

  summary: DashboardSummary | null = null;

  isLoading = false;
  errorMessage = '';

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.dashboardService.getSummary().subscribe({
      next: (response) => {
        this.summary = response;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (error) => {
        console.error('Dashboard loading failed:', error);
        this.errorMessage = 'Unable to load dashboard data.';
        this.isLoading = false;
         this.cdr.markForCheck();
      },
    });
  }
}
