using ArkanoEgo.Classes;
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
        DispatcherTimer gameTimer = new DispatcherTimer();
        private bool goDown = true;

        Ball ball = new Ball();

        //wymiary Canvas'a / pola gry
        int height;
        int width;

        public GamePage()
        {
            InitializeComponent();
            GenerateElements();
            myCanvas.Focus();

            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Tick += new EventHandler(GameTimerEvent);
            gameTimer.Start();

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

            fun();
        }

        public void GenerateElements() // potrzba dodać skrypt odczytujący pola i kolory klocków
        {
            int top = 0;
            int left = 0;
            for (int i = 0; i < 12; i++)
            {//x

                for (int j = 0; j < 12; j++)//y
                {
                    // Create the rectangle
                    Rectangle rec = new Rectangle()
                    {
                        Width = 80,
                        Height = 47,
                        Fill = Brushes.Green,
                        Stroke = Brushes.Red,
                        StrokeThickness = 1,
                    };

                    // Add to a canvas for example
                    myCanvas.Children.Add(rec);
                    Canvas.SetTop(rec, top);
                    Canvas.SetLeft(rec, left);
                    top = top + 47;
                }
                left = left + 80;
                top = 0;
            }
        }

        private void GameTimerEvent(object sender, EventArgs e)
        {
            foreach (var x in myCanvas.Children.OfType<Rectangle>())
            {
                if (x.Name != "player")//jeżeli element jest blokiem to go usun
                {
                    Rect ballEclipseHitBox = new Rect(Canvas.GetLeft(ballEclipse), Canvas.GetTop(ballEclipse), ballEclipse.Width, ballEclipse.Height);
                    Rect BlockHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    if (ballEclipseHitBox.IntersectsWith(BlockHitBox))
                    {
                        myCanvas.Children.Remove(x);

                        // góra klocka
                        if (Canvas.GetLeft(x) < ball.posX && ball.posX < Canvas.GetLeft(x) + x.Width && ball.posY < Canvas.GetTop(x) + x.Height)
                        {
                            //testowyLabel.Content = "góra klocka";
                            ball.top = true;
                        }

                        // dół klocka
                        if (Canvas.GetLeft(x) < ball.posX && ball.posX < Canvas.GetLeft(x) + x.Width && ball.posY > Canvas.GetTop(x))
                        {
                            //testowyLabel.Content = "dół klocka";
                            ball.top = false;
                        }


                        // lewa klocka
                        if (Canvas.GetTop(x) < ball.posY && ball.posY < Canvas.GetTop(x) + x.Height && ball.posX < Canvas.GetLeft(x) + x.Width)
                        {
                            //testowyLabel.Content = "lewa klocka";
                            ball.left = true;
                        }

                        // prawa klocka
                        if (Canvas.GetTop(x) < ball.posY && ball.posY < Canvas.GetTop(x) + x.Height && ball.posX > Canvas.GetLeft(x))
                        {
                            //testowyLabel.Content = "prawa klocka";
                            ball.left = false;
                        }

                        // trzeba dodać jeszcze co gdy jest kant klocka (bo te funkcje tego nie wykrywają)
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
