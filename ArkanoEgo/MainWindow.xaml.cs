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
            musicPlayer.Volume = 0.1;

            string path = @"CustomLVLS\Images";
            string path2 = @"CustomLVLS\";
            string[] filesFolder = Directory.GetFiles(path);
            string[] filesFolder2 = Directory.GetFiles(path2);

            if (filesFolder.Length > 1)
            {
                for (int i = 0; i < filesFolder.Length; i++)
                {
                    FileInfo file2 = new FileInfo(filesFolder[i]);
                    FileInfo file22 = new FileInfo(filesFolder2[i]);
                    
                    if (File.ReadAllText("dataDelete").Contains(file2.Name))
                    {
                        file2.Delete();
                        file22.Delete();
                    }
                }
            }
            File.WriteAllText("dataDelete", "");

            gridFrame.Navigate(new MenuPage()); // tutaj decydujemy jaką stronę wyświetlamy
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

        private void Music_Ended(object sender, RoutedEventArgs e)
        {
            musicPlayer.Position = TimeSpan.Zero;
            musicPlayer.Play();
        }
    }
}
