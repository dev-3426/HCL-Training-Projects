
using TrainingManagementWebAPI.Models;
using TrainingManagementWebAPI.Generics;

namespace TrainingManagementWebAPI.Services
{
    public class EnrollmentService
    {
        private readonly Repository<Enrollment> repository;
        private readonly CourseService courseService;
        private readonly LearnerService learnerService;

        public EnrollmentService(
            Repository<Enrollment> repository,
            CourseService courseService,
            LearnerService learnerService)
        {
            this.repository = repository;
            this.courseService = courseService;
            this.learnerService = learnerService;
        }

        public List<Enrollment> GetAllEnrollments()
        {
            return repository.GetAll();
        }

        public string Enroll(Enrollment enrollment)
        {
            var learner =
                learnerService.SearchLearner(enrollment.LearnerId);

            if (learner == null)
                return "Learner not found. Register the learner first.";

            var course =
                courseService.SearchCourse(enrollment.CourseCode);

            if (course == null)
                return "Course not found.";

            if (course.AvailableSeats <= 0)
                return "No seats available.";

            bool alreadyEnrolled = repository.GetAll().Any(e =>
                e.LearnerId.Equals(
                    enrollment.LearnerId,
                    StringComparison.OrdinalIgnoreCase) &&
                e.CourseCode.Equals(
                    enrollment.CourseCode,
                    StringComparison.OrdinalIgnoreCase) &&
                e.Status == "Active");

            if (alreadyEnrolled)
                return "Learner is already enrolled in this course.";

            enrollment.EnrollmentId =
                repository.GetAll().Count + 1;

            enrollment.EnrollmentDate = DateTime.Now;
            enrollment.BaseFee = course.Fee;

            // 10% discount for demonstration
            enrollment.DiscountAmount = course.Fee * 0.10m;

            enrollment.FinalFee =
                enrollment.BaseFee - enrollment.DiscountAmount;

            enrollment.Status = "Active";

            course.EnrolledCount++;
            repository.Add(enrollment);

            return "Enrollment successful. Final fee: " +
                   enrollment.FinalFee.ToString("C");
        }
    }
}
