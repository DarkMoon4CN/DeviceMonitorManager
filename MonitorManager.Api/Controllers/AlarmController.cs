using MonitorManager.BLL;
using MonitorManager.Model;
using MonitorManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace MonitorManager.Api.Controllers
{
    [EnableCors("*", "*", "*")]
    public class AlarmController : ApiController
    {
        /// <summary>
        ///  告警信息列表
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultDto<dynamic,dynamic> AlarmInfoByPage(AlarmInfoSearchRequest obj)
        {
            ResultDto<dynamic,dynamic> result = new ResultDto<dynamic,dynamic>();
            try
            {
                if (obj != null)
                {
                    RequstPageBase page = new RequstPageBase();
                    page.StartTime = obj.StartTime;
                    page.EndTime = obj.EndTime;
                    if (page.StartTime == null)
                    {
                        page.StartTime = DateTime.Now.AddDays(-1);
                    }
                    if (page.EndTime == null)
                    {
                        page.EndTime = DateTime.Now;
                    }
                    page.PageIndex = obj.PageIndex == 0 ? 1 : obj.PageIndex;
                    page.PageSize = obj.PageSize == 0 ? 20 : obj.PageSize;
                    View_AlarmInfo_LocalInfo_Item info = new View_AlarmInfo_LocalInfo_Item();
                    info.STCD = obj.STCD;
                    info.ItemID = obj.ItemID;
                    info.NiceName = obj.NiceName;
                    info.ItemName = obj.ItemName;
                    var data = AlarmInfoBLL.Instance.AlarmConditionByPage(info,page);
                    result.FirstParam = data;
                    result.SecondParam = page.TotalRows;
                    result.Status = 1;
                    result.Message = "Requst Is Success";
                }
                else
                {
                    result.Status = 0;
                    result.Message = "STCD OR ItemID Is Empty!!";
                }
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = "/Alarm/AlarmInfoByPage Server Error";
                ChuanYe.Utils.LoggerHelper.Error(ex.Message);
            }
            return result;
        }
    }
}
