
using ConsoleApp2TrainingManagementSystem.Exceptions;
using ConsoleApp2TrainingManagementSystem.Models;

namespace ConsoleApp2TrainingManagementSystem.Services
{
    public class LearnerService
    {
        private readonly Dictionary<string, Learner> learners =
            new Dictionary<string, Learner>(
                StringComparer.OrdinalIgnoreCase);

        public bool RegisterLearner(Learner learner)
        {
            string id = learner.LearnerId.Trim();

            if (id == "")
            {
                Console.WriteLine("Learner ID cannot be empty.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(learner.Name))
            {
                Console.WriteLine("Learner name cannot be empty.");
                return false;
            }

            if (!IsValidContact(learner.ContactNumber))
            {
                Console.WriteLine(
                    "Contact number must contain exactly 10 digits.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(learner.Email) ||
                !learner.Email.Contains('@') ||
                !learner.Email.Contains('.'))
            {
                Console.WriteLine("Enter a valid email format.");
                return false;
            }

            if (learners.ContainsKey(id))
            {
                Console.WriteLine("This learner ID already exists.");
                return false;
            }

            learner.LearnerId = id;
            learners.Add(id, learner);

            Console.WriteLine("Learner registered successfully.");
            return true;
        }

        private bool IsValidContact(string contact)
        {
            if (string.IsNullOrWhiteSpace(contact) ||
                contact.Length != 10)
            {
                return false;
            }

            for (int i = 0; i < contact.Length; i++)
            {
                if (contact[i] < '0' || contact[i] > '9')
                {
                    return false;
                }
            }

            return true;
        }

        public bool TryGetLearner(string id, out Learner learner)
        {
            return learners.TryGetValue(id.Trim(), out learner);
        }


        public void SearchLearner(string id)
        {
            id = id?.Trim() ?? "";

            if (id == "")
            {
                throw new InvalidLearnerIdException(
                    "Learner ID cannot be empty.");
            }

            if (learners.TryGetValue(id, out Learner learner))
            {
                DisplayOneLearner(learner);
            }
            else
            {
                throw new InvalidLearnerIdException(
                    $"Learner ID '{id}' does not exist.");
            }
        }


        public void DisplayAllLearners()
        {
            if (learners.Count == 0)
            {
                Console.WriteLine("No learners registered.");
                return;
            }

            foreach (Learner learner in learners.Values)
            {
                DisplayOneLearner(learner);
            }
        }

        private void DisplayOneLearner(Learner learner)
        {
            Console.WriteLine("------------------------------");
            Console.WriteLine($"ID: {learner.LearnerId}");
            Console.WriteLine($"Name: {learner.Name}");
            Console.WriteLine($"Contact: {learner.ContactNumber}");
            Console.WriteLine($"Email: {learner.Email}");
        }


        public bool RemoveLearner(string id)
        {
            id = id?.Trim() ?? "";

            if (id == "")
            {
                throw new InvalidLearnerIdException(
                    "Learner ID cannot be empty.");
            }

            if (learners.Remove(id))
            {
                Console.WriteLine("Learner removed.");
                return true;
            }

            throw new InvalidLearnerIdException(
                $"Cannot remove learner. ID '{id}' does not exist.");
        }

    }
}
