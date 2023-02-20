using ArkanoEgo.Classes;
using ArkanoEgo.Classes.Bricks;
using ArkanoEgo.Classes.Tools;
using System;
using System.Collections.Generic;
using System.IO;
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
        bool playerGoRight = false;
        bool playerGoLeft = false;
        //bool raz = true;
        int[] dane = new int[3]; // 0. indeks  | 1. posX  | 2. posY
        // potrzebne to by ominąć błąd z liczeniem pozostałych klocków

        //bool ok = true;
        int nrr = 0;

        public int levelek = 1;
        public int points = 0;
        public Brick[,] bricks = new Brick[13, 21];
        //public int howManyBricksLeft = 0;

        // test
        public int numberOfChilds = 0;
        public int numberOfBricksLeft = 0;

        DispatcherTimer gameTimer = new DispatcherTimer();

        List<Ball> balls = new List<Ball>();

        Booster booster = new Booster();
        int pointsLeft = 0;
        //wymiary Canvas'a / pola gry
        int height;
        int width;

        public GamePage()
        {
            InitializeComponent();
            bricks = Tools.ReadLvl(levelek); //Wczytywanie mapy
            Game();
        }

        public GamePage(int level, int pkt)
        {
            InitializeComponent();
            levelek = level;
            points = 0;
            bricks = Tools.ReadLvl(levelek); //Wczytywanie mapy
            Game();
        }
        public GamePage(string path)
        {
            InitializeComponent();

            bricks = Tools.ReadLvl("2023-levelektest"); //Wczytywanie mapy
            MessageBox.Show("Count b: " + bricks.Length);
            Game();
        }

        private void Game()
        {
            height = (int) myCanvas.Height;
            width = (int) myCanvas.Width;


            foreach (var x in myCanvas.Children.OfType<Ellipse>().Where(x => x.Tag.ToString() == "ballEclipse"))
            {
                Ball ball = new Ball();
                ball.InitBall(x);
                balls.Add(ball);
            }

            // to jest po to, by klocki nie miały wymiarów w double tak samo jak canvas
            height = 800 / 13; //(int)SystemParameters.FullPrimaryScreenHeight / 13;
            height = height * 13;

            /* np.
             system -> 800
             /13 -> 61
             *13 -> 793 <= i to ma być szerokość i elo  */

            //windowPage.SetValue(WidthProperty, (double)(height));
            //windowPage.UpdateLayout();
            //width = (int)windowPage.Width;

            //bricks = Tools.ReadLvl(levelek); //Wczytywanie mapy
            pointsLeft = Tools.PointsAtLevel;
                                                                      // howManyBricksLeft = Tools.NumberOfBricks; // na razie ze ścieżką, zobaczymy jak będziemy wczytywać custom levele
            nrr = Tools.NumberOfBricks;
            pointsLabel.Content = "Zostało: " + nrr; //+ pointsLeft;
            pointsLabel.Content = "Zostało: " + nrr + " pkt: " + points; //+ pointsLeft;

            Brick.GenerateElements(ref myCanvas, ref bricks, width, height);//Przykładowa funkcja jak można przerzycić metody do innych klas
            myCanvas.Focus();

            //Pętla gry
            gameTimer.Interval = TimeSpan.FromMilliseconds(30);//TODO coś nie działa z tym czasem, gdy się ustawi na 0, to jest szybko, a od 1 do 20 prawie tak samo
            gameTimer.Tick += new EventHandler(GameTimerEvent);
            gameTimer.Start();

            numberOfChilds = myCanvas.Children.Count;
            numberOfBricksLeft = Tools.NumberOfBricks;
        }
        

        private void GameTimerEvent(object sender, EventArgs e)
        {
            int index = 0;
            for (int i = 0; i < 10; i++)
            {
                bool aaaa = false;
                foreach (var x in myCanvas.Children.OfType<Rectangle>())//kolizja piłek
                {
                    bool leave = false;
                    if (!aaaa && x.Name != "player")//jeżeli element jest blokiem to go usun
                    {
                        int posX = (int)Canvas.GetLeft(x) / (width / 13);//element [x,0] tablicy
                        int posY = (int)Canvas.GetTop(x) / (height / 26);//element [0,Y] tablicy
                        Rect BlockHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                        Rect ballEclipseHitBox;
                        index = 0;
                        foreach (var ball in myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "ballEclipse"))
                        {
                            leave = false;
                            ballEclipseHitBox = new Rect(Canvas.GetLeft(ball), Canvas.GetTop(ball), ball.Width, ball.Height);

                            if (!aaaa && ballEclipseHitBox.IntersectsWith(BlockHitBox))
                            {
                                //pointsLabel.Content = "Zostało: " + numberOfChilds; //+ pointsLeft;

                                //MessageBox.Show("górna kraw " + Canvas.GetLeft(x) + " < " + balls[index].posX + " i " + balls[index].posX + " < " + ((Canvas.GetLeft(x)) + x.Width) + " i " + balls[index].posY + " < " + (Canvas.GetTop(x) + x.Height));
                                //MessageBox.Show("dolna kraw " + Canvas.GetLeft(x) + " < " + balls[index].posX + " i " + balls[index].posX + " < " + ((Canvas.GetLeft(x)) + x.Width) + " i " + balls[index].posY + " > " + Canvas.GetTop(x));
                                //MessageBox.Show("lewa kraw " + Canvas.GetTop(x) + " < " + balls[index].posY + " i " + balls[index].posX + " < " + ((Canvas.GetLeft(x)) + x.Width) + " i " + balls[index].posY + " < " + (Canvas.GetTop(x) + x.Height));
                                //MessageBox.Show("prawa kraw " + Canvas.GetTop(x) + " < " + balls[index].posY + " i " + balls[index].posX + " > " + (Canvas.GetLeft(x)) + " i " + balls[index].posY + " < " + (Canvas.GetTop(x) + x.Height));

                                // górna krawędź klocka
                                if (Canvas.GetLeft(x) < balls[index].posX && balls[index].posX < Canvas.GetLeft(x) + x.Width && balls[index].posY < Canvas.GetTop(x) + x.Height)
                                {
                                    if (booster.GetPower() != Power.StrongerHit || bricks[posX, posY].GetType() == typeof(GoldBrick))
                                        balls[index].top = true;
                                    //MessageBox.Show("górna kraw");
                                    HitBlock(posX, posY, x, index);
                                    aaaa = true;
                                    leave = true;
                                }
                                else
                                // dolna krawędź klocka
                                if (Canvas.GetLeft(x) < balls[index].posX && balls[index].posX < Canvas.GetLeft(x) + x.Width && balls[index].posY > Canvas.GetTop(x))
                                {
                                    if (booster.GetPower() != Power.StrongerHit || bricks[posX, posY].GetType() == typeof(GoldBrick))
                                        balls[index].top = false;
                                    //MessageBox.Show("dolna kraw");
                                    HitBlock(posX, posY, x, index);
                                    aaaa = true;
                                    leave = true;
                                }
                                else
                // lewa krawędź klocka
                if (Canvas.GetTop(x) < balls[index].posY && balls[index].posX < Canvas.GetLeft(x) + x.Width && balls[index].posY < Canvas.GetTop(x) + x.Height)
                                {
                                    if (booster.GetPower() != Power.StrongerHit || bricks[posX, posY].GetType() == typeof(GoldBrick))
                                        balls[index].left = true;
                                    //MessageBox.Show("lewa kraw");
                                    HitBlock(posX, posY, x, index);
                                    aaaa = true;
                                    leave = true;
                                }
                                else
                // prawa krawędź klocka
                if (Canvas.GetTop(x) < balls[index].posY && balls[index].posX > Canvas.GetLeft(x) && balls[index].posY < Canvas.GetTop(x) + x.Height)
                                {
                                    if (booster.GetPower() != Power.StrongerHit || bricks[posX, posY].GetType() == typeof(GoldBrick))
                                        balls[index].left = false;
                                    //MessageBox.Show("prawa kraw");
                                    HitBlock(posX, posY, x, index);
                                    aaaa = true;
                                    leave = true;
                                }
                            }
                            index++;
                            if (leave || aaaa)//wyjście z foreacha, bo usuwamy jeden z jego elementów
                                break;
                        }
                        if (leave)
                            break;
                    }
                    if (x.Name == "player") //jeżeli element jest graczem to się od niego odbij
                    {
                        Rect BlockHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                        Rect ballEclipseHitBox;

                        index = 0;
                        foreach (var ball in myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "ballEclipse"))//sprawdzanie czy jakaś piłka nie dotkła paletki
                        {
                            ballEclipseHitBox = new Rect(Canvas.GetLeft(ball), Canvas.GetTop(ball), ball.Width, ball.Height);
                            if (ballEclipseHitBox.IntersectsWith(BlockHitBox))
                            {
                                balls[index].top = true;
                            }

                            index++;
                        }
                        //kolizja gracza z boostem
                        if (PlayerCaughtABoost(BlockHitBox))//kontakt paletki z boostem
                        {
                            break;
                        }
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
            }

        }

        private void BallMovement(int index, int goLeft = 0)
        {
            //tablica balls i jej odpowiednik na planszy mają ten sem index
            balls[index].posX = (int)Canvas.GetLeft(myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "ballEclipse").ElementAt(index));
            balls[index].posY = (int)Canvas.GetTop(myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "ballEclipse").ElementAt(index));

            // dodaj wykonywanie co ileś milisekund (chyba najlepiej 6-10)
            if (balls[index].stop != true)//przyklejamy piłkę do paletki, do momentu wciśnięcia spacji
            {
                if (balls[index].left)
                    balls[index].posX -= 1;
                else if (!balls[index].left)
                    balls[index].posX += 1;

                if (balls[index].top)
                    balls[index].posY -= 1;
                else if (!balls[index].top)
                    balls[index].posY += 1;
            }
            else
            {
                balls[index].posX += goLeft;
            }
            //testowyLabel.Content = "w: " + ball.posX + " h: " + ball.posY;

            //Canvas.SetTop(ballEclipse, Canvas.GetTop(ballEclipse) - 10);    // do góry
            //Canvas.SetTop(ballEclipse, Canvas.GetTop(ballEclipse) + 10);    // do dołu
            //Canvas.SetLeft(ballEclipse, Canvas.GetLeft(ballEclipse) - 10);  // w lewo
            //Canvas.SetLeft(ballEclipse, Canvas.GetLeft(ballEclipse) + 10);  // w prawo

            Canvas.SetLeft(myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "ballEclipse").ElementAt(index), balls[index].posX);
            Canvas.SetTop(myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "ballEclipse").ElementAt(index), balls[index].posY);
            ChangeBallDirection(index);
        }

        private void ChangeBallDirection(int index)
        {
            //testowyLabel.Content = "PosY zwykłe: " + (ball.posY-ball.rad)  + "PosY: " + ball.posY + " Rad: " + ball.rad; // nie usuwajcie tego ~ Wika

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

            if (balls[index].posY >= height)
            {// dół // jest na razie i gdy przegrana będzie zrobiona to do usunięcia
                balls[index].top = true;
            }
            // dodaj jakiś if który sprawdza od czego odbiła się kulka + dodaj wymiary kulki i paletki (ale do zmiennych) ~ Wika
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

                    for (int j = 0; j < balls.Count; j++)//ruch przyklejonych piłek do paletki
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
            //File.WriteAllText("WriteText.txt", Tools.info);
            //MessageBox.Show("ok");

            if (e.Key == Key.D)
                playerGoRight = true;

            if (e.Key == Key.A)
                playerGoLeft = true;

            if (e.Key == Key.Space)//wypuszczenie wszystkich piłek
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
        
        public void HitBlock(int posX, int posY, Rectangle rectangle, int indexOfBall)//akcja po trafieniu piłki w block
        {
            //MessageBox.Show("wywołanie funkcji HitBlock");
            /*
            if (dane[0] != indexOfBall || dane[1] != balls[indexOfBall].posX && dane[2] != balls[indexOfBall].posY)
            {// 0 indeks, 1 posX, 2 posY
                dane[0] = indexOfBall;
                dane[1] = balls[indexOfBall].posX;
                dane[2] = balls[indexOfBall].posY;
                ok = true;
            }
            else
                ok = false;*/
            if (bricks[posX, posY].GetType() != typeof(GoldBrick))//sprawdzanie czy obiekt nie jest GoldBrick (ten obiekt nie ma Value)
            {
                //MessageBox.Show("ttb: " + bricks[posX, posY].TimesToBreak);
                if (bricks[posX, posY].TimesToBreak < 2)
                {
                    pointsLeft -= bricks[posX, posY].Value;
                    myCanvas.Children.Remove(rectangle);
                    //howManyBricksLeft--;
                    RespawnBoost(indexOfBall);

                    //MessageBox.Show("Ile: " + myCanvas.Children.Count);
                    //MessageBox.Show("Dane kulki: "  + XX + " : " + YY + "");
                    //if (ok)
                    {
                        nrr--;
                        points += bricks[posX, posY].Value;
                        pointsLabel.Content = "Zostało: " + nrr +" pkt: " +points + " --"+ Tools.PointsAtLevel; //+ pointsLeft;
                    }
                    //MessageBox.Show("Pozycja: " +ok.ToString() + " -> " +  +"=="+ balls[indexOfBall].posX +", "+ YY +"=="+ balls[indexOfBall].posY);
                    /*
                    pointsLabel.Content = "Zostało: " + numberOfChilds; //+ pointsLeft;
                    //MessageBox.Show("zbicie: -" + bricks[posX, posY].Value);
                    if(levelek == 2 && howManyBricksLeft < 60)
                    //if(howManyBricksLeft < 2)
                    if (raz)
                    //    MessageBox.Show("Left: " + howManyBricksLeft);
                    raz = false;
                    MessageBox.Show("Left wsm: " + myCanvas.Children.Count);
                    MessageBox.Show("DANE: " + (numberOfChilds-1) + " : " + myCanvas.Children.Count);
                    if (numberOfChilds - 1 == myCanvas.Children.Count)
                    {
                            MessageBox.Show("Znika jeden klocek");
                        numberOfBricksLeft--;
                        numberOfChilds = myCanvas.Children.Count;
                    }
                    else
                        numberOfChilds = numberOfChilds+1;
                    MessageBox.Show("Powinny być równe: " + numberOfChilds + " : " + myCanvas.Children.Count);*/
                    //numberOfBricksLeft = Tools.NumberOfBricks;
                    if (points == Tools.PointsAtLevel)
                    {
                        Next_Level();
                    }

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
                    numberOfChilds--;
                    StopBoost();
                    booster.RandomPower();
                    //booster.SetPower(Power.StrongerHit); testowanie PowerUp'ów
                    SetBoost();
                    return true;
                }
            }
            return false;
        }

        public void RespawnBoost(int indexOfBall)//Po zniszczeniu bloku, jest 10% szans na to, że sprespi się nowy boost. Poprzedni wciąż jest aktywny
        {
                        //numberOfChilds = myCanvas.Children.Count;
            if (Tools.RundomNumber(1, 10) == 5)
            {
                numberOfChilds++;
                booster = new Booster(balls[indexOfBall], ref myCanvas, booster);
            }
        }

        public void Next_Level()
        {
            gameTimer.Stop();
            levelek++;
            
            MessageBox.Show("Lvl: " + levelek);
            NavigationService.Navigate(new GamePage(levelek, points));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            gameTimer.Stop();
            MessageBox.Show("ok"); // do usunięcia ten btn
            gameTimer.Start();
        }
    }
}
