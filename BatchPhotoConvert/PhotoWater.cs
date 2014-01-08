using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BatchPhotoConvert
{
    public class PhotoWater
    {
        public System.IO.Stream InputStream { get; set; }

        public string ImageSavePath { get; set; }

        public string FileName { get; set; }

        public string FileDirectory { get; set; }

        public Double Width { get; set; }

        public Double Hight { get; set; }

        public string WaterText1 { get; set; }

        public string WaterText2 { get; set; }

        public System.Drawing.Font font { get; set; }

        public System.Drawing.Brush brush { get; set; }

        public bool IsRightUp { get; set; }

        public bool IsNeedWhite { get; set; }

        public string WaterImageFullPath { get; set; }
    }
}
