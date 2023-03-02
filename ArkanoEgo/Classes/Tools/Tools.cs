using ArkanoEgo.Classes.Bricks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;

namespace ArkanoEgo.Classes.Tools
{
    public static class Tools
    {
        private static Random rnd = new Random();
        public static string info;
        public static int PointsAtLevel = 0;
        public static int NumberOfBricks = 0;
        [XmlRoot("XMLBricks")]
        public class ListBricks
        {
            public ListBricks() { Bricks = new List<XMLBrick>(); }

            [XmlElement("XMLBrick")]
            public List<XMLBrick> Bricks { get; set; }
        }

        public static ListBricks listBricks = new ListBricks();
        public static Brick[,] ReadLvl(int lvl)
        {
            return JustReadLvl("LVLS", "lvl_" + lvl.ToString());
        }
        public static Brick[,] ReadLvl(string path)
        {
            return JustReadLvl(@"CustomLVLS", path);
        }
        private static Brick[,] JustReadLvl(string path, string lvl) //wstępne czytanie mapy z pliku
        {
            Brick[,] Bricks = new Brick[13, 21];
            int bricksCount = 0;
            int levelPoints = 0;
            info = "";
            var serializer = new XmlSerializer(typeof(ListBricks));
            using (var reader = XmlReader.Create($@"{path}\{lvl}.xml"))
            {
                listBricks = (ListBricks)serializer.Deserialize(reader);
            }
            foreach (var Brick in listBricks.Bricks)
            {
                levelPoints += Brick.Value;
                info += "Brick: x" + Brick.PosX + ", y" + Brick.PosY + " ttb:> " + Brick.TimesToBreak + "\n";
                switch (Brick.Type)
                {
                    case 1:
                        Bricks[Brick.PosX, Brick.PosY] = new SimpleBrick(Brick.Color, Brick.Value);
                        bricksCount++;
                        break;
                    case 2:
                        Bricks[Brick.PosX, Brick.PosY] = new SilverBrick(Brick.Value, Brick.TimesToBreak);
                        bricksCount++;
                        break;
                    case 3:
                        Bricks[Brick.PosX, Brick.PosY] = new GoldBrick();
                        break;
                    default: break;
                }
            }
            
            NumberOfBricks = bricksCount;
            PointsAtLevel = levelPoints;
            return Bricks;
        }

        public static int RundomNumber(int from, int to)
        {
            return rnd.Next(from, to + 1);
        }
        public static void SpawnBall(ref Canvas myCanvas, ref List<Ball> balls, Rectangle rectangle)
        {
            Ellipse ballEclipse = new Ellipse()
            {
                Width = 10,
                Height = 10,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0000ff")),
                Tag = "ballEclipse",
            };

            myCanvas.Children.Add(ballEclipse);
            Canvas.SetTop(ballEclipse, Canvas.GetTop(rectangle) - ballEclipse.Height);
            Canvas.SetLeft(ballEclipse, Canvas.GetLeft(rectangle) + (rectangle.Width/2) - (ballEclipse.Width/2));

            Ball ball = new Ball();
            ball.InitBall(ballEclipse);
            balls.Add(ball);
        }
        public static void SpawnShoots(ref Canvas myCanvas, ref List<Ball> balls, Rectangle rectangle)
        {
            Ellipse ballEclipse = new Ellipse()
            {
                Width = 5,
                Height = 5,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff")),
                Tag = "ballEclipse",
            };

            myCanvas.Children.Add(ballEclipse);

            Canvas.SetTop(ballEclipse, Canvas.GetTop(rectangle) - ballEclipse.Height - 3);
            Canvas.SetLeft(ballEclipse, Canvas.GetLeft(rectangle));

            Ball ball = new Ball();
            ball.InitBall(ballEclipse);
            ball.trajectoryX = 0;
            ball.iAmShoot = true;
            ball.stop = false;
            balls.Add(ball);

            ballEclipse = new Ellipse()
            {
                Width = 5,
                Height = 5,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff")),
                Tag = "ballEclipse",
            };

            myCanvas.Children.Add(ballEclipse);

            Canvas.SetTop(ballEclipse, Canvas.GetTop(rectangle) - ballEclipse.Height - 3);
            Canvas.SetLeft(ballEclipse, Canvas.GetLeft(rectangle) + rectangle.Width);

            ball = new Ball();
            ball.InitBall(ballEclipse);
            ball.trajectoryX = 0;
            ball.iAmShoot = true;
            ball.stop = false;
            balls.Add(ball);
        }

        public static void CalculateTrajectory(Rect blockHitBox, Rect ballEclipseHitBox,Rectangle x, Ellipse ball, ref List<Ball> balls, int index)
        {
            if (ballEclipseHitBox.IntersectsWith(blockHitBox))
            {
                if (Canvas.GetLeft(x) < Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 10) > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].trajectoryX = 1.5;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 10) <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 10) * 2 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].trajectoryX = 1.3;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 10) * 2 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 10) * 3 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].trajectoryX = 1;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 10) * 3 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 10) * 4 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].trajectoryX = 0.6;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 10) * 4 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 10) * 5 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].trajectoryX = 0.3;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 10) * 5 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 10) * 6 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].trajectoryX = 0.3;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 10) * 6 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 10) * 7 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].trajectoryX = 0.6;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 10) * 7 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 10) * 8 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].trajectoryX = 1;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 10) * 8 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 10) * 9 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].trajectoryX = 1.3;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 10) * 9 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 10) * 10 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].trajectoryX = 1.5;
                }
            }
        }
    }
}
