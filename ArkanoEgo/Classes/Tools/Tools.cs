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
        [XmlRoot("XMLBricks")]
        public class ListBricks
        {
            public ListBricks() { Bricks = new List<XMLBrick>(); }

            [XmlElement("XMLBrick")]
            public List<XMLBrick> Bricks { get; set; }
        }

        public static ListBricks listBricks = new ListBricks();
        public static Brick[,] ReadLvl(int lvl)//wstępne czytanie mapy z pliku
        {
            Brick[,] Bricks = new Brick[13, 20];

            var serializer = new XmlSerializer(typeof(ListBricks));
            using (var reader = XmlReader.Create($@"LVLS\lvl{lvl}.xml"))
            {
                listBricks = (ListBricks)serializer.Deserialize(reader);
            }
            foreach (var Brick in listBricks.Bricks)
            {
                switch (Brick.Type)
                {
                    case 1:
                        Bricks[Brick.PosX, Brick.PosY] = new SimpleBrick(Brick.Color, Brick.Value);
                        break;
                    case 2:
                        Bricks[Brick.PosX, Brick.PosY] = new SilverBrick(Brick.Value, Brick.TimesToBreak);
                        break;
                    case 3:
                        Bricks[Brick.PosX, Brick.PosY] = new GoldBrick();
                        break;
                    default: break;
                }
            }
            return Bricks;
        }
}
}
