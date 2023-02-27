using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ArkanoEgo.Classes
{
    class GalleryElement
    {
        public string _nr { get; set; }
        public string _name { get; set; }
        public string _image { get; set; }

        public GalleryElement() { }
        public GalleryElement(int nr, string name, string path)
        {
            this._nr = nr.ToString();
            this._name = name;
            this._image = path;
        }

    }
}
