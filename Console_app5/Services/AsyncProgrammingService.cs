
using Console_app5.Models;
using Console_app5.Services;

class Program
{
    static async Task Main(string[] args)
    {
        AsyncProgrammingService service =
            new AsyncProgrammingService();

        List<PracticeOption> options = new List<PracticeOption>
        {
            new PracticeOption
            {
                Id = 1,
                Name = "Basic Async / Await",
                Description = "Learn how await works"
            },
            new PracticeOption
            {
                Id = 2,
                Name = "Task<T> and Task Tracking",
                Description = "Return values and check task status"
            },
            new PracticeOption
            {
                Id = 3,
                Name = "Sequential Execution",
                Description = "Run operations one after another"
            },
            new PracticeOption
            {
                Id = 4,
                Name = "Concurrent Execution",
                Description = "Run multiple operations together"
            },
            new PracticeOption
            {
                Id = 5,
                Name = "Task.WhenAll",
                Description = "Wait for all tasks and collect results"
            },
            new PracticeOption
            {
                Id = 6,
                Name = "Task.WhenAny",
                Description = "Identify the first completed task"
            },
            new PracticeOption
            {
                Id = 7,
                Name = "Exception Handling",
                Description = "Handle asynchronous errors"
            },
            new PracticeOption
            {
                Id = 8,
                Name = "Cancellation Token",
                Description = "Cancel an asynchronous operation"
            }
        };

        bool running = true;

        while (running)
        {
            Console.Clear();

            Console.WriteLine("==================================");
            Console.WriteLine(" ASYNC PROGRAMMING PRACTICE");
            Console.WriteLine("==================================");

            foreach (PracticeOption option in options)
            {
                Console.WriteLine(
                    $"{option.Id}. {option.Name}");

                Console.WriteLine(
                    $"   {option.Description}");
            }

            Console.WriteLine("0. Exit");
            Console.WriteLine("==================================");
            Console.Write("Enter your choice: ");

            string? input = Console.ReadLine();

            if (!int.TryParse(input, out int choice))
            {
                Console.WriteLine(
                    "Please enter a valid number.");

                Console.WriteLine(
                    "Press any key to continue...");

                Console.ReadKey();
                continue;
            }

            if (choice == 0)
            {
                running = false;
                continue;
            }

            if (!options.Exists(option => option.Id == choice))
            {
                Console.WriteLine("Invalid choice.");

                Console.WriteLine(
                    "Press any key to continue...");

                Console.ReadKey();
                continue;
            }

            Console.Clear();

            Console.WriteLine(
                $"Running: {options.Find(
                    option => option.Id == choice)!.Name}");

            Console.WriteLine("----------------------------------");

            try
            {
                await service.RunPracticeAsync(choice);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine(
                    "The operation was cancelled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"An unexpected error occurred: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine(
                "Press any key to return to the menu...");

            Console.ReadKey();
        }

        Console.WriteLine("Async practice application closed.");
    }
}
