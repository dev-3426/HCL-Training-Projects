
using TrainingManagementWebAPI.Models;
using TrainingManagementWebAPI.Generics;

namespace TrainingManagementWebAPI.Services
{
    public class CourseService
    {
        private readonly Repository<Course> repository;

        public CourseService(Repository<Course> repository)
        {
            this.repository = repository;

            if (repository.GetAll().Count == 0)
            {
                repository.Add(new Course
                {
                    CourseCode = "CS101",
                    Title = "C# Programming",
                    Fee = 5000,
                    DurationDays = 30,
                    TotalSeats = 40
                });

                repository.Add(new Course
                {
                    CourseCode = "PY101",
                    Title = "Python Programming",
                    Fee = 4500,
                    DurationDays = 25,
                    TotalSeats = 35
                });

                repository.Add(new Course
                {
                    CourseCode = "WD101",
                    Title = "Web Development",
                    Fee = 6000,
                    DurationDays = 45,
                    TotalSeats = 30
                });

                repository.Add(new Course
                {
                    CourseCode = "DB101",
                    Title = "Database Management",
                    Fee = 3500,
                    DurationDays = 20,
                    TotalSeats = 25
                });

                repository.Add(new Course
                {
                    CourseCode = "AI101",
                    Title = "Artificial Intelligence",
                    Fee = 8000,
                    DurationDays = 60,
                    TotalSeats = 20
                });
            }
        }

        public List<Course> GetAllCourses()
        {
            return repository.GetAll();
        }

        public Course? SearchCourse(string code)
        {
            return repository.Find(c =>
                c.CourseCode.Equals(
                    code, StringComparison.OrdinalIgnoreCase));
        }

        public List<Course> GetAffordableCourses()
        {
            return repository.Filter(c => c.Fee < 5000);
        }

        public List<Course> SortCoursesByFee()
        {
            return repository.GetAll()
                .OrderBy(c => c.Fee)
                .ToList();
        }

        public List<Course> FilterCourses(Func<Course, bool> condition)
        {
            return repository.Filter(condition);
        }

        public void ProcessCourses(Action<Course> action)
        {
            foreach (Course course in repository.GetAll())
            {
                action(course);
            }
        }
    }
}
