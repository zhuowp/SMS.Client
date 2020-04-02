using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace SMS.Client.Controls
{
    public class ImageProcessHelper
    {
        public static Bitmap ArgbBytesToBitmap(byte[] colorizedHeatValueArray, int imageWidth, int imageHeight)
        {
            if (colorizedHeatValueArray == null)
            {
                return null;
            }

            Bitmap bitmap = new Bitmap(imageWidth, imageHeight, PixelFormat.Format32bppArgb);

            //位图矩形
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            //以可读写的方式将图像数据锁定
            BitmapData bmpdata = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            //得到图形在内存中的首地址
            IntPtr ptr = bmpdata.Scan0;

            //构造一个位图数组进行数据存储
            int bytes = bitmap.Width * bitmap.Height * 4;

            //把处理后的图像数组复制回图像
            System.Runtime.InteropServices.Marshal.Copy(colorizedHeatValueArray, 0, ptr, bytes);

            //解锁位图像素
            bitmap.UnlockBits(bmpdata);
            return bitmap;
        }

        public static BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                return null;
            }

            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

        public static BitmapImage ArgbBytesToBitmapImage(byte[] colorizedHeatValueArray, int imageWidth, int imageHeight)
        {
            Bitmap bitmap = ArgbBytesToBitmap(colorizedHeatValueArray, imageWidth, imageHeight);
            BitmapImage bitmapImage = ConvertBitmapToBitmapImage(bitmap);

            return bitmapImage;
        }
    }
}
