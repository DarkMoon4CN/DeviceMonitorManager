//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace MonitorManager.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AlarmInfo_Tab
    {
        public string AlarmId { get; set; }
        public string STCD { get; set; }
        public string ItemID { get; set; }
        public string AlarmContent { get; set; }
        public int Condition { get; set; }
        public int AlarmLevel { get; set; }
        public string State { get; set; }
        public System.DateTime AlarmTime { get; set; }
        public Nullable<System.DateTime> SendTime { get; set; }
    }
}