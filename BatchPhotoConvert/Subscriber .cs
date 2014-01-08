using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BatchPhotoConvert
{
    public class Subscriber
    {
        public int Count { get; set; }
        public string OnNumberChanged()
        {
            //Publishser pu = new Publishser();
            //pu.NumberChanged(100);
            return "subsriber1";
        }
    }
}
