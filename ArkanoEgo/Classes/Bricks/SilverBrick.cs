using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArkanoEgo.Classes.Bricks
{
    public class SilverBrick : SimpleBrick
    {
        public SilverBrick(int value, int timesToBreak) : base("#C0C0C0", value)
        {
            TimesToBreak = timesToBreak;
        }
    }
}
