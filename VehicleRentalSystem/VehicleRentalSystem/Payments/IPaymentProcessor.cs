namespace VehicleRentalSystem.Payments
{
    public interface IPaymentProcessor
    {
        bool ProcessPayment(decimal amount);

        bool RefundPayment(decimal amount);
    }
}