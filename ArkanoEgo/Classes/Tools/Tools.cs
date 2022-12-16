using ArkanoEgo.Classes.Bricks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArkanoEgo.Classes.Tools
{
    public static class Tools
    {
        public static Brick[,] ReadLvl(int lvl)//wstępne czytanie mapy z pliku
        {
            string[] lines = null;
            Brick[,] Bricks = new Brick[13, 20];

            try
            { 
                lines = File.ReadAllLines(System.IO.Path.ChangeExtension($@"LVLS\lvl{lvl}", ".csv"));
            }
            catch(Exception e)//TODO trzeba pomyśleć nad lepszym rozwiązaniem gdy nie ma pliku z mapą
            {
                Bricks[0, 0] = new GoldBrick();
                return Bricks;
            }
            foreach (var line in lines)
            {

                string[] data = line.Split(';');

                switch (Convert.ToInt32(data[0]))
                {
                    case 1:
                        Bricks[Convert.ToInt32(data[1]), Convert.ToInt32(data[2])] = new SimpleBrick(data[4], Convert.ToInt32(data[3]));
                        break;
                    case 2:
                        Bricks[Convert.ToInt32(data[1]), Convert.ToInt32(data[2])] = new SilverBrick(Convert.ToInt32(data[3]), Convert.ToInt32(data[5]));
                        break;
                    case 3:
                        Bricks[Convert.ToInt32(data[1]), Convert.ToInt32(data[2])] = new GoldBrick();
                        break;
                    default: break;
                }
            }
            return Bricks;
        }
}
}
