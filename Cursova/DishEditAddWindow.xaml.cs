using Cursova;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cursova
{
    public partial class DishEditAddWindow : Window
    {
        private MenuItemForOrder _itemToEdit;
        private MenuManager _menuManager;
        public DishEditAddWindow(MenuItemForOrder itemToEdit, MenuManager menuManager)
        {
            InitializeComponent();
            _itemToEdit = itemToEdit;
            _menuManager = menuManager;
            LoadItemData();
        }

        private void LoadItemData()
        {
            if (_itemToEdit == null)
            {
                WindowTitleTextBlock.Text = "Додати нову страву";
            }
            else
            {
                WindowTitleTextBlock.Text = $"Редагувати страву: {_itemToEdit.Name}";

                NameTextBox.Text = _itemToEdit.Name;
                PriceTextBox.Text = _itemToEdit.Price.ToString("F2");
                DescriptionTextBox.Text = _itemToEdit.Description;
                WeightGramsTextBox.Text = _itemToEdit.WeightGrams.ToString();
                AllergensTextBox.Text = string.Join(", ", _itemToEdit.Allergens);

                switch (_itemToEdit.GetCategory())
                {
                    case "Блюда власної кухні":
                        CategoryComboBox.SelectedIndex = 0;
                        break;
                    case "Напої":
                        CategoryComboBox.SelectedIndex = 1;
                        break;
                    case "Десерти":
                        CategoryComboBox.SelectedIndex = 2;
                        break;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Будь ласка, введіть назву страви.", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Будь ласка, введіть коректну ціну (число більше 0).", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!double.TryParse(WeightGramsTextBox.Text, out double weightGrams) || weightGrams <= 0)
            {
                MessageBox.Show("Будь ласка, введіть коректну вагу/об'єм (число більше 0).", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string name = NameTextBox.Text.Trim();
            string description = DescriptionTextBox.Text.Trim();
            List<string> allergens = AllergensTextBox.Text.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                                                         .Select(a => a.Trim())
                                                         .Where(a => !string.IsNullOrEmpty(a))
                                                         .ToList();
            string category = (CategoryComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            MenuItemForOrder newItem;

            switch (category)
            {
                case "Блюда власної кухні":
                    newItem = new Dish(name, price, description, weightGrams, allergens);
                    break;
                case "Напої":
                    newItem = new Drink(name, price, description, weightGrams, allergens);
                    break;
                case "Десерти":
                    newItem = new Dessert(name, price, description, weightGrams, allergens);
                    break;
                default:
                    MessageBox.Show("Невірна категорія страви.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
            }

            if (_itemToEdit == null)
            {
                _menuManager.AddMenuItem(newItem);
                MessageBox.Show("Страву успішно додано до меню!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                _menuManager.UpdateMenuItem(_itemToEdit, newItem);
                MessageBox.Show("Страву успішно оновлено в меню!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            
            if (char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = false;
            }
            
            else if ((e.Text == "." || e.Text == ",") && !textBox.Text.Contains(".") && !textBox.Text.Contains(","))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}