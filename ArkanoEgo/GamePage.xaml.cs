using ArkanoEgo.Classes;
using ArkanoEgo.Classes.Bricks;
using ArkanoEgo.Classes.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ArkanoEgo
{
    public partial class GamePage : Page
    {
        public int points = 0;
        public Brick[,] Bricks = new Brick[13, 20];

        DispatcherTimer gameTimer = new DispatcherTimer();
        Ball ball = new Ball();

        //wymiary Canvas'a / pola gry
        int height;
        int width;

        public GamePage()
        {
            InitializeComponent();

            height = (int)windowPage.Height;
            width = (int)windowPage.Width;

            // kulka przypisanie
            // w linku jest rozpiska co czym jest
            // https://www.canva.com/design/DAFSS32ggNg/ZHP5O-GhpveqJ4X_GtHzrg/view?utm_content=DAFSS32ggNg&utm_campaign=designshare&utm_medium=link2&utm_source=sharebutton#5
            ball.rad = Convert.ToInt32(ballEclipse.Height) / 2; // promień kuli
            ball.posX = Convert.ToInt32(Canvas.GetLeft(ballEclipse));
            ball.posY = Convert.ToInt32(Canvas.GetTop(ballEclipse));

            //testowyLabel.Content = "w: " + ball.posX + " h: " + ball.posY;

            ball.top = true; // potrzebne do testu z onclickiem i Q
            ball.left = true; // potrzebne do testu z onclickiem i Q

            // to jest po to, by klocki nie miały wymiarów w double tak samo jak canvas
            height = (int)SystemParameters.FullPrimaryScreenHeight / 13;
            height = height * 13;

            /* np.
             system -> 800
             /13 -> 61
             *13 -> 793 <= i to ma być szerokość i elo  */

            windowPage.SetValue(WidthProperty, (double)(height));
            windowPage.UpdateLayout();
            width = (int)windowPage.Width;

            Bricks = Tools.ReadLvl(1);//Wczytywanie mapy
            GenerateElements();
            myCanvas.Focus();

            gameTimer.Interval = TimeSpan.FromMilliseconds(1);//TODO coś nie działa z tym czasem, gdy się ustawi na 0, to jest szybko, a od 1 do 20 prawie tak samo
            gameTimer.Tick += new EventHandler(GameTimerEvent);
            gameTimer.Start();

            //testowyLabel.Content = "Wymiary canvy: " + width + " x " + height;
        }

        public void GenerateElements() // potrzba dodać skrypt odczytujący pola i kolory klocków
        {
            int top = 0;
            int left = 0;
            for (int i = 0; i < 13; i++)
            {//x

                for (int j = 0; j < 20; j++)//y
                {

                    if (Bricks[i, j] != null)
                    {
                        // Create the rectangle
                        Rectangle rec = new Rectangle()
                        {
                            Width = width/13,
                            Height = height/26, // 26 albo 27
                            Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Bricks[i, j].Color)),//color pobierany z obiektu
                            Stroke = Brushes.Red,
                            StrokeThickness = 1,
                        };

                        // Add to a canvas for example
                        myCanvas.Children.Add(rec);
                        Canvas.SetTop(rec, top);
                        Canvas.SetLeft(rec, left);
                    }
                    top = top + (height/26);
                }
                left = left + (width/13);
                top = 0;
            }
        }

        private void GameTimerEvent(object sender, EventArgs e)
        {
            foreach (var x in myCanvas.Children.OfType<Rectangle>())
            {
                if (x.Name != "player")//jeżeli element jest blokiem to go usun
                {
                    int posX = (int)Canvas.GetLeft(x) / (width/13);//element [x,0] tablicy
                    int poxY = (int)Canvas.GetTop(x) / (height/26);//element [0,Y] tablicy

                    Rect ballEclipseHitBox = new Rect(Canvas.GetLeft(ballEclipse), Canvas.GetTop(ballEclipse), ballEclipse.Width, ballEclipse.Height);
                    Rect BlockHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    if (ballEclipseHitBox.IntersectsWith(BlockHitBox))
                    {
                        // górna krawędź klocka
                        if (Canvas.GetLeft(x) < ball.posX && ball.posX < Canvas.GetLeft(x) + x.Width && ball.posY < Canvas.GetTop(x) + x.Height)
                        {
                            ball.top = true;
                            if (x.Tag is 2)// TODO trzeba to zamienić na timeToBrick z danego obiektu
                            {
                                x.Tag = 1;
                            }
                            else
                            {
                                if (Bricks[posX, poxY].GetType() != typeof(GoldBrick))//sprawdzanie czy obiekt nie jest GoldBrick (ten obiekt nie ma Value)
                                {
                                    points += Bricks[posX, poxY].Value;
                                    myCanvas.Children.Remove(x);
                                }
                                pointsLabel.Content = "Points: " + points;
                            }
                        }

                        // dolna krawędź klocka
                        if (Canvas.GetLeft(x) < ball.posX && ball.posX < Canvas.GetLeft(x) + x.Width && ball.posY > Canvas.GetTop(x))
                        {
                            ball.top = false;
                            if (x.Tag is 2)
                            {
                                x.Tag = 1;
                            }
                            else
                            {
                                if (Bricks[posX, poxY].GetType() != typeof(GoldBrick))
                                {
                                    points += Bricks[posX, poxY].Value;
                                    myCanvas.Children.Remove(x);
                                }
                                pointsLabel.Content = "Points: " + points;
                            }
                        }


                        // lewa krawędź klocka
                        if (Canvas.GetTop(x) < ball.posY && ball.posY < Canvas.GetTop(x) + x.Height && ball.posX < Canvas.GetLeft(x) + x.Width)
                        {
                            ball.left = true;
                            if (x.Tag is 2)
                            {
                                x.Tag = 1;
                            }
                            else
                            {
                                if (Bricks[posX, poxY].GetType() != typeof(GoldBrick))
                                {
                                    points += Bricks[posX, poxY].Value;
                                    myCanvas.Children.Remove(x);
                                }
                                pointsLabel.Content = "Points: " + points;
                            }
                        }

                        // prawa krawędź klocka
                        if (Canvas.GetTop(x) < ball.posY && ball.posY < Canvas.GetTop(x) + x.Height && ball.posX > Canvas.GetLeft(x))
                        {
                            ball.left = false;
                            if (x.Tag is 2)
                            {
                                x.Tag = 1;
                            }
                            else
                            {
                                if (Bricks[posX, poxY].GetType() != typeof(GoldBrick))
                                {
                                    points += Bricks[posX, poxY].Value;
                                    myCanvas.Children.Remove(x);
                                }
                                pointsLabel.Content = "Points: " + points;
                            }
                        }

                        break;
                    }


                }
                if (x.Name == "player") //jeżeli element jest graczem to się od niego odbij
                {
                    Rect ballEclipseHitBox = new Rect(Canvas.GetLeft(ballEclipse), Canvas.GetTop(ballEclipse), ballEclipse.Width, ballEclipse.Height);
                    Rect BlockHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    if (ballEclipseHitBox.IntersectsWith(BlockHitBox))
                    {
                        ball.top = true;
                    }
                }
            }
            ballMovement();
        }

        private void myCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A || e.Key == Key.Left && Canvas.GetLeft(player) > 5)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) - 10);
            }
            if (e.Key == Key.D || e.Key == Key.Right && Canvas.GetLeft(player) + (player.Width) < myCanvas.Width)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + 10);
            }

            if (e.Key == Key.Q)
            {
                ballMovement();
            }
        }

        private void changeBallDirection()
        {
            //testowyLabel.Content = "PosY zwykłe: " + (ball.posY-ball.rad)  + "PosY: " + ball.posY + " Rad: " + ball.rad; // nie usuwajcie tego ~ Wika

            if (ball.posY <= 0) // góra
            {
                ball.top = false; // odbicie
            }

            if (ball.posX <= 0) // lewy bok
            {
                ball.left = false;
            }

            if (ball.posX >= width - (ball.rad * 2)) // prawy bok
            {
                ball.left = true;
            }

            if (ball.posY >= height) // dół // jest na razie i gdy przegrana będzie zrobiona to do usunięcia
            {
                ball.top = true;
            }
            // dodaj jakiś if który sprawdza od czego odbiła się kulka + dodaj wymiary kulki i paletki (ale do zmiennych) ~ Wika
        }

        private void ballMovement()
        {
            ball.posX = (int)Canvas.GetLeft(ballEclipse);
            ball.posY = (int)Canvas.GetTop(ballEclipse);

            // dodaj wykonywanie co ileś milisekund (chyba najlepiej 6-10)
            if (ball.left)
                ball.posX -= 1;
            else if (!ball.left)
                ball.posX += 1;

            if (ball.top)
                ball.posY -= 1;
            else if (!ball.top)
                ball.posY += 1;

            //testowyLabel.Content = "w: " + ball.posX + " h: " + ball.posY;

            //Canvas.SetTop(ballEclipse, Canvas.GetTop(ballEclipse) - 10);    // do góry
            //Canvas.SetTop(ballEclipse, Canvas.GetTop(ballEclipse) + 10);    // do dołu
            //Canvas.SetLeft(ballEclipse, Canvas.GetLeft(ballEclipse) - 10);  // w lewo
            //Canvas.SetLeft(ballEclipse, Canvas.GetLeft(ballEclipse) + 10);  // w prawo

            Canvas.SetLeft(ballEclipse, ball.posX);
            Canvas.SetTop(ballEclipse, ball.posY);
            changeBallDirection();
        }
    }
}
