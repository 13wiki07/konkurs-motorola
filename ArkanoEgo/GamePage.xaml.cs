using ArkanoEgo.Classes;
using System;
using System.Collections.Generic;
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
        public GamePage()
        {
            InitializeComponent();
            GenerateElements();
            myCanvas.Focus();

            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Tick += new EventHandler(GameTimerEvent);
            gameTimer.Start();

            // kulka przypisanie
            ball.posX = Convert.ToInt32(ballEclipse.GetValue(Grid.WidthProperty));
            ball.posY = Convert.ToInt32(ballEclipse.GetValue(Grid.HeightProperty));
            ball.rad = Convert.ToInt32(ballEclipse.Height)/2; // promień kuli
            //testowyLabel.Content = "Promień: " + ball.rad;
            ball.top = true; // potrzebne do testu z onclickiem
            ball.right = true; // potrzebne do testu z onclickiem
        }

        public void GenerateElements()
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
                        if(goDown==true)
                        goDown = false;
                        else
                        goDown = true;

                        break;
                    }
                }
                if (x.Name == "player")//jeżeli element jest graczem to się od niego odbij
                {
                    Rect ballEclipseHitBox = new Rect(Canvas.GetLeft(ballEclipse), Canvas.GetTop(ballEclipse), ballEclipse.Width, ballEclipse.Height);
                    Rect BlockHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    if (ballEclipseHitBox.IntersectsWith(BlockHitBox))
                    {
                        goDown = false;
                    }
                }

            }

            /*
            if (goDown)
            {
                Canvas.SetTop(ballEclipse, Canvas.GetTop(ballEclipse) + 10);
                if (Canvas.GetTop(ballEclipse) + (ballEclipse.Height) > myCanvas.Height)
                {
                    goDown = false;
                }
            }
            else
            {
                Canvas.SetTop(ballEclipse, Canvas.GetTop(ballEclipse) - 10);
                if (Canvas.GetTop(ballEclipse) < 0)
                {
                    goDown = true;
                }
            }*/
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
        }


        private void changeBallDirection()
        {
            //testowyLabel.Content = ball.posY + " == " + (ball.rad+5);
            if(ball.posY < 5 + ball.rad) // góra
            {
                testowyLabel.Content = "tu powinna byc zmiana";
                // odbicie
                ball.top = false;
            }
            testowyLabel.Content = ball.posX + " == " + ball.rad;

            if (ball.posX < ball.rad) // lewy bok
            {
                testowyLabel.Content = "tu powinna byc zmiana";
                // odbicie
                ball.right = false;
            }
            //testowyLabel.Content = ball.posY + " == " + (ball.rad + 5);

            if (ball.posX > /*Convert.ToInt32(Canvas.WidthProperty)*/ 960 - ball.rad) // prawy bok
            {
                testowyLabel.Content = "tu powinna byc zmiana";
                // odbicie
                ball.right = true;
            }

            if (ball.posY > 805 - ball.rad) // dół
            {
                testowyLabel.Content = "tu powinna byc zmiana";
                // odbicie
                ball.top = true;
            }
            // https://paradacreativa.es/pl/como-hacer-flechas-en-el-teclado-de-laptop/
            // napewno !bool -> !top !bot !left !right
            // dodaj jakiś if który sprawdza od czego odbiła się kulka + dodaj wymiary kulki i paletki
        }

        private void ballMovement()
        {
            ball.posX = (int) Canvas.GetLeft(ballEclipse);
            ball.posY = (int) Canvas.GetTop(ballEclipse);
            // co ileś milisekund
            if (ball.right)
                ball.posX -= 10;
            else if (!ball.right)
                ball.posX += 10;

            if (ball.top)
                ball.posY -= 10;
            else if (!ball.top)
                ball.posY += 10;

            //Canvas.SetTop(ballEclipse, Canvas.GetTop(ballEclipse) - 10);    // do góry
            //Canvas.SetTop(ballEclipse, Canvas.GetTop(ballEclipse) + 10);    // do dołu
            //Canvas.SetLeft(ballEclipse, Canvas.GetLeft(ballEclipse) - 10);  // w lewo
            //Canvas.SetLeft(ballEclipse, Canvas.GetLeft(ballEclipse) + 10);  // w prawo

            Canvas.SetLeft(ballEclipse, ball.posX);
            Canvas.SetTop(ballEclipse, ball.posY);
            changeBallDirection();
            //testowyLabel.Content = "R: " + ball.right + " T: " + ball.top;

        }

        private void Button_Click(object sender, RoutedEventArgs e) // kontrolne, do kontroli ruchu
        {
            ballMovement();
        }
    }
}
