using System;

namespace ConsoleApp2TrainingManagementSystem.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int EnrollmentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentStatus Status { get; set; }

        public Payment(
            int paymentId,
            int enrollmentId,
            decimal amount)
        {
            PaymentId = paymentId;
            EnrollmentId = enrollmentId;
            Amount = amount;
            PaymentDate = DateTime.Now;
            Status = PaymentStatus.Pending;
        }
    }
}