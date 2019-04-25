using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace Trivago.Front_End
{
    public class CustomImage
    {
        //
        public static CustomImage NoImage = new CustomImage(@"resources\images\no-photo-available.png");

        private byte[] ByteImage; //contains the byte array of the image

        /// <summary>
        /// Initialize the ByteImage Using the given path to read from file
        /// </summary>
        public CustomImage(string path)
        {
            ByteImage = File.ReadAllBytes(path);
        }

        /// <summary>
        /// initialize the ByteImage using the give byte array
        /// </summary>
        public CustomImage(byte[] obj)
        {
            ByteImage = (byte[])obj.Clone();
        }

        /// <summary>
        /// sets the byte image using the file to read the byte array of the given path
        /// </summary>
        public void SetImage(string path)
        {
            ByteImage = File.ReadAllBytes(path);
        }

        /// <summary>
        /// Sets the byteimage using another byte array
        /// </summary>
        public void SetImage(byte[] obj)
        {
            ByteImage = (byte[])obj.Clone();
        }

        /// <summary>
        /// return the byteimage array
        /// </summary>
        public Byte[] GetByteImage()
        {
            Byte[] Ret = (Byte[])ByteImage.Clone();
            return Ret;
        }

        /// <summary>
        /// converts the byteimage array into an image object
        /// </summary>
        public Image GetImage()
        {
            if (ByteImage == null)
                return null;
            try
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.CreateOptions = BitmapCreateOptions.None;
                bi.CacheOption = BitmapCacheOption.Default;
                bi.StreamSource = new MemoryStream(ByteImage);
                bi.EndInit();
                Image img = new Image
                {
                    Source = bi
                };

                return img;
            }
            catch
            {
                return null;
            }
        }

    }
}
