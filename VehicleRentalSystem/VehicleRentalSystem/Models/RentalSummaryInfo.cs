namespace VehicleRentalSystem.Models
{
    public readonly struct RentalSummaryInfo
    {
        public string ContractId { get; }
        public decimal RentalCharge { get; }
        public decimal Deposit { get; }

        public RentalSummaryInfo(
            string contractId,
            decimal rentalCharge,
            decimal deposit)
        {
            ContractId = contractId;
            RentalCharge = rentalCharge;
            Deposit = deposit;
        }
    }
}