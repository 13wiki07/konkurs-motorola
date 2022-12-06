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
        public int posX { get; set; }
        public int posY { get; set; }

        public bool top_bot { get; set; } // top = true; bot = false;
        public bool left_right { get; set; } // right = true; left = false;
    }
}
