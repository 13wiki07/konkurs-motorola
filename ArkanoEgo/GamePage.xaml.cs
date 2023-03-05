using ArkanoEgo.Classes;
using ArkanoEgo.Classes.Bricks;
using ArkanoEgo.Classes.Struct;
using ArkanoEgo.Classes.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ArkanoEgo
{
    public partial class GamePage : Page
    {
        bool customLvl = false;
        string maincolor = "violet";
        bool playerGoRight = false;
        bool playerGoLeft = false;
        bool gamePlay = true;
        bool mouseControl = false;

        public int levelek = 1; // 1-32 levele, 33 - DOH
        public int points = 0;
        public int allPoints = 0;
        public int pointsLeft = 0;

        public int skipSpace = 0;
        public int shoots = 0;
        public int hearts = 3; // życia gracza
        public bool reloadedShoot = false;
        public bool stickyPlayer = false;

        //boss mechanicks
        public bool betterHit = false;
        public int changeOrientation = 0;
        public bool UnChangeOrientation = false;

        public Brick[,] bricks = new Brick[13, 21];
        public int numberOfBricksLeft = 0;

        public bool changeHeadsDirections;
        public List<int> headsDirections = new List<int>();

        DispatcherTimer gameTimer = new DispatcherTimer();
        DispatcherTimer reloadingShoot = new DispatcherTimer();
        DispatcherTimer changeHeadsDirectionsTimer = new DispatcherTimer();
        DispatcherTimer BossHitTimer = new DispatcherTimer();
        List<Ball> balls = new List<Ball>();
        Booster booster = new Booster();

        //wymiary Canvas'a / pola gry
        int height;
        int width;

        const int tickRate = 10;
        Physics Physics = new Physics(tickRate);
        CartesianPosition CurrentPosition;

        private void PlayMusic_Loaded(object sender, RoutedEventArgs e)
        {
            if (levelek == 32)
                (Application.Current.MainWindow as MainWindow).musicPlayer.Source = new Uri(@"..\..\Resources\Music\LobbyMusic_v11.mp3", UriKind.RelativeOrAbsolute);
            if (levelek == 33)
                (Application.Current.MainWindow as MainWindow).musicPlayer.Source = new Uri(@"..\..\Resources\Music\Prequel_lvl1.mp3", UriKind.RelativeOrAbsolute);
            (Application.Current.MainWindow as MainWindow).musicPlayer.Play();
        }
        public GamePage() // normalna gra, lvl 1
        {
            InitializeComponent();
            customLvl = false;
            bricks = Tools.ReadLvl(levelek); //Wczytywanie mapy
            levelTB.Text = "Level " + levelek;
            Game();
        }

        public GamePage(int level, int allpkt) // next level
        {
            InitializeComponent();
            customLvl = false;
            levelek = level;
            allPoints = allpkt;
            levelTB.Text = "Level " + levelek;
            points = 0;
            bricks = Tools.ReadLvl(levelek);
            levelTB.Text = "Level DOH";
            Game();
        }
        public GamePage(string path) // custom level
        {
            InitializeComponent();
            levelek = 1;
            customLvl = true;
            bricks = Tools.ReadLvl(path);
            levelTB.Text = "Level " + (path.StartsWith("lvl_") ? path.Substring(4) : path);
            Game();
        }

        public GamePage(int allpkt)
        {
            InitializeComponent();
            customLvl = false;
            levelek = 33;
            levelTB.Text = "Level DOH";
            //powerTextBlock.Visibility = Visibility.Visible;
            powerIcon.Visibility = Visibility.Visible;
            Game();
            DohLvL();
        }

        private void Game()
        {
            height = (int)myCanvas.Height;
            width = (int)myCanvas.Width;

            foreach (var x in myCanvas.Children.OfType<Ellipse>().Where(x => x.Tag.ToString() == "ballEclipse"))
            {
                Ball ball = new Ball();
                ball.InitBall(x, 0);
                balls.Add(ball);
            }

            // to jest po to, by klocki nie miały wymiarów w double tak samo jak canvas
            height = 800 / 13;
            height = height * 13;

            if (levelek != 33 && levelek != 0)
            {
                numberOfBricksLeft = Tools.NumberOfBricks;
                Brick.GenerateElements(ref myCanvas, ref bricks, width, height);
                myCanvas.Focus();
            }

            pointsLabel.Content = "" + allPoints;
            heartsTextBlock.Text = "" + hearts;
            pointsLeft = Tools.PointsAtLevel;
            numberOfBricksLeft = Tools.NumberOfBricks;

            if (levelek == 33 && customLvl == false)
            {
                DohLvL();
                Tools.PointsAtLevel = 3500;
            }

            //Pętla gry
            gameTimer.Interval = TimeSpan.FromMilliseconds(30);
            gameTimer.Tick += new EventHandler(GameTimerEvent);
            gameTimer.Start();
            reloadingShoot.Interval = TimeSpan.FromMilliseconds(3000);
            reloadingShoot.Tick += new EventHandler(Shooting);
        }

        private void GameTimerEvent(object sender, EventArgs e)
        {
            try
            {
                OnLoseAllBalls();
                if (mouseControl)
                {
                    Grid_MouseMove();
                }
                for (int i = 0; i < tickRate; i++)
                {
                    bool isTheSameBrick = false; // potrzebne, by naprawić błąd z kilkukrotnym zbiciem
                    foreach (var x in myCanvas.Children.OfType<Rectangle>()) //kolizja piłek
                    {
                        bool leave = false;
                        if (!isTheSameBrick && x.Name != "player" && x.Name != "boss") //jeżeli element jest blokiem to go usun
                        {
                            int posX = (int)Canvas.GetLeft(x) / (width / 13); //element [x,0] tablicy
                            int posY = (int)Canvas.GetTop(x) / (height / 26); //element [0,y] tablicy
                            Rect BlockHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                            Rect ballEclipseHitBox;

                            foreach (var (ball, index) in myCanvas.Children.OfType<Ellipse>().Where(ball => ball.Tag.ToString() == "ballEclipse").Select((ball, index) => (ball, index)))
                            {
                                leave = false;
                                ballEclipseHitBox = new Rect(Canvas.GetLeft(ball), Canvas.GetTop(ball), ball.Width, ball.Height);

                                if (!isTheSameBrick && ballEclipseHitBox.IntersectsWith(BlockHitBox))
                                {
                                    // górna krawędź klocka
                                    if (balls[index].posY + balls[index].rad < Canvas.GetTop(x))
                                    {
                                        if (booster.GetPower() != Power.StrongerHit && levelek == 33)
                                        {
                                            balls[index].top = true;
                                        }
                                        else if (levelek != 0 && levelek != 33)
                                        {
                                            if (booster.GetPower() != Power.StrongerHit || bricks[posX, posY].GetType() == typeof(GoldBrick))
                                            {
                                                balls[index].top = true;
                                            }
                                        }

                                        HitBlock(posX, posY, x, index);
                                        isTheSameBrick = true;
                                        leave = true;
                                    }

                                    // dolna krawędź klocka
                                    else if (balls[index].posY + balls[index].rad > Canvas.GetTop(x) + x.Height)
                                    {
                                        if (booster.GetPower() != Power.StrongerHit && levelek == 33)
                                        {
                                            balls[index].top = false;
                                        }
                                        else if (levelek != 0 && levelek != 33)
                                        {
                                            if (booster.GetPower() != Power.StrongerHit || bricks[posX, posY].GetType() == typeof(GoldBrick))
                                            {
                                                balls[index].top = false;
                                            }
                                        }

                                        HitBlock(posX, posY, x, index);
                                        isTheSameBrick = true;
                                        leave = true;
                                    }

                                    // lewa krawędź klocka
                                    else if (balls[index].posX + balls[index].rad < Canvas.GetLeft(x))
                                    {
                                        if (booster.GetPower() != Power.StrongerHit && levelek == 33)
                                        {
                                            balls[index].left = true;
                                        }
                                        else if (levelek != 0 && levelek != 33)
                                        {
                                            if (booster.GetPower() != Power.StrongerHit || bricks[posX, posY].GetType() == typeof(GoldBrick))
                                            {
                                                balls[index].left = true;
                                            }
                                        }

                                        HitBlock(posX, posY, x, index);
                                        isTheSameBrick = true;
                                        leave = true;
                                    }

                                    // prawa krawędź klocka
                                    else if (balls[index].posX + balls[index].rad > Canvas.GetLeft(x) + x.Width)
                                    {
                                        if (booster.GetPower() != Power.StrongerHit && levelek == 33)
                                        {
                                            balls[index].left = false;
                                        }
                                        else if (levelek != 0 && levelek != 33)
                                        {
                                            if (booster.GetPower() != Power.StrongerHit || bricks[posX, posY].GetType() == typeof(GoldBrick))
                                            {
                                                balls[index].left = false;
                                            }
                                        }

                                        HitBlock(posX, posY, x, index);
                                        isTheSameBrick = true;
                                        leave = true;
                                    }
                                    if (balls[index].iAmShoot)
                                    {
                                        balls.RemoveAt(index);
                                        myCanvas.Children.Remove(ball);
                                        leave = true;
                                    }
                                }
                                if (leave || isTheSameBrick) //wyjście z foreacha, bo usuwamy jeden z jego elementów
                                    break;
                            }
                            if (leave)
                                break;
                        }
                        if (x.Name == "player") //jeżeli element jest graczem to się od niego odbij
                        {
                            Rect blockHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                            Rect ballEclipseHitBox;

                            foreach (var (ball, index) in myCanvas.Children.OfType<Ellipse>().Where(ball => ball.Tag.ToString() == "ballEclipse").Select((ball, index) => (ball, index))) //sprawdzanie czy jakaś piłka nie dotkła paletki
                            {
                                bool gotHit = false;
                                ballEclipseHitBox = new Rect(Canvas.GetLeft(ball), Canvas.GetTop(ball), ball.Width, ball.Height);
                                gotHit = Tools.CalculateTrajectory(blockHitBox, ballEclipseHitBox, x, ball, ref balls, index, stickyPlayer, tickRate);
                                if (!gotHit)
                                {
                                    for (int b = 0; b < myCanvas.Children.OfType<Ellipse>().Where(deletedBall => deletedBall.Tag.ToString() == "ballEclipse").Count(); b++)
                                    {
                                        balls.RemoveAt(b);
                                        myCanvas.Children.Remove(myCanvas.Children.OfType<Ellipse>().Where(deletedBall => deletedBall.Tag.ToString() == "ballEclipse").ElementAt(b));
                                    }
                                    //OnLoseAllBalls();
                                    return;
                                }
                            }
                            //kolizja gracza z boostem
                            if (PlayerCaughtABoost(blockHitBox)) { break; }//kontakt paletki z boostem
                        }
                        if (x.Name == "boss")
                        {
                            Rect blockHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                            Rect ballEclipseHitBox;
                            foreach (var (ball, index) in myCanvas.Children.OfType<Ellipse>().Where(ball => ball.Tag.ToString() == "ballEclipse").Select((ball, index) => (ball, index))) //sprawdzanie czy jakaś piłka nie dotkła paletki
                            {
                                if (balls[index].iAmBossShoot) break;
                                ballEclipseHitBox = new Rect(Canvas.GetLeft(ball), Canvas.GetTop(ball), ball.Width, ball.Height);
                                if (ballEclipseHitBox.IntersectsWith(blockHitBox))
                                {
                                    if (booster.GetPower() == Power.StrongerHit)
                                    {
                                        points += 210;
                                        allPoints += 210;
                                    }
                                    else
                                    {
                                        points += 70;
                                        allPoints += 70;
                                    }
                                    pointsLabel.Content = "" + allPoints;

                                    if (points >= Tools.PointsAtLevel)
                                    {
                                        MessageBox.Show("Wygrana :)");
                                        gameTimer.Stop();
                                        NavigationService.Navigate(new MenuPage());
                                    }
                                    RespawnBoost(index);
                                    if (balls[index].posY + balls[index].rad < Canvas.GetTop(x))
                                    {
                                        balls[index].top = true;
                                        leave = true;
                                    }

                                    // dolna krawędź klocka
                                    else if (balls[index].posY + balls[index].rad > Canvas.GetTop(x) + x.Height)
                                    {
                                        balls[index].top = false;
                                        leave = true;
                                    }

                                    // lewa krawędź klocka
                                    else if (balls[index].posX + balls[index].rad < Canvas.GetLeft(x))
                                    {
                                        balls[index].left = true;
                                        leave = true;
                                    }

                                    // prawa krawędź klocka
                                    else if (balls[index].posX + balls[index].rad > Canvas.GetLeft(x) + x.Width)
                                    {
                                        balls[index].left = false;
                                        leave = true;
                                    }
                                    if (balls[index].iAmShoot)
                                    {
                                        balls.RemoveAt(index);
                                        myCanvas.Children.Remove(ball);
                                        leave = true;
                                    }
                                }
                                if (leave)
                                    break;
                            }
                        }
                        if (leave)
                            break;
                    }

                    if (!mouseControl)
                    {
                    if (playerGoRight && !playerGoLeft)
                        PlayerMovement(true);
                    if (playerGoLeft && !playerGoRight)
                        PlayerMovement(false);
                    }


                    for (int j = 0; j < myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "ballEclipse").Count(); j++)
                    {
                        if (!BallMovement(j))
                        {
                            break;
                        }
                    }

                    for (int j = 0; j < myCanvas.Children.OfType<Rectangle>().Where(element => element.Tag.ToString() == "bossHeads").Count(); j++)
                    {
                        BossHeadsMovement(j);
                    }

                    BoostMovement();

                    foreach (var (element, index) in myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "ballEclipse").Select((element, index) => (element, index)))
                    {
                        if (Canvas.GetTop(element) > Canvas.GetTop(player))
                        {
                            myCanvas.Children.Remove(element);
                            balls.RemoveAt(index);
                            if (balls.Count == 0) return;
                            break;
                        }
                    }

                    foreach (var x in myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "Booster"))
                    {
                        if (Canvas.GetTop(x) > Canvas.GetTop(player))
                        {
                            myCanvas.Children.Remove(x);
                            break;
                        }
                    }
                }
                if (Canvas.GetLeft(player) >= width) Next_Level();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "" + ex.StackTrace);
            }

        }

        private void BossHeadsMovement(int index)
        {
            double posX = Canvas.GetLeft(myCanvas.Children.OfType<Rectangle>().Where(element => element.Tag.ToString() == "bossHeads").ElementAt(index));
            double posY = Canvas.GetTop(myCanvas.Children.OfType<Rectangle>().Where(element => element.Tag.ToString() == "bossHeads").ElementAt(index));



            if (posX < 15 || posX + 50 > 780 || posY > 600 || posY < 15)
            {
                if (posX > 400 && posY > 400)
                    headsDirections[index] = 4;
                if (posX < 400 && posY > 400)
                    headsDirections[index] = 3;
                if (posX > 400 && posY < 400)
                    headsDirections[index] = 2;
                if (posX < 400 && posY < 400)
                    headsDirections[index] = 1;
            }

            if (headsDirections.Count == 0)
            {
                return;
            }

            switch (headsDirections[index])
            {
                case 1:
                    posX += 0.5;
                    posY += 0.5;
                    break;
                case 2:
                    posX += -0.5;
                    posY += 0.5;
                    break;
                case 3:
                    posX += 0.5;
                    posY += -0.5;
                    break;
                case 4:
                    posX += -0.5;
                    posY += -0.5;
                    break;
            }

            Canvas.SetLeft(myCanvas.Children.OfType<Rectangle>().Where(element => element.Tag.ToString() == "bossHeads").ElementAt(index), posX);
            Canvas.SetTop(myCanvas.Children.OfType<Rectangle>().Where(element => element.Tag.ToString() == "bossHeads").ElementAt(index), posY);
        }
        private bool BallMovement(int index, int goLeft = 0)
        {
            //tablica balls i jej odpowiednik na planszy mają ten sem index
            balls[index].posX = Canvas.GetLeft(myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "ballEclipse").ElementAt(index));
            balls[index].posY = Canvas.GetTop(myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "ballEclipse").ElementAt(index));
            CurrentPosition = Physics.ExtractValue(balls[index].left, balls[index].top, balls[index].position);
            if (balls[index].stop != true) //przyklejamy piłkę do paletki, do momentu wciśnięcia spacji
            {
                balls[index].posX += CurrentPosition.HorizontalPosition;
                balls[index].posY += CurrentPosition.VerticalPosition;
            }
            else if (balls[index].stop && balls[index].posY + balls[index].rad * 2 > Canvas.GetTop(player) - 3)
            {
                balls[index].posY -= 3;
                Canvas.SetTop(myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "ballEclipse").ElementAt(index), balls[index].posY - 3);
            }
            else balls[index].posX += goLeft;

            Canvas.SetLeft(myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "ballEclipse").ElementAt(index), balls[index].posX);
            Canvas.SetTop(myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "ballEclipse").ElementAt(index), balls[index].posY);

            if (!ChangeBallDirection(index))
            {
                myCanvas.Children.Remove(myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "ballEclipse").ElementAt(index));
                return false;
            }
            return true;
        }

        private bool ChangeBallDirection(int index)
        {
            if (balls[index].posY <= 0)// góra
            {
                if (balls[index].iAmShoot)
                {
                    balls.RemoveAt(index);
                    return false;
                }
                balls[index].top = false;
            }
            if (balls[index].posX <= 0) balls[index].left = false; // lewy bok
            if (balls[index].posX >= width - (balls[index].rad * 2)) balls[index].left = true;// prawy bok
            return true;
        }

        private void BoostMovement()
        {
            foreach (var x in myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "Booster"))
            {
                booster.posX = (int)Canvas.GetLeft(x);
                booster.posY = (int)Canvas.GetTop(x);

                booster.posY += 1;

                Canvas.SetLeft(x, booster.posX);
                Canvas.SetTop(x, booster.posY);
            }
        }

        private void PlayerMovement(bool direction, int playerSpeed = 2)
        {
            for (int i = 0; i < playerSpeed; i++)
            {
                if (Canvas.GetLeft(player) + (player.Width) < width + skipSpace && direction)
                {
                    Canvas.SetLeft(player, Canvas.GetLeft(player) + 1);
                    for (int j = 0; j < balls.Count; j++) //ruch przyklejonych piłek do paletki
                    {
                        if (balls[j].stop == true)
                            BallMovement(j, 1);
                    }
                }
                if (Canvas.GetLeft(player) > 0 && !direction)
                {
                    Canvas.SetLeft(player, Canvas.GetLeft(player) - 1);
                    for (int j = 0; j < balls.Count; j++)
                    {
                        if (balls[j].stop == true)
                            BallMovement(j, -1);
                    }
                }
            }
        }

        private void myCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            /*booster.SetPower(Power.None);
            SetBoost();
            MessageBox.Show("Booster: " + booster.GetPower());
            booster.SetPower(Power.NewBall);
            SetBoost();
            MessageBox.Show("Booster: " + booster.GetPower());
            booster.SetPower(Power.PlayerLenght);
            SetBoost();
            MessageBox.Show("Booster: " + booster.GetPower());
            booster.SetPower(Power.StickyPlayer);
            SetBoost();
            MessageBox.Show("Booster: " + booster.GetPower());
            booster.SetPower(Power.Shooting);
            SetBoost();
            MessageBox.Show("Booster: " + booster.GetPower());
            booster.SetPower(Power.StrongerHit);
            SetBoost();
            MessageBox.Show("Booster: " + booster.GetPower());*/

            switch (e.Key)
            {
                case Key.Left:
                    e.Handled = true;
                    break;
                case Key.Right:
                    e.Handled = true;
                    break;
                case Key.Up:
                    e.Handled = true;
                    break;
                case Key.Down:
                    e.Handled = true;
                    break;
                case Key.Tab:
                    e.Handled = true;
                    break;
                default:
                    break;
            }

            if (e.Key == Key.D) playerGoRight = true;

            if (e.Key == Key.A) playerGoLeft = true;

            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl) Shot();

            if (e.Key == Key.Space) //wypuszczenie wszystkich piłek
            {
                for (int i = 0; i < balls.Count; i++)
                {
                    balls[i].stop = false;
                }
            }
        }

        private void myCanvas_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D) playerGoRight = false;
            if (e.Key == Key.A) playerGoLeft = false;
        }

        public void SetBoost()
        {
            if(booster.GetPower() != Power.None)
                powerIcon.Visibility = Visibility.Visible;

            switch (booster.GetPower())
            {
                case Power.PlayerLenght:
                    booster.SetBoostPlayerLenght(ref player);

                    powerTextBlock.Text = "Player lenght";
                    powerIcon.Source = new BitmapImage(new Uri(@"Resources/Images/player-length.png", UriKind.Relative));
                    break;
                case Power.NewBall:
                    booster.NewBallSetBoost(ref myCanvas, ref balls);
                    powerTextBlock.Text = "New ball";
                    powerIcon.Source = new BitmapImage(new Uri(@"Resources/Images/add-ball.png", UriKind.Relative));
                    break;
                case Power.StrongerHit:
                    booster.SetPower(Power.StrongerHit);
                    powerTextBlock.Text = "Stronger hit";
                    powerIcon.Source = new BitmapImage(new Uri(@"Resources/Images/stronger-hit.png", UriKind.Relative));
                    break;
                case Power.SkipLevel:
                    powerTextBlock.Text = "SkipLevel";
                    SkipLvl();
                    break;
                case Power.Shooting:
                    powerTextBlock.Text = "Shooting";
                    powerIcon.Source = new BitmapImage(new Uri(@"Resources/Images/shooting-mode.png", UriKind.Relative));
                    reloadingShoot.Start();
                    break;
                case Power.StickyPlayer:
                    powerTextBlock.Text = "Sticky player";
                    powerIcon.Source = new BitmapImage(new Uri(@"Resources/Images/sticky-player.png", UriKind.Relative));
                    stickyPlayer = true;
                    break;
                case Power.None:
                    powerTextBlock.Text = "";
                    break;
                default:
                    break;
            }
        }

        public void StopBoost()
        {
            powerIcon.Visibility = Visibility.Collapsed;

            if (booster.GetPower() != Power.None)
                powerIcon.Visibility = Visibility.Visible;

            switch (booster.GetPower())
            {
                case Power.PlayerLenght:
                    booster.StopBoostPlayerLenght(ref player);
                    break;
                case Power.NewBall:
                    //booster.StopBoost(ref player);
                    break;
                case Power.StrongerHit:
                    booster.SetPower(Power.None);
                    break;
                case Power.SkipLevel:
                    SkipLvl(false);
                    break;
                case Power.Shooting:
                    reloadedShoot = false;
                    reloadingShoot.Stop();
                    break;
                case Power.StickyPlayer:
                    stickyPlayer = false;
                    break;
                case Power.None:
                    break;
                default:
                    break;
            }
        }

        public void HitBlock(int posX, int posY, Rectangle rectangle, int indexOfBall) //akcja po trafieniu piłki w block
        {
            if (rectangle.Tag.ToString() == "bossHeads")
            {
                myCanvas.Children.Remove(rectangle);
                headsDirections.RemoveAt(headsDirections.Count - 1);
                return;
            }
            if (bricks[posX, posY].GetType() != typeof(GoldBrick)) //sprawdzanie czy obiekt nie jest GoldBrick (ten obiekt nie ma Value)
            {
                if (bricks[posX, posY].TimesToBreak < 2)
                {
                    myCanvas.Children.Remove(rectangle);

                    RespawnBoost(indexOfBall);

                    points += bricks[posX, posY].Value;
                    allPoints += bricks[posX, posY].Value;
                    pointsLabel.Content = "" + allPoints;

                    if (points == Tools.PointsAtLevel) Next_Level();
                }
                else
                {
                    bricks[posX, posY].TimesToBreak--;
                    return;
                }
            }
        }

        public bool PlayerCaughtABoost(Rect rect)
        {
            foreach (var g in myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "Booster"))
            {
                Rect boosterEclipseHitBox = new Rect(Canvas.GetLeft(g), Canvas.GetTop(g), g.Width, g.Height);
                if (boosterEclipseHitBox.IntersectsWith(rect))
                {
                    myCanvas.Children.Remove(g);
                    StopBoost();
                    if (levelek == 33 && customLvl == false)
                        booster.RandomPower(5);
                    else
                        booster.RandomPower();
                    SetBoost();
                    return true;
                }
            }
            return false;
        }

        public void RespawnBoost(int indexOfBall) //Po zniszczeniu bloku, jest 10% szans na to, że zrespi się nowy boost. Poprzedni wciąż jest aktywny
        {
            if (Tools.RundomNumber(1, 10) == 5)
            {
                if (myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "Booster").Count() == 0)
                    booster = new Booster(balls[indexOfBall], ref myCanvas, booster);
            }
        }

        public void OnLoseAllBalls()
        {
            heartsTextBlock.Text = "" + hearts;

            if (balls.Count == 0 && hearts == 0)//przegrana
            {
                MessageBox.Show("Przegrana :(");
                gameTimer.Stop();
                NavigationService.Navigate(new MenuPage());
                return;
            }
            else if (balls.Count == 0 && hearts > 0)
            {
                Tools.SpawnBall(ref myCanvas, ref balls, player);
                hearts--;
            }
        }

        public void Next_Level()
        {
            gameTimer.Stop();
            levelek++;

            if (customLvl)
            {
                MessageBox.Show("End of level. Click OK to back to the gallery.", "Test end", MessageBoxButton.OK);
                //MessageBox.Show("End of the level. Click OK to go back to gallery.", "Test end", MessageBoxButton.OK);
                NavigationService.Navigate(new GalleryPage());
            }
            else if (levelek == 33) // DOH
                NavigationService.Navigate(new GamePage(allPoints));
            else
                NavigationService.Navigate(new GamePage(levelek, allPoints));
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Image img = (Image)btn.Content;

            if (gamePlay)
            {
                gameTimer.Stop();
                gamePlay = false;
                myCanvas.Focus();
                img.Source = new BitmapImage(new Uri(@"Resources/Images/play-white.png", UriKind.Relative));
            }
            else
            {
                gameTimer.Start();
                gamePlay = true;
                myCanvas.Focus();
                img.Source = new BitmapImage(new Uri(@"Resources/Images/pause-white.png", UriKind.Relative));
            }
        }

        private void ChangeMouseControl_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Image img = (Image)btn.Content;

            if (mouseControl)
            {
                playerGoRight = false;
                playerGoLeft = false;
                mouseControl = false;
                img.Source = new BitmapImage(new Uri(@"Resources/Images/keyboard.png", UriKind.Relative));
            }
            else
            {
                mouseControl = true;
                img.Source = new BitmapImage(new Uri(@"Resources/Images/mouse.png", UriKind.Relative));
            }

            myCanvas.Focus();
        }

        private void Button_MouseEvent(object sender, MouseEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow).changeColors(sender as Button, maincolor);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if(customLvl)
                NavigationService.Navigate(new GalleryPage());
            else
                NavigationService.Navigate(new MenuPage());
        }
        private void Shot()
        {
            if (reloadedShoot)
            {
                Tools.SpawnShoots(ref myCanvas, ref balls, player);
                //shoots--;
                reloadedShoot = false;
            }
        }
        private void SkipLvl(bool skip = true)
        {
            if (skip)
            {
                skipSpace = (int)player.Width;
                door.Visibility = Visibility.Visible;
            }
            else
            {
                door.Visibility = Visibility.Hidden;
                skipSpace = 0;
                if (Canvas.GetLeft(player) + player.Width > width)
                    Canvas.SetLeft(player, width - player.Width);
            }
        }


        private void RotateCanvas()
        {
            RotateTransform rotateTransform = new RotateTransform(180);
            rotateTransform.CenterX = 396;
            rotateTransform.CenterY = 413;
            myCanvas.RenderTransform = rotateTransform;

            rotateTransform = new RotateTransform(180);
            rotateTransform.CenterX = 100;
            rotateTransform.CenterY = 150;
            foreach (var x in myCanvas.Children.OfType<Rectangle>().Where(element => element.Tag.ToString() == "boss"))
            {
                x.RenderTransform = rotateTransform;
            }

            rotateTransform = new RotateTransform(180);
            rotateTransform.CenterX = 25;
            rotateTransform.CenterY = 38;
            foreach (var x in myCanvas.Children.OfType<Rectangle>().Where(element => element.Tag.ToString() == "bossHeads"))
            {
                x.RenderTransform = rotateTransform;
            }
        }
        private void UnRotateCanvas()
        {
            RotateTransform rotateTransform = new RotateTransform(0);
            rotateTransform.CenterX = 396;
            rotateTransform.CenterY = 413;
            myCanvas.RenderTransform = rotateTransform;


            rotateTransform = new RotateTransform(0);
            rotateTransform.CenterX = 100;
            rotateTransform.CenterY = 150;
            foreach (var x in myCanvas.Children.OfType<Rectangle>().Where(element => element.Tag.ToString() == "boss"))
            {
                x.RenderTransform = rotateTransform;
            }

            rotateTransform = new RotateTransform(0);
            rotateTransform.CenterX = 25;
            rotateTransform.CenterY = 38;
            foreach (var x in myCanvas.Children.OfType<Rectangle>().Where(element => element.Tag.ToString() == "bossHeads"))
            {
                x.RenderTransform = rotateTransform;
            }
        }

        private void ChangeHeadsDirection(object sender, EventArgs e)
        {
            headsDirections.Clear();
            for (int j = 0; j < myCanvas.Children.OfType<Rectangle>().Where(element => element.Tag.ToString() == "bossHeads").Count(); j++)
            {
                int randomNumber = Tools.RundomNumber(1, 4);
                headsDirections.Add(randomNumber);
            }
        }
        
        private void DohLvL()
        {
            Tools.SpawnBoss(ref myCanvas);
            for (int j = 0; j < myCanvas.Children.OfType<Rectangle>().Where(element => element.Tag.ToString() == "bossHeads").Count(); j++)
            {
                int randomNumber = Tools.RundomNumber(1, 4);
                headsDirections.Add(randomNumber);
            }

            changeHeadsDirectionsTimer.Interval = TimeSpan.FromMilliseconds(300);
            changeHeadsDirectionsTimer.Tick += new EventHandler(ChangeHeadsDirection);

            BossHitTimer.Interval = TimeSpan.FromMilliseconds(5000);
            BossHitTimer.Tick += new EventHandler(DohHit);

            BossHitTimer.Start();
            changeHeadsDirectionsTimer.Start();

            myCanvas.Focus();
        }

        private void Grid_MouseMove()
        {
            System.Windows.Point position = Mouse.GetPosition(myCanvas);
            double pX = Math.Round(position.X);
            for(int i = 0; i < 10; i++)
            {

            if (pX > Canvas.GetLeft(player) + player.Width/2)
            {
                playerGoRight = true;
                playerGoLeft = false;
                if (playerGoRight && !playerGoLeft)
                    PlayerMovement(true);
                if (playerGoLeft && !playerGoRight)
                    PlayerMovement(false);
            }
            if (pX < Canvas.GetLeft(player) + player.Width / 2)
            {
                playerGoRight = false;
                playerGoLeft = true;
                if (playerGoRight && !playerGoLeft)
                    PlayerMovement(true);
                if (playerGoLeft && !playerGoRight)
                    PlayerMovement(false);
            }
            if (pX == Canvas.GetLeft(player) + player.Width / 2)
            {
                playerGoRight = true;
                playerGoLeft = true;
                if (playerGoRight && !playerGoLeft)
                    PlayerMovement(true);
                if (playerGoLeft && !playerGoRight)
                    PlayerMovement(false);
            }
            }
        }
        private void DohHit(object sender, EventArgs e)
        {
            if (betterHit)
            {
                Tools.SpawnBossHead(ref myCanvas, ref headsDirections, UnChangeOrientation);
                Tools.SpawnShoots(ref myCanvas, ref balls, player, true);
                betterHit = false;
                changeOrientation++;
            }
            else
            {
                Tools.SpawnShoots(ref myCanvas, ref balls, player, true);
                betterHit = true;
                changeOrientation++;
            }
            if (changeOrientation > 5 && !UnChangeOrientation)
            {
                RotateCanvas();
                UnChangeOrientation = true;
                changeOrientation = 0;
            }
            else if (changeOrientation > 5 && UnChangeOrientation)
            {
                UnRotateCanvas();
                UnChangeOrientation = false;
                changeOrientation = 0;
            }
        }
        private void Shooting(object sender, EventArgs e)
        {
            reloadedShoot = true;
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.XButton1 || e.ChangedButton == MouseButton.XButton2)
            {
                e.Handled = true;
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.XButton1 || e.ChangedButton == MouseButton.XButton2)
            {
                e.Handled = true;
            }

            base.OnMouseUp(e);
        }
    }
}