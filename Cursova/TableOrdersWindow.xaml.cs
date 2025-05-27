using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Cursova
{
    public enum OrderStatus
    {
        AwaitingConfirmation,
        Confirmed,
        Preparing,
        Ready,
        Completed
    }
    public class MenuItemWorking
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public double WeightGrams { get; set; }
        public List<string> Allergens { get; set; } = new List<string>();

        public MenuItemWorking(string name, decimal price, string description, double weightGrams, List<string> allergens = null)
        {
            Name = name;
            Price = price;
            Description = description;
            WeightGrams = weightGrams;
            if (allergens != null)
            {
                Allergens = allergens;
            }
        }

        public override string ToString()
        {
            return $"{Name} ({WeightGrams}г) - {Price:C}";
        }
    }

    public class OrderItem
    {
        public MenuItemWorking Item { get; set; }
        public int Quantity { get; set; }
        public string Notes { get; set; }

        public OrderItem(MenuItemWorking item, int quantity, string notes = "")
        {
            Item = item;
            Quantity = quantity;
            Notes = notes;
        }

        public decimal TotalPrice => Item.Price * Quantity;
    }

    public class Order
    {
        private static int _nextOrderId = 1;
        public int OrderId { get; set; }
        public int TableNumber { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        public decimal TotalCost { get; set; }

        public Order(int tableNumber)
        {
            OrderId = _nextOrderId++;
            TableNumber = tableNumber;
            Status = OrderStatus.AwaitingConfirmation;
            CalculateTotalCost();
        }

        public void AddItem(OrderItem item)
        {
            Items.Add(item);
            CalculateTotalCost();
        }

        public void RemoveItem(OrderItem item)
        {
            Items.Remove(item);
            CalculateTotalCost();
        }

        public void CalculateTotalCost()
        {
            TotalCost = Items.Sum(item => item.TotalPrice);
        }
    }


    public partial class TableOrdersWindow : Window
    {
        private int _tableNumber;
        private List<Order> _orders = new List<Order>();

        public TableOrdersWindow(int tableNumber)
        {
            InitializeComponent();
            _tableNumber = tableNumber;
            TableNumberTextBlock.Text = $"Замовлення для столика #{_tableNumber}";
            LoadOrdersForTable();
            DisplayOrders();
        }

        private void LoadOrdersForTable()
        {
            // !from file!
            _orders.Add(new Order(_tableNumber)
            {
                Status = OrderStatus.Confirmed,
                Items = new List<OrderItem>
                {
                    new OrderItem(new MenuItemWorking("Борщ Український", 85.00m, "Традиційний борщ", 350, new List<string>{"Буряк"}), 1),
                    new OrderItem(new MenuItemWorking("Узвар", 25.00m, "Компот із сухофруктів", 200), 2)
                }
            });
            _orders[0].CalculateTotalCost();

            _orders.Add(new Order(_tableNumber)
            {
                Status = OrderStatus.Preparing,
                Items = new List<OrderItem>
                {
                    new OrderItem(new MenuItemWorking("М'ясний салат", 120.00m, "Салат з куркою", 250), 1)
                }
            });
            _orders[1].CalculateTotalCost();
        }

        private void DisplayOrders()
        {
            OrdersStackPanel.Children.Clear(); 

            foreach (var order in _orders)
            {
                Border orderBorder = new Border
                {
                    BorderBrush = Brushes.LightGray,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(5),
                    Margin = new Thickness(0, 5, 0, 10),
                    Padding = new Thickness(10),
                    Background = Brushes.White
                };

                Grid orderGrid = new Grid();
                orderGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                orderGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                orderGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                orderGrid.ColumnDefinitions.Add(new ColumnDefinition { Width =GridLength.Auto });
                orderGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                TextBlock statusTextBlock = new TextBlock
                {
                    Text = $"Статус: {GetOrderStatusDisplayName(order.Status)}",
                    FontSize = 16,
                    FontWeight = FontWeights.SemiBold,
                    Margin = new Thickness(0, 0, 0, 5)
                };
                Grid.SetRow(statusTextBlock, 0);
                Grid.SetColumn(statusTextBlock, 0);
                orderGrid.Children.Add(statusTextBlock);

                Button threeDotsButton = new Button
                {
                    Content = "...",
                    Style = (Style)FindResource("ThreeDotsButtonStyle"),
                    Tag = order
                };
                Grid.SetRow(threeDotsButton, 0);
                Grid.SetColumn(threeDotsButton, 1);
                threeDotsButton.Click += ThreeDotsButton_Click;
                orderGrid.Children.Add(threeDotsButton);

                TextBlock totalCostTextBlock = new TextBlock
                {
                    Text = $"Загальна вартість: {order.TotalCost:C}",
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 5)
                };
                Grid.SetRow(totalCostTextBlock, 1);
                Grid.SetColumn(totalCostTextBlock, 0);
                Grid.SetColumnSpan(totalCostTextBlock, 2);
                orderGrid.Children.Add(totalCostTextBlock);

                // Список позицій замовлення
                StackPanel itemsStackPanel = new StackPanel();
                foreach (var item in order.Items)
                {
                    TextBlock itemTextBlock = new TextBlock
                    {
                        Text = $"- {item.Item.Name} x{item.Quantity} ({item.TotalPrice:C})",
                        FontSize = 14,
                        Margin = new Thickness(10, 0, 0, 2)
                    };
                    itemsStackPanel.Children.Add(itemTextBlock);
                }
                Grid.SetRow(itemsStackPanel, 2);
                Grid.SetColumn(itemsStackPanel, 0);
                Grid.SetColumnSpan(itemsStackPanel, 2);
                orderGrid.Children.Add(itemsStackPanel);

                orderBorder.Child = orderGrid;
                OrdersStackPanel.Children.Add(orderBorder);
            }
        }
        private string GetOrderStatusDisplayName(OrderStatus status)
        {
            switch (status)
            {
                case OrderStatus.AwaitingConfirmation: return "Очікує підтвердження";
                case OrderStatus.Confirmed: return "Підтверджено";
                case OrderStatus.Preparing: return "Готується";
                case OrderStatus.Ready: return "Готове";
                case OrderStatus.Completed: return "Завершене";
                default: return status.ToString();
            }
        }

        private void ThreeDotsButton_Click(object sender, RoutedEventArgs e)
        {
            Button threeDotsButton = sender as Button;
            Order clickedOrder = threeDotsButton.Tag as Order;

            if (clickedOrder != null)
            {
                ContextMenu contextMenu = new ContextMenu();
                contextMenu.PlacementTarget = threeDotsButton;
                contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;

                MenuItem changeStatusMenuItem = new MenuItem { Header = "Змінити статус замовлення" };
                changeStatusMenuItem.Click += (s, ev) => ShowChangeStatusDialog(clickedOrder);
                contextMenu.Items.Add(changeStatusMenuItem);

                MenuItem editOrderMenuItem = new MenuItem { Header = "Редагувати замовлення" };
                editOrderMenuItem.Click += (s, ev) => EditOrder(clickedOrder);
                contextMenu.Items.Add(editOrderMenuItem);

                MenuItem deleteOrderMenuItem = new MenuItem { Header = "Видалити замовлення" };
                deleteOrderMenuItem.Click += (s, ev) => DeleteOrder(clickedOrder);
                contextMenu.Items.Add(deleteOrderMenuItem);


                contextMenu.IsOpen = true;
            }
        }

        private void DeleteOrder(Order orderToDelete)
        {
            MessageBoxResult result = MessageBox.Show($"Ви впевнені, що хочете видалити замовлення №{orderToDelete.OrderId}?",
                                                      "Підтвердження видалення",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                _orders.Remove(orderToDelete);
                DisplayOrders();
                MessageBox.Show($"Замовлення №{orderToDelete.OrderId} видалено.");
            }
        }


        private void ShowChangeStatusDialog(Order order)
        {
            Window changeStatusDialog = new Window
            {
                Title = "Змінити статус",
                Width = 300,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize
            };

            StackPanel panel = new StackPanel { Margin = new Thickness(10) };

            TextBlock promptText = new TextBlock
            {
                Text = $"Оберіть новий статус для замовлення №{order.OrderId}:",
                Margin = new Thickness(0, 0, 0, 10),
                TextWrapping = TextWrapping.Wrap
            };
            panel.Children.Add(promptText);

            ComboBox statusComboBox = new ComboBox();
            statusComboBox.ItemsSource = System.Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>();
            statusComboBox.SelectedItem = order.Status;
            panel.Children.Add(statusComboBox);

            StackPanel buttonsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 20, 0, 0)
            };

            Button confirmButton = new Button
            {
                Content = "Підтвердити",
                Padding = new Thickness(10, 5, 0,0),
                Margin = new Thickness(0, 0, 10, 0)
            };
            confirmButton.Click += (s, e) =>
            {
                OrderStatus newStatus = (OrderStatus)statusComboBox.SelectedItem;
                MessageBoxResult result = MessageBox.Show($"Точно змінити статус замовлення №{order.OrderId} на '{GetOrderStatusDisplayName(newStatus)}'?",
                                                          "Підтвердження зміни статусу",
                                                          MessageBoxButton.YesNo,
                                                          MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    order.Status = newStatus;
                    DisplayOrders();
                    changeStatusDialog.Close();
                }
            };
            buttonsPanel.Children.Add(confirmButton);

            Button cancelButton = new Button
            {
                Content = "Скасувати",
                Padding = new Thickness(10, 5, 0, 0)
            };
            cancelButton.Click += (s, e) => changeStatusDialog.Close();
            buttonsPanel.Children.Add(cancelButton);

            panel.Children.Add(buttonsPanel);
            changeStatusDialog.Content = panel;
            changeStatusDialog.ShowDialog();
        }


        private void EditOrder(Order order)
        {
            EditOrderWindow editWindow = new EditOrderWindow(order);
            bool? dialogResult = editWindow.ShowDialog();

            if (dialogResult == true)
            {
                order.CalculateTotalCost();
                DisplayOrders();
            }
        }

        private void CreateNewOrderButton_Click(object sender, RoutedEventArgs e)
        {
            // ...
        }
    }
}