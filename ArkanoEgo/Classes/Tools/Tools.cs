using ArkanoEgo.Classes.Bricks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
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
                reader.Close();
                reader.Dispose();
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
        public static int RundomNumberWithConditions(List<int> conditions)
        {
            int num = rnd.Next(0, conditions.Count);
            return conditions[num];
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
            ball.InitBall(ballEclipse, 0);
            ball.top = true;
            balls.Add(ball);
        }
        public static void SpawnShoots(ref Canvas myCanvas, ref List<Ball> balls, Rectangle rectangle, bool bossShot = false)
        {
            Ellipse ballEclipse = new Ellipse()
            {
                Width = 5,
                Height = 5,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff")),
                Tag = "ballEclipse",
            };

            myCanvas.Children.Add(ballEclipse);

            if (bossShot)
            {
                Canvas.SetTop(ballEclipse, 375);
                Canvas.SetLeft(ballEclipse, 375);

                Ball ballBossShot = new Ball();
                ballBossShot.InitBall(ballEclipse, 10); //TODO Tu trzeba obliczyć
                ballBossShot.trajectoryX = 0;
                ballBossShot.iAmShoot = true;
                ballBossShot.iAmBossShoot = true;
                ballBossShot.stop = false;
                int x = 0;
                ballBossShot.left = CalculateBossShotTrajectory(rectangle, ref x);
                ballBossShot.position = x;
                ballBossShot.top = false;
                balls.Add(ballBossShot);
                return;
            }

            Canvas.SetTop(ballEclipse, Canvas.GetTop(rectangle) - ballEclipse.Height - 3);
            Canvas.SetLeft(ballEclipse, Canvas.GetLeft(rectangle));

            Ball ball = new Ball();
            ball.InitBall(ballEclipse, 0);
            ball.top = true;
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
            ball.InitBall(ballEclipse, 0);
            ball.top = true;
            ball.trajectoryX = 0;
            ball.iAmShoot = true;
            ball.stop = false;
            balls.Add(ball);
        }
        public static void SpawnBoss(ref Canvas myCanvas)
        {
            Rectangle RectangleEclipse = new Rectangle()
            {
                Width = 200,
                Height = 300,
                Fill = new ImageBrush(new BitmapImage(new Uri(@"../../Resources/Images/doh.png", UriKind.Relative))),
                Tag = "boss",
                Name = "boss",

            };
            myCanvas.Children.Add(RectangleEclipse);
            Canvas.SetTop(RectangleEclipse, 263);
            Canvas.SetLeft(RectangleEclipse, 300);
        }
        public static void SpawnBossHead(ref Canvas myCanvas, ref List<int> list,bool UnChangeOrientation = false)
        {
            Rectangle RectangleEclipse = new Rectangle()
            {
                Width = 50,
                Height = 76,
                Fill = new ImageBrush(new BitmapImage(new Uri(@"../../Resources/Images/doh.png", UriKind.Relative))),
                Tag = "bossHeads",
            };
            if (UnChangeOrientation)
            {
                RotateTransform rotateTransform = new RotateTransform(180);
                rotateTransform.CenterX = 25;
                rotateTransform.CenterY = 38;
                RectangleEclipse.RenderTransform = rotateTransform;
            }
            myCanvas.Children.Add(RectangleEclipse);
            Canvas.SetTop(RectangleEclipse, 375);
            Canvas.SetLeft(RectangleEclipse, 375);
            list.Add(1);
        }

        public static bool CalculateTrajectory(Rect blockHitBox, Rect ballEclipseHitBox,Rectangle x, Ellipse ball, ref List<Ball> balls, int index,bool stickyPlayer, int tickRate)
        {
            if (ballEclipseHitBox.IntersectsWith(blockHitBox))
            {
                if (stickyPlayer)
                {
                    balls[index].stop = true;
                }
                if (balls[index].iAmBossShoot) return false;
                if (Canvas.GetLeft(x) < Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].trajectoryX = 1.5;
                    balls[index].position = 1;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 2 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].trajectoryX = 1.3;
                    balls[index].position = 2;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 2 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 3 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].trajectoryX = 1;
                    balls[index].position = 3;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 3 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 4 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].trajectoryX = 1;
                    balls[index].position = 4;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 4 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 5 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].trajectoryX = 1;
                    balls[index].position = 5;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 5 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 6 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].trajectoryX = 1;
                    balls[index].position = 6;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 6 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 7 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].trajectoryX = 1;
                    balls[index].position = 7;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 7 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 8 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].trajectoryX = 0.6;
                    balls[index].position = 8;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 8 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 9 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].trajectoryX = 0.6;
                    balls[index].position = 9;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 9 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 10 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].trajectoryX = 0.6;
                    balls[index].position = 10;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 10 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 11 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].trajectoryX = 0.6;
                    balls[index].position = 11;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 11 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 12 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].trajectoryX = 0.6;
                    balls[index].position = 12;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 12 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 13 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].trajectoryX = 0.6;
                    balls[index].position = 13;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 13 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 14 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].trajectoryX = 0.6;
                    balls[index].position = 14;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 14 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 15 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].trajectoryX = 0.6;
                    balls[index].position = 15;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 15 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 16 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].trajectoryX = 0.6;
                    balls[index].position = 16;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 16 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 17 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].trajectoryX = 0.6;
                    balls[index].position = 17;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 17 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 18 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].trajectoryX = 0.6;
                    balls[index].position = 18;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 18 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 19 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].trajectoryX = 0.6;
                    balls[index].position = 19;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 19 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 20 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].trajectoryX = 0.6;
                    balls[index].position = 19;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 20 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 21 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].trajectoryX = 0.3;
                    balls[index].position = 19;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 21 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 22 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].trajectoryX = 0.3;
                    balls[index].position = 19;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 22 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 23 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].trajectoryX = 0.3;
                    balls[index].position = 18;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 23 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 24 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].trajectoryX = 0.3;
                    balls[index].position = 17;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 24 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 25 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].trajectoryX = 0.3;
                    balls[index].position = 16;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 25 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 26 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].trajectoryX = 0.3;
                    balls[index].position = 15;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 26 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 27 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].trajectoryX = 0.3;
                    balls[index].position = 14;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 27 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 28 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].trajectoryX = 0.3;
                    balls[index].position = 13;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 28 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 29 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].trajectoryX = 0.3;
                    balls[index].position = 12;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 29 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 30 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].trajectoryX = 0.3;
                    balls[index].position = 11;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 30 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 31 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].trajectoryX = 0.3;
                    balls[index].position = 10;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 31 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 32 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].trajectoryX = 0.3;
                    balls[index].position = 9;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 32 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 33 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].trajectoryX = 0.3;
                    balls[index].position = 8;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 33 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 34 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].trajectoryX = 0.3;
                    balls[index].position = 7;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 34 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 35 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].trajectoryX = 0.3;
                    balls[index].position = 6;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 35 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 36 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].trajectoryX = 0.3;
                    balls[index].position = 5;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 36 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 37 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].trajectoryX = 0.3;
                    balls[index].position = 4;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 37 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 38 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].trajectoryX = 0.3;
                    balls[index].position = 3;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 38 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 39 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].trajectoryX = 0.3;
                    balls[index].position = 2;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 39 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 40 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].trajectoryX = 0.3;
                    balls[index].position = 1;
                }
            }
            return true;
        }
        public static bool CalculateBossShotTrajectory(Rectangle x, ref int position)
        {
            if (Canvas.GetLeft(x)+x.Width/2 < (793 / 20))
            {
                position = 10;// całkiem po lewej

                return true;
            }
            else if (Canvas.GetLeft(x)+x.Width/2 >= (793 / 20) && Canvas.GetLeft(x)+x.Width/2 < (793 / 20) * 2)
            {
                position = 11;

                return true;
            }
            else if (Canvas.GetLeft(x)+x.Width/2 >= (793 / 20) * 2 && Canvas.GetLeft(x)+x.Width/2 < (793 / 20) * 3)
            {
                position = 12;

                return true;
            }
            else if (Canvas.GetLeft(x)+x.Width/2 >= (793 / 20) * 3 && Canvas.GetLeft(x)+x.Width/2 < (793 / 20) * 4)
            {
                position = 13;

                return true;
            }
            else if (Canvas.GetLeft(x)+x.Width/2 >= (793 / 20) * 4 && Canvas.GetLeft(x)+x.Width/2 < (793 / 20) * 5)
            {
                position = 14;

                return true;
            }
            else if (Canvas.GetLeft(x)+x.Width/2 >= (793 / 20) * 5 && Canvas.GetLeft(x)+x.Width/2 < (793 / 20) * 6)
            {
                position = 15;

                return true;
            }
            else if (Canvas.GetLeft(x)+x.Width/2 >= (793 / 20) * 6 && Canvas.GetLeft(x)+x.Width/2 < (793 / 20) * 7)
            {
                position = 16;

                return true;
            }
            else if (Canvas.GetLeft(x)+x.Width/2 >= (793 / 20) * 7 && Canvas.GetLeft(x)+x.Width/2 < (793 / 20) * 8)
            {
                position = 17;

                return true;
            }
            else if (Canvas.GetLeft(x)+x.Width/2 >= (793 / 20) * 8 && Canvas.GetLeft(x)+x.Width/2 < (793 / 20) * 9)
            {
                position = 18;

                return true;
            }
            else if (Canvas.GetLeft(x)+x.Width/2 >= (793 / 20) * 9 && Canvas.GetLeft(x)+x.Width/2 < (793 / 208) * 10)
            {
                position = 19;

                return true;
            }
            else if (Canvas.GetLeft(x)+x.Width/2 >= (793 / 20) * 10 && Canvas.GetLeft(x)+x.Width/2 < (793 / 20) * 11)
            {
                position = 19;
                return false;
            }
            else if (Canvas.GetLeft(x)+x.Width/2 >= (793 / 20) * 11 && Canvas.GetLeft(x)+x.Width/2 < (793 / 20) * 12)
            {
                position = 18;
                return false;
            }
            else if (Canvas.GetLeft(x)+x.Width/2 >= (793 / 20) * 12 && Canvas.GetLeft(x)+x.Width/2 < (793 / 20) * 13)
            {
                position = 17;
                return false;
            }
            else if (Canvas.GetLeft(x)+x.Width/2 >= (793 / 20) * 13 && Canvas.GetLeft(x)+x.Width/2 < (793 / 20) * 14)
            {
                position = 16;
                return false;
            }
            else if (Canvas.GetLeft(x)+x.Width/2 >= (793 / 20) * 14 && Canvas.GetLeft(x)+x.Width/2 < (793 / 20) * 15)
            {
                position = 15;
                return false;
            }
            else if (Canvas.GetLeft(x)+x.Width/2 >= (793 / 20) * 15 && Canvas.GetLeft(x)+x.Width/2 < (793 / 20) * 16)
            {
                position = 14;
                return false;
            }
            else if (Canvas.GetLeft(x)+x.Width/2 >= (793 / 20) * 16 && Canvas.GetLeft(x)+x.Width/2 < (793 / 20) * 17)
            {
                position = 13;
                return false;
            }
            else if (Canvas.GetLeft(x)+x.Width/2 >= (793 / 20) * 17 && Canvas.GetLeft(x)+x.Width/2 < (793 / 20) * 18)
            {
                position = 12;
                return false;
            }
            else if (Canvas.GetLeft(x)+x.Width/2 >= (793 / 20) * 18 && Canvas.GetLeft(x)+x.Width/2 < (793 / 20) * 19)
            {
                position = 11;
                return false;
            }
            else if (Canvas.GetLeft(x)+x.Width/2 >= (793 / 20) * 19 && Canvas.GetLeft(x)+x.Width/2 < (793 / 20) * 20)
            {
                position = 10;
                return false;
            }
            return true;
        }
    }
}
