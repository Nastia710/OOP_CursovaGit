using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Cursova
{
    public class Dessert : MenuItemForOrder
    {
        public Dessert(string name, decimal price, string description, double weightGrams, List<string> allergens = null)
            : base(name, price, description, weightGrams, allergens)
        {
        }

        public override string GetCategory()
        {
            return "Десерти";
        }
    }
}
