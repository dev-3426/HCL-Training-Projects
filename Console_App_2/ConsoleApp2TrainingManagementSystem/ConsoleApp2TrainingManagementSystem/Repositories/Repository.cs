
using System.Collections.Generic;

namespace ConsoleApp2TrainingManagementSystem.Repositories
{
    public class Repository<T>
    {
        private readonly List<T> items = new List<T>();

        public void Add(T item)
        {
            items.Add(item);
        }

        public List<T> GetAll()
        {
            return new List<T>(items);
        }

        public int Count()
        {
            return items.Count;
        }
    }
}
