using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArkanoEgo.Classes.Bricks
{
    public abstract class Brick
    {
        private string _color;
        private bool _canBreak;

        public Brick(string color, bool canBreak)
        {
            _color = color;
            _canBreak = canBreak;
        }
        public string Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }

        public bool CanBreak
        {
            get 
            { 
                return _canBreak; 
            }
            set
            {
                _canBreak = value;
            }
        }
        public abstract int Value { get; set; }
    }
}
