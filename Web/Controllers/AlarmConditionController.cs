using ChuanYe.Utils;
using MonitorManager.BLL;
using MonitorManager.Model;
using MonitorManager.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class AlarmConditionController : Controller
    {
        // GET: AlarmCondition
        public ActionResult Index()
        {
            string mid = CheckRequest.GetString("menuId");
            ViewBag.MenuId = mid;
            return View();
        }

        public ContentResult GetAlarmConditionList()
        {
            string niceName = CheckRequest.GetString("NiceName");
            string page = CheckRequest.GetString("page");
            string rows = CheckRequest.GetString("rows");
            string adddatestart = CheckRequest.GetString("adddatestart");
            string adddateend = CheckRequest.GetString("adddateend");

            DateTime? startTime = string.IsNullOrEmpty(adddatestart) == true ? new Nullable<DateTime>() : adddatestart.ToDateTime();
            DateTime? endTime = string.IsNullOrEmpty(adddateend) == true ? new Nullable<DateTime>() : adddateend.ToDateTime();
            RequstPageBase pager = new RequstPageBase();
            pager.StartTime = startTime;
            pager.EndTime = endTime;
            pager.PageIndex = string.IsNullOrEmpty(page) ? 1 : page.ToInt();
            pager.PageSize = string.IsNullOrEmpty(rows) ? 20 : rows.ToInt();

            View_Alarm_LocalInfo_Item info = new View_Alarm_LocalInfo_Item();
            info.NiceName = niceName;

            var result = AlarmConditionBLL.Instance.AlarmConditionByPage(info, pager);
            var pageJson = result.SerializeJSON();
            return Content("{\"total\": " + pager.TotalRows.ToString() + ",\"rows\":" + pageJson + "}");
        }

        public ActionResult AddPage() {
            return View();
        }

        public ContentResult AddAlarmCondition()
        {
            string stcd = CheckRequest.GetString("STCD");
            string itemID = CheckRequest.GetString("ItemID");
            string condition = CheckRequest.GetString("Condition");
            string DATAVALUE = CheckRequest.GetString("DATAVALUE");
            string ALWaitConvert = CheckRequest.GetString("AlarmLevel");

            //string AlarmAreaMin = CheckRequest.GetString("AlarmAreaMin");
            //string AlarmAreaMax = CheckRequest.GetString("AlarmAreaMax");
            //string DisplayAreaMin = CheckRequest.GetString("DisplayAreaMin");
            //string DisplayAreaMax = CheckRequest.GetString("DisplayAreaMax");


            int alarmLevel = ALWaitConvert.ToInt();
            var exist = AlarmConditionBLL.Instance.GetAlarmCondition(stcd, itemID, alarmLevel);
            if (exist.Count >= 1)
            {
                return Content("{\"msg\":\"已有相同的告警信息！\",\"success\":false}");
            }
            AlarmCondition_Tab info = new AlarmCondition_Tab();
            info.STCD = stcd;
            info.ItemID = itemID;
            info.Condition = condition.ToInt();
            info.DATAVALUE = DATAVALUE.ToDecimal();
            info.AlarmLevel = alarmLevel;
            //if (!string.IsNullOrEmpty(AlarmAreaMin) && !string.IsNullOrEmpty(AlarmAreaMax)
            //    && !string.IsNullOrEmpty(DisplayAreaMin) && !string.IsNullOrEmpty(DisplayAreaMin))
            //{
            //    info.AlarmAreaMax = AlarmAreaMax.ToDecimal();
            //    info.AlarmAreaMin = AlarmAreaMin.ToDecimal();
            //    info.DisplayAreaMax = DisplayAreaMax.ToDecimal();
            //    info.DisplayAreaMin = DisplayAreaMin.ToDecimal();
            //}

            int state = AlarmConditionBLL.Instance.AddAlarmCondition(info);
            if (state > 0)
            {
                return Content("{\"msg\":\"添加成功！\",\"success\":true}");
            }
            else
            {
                return Content("{\"msg\":\"添加失败！\",\"success\":false}");
            }
        }

        public ActionResult EditPage() {
            return View();
        }

        public ContentResult EditAlarmCondition()
        {
            string stcd = CheckRequest.GetString("STCD");
            string itemID = CheckRequest.GetString("ItemID");
            string condition = CheckRequest.GetString("Condition");
            string DATAVALUE = CheckRequest.GetString("DATAVALUE");
            string ALWaitConvert = CheckRequest.GetString("AlarmLevel");

            //string AlarmAreaMin = CheckRequest.GetString("AlarmAreaMin");
            //string AlarmAreaMax = CheckRequest.GetString("AlarmAreaMax");
            //string DisplayAreaMin = CheckRequest.GetString("DisplayAreaMin");
            //string DisplayAreaMax = CheckRequest.GetString("DisplayAreaMax");

            int alarmLevel = ALWaitConvert.ToInt();
            var exist = AlarmConditionBLL.Instance.GetAlarmCondition(stcd, itemID, alarmLevel);
            if (exist.Count >= 2)
            {
                return Content("{\"msg\":\"已有相同的告警信息！\",\"success\":false}");
            }

            AlarmCondition_Tab info = new AlarmCondition_Tab();
            info.STCD = stcd;
            info.ItemID = itemID;
            info.Condition = condition.ToInt();
            info.DATAVALUE = DATAVALUE.ToDecimal();
            info.AlarmLevel = alarmLevel;
            //if (!string.IsNullOrEmpty(AlarmAreaMin) && !string.IsNullOrEmpty(AlarmAreaMax)
            //&& !string.IsNullOrEmpty(DisplayAreaMin) && !string.IsNullOrEmpty(DisplayAreaMin))
            //{
            //    info.AlarmAreaMax = AlarmAreaMax.ToDecimal();
            //    info.AlarmAreaMin = AlarmAreaMin.ToDecimal();
            //    info.DisplayAreaMax = DisplayAreaMax.ToDecimal();
            //    info.DisplayAreaMin = DisplayAreaMin.ToDecimal();
            //}

            int state = AlarmConditionBLL.Instance.UpDataAlarmCondition(info);
            if (state > 0)
            {
                return Content("{\"msg\":\"修改成功！\",\"success\":true}");
            }
            else
            {
                return Content("{\"msg\":\"修改失败！\",\"success\":false}");
            }
        }


        public ContentResult GetLocalInfo()
        {
            List<View_LocalInfo_YY_RTU_Basic> temp = LocalInfoBLL.Instance.GetAll();
            var result = temp.Select(s => new { STCD = s.STCD, NiceName = s.NiceName }).ToList();
            return Content(result.SerializeJSON());
        }

        public ContentResult GetItem()
        {
            string stcd = CheckRequest.GetString("STCD");
            var temp = ItemBLL.Instance.GetAll(stcd);
            var result = temp
                        .Select(s => new { ItemID = s.ItemID, ItemName = s.ItemName })
                        .ToList();
            return Content(result.SerializeJSON());
        }

        public ContentResult DelAlarmCondition()
        {
            string delAlarmCondition = CheckRequest.GetString("DelAlarmCondition");
            int result = 0;
            //STCD_ItemID_AlarmLevel,STCD_ItemID_AlarmLevel
            if (!string.IsNullOrEmpty(delAlarmCondition))
            {
                string[] temp = delAlarmCondition.Split(',');
                for (int i = 0; i < temp.Length; i++)
                {
                    if (temp[i].IndexOf('_') > 0)
                    {
                        var delData =temp[i].Split('_');
                        if (delData.Length == 3)
                        {
                            AlarmCondition_Tab info = new AlarmCondition_Tab()
                            {
                                STCD = delData[0],
                                ItemID = delData[1],
                                AlarmLevel = Convert.ToInt32(delData[2]),
                            };
                            result += AlarmConditionBLL.Instance.DelAlarmCondition(info);
                        }
                    }
                   
                }
            }
            if (result >0)
            {
                return Content("{\"msg\":\"删除成功！\",\"success\":true}");
            }
            else
            {
                return Content("{\"msg\":\"删除失败！\",\"success\":false}");
            }
        }

    }
}