using System;
using System.Collections.Generic;

namespace WMS.Controllers
{
    public class Datas
    {
        public int antenna { get; set; }
        public int eventNum { get; set; }
        public string format { get; set; }
        public string idHex { get; set; }
        public int reads { get; set; }
        public string userDefined { get; set; }
    }

    public class Root
    {
        public Datas data { get; set; }
        public string timestamp { get; set; }
        public string type { get; set; }
    }


}