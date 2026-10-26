
using System;
using System.Collections.Generic;
using ConsoleApp2TrainingManagementSystem.Models;

namespace ConsoleApp2TrainingManagementSystem.Services
{
    public class EnrollmentService
    {
        private readonly List<Enrollment> enrollments =
            new List<Enrollment>();

        private readonly LearnerService learnerService;
        private readonly CourseService courseService;

        private readonly FeeCalculator feeCalculator =
    new FeeCalculator();

        private int nextEnrollmentId = 1;

        public EnrollmentService(
            LearnerService learnerService,
            CourseService courseService)
        {
            this.learnerService = learnerService;
            this.courseService = courseService;
        }

        public void EnrollLearner(string learnerId, string courseCode)
        {
            // Check learner
            if (!learnerService.TryGetLearner(
                learnerId, out Learner learner))
            {
                Console.WriteLine("Learner does not exist.");
                return;
            }

            // Check course
            Course course = courseService.FindCourse(courseCode);

            if (course == null)
            {
                Console.WriteLine("Course does not exist.");
                return;
            }

            // Check duplicate active enrollment
            for (int i = 0; i < enrollments.Count; i++)
            {
                Enrollment existing = enrollments[i];

                if (string.Equals(existing.LearnerId, learner.LearnerId,
                        StringComparison.OrdinalIgnoreCase)
                    && string.Equals(existing.CourseCode, course.CourseCode,
                        StringComparison.OrdinalIgnoreCase)
                    && existing.Status == EnrollmentStatus.Active)
                {
                    Console.WriteLine(
                        "Learner is already actively enrolled in this course.");
                    return;
                }
            }

            // Check available seats
            if (course.AvailableSeats <= 0)
            {
                Console.WriteLine("This course is full.");
                return;
            }

            decimal baseFee = course.Fee;
            decimal discountPercentage = 0m;

            decimal discountAmount =
                feeCalculator.CalculateDiscount(
                    baseFee, discountPercentage);

            decimal finalFee =
                feeCalculator.CalculateFinalFee(
                    baseFee, discountAmount);

            Enrollment enrollment = new Enrollment(
                nextEnrollmentId,
                learner.LearnerId,
                course.CourseCode,
                DateTime.Now,
                baseFee,
                discountAmount,
                finalFee);

            enrollments.Add(enrollment);
            nextEnrollmentId++;
            course.EnrolledCount++;

            Console.WriteLine("Enrollment successful!");
            Console.WriteLine($"Enrollment ID: {enrollment.EnrollmentId}");
            Console.WriteLine($"Learner: {learner.Name}");
            Console.WriteLine($"Course: {course.Title}");
            Console.WriteLine($"Final fee: {enrollment.FinalFee:F2}");
            Console.WriteLine($"Available seats: {course.AvailableSeats}");
        }

        public void DisplayAllEnrollments()
        {
            if (enrollments.Count == 0)
            {
                Console.WriteLine("No enrollment records found.");
                return;
            }

            for (int i = 0; i < enrollments.Count; i++)
            {
                Enrollment e = enrollments[i];

                Console.WriteLine("------------------------------");
                Console.WriteLine($"Enrollment ID: {e.EnrollmentId}");
                Console.WriteLine($"Learner ID: {e.LearnerId}");
                Console.WriteLine($"Course Code: {e.CourseCode}");
                Console.WriteLine($"Date: {e.EnrollmentDate:dd-MM-yyyy HH:mm}");
                Console.WriteLine($"Base fee: {e.BaseFee:F2}");
                Console.WriteLine($"Discount: {e.DiscountAmount:F2}");
                Console.WriteLine($"Final fee: {e.FinalFee:F2}");
                Console.WriteLine($"Status: {e.Status}");
            }

            Console.WriteLine($"Total records: {enrollments.Count}");
        }

        public void SearchEnrollment(int enrollmentId)
        {
            for (int i = 0; i < enrollments.Count; i++)
            {
                if (enrollments[i].EnrollmentId == enrollmentId)
                {
                    Enrollment e = enrollments[i];

                    Console.WriteLine($"Enrollment ID: {e.EnrollmentId}");
                    Console.WriteLine($"Learner ID: {e.LearnerId}");
                    Console.WriteLine($"Course Code: {e.CourseCode}");
                    Console.WriteLine($"Final fee: {e.FinalFee:F2}");
                    Console.WriteLine($"Status: {e.Status}");
                    return;
                }
            }

            Console.WriteLine("Enrollment not found.");
        }

        public void CancelEnrollment(int enrollmentId)
        {
            for (int i = 0; i < enrollments.Count; i++)
            {
                Enrollment e = enrollments[i];

                if (e.EnrollmentId == enrollmentId)
                {
                    if (e.Status != EnrollmentStatus.Active)
                    {
                        Console.WriteLine(
                            "Only active enrollments can be cancelled.");
                        return;
                    }

                    Course course = courseService.FindCourse(e.CourseCode);

                    e.Status = EnrollmentStatus.Cancelled;

                    if (course != null && course.EnrolledCount > 0)
                    {
                        course.EnrolledCount--;
                    }

                    Console.WriteLine(
                        "Enrollment cancelled. The history record is retained.");
                    return;
                }
            }

            Console.WriteLine("Enrollment not found.");
        }
    
    public Enrollment FindEnrollmentById(int enrollmentId)
        {
            for (int i = 0; i < enrollments.Count; i++)
            {
                if (enrollments[i].EnrollmentId == enrollmentId)
                {
                    return enrollments[i];
                }
            }

            return null;
        }



    }
}

