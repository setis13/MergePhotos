using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace MergePhotos
{

    public class ImageDetails : INotifyPropertyChanged
    {
        /// <summary>
        /// A name for the image, not the file name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A description for the image.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Full path such as c:\path\to\image.png
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The image file name such as image.png
        /// </summary>
        public string FileName { get; set; }

        public string FileRename { get; set; }

        /// <summary>
        /// The file name extension: bmp, gif, jpg, png, tiff, etc...
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// The image height
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The image width.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The file size of the image.
        /// </summary>
        public long Size { get; set; }

        public DateTime DateTimeShot { get; set; }

        public BitmapImage BitmapImage { get; set; }

        public DateTime NewDateTime
        {
            get { return new DateTime(DateTimeShot.Ticks + DeltaDatetime[0]); }
        }

        public string Camera { get; set; }

        public byte SourceIndex { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public long[] DeltaDatetime { get; set; }
        public int Index { get; set; } 
    }
}
