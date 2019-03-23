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
    public class ItemController : Web.Factory.BaseController
    {
        // GET: Item
        public ActionResult Index()
        {
            string mid = CheckRequest.GetString("menuId");//左侧菜单  Home/Index  92行
            ViewBag.MenuId = mid;
            return View();
        }

        public ContentResult GetLocalInfoItemList()
        {
            string stcd = CheckRequest.GetString("STCD");
            string itemName = CheckRequest.GetString("ItemName");
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
            View_LocaInfo_YY_RTU_ITEM info = new View_LocaInfo_YY_RTU_ITEM();
            info.ItemName = itemName;
            info.STCD = stcd;

            var result = ItemBLL.Instance.LocalInfoItemByPage(info, pager);
            var pageJson = result.SerializeJSON();
            return Content("{\"total\": " + pager.TotalRows.ToString() + ",\"rows\":" + pageJson + "}");
        }

        public ContentResult GetItemList()
        {
            string itemName = CheckRequest.GetString("ItemName");
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
            View_YY_RTU_ITEM_Alarm info = new View_YY_RTU_ITEM_Alarm();
            info.ItemName = itemName;

            var result = ItemBLL.Instance.ItemAlarmByPage(info, pager);
            var pageJson = result.SerializeJSON();
            return Content("{\"total\": " + pager.TotalRows.ToString() + ",\"rows\":" + pageJson + "}");
        }

        public ContentResult GetItemTypeList()
        {
            string itemID = CheckRequest.GetString("ItemID");
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
            View_YY_RTU_ItemType info = new View_YY_RTU_ItemType();
            info.ItemID = itemID;

            var result = ItemBLL.Instance.ItemTypeByPage(info, pager);
            var pageJson = result.SerializeJSON();
            return Content("{\"total\": " + pager.TotalRows.ToString() + ",\"rows\":" + pageJson + "}");
        }

        public ActionResult AddPage()
        {

            return View();
        }

        public ContentResult AddItem()
        {
            string ItemID = CheckRequest.GetString("ItemID");
            string ItemName = CheckRequest.GetString("ItemName");
            string ItemCode = CheckRequest.GetString("ItemCode");
            string ItemInteger = CheckRequest.GetString("ItemInteger");
            string ItemDecimal = CheckRequest.GetString("ItemDecimal");
            string PlusOrMinus = CheckRequest.GetString("PlusOrMinus");
            string Units = CheckRequest.GetString("Units");

            string AlarmAreaMin = CheckRequest.GetString("AlarmAreaMin");
            string AlarmAreaMax = CheckRequest.GetString("AlarmAreaMax");
            string DisplayAreaMin = CheckRequest.GetString("DisplayAreaMin");
            string DisplayAreaMax = CheckRequest.GetString("DisplayAreaMax");

            YY_RTU_ITEM info = new YY_RTU_ITEM()
            {
                ItemID = ItemID,
                ItemName = ItemName,
                ItemCode = ItemCode,
                PlusOrMinus =  !string.IsNullOrEmpty(PlusOrMinus)&& PlusOrMinus=="1"?true:false,
                ItemInteger = ItemInteger.ToInt(),
                ItemDecimal = ItemDecimal.ToInt(),
                Units=Units,
            };


            ElementsChartsInfo_Tab rel =null;
            if (!string.IsNullOrEmpty(AlarmAreaMin) && !string.IsNullOrEmpty(AlarmAreaMax)
                 && !string.IsNullOrEmpty(DisplayAreaMin) && !string.IsNullOrEmpty(DisplayAreaMin))
            {
                rel = new ElementsChartsInfo_Tab();
                rel.ItemID = info.ItemID;
                rel.AlarmAreaMax = AlarmAreaMax.ToDecimal();
                rel.AlarmAreaMin = AlarmAreaMin.ToDecimal();
                rel.DisplayAreaMax = DisplayAreaMax.ToDecimal();
                rel.DisplayAreaMin = DisplayAreaMin.ToDecimal();
            }

            var exist = ItemBLL.Instance.GetItem(info.ItemID);

            if (exist.Count >= 1)
            {
                return Content("{\"msg\":\"已有相同的元素名称！\",\"success\":false}");
            }

            int state = ItemBLL.Instance.AddItem(info, rel);
            if (state > 0)
            {
                return Content("{\"msg\":\"添加成功！\",\"success\":true}");
            }
            else
            {
                return Content("{\"msg\":\"添加失败！\",\"success\":false}");
            }
        }

        public ActionResult EditPage()
        {
            return View();
        }

        public ContentResult EditItem()
        {
            string ItemID = CheckRequest.GetString("ItemID");
            string ItemName = CheckRequest.GetString("ItemName");
            string ItemCode = CheckRequest.GetString("ItemCode");
            string ItemInteger = CheckRequest.GetString("ItemInteger");
            string ItemDecimal = CheckRequest.GetString("ItemDecimal");
            string PlusOrMinus = CheckRequest.GetString("PlusOrMinus");
            string Units = CheckRequest.GetString("Units");

            string AlarmAreaMin = CheckRequest.GetString("AlarmAreaMin");
            string AlarmAreaMax = CheckRequest.GetString("AlarmAreaMax");
            string DisplayAreaMin = CheckRequest.GetString("DisplayAreaMin");
            string DisplayAreaMax = CheckRequest.GetString("DisplayAreaMax");

            YY_RTU_ITEM info = new YY_RTU_ITEM()
            {
                ItemID = ItemID,
                ItemName = ItemName,
                ItemCode = ItemCode,
                PlusOrMinus = !string.IsNullOrEmpty(PlusOrMinus) && PlusOrMinus == "1" ? true : false,
                ItemInteger = ItemInteger.ToInt(),
                ItemDecimal = ItemDecimal.ToInt(),
                Units = Units,
            };
            ElementsChartsInfo_Tab rel = null;
            if (!string.IsNullOrEmpty(AlarmAreaMin) && !string.IsNullOrEmpty(AlarmAreaMax)
                 && !string.IsNullOrEmpty(DisplayAreaMin) && !string.IsNullOrEmpty(DisplayAreaMin))
            {
                rel = new ElementsChartsInfo_Tab();
                rel.ItemID = info.ItemID;
                rel.AlarmAreaMax = AlarmAreaMax.ToDecimal();
                rel.AlarmAreaMin = AlarmAreaMin.ToDecimal();
                rel.DisplayAreaMax = DisplayAreaMax.ToDecimal();
                rel.DisplayAreaMin = DisplayAreaMin.ToDecimal();
            }

            var exist= ItemBLL.Instance.GetItem(ItemID);
            if (exist != null && exist.Count > 2)
            {
                return Content("{\"msg\":\"修改失败！,已存在相同元素编号\",\"success\":false}");
            }
      
            int state = ItemBLL.Instance.UpdateItem(info, rel);
            if (state > 0)
            {
                return Content("{\"msg\":\"更新成功！\",\"success\":true}");
            }
            else
            {
                return Content("{\"msg\":\"更新失败！\",\"success\":false}");
            }
        }

        public ContentResult GetItemType()
        {
            List<YY_ITEMTYPE>  temp= ItemBLL.Instance.GetItemType();
            string sb = "[";
            foreach (var item  in temp)
            {
                sb += "{\"ItemTypeID\":\"" + item.ItemTypeID + "\",\"ItemType\":\"" + item.ItemType + "\"},";
            }
            sb = sb.Trim(",".ToCharArray());
            sb += "]";
            return Content(sb.ToString());

        }

        public ContentResult GetItemType2()
        {

            List<YY_ITEMTYPE> temp = ItemBLL.Instance.GetItemType();
            
            return Content(temp.SerializeJSON());

        }


        public ContentResult DelItem() {
            string ids = CheckRequest.GetString("IDs");
            List<string> delIDs = ids.Split(',').ToArray().ToList();
            int state = ItemBLL.Instance.DelItem(delIDs);
            if (state>0)
            {
                return Content("{\"msg\":\"删除成功！\",\"success\":true}");
            }
            else
            {
                return Content("{\"msg\":\"删除失败！\",\"success\":false}");
            }
        }

        public ContentResult DelItemRel()
        {
            string ItemID = CheckRequest.GetString("ItemID");
            string ItemTypeID = CheckRequest.GetString("ItemTypeID");
            if (!string.IsNullOrEmpty(ItemID) && !string.IsNullOrEmpty(ItemTypeID))
            {
                int state = ItemBLL.Instance.DelItemRel(ItemID, ItemTypeID);
                if (state > 0)
                {
                    return Content("{\"msg\":\"删除成功！\",\"success\":true}");
                }
                else
                {
                    return Content("{\"msg\":\"删除失败！\",\"success\":false}");
                }
            }
            else
            {
                return Content("{\"msg\":\"数据传输失败！\",\"success\":false}");
            }
        }

        public ActionResult SetItemTypePage() {
            return View();
        }

        public ContentResult SetItemType()
        {
            string ItemID = CheckRequest.GetString("ItemID");
            string ItemTypeID = CheckRequest.GetString("ItemTypeID");
            string ItemIndex = CheckRequest.GetString("ItemIndex");
             
            int state = ItemBLL.Instance.AddItemRel(new YY_ITEM_TI(){
                           ItemID =ItemID, ItemTypeID= ItemTypeID ,
                           ItemIndex=string.IsNullOrEmpty(ItemIndex)? 0:ItemIndex.ToInt()
                           });
            if (state > 0)
            {
                return Content("{\"msg\":\"设置成功！\",\"data\":\"" + ItemID + "\",\"success\":true}");
            }
            else
            {
                return Content("{\"msg\":\"设置失败！\",\"success\":false}");
            }
        }
}
}