using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ArkanoEgo.Classes
{
    public enum Power
    {
        None = 0,
        PlayerLenght = 1,
        NewBall = 2,
        StrongerHit = 3,
    }
    public class Booster
    {
        public int rad { get; set; } // promień kulki (potrzebny do odbić)
        public int speed { get; set; }
        public int posX { get; set; } // - w góre, + w dół
        public int posY { get; set; } // - w lewo, + w prawo

        public bool top { get; set; } // top = true; bottom = false;
        public bool left { get; set; } // left = true; right = false;

        // _power boostera
        private Power _power = Power.None;

        public int PowerDuration = 10000;


        public Booster(Ball ball, ref Canvas myCanvas)// po wywołaniu tego konstruktora, booster zrespi się tma gdzie jest aktualnie piłka
        {
            Ellipse boost = new Ellipse()
            {
                Width = 20,
                Height = 20, // 26 albo 27
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#32CD32")),
                Tag = "Booster",
            };
            myCanvas.Children.Add(boost);
            Canvas.SetTop(boost, ball.posY);
            Canvas.SetLeft(boost, ball.posX);

            posX = ball.posX;
            posY = ball.posY;
        }
        public Power GetPower()
        {
            return _power;
        }
        public void SetPower(Power power)
        {
            _power = power;
        }

        public void RandomPower()
        {
            switch (Tools.Tools.RundomNumber(1, 3))
            {
                case 1:
                    _power = Power.PlayerLenght;
                    break;
                case 2:
                    _power = Power.NewBall;
                    break;
                case 3:
                    _power = Power.StrongerHit;
                    break;
                default:
                    _power = Power.None;
                    break;
            }
        }


        public void SetBoostPlayerLenght(ref Rectangle rectangle)
        {
            rectangle.Width = rectangle.Width * 2;
            Canvas.SetLeft(rectangle, Canvas.GetLeft(rectangle) - rectangle.ActualWidth / 2);
        }

        public void StopBoostPlayerLenght(ref Rectangle rectangle)
        {
            Canvas.SetLeft(rectangle, Canvas.GetLeft(rectangle) + rectangle.ActualWidth / 4);
            rectangle.Width = rectangle.Width / 2;
        }
        public void NewBallSetBoost(ref Canvas myCanvas, ref List<Ball> balls)
        {
            Ellipse ballEclipse = new Ellipse()
            {
                Width = 10,
                Height = 10, // 26 albo 27
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0000ff")),
                Tag = "ballEclipse",
            };

            myCanvas.Children.Add(ballEclipse);
            Canvas.SetTop(ballEclipse, balls[0].posY);
            Canvas.SetLeft(ballEclipse, balls[0].posX);

            Ball ball = new Ball();
            ball.InitBall(ballEclipse);

            ball.top = balls[0].top;
            if (balls[0].left)
                ball.left = false;
            else
                ball.left = true;

            balls.Add(ball);
        }
        public void NewBallStopBoost(ref List<Ball> balls)
        {

        }
        public float durationTime { get; set; }
        // ew można kilka klas/typów boosterów zrobić
    }
}
