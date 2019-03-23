using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorManager.Model
{
    public class FFTSearchRequest : RequstPageBase
    {
         public string STCD { get; set; }

        public string ItemID { get; set; }
    }
}
