using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace Infrastructure
{
    /// <summary>
    /// QRCode 生成类
    /// </summary>
    public class QRCodeHelper
    {
        /// <summary>
        /// 生成QRCode 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="level">容错级别 0-3 Gma.QrCodeNet.Encoding.ErrorCorrectionLevel</param>
        /// <param name="Multiple">倍数</param>
        /// <returns></returns>
        public static byte[] CreateQRCodeByte(string code = "", int level = 2, int Multiple = 1)
        {
            byte[] imageBytes = null;
            if (string.IsNullOrWhiteSpace(code))
            {
                return null;
            }
            try
            {
                // ErrorCorrectionLevel:容错率：容错率越高, 有效像素点就越多，图片也就越大
                var qrEncoder = new Gma.QrCodeNet.Encoding.QrEncoder((Gma.QrCodeNet.Encoding.ErrorCorrectionLevel)level);
                var qrCode = new Gma.QrCodeNet.Encoding.QrCode();
                qrEncoder.TryEncode(code, out qrCode);

                MemoryStream ms = new MemoryStream();
                var imgWidth = qrCode.Matrix.Width; //图片宽度 以生成的码量为宽度
                Bitmap bmp = new Bitmap(imgWidth, imgWidth);

                Graphics g = Graphics.FromImage(bmp);

                Pen pB = new Pen(Color.Black, 1f); //黑笔
                Pen pW = new Pen(Color.White, 1f); //白笔

                g.Clear(Color.White);  // 背景色白

                for (int j = 0; j < qrCode.Matrix.Width; j++)
                {
                    for (int i = 0; i < qrCode.Matrix.Width; i++)
                    {
                        //char charToPrint = qrCode.Matrix[i, j] ? '█' : ' ';  //此乃生成的QRCode矩阵 
                        //根据QRCode矩阵画点
                        if (qrCode.Matrix[i, j])
                        {
                            bmp.SetPixel(i, j, Color.Black);
                            //g.DrawRectangle(pB, i, j, imgWidth, Multiple); 
                        }
                        else
                        {
                            bmp.SetPixel(i, j, Color.White);
                            //g.DrawRectangle(pW, i * Multiple, j * Multiple, Multiple, Multiple);
                        }
                    }
                }
                //把生成的QRCode原图放大，Multiple为倍数
                Bitmap bmpNew = new Bitmap(bmp, new Size(imgWidth * Multiple, imgWidth * Multiple));
                using (ms)
                { 
                    bmpNew.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);//将图片保存至内存流 
                    imageBytes = new byte[ms.Length];
                    ms.Position = 0;
                    //通过内存流读取到imageBytes
                    ms.Read(imageBytes, 0, imageBytes.Length);
                    ms.Close();
                    bmp.Dispose();
                    bmpNew.Dispose();
                    return imageBytes;
                }
            }
            catch
            {
                return imageBytes;
            }
        }

        /// <summary>
        /// 生成QRCode 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="level">容错级别 0-3 Gma.QrCodeNet.Encoding.ErrorCorrectionLevel</param>
        /// <param name="Multiple">倍数</param>
        /// <returns></returns>
        public static Bitmap CreateQRCodeBitmap(string code = "", int level = 2, int Multiple = 1)
        {
            Bitmap bmpRtn = null;
            if (string.IsNullOrWhiteSpace(code))
            {
                return null;
            }
            try
            {
                // ErrorCorrectionLevel:容错率：容错率越高, 有效像素点就越多，图片也就越大
                var qrEncoder = new Gma.QrCodeNet.Encoding.QrEncoder((Gma.QrCodeNet.Encoding.ErrorCorrectionLevel)level);
                var qrCode = new Gma.QrCodeNet.Encoding.QrCode();
                qrEncoder.TryEncode(code, out qrCode);

                MemoryStream ms = new MemoryStream();
                var imgWidth = qrCode.Matrix.Width; //图片宽度 以生成的码量为宽度
                Bitmap bmp = new Bitmap(imgWidth, imgWidth);

                Graphics g = Graphics.FromImage(bmp);

                Pen pB = new Pen(Color.Black, 1f); //黑笔
                Pen pW = new Pen(Color.White, 1f); //白笔

                g.Clear(Color.White);  // 背景色白

                for (int j = 0; j < qrCode.Matrix.Width; j++)
                {
                    for (int i = 0; i < qrCode.Matrix.Width; i++)
                    {
                        //char charToPrint = qrCode.Matrix[i, j] ? '█' : ' ';  //此乃生成的QRCode矩阵 
                        //根据QRCode矩阵画点
                        if (qrCode.Matrix[i, j])
                        {
                            bmp.SetPixel(i, j, Color.Black);
                            //g.DrawRectangle(pB, i, j, imgWidth, Multiple); 
                        }
                        else
                        {
                            bmp.SetPixel(i, j, Color.White);
                            //g.DrawRectangle(pW, i * Multiple, j * Multiple, Multiple, Multiple);
                        }
                    }
                }
                //把生成的QRCode原图放大，Multiple为倍数
                Bitmap bmpNew = new Bitmap(bmp, new Size(imgWidth * Multiple, imgWidth * Multiple));
                using (ms)
                {
                    bmpRtn = bmpNew; 
                    bmp.Dispose();
                    bmpNew.Dispose();
                    return bmpRtn;
                }
            }
            catch 
            {
                return bmpRtn;
            }
        }
    }
}
