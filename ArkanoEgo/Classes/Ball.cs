using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace ArkanoEgo.Classes
{
    public class Ball
    {
        public int rad { get; set; } // promień kulki (potrzebny do odbić)
        public int speed { get; set; }
        public int posX { get; set; } // - w góre, + w dół
        public int posY { get; set; } // - w lewo, + w prawo

        public bool top { get; set; } // top = true; bottom = false;
        public bool left { get; set; } // left = true; right = false;


        // kulka przypisanie
        // w linku jest rozpiska co czym jest
        // https://www.canva.com/design/DAFSS32ggNg/ZHP5O-GhpveqJ4X_GtHzrg/view?utm_content=DAFSS32ggNg&utm_campaign=designshare&utm_medium=link2&utm_source=sharebutton#5
        public void InitBall(ref Ellipse ballEclipse)
        {
            rad = Convert.ToInt32(ballEclipse.Height) / 2; // promień kuli
            posX = Convert.ToInt32(Canvas.GetLeft(ballEclipse));
            posY = Convert.ToInt32(Canvas.GetTop(ballEclipse));

            top = true; // potrzebne do testu z onclickiem i Q
            left = true; // potrzebne do testu z onclickiem i Q
        }
    }
}
