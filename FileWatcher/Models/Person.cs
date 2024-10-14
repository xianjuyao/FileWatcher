namespace FileWatcher.Models
{
    public class Person
    {
        public string Id { get; set; }
        public string Name { get; set; } 
        public string Age { get; set; }
        public string Address { get; set; } 
        public string Created { get; set; } 

        public Person(string id, string name, string age, string address, string created)
        {
            Id = id;
            Name = name;
            Age = age;
            Address = address;
            Created = created;
        }
    }
}