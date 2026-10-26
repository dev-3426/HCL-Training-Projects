namespace ConsoleApp2TrainingManagementSystem.Services
{
    public class FeeCalculator
    {
        public decimal CalculateDiscount(
            decimal baseFee,
            decimal discountPercentage)
        {
            if (baseFee < 0)
            {
                throw new System.ArgumentException(
                    "Base fee cannot be negative.");
            }

            if (discountPercentage < 0 ||
                discountPercentage > 100)
            {
                throw new System.ArgumentException(
                    "Discount must be between 0 and 100.");
            }

            return baseFee * discountPercentage / 100m;
        }

        public decimal CalculateFinalFee(
            decimal baseFee,
            decimal discountAmount)
        {
            if (baseFee < 0 || discountAmount < 0 ||
                discountAmount > baseFee)
            {
                throw new System.ArgumentException(
                    "Invalid fee or discount amount.");
            }

            return baseFee - discountAmount;
        }
    }
}