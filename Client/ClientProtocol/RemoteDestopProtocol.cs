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

        public override byte[] GenerateMsg(String data)
        {
            MemoryStream ms = ScreenShotEx.CreateBitmapSourceFromBitmap();
            App.log.InfoFormat("压缩前大小：{0}", ms.Length);
            return CompressionImage(ms,20L);
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
                        // fileStream.Close();
                        //fileStream.Dispose();
                        App.log.InfoFormat("压缩后大小：{0}", ms.Length);
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
        Thread sendDestpThr;
       public void SendRemoteDestop()
       {
            sendImage = true;
            if (sendDestpThr == null)
            {
                sendDestpThr = new Thread(new ThreadStart(SendDestopImage));
                sendDestpThr.Start();
            }
          
          
       }
     static   bool sendImage = true;
       private void SendDestopImage()
       {

           while (sendImage)
           {
               byte[] bytes = GenerateMsg("DestopImage");           
               this.SplitSendData(App.client, bytes, 1024 * 1024, 700);    
               Thread.Sleep(300);
           }

       }
        MouseKeyOperate mko = new MouseKeyOperate();
        internal void KeyDown(string keyDownMsg)
       {
           Console.WriteLine(keyDownMsg);
        
         //  mko.keybd(mko.getKeys(keyDownMsg));
          
       }

       internal void KeyUp(string keyUpMsg)
       {
           Console.WriteLine(keyUpMsg);
       
         //  mko.keybd(mko.getKeys(keyUpMsg));
       }

       internal void MouseLeftDown(string MouseLeftDownMsg)
       {
           string[] pos = MouseLeftDownMsg.Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
           int x = (int)Convert.ToDouble(pos[0]);
           int y = (int)Convert.ToDouble(pos[1]);
            Console.WriteLine("MouseLeftDown");
            mko.mouse_move(x, y);
            mko.MouseLeftDown(x, y);
        }

       internal void MouseLeftUp(string MouseLeftUpMsg)
       {
           string[] pos = MouseLeftUpMsg.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
           int x = (int)Convert.ToDouble(pos[0]);
           int y = (int)Convert.ToDouble(pos[1]);
            Console.WriteLine("MouseLeftUp");
            mko.mouse_move(x, y);
            mko.MouseLeftUp(x, y);
        }

       internal void MouseRightDown(string MouseRightDownMsg)
       {
           string[] pos = MouseRightDownMsg.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
           int x = (int)Convert.ToDouble(pos[0]);
           int y = (int)Convert.ToDouble(pos[1]);
            Console.WriteLine("MouseRightDown(");
           //mko.mouse_move(x, y);
           //mko.MouseRightDown(x, y);
       }

       internal void MouseRightUp(string MouseRightUpnMsg)
       {
           string[] pos = MouseRightUpnMsg.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
           int x = (int)Convert.ToDouble(pos[0]);
           int y = (int)Convert.ToDouble(pos[1]);
            Console.WriteLine("MouseRightUp");
           //mko.mouse_move(x, y);
           //mko.MouseRightUp(x, y);
       }

        internal void CloseRemoteDestop()
        {
            sendImage = false;
            if (sendDestpThr != null)
            {
                sendDestpThr.Abort();
                sendDestpThr.Join();
                sendDestpThr = null;
            }
        }
    }
}
