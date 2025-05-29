using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Text.Json;
using System.IO;

namespace Cursova
{
    public class MenuManager
    {
        private const string MENU_FILE_PATH = "menu_data.json";
        public List<MenuItemForOrder> AllMenuItems { get; set; } = new List<MenuItemForOrder>();

        public MenuManager()
        {
            LoadMenuData();
        }

        private void LoadMenuData()
        {
            if (File.Exists(MENU_FILE_PATH))
            {
                try
                {
                    string jsonString = File.ReadAllText(MENU_FILE_PATH);
                    var menuItems = JsonSerializer.Deserialize<List<MenuItemJson>>(jsonString);
                    AllMenuItems.Clear();
                    
                    foreach (var item in menuItems)
                    {
                        MenuItemForOrder menuItem = null;
                        switch (item.Category)
                        {
                            case "Блюда власної кухні":
                                menuItem = new Dish(item.Name, item.Price, item.Description, item.WeightGrams, item.Allergens);
                                break;
                            case "Напої":
                                menuItem = new Drink(item.Name, item.Price, item.Description, item.WeightGrams, item.Allergens);
                                break;
                            case "Десерти":
                                menuItem = new Dessert(item.Name, item.Price, item.Description, item.WeightGrams, item.Allergens);
                                break;
                        }
                        if (menuItem != null)
                        {
                            AllMenuItems.Add(menuItem);
                        }
                    }
                }
                catch
                {
                    LoadSampleMenuData(); // Если возникла ошибка при чтении файла, загружаем примеры
                }
            }
            else
            {
                LoadSampleMenuData(); // Если файл не существует, загружаем примеры
            }
        }

        private void SaveMenuData()
        {
            var menuItems = AllMenuItems.Select(item => new MenuItemJson
            {
                Name = item.Name,
                Price = item.Price,
                Description = item.Description,
                WeightGrams = item.WeightGrams,
                Allergens = item.Allergens,
                Category = item.GetCategory()
            }).ToList();

            string jsonString = JsonSerializer.Serialize(menuItems, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            File.WriteAllText(MENU_FILE_PATH, jsonString);
        }

        private class MenuItemJson
        {
            public string Name { get; set; }
            public decimal Price { get; set; }
            public string Description { get; set; }
            public double WeightGrams { get; set; }
            public List<string> Allergens { get; set; }
            public string Category { get; set; }
        }

        private void LoadSampleMenuData()
        {
            AllMenuItems.Clear();
            AllMenuItems.Add(new Dish("Борщ Український", 85.00m, "Традиційний український борщ з пампушками", 350, new List<string> { "М'ясо", "Буряк" }));
            AllMenuItems.Add(new Dish("Вареники з картоплею", 70.00m, "Домашні вареники зі сметаною", 250, new List<string> { "Борошно" }));
            AllMenuItems.Add(new Dish("Котлета по-київськи", 130.00m, "Соковита куряча котлета з маслом та зеленню", 300, new List<string> { "Курка", "Масло" }));

            AllMenuItems.Add(new Drink("Узвар", 25.00m, "Компот із сухофруктів", 200));
            AllMenuItems.Add(new Drink("Фреш Апельсиновий", 60.00m, "Свіжовижатий апельсиновий сік", 300));
            AllMenuItems.Add(new Drink("Кава Еспресо", 40.00m, "Міцна кава", 50));

            AllMenuItems.Add(new Dessert("Наполеон", 90.00m, "Класичний торт з листкового тіста", 150, new List<string> { "Молоко", "Яйця" }));
            AllMenuItems.Add(new Dessert("Чізкейк", 100.00m, "Ніжний сирний пиріг", 180, new List<string> { "Сир", "Молоко" }));
            AllMenuItems.Add(new Dessert("Морозиво", 55.00m, "Пломбір з джемом", 100, new List<string> { "Молоко" }));
            SaveMenuData(); // Сохраняем примеры в файл
        }

        public List<MenuItemForOrder> GetItemsByCategory(string category)
        {
            return AllMenuItems.Where(item => item.GetCategory() == category).ToList();
        }

        public void AddMenuItem(MenuItemForOrder item)
        {
            AllMenuItems.Add(item);
            SaveMenuData();
        }

        public void RemoveMenuItem(MenuItemForOrder item)
        {
            AllMenuItems.Remove(item);
            SaveMenuData();
        }

        public void UpdateMenuItem(MenuItemForOrder oldItem, MenuItemForOrder newItem)
        {
            int index = AllMenuItems.IndexOf(oldItem);
            if (index != -1)
            {
                AllMenuItems[index] = newItem;
                SaveMenuData();
            }
        }
    }
}
