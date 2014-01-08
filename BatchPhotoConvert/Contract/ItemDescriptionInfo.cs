using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BatchPhotoConvert
{
    public class ItemDescriptionInfo
    {
        public int ID { get; set; }
        public string ItemNO { get; set; }
        public string Description { get; set; }
        public string WapDescription { get; set; }
        public string CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? CreateDateTo { get; set; }
        public string EditUser { get; set; }
        public DateTime? EditDate { get; set; }
        public DateTime? EditDateTo { get; set; }
    }
}
