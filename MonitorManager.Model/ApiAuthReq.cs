using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorManager.Model
{
    public class ApiAuthReq
    {
        /// <summary>
        /// 安全码
        /// </summary>
        public string AppSecret { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 请求数据体字符串
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public string TimeStamp { get; set; }
    }
}
