using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchPhotoConvert
{
    public class PhotoZip
    {

        #region 图片等比缩放_水印
        /// <summary>
        /// 图片等比缩放
        /// </summary>
        /// <param name="inputStream">原图流对象</param>
        /// <param name="savePath">缩略图存放地址</param>
        /// <param name="targetWidth">指定的最大宽度</param>
        /// <param name="targetHeight">指定的最大高度</param>
        /// <param name="watermarkText">水印文字(为""表示不使用水印)</param>
        /// <param name="watermarkImage">水印图片路径(为""表示不使用水印)</param>
        public static void ZoomAuto(Stream inputStream, string savePath, System.Double targetWidth, System.Double targetHeight, string watermarkText1, string watermarkText2, string watermarkImage, Font font, Brush brush, bool isNeedWhite, bool isRightUp)
        {
            try
            {
                //创建目录
                string dir = Path.GetDirectoryName(savePath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                //原始图片（获取原始图片创建对象，并使用流中嵌入的颜色管理信息）
                System.Drawing.Image initImage = System.Drawing.Image.FromStream(inputStream, true);
                targetHeight = targetHeight == 0 ? initImage.Height : targetHeight;
                targetWidth = targetWidth == 0 ? initImage.Width : targetWidth;

                if (initImage.Width == targetWidth && initImage.Height == targetHeight)
                {
                    SetWaterMark(watermarkText1, watermarkText2, watermarkImage, initImage, font, brush, isRightUp);

                    initImage.Save(savePath);
                    initImage.Dispose();
                    return;
                }

                //缩略图宽、高计算
                double newWidth = initImage.Width;
                double newHeight = initImage.Height;

                double rate = targetHeight / targetWidth;
                double r = newHeight / newWidth;

                if (rate > r)
                {
                    newWidth = targetWidth;
                    newHeight = Convert.ToInt32(targetWidth * r);
                }
                else
                {
                    newHeight = targetHeight;
                    newWidth = Convert.ToInt32(targetHeight / r);
                }

                //生成新图
                //新建一个bmp图片
                System.Drawing.Image newImage = new System.Drawing.Bitmap((int)newWidth, (int)newHeight);

                //新建一个画板
                System.Drawing.Graphics newG = System.Drawing.Graphics.FromImage(newImage);
                //设置质量
                newG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                newG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //置背景色
                newG.Clear(Color.White);
                //画图
                newG.DrawImage(initImage, new System.Drawing.Rectangle(0, 0, newImage.Width, newImage.Height), new System.Drawing.Rectangle(0, 0, initImage.Width, initImage.Height), System.Drawing.GraphicsUnit.Pixel);

                if (isNeedWhite)
                {
                    System.Drawing.Image targetImage = new System.Drawing.Bitmap((int)targetWidth, (int)targetHeight);
                    System.Drawing.Graphics targetG = System.Drawing.Graphics.FromImage(targetImage);
                    int x = (int)((targetWidth - newImage.Width) / 2);
                    int y = (int)((targetHeight - newImage.Height) / 2);
                    targetG.Clear(Color.White);
                    targetG.DrawImage(newImage, new System.Drawing.Rectangle(x, y, newImage.Width, newImage.Height), new System.Drawing.Rectangle(0, 0, newImage.Width, newImage.Height), System.Drawing.GraphicsUnit.Pixel);

                    //SetWaterMark(watermarkText, watermarkImage, targetImage);
                    SetWaterMark(watermarkText1, watermarkText2, watermarkImage, newImage, font, brush, isRightUp);
                    //保存缩略图
                    targetImage.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    //释放资源
                    targetG.Dispose();
                    targetImage.Dispose();
                    newG.Dispose();
                    newImage.Dispose();
                    initImage.Dispose();
                }
                else
                {
                    //SetWaterMark(watermarkText, watermarkImage, newImage);
                    SetWaterMark(watermarkText1, watermarkText2, watermarkImage, newImage, font, brush, isRightUp);
                    //保存缩略图
                    newImage.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    //释放资源
                    newG.Dispose();
                    newImage.Dispose();
                    initImage.Dispose();
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        private static void SetWaterMark(string watermarkText1, string watermarkText2, string watermarkImage, System.Drawing.Image targetImage, Font font, Brush brush, bool isRightUp)
        {
            try
            {
                //文字水印
                if (!string.IsNullOrEmpty(watermarkText1.Trim()) || !string.IsNullOrEmpty(watermarkText2.Trim()))
                {
                    using (System.Drawing.Graphics gWater = System.Drawing.Graphics.FromImage(targetImage))
                    {
                        //System.Drawing.Font fontWater = new Font("微软雅黑", 10, FontStyle.Regular);

                        //System.Drawing.Brush brushWater = fontIsBlack == true ? new SolidBrush(Color.Black) : new SolidBrush(Color.Azure);

                        System.Drawing.Font fontWater = font;
                        System.Drawing.Brush brushWater = brush;

                        if (isRightUp)
                        {
                            float text1Width = fontWater.Size * watermarkText1.Length;
                            float text2Width = fontWater.Size * watermarkText2.Length;
                            gWater.DrawString(watermarkText1, fontWater, brushWater, targetImage.Width - text1Width, 0);
                            gWater.DrawString(watermarkText2, fontWater, brushWater, targetImage.Width - text2Width + 30, 12);
                        }
                        else
                        {
                            gWater.DrawString(watermarkText1, fontWater, brushWater, 4, 0);

                            gWater.DrawString(watermarkText2, fontWater, brushWater, 4, 12);
                            gWater.Dispose();
                        }
                    }
                }

                //透明图片水印
                if (watermarkImage != "")
                {
                    if (File.Exists(watermarkImage))
                    {
                        //获取水印图片
                        using (System.Drawing.Image wrImage = System.Drawing.Image.FromFile(watermarkImage))
                        {
                            //水印绘制条件：原始图片宽高均大于或等于水印图片
                            if (targetImage.Width >= wrImage.Width && targetImage.Height >= wrImage.Height)
                            {
                                Graphics gWater = Graphics.FromImage(targetImage);
                                //透明属性
                                ImageAttributes imgAttributes = new ImageAttributes();
                                ColorMap colorMap = new ColorMap();
                                colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
                                colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
                                ColorMap[] remapTable = { colorMap };
                                imgAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);
                                float[][] colorMatrixElements = { 
                                   new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                                   new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
                                   new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
                                   new float[] {0.0f,  0.0f,  0.0f,  0.5f, 0.0f},//透明度:0.5
                                   new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
                                };
                                ColorMatrix wmColorMatrix = new ColorMatrix(colorMatrixElements);
                                imgAttributes.SetColorMatrix(wmColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                                gWater.DrawImage(wrImage, new Rectangle(targetImage.Width - wrImage.Width, targetImage.Height - wrImage.Height, wrImage.Width, wrImage.Height), 0, 0, wrImage.Width, wrImage.Height, GraphicsUnit.Pixel, imgAttributes);
                                gWater.Dispose();
                            }
                            wrImage.Dispose();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        //#region 图片等比缩放_水印
        ///// <summary>
        ///// 图片等比缩放
        ///// </summary>
        ///// <param name="inputStream">原图流对象</param>
        ///// <param name="savePath">缩略图存放地址</param>
        ///// <param name="targetWidth">指定的最大宽度</param>
        ///// <param name="targetHeight">指定的最大高度</param>
        ///// <param name="watermarkText">水印文字(为""表示不使用水印)</param>
        ///// <param name="watermarkImage">水印图片路径(为""表示不使用水印)</param>
        //public static void ZoomAuto(Stream inputStream, string savePath, System.Double targetWidth, System.Double targetHeight, string watermarkText, string watermarkImage)
        //{
        //    //创建目录
        //    string dir = Path.GetDirectoryName(savePath);
        //    if (!Directory.Exists(dir))
        //        Directory.CreateDirectory(dir);
        //    //原始图片（获取原始图片创建对象，并使用流中嵌入的颜色管理信息）
        //    System.Drawing.Image initImage = System.Drawing.Image.FromStream(inputStream, true);

        //    if (initImage.Width == targetWidth && initImage.Height == targetHeight)
        //    {
        //        SetWaterMark(watermarkText, watermarkImage, initImage);

        //        initImage.Save(savePath);
        //        initImage.Dispose();
        //        return;
        //    }

        //    //原图宽高均小于模版，留白，保存
        //    if (initImage.Width <= targetWidth && initImage.Height <= targetHeight)
        //    {
        //        //缩略图宽、高计算
        //        double newWidth = initImage.Width;
        //        double newHeight = initImage.Height;

        //        double rate = targetHeight / targetWidth;
        //        double r = newHeight / newWidth;

        //        if (rate > r)
        //        {
        //            newWidth = targetWidth;
        //            newHeight = Convert.ToInt32(targetWidth * r);
        //        }
        //        else
        //        {
        //            newHeight = targetHeight;
        //            newWidth = Convert.ToInt32(targetHeight / r);
        //        }
        //        //生成新图
        //        //新建一个bmp图片
        //        System.Drawing.Image newImage = new System.Drawing.Bitmap((int)newWidth, (int)newHeight);

        //        //新建一个画板
        //        System.Drawing.Graphics newG = System.Drawing.Graphics.FromImage(newImage);
        //        //设置质量
        //        newG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        //        newG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        //        //置背景色
        //        newG.Clear(Color.White);
        //        //画图
        //        newG.DrawImage(initImage, new System.Drawing.Rectangle(0, 0, newImage.Width, newImage.Height), new System.Drawing.Rectangle(0, 0, initImage.Width, initImage.Height), System.Drawing.GraphicsUnit.Pixel);

        //        System.Drawing.Image targetImage = new System.Drawing.Bitmap((int)targetWidth, (int)targetHeight);
        //        System.Drawing.Graphics targetG = System.Drawing.Graphics.FromImage(targetImage);
        //        int x = (int)((targetWidth - newImage.Width) / 2);
        //        int y = (int)((targetHeight - newImage.Height) / 2);
        //        targetG.Clear(Color.White);
        //        targetG.DrawImage(newImage, new System.Drawing.Rectangle(x, y, newImage.Width, newImage.Height), new System.Drawing.Rectangle(0, 0, newImage.Width, newImage.Height), System.Drawing.GraphicsUnit.Pixel);

        //        SetWaterMark(watermarkText, watermarkImage, targetImage);
        //        //保存缩略图
        //        targetImage.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg);
        //        //释放资源
        //        targetG.Dispose();
        //        targetImage.Dispose();
        //        newG.Dispose();
        //        newImage.Dispose();
        //        initImage.Dispose();
        //    }
        //    else
        //    {
        //        //缩略图宽、高计算
        //        double newWidth = initImage.Width;
        //        double newHeight = initImage.Height;

        //        double rate = targetHeight / targetWidth;
        //        double r = newHeight / newWidth;

        //        if (rate > r)
        //        {
        //            newWidth = targetWidth;
        //            newHeight = Convert.ToInt32(targetWidth * r);
        //        }
        //        else
        //        {
        //            newHeight = targetHeight;
        //            newWidth = Convert.ToInt32(targetHeight / r);
        //        }

        //        //生成新图
        //        //新建一个bmp图片
        //        System.Drawing.Image newImage = new System.Drawing.Bitmap((int)newWidth, (int)newHeight);

        //        //新建一个画板
        //        System.Drawing.Graphics newG = System.Drawing.Graphics.FromImage(newImage);
        //        //设置质量
        //        newG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        //        newG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        //        //置背景色
        //        newG.Clear(Color.White);
        //        //画图
        //        newG.DrawImage(initImage, new System.Drawing.Rectangle(0, 0, newImage.Width, newImage.Height), new System.Drawing.Rectangle(0, 0, initImage.Width, initImage.Height), System.Drawing.GraphicsUnit.Pixel);

        //        System.Drawing.Image targetImage = new System.Drawing.Bitmap((int)targetWidth, (int)targetHeight);
        //        System.Drawing.Graphics targetG = System.Drawing.Graphics.FromImage(targetImage);
        //        int x = (int)((targetWidth - newImage.Width) / 2);
        //        int y = (int)((targetHeight - newImage.Height) / 2);
        //        targetG.Clear(Color.White);
        //        targetG.DrawImage(newImage, new System.Drawing.Rectangle(x, y, newImage.Width, newImage.Height), new System.Drawing.Rectangle(0, 0, newImage.Width, newImage.Height), System.Drawing.GraphicsUnit.Pixel);

        //        SetWaterMark(watermarkText, watermarkImage, targetImage);

        //        //保存缩略图
        //        targetImage.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg);
        //        //释放资源
        //        targetG.Dispose();
        //        targetImage.Dispose();
        //        newG.Dispose();
        //        newImage.Dispose();
        //        initImage.Dispose();
        //    }
        //}

        //private static void SetWaterMark(string watermarkText, string watermarkImage, System.Drawing.Image targetImage)
        //{
        //    //文字水印
        //    if (watermarkText != "")
        //    {
        //        using (System.Drawing.Graphics gWater = System.Drawing.Graphics.FromImage(targetImage))
        //        {
        //            System.Drawing.Font fontWater = new Font("黑体", 10);
        //            System.Drawing.Brush brushWater = new SolidBrush(Color.BlueViolet);
        //            gWater.DrawString(watermarkText, fontWater, brushWater, 0, 0);
        //            gWater.Dispose();
        //        }
        //    }
        //    ////透明图片水印
        //    ////获取水印图片
        //    //using (System.Drawing.Image wrImage = System.Drawing.Image.FromHbitmap(Resource1.icon.GetHbitmap()))
        //    //{
        //    //    //水印绘制条件：原始图片宽高均大于或等于水印图片
        //    //    if (targetImage.Width >= wrImage.Width && targetImage.Height >= wrImage.Height)
        //    //    {
        //    //        Graphics gWater = Graphics.FromImage(targetImage);
        //    //        //透明属性
        //    //        ImageAttributes imgAttributes = new ImageAttributes();
        //    //        ColorMap colorMap = new ColorMap();
        //    //        colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
        //    //        colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
        //    //        ColorMap[] remapTable = { colorMap };
        //    //        imgAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);
        //    //        float[][] colorMatrixElements = { 
        //    //                       new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
        //    //                       new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
        //    //                       new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
        //    //                       new float[] {0.0f,  0.0f,  0.0f,  0.5f, 0.0f},//透明度:0.5
        //    //                       new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
        //    //                    };
        //    //        ColorMatrix wmColorMatrix = new ColorMatrix(colorMatrixElements);
        //    //        imgAttributes.SetColorMatrix(wmColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
        //    //        gWater.DrawImage(wrImage, new Rectangle(targetImage.Width - wrImage.Width, targetImage.Height - wrImage.Height, wrImage.Width, wrImage.Height), 0, 0, wrImage.Width, wrImage.Height, GraphicsUnit.Pixel, imgAttributes);
        //    //        gWater.Dispose();
        //    //    }
        //    //    wrImage.Dispose();
        //    //}

        //    //透明图片水印
        //    if (watermarkImage != "")
        //    {
        //        if (File.Exists(watermarkImage))
        //        {
        //            //获取水印图片
        //            using (System.Drawing.Image wrImage = System.Drawing.Image.FromFile(watermarkImage))
        //            {
        //                //水印绘制条件：原始图片宽高均大于或等于水印图片
        //                if (targetImage.Width >= wrImage.Width && targetImage.Height >= wrImage.Height)
        //                {
        //                    Graphics gWater = Graphics.FromImage(targetImage);
        //                    //透明属性
        //                    ImageAttributes imgAttributes = new ImageAttributes();
        //                    ColorMap colorMap = new ColorMap();
        //                    colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
        //                    colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
        //                    ColorMap[] remapTable = { colorMap };
        //                    imgAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);
        //                    float[][] colorMatrixElements = { 
        //                           new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
        //                           new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
        //                           new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
        //                           new float[] {0.0f,  0.0f,  0.0f,  0.5f, 0.0f},//透明度:0.5
        //                           new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
        //                        };
        //                    ColorMatrix wmColorMatrix = new ColorMatrix(colorMatrixElements);
        //                    imgAttributes.SetColorMatrix(wmColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
        //                    gWater.DrawImage(wrImage, new Rectangle(targetImage.Width - wrImage.Width, targetImage.Height - wrImage.Height, wrImage.Width, wrImage.Height), 0, 0, wrImage.Width, wrImage.Height, GraphicsUnit.Pixel, imgAttributes);
        //                    gWater.Dispose();
        //                }
        //                wrImage.Dispose();
        //            }
        //        }
        //    }
        //}
        //#endregion
    }
}
