namespace Redis_Caching.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        public Product(int id, string name, string description, decimal price, int stock)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            Stock = stock;
        }

        public override string ToString()
        {
            return $"Product [Id={Id}, Name={Name}, Description={Description}, Price={Price}, Stock={Stock}]";
        }
    }
}