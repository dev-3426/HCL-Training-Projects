namespace VehicleRentalSystem.Payments
{
    public class PaymentProcessor : IPaymentProcessor
    {
        // Implicit interface implementation
        public bool ProcessPayment(decimal amount)
        {
            if (amount < 0)
                return false;

            Console.WriteLine(
                $"Payment of ₹{amount:F2} simulated successfully.");

            return true;
        }

        // Explicit interface implementation
        bool IPaymentProcessor.RefundPayment(decimal amount)
        {
            if (amount <= 0)
                return false;

            Console.WriteLine(
                $"Refund of ₹{amount:F2} simulated successfully.");

            return true;
        }
    }
}