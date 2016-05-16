using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FluxJpeg;
namespace ScreenTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }
        WriteableBitmap r_Screenshot = new WriteableBitmap((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, 96.0, 96.0, PixelFormats.Rgb24, null);
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
           
            Thread t = new Thread(new ThreadStart(() =>
            {

               
   
                while (true)
                {
                   
                    Thread.Sleep(300);
                    
                    App.Current.Dispatcher.Invoke((()=> {
                        TakeScreenshotCore(ref r_Screenshot);



                        //var width = r_Screenshot.PixelWidth;
                        //var height = r_Screenshot.PixelHeight;
                        //var stride = width * ((r_Screenshot.Format.BitsPerPixel + 7) / 8);

                        //var bitmapData = new byte[3148798];

                        //r_Screenshot.CopyPixels(bitmapData, r_Screenshot.BackBufferStride, 0);
                      byte[] bytes=  SaveToFile(r_Screenshot);
                        //    File.WriteAllBytes("123", bitmapData);
                        //          BitmapDecoder bitmapDecoder = BitmapDecoder.Create(
                        //new MemoryStream(bitmapData),
                        //BitmapCreateOptions.PreservePixelFormat,
                        //BitmapCacheOption.OnLoad);

                        // This will disconnect the stream from the image completely...

                        // WriteableBitmap         writable = new WriteableBitmap(bitmapDecoder.Frames.Single());
                        //        writable.Freeze();

                        //JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                        //MemoryStream memoryStream = new MemoryStream();
                        //encoder.Frames.Add(BitmapFrame.Create(r_Screenshot));
                        //encoder.Save(memoryStream);
                        //  JpegBitmapDecoder decoder = new JpegBitmapDecoder(new MemoryStream(bitmapData), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                        //   decoder.DownloadCompleted += Decoder_DownloadCompleted;
                        //       BitmapDecoder bitmapDecoder = BitmapDecoder.Create(
                        //memoryStream,
                        //BitmapCreateOptions.PreservePixelFormat,
                        //BitmapCacheOption.OnLoad);

                        // This will disconnect the stream from the image completely...

                        //   WriteableBitmap   writable = new WriteableBitmap(decoder.Frames.Single());

                        var bitmap = new BitmapImage();

                        bitmap.BeginInit();
                        bitmap.StreamSource = new MemoryStream(bytes);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        bitmap.Freeze();

                        destopImage.Source = bitmap;
                       


                    }));
                }
            }));
          
            t.Start();



        }

        public static WriteableBitmap ByteToImage(byte[] imageBytes)
        {
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                BitmapImage im = new BitmapImage();
                im.BeginInit();
                im.StreamSource=new MemoryStream(imageBytes);
                im.EndInit();
                return new WriteableBitmap(im);
            }
        }



        private static byte[] SaveToFile(WriteableBitmap bitmap)
        {
           
            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;
            int bands = 3;
            byte[][,] raster = new byte[bands][,];

            for (int i = 0; i < bands; i++)
            {
                raster[i] = new byte[width, height];
            }

            //var width1 = bitmap.PixelWidth;
            //var height1 = bitmap.PixelHeight;
            

            var bitmapData = new int[width*height];
         
             bitmap.CopyPixels(bitmapData, (int)bitmap.Width*3, 0);

            for (int row = 0; row < height; row++)
            {
                for (int column = 0; column < width; column++)
                {
                    
                    int pixel = bitmapData[width * row + column];
                    raster[0][column, row] = (byte)(pixel >> 16);
                    raster[1][column, row] = (byte)(pixel >> 8);
                    raster[2][column, row] = (byte)pixel;
                }

            }
            FluxJpeg.Core.ColorModel model = new FluxJpeg.Core.ColorModel { colorspace = FluxJpeg.Core.ColorSpace.RGB };
            FluxJpeg.Core.Image img = new FluxJpeg.Core.Image(model, raster);

            //Encode the Image as a JPEG  
            MemoryStream stream = new MemoryStream();
            FluxJpeg.Core.Encoder.JpegEncoder encoder = new FluxJpeg.Core.Encoder.JpegEncoder(img, 100, stream);
            encoder.Encode();

            //Back to the start  
            stream.Seek(0, SeekOrigin.Begin);

            //Get teh Bytes and write them to the stream  
            byte[] binaryData = new Byte[stream.Length];
            stream.Read(binaryData, 0, binaryData.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);
            return binaryData;
        }

       

        public byte[] ImageSourceToBytes(BitmapEncoder encoder, ImageSource imageSource)
        {
            byte[] bytes = null;
            var bitmapSource = imageSource as BitmapSource;

            if (bitmapSource != null)
            {
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                using (var stream = new MemoryStream())
                {
                    encoder.Save(stream);
                    bytes = stream.ToArray();
                }
            }

            return bytes;
        }


        void TakeScreenshotCore( ref WriteableBitmap r_Screenshot)
        {
           
            var rWidth = (int)SystemParameters.PrimaryScreenWidth;
            var rHeight = (int)SystemParameters.PrimaryScreenHeight;
            var rPixelBytes = rWidth * rHeight * 3;

            using (var rBitmap = new Bitmap(rWidth, rHeight,System.Drawing.Imaging.PixelFormat.Format24bppRgb))
            {
                 
                using (var rGraphics = Graphics.FromImage(rBitmap))
                    rGraphics.CopyFromScreen(0, 0, 0, 0, rBitmap.Size);

                var rBitmapData = rBitmap.LockBits(new System.Drawing.Rectangle(0, 0, rWidth, rHeight), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                r_Screenshot.Lock();
                unsafe
                {
                    Buffer.MemoryCopy(rBitmapData.Scan0.ToPointer(), r_Screenshot.BackBuffer.ToPointer(), rPixelBytes, rPixelBytes);
                }
                r_Screenshot.AddDirtyRect(new Int32Rect(0, 0, rWidth, rHeight));
                r_Screenshot.Unlock();
               
                rBitmap.UnlockBits(rBitmapData);
              
               
            }
        }

      

    }
}
