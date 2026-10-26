namespace ConsoleApp2TrainingManagementSystem.Payments
{
    public interface IPaymentProcessor
    {
        bool ProcessPayment(decimal amount);
    }
}