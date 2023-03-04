using System.IO;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Media;
using Microsoft.Win32;
using System.Windows.Media;
using System.Threading.Tasks;

namespace ArkanoEgo
{
    public partial class MenuPage : Page
    {
        public MenuPage()
        {
            InitializeComponent();
        }

        protected virtual bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                MessageBox.Show(ex.ToString());
                return true;
            }

            //file is not locked
            return false;
        }

        private void Game_Click(object sender, RoutedEventArgs e)
        {
            //player1.Stop();
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
            SystemSounds.Beep.Play();
            var dialog = MessageBox.Show("Are you sure you want to leave the game?", "Exit", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (dialog == MessageBoxResult.OK)
            {
                Application.Current.Shutdown();
            }
        }

        private void PlayMusic_Loaded(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow).musicPlayer.Source = new Uri(@"..\..\Resources\Music\NieMamIdeaForNazwa.mp3", UriKind.RelativeOrAbsolute);
            (Application.Current.MainWindow as MainWindow).musicPlayer.Play();
        }
    }
}
