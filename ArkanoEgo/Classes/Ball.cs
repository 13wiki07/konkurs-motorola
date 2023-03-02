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
    public class Ball
    {
        public int rad { get; set; } // promień kulki (potrzebny do odbić)
        public int speed { get; set; }
        public double posX { get; set; } // - w góre, + w dół
        public double posY { get; set; } // - w lewo, + w prawo
        public double trajectoryX { get; set; } // - w lewo, + w prawo
        public double trajectoryY { get; set; } // - w lewo, + w prawo

        public bool top { get; set; } // top = true; bottom = false;
        public bool left { get; set; } // left = true; right = false;

        public bool stop { get; set; }
        public bool iAmShoot { get; set; }

        public void InitBall(Ellipse ballEclipse)
        {
            stop = true;

            rad = Convert.ToInt32(ballEclipse.Height) / 2; // promień kuli
            posX = Convert.ToInt32(Canvas.GetLeft(ballEclipse));
            posY = Convert.ToInt32(Canvas.GetTop(ballEclipse));

            ballEclipse.Fill = new SolidColorBrush(Color.FromRgb(139, 164, 223));
            trajectoryX = 1;
            trajectoryY = 1;

            top = true;
            left = true;
            iAmShoot = false;
        }
    }
}
