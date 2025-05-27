using System.Collections.Generic;

namespace Cursova
{
    public class Dish : MenuItemForOrder
    {
        public Dish(string name, decimal price, string description, double weightGrams, List<string> allergens = null)
            : base(name, price, description, weightGrams, allergens)
        {
        }

        public override string GetCategory()
        {
            return "Блюда власної кухні";
        }
    }
}