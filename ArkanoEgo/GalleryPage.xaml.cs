using ArkanoEgo.Classes;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for GalleryPage.xaml
    /// </summary>
    public partial class GalleryPage : Page
    {
        List<GalleryElement> levels = new List<GalleryElement>();

        public GalleryPage()
        {
            InitializeComponent();
        }

        private void Button_MouseEvent(object sender, MouseEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow).changeColors(sender as Button, "blue");
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MenuPage());
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string path = @"..\..\Aloes";
            int nr = 1;
            try
            {
                DirectoryInfo folder = new DirectoryInfo(path);
                if (folder.Exists)
                {
                    foreach (FileInfo fileInfo in folder.GetFiles())
                    {
                        if (".png".Contains(fileInfo.Extension.ToLower()))
                        {
                            Image newImage = new Image();

                            BitmapImage src = new BitmapImage();
                            src.BeginInit();
                            src.UriSource = new Uri(fileInfo.FullName, UriKind.Absolute);
                            src.EndInit();
                            
                            newImage.Source = src;
                            levels.Add(new GalleryElement(nr, fileInfo.Name.Substring(0, fileInfo.Name.Length - 4), fileInfo.FullName));
                            /*Frame fr = new Frame();
                            fr.Navigate(new GalleryElementPage(src));
                            myUniformGrid.Children.Add(fr);*/
                            nr++;
                        }
                    }
                }
                levelList.ItemsSource = levels;
            }
            catch { MessageBox.Show("catch"); }
        }

        private void OpenLevel_Click(object sender, RoutedEventArgs e)
        {
            foreach (GalleryElement level in levels)
            {
                //MessageBox.Show(level._image);
                if ((sender as Button).Tag.ToString() == level._nr)
                    NavigationService.Navigate(new GamePage("lvl_" + level._name));
            }
        }
    }
}
