using ArkanoEgo.Classes;
using ArkanoEgo.Classes.Bricks;
using ArkanoEgo.Classes.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        List<XMLBrick> bricksList = new List<XMLBrick>();
        List<Button> allButtons= new List<Button>();

        public CreatorPage()
        {
            InitializeComponent();
            newMap();
            
            wybrany = emptyBtn;
            for(int i = 0; i < 21*13; i++)
            {
                allButtons[i].Background = wybrany.Background;
            }
        }

        private void newMap()
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
                    emptyBtn.Background = btn.Background;
                    allButtons.Add(btn);
                }
            }
        }

        private void TemplateBtn_Click(object sender, RoutedEventArgs e)
        {
            wybrany.BorderBrush = wybrany.Background;
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
                                                          // datagrid w nowej zakładce czy coś można by je pokazywać

            XmlSerializer sr = new XmlSerializer(typeof(XMLBrick));
            StreamWriter writer = new StreamWriter(path);
            bricksList.Clear();

            for (int n = 0; n < allButtons.Count; n++)
            {
                if (allButtons[n].Background != emptyBtn.Background)
                {
                    XMLBrick b = createBrick(allButtons[n]);
                    b.Type = n;
                    bricksList.Add(b);
                }
            }

            for (int i = 0; i < bricksList.Count; i++)
            {
                sr.Serialize(writer, bricksList[i]); // dodaje dany brick do .xml
            }
            writer.Close();
        }

        private XMLBrick createBrick(Button btn)
        {
            XMLBrick brick = new XMLBrick();

            brick.Type = 1;
            brick.Value = 0;
            brick.Color = btn.Background.ToString();
            brick.PosX = (int)btn.GetValue(Grid.ColumnProperty);
            brick.PosY = (int)btn.GetValue(Grid.RowProperty);
            brick.TimesToBreak = 0;

            if (btn.Name == "silver")
            {
                brick.Type = 2;
                //brick.TimesToBreak = 5;
            }

            if (btn.Name == "gold")
            {
                brick.Type = 3;
                //brick.Value = 0;
            }

            return brick;

            
        /*  brick.Type;
            brick.Value;
            brick.Color;
            brick.PosX;
            brick.PosY;
            brick.TimesToBreak;*/
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveMapToFile();
        }
    }
}
