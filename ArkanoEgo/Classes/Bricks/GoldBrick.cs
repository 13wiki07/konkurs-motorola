using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArkanoEgo.Classes.Bricks
{
    public class GoldBrick : Brick
    {
        public GoldBrick() : base("#FFD700", false) { }

        public override int Value { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
