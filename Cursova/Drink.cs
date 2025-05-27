using System.Collections.Generic;
using System.Windows.Controls;

namespace Cursova
{
    public class Drink : MenuItemForOrder
    {
        public Drink(string name, decimal price, string description, double weightGrams, List<string> allergens = null)
            : base(name, price, description, weightGrams, allergens)
        {
        }

        public override string GetCategory()
        {
            return "Напої";
        }
    }
}