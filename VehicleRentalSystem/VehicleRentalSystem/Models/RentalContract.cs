using VehicleRentalSystem.Models;

namespace VehicleRentalSystem.Contracts
{
    public abstract class RentalContract
    {
        public string ContractId { get; }
        public Vehicle Vehicle { get; }
        public Renter Renter { get; }
        public RentalPeriod Period { get; }

        public bool IsReturned { get; set; }

        protected RentalContract(
            string contractId,
            Vehicle vehicle,
            Renter renter,
            RentalPeriod period)
        {
            this.ContractId = contractId;
            this.Vehicle = vehicle;
            this.Renter = renter;
            this.Period = period;
            this.IsReturned = false;
        }

        public abstract decimal CalculateRentalCharge();

        public virtual decimal CalculateDeposit()
        {
            return 1000m;
        }

        public virtual decimal CalculateLateCharge(int extraDays)
        {
            return extraDays * Vehicle.DailyRate;
        }

        // Method overloading: first version
        public static decimal CalculateCharge(
            decimal dailyRate, int days)
        {
            return dailyRate * days;
        }

        // Method overloading: second version
        public static decimal CalculateCharge(
            decimal dailyRate, int days, decimal discountPercent)
        {
            decimal charge = dailyRate * days;
            return charge - (charge * discountPercent / 100m);
        }
    }
}