﻿using ArkanoEgo.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace ArkanoEgo
{
    public partial class GalleryPage : Page
    {
        List<GalleryElement> levels = new List<GalleryElement>();
        string toDeleteFile = "";
        public GalleryPage()
        {
            InitializeComponent();
        }

        public GalleryPage(string fileToDelete)
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
        private void Window_OnLoad(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow).musicPlayer.Source = new Uri(@"..\..\Resources\Music\Prequel_lvl1.mp3", UriKind.RelativeOrAbsolute);
            (Application.Current.MainWindow as MainWindow).musicPlayer.Play();
            
            string path = @"CustomLVLS\Images";
            int nr = 1; // pozycja w galerii
            try
            {
                string[] filesFolder = Directory.GetFiles(path);

                Image newImage = new Image();
                BitmapImage src = new BitmapImage();

                var lines = File.ReadAllLines("dataDelete");
                List<string> pominiete = new List<string>();
                if (lines.Length == 0)
                {
                    for (int i = 0; i < filesFolder.Length; i++)
                    {
                        FileInfo file2 = new FileInfo(filesFolder[i]);
                        using (FileStream stream = file2.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            src.UriSource = new Uri(new FileInfo(filesFolder[i]).FullName, UriKind.Absolute);
                            newImage.Source = src;
                            GalleryElement lvl = new GalleryElement(nr, new FileInfo(filesFolder[i]).Name.Substring(0, new FileInfo(filesFolder[i]).Name.Length - 4), new FileInfo(filesFolder[i]).FullName);
                            levels.Add(lvl);
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            stream.Dispose();
                            stream.Close();
                            nr++;
                        }
                    }
                }
                else
                {
                    for (int x = 0; x < lines.Length; x++)
                    {
                        for (int i = 0; i < filesFolder.Length; i++)
                        {
                            if (filesFolder[i].Contains(lines[x]))
                                pominiete.Add(filesFolder[i]);
                        }
                    }
                }

                string tekstDoPliku = "";
                for(int a = 0; a < pominiete.Count; a++)
                {
                    tekstDoPliku += pominiete[a] + "\n";
                }
                File.WriteAllText("dataDelete",tekstDoPliku);
                for (int i = 0; i < filesFolder.Length; i++)
                {
                    FileInfo file2 = new FileInfo(filesFolder[i]);
                    foreach (string pominiety in pominiete)
                    {
                        if (!File.ReadAllText("dataDelete").Contains(file2.Name))
                        {
                            using (FileStream stream = file2.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                src.UriSource = new Uri(new FileInfo(filesFolder[i]).FullName, UriKind.Absolute);
                                newImage.Source = src;
                                GalleryElement lvl = new GalleryElement(nr, new FileInfo(filesFolder[i]).Name.Substring(0, new FileInfo(filesFolder[i]).Name.Length - 4), new FileInfo(filesFolder[i]).FullName);
                                levels.Add(lvl);
                                GC.Collect();
                                GC.WaitForPendingFinalizers();
                                stream.Dispose();
                                stream.Close();
                                nr++;
                                break;
                            }
                        }
                    }
                }

                if (levels.Count == 0)
                {
                    scrollV.Visibility = Visibility.Collapsed;
                    emptyBTN.Visibility = Visibility.Visible;
                    emptyLB.Visibility = Visibility.Visible;
                }
                else
                {
                    scrollV.Visibility = Visibility.Visible;
                    emptyBTN.Visibility = Visibility.Collapsed;
                    emptyLB.Visibility = Visibility.Collapsed;
                }
                levelList.ItemsSource = levels;
            }
            catch { }
        }

        private void OpenLevel_Click(object sender, RoutedEventArgs e)
        {
            foreach (GalleryElement level in levels)
            {
                if ((sender as Button).Tag.ToString() == level._nr)
                    NavigationService.Navigate(new GamePage("lvl_" + level._name));
            }
        }

        private void ToCreatorPage_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CreatorPage());
        }

        private void EditLevel_Click(object sender, RoutedEventArgs e)
        {
            foreach (GalleryElement level in levels)
            {
                if ((sender as Button).Tag.ToString() == level._nr)
                    NavigationService.Navigate(new CreatorPage("lvl_" + level._name));
            }
        }

        private void DeleteLevel_Click(object sender, RoutedEventArgs e)
        {
            var dialog = MessageBox.Show("Are you sure you wanna delete this level?", "Delete level", MessageBoxButton.YesNo);

            if (dialog == MessageBoxResult.Yes)
            {
                foreach (GalleryElement level in levels)
                {
                    if ((sender as Button).Tag.ToString() == level._nr)
                    {
                        toDeleteFile = level._name + ".png";
                        string all = File.ReadAllText("dataDelete");
                        if (all == "")
                            File.WriteAllText("dataDelete", toDeleteFile);
                        else
                            File.WriteAllText("dataDelete", all + "\n" + toDeleteFile);
                        break;
                    }
                }
                NavigationService.Navigate(new GalleryPage(toDeleteFile));
            }
        }
    }
}

