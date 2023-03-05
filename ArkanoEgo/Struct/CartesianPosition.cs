using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArkanoEgo.Classes.Struct
{
    public struct CartesianPosition
    {
        public CartesianPosition(float X, float Y)
        {
            HorizontalPosition = X;
            VerticalPosition = Y;
        }

        public CartesianPosition(CartesianPosition element)
        {
            HorizontalPosition = element.HorizontalPosition;
            VerticalPosition = element.VerticalPosition;
        }


        public float HorizontalPosition { get; private set; }
        public float VerticalPosition { get; private set; }
    }
}
