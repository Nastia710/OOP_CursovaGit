using System.Collections.Generic;

namespace Cursova
{
    public abstract class MenuItemForOrder
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public double WeightGrams { get; set; }
        public List<string> Allergens { get; set; } = new List<string>();

        
        public MenuItemForOrder(string name, decimal price, string description, double weightGrams, List<string> allergens = null)
        {
            Name = name;
            Price = price;
            Description = description;
            WeightGrams = weightGrams;
            if (allergens != null)
            {
                Allergens = new List<string>(allergens);
            }
        }

        public abstract string GetCategory();

        public override string ToString()
        {
            return $"{Name} - {Price:C}";
        }
    }
}