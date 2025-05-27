using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Cursova
{
    public class MenuManager
    {
        public List<MenuItemForOrder> AllMenuItems { get; set; } = new List<MenuItemForOrder>();

        public MenuManager()
        {
            LoadSampleMenuData();
        }

        private void LoadSampleMenuData()
        {
            AllMenuItems.Add(new Dish("Борщ Український", 85.00m, "Традиційний український борщ з пампушками", 350, new List<string> { "М'ясо", "Буряк" }));
            AllMenuItems.Add(new Dish("Вареники з картоплею", 70.00m, "Домашні вареники зі сметаною", 250, new List<string> { "Борошно" }));
            AllMenuItems.Add(new Dish("Котлета по-київськи", 130.00m, "Соковита куряча котлета з маслом та зеленню", 300, new List<string> { "Курка", "Масло" }));

            AllMenuItems.Add(new Drink("Узвар", 25.00m, "Компот із сухофруктів", 200));
            AllMenuItems.Add(new Drink("Фреш Апельсиновий", 60.00m, "Свіжовижатий апельсиновий сік", 300));
            AllMenuItems.Add(new Drink("Кава Еспресо", 40.00m, "Міцна кава", 50));

            AllMenuItems.Add(new Dessert("Наполеон", 90.00m, "Класичний торт з листкового тіста", 150, new List<string> { "Молоко", "Яйця" }));
            AllMenuItems.Add(new Dessert("Чізкейк", 100.00m, "Ніжний сирний пиріг", 180, new List<string> { "Сир", "Молоко" }));
            AllMenuItems.Add(new Dessert("Морозиво", 55.00m, "Пломбір з джемом", 100, new List<string> { "Молоко" }));
        }

        public List<MenuItemForOrder> GetItemsByCategory(string category)
        {
            return AllMenuItems.Where(item => item.GetCategory() == category).ToList();
        }

        public void AddMenuItem(MenuItemForOrder item)
        {
            AllMenuItems.Add(item);
            // !save to files!
        }

        public void RemoveMenuItem(MenuItemForOrder item)
        {
            AllMenuItems.Remove(item);
            // !save to files!
        }

        public void UpdateMenuItem(MenuItemForOrder oldItem, MenuItemForOrder newItem)
        {
            int index = AllMenuItems.IndexOf(oldItem);
            if (index != -1)
            {
                AllMenuItems[index] = newItem;
                // !save to files!
            }
        }
    }
}
