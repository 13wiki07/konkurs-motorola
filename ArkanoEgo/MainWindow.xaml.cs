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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ArkanoEgo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer  = new DispatcherTimer();
        private bool goDown = true;
        public MainWindow()
        {
            InitializeComponent();

            myCanvas.Focus();

            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Tick += new EventHandler(GameTimerEvent);
            gameTimer.Start();

        }
        private void GameTimerEvent(object sender, EventArgs e)
        {
            if (goDown)
            {
                Canvas.SetTop(ball, Canvas.GetTop(ball) + 15);
                if(Canvas.GetTop(ball) + (ball.Height) > Application.Current.MainWindow.Height)
                {
                    goDown=false;
                }
            }
            else
            {
                Canvas.SetTop(ball, Canvas.GetTop(ball) - 15);
                if (Canvas.GetTop(ball) < 0)
                {
                    goDown = true;
                }
            }
        }

        private void myCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.A || e.Key == Key.Left && Canvas.GetLeft(player) > 5)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) - 10);
            }
            if (e.Key == Key.D || e.Key == Key.Right && Canvas.GetLeft(player) + (player.Width) < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + 10);
            }
        }
    }
}
