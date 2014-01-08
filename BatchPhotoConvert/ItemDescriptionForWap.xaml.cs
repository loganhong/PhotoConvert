using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BatchPhotoConvert
{
    /// <summary>
    /// ItemDescriptionForWap.xaml 的交互逻辑
    /// </summary>
    public partial class ItemDescriptionForWap : Window
    {
        public ItemDescriptionForWap()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void ButtonConvertToWap_Click(object sender, RoutedEventArgs e)
        {
            ShowBox showBox = new ShowBox();
            try
            {
                //int value = 3;
                //EnumForPlatforms fp = (EnumForPlatforms)value;
                //fp = (EnumForPlatforms)Enum.ToObject(typeof(EnumForPlatforms), 6);
                //bool flag = fp.HasFlag(EnumForPlatforms.ISO);


                //var description = GetDescriptionForPlatforms(fp);
                //this.wapDescription.Text = fp.ToString();
                //showBox.TxtMSG.Text = description;
                IItemDescriptionServiceInfo service = new ItemDescriptionInfoBL();
                string wwwHtml = this.wwwDescription.Text;
                string wapHtml = service.GetWapHtml(wwwHtml);
                this.wapDescription.Text = wapHtml;
                showBox.TxtMSG.Text = wapHtml;
                showBox.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                showBox.Show();
            }
            catch (Exception ex)
            {
                showBox.TxtMSG.Text = ex.Message;
                showBox.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                showBox.Show();
            }
        }


        [FlagsAttribute]
        private enum EnumForPlatforms
        {
            [DescriptionAttribute("web")]
            WWW = 1,
            [DescriptionAttribute("移动设备")]
            WAP = 2,
            [DescriptionAttribute("苹果")]
            ISO = 4,
            [DescriptionAttribute("安卓")]
            ANDROIN = 8
        }

        private string GetDescriptionForPlatforms(EnumForPlatforms fp)
        {
            string description = string.Empty;
            var fields = fp.GetType().GetFields((BindingFlags)24);
            //var fields = fp.GetType().GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (var fi in fields)
            {
                var attr = Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute), true);
                if (fp.HasFlag((EnumForPlatforms)Enum.Parse(fp.GetType(), fi.Name)))
                {
                    description += string.IsNullOrEmpty(description) ? ((DescriptionAttribute)attr).Description : "|" + ((DescriptionAttribute)attr).Description;
                }
            }
            return description;
        }
    }
}
