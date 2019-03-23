using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorManager.Model
{
    public class LocalInfoSearchRequest : RequstPageBase
    {
        public string NiceName { get; set; }

        public string Address { get; set; }
    }

    public class LocalInfoImageRequest
    {
        public string STCD { get; set; }
    }

    /// <summary>
    /// 轮询时，把上一次获取到的数据第一条中的TM当做请求参数
    /// </summary>
    public class YY_DATA_AUTOSearchRequest : RequstPageBase
    {
        /// <summary>
        ///  项目ID
        /// </summary>
        public string STCD { get; set; }

        /// <summary>
        ///  元素ID
        /// </summary>
        public string ItemID { get; set; }
        /// <summary>
        ///  元素名称
        /// </summary>
        public string ItemName { get; set; }

    }

    public class YY_DATA_AUTOTabSearchRequest : RequstPageBase
    {
        /// <summary>
        ///  项目ID
        /// </summary>
        public string STCD { get; set; }

        /// <summary>
        ///  元素ID
        /// </summary>
        public List<string> ItemIDs{ get; set; }

        /// <summary>
        /// 元素类型
        /// </summary>

        public string ItemTypeID { get; set; }

        /// <summary>
        /// 指定时间
        /// </summary>
        public DateTime? AppointTime { get; set; }


        /// <summary>
        /// 操作人 当前登录用户 名称
        /// </summary>
        public string Operator { get; set; }
    }

    public class YY_DATA_AUTOTabSearchRequest2:RequstPageBase
    {
        /// <summary>
        ///  项目ID
        /// </summary>
        public string STCD { get; set; }

        /// <summary>
        ///  元素ID
        /// </summary>
        public List<string> ItemIDs { get; set; }

        /// <summary>
        /// 元素类型
        /// </summary>

        public string ItemTypeID { get; set; }


        /// <summary>
        /// 操作人 当前登录用户 名称
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// 查询类型  0.10秒间隔   1.1分钟间隔  2.1小时间隔
        /// </summary>
        public int SearchType { get; set; }
    }



}
