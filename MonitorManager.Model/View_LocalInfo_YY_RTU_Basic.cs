using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorManager.Model
{
    public class View_LocalInfo_YY_RTU_Basic
    {

        public string STCD { get; set; }
        public string LocaManager { get; set; }
        public string Tel { get; set; }
        public Nullable<decimal> Longitude { get; set; }
        public Nullable<decimal> Latitude { get; set; }
        public Nullable<decimal> Altitude { get; set; }
        public string Address { get; set; }
        public Nullable<System.DateTime> AddTime { get; set; }
        public string Describe { get; set; }

        public string NiceName { get; set; }
        public string PassWord { get; set; }
        public string STCDTemp { get; set; }
       
     
    }
}
