using App_ui.DllImport;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace App_ui.Common
{
    public class ImageEx
    {
        /// <summary>
        /// Image to byte array.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static byte[] ImageToByte(BitmapImage bitmap)
        {
            byte[] data;
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }

        /// <summary>
        /// Byte to bitmapImage.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static BitmapImage ByteToBitmapImage(byte[] param)
        {
            if (param.Length == 0) { throw new Exception("Falha ao carregar a imagem: Imagem inexistente!"); };

            MemoryStream byteStream = new MemoryStream(param);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = byteStream;
            image.EndInit();
            return image;
        }

        /// <summary>
        /// 效率快，但是有BUG！
        /// </summary>
        /// <param name="bm"></param>
        /// <param name="b"></param>
        public static void Pixels2Bitmap(Bitmap bm, byte[] b)
        {
            //direct bit manipulation
            Rectangle rect = new Rectangle(0, 0, bm.Width, bm.Height);

            //lock the bits
            System.Drawing.Imaging.BitmapData bmpData =
                     bm.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly,
                     bm.PixelFormat);
            IntPtr ptr = bmpData.Scan0;

            int bytes = b.Length;

            System.Runtime.InteropServices.Marshal.Copy(b, 0, ptr, bytes);
            // Unlock the bits.
            bm.UnlockBits(bmpData);
            bmpData = null;
        }

        public static void Bitmap2Pixels(System.Drawing.Bitmap bm, byte[] b)
        {
            //direct bit manipulation
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bm.Width, bm.Height);

            //lock the bits
            System.Drawing.Imaging.BitmapData bmpData =
                     bm.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                     bm.PixelFormat);
            IntPtr ptr = bmpData.Scan0;

            int bytes = b.Length;
            Marshal.Copy(ptr, b, 0, bytes);

            // Unlock the bits.
            bm.UnlockBits(bmpData);
            bmpData = null;
        }

        public static BitmapSource CreateBitmapSourceFromFile(string filePath, PixelFormat targetPixelFormat)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                var decoder = BitmapDecoder.Create(fileStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                return new FormatConvertedBitmap(decoder.Frames[0], targetPixelFormat, null, 0);
            }
        }

        public static System.Drawing.Bitmap ConvertByteArrayToBitmap(byte[] bytes)
        {
            System.Drawing.Bitmap img = null;
            try
            {
                if (bytes != null && bytes.Length != 0)
                {
                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        img = new System.Drawing.Bitmap(ms);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return img;
        }

        /// <summary>
        /// BitmapImage to Bitmap.
        /// </summary>
        /// <param name="bitmapImage"></param>
        /// <returns></returns>
        public static Bitmap BitmapImage2Bitmap(BitmapSource bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        /// <summary>
        /// Bitmap to BitmapImage.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapSource Bitmap2BitmapImage(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            BitmapSource retval;

            try
            {
                retval = Imaging.CreateBitmapSourceFromHBitmap(
                             hBitmap,
                             IntPtr.Zero,
                             Int32Rect.Empty,
                             BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);
            }

            return retval;
        }

        /// <summary>
        /// 获取位图信息
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static BitmapInfo GetBitmapPixels(Bitmap src)
        {
            int width = src.Width;
            int height = src.Height;
            Rectangle rect = new Rectangle(0, 0, width, height);
            System.Drawing.Imaging.BitmapData bmpData = src.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, src.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int bytes = width * height * GetBitmapChannels(src);
            byte[] pixelValues = new byte[bytes];
            Marshal.Copy(ptr, pixelValues, 0, bytes);
            src.UnlockBits(bmpData);
            return new BitmapInfo { data = pixelValues, step = bmpData.Stride };

        }

        /// <summary>
        /// 获取Bitmap图像通道数
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static int GetBitmapChannels(Bitmap src)
        {
            int channels;
            switch (src.PixelFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                    channels = 3; break;
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                case System.Drawing.Imaging.PixelFormat.Format32bppPArgb:
                    channels = 4; break;
                case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
                case System.Drawing.Imaging.PixelFormat.Format1bppIndexed:
                    channels = 1; break;
                default:
                    throw new NotImplementedException();
            }
            return channels;
        }

        public static System.Windows.Media.PixelFormat ConvertPixelFormat(System.Drawing.Imaging.PixelFormat sourceFormat)
        {
            switch (sourceFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    return PixelFormats.Bgr24;

                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    return PixelFormats.Bgra32;

                case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                    return PixelFormats.Bgr32;

                    // .. as many as you need...
            }
            return new System.Windows.Media.PixelFormat();
        }

        public static System.Drawing.Imaging.PixelFormat ConvertPixelFormat2(System.Windows.Media.PixelFormat sourceFormat)
        {
            switch (sourceFormat.BitsPerPixel)
            {
                case 24:
                    return System.Drawing.Imaging.PixelFormat.Format24bppRgb;

                case 32:
                    return System.Drawing.Imaging.PixelFormat.Format32bppRgb;

                    // .. as many as you need...
            }
            return new System.Drawing.Imaging.PixelFormat();
        }

        public Bitmap GetChannels(Image image, bool r, bool g, bool b)
        {

            /*
             1: piksel rengini al
             2: rengi integer değere çevir
             3: eğer  integer değeri >> 16 ile işleme sokup 0xFF ile ANDlersek o rengin kırmızı tonunu elde ederiz
                eğer  integer değeri >> 8 ile işleme sokup 0xFF ile ANDlersek o rengin yeşil tonunu elde ederiz
                eğer  integer değeri 0xFF ile ANDlersek o rengin mavi tonunu elde ederiz
             4: işlemlerden sonra istenilmeyen rengi yok etmek için 0x00 ile andleyerek o rengi yok edebiliriz
             
             
             
             */
            byte bR = 0xFF, bG = 0xFF, bB = 0xFF; // her sayıyı 1 ile andlemek kendisne eşittir
            if (!r)
            {
                bR = 0x00; // her sayıyı 0 ile andlemek 0 eşittir
            }
            if (!g)
            {
                bG = 0x00;
            }
            if (!b)
            {
                bB = 0x00;
            }
            Bitmap bitmap = new Bitmap(image);
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    System.Drawing.Color i = bitmap.GetPixel(x, y);
                    if (i.R == 0 && i.G == 0 && i.A == 0 && i.B == 0)
                        continue;
                    int bitmapColor = i.ToArgb();
                    int redPixel = (bitmapColor >> 16) & bR;
                    int greenPixel = (bitmapColor >> 8) & bG;
                    int bluePixel = bitmapColor & bB;
                    System.Drawing.Color color = System.Drawing.Color.FromArgb(redPixel, greenPixel, bluePixel);
                    bitmap.SetPixel(x, y, color);
                }
            }
            return bitmap;
        }
    }
}
