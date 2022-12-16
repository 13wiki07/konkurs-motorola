using ArkanoEgo.Classes;
using ArkanoEgo.Classes.Bricks;
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
using System.Xml;
using System.Xml.Serialization;

namespace ArkanoEgo
{
    public partial class CreatorPage : Page
    {
        Button wybrany = new Button();
        Button emptyBtn = new Button();
        public CreatorPage()
        {
            InitializeComponent();
            clearMap();
            wybrany = emptyBtn;
        }

        private void clearMap()
        {
            for(int i = 0; i < 13; i++) //x
            {
                for(int j = 0; j < 21; j++) //y
                {
                    Button btn = new Button();
                    btn.SetValue(Grid.ColumnProperty, i);
                    btn.SetValue(Grid.RowProperty, j);
                    btn.Click += Field_LeftClick;
                    btn.MouseDown+= Field_RightClick;
                    btn.Background = new SolidColorBrush(Color.FromArgb(10, 36, 36, 36));
                    gridCreator.Children.Add(btn);
                    emptyBtn = btn;
                }
            }
        }

        private void TemplateBtn_Click(object sender, RoutedEventArgs e)
        {
            wybrany.BorderBrush = Brushes.Transparent;
            wybrany = sender as Button;
            wybrany.BorderBrush = Brushes.Black;
            wybrany.BorderThickness = new Thickness(3);
        }

        private void Field_LeftClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            btn.Background = wybrany.Background;
        }

        private void Field_RightClick(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                Button btn = sender as Button;
                btn.Background = emptyBtn.Background;
            }
        }

        private void SaveMapToFile()
        {
            string path = @"..\..\LVLS\testowyLevel.xml"; // trzeba coś wykombinować by się robiły też nowe levele i by można było edytować stare

            try // TODO: dodać zapisywanie wszystkich buttonów, które na mapie nie są empty albo default
            {
                BrickXML brick = new BrickXML();
                brick.Type = 1;
                brick.PosX = 2;
                brick.PosY = 3;
                brick.Value = 150;
                brick.Color = "#ff00cc";
                brick.TimesToBreak = 1;

                XmlSerializer sr = new XmlSerializer(typeof(BrickXML));
                StreamWriter writer = new StreamWriter(path);
                sr.Serialize(writer, brick);
                writer.Close();
            }
            catch { }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveMapToFile();
        }
    }
}
