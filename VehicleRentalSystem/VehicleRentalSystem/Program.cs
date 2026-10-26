using VehicleRentalSystem.Models;
using VehicleRentalSystem.Contracts;
using VehicleRentalSystem.Services;
using VehicleRentalSystem.Payments;

namespace VehicleRentalSystem
{
    internal class Program
    {
        static void Main()
        {
            RentalService service = new RentalService();

            int renterCount = 0;
            int rentalCount = 0;

            while (true)
            {
                Console.WriteLine("\n===== VEHICLE RENTAL SYSTEM =====");
                Console.WriteLine("1. List Vehicles");
                Console.WriteLine("2. Search Vehicles");
                Console.WriteLine("3. Sort by Daily Rate");
                Console.WriteLine("4. Register Renter");
                Console.WriteLine("5. Rent Vehicle");
                Console.WriteLine("6. Return Vehicle");
                Console.WriteLine("7. Simulate Payment");
                Console.WriteLine("8. View Rental Summary");
                Console.WriteLine("9. Exit");
                Console.Write("Enter choice: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Enter a valid number.");
                    continue;
                }

                // Switch expression
                string action = choice switch
                {
                    1 => "List",
                    2 => "Search",
                    3 => "Sort",
                    4 => "Register",
                    5 => "Rent",
                    6 => "Return",
                    7 => "Payment",
                    8 => "Summary",
                    9 => "Exit",
                    _ => "Invalid"
                };

                Console.WriteLine($"Selected: {action}");

                // Switch statement
                switch (choice)
                {
                    case 1:
                        service.ListVehicles();
                        break;

                    case 2:
                        Console.Write("Enter model or category: ");
                        string keyword = Console.ReadLine() ?? "";
                        service.SearchVehicles(keyword);
                        break;

                    case 3:
                        service.SortVehiclesByRate();
                        break;

                    case 4:
                        RegisterRenter(service, ref renterCount);
                        break;

                    case 5:
                        RentVehicle(service, ref rentalCount);
                        break;

                    case 6:
                        ReturnVehicle(service);
                        break;

                    case 7:
                        SimulatePayment();
                        break;

                    case 8:
                        ViewSummary(service);
                        break;

                    case 9:
                        Console.WriteLine("Thank you!");
                        return;

                    default:
                        Console.WriteLine("Invalid menu choice.");
                        break;
                }
            }
        }

        static void RegisterRenter(
            RentalService service, ref int renterCount)
        {
            Console.Write("Enter name: ");
            string name = (Console.ReadLine() ?? "").Trim();

            Console.Write("Enter phone number: ");
            string phone = (Console.ReadLine() ?? "").Trim();

            Console.Write("Enter driving license: ");
            string license = (Console.ReadLine() ?? "").Trim();

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(license))
            {
                Console.WriteLine("All fields are required.");
                return;
            }

            if (phone.Length != 10 ||
                !phone.All(char.IsDigit))
            {
                Console.WriteLine(
                    "Enter a valid 10-digit phone number.");
                return;
            }

            renterCount++;

            Renter renter = new Renter(
                renterCount, name, phone, license);

            service.RegisterRenter(renter);
        }

        static void RentVehicle(
            RentalService service, ref int rentalCount)
        {
            Console.Write("Enter renter ID: ");

            if (!int.TryParse(Console.ReadLine(), out int renterId))
            {
                Console.WriteLine("Invalid renter ID.");
                return;
            }

            Renter? renter = service.GetRenter(renterId);

            if (renter == null)
            {
                Console.WriteLine("Renter not found.");
                return;
            }

            Console.Write("Enter vehicle registration: ");
            string registration =
                (Console.ReadLine() ?? "").Trim();

            if (!service.TryGetVehicle(
                registration, out Vehicle? vehicle))
            {
                Console.WriteLine("Vehicle not found.");
                return;
            }

            if (!vehicle!.IsAvailable)
            {
                Console.WriteLine("Vehicle is already rented.");
                return;
            }

            DateTime startDate = ReadDate("Enter start date (yyyy-MM-dd): ");
            DateTime endDate = ReadDate(
                "Enter expected return date (yyyy-MM-dd): ");

            if (endDate <= startDate)
            {
                Console.WriteLine(
                    "Return date must be after start date.");
                return;
            }

            RentalPeriod period;

            try
            {
                period = new RentalPeriod(startDate, endDate);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            RentalContract? contract = service.RentVehicle(
                registration, renter, period, ref rentalCount);

            if (contract == null)
                return;

            decimal charge = contract.CalculateRentalCharge();
            decimal deposit = contract.CalculateDeposit();
            decimal total = charge + deposit;

            Console.WriteLine("\n===== RENTAL BILL =====");
            Console.WriteLine($"Contract ID: {contract.ContractId}");
            Console.WriteLine($"Renter: {renter.Name}");
            Console.WriteLine($"Vehicle: {vehicle.Model}");
            Console.WriteLine($"Rental days: {period.Days}");
            Console.WriteLine($"Rental charge: ₹{charge:F2}");
            Console.WriteLine($"Deposit: ₹{deposit:F2}");
            Console.WriteLine($"Initial amount: ₹{total:F2}");

            // Demonstrate record, with, and equality
            RentalSummary summary = new RentalSummary(
                contract.ContractId,
                vehicle.Model,
                charge,
                deposit,
                total);

            RentalSummary copy = summary with { };

            Console.WriteLine(
                $"Record equality test: {summary == copy}");

            RentalSummary updated = summary with
            {
                Total = summary.Total + 100m
            };

            Console.WriteLine(
                $"Example updated total using with: ₹{updated.Total:F2}");
        }

        static void ReturnVehicle(RentalService service)
        {
            Console.Write("Enter contract ID (example R001): ");
            string contractId =
                (Console.ReadLine() ?? "").Trim();

            if (string.IsNullOrWhiteSpace(contractId) ||
                contractId.Length < 2 ||
                contractId[0] != 'R' ||
                !int.TryParse(contractId.Substring(1), out int id) ||
                id <= 0)
            {
                Console.WriteLine("Malformed contract ID.");
                return;
            }

            RentalContract? contract =
                service.GetContract(contractId);

            if (contract == null)
            {
                Console.WriteLine("Contract not found.");
                return;
            }

            if (contract.IsReturned)
            {
                Console.WriteLine("Vehicle already returned.");
                return;
            }

            DateTime actualReturn = ReadDate(
                "Enter actual return date (yyyy-MM-dd): ");

            Console.Write("Is the vehicle damaged? (yes/no): ");
            string answer =
                (Console.ReadLine() ?? "").Trim();

            if (!answer.Equals("yes",
                    StringComparison.OrdinalIgnoreCase) &&
                !answer.Equals("no",
                    StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Enter yes or no.");
                return;
            }

            bool damaged = answer.Equals(
                "yes", StringComparison.OrdinalIgnoreCase);

            Console.Write("Kilometres travelled: ");

            if (!decimal.TryParse(
                Console.ReadLine(), out decimal kilometres) ||
                kilometres < 0)
            {
                Console.WriteLine("Invalid mileage.");
                return;
            }

            try
            {
                var bill = service.ReturnVehicle(
                    contract, actualReturn, damaged, kilometres);

                Console.WriteLine("\n===== FINAL BILL =====");
                Console.WriteLine(
                    $"Rental charge: ₹{bill.RentalCharge:F2}");
                Console.WriteLine(
                    $"Late fee: ₹{bill.LateFee:F2}");
                Console.WriteLine(
                    $"Damage fee: ₹{bill.DamageFee:F2}");
                Console.WriteLine(
                    $"Mileage fee: ₹{bill.MileageFee:F2}");
                Console.WriteLine(
                    $"Total charges: ₹{bill.FinalCharges:F2}");
                Console.WriteLine(
                    $"Deposit held: ₹{bill.Deposit:F2}");
                Console.WriteLine(
                    $"Deposit refund: ₹{bill.DepositRefund:F2}");
                Console.WriteLine(
                    $"Additional amount due: ₹{bill.AdditionalDue:F2}");

                if (bill.DepositRefund > 0)
                {
                    object processor = new PaymentProcessor();

                    if (processor is IPaymentProcessor payment)
                    {
                        // Explicit interface implementation
                        payment.RefundPayment(bill.DepositRefund);
                    }
                }

                if (bill.AdditionalDue > 0)
                {
                    object processor = new PaymentProcessor();

                    if (processor is IPaymentProcessor payment)
                    {
                        payment.ProcessPayment(bill.AdditionalDue);
                    }
                }

                Console.WriteLine(
                    "Vehicle returned successfully.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void SimulatePayment()
        {
            Console.Write("Enter payment amount: ");

            if (!decimal.TryParse(
                Console.ReadLine(), out decimal amount) ||
                amount <= 0)
            {
                Console.WriteLine("Enter a valid positive amount.");
                return;
            }

            object processor = new PaymentProcessor();

            // Safe pattern matching
            if (processor is IPaymentProcessor payment)
            {
                payment.ProcessPayment(amount);
            }
            else
            {
                Console.WriteLine("Payment processor unavailable.");
            }
        }

        static void ViewSummary(RentalService service)
        {
            Console.Write("Enter contract ID: ");
            string contractId =
                (Console.ReadLine() ?? "").Trim();

            RentalContract? contract =
                service.GetContract(contractId);

            if (contract == null)
            {
                Console.WriteLine("Contract not found.");
                return;
            }

            RentalSummaryInfo info = new RentalSummaryInfo(
                contract.ContractId,
                contract.CalculateRentalCharge(),
                contract.CalculateDeposit());

            DisplaySummary(in info);

            // Demonstrate value-type vs reference-type assignment
            int first = 10;
            int second = first;
            second = 20;

            Console.WriteLine($"Value type: first = {first}");
            Console.WriteLine($"Value type: second = {second}");

            Vehicle original = new Vehicle(
                "TEMP001", "Test Car", "Car", 1000m);

            Vehicle referenceCopy = original;
            referenceCopy.Model = "Updated Car";

            Console.WriteLine(
                $"Reference type original model: {original.Model}");
        }

        static void DisplaySummary(in RentalSummaryInfo summary)
        {
            Console.WriteLine("\n===== RENTAL SUMMARY =====");
            Console.WriteLine($"Contract ID: {summary.ContractId}");
            Console.WriteLine(
                $"Rental charge: ₹{summary.RentalCharge:F2}");
            Console.WriteLine($"Deposit: ₹{summary.Deposit:F2}");
        }

        static DateTime ReadDate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);

                if (DateTime.TryParse(
                    Console.ReadLine(), out DateTime date))
                {
                    return date.Date;
                }

                Console.WriteLine(
                    "Invalid date. Try again, for example 2026-10-13.");
            }
        }
    }
}