
using TrainingManagementWebAPI.Models;
using TrainingManagementWebAPI.Generics;

namespace TrainingManagementWebAPI.Services
{
    public class LearnerService
    {
        private readonly Repository<Learner> repository;

        public LearnerService(Repository<Learner> repository)
        {
            this.repository = repository;
        }

        public List<Learner> GetAllLearners()
        {
            return repository.GetAll();
        }

        public Learner? SearchLearner(string id)
        {
            return repository.Find(l =>
                l.LearnerId.Equals(
                    id, StringComparison.OrdinalIgnoreCase));
        }

        public string AddLearner(Learner learner)
        {
            if (string.IsNullOrWhiteSpace(learner.LearnerId) ||
                string.IsNullOrWhiteSpace(learner.Name))
            {
                return "Learner ID and name are required.";
            }

            if (learner.ContactNumber.Length != 10 ||
                !learner.ContactNumber.All(char.IsDigit))
            {
                return "Contact number must contain 10 digits.";
            }

            if (!learner.Email.Contains('@') ||
                !learner.Email.Contains('.'))
            {
                return "Enter a valid email address.";
            }

            if (SearchLearner(learner.LearnerId) != null)
            {
                return "Learner ID already exists.";
            }

            repository.Add(learner);
            return "Learner registered successfully.";
        }
    }
}
