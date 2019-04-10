using MonitorManager.BLL;
using MonitorManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using MonitorManager.Models;
using Microsoft.AspNet.SignalR;
using System.Web.Http.Cors;
using ChuanYe.Utils;
using System.IO;
using System.Web;
using System.Net.Http.Headers;
using System.Text;

namespace MonitorManager.Api.Controllers
{
    [EnableCors("*", "*", "*")]
    public class YY_DATA_AUTOController : ApiController
    {
        private static IHubContext hub = new Lazy<IHubContext>(() => GlobalHost.ConnectionManager.GetHubContext<ItemDataHub>()).Value;
        private static string EXCEL_PATH = AppDomain.CurrentDomain.BaseDirectory + "/EXCEL/" ;

        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultDto<dynamic, dynamic,dynamic> YY_DATA_AUTOByPage(YY_DATA_AUTOSearchRequest obj)
        {
            ResultDto<dynamic, dynamic, dynamic> result = new ResultDto<dynamic, dynamic, dynamic>();
            try
            {
                var info = new View_YY_DATA_AUTO()
                {
                    STCD =obj !=null  && !string.IsNullOrEmpty(obj.STCD) ? obj.STCD:null,
                    ItemID = obj != null && !string.IsNullOrEmpty(obj.ItemID) ? obj.ItemID : null,
                    ItemName = obj != null && obj.ItemName !=null ? obj.ItemName: null,
                };
                 List<dynamic> alarmList = new  List<dynamic>();
                if (!string.IsNullOrEmpty(info.STCD) && !string.IsNullOrEmpty(info.ItemID))
                {
                    var alarmTempList = AlarmConditionBLL.Instance
                        .GetAlarmCondition(info.STCD, info.ItemID, 0);
                    foreach (var item in alarmTempList)
                    {
                         alarmList.Add(new { item.STCD,item.ItemID
                                        ,item.DATAVALUE,item.Condition
                                        ,ConditionName= GetConditionName(item.Condition),item.AlarmLevel });
                    }
                }
                RequstPageBase page = new RequstPageBase();
                page.StartTime = obj.StartTime;
                page.EndTime = obj.EndTime;
                page.PageIndex = obj.PageIndex == 0 ? 1 : obj.PageIndex;
                page.PageSize = obj.PageSize == 0 ? 20 : obj.PageSize;
                result.FirstParam = YY_DATA_AUTO_BLL.Instance.Page2(info, page);
                result.SecondParam = page.TotalRows;
                result.ThirdParam = alarmList;
                result.Status = 1;
                result.Message = "Requst Is Success";
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = "/YY_DATA_AUTO/YY_DATA_AUTOByPage Server Error";
                ChuanYe.Utils.LoggerHelper.Error(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 传输数据集合
        /// </summary>
        /// <param name="obj"></param>
        [HttpPost]
        public void SD_YY_DATA_AUTO(YY_DATA_AUTOSRequest obj)
        {
            ResultDto<dynamic> result = new ResultDto<dynamic>();
            try
            {
                ChuanYe.Utils.LoggerHelper.Info(obj);
                if (obj != null && obj.List != null)
                {
                    //进行分组发送
                    foreach (var item in ItemDataHub.clientList)
                    {
                        string connectionId = item.ConnectionId;
                        if (item.Condition != null && !string.IsNullOrEmpty(item.Condition.STCD)
                               && item.Condition.ItemIDs != null && item.Condition.ItemIDs.Count > 0)
                        {
                            var data = obj.List.Where(p => p.STCD == item.Condition.STCD && item.Condition.ItemIDs.Contains(p.ItemID))
                                                       .ToList(); 
                            List<YY_DATA_AUTOResponse> deepCopy =
                                                       data.SerializeJSON().DeserializeJSON<List<YY_DATA_AUTOResponse>>();
                            foreach (var condition in deepCopy)
                            {
                                decimal dataValue = condition.DATAVALUE == null ? (decimal)0.00 : condition.DATAVALUE.Value;
                                condition.AlarmsLevels = ConditionLogic(condition.STCD, condition.ItemID, dataValue);
                            }
                            if (deepCopy != null && deepCopy.Count > 0)
                            {
                                hub.Clients.Client(connectionId).Callback(deepCopy.SerializeJSON());
                            }
                        }
                        else if(item.Condition != null && !string.IsNullOrEmpty(item.Condition.STCD))
                        {
                            var data = obj.List.Where(p => p.STCD == item.Condition.STCD).ToList();
                            List<YY_DATA_AUTOResponse> deepCopy =
                                                     data.SerializeJSON().DeserializeJSON<List<YY_DATA_AUTOResponse>>();
                            foreach (var condition in deepCopy)
                            {
                                decimal dataValue = condition.DATAVALUE == null ? (decimal)0.00 : condition.DATAVALUE.Value;
                                condition.AlarmsLevels = ConditionLogic(condition.STCD, condition.ItemID, dataValue);
                            }

                            if (deepCopy != null && deepCopy.Count > 0)
                            {
                                hub.Clients.Client(connectionId).Callback(deepCopy.SerializeJSON());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ChuanYe.Utils.LoggerHelper.Error(ex.Message);
            }
           
        }

        /// <summary>
        /// 告警逻辑
        /// </summary>
        /// <param name="stcd"></param>
        /// <param name="itemID"></param>
        /// <param name="dataValue"></param>
        /// <returns></returns>
        private  List<int> ConditionLogic(string stcd,string itemID,decimal dataValue)
        {
            //符合条件的
            List<AlarmCondition_Tab> conditions = 
                             //全局文件半个小时更新一次 lock
                             WebApiApplication.ALARM_CONDIATION 
                            .Where(p => p.STCD == stcd && p.ItemID == itemID).ToList();
            List<int> alarmLevels = new List<int>();
            foreach (var item in conditions)
            {
                if (item.Condition == 1 && dataValue > item.DATAVALUE)
                {
                    alarmLevels.Add(item.AlarmLevel);
                }
                else if (item.Condition == 2 && dataValue < item.DATAVALUE)
                {
                    alarmLevels.Add(item.AlarmLevel);
                }
                else if (item.Condition == 3 && dataValue == item.DATAVALUE)
                {
                    alarmLevels.Add(item.AlarmLevel);
                }
                else if (item.Condition == 4 && dataValue >= item.DATAVALUE)
                {
                    alarmLevels.Add(item.AlarmLevel);
                }
                else  if (item.Condition == 5 && dataValue <= item.DATAVALUE)
                {
                    alarmLevels.Add(item.AlarmLevel);
                }
            }
            return alarmLevels.OrderByDescending(o=>o).ToList();
        }

        private string GetConditionName(int condition)
        {
            if (condition == 1)
            {
                return "大于";
            }
            else if (condition == 2)
            {
                return "小于";
            }
            else if (condition == 3)
            {
                return "等于";
            }
            else if (condition == 4)
            {
                return "大于等于";
            }
            else if (condition == 5)
            {
                return "小于等于";
            }
            else if (condition == 6)
            {
                return "时长超过";
            }
            return null;
        }

        /// <summary>
        /// 横向单一数据 Excel
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetDataForLateralAxisXLS(YY_DATA_AUTOTabSearchRequest obj)
        {
            try
            {
                if (!Directory.Exists(EXCEL_PATH))
                {
                    Directory.CreateDirectory(EXCEL_PATH);
                }
                var list= YY_DATA_AUTO_BLL.Instance.GetDataForOne(obj.STCD, obj.ItemIDs, obj.ItemTypeID, obj.AppointTime,obj.AppointTime);
                if (list == null || list.Count == 0)
                {
                    var lastData = YY_DATA_AUTO_BLL.Instance.GetLastDataByTM(obj.STCD, obj.ItemIDs, obj.ItemTypeID);
                    if (lastData != null && lastData.Count != 0)
                    {
                        DateTime? lastTime = lastData.FirstOrDefault().TM;
                        list=YY_DATA_AUTO_BLL.Instance.GetDataForOne(obj.STCD, obj.ItemIDs, obj.ItemTypeID, lastTime, lastTime);
                    }
                }
                foreach (var condition in list)
                {
                    decimal dataValue = condition.DATAVALUE == null ? (decimal)0.00 : condition.DATAVALUE.Value;
                    condition.AlarmsLevels = ConditionLogic(condition.STCD, condition.ItemID, dataValue);
                }
                DataTable dt = new DataTable("sheet1");
                dt.Columns.Add("序号", typeof(System.Int32));
                dt.Columns.Add("监测参数", typeof(string));
                dt.Columns.Add("数值", typeof(string));
                dt.Columns.Add("单位", typeof(string));
                dt.Columns.Add("是否告警", typeof(string));
                dt.Columns.Add("时间", typeof(string));

                for (int i = 0; i < list.Count; i++)
                {
                    var temp = list[i];
                    string alarmTxt = string.Empty;
                    if (temp.AlarmsLevels != null && temp.AlarmsLevels.Count > 0)
                    {
                        var maxLevel = temp.AlarmsLevels.Max(m=>m);
                        if (maxLevel == 5)
                        {
                            alarmTxt="高高"; 
                        }
                        if (maxLevel == 4)
                        {
                            alarmTxt = "高";
                        }
                    }
                    var dr = dt.NewRow();
                    dr[0] = i + 1;
                    dr[1] = temp.ItemName;
                    dr[2] =temp.DATAVALUE!=null? temp.DATAVALUE.Value.ToString("#0.00"):"0.00";
                    dr[3] = temp.Units;
                    dr[4] = alarmTxt;
                    dr[5] = temp.TM.ToString("yyyy-MM-dd HH:mm:ss");
                    dt.Rows.Add(dr);
                }
                var exist = list.Where(p => p.ItemTypeID == obj.ItemTypeID).FirstOrDefault();
                string excelName = string.Empty;
                if (exist != null)
                {
                    excelName = exist.ItemName + exist.ItemTypeName;
                }
                else
                {
                    excelName = "";
                }
                string footerStr = "操作人：{0}     时间：{1}";
                footerStr = string.Format(footerStr,obj.Operator,DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
                var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                var filePath = EXCEL_PATH+ fileName;
                ExcelHelper.WanHuaTableToExcel(dt, filePath, excelName+"实时数据", footerStr);
                var browser = String.Empty;
                if (HttpContext.Current.Request.UserAgent != null)
                {
                    browser = HttpContext.Current.Request.UserAgent.ToUpper();
                }
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                FileStream fs = new FileStream(filePath, FileMode.Open);
                httpResponseMessage.Content = new StreamContent(fs);
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName =
                       browser.Contains("FIREFOX")
                           ? Path.GetFileName(filePath)
                           : HttpUtility.UrlEncode(Path.GetFileName(filePath))
                };
                return ResponseMessage(httpResponseMessage);
            }
            catch (Exception ex)
            {
                ChuanYe.Utils.LoggerHelper.Error(ex.Message);
                return ResponseMessage(new HttpResponseMessage(HttpStatusCode.Forbidden)); 
            }
          
        }


        /// <summary>
        /// 横向单一数据 Excel
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult DataForLateralAxisXLS(YY_DATA_AUTOTabSearchRequest obj)
        {
            try
            {
                if (!Directory.Exists(EXCEL_PATH))
                {
                    Directory.CreateDirectory(EXCEL_PATH);
                }
                var list = YY_DATA_AUTO_BLL.Instance.GetDataForOne(obj.STCD, obj.ItemIDs, obj.ItemTypeID, obj.AppointTime, obj.AppointTime);
                if (list == null || list.Count == 0)
                {
                    var lastData = YY_DATA_AUTO_BLL.Instance.GetLastDataByTM(obj.STCD, obj.ItemIDs, obj.ItemTypeID);
                    if (lastData != null && lastData.Count != 0)
                    {
                        DateTime? lastTime = lastData.FirstOrDefault().TM;
                        list = YY_DATA_AUTO_BLL.Instance.GetDataForOne(obj.STCD, obj.ItemIDs, obj.ItemTypeID, lastTime, lastTime);
                    }
                }
                foreach (var condition in list)
                {
                    decimal dataValue = condition.DATAVALUE == null ? (decimal)0.00 : condition.DATAVALUE.Value;
                    condition.AlarmsLevels = ConditionLogic(condition.STCD, condition.ItemID, dataValue);
                }
                DataTable dt = new DataTable("sheet1");
                dt.Columns.Add("序号", typeof(System.Int32));
                dt.Columns.Add("监测参数", typeof(string));
                dt.Columns.Add("数值", typeof(string));
                dt.Columns.Add("单位", typeof(string));
                dt.Columns.Add("是否告警", typeof(string));
                dt.Columns.Add("时间", typeof(string));

                for (int i = 0; i < list.Count; i++)
                {
                    var temp = list[i];
                    string alarmTxt = string.Empty;
                    if (temp.AlarmsLevels != null && temp.AlarmsLevels.Count > 0)
                    {
                        var maxLevel = temp.AlarmsLevels.Max(m => m);
                        if (maxLevel == 5)
                        {
                            alarmTxt = "高高";
                        }
                        if (maxLevel == 4)
                        {
                            alarmTxt = "高";
                        }
                    }
                    var dr = dt.NewRow();
                    dr[0] = i + 1;
                    dr[1] = temp.ItemName;
                    dr[2] = temp.DATAVALUE != null ? temp.DATAVALUE.Value.ToString("#0.00") : "0.00";
                    dr[3] = temp.Units;
                    dr[4] = alarmTxt;
                    dr[5] = temp.TM.ToString("yyyy-MM-dd HH:mm:ss");
                    dt.Rows.Add(dr);
                }
               
                string excelName = string.Empty;

                var stcdExist=  LocalInfoBLL.Instance.GetAll(obj.STCD).FirstOrDefault();

                var typExist = list.Where(p => p.ItemTypeID == obj.ItemTypeID).FirstOrDefault();

                if (stcdExist != null)
                {
                    excelName += stcdExist.NiceName;
                }
                if (typExist != null)
                {
                    if (obj.ItemIDs != null && obj.ItemIDs.Count ==1)
                    {
                        excelName += typExist.ItemTypeName + typExist.ItemName;
                    }
                    else
                    {
                        excelName += typExist.ItemTypeName;
                    }
                }
                string footerStr = "操作人：{0}     时间：{1}";
                footerStr = string.Format(footerStr, obj.Operator, DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
                var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                var filePath = EXCEL_PATH + fileName;
                ExcelHelper.WanHuaTableToExcel(dt, filePath, excelName + "实时数据", footerStr);
                var browser = String.Empty;
                if (HttpContext.Current.Request.UserAgent != null)
                {
                    browser = HttpContext.Current.Request.UserAgent.ToUpper();
                }
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                FileStream fs = new FileStream(filePath, FileMode.Open);
                httpResponseMessage.Content = new StreamContent(fs);
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName =
                       browser.Contains("FIREFOX")
                           ? Path.GetFileName(filePath)
                           : HttpUtility.UrlEncode(Path.GetFileName(filePath))
                };
                return ResponseMessage(httpResponseMessage);
            }
            catch (Exception ex)
            {
                ChuanYe.Utils.LoggerHelper.Error(ex.Message);
                return ResponseMessage(new HttpResponseMessage(HttpStatusCode.Forbidden));
            }

        }


        /// <summary>
        /// 横向单一数据 分页
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        public ResultDto<dynamic, dynamic,dynamic> DataForLateralAxisByPage(YY_DATA_AUTOTabSearchRequest obj)
        {
            ResultDto<dynamic, dynamic,dynamic> result = new ResultDto<dynamic, dynamic,dynamic>();
            try
            {
                RequstPageBase page = new RequstPageBase();
                page.PageIndex = obj.PageIndex == 0 ? 1 : obj.PageIndex;
                page.PageSize = obj.PageSize == 0 ? 20 : obj.PageSize;
                page.StartTime = page.EndTime = obj.AppointTime;
                var list = YY_DATA_AUTO_BLL.Instance.GetDataForOneByPage(obj.STCD, obj.ItemIDs, obj.ItemTypeID,page);
                result.SecondParam = page.TotalRows;
                DateTime? lastTime = null;
                if(list==null || list.Count==0)
                {
                    var lastData = YY_DATA_AUTO_BLL.Instance.GetLastDataByTM(obj.STCD, obj.ItemIDs, obj.ItemTypeID);
                    if (lastData != null && lastData.Count != 0)
                    {
                        lastTime = lastData.FirstOrDefault().TM;
                        var tempPage = new RequstPageBase()
                        {
                            PageIndex = page.PageIndex,
                            PageSize = page.PageSize,
                            StartTime = lastTime,
                            EndTime = lastTime,
                        };
                        list = YY_DATA_AUTO_BLL.Instance.GetDataForOneByPage(obj.STCD, obj.ItemIDs, obj.ItemTypeID, tempPage);
                        result.SecondParam = tempPage.TotalRows;
                    }

                }
                foreach (var condition in list)
                {
                    decimal dataValue = condition.DATAVALUE == null ? (decimal)0.00 : condition.DATAVALUE.Value;
                    condition.AlarmsLevels = ConditionLogic(condition.STCD, condition.ItemID, dataValue);
                }
                result.FirstParam = list;
                result.ThirdParam = lastTime;
                result.Status = 1;
                result.Message = "Requst Is Success";

            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = "/YY_DATA_AUTO/DataForLateralAxisByPage Server Error";
                ChuanYe.Utils.LoggerHelper.Error(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 纵向多条数据 分页
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultDto<dynamic, dynamic,dynamic> DataForLongitudinalAxisByPage(YY_DATA_AUTOTabSearchRequest2 obj)
        {
            ResultDto<dynamic, dynamic,dynamic> result = new ResultDto<dynamic, dynamic,dynamic>();
            try
            {
                if (obj != null && obj.ItemIDs != null && obj.ItemIDs.Count>0)
                {
                    RequstPageBase page = new RequstPageBase();
                    page.PageIndex = obj.PageIndex == 0 ? 1 : obj.PageIndex;
                    page.PageSize = obj.PageSize == 0 ? 20 : obj.PageSize;
                    List<DateTime> dts = new List<DateTime>();
                    DateTime os = obj.StartTime.Value;
                    DateTime oe = obj.EndTime.Value;
                    OptimizeAxisTime(obj.SearchType, ref os, ref oe);
                    var tempStartTime = os;
                    var tempEndTime = oe;
                    int allCount = 0;
                    //2019/02/26 倒叙调整
                    while (tempEndTime >= tempStartTime && allCount < 500)
                    {
                        allCount++;
                        dts.Add(tempEndTime);
                        if (obj.SearchType == 1)
                        {
                            tempEndTime = tempEndTime.AddSeconds(-60);
                        }
                        else if (obj.SearchType == 2)
                        {
                            tempEndTime = tempEndTime.AddSeconds(-(60 * 60));
                        }
                        else
                        {
                            tempEndTime = tempEndTime.AddSeconds(-10);
                        }
                    }
                    int startRow = page.PageSize * (page.PageIndex - 1);
                    var alldataTime = dts.Skip(startRow).Take(page.PageSize).ToList();
                    var searchStartDatime = alldataTime.LastOrDefault();
                    var searchEndDatime = alldataTime.FirstOrDefault();
                    var list = YY_DATA_AUTO_BLL.Instance.GetDataForOne(obj.STCD, obj.ItemIDs, obj.ItemTypeID, searchStartDatime, searchEndDatime);

                    //获取出横向轴item名称
                    List<View_LocaInfo_YY_RTU_ITEM> lateralAxis = ItemBLL.Instance.GetItem(obj.STCD, obj.ItemTypeID, obj.ItemIDs);
                    List<string> lateralAxisNameList = lateralAxis.Select(s => s.ItemName + "(" + s.Units + ")").ToList();
                    var tempSearchStartDatime = searchStartDatime;
                    var tempSearchEndDatime = searchEndDatime;
                    List<dynamic> data = new List<dynamic>();
                    while (tempSearchEndDatime >= tempSearchStartDatime)
                    {
                        //获取当前秒数的数据
                        var nowData = list.Where(p => p.TM == tempSearchEndDatime).ToList();
                        List<string> lateralAxisDatas = new List<string>();
                        for (int i = 0; i < lateralAxis.Count; i++)
                        {
                            var detailData = nowData.Where(p => p.STCD == lateralAxis[i].STCD
                                              && p.ItemTypeID == lateralAxis[i].ItemTypeID
                                              && p.ItemID == lateralAxis[i].ItemID).FirstOrDefault();
                            if (detailData == null)
                            {
                                lateralAxisDatas.Add("-");
                            }
                            else
                            {
                                lateralAxisDatas.Add(detailData.DATAVALUE != null ? detailData.DATAVALUE.Value.ToString("#0.00") : "0.00");
                            }
                        }
                        data.Add(new { DateTime = tempSearchEndDatime, LateralAxisData = lateralAxisDatas });

                        if (obj.SearchType == 1)
                        {
                            tempSearchEndDatime = tempSearchEndDatime.AddSeconds(-60);
                        }
                        else if (obj.SearchType == 2)
                        {
                            tempSearchEndDatime = tempSearchEndDatime.AddSeconds(-(60 * 60));
                        }
                        else
                        {
                            tempSearchEndDatime = tempSearchEndDatime.AddSeconds(-10);
                        }
                    }
                    result.FirstParam = lateralAxisNameList;
                    result.SecondParam = data.ToList();
                    result.ThirdParam = dts.Count();
                    result.Status = 1;
                    result.Message = "Requst Is Success";
                }
                else
                {
                    result.Status =0;
                    result.Message = "ItemIDs Field Cannot Be Empty!!";
                }
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = "/YY_DATA_AUTO/DataForLongitAxisByPage Server Error";
                ChuanYe.Utils.LoggerHelper.Error(ex.Message);
            }
            return result;
        }

        /// <summary>
        ///  纵向多条数据 Excel
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult DataForLongitudinalAxisXLS(YY_DATA_AUTOTabSearchRequest2 obj)
        {
            try
            {
                if (obj != null && obj.ItemIDs != null && obj.ItemIDs.Count > 0)
                {
                    if (!Directory.Exists(EXCEL_PATH))
                    {
                        Directory.CreateDirectory(EXCEL_PATH);
                    }

                    RequstPageBase page = new RequstPageBase();
                    page.PageIndex = 1;
                    page.PageSize = int.MaxValue;
                    List<DateTime> dts = new List<DateTime>();

                    DateTime os = obj.StartTime.Value;
                    DateTime oe = obj.EndTime.Value;
                    OptimizeAxisTime(obj.SearchType, ref os, ref oe);
                    var tempStartTime = os;
                    var tempEndTime = oe;
                    int allCount = 0;
                    //2019/02/26 倒叙调整
                    while (tempEndTime >= tempStartTime && allCount < 500)
                    {
                        allCount++;
                        dts.Add(tempEndTime);
                        if (obj.SearchType == 1)
                        {
                            tempEndTime = tempEndTime.AddSeconds(-60);
                        }
                        else if (obj.SearchType == 2)
                        {
                            tempEndTime = tempEndTime.AddSeconds(-(60 * 60));
                        }
                        else
                        {
                            tempEndTime = tempEndTime.AddSeconds(-10);
                        }
                    }
                    int startRow = page.PageSize * (page.PageIndex - 1);
                    var alldataTime = dts.Skip(startRow).Take(page.PageSize).ToList();
                    var searchStartDatime = alldataTime.LastOrDefault();
                    var searchEndDatime = alldataTime.FirstOrDefault();
                    var list = YY_DATA_AUTO_BLL.Instance.GetDataForOne(obj.STCD, obj.ItemIDs, obj.ItemTypeID, searchStartDatime, searchEndDatime);

                    //获取出横向轴item名称
                    List<View_LocaInfo_YY_RTU_ITEM> lateralAxis = ItemBLL.Instance.GetItem(obj.STCD, obj.ItemTypeID, obj.ItemIDs);
                    List<string> lateralAxisNameList = lateralAxis.Select(s => s.ItemName + "(" + s.Units + ")").ToList();
                    var tempSearchStartDatime = searchStartDatime;
                    var tempSearchEndDatime = searchEndDatime;
                    List<dynamic> data = new List<dynamic>();
                    while (tempSearchEndDatime >= tempSearchStartDatime)
                    {
                        //获取当前秒数的数据
                        var nowData = list.Where(p => p.TM == tempSearchEndDatime).ToList();
                        List<string> lateralAxisDatas = new List<string>();
                        for (int i = 0; i < lateralAxis.Count; i++)
                        {
                            var detailData = nowData.Where(p => p.STCD == lateralAxis[i].STCD
                                              && p.ItemTypeID == lateralAxis[i].ItemTypeID
                                              && p.ItemID == lateralAxis[i].ItemID).FirstOrDefault();
                            if (detailData == null)
                            {
                                lateralAxisDatas.Add("-");
                            }
                            else
                            {
                                lateralAxisDatas.Add(detailData.DATAVALUE != null ? detailData.DATAVALUE.Value.ToString("#0.00") : "0.00");
                            }
                        }
                        data.Add(new { DateTime = tempSearchEndDatime, LateralAxisData = lateralAxisDatas });

                        if (obj.SearchType == 1)
                        {
                            tempSearchEndDatime = tempSearchEndDatime.AddSeconds(-60);
                        }
                        else if (obj.SearchType == 2)
                        {
                            tempSearchEndDatime = tempSearchEndDatime.AddSeconds(-(60 * 60));
                        }
                        else
                        {
                            tempSearchEndDatime = tempSearchEndDatime.AddSeconds(-10);
                        }
                    }
                    DataTable dt = new DataTable("sheet1");
                    dt.Columns.Add("    ", typeof(System.String));

                    for (int i = 0; i < lateralAxisNameList.Count; i++)
                    {
                        dt.Columns.Add("" + lateralAxisNameList[i], typeof(string));
                    }

                    for (int m = 0; m < data.Count; m++)
                    {
                        var dr = dt.NewRow();
                        dr[0] = (data[m].DateTime as DateTime?).ToString();
                        var tempData = data[m].LateralAxisData as List<string>;
                        for (int n = 0; n < tempData.Count; n++)
                        {
                            dr[n + 1] = tempData[n];
                        }
                        dt.Rows.Add(dr);
                    }

                    string excelName = string.Empty;
                    var stcdExist = LocalInfoBLL.Instance.GetAll(obj.STCD).FirstOrDefault();
                    var typExist = ItemTypeBLL.Instance.GetItemTypeByID(obj.ItemTypeID);
                   
                    if (stcdExist != null)
                    {
                        excelName += stcdExist.NiceName;
                    }
                    if (typExist != null)
                    {
                        if (obj.ItemIDs != null && obj.ItemIDs.Count == 1)
                        {
                            var itemExist = ItemBLL.Instance.GetItem(obj.ItemIDs[0]).FirstOrDefault();
                            excelName += typExist.ItemType + itemExist != null ? itemExist.ItemName:string.Empty;
                        }
                        else
                        {
                            excelName += typExist.ItemType;
                        }
                    }
                    string footerStr = "操作人：{0}     时间：{1}";
                    footerStr = string.Format(footerStr, obj.Operator, DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
                    var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    var filePath = EXCEL_PATH + fileName;

                    ExcelHelper.WanHuaTableToExcel(dt, filePath, excelName + "历史数据", footerStr);
                    var browser = String.Empty;
                    if (HttpContext.Current.Request.UserAgent != null)
                    {
                        browser = HttpContext.Current.Request.UserAgent.ToUpper();
                    }
                    HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                    FileStream fs = new FileStream(filePath, FileMode.Open);
                    httpResponseMessage.Content = new StreamContent(fs);
                    httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName =
                           browser.Contains("FIREFOX")
                               ? Path.GetFileName(filePath)
                               : HttpUtility.UrlEncode(Path.GetFileName(filePath))
                    };
                    return ResponseMessage(httpResponseMessage);
                }
                else
                {
                    return ResponseMessage(new HttpResponseMessage(HttpStatusCode.BadRequest));
                }
            }
            catch (Exception ex)
            {
                ChuanYe.Utils.LoggerHelper.Error(ex.Message);
                return ResponseMessage(new HttpResponseMessage(HttpStatusCode.Forbidden));
            }
        }

        /// <summary>
        /// 根据纵向数据类型进行优化时间
        /// </summary>
        /// <param name="SearchType"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        private void OptimizeAxisTime(int searchType,ref DateTime startTime,ref DateTime endTime)
        {
            if (searchType == 0)
            {
                startTime = startTime.AddSeconds(-(startTime.Second % 10));
                endTime = endTime.AddSeconds(-(endTime.Second % 10));
            }
            if (searchType == 1)
            {
                startTime =Convert.ToDateTime(startTime.ToString("yyyy-MM-dd HH:mm"));
                endTime = Convert.ToDateTime(endTime.ToString("yyyy-MM-dd HH:mm"));
            }
            if (searchType == 2)
            {
                startTime = Convert.ToDateTime(startTime.ToString("yyyy-MM-dd")+" "+startTime.Hour+":00:00");
                endTime = Convert.ToDateTime(endTime.ToString("yyyy-MM-dd") + " "+endTime.Hour + ":00:00");
            }
        }

    }
}
