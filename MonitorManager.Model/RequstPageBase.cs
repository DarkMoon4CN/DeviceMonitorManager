using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorManager.Model
{
    public class RequstPageBase
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int TotalPages
        {
            get
            {
                return (int)Math.Ceiling((double)(((float)this.TotalRows) / ((float)this.PageSize)));
            }
        }
        public int TotalRows { get; set; }
    }
}
