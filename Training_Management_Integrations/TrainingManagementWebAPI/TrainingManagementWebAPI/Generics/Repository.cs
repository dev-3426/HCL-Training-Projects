
namespace TrainingManagementWebAPI.Generics
{
    public class Repository<T>
    {
        private readonly List<T> items = new();

        public void Add(T item)
        {
            items.Add(item);
        }

        public List<T> GetAll()
        {
            return items.ToList();
        }

        public T? Find(Func<T, bool> condition)
        {
            return items.FirstOrDefault(condition);
        }

        public List<T> Filter(Func<T, bool> condition)
        {
            return items.Where(condition).ToList();
        }
    }
}
