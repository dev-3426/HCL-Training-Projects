import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AvailableCourses } from './available-courses';

describe('AvailableCourses', () => {
  let component: AvailableCourses;
  let fixture: ComponentFixture<AvailableCourses>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AvailableCourses],
    }).compileComponents();

    fixture = TestBed.createComponent(AvailableCourses);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
