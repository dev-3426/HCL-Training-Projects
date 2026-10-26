using VehicleRentalSystem.Models;

namespace VehicleRentalSystem.Contracts
{
    public class BikeRental : RentalContract
    {
        public BikeRental(
            string contractId,
            Vehicle vehicle,
            Renter renter,
            RentalPeriod period)
            : base(contractId, vehicle, renter, period)
        {
        }

        public override decimal CalculateRentalCharge()
        {
            // Bike service fee: ₹50 per rental day
            return (Vehicle.DailyRate + 50m) * Period.Days;
        }

        public override decimal CalculateDeposit()
        {
            return 1000m;
        }
    }
}