
using ConsoleApp2TrainingManagementSystem.Exceptions;
using ConsoleApp2TrainingManagementSystem.Models;
using ConsoleApp2TrainingManagementSystem.Services;
using ConsoleApp2TrainingManagementSystem.Payments;
using ConsoleApp2TrainingManagementSystem.Repositories;

namespace ConsoleApp2TrainingManagementSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Create services once so they share the same data
            CourseService courseService = new CourseService();
            CourseFilterService filterService =
    new CourseFilterService();

            Console.WriteLine("\nCourses above Rs.5000:");

            filterService.FilterCourses(
                courseService.GetCourses(),
                course => course.Fee > 5000m);

            //Repo
            Repository<Course> courseRepository =
                new Repository<Course>();

            Course course1 = new Course(
                "CS101", "C# Programming", 5000m, 30, 40);

            Course course2 = new Course(
                "PY101", "Python Programming", 4500m, 25, 35);

            courseRepository.Add(course1);
            courseRepository.Add(course2);

            Console.WriteLine(
                $"Total courses in repository: {courseRepository.Count()}");

            foreach (Course course in courseRepository.GetAll())
            {
                Console.WriteLine(course.Title);
            }//Repo

            LearnerService learnerService = new LearnerService();

            EnrollmentService enrollmentService =
                new EnrollmentService(learnerService, courseService);

            IPaymentProcessor paymentProcessor =
    new OnlinePaymentProcessor();

            PaymentService paymentService =
                new PaymentService(
                    enrollmentService, paymentProcessor);

            bool running = true;

            while (running)
            {
                Console.WriteLine("\n======================================");
                Console.WriteLine("  TRAINING CENTRE MANAGEMENT SYSTEM");
                Console.WriteLine("======================================");
                Console.WriteLine("1.  Display all courses");
                Console.WriteLine("2.  Search course by code");
                Console.WriteLine("3.  Search course by title");
                Console.WriteLine("4.  Sort courses by fee (ascending)");
                Console.WriteLine("5.  Sort courses by fee (descending)");
                Console.WriteLine("6.  Register learner");
                Console.WriteLine("7.  Display all learners");
                Console.WriteLine("8.  Search learner by ID");
                Console.WriteLine("9.  Enroll learner");
                Console.WriteLine("10. Display enrollments");
                Console.WriteLine("11. Search enrollment by ID");
                Console.WriteLine("12. Cancel enrollment");
                Console.WriteLine("13. Make payment");
                Console.WriteLine("14. Display all payments");
                Console.WriteLine("0.  Exit");
                Console.WriteLine("======================================");
                Console.Write("Enter your choice: ");

                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        courseService.DisplayCourses();
                        break;

                    case "2":
                        Console.Write("Enter course code: ");
                        courseService.SearchByCode(
                            Console.ReadLine() ?? "");
                        break;

                    case "3":
                        Console.Write("Enter course title or keyword: ");
                        courseService.SearchByTitle(
                            Console.ReadLine() ?? "");
                        break;

                    case "4":
                        courseService.SortByFee(true);
                        break;

                    case "5":
                        courseService.SortByFee(false);
                        break;

                    case "6":
                        Console.Write("Enter learner ID: ");
                        string learnerId = Console.ReadLine() ?? "";

                        Console.Write("Enter learner name: ");
                        string name = Console.ReadLine() ?? "";

                        Console.Write("Enter contact number (10 digits): ");
                        string contact = Console.ReadLine() ?? "";

                        Console.Write("Enter email: ");
                        string email = Console.ReadLine() ?? "";

                        Learner learner = new Learner(
                            learnerId, name, contact, email);

                        learnerService.RegisterLearner(learner);
                        break;

                    case "7":
                        learnerService.DisplayAllLearners();
                        break;

                    case "8":
                        Console.Write("Enter learner ID: ");
                        string searchLearnerId = Console.ReadLine() ?? "";

                        try
                        {
                            learnerService.SearchLearner(searchLearnerId);
                        }
                        catch (InvalidLearnerIdException ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }

                        break;


                    case "9":
                        Console.Write("Enter learner ID: ");
                        string enrollLearnerId = Console.ReadLine() ?? "";

                        Console.Write("Enter course code: ");
                        string enrollCourseCode = Console.ReadLine() ?? "";

                        enrollmentService.EnrollLearner(
                            enrollLearnerId, enrollCourseCode);
                        break;

                    case "10":
                        enrollmentService.DisplayAllEnrollments();
                        break;

                    case "11":
                        Console.Write("Enter enrollment ID: ");

                        if (int.TryParse(
                            Console.ReadLine(), out int searchId))
                        {
                            enrollmentService.SearchEnrollment(searchId);
                        }
                        else
                        {
                            Console.WriteLine("Enter a valid numeric ID.");
                        }
                        break;

                    case "12":
                        Console.Write("Enter enrollment ID to cancel: ");

                        if (int.TryParse(
                            Console.ReadLine(), out int cancelId))
                        {
                            enrollmentService.CancelEnrollment(cancelId);
                        }
                        else
                        {
                            Console.WriteLine("Enter a valid numeric ID.");
                        }
                        break;

                    case "13":
                        Console.Write("Enter enrollment ID: ");

                        if (int.TryParse(Console.ReadLine(), out int paymentEnrollmentId))
                        {
                            paymentService.MakePayment(paymentEnrollmentId);
                        }
                        else
                        {
                            Console.WriteLine("Enter a valid numeric ID.");
                        }

                        break;

                    case "14":
                        paymentService.DisplayAllPayments();
                        break;

                    case "0":
                        running = false;
                        Console.WriteLine("Thank you. Goodbye!");
                        break;

                    default:
                        Console.WriteLine(
                            "Invalid choice. Enter a number from the menu.");
                        break;
                }
            }
        }
    }
}
