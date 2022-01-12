using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DobbleCreator
{
    public class ImageInfo
    {
        public BitmapImage Source {get;set;}
        public int Rotation {get;set;}
        public double Scale {get;set;}

        public ImageInfo(BitmapImage source, int rotation, double scale) {
            Source = source;
            Rotation = rotation;
            Scale = scale;
        }
    }
}
