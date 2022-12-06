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
        public GamePage()
        {
            InitializeComponent();
            GenerateElements();
            myCanvas.Focus();

            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Tick += new EventHandler(GameTimerEvent);
            gameTimer.Start();

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

                    Rect ballHitBox = new Rect(Canvas.GetLeft(ball), Canvas.GetTop(ball), ball.Width, ball.Height);
                    Rect BlockHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    if (ballHitBox.IntersectsWith(BlockHitBox))
                    {
                        myCanvas.Children.Remove(x);
                        break;
                    }
                }
                if (x.Name == "player")//jeżeli element jest graczem to się od niego odbij
                {
                    Rect ballHitBox = new Rect(Canvas.GetLeft(ball), Canvas.GetTop(ball), ball.Width, ball.Height);
                    Rect BlockHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    if (ballHitBox.IntersectsWith(BlockHitBox))
                    {
                        goDown = false;
                    }
                }

            }


            if (goDown)
            {
                Canvas.SetTop(ball, Canvas.GetTop(ball) + 10);
                if (Canvas.GetTop(ball) + (ball.Height) > myCanvas.Height)
                {
                    goDown = false;
                }
            }
            else
            {
                Canvas.SetTop(ball, Canvas.GetTop(ball) - 10);
                if (Canvas.GetTop(ball) < 0)
                {
                    goDown = true;
                }
            }
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
    }
}
