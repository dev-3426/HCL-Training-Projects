
using System;
class Program
{
    public static void Main()
    {
        int orderId;
        string customerName;
        string productName;
        decimal quantity;
        decimal unitPrice;
        decimal subtotal = 0;


        while (true)
        {
            Console.Write("Enter Order ID: ");
            string input = Console.ReadLine() ?? "";

            if (int.TryParse(input, out orderId) && orderId > 0)
            {
                break;
            }

            Console.WriteLine("Error: Order ID must be a positive integer.");
        }


        while (true)
        {
            Console.Write("Enter Customer Name: ");
            customerName = Console.ReadLine() ?? "";

            if (!string.IsNullOrWhiteSpace(customerName))
            {
                break;
            }

            Console.WriteLine("Error: Customer name cannot be empty.");
        }

        while (true)
        {
            Console.Write("Enter Product Name: ");
            productName = Console.ReadLine() ?? "";

            if (!string.IsNullOrWhiteSpace(productName))
            {
                break;
            }
            Console.WriteLine("Error: Product name cannot be empty");
        }

        Console.WriteLine("Enter a quantity: ");
        bool quan = decimal.TryParse(Console.ReadLine(), out quantity);
        while (!quan || quantity <= 0)
        {
            Console.WriteLine("Invalid quantity, enter valid quantity");
            quan = decimal.TryParse(Console.ReadLine(), out quantity);
        }

        Console.WriteLine("Enter a UnitPrice: ");
        bool un = decimal.TryParse(Console.ReadLine(), out unitPrice);
        while (!un || unitPrice <= 0)
        {
            Console.WriteLine("Invalid quantity, enter valid quantity");
            un = decimal.TryParse(Console.ReadLine(), out unitPrice);
        }

        subtotal = unitPrice * quantity;

        Console.WriteLine(" ========== ORDER SUMMARY ========== ");
        Console.WriteLine($"Order ID: {orderId}");
        Console.WriteLine($"Customer Name: {customerName}");
        Console.WriteLine($"Product Name:{productName} ");
        Console.WriteLine($"Quantity:{quantity:F2} ");
        Console.WriteLine($"Unit Price:{unitPrice:F2} ");
        Console.WriteLine($"Subtotal:{subtotal:F2} ");









    }
}
