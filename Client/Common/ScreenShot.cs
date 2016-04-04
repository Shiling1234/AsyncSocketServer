using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;

namespace Client.Common
{

    public class ScreenShot
    {
        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);
        //public static BitmapSource CopyScreen()
        //{
        //    BitmapSource source = null;
        //    var screenBmp = new Bitmap((int)SystemParameters.PrimaryScreenWidth,
        //        (int)SystemParameters.PrimaryScreenHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        //    IntPtr ip = screenBmp.GetHbitmap();
        //    try
        //    {
        //        using (var bmpGraphics = Graphics.FromImage(screenBmp))
        //        {


        //            bmpGraphics.CopyFromScreen(0, 0, 0, 0, screenBmp.Size);
        //            source = Imaging.CreateBitmapSourceFromHBitmap(
        //                 ip,
        //                 IntPtr.Zero,
        //                 Int32Rect.Empty,
        //                 BitmapSizeOptions.FromEmptyOptions());
        //            DeleteObject(ip);
        //            return source;


        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //        return source;
        //    }
        //    finally
        //    {
        //        //screenBmp.Dispose();
        //        //screenBmp = null;



        //    }


        //}


        public static BitmapSource GetScreenSnapshot()

        {



            var bitmap = new Bitmap((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            IntPtr handle = bitmap.GetHicon();

            using (Graphics g = Graphics.FromImage(bitmap))

            {

                g.CopyFromScreen(0, 0, 0, 0, bitmap.Size, CopyPixelOperation.SourceCopy);

            }


            DeleteObject(handle);
            return ToBitmapSource(bitmap);

        }



        public static BitmapSource ToBitmapSource(Bitmap bmp)

        {

            BitmapSource returnSource;



            try

            {

                returnSource = Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            }

            catch

            {

                returnSource = null;

            }


            bmp.Dispose();
            return returnSource;



        }
        WriteableBitmap r_Screenshot = new WriteableBitmap((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, 96.0, 96.0, System.Windows.Media.PixelFormats.Bgr24, null);

        public BitmapSource TakeScreenshotCore()
        {
            var rWidth = (int)SystemParameters.PrimaryScreenWidth;
            var rHeight = (int)SystemParameters.PrimaryScreenHeight;
            var rImageBytes = rWidth * rHeight * 3;

            using (var rBitmap = new Bitmap(rWidth, rHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
            {
                using (var rGraphics = Graphics.FromImage(rBitmap))
                    rGraphics.CopyFromScreen(0, 0, 0, 0, rBitmap.Size);

                var rBitmapData = rBitmap.LockBits(new Rectangle(0, 0, rWidth, rHeight), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                return (BitmapSource)Application.Current.Dispatcher.Invoke(
                         new Func<WriteableBitmap, BitmapSource>(w =>
                         {
                           
                             w.Lock();

                             unsafe
                             {
                                 Buffer.MemoryCopy(rBitmapData.Scan0.ToPointer(), r_Screenshot.BackBuffer.ToPointer(), rImageBytes, rImageBytes);
                             }
                             w.AddDirtyRect(new Int32Rect(0, 0, rWidth, rHeight));
                             w.Unlock();

                             rBitmap.UnlockBits(rBitmapData);

                             return w;
                         }),
                         DispatcherPriority.Normal,
                         r_Screenshot);


            }
        }

    }
}