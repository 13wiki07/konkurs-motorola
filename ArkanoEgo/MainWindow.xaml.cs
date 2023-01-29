using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ArkanoEgo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            gridFrame.Navigate(new GamePage()); // tutaj decydujemy jaką stronę wyświetlamy
        }

        private void Creator_Click(object sender, RoutedEventArgs e)
        {
            gridFrame.Navigate(new CreatorPage());
            newGameBtn.Visibility = Visibility.Collapsed;
        }

        private void Game_Click(object sender, RoutedEventArgs e)
        {
            gridFrame.Navigate(new GamePage());
            newGameBtn.Visibility = Visibility.Visible;
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            gridFrame.Navigate(new MenuPage());
        }
        private void Quick_Click(object sender, RoutedEventArgs e)
        {
            var dialog = MessageBox.Show("Czy napewno chcesz zamknąć grę?", "Zamykanie", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (dialog == MessageBoxResult.OK)
            {
                Close();
            }
        }
    }
}
