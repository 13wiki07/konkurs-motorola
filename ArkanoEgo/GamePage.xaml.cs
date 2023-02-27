using ArkanoEgo.Classes;
using ArkanoEgo.Classes.Bricks;
using ArkanoEgo.Classes.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ArkanoEgo
{
    public partial class GamePage : Page
    {
        string maincolor = "violet";
        bool playerGoRight = false;
        bool playerGoLeft = false;
        bool gamePlay = true;

        public int levelek = 1;
        public int points = 0;
        public int allPoints = 0;
        public int pointsLeft = 0;

        public int hearts = 3; // życia gracza
        public Brick[,] bricks = new Brick[13, 21];
        public int numberOfBricksLeft = 0;

        DispatcherTimer gameTimer = new DispatcherTimer();
        List<Ball> balls = new List<Ball>();
        Booster booster = new Booster();


        //wymiary Canvas'a / pola gry
        int height;
        int width;

        public GamePage() // normalna gra, lvl 1
        {
            InitializeComponent();
            bricks = Tools.ReadLvl(levelek); //Wczytywanie mapy
            levelTB.Text = "Level " + levelek;
            Game();
        }

        public GamePage(int level, int allpkt) // next level
        {
            InitializeComponent();
            levelek = level;
            allPoints = allpkt;
            levelTB.Text = "Level " + levelek;
            points = 0;
            bricks = Tools.ReadLvl(levelek);
            Game();
        }
        public GamePage(string path) // custom level
        {
            InitializeComponent();

            bricks = Tools.ReadLvl(path);
            levelTB.Text = "Level " + (path.StartsWith("lvl_") ? path.Substring(4) : path);
            Game();
        }

        private void Game()
        {
            height = (int)myCanvas.Height;
            width = (int)myCanvas.Width;

            foreach (var x in myCanvas.Children.OfType<Ellipse>().Where(x => x.Tag.ToString() == "ballEclipse"))
            {
                Ball ball = new Ball();
                ball.InitBall(x);
                balls.Add(ball);
            }

            // to jest po to, by klocki nie miały wymiarów w double tak samo jak canvas
            height = 800 / 13;
            height = height * 13;

            Brick.GenerateElements(ref myCanvas, ref bricks, width, height);
            myCanvas.Focus();

            pointsLabel.Content = "" + allPoints;
            heatsLabel.Content = "" + hearts;

            pointsLeft = Tools.PointsAtLevel;
            numberOfBricksLeft = Tools.NumberOfBricks;

            //Pętla gry
            gameTimer.Interval = TimeSpan.FromMilliseconds(30);
            gameTimer.Tick += new EventHandler(GameTimerEvent);
            gameTimer.Start();
        }

        private void GameTimerEvent(object sender, EventArgs e)
        {
            heatsLabel.Content = "" + hearts;
            if (balls.Count == 0 && hearts == 0)
            {
                MessageBox.Show("Przegrana");
                gameTimer.Stop();
                NavigationService.Navigate(new MenuPage());
                return;
            }
            else if(balls.Count == 0 && hearts > 0)
            {
                Tools.SpawnBall(ref myCanvas, ref balls, player);
                hearts--;
            }

            for (int i = 0; i < 10; i++)
            {
                bool isTheSameBrick = false; // potrzebne, by naprawić błąd z kilkukrotnym zbiciem
                foreach (var x in myCanvas.Children.OfType<Rectangle>()) //kolizja piłek
                {
                    bool leave = false;
                    if (!isTheSameBrick && x.Name != "player") //jeżeli element jest blokiem to go usun
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
                                    if (booster.GetPower() != Power.StrongerHit || bricks[posX, posY].GetType() == typeof(GoldBrick))
                                        balls[index].top = true;

                                    HitBlock(posX, posY, x, index);
                                    isTheSameBrick = true;
                                    leave = true;
                                }

                                // dolna krawędź klocka
                                else if(balls[index].posY + balls[index].rad > Canvas.GetTop(x) + x.Height)
                                {
                                    if (booster.GetPower() != Power.StrongerHit || bricks[posX, posY].GetType() == typeof(GoldBrick))
                                        balls[index].top = false;

                                    HitBlock(posX, posY, x, index);
                                    isTheSameBrick = true;
                                    leave = true;
                                }

                                // lewa krawędź klocka
                                else if(balls[index].posX + balls[index].rad < Canvas.GetLeft(x))
                                {
                                    if (booster.GetPower() != Power.StrongerHit || bricks[posX, posY].GetType() == typeof(GoldBrick))
                                        balls[index].left = true;

                                    HitBlock(posX, posY, x, index);
                                    isTheSameBrick = true;
                                    leave = true;
                                }

                                // prawa krawędź klocka
                                else if(balls[index].posX + balls[index].rad > Canvas.GetLeft(x) + x.Width)
                                {
                                    if (booster.GetPower() != Power.StrongerHit || bricks[posX, posY].GetType() == typeof(GoldBrick))
                                        balls[index].left = false;

                                    HitBlock(posX, posY, x, index);
                                    isTheSameBrick = true;
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
                        Rect BlockHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                        Rect ballEclipseHitBox;

                        foreach (var (ball, index) in myCanvas.Children.OfType<Ellipse>().Where(ball => ball.Tag.ToString() == "ballEclipse").Select((ball, index) => (ball, index))) //sprawdzanie czy jakaś piłka nie dotkła paletki
                        {
                            ballEclipseHitBox = new Rect(Canvas.GetLeft(ball), Canvas.GetTop(ball), ball.Width, ball.Height);
                            if (ballEclipseHitBox.IntersectsWith(BlockHitBox))
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
                        //kolizja gracza z boostem
                        if (PlayerCaughtABoost(BlockHitBox)) { break; }//kontakt paletki z boostem
                    }
                }

                if (playerGoRight && !playerGoLeft)
                    PlayerMovement(true);
                if (playerGoLeft && !playerGoRight)
                    PlayerMovement(false);

                for (int j = 0; j < myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "ballEclipse").Count(); j++)
                {
                    BallMovement(j);
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
                    // Access `value` and `i` directly here.
                    if (Canvas.GetTop(x) > Canvas.GetTop(player))
                    {
                        myCanvas.Children.Remove(x);
                        break;
                    }
                }
            }
        }

        private void BallMovement(int index, int goLeft = 0)
        {
            //tablica balls i jej odpowiednik na planszy mają ten sem index
            balls[index].posX = Canvas.GetLeft(myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "ballEclipse").ElementAt(index));
            balls[index].posY = Canvas.GetTop(myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "ballEclipse").ElementAt(index));

            // dodaj wykonywanie co ileś milisekund (chyba najlepiej 6-10)
            if (balls[index].stop != true) //przyklejamy piłkę do paletki, do momentu wciśnięcia spacji
            {
                if (balls[index].left)
                    balls[index].posX -= balls[index].trajectoryX;
                else if (!balls[index].left)
                    balls[index].posX += balls[index].trajectoryX;

                if (balls[index].top)
                    balls[index].posY -= balls[index].trajectoryY;
                else if (!balls[index].top)
                    balls[index].posY += balls[index].trajectoryY;
            }
            else
                balls[index].posX += goLeft;

            Canvas.SetLeft(myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "ballEclipse").ElementAt(index), balls[index].posX);
            Canvas.SetTop(myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "ballEclipse").ElementAt(index), balls[index].posY);
            ChangeBallDirection(index);
        }

        private void ChangeBallDirection(int index)
        {
            if (balls[index].posY <= 0)
            {// góra
                balls[index].top = false; // odbicie
            }

            if (balls[index].posX <= 0)
            { // lewy bok
                balls[index].left = false;
            }

            if (balls[index].posX >= width - (balls[index].rad * 2))
            {// prawy bok
                balls[index].left = true;
            }
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

        private void PlayerMovement(bool direction)
        {
            int playerSpeed = 2;
            for (int i = 0; i < playerSpeed; i++)
            {
                if (Canvas.GetLeft(player) + (player.Width) < width && direction)
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
            if (e.Key == Key.D)
                playerGoRight = true;

            if (e.Key == Key.A)
                playerGoLeft = true;

            if (e.Key == Key.Space) //wypuszczenie wszystkich piłek
            {
                for (int j = 0; j < balls.Count; j++)
                {
                    balls[j].stop = false;
                }
            }
        }

        private void myCanvas_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D)
                playerGoRight = false;
            if (e.Key == Key.A)
                playerGoLeft = false;
        }

        public void SetBoost()
        {
            switch (booster.GetPower())
            {
                case Power.PlayerLenght:
                    booster.SetBoostPlayerLenght(ref player);
                    break;
                case Power.NewBall:
                    booster.NewBallSetBoost(ref myCanvas, ref balls);
                    break;
                case Power.StrongerHit:
                    booster.SetPower(Power.StrongerHit);
                    break;
                case Power.None:
                    break;
                default:
                    break;
            }
        }

        public void StopBoost()
        {
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
                case Power.None:
                    break;
                default:
                    break;
            }
        }

        public void HitBlock(int posX, int posY, Rectangle rectangle, int indexOfBall) //akcja po trafieniu piłki w block
        {
            if (bricks[posX, posY].GetType() != typeof(GoldBrick)) //sprawdzanie czy obiekt nie jest GoldBrick (ten obiekt nie ma Value)
            {
                if (bricks[posX, posY].TimesToBreak < 2)
                {
                    pointsLeft -= bricks[posX, posY].Value;
                    myCanvas.Children.Remove(rectangle);
                    RespawnBoost(indexOfBall);

                    points += bricks[posX, posY].Value;
                    allPoints += bricks[posX, posY].Value;
                    pointsLabel.Content = "" + allPoints;
                    
                    if (points == Tools.PointsAtLevel)
                        Next_Level();
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
                    booster.RandomPower();
                    //booster.SetPower(Power.StrongerHit); testowanie PowerUp'ów
                    SetBoost();
                    return true;
                }
            }
            return false;
        }

        public void RespawnBoost(int indexOfBall) //Po zniszczeniu bloku, jest 10% szans na to, że zrespi się nowy boost. Poprzedni wciąż jest aktywny
        {
            if (Tools.RundomNumber(1, 10) == 5)
                booster = new Booster(balls[indexOfBall], ref myCanvas, booster);
        }

        public void Next_Level()
        {
            gameTimer.Stop();
            levelek++;

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

        private void Button_MouseEvent(object sender, MouseEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow).changeColors(sender as Button, maincolor);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MenuPage());
        }
    }
}