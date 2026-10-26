using System;
using System.Collections.Generic;
using ConsoleApp2TrainingManagementSystem.Models;
using ConsoleApp2TrainingManagementSystem.Payments;

namespace ConsoleApp2TrainingManagementSystem.Services
{
    public class PaymentService
    {
        private readonly List<Payment> payments =
            new List<Payment>();

        private readonly EnrollmentService enrollmentService;
        private readonly IPaymentProcessor paymentProcessor;

        private int nextPaymentId = 1;

        public PaymentService(
            EnrollmentService enrollmentService,
            IPaymentProcessor paymentProcessor)
        {
            this.enrollmentService = enrollmentService;
            this.paymentProcessor = paymentProcessor;
        }

        public void MakePayment(int enrollmentId)
        {
            Enrollment enrollment =
                enrollmentService.FindEnrollmentById(enrollmentId);

            if (enrollment == null)
            {
                Console.WriteLine("Enrollment not found.");
                return;
            }

            if (enrollment.Status != EnrollmentStatus.Active)
            {
                Console.WriteLine(
                    "Payment is allowed only for active enrollments.");
                return;
            }

            for (int i = 0; i < payments.Count; i++)
            {
                if (payments[i].EnrollmentId == enrollmentId &&
                    payments[i].Status == PaymentStatus.Successful)
                {
                    Console.WriteLine(
                        "This enrollment has already been paid.");
                    return;
                }
            }

            Payment payment = new Payment(
                nextPaymentId,
                enrollmentId,
                enrollment.FinalFee);

            payments.Add(payment);
            nextPaymentId++;

            bool success =
                paymentProcessor.ProcessPayment(payment.Amount);

            if (success)
            {
                payment.Status = PaymentStatus.Successful;
                Console.WriteLine("Payment recorded successfully.");
            }
            else
            {
                payment.Status = PaymentStatus.Failed;
                Console.WriteLine("Payment failed.");
            }

            Console.WriteLine($"Payment ID: {payment.PaymentId}");
            Console.WriteLine($"Enrollment ID: {payment.EnrollmentId}");
            Console.WriteLine($"Amount: {payment.Amount:F2}");
            Console.WriteLine($"Status: {payment.Status}");
        }

        public void DisplayAllPayments()
        {
            if (payments.Count == 0)
            {
                Console.WriteLine("No payment records found.");
                return;
            }

            for (int i = 0; i < payments.Count; i++)
            {
                Payment payment = payments[i];

                Console.WriteLine("------------------------------");
                Console.WriteLine($"Payment ID: {payment.PaymentId}");
                Console.WriteLine(
                    $"Enrollment ID: {payment.EnrollmentId}");
                Console.WriteLine($"Amount: {payment.Amount:F2}");
                Console.WriteLine(
                    $"Date: {payment.PaymentDate:dd-MM-yyyy HH:mm}");
                Console.WriteLine($"Status: {payment.Status}");
            }
        }
    }
}