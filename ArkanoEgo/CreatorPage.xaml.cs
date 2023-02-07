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
using System.Xml.Linq;
using System.Xml.Serialization;
using static ArkanoEgo.Classes.Tools.Tools;

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
            /*for(int i = 0; i < 21*13; i++) // chyba nie potrzebne
            {
                allButtons[i].Background = wybrany.Background;
            }*/
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
                    btn.Background = new SolidColorBrush(Color.FromRgb(36, 36, 36));
                    btn.BorderBrush = Brushes.Black;
                    btn.BorderThickness = new Thickness(1.5);
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
            string path = @"..\..\LVLS\lvl5.xml"; // trzeba coś wykombinować by się robiły też nowe levele i by można było edytować stare
                                                  // datagrid w nowej zakładce czy coś można by je pokazywać

            XmlDocument doc = new XmlDocument();

            //doc.AppendChild();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("    ");
            settings.CloseOutput = false;
            settings.OmitXmlDeclaration = true;
            
            XmlWriter writer = XmlWriter.Create("lvl9.xml", settings);
            //int count = 0;
            writer.WriteStartElement("XMLBricks");

            for (int n = 0; n < allButtons.Count; n++)
            {
                if (allButtons[n].Background.ToString() != emptyBtn.Background.ToString())
                {
                    /*  KOLORKI
                        biały               1 50  #FFFFFF
                        pomarańczowy        1 60  #F8B34B
                        aqua                1 70  #6CD4C5
                        zielony             1 80  #98E677
                        ciemny róż          1 90  #FD6B6B
                        ciemny niebieski    1 100 #79A5F2
                        jasny róż           1 110 #E5989B
                        żółty               1 120 #FFDC6C
                        srebrny             2 50  //#626161
                        złoty               3 //-   #C69245
                     */
                    //count++;
                    //MessageBox.Show("okk " + allButtons[n].Background.ToString());

                    writer.WriteStartElement("XMLBrick");

                    if (allButtons[n].Background.ToString() == "#626161") // srebrny
                        writer.WriteElementString("Type", "2");
                    else if (allButtons[n].Background.ToString() == "#C69245") // złoty
                        writer.WriteElementString("Type", "3");
                    else
                        writer.WriteElementString("Type", "1");

                    writer.WriteElementString("PosX", allButtons[n].GetValue(Grid.ColumnProperty).ToString());
                    writer.WriteElementString("PosY", allButtons[n].GetValue(Grid.RowProperty).ToString());

                    if (allButtons[n].Background.ToString() == "#FFFFFF") // biały
                    {
                        writer.WriteElementString("Value", "50");
                        writer.WriteElementString("Color", "#FFFFFF");
                        writer.WriteElementString("TimesToBreak", "1");
                    }
                    else if (allButtons[n].Background.ToString() == "#F8B34B") // pomarańczowy
                    {
                        writer.WriteElementString("Value", "60");
                        writer.WriteElementString("Color", "#F8B34B");
                        writer.WriteElementString("TimesToBreak", "1");
                    }
                    else if (allButtons[n].Background.ToString() == "#6CD4C5") // aqua
                    {
                        writer.WriteElementString("Value", "70");
                        writer.WriteElementString("Color", "#6CD4C5");
                        writer.WriteElementString("TimesToBreak", "1");
                    }
                    else if (allButtons[n].Background.ToString() == "#98E677") // zielony
                    {
                        writer.WriteElementString("Value", "80");
                        writer.WriteElementString("Color", "#98E677");
                        writer.WriteElementString("TimesToBreak", "1");
                    }
                    else if (allButtons[n].Background.ToString() == "#FD6B6B") // ciemny róż
                    {
                        writer.WriteElementString("Value", "90");
                        writer.WriteElementString("Color", "#FD6B6B");
                        writer.WriteElementString("TimesToBreak", "1");
                    }
                    else if (allButtons[n].Background.ToString() == "#79A5F2") // ciemny niebieski
                    {
                        writer.WriteElementString("Value", "100");
                        writer.WriteElementString("Color", "#79A5F2");
                        writer.WriteElementString("TimesToBreak", "1");
                    }
                    else if (allButtons[n].Background.ToString() == "#E5989B") // jasny róż
                    {
                        writer.WriteElementString("Value", "110");
                        writer.WriteElementString("Color", "#E5989B");
                        writer.WriteElementString("TimesToBreak", "1");
                    }
                    else if (allButtons[n].Background.ToString() == "#FFDC6C") // żółty
                    {
                        writer.WriteElementString("Value", "120");
                        writer.WriteElementString("Color", "#FFDC6C");
                        writer.WriteElementString("TimesToBreak", "1");
                    }
                    else if (allButtons[n].Background.ToString() == "#626161") // srebrny
                    {
                        writer.WriteElementString("Value", "50");
                        writer.WriteElementString("Color", "#626161");
                        writer.WriteElementString("TimesToBreak", "2");
                    }
                    else if (allButtons[n].Background.ToString() == "#C69245") // złoty
                    {
                        writer.WriteElementString("Value", "");
                        writer.WriteElementString("Color", "#C69245");
                        writer.WriteElementString("TimesToBreak", "");
                    }
                    writer.WriteEndElement();
                    /*
                    writer.WriteElementString("Type", "1");    // int
                    writer.WriteElementString("PosX", "1");    // int
                    writer.WriteElementString("PosY", "1");    // int
                    writer.WriteElementString("Value", "1");    // int
                    writer.WriteElementString("Color", "#FFFFFF");    // string
                    writer.WriteElementString("TimesToBreak", "20");    // int
                    writer.WriteEndElement();*/
                }
            }
            writer.WriteEndElement();

            //MessageBox.Show("Ile: " + count);
            writer.Flush();

            /*      STARA METODA
             XmlSerializer sr = new XmlSerializer(typeof(XMLBrick));

            StreamWriter writer = new StreamWriter(path);
            bricksList.Clear();



            sr.Serialize(writer, "XMLBricks", )

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
            writer.Close();*/
            MessageBox.Show("Zapisano level \n" + path);
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

        private void SaveLevel_Click(object sender, RoutedEventArgs e)
        {
            SaveMapToFile();
        }
    }
}
