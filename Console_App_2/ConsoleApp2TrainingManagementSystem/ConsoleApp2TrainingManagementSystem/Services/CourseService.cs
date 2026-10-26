using System.Linq;
using System.Collections.Generic;
using ConsoleApp2TrainingManagementSystem.Models;

namespace ConsoleApp2TrainingManagementSystem.Services
{
    public class CourseService
    {
        private readonly Course[] courses;

        public CourseService()
        {
            courses = new Course[]
            {
                new Course("CS101", "C# Programming", 5000m, 30, 40),
                new Course("PY101", "Python Programming", 4500m, 25, 35),
                new Course("WD101", "Web Development", 6000m, 45, 30),
                new Course("DB101", "Database Management", 3500m, 20, 25),
                new Course("AI101", "Artificial Intelligence", 8000m, 60, 20)
            };
        }

        public Course[] GetCourses()
        {
            return courses;
        }


        public Course? FindCourse(string code)
        {
            return courses.FirstOrDefault(
                c => string.Equals(c.CourseCode, code.Trim(), StringComparison.OrdinalIgnoreCase));
        }
        public void DisplayCourses()
        {
            Console.WriteLine();
            Console.WriteLine(
                "{0,-10} {1,-25} {2,10} {3,10} {4,10} {5,10}",
                "Code", "Title", "Fee", "Duration",
                "Total", "Available");

            Console.WriteLine(new string('-', 85));

            for (int i = 0; i < courses.Length; i++)
            {
                Course course = courses[i];

                Console.WriteLine(
                    "{0,-10} {1,-25} {2,10:F2} {3,10} {4,10} {5,10}",
                    course.CourseCode,
                    course.Title,
                    course.Fee,
                    course.DurationDays,
                    course.TotalSeats,
                    course.AvailableSeats);
            }
        }

        public void SearchByCode(string code)
        {
            Course? course = FindCourse(code);

            if (course == null)
            {
                Console.WriteLine("Course not found.");
                return;
            }

            DisplayOneCourse(course);
        }

        public void SearchByTitle(string title)
        {
            bool found = false;

            for (int i = 0; i < courses.Length; i++)
            {
                if (courses[i].Title.Contains(
                    title,
                    StringComparison.OrdinalIgnoreCase))
                {
                    DisplayOneCourse(courses[i]);
                    found = true;
                }
            }

            if (!found)
            {
                Console.WriteLine("No matching course found.");
            }
        }

        private void DisplayOneCourse(Course course)
        {
            Console.WriteLine($"Code: {course.CourseCode}");
            Console.WriteLine($"Title: {course.Title}");
            Console.WriteLine($"Fee: {course.Fee:F2}");
            Console.WriteLine($"Duration: {course.DurationDays} days");
            Console.WriteLine($"Total seats: {course.TotalSeats}");
            Console.WriteLine($"Enrolled: {course.EnrolledCount}");
            Console.WriteLine($"Available seats: {course.AvailableSeats}");
            Console.WriteLine();
        }
        public void DisplayCoursesAboveFee(decimal minimumFee)
        {
            var result = courses.Where(
                c => c.Fee > minimumFee);

            foreach (Course course in result)
            {
                Console.WriteLine(
                    $"{course.CourseCode} | {course.Title} | " +
                    $"Fee: {course.Fee:F2}");
            }
        }



        public void SortByFee(bool ascending)
        {
            IEnumerable<Course> sortedCourses;

            if (ascending)
            {
                sortedCourses = courses.OrderBy(c => c.Fee);
            }
            else
            {
                sortedCourses = courses.OrderByDescending(c => c.Fee);
            }

            foreach (Course course in sortedCourses)
            {
                Console.WriteLine(
                    $"{course.CourseCode} | {course.Title} | " +
                    $"{course.Fee:F2} | " +
                    $"Available: {course.AvailableSeats}");
            }
        }

    }
}
