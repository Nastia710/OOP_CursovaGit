using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Cursova
{
    public partial class EditOrderWindow : Window
    {
        private Order _currentOrder;
        
        private Order _originalOrderCopy;

        public EditOrderWindow(Order orderToEdit)
        {
            InitializeComponent();
            _currentOrder = orderToEdit;
            _originalOrderCopy = DeepCopyOrder(orderToEdit);

            OrderIdTextBlock.Text = $"Замовлення №{_currentOrder.OrderId}";
            DisplayOrderItems();
            UpdateTotalCost();
        }

        private Order DeepCopyOrder(Order original)
        {
            Order copy = new Order(original.TableNumber)
            {
                OrderId = original.OrderId,
                Status = original.Status,
            };
            
            foreach (var item in original.Items)
            {
                copy.Items.Add(new OrderItem(item.Item, item.Quantity, item.Notes));
            }
            copy.CalculateTotalCost();
            return copy;
        }

        public void DisplayOrderItems()
        {
            OrderItemsStackPanel.Children.Clear();

            if (_currentOrder.Items.Count == 0)
            {
                OrderItemsStackPanel.Children.Add(new TextBlock
                {
                    Text = "Замовлення порожнє. Додайте позиції.",
                    FontSize = 16,
                    Foreground = Brushes.Gray,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 50, 0, 0)
                });
                UpdateTotalCost();
                return;
            }

            foreach (var orderItem in _currentOrder.Items)
            {
                Border itemBorder = new Border
                {
                    BorderBrush = Brushes.LightGray,
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    Padding = new Thickness(0, 10, 0, 10),
                    Margin = new Thickness(0, 0, 0, 5)
                };

                Grid itemGrid = new Grid();
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                StackPanel itemDetailsPanel = new StackPanel();

                TextBlock namePriceTextBlock = new TextBlock
                {
                    Text = $"{orderItem.Item.Name} - {orderItem.Item.Price:C}",
                    FontSize = 16,
                    FontWeight = FontWeights.SemiBold
                };
                itemDetailsPanel.Children.Add(namePriceTextBlock);

                TextBlock descriptionWeightTextBlock = new TextBlock
                {
                    Text = $"{orderItem.Item.Description} ({orderItem.Item.WeightGrams}г)",
                    FontSize = 14,
                    Foreground = Brushes.Gray,
                    Margin = new Thickness(0, 2, 0, 0)
                };
                itemDetailsPanel.Children.Add(descriptionWeightTextBlock);

                if (orderItem.Item.Allergens != null && orderItem.Item.Allergens.Any())
                {
                    TextBlock allergensTextBlock = new TextBlock
                    {
                        Text = $"Алергени: {string.Join(", ", orderItem.Item.Allergens)}",
                        FontSize = 12,
                        Foreground = Brushes.Red,
                        Margin = new Thickness(0, 2, 0, 0)
                    };
                    itemDetailsPanel.Children.Add(allergensTextBlock);
                }
                if (!string.IsNullOrEmpty(orderItem.Notes))
                {
                    TextBlock notesTextBlock = new TextBlock
                    {
                        Text = $"Нотатки: {orderItem.Notes}",
                        FontSize = 12,
                        Foreground = Brushes.DarkBlue,
                        FontStyle = FontStyles.Italic,
                        Margin = new Thickness(0, 2, 0, 0)
                    };
                    itemDetailsPanel.Children.Add(notesTextBlock);
                }

                Grid.SetColumn(itemDetailsPanel, 0);
                itemGrid.Children.Add(itemDetailsPanel);

                StackPanel quantityControlPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(10, 0, 0, 0)
                };

                Button minusButton = new Button
                {
                    Content = "-",
                    Style = (Style)FindResource("QuantityButtonStyle"),
                    Tag = orderItem
                };
                minusButton.Click += QuantityMinusButton_Click;
                quantityControlPanel.Children.Add(minusButton);

                TextBox quantityTextBox = new TextBox
                {
                    Text = orderItem.Quantity.ToString(),
                    Style = (Style)FindResource("NumericUpDownTextBoxStyle"),
                    Tag = orderItem
                };

                quantityTextBox.PreviewTextInput += NumericTextBox_PreviewTextInput;
              
                quantityTextBox.LostFocus += QuantityTextBox_LostFocus;
                quantityControlPanel.Children.Add(quantityTextBox);

                Button plusButton = new Button
                {
                    Content = "+",
                    Style = (Style)FindResource("QuantityButtonStyle"),
                    Tag = orderItem
                };
                plusButton.Click += QuantityPlusButton_Click;
                quantityControlPanel.Children.Add(plusButton);

                Grid.SetColumn(quantityControlPanel, 1);
                itemGrid.Children.Add(quantityControlPanel);

                TextBlock itemTotalPriceTextBlock = new TextBlock
                {
                    Text = $"{orderItem.TotalPrice:C}",
                    FontSize = 16,
                    FontWeight = FontWeights.SemiBold,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(10, 0, 0, 0)
                };
                Grid.SetColumn(itemTotalPriceTextBlock, 2);
                itemGrid.Children.Add(itemTotalPriceTextBlock);

                Button removeButton = new Button
                {
                    Content = "Видалити",
                    Style = (Style)FindResource("RemoveItemButtonStyle"),
                    Tag = orderItem,
                    VerticalAlignment = VerticalAlignment.Center
                };
                removeButton.Click += RemoveItemButton_Click;
                Grid.SetColumn(removeButton, 3);
                itemGrid.Children.Add(removeButton);

                itemBorder.Child = itemGrid;
                OrderItemsStackPanel.Children.Add(itemBorder);
            }
            UpdateTotalCost();
        }

        private void UpdateTotalCost()
        {
            _currentOrder.CalculateTotalCost();
            TotalOrderCostTextBlock.Text = $"Загальна вартість: {_currentOrder.TotalCost:C}";
        }

        private void QuantityPlusButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            OrderItem item = button.Tag as OrderItem;
            if (item != null)
            {
                item.Quantity++;
                DisplayOrderItems();
            }
        }

        private void QuantityMinusButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            OrderItem item = button.Tag as OrderItem;
            if (item != null)
            {
                if (item.Quantity > 1)
                {
                    item.Quantity--;
                    DisplayOrderItems();
                }
                else if (item.Quantity == 1)
                {
                    MessageBoxResult result = MessageBox.Show($"Видалити \"{item.Item.Name}\" з замовлення?",
                                                              "Видалити позицію",
                                                              MessageBoxButton.YesNo,
                                                              MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        _currentOrder.RemoveItem(item);
                        DisplayOrderItems();
                    }
                }
            }
        }

        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        private void QuantityTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            OrderItem item = textBox.Tag as OrderItem;
            if (item != null)
            {
                if (int.TryParse(textBox.Text, out int newQuantity) && newQuantity > 0)
                {
                    item.Quantity = newQuantity;
                }
                else
                {
                    textBox.Text = item.Quantity.ToString();
                    MessageBox.Show("Кількість повинна бути цілим числом більше нуля.", "Некоректне значення", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                DisplayOrderItems();
            }
        }

        /*private void QuantityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }*/

        private void RemoveItemButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            OrderItem itemToRemove = button.Tag as OrderItem;

            if (itemToRemove != null)
            {
                MessageBoxResult result = MessageBox.Show($"Ви впевнені, що хочете видалити \"{itemToRemove.Item.Name}\" з замовлення?",
                                                          "Видалити позицію",
                                                          MessageBoxButton.YesNo,
                                                          MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _currentOrder.RemoveItem(itemToRemove);
                    DisplayOrderItems();
                }
            }
        }

        private void AddPositionButton_Click(object sender, RoutedEventArgs e)
        {
            MenuWindow menuWindow = new MenuWindow(_currentOrder, this);
            menuWindow.ShowDialog();

            
        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            
            DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            
            _currentOrder.Items.Clear();
            foreach (var item in _originalOrderCopy.Items)
            {
                _currentOrder.Items.Add(new OrderItem(item.Item, item.Quantity, item.Notes));
            }
            _currentOrder.Status = _originalOrderCopy.Status;
            _currentOrder.CalculateTotalCost();

            DialogResult = false;
            this.Close();
        }
    }
}