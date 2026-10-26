
using System;
using System.Collections.Generic;
using ConsoleApp2TrainingManagementSystem.Models;

namespace ConsoleApp2TrainingManagementSystem.Services
{
    public class CourseFilterService
    {
        public void FilterCourses(
            Course[] courses,
            Predicate<Course> condition)
        {
            foreach (Course course in courses)
            {
                if (condition(course))
                {
                    Console.WriteLine(
                        $"{course.CourseCode} | " +
                        $"{course.Title} | " +
                        $"Fee: {course.Fee:F2}");
                }
            }
        }
    }
}
