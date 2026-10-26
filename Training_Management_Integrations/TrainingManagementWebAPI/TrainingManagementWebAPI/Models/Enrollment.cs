using System;

namespace TrainingManagementWebAPI.Models
{
    public class Enrollment
    {
        public int EnrollmentId { get; set; }
        public string LearnerId { get; set; }
        public string CourseCode { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public decimal BaseFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalFee { get; set; }
        public string Status { get; set; } = "Active";
    }
}