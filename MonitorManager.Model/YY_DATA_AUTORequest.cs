using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorManager.Model
{


    public class YY_DATA_AUTOSRequest {
        public List<YY_DATA_AUTORequest> List { get; set; }
    }

    public class YY_DATA_AUTORequest
    {
        /// <summary>
        /// 项目编号
        /// </summary>
        public string STCD { get; set; }

        /// <summary>
        /// 元素编号
        /// </summary>
        public string ItemID { get; set; }

        /// <summary>
        /// 监测时间
        /// </summary>
        public System.DateTime TM { get; set; }

        /// <summary>
        /// 接受时间
        /// </summary>
        public Nullable<System.DateTime> DOWNDATE { get; set; }

        /// <summary>
        /// 信道类型
        /// </summary>
        public Nullable<int> NFOINDEX { get; set; }
        /// <summary>
        /// 数据值
        /// </summary>
        public Nullable<decimal> DATAVALUE { get; set; }
        /// <summary>
        /// 修正值
        /// </summary>
        public Nullable<decimal> CorrectionVALUE { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public Nullable<int> DATATYPE { get; set; }
        /// <summary>
        ///  监测项目类型
        /// </summary>
        public string STTYPE { get; set; }
    }

    public class YY_DATA_AUTOResponse: YY_DATA_AUTORequest
    {

        /// <summary>
        /// 1.绿 2.蓝 3.黄 4.橙 5.红
        /// </summary>
        public List<int> AlarmsLevels { get; set;}
    }


    public class YY_DATA_AUTOSResponse
    {
        public List<YY_DATA_AUTOResponse> List { get; set; }
    }

    public class YY_DATA_AUTOTabSearchResponse
    {
        /// <summary>
        /// 项目编号
        /// </summary>
        public string STCD { get; set; }

        /// <summary>
        /// 元素编号
        /// </summary>
        public string ItemID { get; set; }

        /// <summary>
        /// 元素名称
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 元素分类编号
        /// </summary>
        public string ItemTypeID { get; set; }

        /// <summary>
        /// 元素分类名称
        /// </summary>
        public string ItemTypeName { get; set; }


        /// <summary>
        /// 元素分类排序
        /// </summary>
        public int? ItemTypeIndex { get; set; }

        /// <summary>
        ///  单位
        /// </summary>
        public string Units { get;set; }

        /// <summary>
        /// 告警级别
        /// </summary>
        public List<int> AlarmsLevels { get; set; }

        /// <summary>
        /// 数据值
        /// </summary>
        public decimal? DATAVALUE { get; set; }

        /// <summary>
        /// 实际时间
        /// </summary>
        public DateTime TM { get; set; }

        public int? ItemIndex { get; set; }
    }


}
