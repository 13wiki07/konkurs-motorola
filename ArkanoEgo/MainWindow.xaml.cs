using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ArkanoEgo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            gridFrame.Navigate(new GamePage()); // tutaj decydujemy jaką stronę wyświetlamy
            this.Cursor = new Cursor(Application.GetResourceStream(new Uri("/Resources/kursorek.cur", UriKind.Relative)).Stream);
        }

        // do wszystkich pages
        public void changeColors(Button sender, string color)
        {
            Button btn = sender as Button;
            Image img = (Image) btn.Content;

            string path = img.Source.ToString();
            if (path.Contains("white"))
                path = path.Replace("white", color);
            else
                path = path.Replace(color, "white");
            img.Source = new BitmapImage(new Uri(path));
        }
    }
}
