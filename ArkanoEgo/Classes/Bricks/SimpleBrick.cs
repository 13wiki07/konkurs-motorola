using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArkanoEgo.Classes.Bricks
{
    public class SimpleBrick : Brick
    {
        private int _value;
        private int _timesToBreak;
        public SimpleBrick(string color, int value) : base(color, true)
        {
            _value = value;
            _timesToBreak = 1;
        }
        public override int Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }
        public int TimesToBreak
        {
            get
            {
                return _timesToBreak;
            }
            set
            {
                _timesToBreak = value;
            }
        }
    }
}
