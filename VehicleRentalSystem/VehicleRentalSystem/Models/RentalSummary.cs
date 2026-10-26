namespace VehicleRentalSystem.Models
{
    public record RentalSummary(
        string ContractId,
        string Model,
        decimal RentalCharge,
        decimal Deposit,
        decimal Total);
}