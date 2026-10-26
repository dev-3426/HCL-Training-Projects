namespace VehicleRentalSystem.Models
{
    public struct RentalPeriod
    {
        public DateTime StartDate { get; }
        public DateTime ExpectedReturnDate { get; }

        public RentalPeriod(DateTime startDate,
                            DateTime expectedReturnDate)
        {
            if (expectedReturnDate <= startDate)
            {
                throw new ArgumentException(
                    "Return date must be after start date.");
            }

            this.StartDate = startDate;
            this.ExpectedReturnDate = expectedReturnDate;
        }

        public int Days =>
            (ExpectedReturnDate - StartDate).Days;
    }
}