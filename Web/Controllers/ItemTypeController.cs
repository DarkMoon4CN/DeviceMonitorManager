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
    public class ItemTypeController : Web.Factory.BaseController
    {
        // GET: ItemType
        public ActionResult Index()
        {
            string mid = CheckRequest.GetString("menuId");//左侧菜单  Home/Index  92行
            ViewBag.MenuId = mid;
            return View();
        }

        public ContentResult GetItemTypeList()
        {
            string itemTypeName = CheckRequest.GetString("ItemType");
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
            var result = ItemTypeBLL.Instance.ItemTypeByPage(itemTypeName, pager);
            var pageJson = result.SerializeJSON();
            return Content("{\"total\": " + pager.TotalRows.ToString() + ",\"rows\":" + pageJson + "}");
        }

        public ActionResult AddPage() {
            return View();
        }

        public ContentResult AddItemType()
        {
            string ItemType = CheckRequest.GetString("ItemType");
            string ItemTypeIndex = CheckRequest.GetString("ItemTypeIndex");

            YY_ITEMTYPE info = new YY_ITEMTYPE()
            {
                ItemTypeID = System.Guid.NewGuid().ToString(),
                ItemType = ItemType,
                ItemTypeIndex = string.IsNullOrEmpty(ItemTypeIndex) ? 1 : ItemTypeIndex.ToInt()
            };
            var exist = ItemTypeBLL.Instance.GetItemTypeByName(info.ItemType);
            if (exist.Count >= 1)
            {
                return Content("{\"msg\":\"已有相同的分类名称！\",\"success\":false}");
            }
            int state = ItemTypeBLL.Instance.AddItemType(info);
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

        public ContentResult EditItemType()
        {
            string ItemTypeID = CheckRequest.GetString("ItemTypeID");
            string ItemType = CheckRequest.GetString("ItemType");
            string ItemTypeIndex = CheckRequest.GetString("ItemTypeIndex");
            YY_ITEMTYPE info = new YY_ITEMTYPE()
            {
                ItemTypeID = ItemTypeID,
                ItemType = ItemType,
                ItemTypeIndex = string.IsNullOrEmpty(ItemTypeIndex) ? 1 : ItemTypeIndex.ToInt()
            };

            var exist = ItemTypeBLL.Instance.GetItemTypeByName(info.ItemType);
            if (exist.Count >= 2)
            {
                return Content("{\"msg\":\"已有相同的分类名称！\",\"success\":false}");
            }
            int state = ItemTypeBLL.Instance.UpdateItemType(info);
            if (state > 0)
            {
                return Content("{\"msg\":\"修改成功！\",\"success\":true}");
            }
            else
            {
                return Content("{\"msg\":\"修改失败！\",\"success\":false}");
            }
        }

        public ContentResult DelItemType()
        {
            string ids = CheckRequest.GetString("IDs");
            List<string> delIDs = ids.Split(',').ToArray().ToList();
            bool result = ItemTypeBLL.Instance.DelItemType(delIDs);
            if (result)
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