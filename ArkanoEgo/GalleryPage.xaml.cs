using ArkanoEgo.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArkanoEgo
{
    public partial class GalleryPage : Page
    {
        List<GalleryElement> levels = new List<GalleryElement>();

        public GalleryPage()
        {
            InitializeComponent();
        }

        public GalleryPage(string fileToDelete)
        {
            InitializeComponent();
            MessageBox.Show("CZY: " + IsFileLocked(new FileInfo("CustomLVLS/Images/03.03.20231148916.png")));
            File.Delete("CustomLVLS/Images/" + "03.03.20231148916.png");
        }

        private void Button_MouseEvent(object sender, MouseEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow).changeColors(sender as Button, "blue");
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MenuPage());
        }
        private void Window_OnLoad(object sender, RoutedEventArgs e)
        {
            string path = @"CustomLVLS\Images";
            int nr = 1; // pozycja w galerii
            try
            {
                DirectoryInfo info = new DirectoryInfo(path); // potrzebne do pełnej ścieżki
                string[] filesFolder = Directory.GetFiles(path);

                foreach (string file in filesFolder)
                {
                    string fullPath = info.FullName + file.Replace(@"CustomLVLS\Images\", @"\");

                    if (file.EndsWith(".png"))
                    {
                        Image newImage = new Image();
                        BitmapImage src = new BitmapImage();

                        src.BeginInit();
                        src.UriSource = new Uri(fullPath, UriKind.Absolute);
                        src.EndInit();

                        newImage.Source = src;
                        levels.Add(new GalleryElement(nr, file.Substring(0, file.Length - 4).Replace(@"CustomLVLS\Images\", ""), fullPath));
                        nr++;
                    }
                }

                ///////////////// STARA METODA /////////////////
                /*DirectoryInfo folder = new DirectoryInfo(path);
                if (folder.Exists)
                {
                    for(int i = 0; i < folder.GetFiles().Length; i++)
                    {
                        //
                        //using (FileInfo fileInfo = new FileInfo)
                    }
                    foreach (FileInfo fileInfo in folder.GetFiles())
                    {
                        MessageBox.Show("f: " + fileInfo.FullName);
                        if (".png".Contains(fileInfo.Extension.ToLower()))
                        {
                            Image newImage = new Image();
                            BitmapImage src = new BitmapImage();

                            src.BeginInit();
                            src.UriSource = new Uri(fileInfo.FullName, UriKind.Absolute);
                            src.EndInit();
                            
                            newImage.Source = src;
                            levels.Add(new GalleryElement(nr, fileInfo.Name.Substring(0, fileInfo.Name.Length - 4), fileInfo.FullName));
                            nr++;
                        }
                    }
                }*/
                levelList.ItemsSource = levels;
            }
            catch { MessageBox.Show("catch"); } // todo kangurek do usunięcia 
        }

        private void OpenLevel_Click(object sender, RoutedEventArgs e)
        {
            foreach (GalleryElement level in levels)
            {
                if ((sender as Button).Tag.ToString() == level._nr)
                    NavigationService.Navigate(new GamePage("lvl_" + level._name));
            }
        }

        private void Element_MouseRightClick(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show("okk");
            MessageBox.Show((sender as Button).Tag.ToString() + "okk");
            var dialog = MessageBox.Show("Yes - creator, No - usuń", "aa", MessageBoxButton.YesNo);

            if (dialog == MessageBoxResult.Yes)
            {
                foreach (GalleryElement level in levels)
                {
                    if ((sender as Button).Tag.ToString() == level._nr)
                        NavigationService.Navigate(new CreatorPage("lvl_" + level._name));
                }
            }
            //File.Copy(@"CustomLVLS/Images/03.03.2023104650.png", @"CustomLVLS/Images/03.03.aa.png");
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();

            MessageBox.Show("CZY: " + IsFileLocked(new FileInfo("CustomLVLS/Images/03.03.20231148916.png")));
            //File.Delete("CustomLVLS/Images/" + "03.03.20231148916.png");

            //File.Delete(@"CustomLVLS/Images/03.03.2023104650.png");
            if (dialog == MessageBoxResult.No)
            {
                //MessageBox.Show("teraz nowa page");
                //NavigationService.Navigate(new MenuPage("s"));
                //NavigationService.Navigate(new GalleryPage("str"));
                foreach (GalleryElement level in levels)
                {
                    if ((sender as Button).Tag.ToString() == level._nr)
                    {
                        //System.IO.File.Delete("CustomLVLS/Images/" + level._name + ".png");

                        //System.GC.Collect();
                        //System.GC.WaitForPendingFinalizers();
                        //File.Delete("CustomLVLS/Images/" + level._name + ".png");
                        break;
                    }
                }
            }
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
    }
}

