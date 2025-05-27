using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Cursova
{
    public partial class MainWindow : Window
    {
        public static int SelectedTableNumber { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            LoadTables();
        }

        private void LoadTables()
        {
            for (int i = 1; i <= 5; i++)
            {
                Button tableButton = new Button
                {
                    Content = $"Столик {i}",
                    Tag = i,
                    Width = 100,
                    Height = 100,
                    Margin = new Thickness(10),
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    Background = new SolidColorBrush(Color.FromRgb(137, 192, 249)),
                    Foreground = Brushes.White,
                    Cursor = Cursors.Hand,
                    Style = (Style)FindResource("TableButtonStyle")
                };
                tableButton.Click += TableButton_Click;
                TablesWrapPanel.Children.Add(tableButton);
            }
        }

        private void TableButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                SelectedTableNumber = (int)clickedButton.Tag;

                TableOrdersWindow tableOrdersWindow = new TableOrdersWindow(SelectedTableNumber);
                tableOrdersWindow.Show();
            }
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            /*if (SearchTextBox.Text == "Search...")
            {
                SearchTextBox.Text = "";
                SearchTextBox.Foreground = Brushes.Black;
            }*/
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            /*if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                SearchTextBox.Text = "Search...";
                SearchTextBox.Foreground = Brushes.Gray;
            }*/
        }
    }
}