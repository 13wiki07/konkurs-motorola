using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        // power boostera
        public Power power = Power.None;

        private bool PowerActive = false;

        private int PowerSecDuration = 10;


        public Booster(Ball ball)
        {
            posX = ball.posX;
            posY = ball.posY;
        }

        public void RandomPower()
        {
            switch (Tools.Tools.RundomNumber(1, 3))
            {
                case 1:
                    power = Power.PlayerLenght;
                    break;
                case 2:
                    power = Power.NewBall;
                    break;
                case 3:
                    power = Power.StrongerHit;
                    break;
                default:
                    power = Power.None;
                    break;
            }
        }

        public Rectangle SetBoost(Rectangle rectangle)
        {
            rectangle.Width = rectangle.Width * 2;
            return rectangle;
        }

        public Rectangle StopBoost(Rectangle rectangle)
        {
            rectangle.Width = rectangle.Width / 2;
            return rectangle;
        }
        public float durationTime { get; set; }
        // ew można kilka klas/typów boosterów zrobić
    }
}
