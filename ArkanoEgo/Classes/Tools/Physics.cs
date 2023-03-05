using ArkanoEgo.Classes.Bricks;
using ArkanoEgo.Classes.Struct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace ArkanoEgo.Classes.Tools
{
    
    public class Physics
    {
        private int tickRateValue = new int();
        private List<CartesianPosition> baseTopRight = new List<CartesianPosition>();
        private List<CartesianPosition> baseTopLeft = new List<CartesianPosition>();
        private List<CartesianPosition> baseBottomRight = new List<CartesianPosition>();
        private List<CartesianPosition> baseBottomLeft = new List<CartesianPosition>();
        public int NumberOfMoves; /// Base position is tickRateValue;
        public Physics(int tickRate)
        {
            tickRateValue = tickRate;
            NumberOfMoves = tickRateValue * 2 - 1;
            InitializeValues();
            NumberOfMoves = tickRateValue * 2 - 1;

        }
        private void InitializeValues()
        {
            GenerateBaseTopRightValue();
            GenerateBaseTopLeftValue();
            GenerateBaseBottomRightValue();
            GenerateBaseBottomLeftValue();
        }
        private void GenerateBaseTopRightValue()
        {
            //Sharp angle
            for(int i = 1; i <= tickRateValue; i++)
            {
                baseTopRight.Add(new CartesianPosition(tickRateValue, -i));
            }
            //We have to do it in 2 diffrent loops to keep the order of positons
            //Obtuse angle
            for (int i = 1; i <= tickRateValue; i++)
            {
                baseTopRight.Add(new CartesianPosition(i, -tickRateValue));
            }
            //There is dupilcate on tickRateValue-1 and tickRateValue index
            //So, we have to delete one
            baseTopRight.RemoveAt(NumberOfMoves);
            //its doesnt metter which one
        }
        private void GenerateBaseTopLeftValue()
        {
            //We just have to verticaly reflect baseTopRight values
            for (int i=0;i<baseTopRight.Count;i++)
            {
                baseTopLeft.Add(new CartesianPosition(-baseTopRight[i].HorizontalPosition, baseTopRight[i].VerticalPosition));
            }
        }
        private void GenerateBaseBottomRightValue()
        {
            //We just have to horizontaly reflect baseTopRight values
            for (int i = 0; i < baseTopRight.Count; i++)
            {
                baseBottomRight.Add(new CartesianPosition(baseTopRight[i].HorizontalPosition, -baseTopRight[i].VerticalPosition));
            }
        }
        private void GenerateBaseBottomLeftValue()
        {
            //We just have to verticaly reflect baseBottomRight values
            for (int i = 0; i < baseTopRight.Count; i++)
            {
                baseBottomLeft.Add(new CartesianPosition(-baseBottomRight[i].HorizontalPosition, baseBottomRight[i].VerticalPosition));
            }
        }
        public CartesianPosition ExtractValue(bool HorizontalPosition, bool VerticalPosition, int position)
        {
            float X = new float();
            float Y = new float();
            if (HorizontalPosition)
            {
                if (VerticalPosition)
                {
                    X = baseBottomLeft[position].HorizontalPosition;
                    Y = baseBottomLeft[position].VerticalPosition;
                }
                else
                {
                    X = baseBottomRight[position].HorizontalPosition;
                    Y = baseBottomRight[position].VerticalPosition;
                }
            }
            else
            {
                if (VerticalPosition)
                {
                    X = baseTopLeft[position].HorizontalPosition;
                    Y = baseTopLeft[position].VerticalPosition;
                }
                else
                {
                    X = baseTopRight[position].HorizontalPosition;
                    Y = baseTopRight[position].VerticalPosition;
                }
            }
            X /= tickRateValue;
            Y /= tickRateValue;
            return new CartesianPosition(X,Y);
        }
        public CartesianPosition ExtractValue(bool HorizontalPosition, bool VerticalPosition)
        {
            float X = new float();
            float Y = new float();
            if (HorizontalPosition)
            {
                if (VerticalPosition)
                {
                    X = baseTopLeft[tickRateValue - 1].HorizontalPosition;
                    Y = baseTopLeft[tickRateValue - 1].VerticalPosition;
                }
                else
                {
                    X = baseBottomLeft[tickRateValue - 1].HorizontalPosition;
                    Y = baseBottomLeft[tickRateValue - 1].VerticalPosition;
                }
            }
            else
            {
                if (VerticalPosition)
                {
                    X = baseTopRight[tickRateValue - 1].HorizontalPosition;
                    Y = baseTopRight[tickRateValue - 1].VerticalPosition;
                }
                else
                {
                    X = baseBottomRight[tickRateValue - 1].HorizontalPosition;
                    Y = baseBottomRight[tickRateValue - 1].VerticalPosition;
                }
            }
            X /= tickRateValue;
            Y /= tickRateValue;
            return new CartesianPosition(X, Y);
        }
    }
}
