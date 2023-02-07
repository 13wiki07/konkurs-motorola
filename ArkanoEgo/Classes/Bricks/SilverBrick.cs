using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArkanoEgo.Classes.Bricks
{
    public class SilverBrick : SimpleBrick
    {
        public SilverBrick(int value, int timesToBreak) : base("#626161", value)
        {
            TimesToBreak = timesToBreak;
        }
    }
}
