using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Client.Common;

namespace Client.ClientProtocol
{
   public class RemoteDestopProtocol:ProtocolBase
    {
        ScreenShot screenShot = new ScreenShot();

        public override System.IO.Stream PacketData(string data)
        {
            MemoryStream memoryStream=null;
            if (data == "DestopImage")
            {
             
             // BitmapSource bitmapSource = ScreenShotEx.CreateBitmapSourceFromBitmap();
                BitmapSource bitmapSource = screenShot.TakeScreenshotCore();
                memoryStream =new MemoryStream();
              JpegBitmapEncoder encoder = new JpegBitmapEncoder();
              memoryStream = new MemoryStream();
              encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
              encoder.Save(memoryStream);
              
            }
            return new MemoryStream( CompressionImage(memoryStream,0L));
        }


        private byte[] CompressionImage(Stream fileStream, long quality)
        {
            using (System.Drawing.Image img = System.Drawing.Image.FromStream(fileStream))
            {
                using (Bitmap bitmap = new Bitmap(img))
                {
                    ImageCodecInfo CodecInfo = GetEncoder(img.RawFormat);
                    System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, quality);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bitmap.Save(ms, CodecInfo, myEncoderParameters);
                        myEncoderParameters.Dispose();
                        myEncoderParameter.Dispose();
                        return ms.ToArray();
                    }
                }
            }
        }
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

       public void SendRemoteDestop()
       {
            Thread sendDestpThr = new Thread(new ThreadStart(SendDestopImage));
            sendDestpThr.Start();
          
       }

       private void SendDestopImage()
       {

           while (true)
           {
               Stream s = PacketData("DestopImage");
            
               this.SplitSendData(App.client, s, 1024 * 1024, 700);
               
               
               Thread.Sleep(300);
           }

       }

       internal void KeyDown(string keyDownMsg)
       {
           Console.WriteLine(keyDownMsg);
           MouseKeyOperate mko = new MouseKeyOperate();
           mko.keybd(mko.getKeys(keyDownMsg));
          
       }

       internal void KeyUp(string keyUpMsg)
       {
           Console.WriteLine(keyUpMsg);
           MouseKeyOperate mko = new MouseKeyOperate();
           mko.keybd(mko.getKeys(keyUpMsg));
       }

       internal void MouseLeftDown(string MouseLeftDownMsg)
       {
           string[] pos = MouseLeftDownMsg.Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
           int x = (int)Convert.ToDouble(pos[0]);
           int y = (int)Convert.ToDouble(pos[1]);
           MouseKeyOperate mko=new MouseKeyOperate();
           mko.mouse_move(x,y);
           mko.MouseLeftDown(x,y);
       }

       internal void MouseLeftUp(string MouseLeftUpMsg)
       {
           string[] pos = MouseLeftUpMsg.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
           int x = (int)Convert.ToDouble(pos[0]);
           int y = (int)Convert.ToDouble(pos[1]);
           MouseKeyOperate mko = new MouseKeyOperate();
           mko.mouse_move(x, y);
           mko.MouseLeftUp(x, y);
       }

       internal void MouseRightDown(string MouseRightDownMsg)
       {
           string[] pos = MouseRightDownMsg.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
           int x = (int)Convert.ToDouble(pos[0]);
           int y = (int)Convert.ToDouble(pos[1]);
           MouseKeyOperate mko = new MouseKeyOperate();
           mko.mouse_move(x, y);
           mko.MouseRightDown(x, y);
       }

       internal void MouseRightUp(string MouseRightUpnMsg)
       {
           string[] pos = MouseRightUpnMsg.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
           int x = (int)Convert.ToDouble(pos[0]);
           int y = (int)Convert.ToDouble(pos[1]);
           MouseKeyOperate mko = new MouseKeyOperate();
           mko.mouse_move(x, y);
           mko.MouseRightUp(x, y);
       }
    }
}
