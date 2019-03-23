using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorManager.Model
{
    public class AlarmInfoSearchRequest : RequstPageBase
    {
        /// <summary>
        ///  项目ID
        /// </summary>
        public string STCD { get; set; }

        /// <summary>
        ///  项目名称 选填
        /// </summary>
        public string NiceName { get; set; }


        /// <summary>
        ///  元素ID 选填
        /// </summary>
        public string ItemID { get; set; }


        /// <summary>
        ///  元素名称 选填
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 告警等级 选填
        /// </summary>
        public int AlarmLevel { get; set; }


    }
}
