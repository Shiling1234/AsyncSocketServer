
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Client.Common
{
   static class ScreenShotEx
    {
        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);
        public static BitmapSource CreateBitmapSourceFromBitmap()
        {
            var bitmap = new Bitmap((int)SystemParameters.PrimaryScreenWidth,
              (int)SystemParameters.PrimaryScreenHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

               IntPtr ip = bitmap.GetHbitmap();
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            if (System.Windows.Application.Current.Dispatcher == null)
                return null; // Is it possible?
            using (var bmpGraphics = Graphics.FromImage(bitmap))
            {

                bmpGraphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
       
            }


            try
                {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // You need to specify the image format to fill the stream. 
                    // I'm assuming it is PNG
                    bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    // Make sure to create the bitmap in the UI thread
                    if (InvokeRequired)
                        return (BitmapSource)Application.Current.Dispatcher.Invoke(
                            new Func<Stream, BitmapSource>(CreateBitmapSourceFromBitmap),
                            DispatcherPriority.Normal,
                            memoryStream);
                    DeleteObject(ip);
                    return CreateBitmapSourceFromBitmap(memoryStream);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static bool InvokeRequired
        {
            get { return Dispatcher.CurrentDispatcher != Application.Current.Dispatcher; }
        }

        private static BitmapSource CreateBitmapSourceFromBitmap(Stream stream)
        {
            BitmapDecoder bitmapDecoder = BitmapDecoder.Create(
                stream,
                BitmapCreateOptions.PreservePixelFormat,
                BitmapCacheOption.OnLoad);

            // This will disconnect the stream from the image completely...
            WriteableBitmap writable = new WriteableBitmap(bitmapDecoder.Frames.Single());
            writable.Freeze();

            return writable;
        }
    }
}
