using VehicleRentalSystem.Models;
using VehicleRentalSystem.Contracts;

namespace VehicleRentalSystem.Services
{
    public class RentalService
    {
        private readonly Vehicle[] vehicles =
        {
            new Vehicle("TN01AB1234", "Swift", "Car", 2000m),
            new Vehicle("TN01CD5678", "Honda City", "Car", 3000m),
            new Vehicle("TN02EF1234", "Activa", "Bike", 500m),
            new Vehicle("TN02GH5678", "Royal Enfield", "Bike", 800m),
            new Vehicle("TN03JK9012", "Creta", "Car", 2500m)
        };

        private readonly List<Renter> renters = new();
        private readonly List<RentalContract> contracts = new();

        public void ListVehicles()
        {
            Console.WriteLine(
                "\nRegistration     Model              Category  Rate    Status");

            foreach (Vehicle vehicle in vehicles)
            {
                Console.WriteLine(
                    $"{vehicle.RegistrationNumber,-16} " +
                    $"{vehicle.Model,-18} " +
                    $"{vehicle.Category,-9} " +
                    $"₹{vehicle.DailyRate,-6} " +
                    $"{(vehicle.IsAvailable ? "Available" : "Rented")}");
            }
        }

        public void SearchVehicles(string keyword)
        {
            bool found = false;

            foreach (Vehicle vehicle in vehicles)
            {
                if (!vehicle.Model.Contains(
                        keyword, StringComparison.OrdinalIgnoreCase)
                    && !vehicle.Category.Contains(
                        keyword, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                Console.WriteLine(
                    $"{vehicle.Model} | {vehicle.Category} | " +
                    $"₹{vehicle.DailyRate} | " +
                    $"{(vehicle.IsAvailable ? "Available" : "Rented")}");

                found = true;
            }

            if (!found)
                Console.WriteLine("No matching vehicles found.");
        }

        public void SortVehiclesByRate()
        {
            Array.Sort(vehicles,
                (a, b) => a.DailyRate.CompareTo(b.DailyRate));

            Console.WriteLine("Vehicles sorted by daily rate.");
            ListVehicles();
        }

        // out: return a vehicle through a parameter
        public bool TryGetVehicle(
            string registration, out Vehicle? vehicle)
        {
            vehicle = null;

            foreach (Vehicle item in vehicles)
            {
                if (item.RegistrationNumber.Equals(
                    registration, StringComparison.OrdinalIgnoreCase))
                {
                    vehicle = item;
                    return true;
                }
            }

            return false;
        }

        public void RegisterRenter(Renter renter)
        {
            renters.Add(renter);
            Console.WriteLine(
                $"Renter registered. ID: {renter.RenterId}");
        }

        public Renter? GetRenter(int renterId)
        {
            foreach (Renter renter in renters)
            {
                if (renter.RenterId == renterId)
                    return renter;
            }

            return null;
        }

        // ref: update the rental count in the calling method
        public RentalContract? RentVehicle(
            string registration,
            Renter renter,
            RentalPeriod period,
            ref int rentalCount)
        {
            if (!TryGetVehicle(registration, out Vehicle? vehicle))
            {
                Console.WriteLine("Invalid registration number.");
                return null;
            }

            if (!vehicle!.IsAvailable)
            {
                Console.WriteLine("Vehicle is already rented.");
                return null;
            }

            rentalCount++;

            string contractId = $"R{rentalCount:000}";

            RentalContract contract;

            if (vehicle.Category.Equals(
                "Car", StringComparison.OrdinalIgnoreCase))
            {
                contract = new CarRental(
                    contractId, vehicle, renter, period);
            }
            else if (vehicle.Category.Equals(
                "Bike", StringComparison.OrdinalIgnoreCase))
            {
                contract = new BikeRental(
                    contractId, vehicle, renter, period);
            }
            else
            {
                rentalCount--;
                Console.WriteLine("Unsupported vehicle category.");
                return null;
            }

            vehicle.IsAvailable = false;
            contracts.Add(contract);

            Console.WriteLine($"Contract created: {contractId}");
            return contract;
        }

        public RentalContract? GetContract(string contractId)
        {
            foreach (RentalContract contract in contracts)
            {
                if (contract.ContractId.Equals(
                    contractId, StringComparison.OrdinalIgnoreCase))
                {
                    return contract;
                }
            }

            return null;
        }

        // Named tuple: return multiple billing values
        public (
            decimal RentalCharge,
            decimal LateFee,
            decimal DamageFee,
            decimal MileageFee,
            decimal Deposit,
            decimal FinalCharges,
            decimal DepositRefund,
            decimal AdditionalDue
        ) ReturnVehicle(
            RentalContract contract,
            DateTime actualReturnDate,
            bool damaged,
            decimal kilometresTravelled)
        {
            if (contract.IsReturned)
                throw new InvalidOperationException(
                    "This contract has already been returned.");

            if (actualReturnDate < contract.Period.StartDate)
                throw new ArgumentException(
                    "Return date cannot be before rental start.");

            if (kilometresTravelled < 0)
                throw new ArgumentException(
                    "Distance cannot be negative.");

            int extraDays = Math.Max(
                0,
                (actualReturnDate -
                 contract.Period.ExpectedReturnDate).Days);

            decimal rentalCharge =
                contract.CalculateRentalCharge();

            decimal lateFee =
                contract.CalculateLateCharge(extraDays);

            decimal damageFee = damaged ? 1500m : 0m;

            // Bonus: mileage fee for bikes, ₹2 per kilometre
            decimal mileageFee =
                contract.Vehicle.Category.Equals(
                    "Bike", StringComparison.OrdinalIgnoreCase)
                ? kilometresTravelled * 2m
                : 0m;

            decimal deposit = contract.CalculateDeposit();

            decimal extraCharges =
                lateFee + damageFee + mileageFee;

            decimal finalCharges =
                rentalCharge + extraCharges;

            decimal depositRefund =
                Math.Max(0m, deposit - extraCharges);

            decimal additionalDue =
                Math.Max(0m, extraCharges - deposit);

            contract.IsReturned = true;
            contract.Vehicle.IsAvailable = true;

            return (
                RentalCharge: rentalCharge,
                LateFee: lateFee,
                DamageFee: damageFee,
                MileageFee: mileageFee,
                Deposit: deposit,
                FinalCharges: finalCharges,
                DepositRefund: depositRefund,
                AdditionalDue: additionalDue
            );
        }
    }
}