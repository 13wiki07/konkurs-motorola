using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
