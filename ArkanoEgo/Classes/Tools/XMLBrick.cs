using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ArkanoEgo.Classes.Tools
{
    public class XMLBrick
    {
        [XmlElement("Type")]
        public int Type { get; set; }

        [XmlElement("PosX")]
        public int PosX { get; set; }

        [XmlElement("PosY")]
        public int PosY { get; set; }

        [XmlElement("Value")]
        public int Value { get; set; }

        [XmlElement("Color")]
        public String Color { get; set; }

        [XmlElement("TimeToBreak")]
        public int TimeToBreak { get; set; }
    }
}
