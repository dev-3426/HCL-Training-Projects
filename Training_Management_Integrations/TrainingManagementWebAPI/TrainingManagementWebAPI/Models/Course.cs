namespace TrainingManagementWebAPI.Models
{
    public class Course
    {
        public string CourseCode { get; set; }
        public string Title { get; set; }
        public decimal Fee { get; set; }
        public int DurationDays { get; set; }
        public int TotalSeats { get; set; }
        public int EnrolledCount { get; set; }

        public int AvailableSeats
        {
            get { return TotalSeats - EnrolledCount; }
        }
    }
}