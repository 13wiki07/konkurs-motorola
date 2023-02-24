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

namespace ArkanoEgo
{
    /// <summary>
    /// Logika interakcji dla klasy MenuPage.xaml
    /// </summary>
    public partial class MenuPage : Page
    {
        public MenuPage()
        {
            InitializeComponent();
        }

        private void Game_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new GamePage());
        }

        private void Kreator_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CreatorPage());
        }

        private void Gallery_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new GalleryPage());
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            var dialog = MessageBox.Show("Czy napewno chcesz zamknąć grę?", "Zamykanie", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (dialog == MessageBoxResult.OK)
            {
                Application.Current.Shutdown();
            }
        }


    }
}
