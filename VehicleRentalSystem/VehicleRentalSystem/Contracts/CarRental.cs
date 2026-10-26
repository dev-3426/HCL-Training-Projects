using VehicleRentalSystem.Models;

namespace VehicleRentalSystem.Contracts
{
    public class CarRental : RentalContract
    {
        public CarRental(
            string contractId,
            Vehicle vehicle,
            Renter renter,
            RentalPeriod period)
            : base(contractId, vehicle, renter, period)
        {
        }

        public override decimal CalculateRentalCharge()
        {
            return Vehicle.DailyRate * Period.Days;
        }

        public override decimal CalculateDeposit()
        {
            return 3000m;
        }
    }
}