using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BatchPhotoConvert
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            PhotoWater photoWater = new PhotoWater();

            double targetWidth = 0;
            double.TryParse(txtWidth.Text, out targetWidth);
            double targetHeight = 0;
            double.TryParse(txtHeight.Text, out targetHeight);
            photoWater.Hight = targetHeight;
            photoWater.Width = targetWidth;

            OpenFileDialog open = new OpenFileDialog();//定义打开文本框实体
            open.Title = "打开文件";//对话框标题
            open.Filter = "图片文件(*.jpg,*.png)|*.jpg;*.png";//文件扩展名

            if (open.ShowDialog() == System.Windows.Forms.DialogResult.OK)//打开
            {
                if (open.CheckFileExists)
                {
                    string fileFullName = open.FileName;
                    photoWater.InputStream = File.OpenRead(fileFullName);
                    if (photoWater.InputStream != null)
                    {
                        if (string.IsNullOrEmpty(txtfileName.Text.Trim()))
                        {
                            photoWater.FileName = string.Format("{0}_logo{1}", System.IO.Path.GetFileNameWithoutExtension(fileFullName), ".jpg");
                        }
                        else
                        {
                            photoWater.FileName = string.Format("{0}{1}", txtfileName.Text.Trim(), ".jpg");
                        }
                        if (string.IsNullOrEmpty(txtfilePath.Text.Trim()))
                        {
                            photoWater.FileDirectory = System.IO.Path.GetDirectoryName(fileFullName);
                        }
                        else
                        {
                            photoWater.FileDirectory = txtfilePath.Text.Trim();
                        }

                        photoWater.ImageSavePath = System.IO.Path.Combine(photoWater.FileDirectory, photoWater.FileName);

                        photoWater.WaterText1 = txtWaterText1.Text.ToString().Trim();
                        photoWater.WaterText2 = txtWaterText2.Text.ToString().Trim();

                        photoWater.font = new System.Drawing.Font("微软雅黑", 10, System.Drawing.FontStyle.Regular);
                        photoWater.brush = (bool)ckbFontIsBlack.IsChecked == true ? new SolidBrush(System.Drawing.Color.Black) : new SolidBrush(System.Drawing.Color.Azure);

                        photoWater.IsNeedWhite = (bool)ckbIsWhite.IsChecked;
                        photoWater.IsRightUp = (bool)ckbIsRightUp.IsChecked;
                        if (ckbIsWater.IsChecked == true)
                        {
                            //PhotoZip.ZoomAuto(inputFile, System.IO.Path.Combine(filePath, fileName), targetWidth, targetHeight, string.Empty, @"\\JHLOGANHONG\\logo\\LOGO231x67.png", true);
                            photoWater.WaterImageFullPath = @"\\JHLOGANHONG\\logo\\LOGO231x67.png";
                        }

                        PhotoZip.ZoomAuto(photoWater.InputStream, photoWater.ImageSavePath, targetWidth, targetHeight, photoWater.WaterText1, photoWater.WaterText2, photoWater.WaterImageFullPath, photoWater.font, photoWater.brush, photoWater.IsNeedWhite, photoWater.IsRightUp);

                    }
                }
            }
        }

    }
}
