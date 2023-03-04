using ArkanoEgo.Classes;
using ArkanoEgo.Classes.Bricks;
using ArkanoEgo.Classes.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Xml;

namespace ArkanoEgo
{
    public partial class CreatorPage : Page
    {
        bool edycja = false;
        string nameFile = "";
        Button wybrany = new Button();
        Button emptyBtn = new Button();
        List<Button> allButtons = new List<Button>();
        DateTime today = new DateTime();
        int totalPoints = 0;
        int totalBlocks = 0;
        int silverCount = 0;
        int goldCount = 0;

        string white, orange, aqua, green, rose, blue, pink, yellow, silver, gold;

        public CreatorPage(string fileName)
        {
            InitializeComponent();
            today = DateTime.Now; // domyślnie będzie tworzona nazwa z daty

            edycja = true;
            wybrany = emptyBtn;
            Colors();
            aktualizuj_dane();
            newLevelBtn.Visibility = Visibility.Collapsed;
            nameFile = fileName;
            tbFileName.Text = nameFile;
            LoadLevel(nameFile);
        }
        
        public CreatorPage()
        {
            InitializeComponent();
            today = DateTime.Now; // domyślnie będzie tworzona nazwa z daty
            edycja = false;
            howManySilver.Content = "0";
            newMap();
            aktualizuj_dane();
            wybrany = emptyBtn;
            Colors();
            
            aktualizuj_dane();
            newLevelBtn.Visibility = Visibility.Collapsed;
        }

        private void Colors()
        {
            white = ColorConverter.ConvertFromString("#FFFFFF").ToString(); // biały
            orange = ColorConverter.ConvertFromString("#F8B34B").ToString(); // pomarańczowy
            aqua = ColorConverter.ConvertFromString("#6CD4C5").ToString(); // aqua
            green = ColorConverter.ConvertFromString("#98E677").ToString(); // zielony
            rose = ColorConverter.ConvertFromString("#FD6B6B").ToString(); // ciemny róż
            blue = ColorConverter.ConvertFromString("#79A5F2").ToString(); // ciemny niebieski
            pink = ColorConverter.ConvertFromString("#E5989B").ToString(); // jasny róż
            yellow = ColorConverter.ConvertFromString("#FFDC6C").ToString(); // żółty
            silver = ColorConverter.ConvertFromString("#626161").ToString(); // srebrny
            gold = ColorConverter.ConvertFromString("#C69245").ToString(); // złoty

        }
        private void newMap()
        {
            for (int i = 0; i < 13; i++) //x
            {
                for (int j = 0; j < 21; j++) //y
                {
                    Button btn = new Button();
                    btn.SetValue(Grid.ColumnProperty, i);
                    btn.SetValue(Grid.RowProperty, j);
                    btn.Click += Field_LeftClick;
                    btn.MouseDown += Field_RightClick;
                    btn.Background = new SolidColorBrush(Color.FromRgb(58, 63, 75));//58, 63, 75));
                    btn.BorderBrush = new SolidColorBrush(Color.FromRgb(24, 29, 41));//33, 39, 53));
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
            (sender as Button).Background = wybrany.Background;
            aktualizuj_dane();
        }

        private void Field_RightClick(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                Button btn = sender as Button;
                if (kolor(btn) != kolor(emptyBtn))
                {
                    totalBlocks--;
                    if (kolor(btn) == silver)
                        silverCount--;

                    btn.Background = emptyBtn.Background;
                }
                aktualizuj_dane();
            }
        }

        private void SaveMapToFile()
        {
            aktualizuj_dane();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("    ");
            settings.OmitXmlDeclaration = true;

            string nazwa = "" + today.ToShortDateString() + today.Hour + today.Minute + today.Millisecond;

            XmlWriter writer = XmlWriter.Create(@"CustomLVLS\lvl_" + nazwa + ".xml", settings);
            writer.WriteStartElement("XMLBricks");

            tbFileName.Text = @"lvl_" + nazwa;

            for (int n = 0; n < allButtons.Count; n++)
            {
                if (kolor(allButtons[n]) != kolor(emptyBtn))
                {
                    writer.WriteStartElement("XMLBrick");
                    if (kolor(allButtons[n]) == silver)
                    { // srebrny
                        writer.WriteElementString("Type", "2");
                        writer.WriteElementString("PosX", allButtons[n].GetValue(Grid.ColumnProperty).ToString());
                        writer.WriteElementString("PosY", allButtons[n].GetValue(Grid.RowProperty).ToString());
                        writer.WriteElementString("Value", silverValue.Text);
                        writer.WriteElementString("Color", silver);
                        writer.WriteElementString("TimesToBreak", silverTimesToBreak.Text);
                    }
                    else if (kolor(allButtons[n]) == gold)
                    { // złoty
                        writer.WriteElementString("Type", "3");
                        writer.WriteElementString("PosX", allButtons[n].GetValue(Grid.ColumnProperty).ToString());
                        writer.WriteElementString("PosY", allButtons[n].GetValue(Grid.RowProperty).ToString());
                        writer.WriteElementString("Value", "0");
                        writer.WriteElementString("Color", gold);
                        writer.WriteElementString("TimesToBreak", "0");
                    }
                    else
                    {// cała reszta
                        writer.WriteElementString("Type", "1");
                        writer.WriteElementString("PosX", allButtons[n].GetValue(Grid.ColumnProperty).ToString());
                        writer.WriteElementString("PosY", allButtons[n].GetValue(Grid.RowProperty).ToString());

                        if (kolor(allButtons[n]) == white)
                            writer.WriteElementString("Value", "50");

                        else if (kolor(allButtons[n]) == orange)
                            writer.WriteElementString("Value", "60");

                        else if (kolor(allButtons[n]) == aqua)
                            writer.WriteElementString("Value", "70");

                        else if (kolor(allButtons[n]) == green)
                            writer.WriteElementString("Value", "80");

                        else if (kolor(allButtons[n]) == rose)
                            writer.WriteElementString("Value", "90");

                        else if (kolor(allButtons[n]) == blue)
                            writer.WriteElementString("Value", "100");

                        else if (kolor(allButtons[n]) == pink)
                            writer.WriteElementString("Value", "110");

                        else if (kolor(allButtons[n]) == yellow)
                            writer.WriteElementString("Value", "120");

                        writer.WriteElementString("Color", kolor(allButtons[n]));
                        writer.WriteElementString("TimesToBreak", "1");
                    }
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
            writer.Flush();
            writer.Close();

            createLevelImage(nazwa);
            

            MessageBox.Show("Pomyślnie zapisano level");
            newLevelBtn.Visibility = Visibility.Visible;
            saveBtn.Visibility = Visibility.Collapsed;
            playBtn.Visibility = Visibility.Visible;
        }
        public Brick[,] bricks = new Brick[13, 21]; // przenieś potem do góry kodu

        private void LoadLevel(string fileName)// tekstDoPliku path
        {
            edycja = true;
            bricks = Tools.ReadLvl(fileName);
            
            for (int i = 0; i < 13; i++) //x
            {
                for (int j = 0; j < 21; j++) //y
                {
                    Button btn = new Button();
                    btn.SetValue(Grid.ColumnProperty, i);
                    btn.SetValue(Grid.RowProperty, j);
                    btn.Click += Field_LeftClick;
                    btn.MouseDown += Field_RightClick;

                    if (bricks[i, j] != null) // jeśli jest klocek kolorowy
                        btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bricks[i, j].Color));
                        
                    else // jeśli jest empty
                        btn.Background = new SolidColorBrush(Color.FromRgb(58, 63, 75));//58, 63, 75));

                    btn.BorderBrush = new SolidColorBrush(Color.FromRgb(24, 29, 41));//33, 39, 53));
                    btn.BorderThickness = new Thickness(1.5);
                    gridCreator.Children.Add(btn);
                    emptyBtn.Background = btn.Background;
                    allButtons.Add(btn);
                }
            }
            playBtn.Visibility = Visibility.Visible;
            aktualizuj_dane();
        }
        #region funkcje
        private void NewLevel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CreatorPage());
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new GamePage(tbFileName.Text.ToString()));
        }

        private void SaveLevel_Click(object sender, RoutedEventArgs e)
        {
            SaveMapToFile();
        }

        private void ClearMap_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CreatorPage());
        }

        private void TextBoxes_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);

            if (new Regex("( ^[0-9]*$)").IsMatch(e.Text))
            {
                TextBox tb = sender as TextBox;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            getBoostersFromCheckboxes();
        }

        private void SValue_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (tb.Text == "")
                tb.Text = "10";

            int numerek = Convert.ToInt32(tb.Text);
            numerek = numerek / 10;
            numerek = numerek * 10;

            if (numerek < 10)
                numerek = 10;

            tb.Text = "" + numerek;
        }

        private void TtB_LostFocus(object sender, RoutedEventArgs e) // kiedy pole TimesToBreak zostanie opuszczone
        {
            TextBox tb = sender as TextBox;
            if (tb.Text == "")
                tb.Text = "1";

            aktualizuj_dane();
        }

        private void Info_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (tb.Text.StartsWith("0"))
                tb.Text = tb.Text.Substring(1);

            svalue.Content = silverValue.Text;
        }

        private void aktualizuj_dane()
        {
            playBtn.Visibility = Visibility.Collapsed;
            totalBlocks = 0;
            totalPoints = 0;
            silverCount = 0;
            goldCount = 0;

            for (int n = 0; n < allButtons.Count; n++)
            {
                if (kolor(allButtons[n]) != kolor(emptyBtn))
                {
                    totalBlocks++;
                    if (kolor(allButtons[n]) == white)
                        totalPoints += 50;

                    else if (kolor(allButtons[n]) == orange)
                        totalPoints += 60;

                    else if (kolor(allButtons[n]) == aqua)
                        totalPoints += 70;

                    else if (kolor(allButtons[n]) == green)
                        totalPoints += 80;

                    else if (kolor(allButtons[n]) == rose)
                        totalPoints += 90;

                    else if (kolor(allButtons[n]) == blue)
                        totalPoints += 100;

                    else if (kolor(allButtons[n]) == pink)
                        totalPoints += 110;

                    else if (kolor(allButtons[n]) == yellow)
                        totalPoints += 120;

                    else if (kolor(allButtons[n]) == silver)
                    {
                        totalPoints += Convert.ToInt32(silverTimesToBreak.Text);
                        silverCount++;
                    }

                    else if (kolor(allButtons[n]) == gold)
                        goldCount++;
                }
            }
            howManyColor.Content = totalBlocks - silverCount - goldCount;
            howManySilver.Content = silverCount;
            howManyGold.Content = goldCount;
            lbTotalPoints.Content = totalPoints;
            lbBlocksAtLevel.Content = totalBlocks;

            if (totalBlocks == 0)
            {
                saveBtn.Visibility = Visibility.Collapsed;
                clearBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                saveBtn.Visibility = Visibility.Visible;
                clearBtn.Visibility = Visibility.Visible;
            }
        }

        private string kolor(Button btn)
        {
            return btn.Background.ToString();
        }

        private void Button_MouseEvent(object sender, MouseEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow).changeColors(sender as Button, "blue");
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MenuPage());
        }
        #endregion

        private void createLevelImage(string nazwa)
        {
            FrameworkElement element = gridCreator;
            BitmapEncoder imgEncoder = new BmpBitmapEncoder();

            if (element != null)
            {
                DrawingVisual drawingVisual = new DrawingVisual();
                using (DrawingContext context = drawingVisual.RenderOpen())
                {
                    VisualBrush brush = new VisualBrush(element) { Stretch = Stretch.Fill };
                    context.DrawRectangle(brush, null, new Rect(0, 0, element.ActualWidth, element.ActualHeight));
                    context.Close();
                }

                RenderTargetBitmap bitmap = new RenderTargetBitmap((int)element.ActualWidth, (int)element.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                bitmap.Render(drawingVisual);

                Image img = new Image();
                img.Source = bitmap;

                if (System.IO.File.Exists("CustomLVLS/Images/" + nazwa + ".png")) // nadpisanie pliku
                    System.IO.File.Delete("CustomLVLS/Images/" + nazwa + ".png");

                imgEncoder.Frames.Add(BitmapFrame.Create(bitmap));
                using (Stream stream = File.Create("CustomLVLS/Images/" + nazwa + ".png"))
                {
                    imgEncoder.Save(stream);
                    stream.Close();
                    stream.Dispose();
                }
            }
        }

        private void getBoostersFromCheckboxes()
        {
            List<int> powers = new List<int>();

            //case 1:_power = Power.PlayerLenght;
            //case 2:_power = Power.NewBall;
            //case 3:_power = Power.StrongerHit;
            //case 4:_power = Power.SkipLevel;
            //case 5:_power = Power.CatchBall;

            if ((bool)checkIncerasePlayerLength.IsChecked)
                powers.Add(1);

            if ((bool)checkAddNewBall.IsChecked)
                powers.Add(2);
            
            if ((bool)checkStrongerHitBall.IsChecked)
                powers.Add(3);
            
            if ((bool)checkSkipLevel.IsChecked)
                powers.Add(4);
            
            if ((bool)checkCatchBall.IsChecked)
                powers.Add(5);

            //if (powers.Count > 0)
            //    a = Tools.RundomNumberWithConditions(powers);
        }
    }
}
