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

            //BitmapImage src = new BitmapImage();
            //src.BeginInit();
            //src.UriSource = new Uri(sour, UriKind.Relative);
            //src.CacheOption = BitmapCacheOption.OnLoad;
            //src.EndInit();

            //Image nImg = new Image();
            //nImg.Source = src;
            //this._bmpImage = nImg;

            this._image = path;
        }

    }
}
