using ArkanoEgo.Classes.Bricks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace ArkanoEgo.Classes.Tools
{
    public static class Tools
    {
        private static Random rnd = new Random();
        public static string info;
        public static int PointsAtLevel = 0;
        public static int NumberOfBricks = 0;
        [XmlRoot("XMLBricks")]
        public class ListBricks
        {
            public ListBricks() { Bricks = new List<XMLBrick>(); }

            [XmlElement("XMLBrick")]
            public List<XMLBrick> Bricks { get; set; }
        }

        public static ListBricks listBricks = new ListBricks();
        public static Brick[,] ReadLvl(int lvl)
        {
            return JustReadLvl("LVLS", "lvl_" + lvl.ToString());
        }

        public static Brick[,] ReadLvl(string path)
        {
            return JustReadLvl(@"..\..\CustomLVLS", path);
        }
        private static Brick[,] JustReadLvl(string path, string lvl)//wstępne czytanie mapy z pliku
        {
            Brick[,] Bricks = new Brick[13, 21];
            int bricksCount = 0;
            int levelPoints = 0;
            info = "";
            var serializer = new XmlSerializer(typeof(ListBricks));
            using (var reader = XmlReader.Create($@"{path}\{lvl}.xml"))
            {
                listBricks = (ListBricks)serializer.Deserialize(reader);
            }
            foreach (var Brick in listBricks.Bricks)
            {
                levelPoints += Brick.Value;
               info += "Brick: x" + Brick.PosX + ", y" + Brick.PosY + " ttb:> " + Brick.TimesToBreak + "\n";
                switch (Brick.Type)
                {
                    case 1:
                        Bricks[Brick.PosX, Brick.PosY] = new SimpleBrick(Brick.Color, Brick.Value);
                        bricksCount++;
                        break;
                    case 2:
                        Bricks[Brick.PosX, Brick.PosY] = new SilverBrick(Brick.Value, Brick.TimesToBreak);
                        bricksCount++;
                        break;
                    case 3:
                        Bricks[Brick.PosX, Brick.PosY] = new GoldBrick();
                        break;
                    default: break;
                }
            }
            

            NumberOfBricks = bricksCount;
            PointsAtLevel = levelPoints;
            return Bricks;
        }

        public static int RundomNumber(int from, int to)
        {
            return rnd.Next(from, to + 1);
        }
}
}
