
using System;

namespace ConsoleApp2TrainingManagementSystem.Exceptions
{
    public class InvalidLearnerIdException : Exception
    {
        public InvalidLearnerIdException(string message)
            : base(message)
        {
        }
    }
}
