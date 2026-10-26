namespace VehicleRentalSystem.Models
{
    public class Vehicle
    {
        public string RegistrationNumber { get; set; }
        public string Model { get; set; }
        public string Category { get; set; }
        public decimal DailyRate { get; set; }
        public bool IsAvailable { get; set; }

        public Vehicle(string registrationNumber,
                       string model,
                       string category,
                       decimal dailyRate)
        {
            this.RegistrationNumber = registrationNumber;
            this.Model = model;
            this.Category = category;
            this.DailyRate = dailyRate;
            this.IsAvailable = true;
        }
    }
}