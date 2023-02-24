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
    /// Interaction logic for GalleryElementPage.xaml
    /// </summary>
    public partial class GalleryElementPage : Page
    {
        public GalleryElementPage()
        {
            InitializeComponent();
        }
        public GalleryElementPage(Image img)
        {
            InitializeComponent();
            imgLvL = img;
        }
        public GalleryElementPage(BitmapImage img)
        {
            InitializeComponent();
            imgLvL.Source = img;
        }

        public GalleryElementPage(string namee)
        {
            InitializeComponent();
            nameLvL.Content = namee;

            string[] tab = Directory.GetFiles(@"../../Aloes/", "*.png", SearchOption.AllDirectories);
            //MessageBox.Show("Tab 0 " + tab[0]);
            //MessageBox.Show("Tab 1 " + tab[1]);

            Image im = new Image();
            //imgLvL.Source = new BitmapImage(new Uri(@"Resources/Aloes/cos.png", UriKind.Relative));

            //string s = ((BitmapFrame)imgLvL.Source).Decoder.ToString();
            //MessageBox.Show(s);


            //im.Source = imgLvL.Source;
            //if (tab[0] == im.Source.ToString())
            //    MessageBox.Show("ok");
            //else
             //   MessageBox.Show(tab[0] + " -- " + im.Source.ToString());
            /*filePaths[0] = @"../../Aloes/cos.png";
            Frame fr = new Frame();
            fr.Navigate(new GalleryElementPage(filePaths[i]));
            myUniformGrid.Children.Add(fr);*/
            //imgLvL.Source = new BitmapImage(new Uri(@"pack://application:,,,/../../Aloes/cos.png"));
            //imgLvL.Source = new BitmapImage(new Uri(namee, UriKind.Relative));
        }
    }
}
