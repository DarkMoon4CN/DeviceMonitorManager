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
    public class YY_HS_DATA_AUTOController : ApiController
    {

        /// <summary>
        ///  FFT列表
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultDto<dynamic,dynamic> YY_HS_DATA_AUTOByPage(FFTSearchRequest obj)
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
                    YY_HS_DATA_AUTO info = new YY_HS_DATA_AUTO();
                    info.STCD = obj.STCD;
                    info.ItemID = obj.ItemID;
                    var data = YY_HS_DATA_AUTO_BLL.Instance.YY_HS_DATA_AUTOByPage(info,page);
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
