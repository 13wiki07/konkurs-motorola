﻿using System.Collections.Generic;
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
        SkipLevel = 4,
        Shooting = 5,
        StickyPlayer = 6
    }
    public class Booster
    {
        public int rad { get; set; } // promień kulki (potrzebny do odbić)
        public int speed { get; set; }
        public double posX { get; set; } // - w góre, + w dół
        public double posY { get; set; } // - w lewo, + w prawo

        public bool top { get; set; } // top = true; bottom = false;
        public bool left { get; set; } // left = true; right = false;

        // _power boostera
        private Power _power = Power.None;

        public Booster() { }
        public Booster(Ball ball, ref Canvas myCanvas) // po wywołaniu tego konstruktora, booster zrespi się tma gdzie jest aktualnie piłka
        {
            Ellipse boost = new Ellipse()
            {
                Width = 20,
                Height = 20,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#32CD32")),
                Tag = "Booster",
            };
            myCanvas.Children.Add(boost);
            Canvas.SetTop(boost, ball.posY);
            Canvas.SetLeft(boost, ball.posX);

            posX = ball.posX;
            posY = ball.posY;
        }

        public Booster(Ball ball, ref Canvas myCanvas, Booster booster) // po wywołaniu tego konstruktora, booster zrespi się tma gdzie jest aktualnie piłka
        {
            Ellipse boost = new Ellipse()
            {
                Width = 20,
                Height = 20,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#32CD32")),
                Tag = "Booster",
            };
            myCanvas.Children.Add(boost);
            Canvas.SetTop(boost, ball.posY);
            Canvas.SetLeft(boost, ball.posX);

            posX = ball.posX;
            posY = ball.posY;

            _power = booster.GetPower();
        }
        public Power GetPower()
        {
            return _power;
        }
        public void SetPower(Power power)
        {
            _power = power;
        }

        public void RandomPower(int number = 6)
        {
            switch (Tools.Tools.RundomNumber(1, number))
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
                case 4:
                    _power = Power.StickyPlayer;
                    break;
                case 5:
                    _power = Power.Shooting;
                    break;
                case 6:
                    _power = Power.SkipLevel;
                    break;
                default:
                    _power = Power.None;
                    break;
            }
        }

        public void RandomPowerWithCondition()
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

        public void SetBoostPlayerLenght(ref Rectangle rectangle) //powiększamy gracza
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
                Height = 10,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0000ff")),
                Tag = "ballEclipse",
            };

            myCanvas.Children.Add(ballEclipse);
            Canvas.SetTop(ballEclipse, balls[0].posY);
            Canvas.SetLeft(ballEclipse, balls[0].posX);

            Ball ball = new Ball();
            ball.InitBall(ballEclipse, ball.position*-1);
            ball.stop = false;
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
    }
}
