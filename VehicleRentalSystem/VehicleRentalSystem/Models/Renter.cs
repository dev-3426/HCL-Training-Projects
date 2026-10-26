namespace VehicleRentalSystem.Models
{
    public class Renter
    {
        public int RenterId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string DrivingLicense { get; set; }

        public Renter(int renterId, string name,
                      string phone, string drivingLicense)
        {
            this.RenterId = renterId;
            this.Name = name;
            this.Phone = phone;
            this.DrivingLicense = drivingLicense;
        }
    }
}