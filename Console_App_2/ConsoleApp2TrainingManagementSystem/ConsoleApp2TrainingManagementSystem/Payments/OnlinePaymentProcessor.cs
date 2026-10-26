using System;

namespace ConsoleApp2TrainingManagementSystem.Payments
{
    public class OnlinePaymentProcessor : IPaymentProcessor
    {
        public bool ProcessPayment(decimal amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine(
                    "Payment amount must be greater than zero.");

                return false;
            }

            Console.WriteLine();
            Console.WriteLine("Processing simulated online payment...");
            Console.WriteLine($"Amount: {amount:F2}");

            Console.WriteLine("Payment successful!");

            return true;
        }
    }
}