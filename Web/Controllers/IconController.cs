using ChuanYe.BMS.BLL;
using ChuanYe.BMS.DAL.Table;
using ChuanYe.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class IconController  : Web.Factory.BaseController
    {
        // GET: Icon
        public ActionResult Index()
        {
            string mid = CheckRequest.GetString("menuId");//左侧菜单  Home/Index  92行
            ViewBag.MenuId = mid;
            return View();
        }

        #region 增加
        public ActionResult AddPage()
        {
            return View();
        }


        public ContentResult AddIcon()
        {
            try
            {
                var sessionRes = base.GetSessionUser();
                string iconName = CheckRequest.GetString("IconName");
                string iconCssInfo = CheckRequest.GetString("IconCssInfo");
                B_TbIcon entity = new B_TbIcon();
                entity.IconCssInfo = iconCssInfo;
                entity.IconName = iconName;
                entity.Creater = sessionRes.FirstParam.AccountName;
                entity.CreateTime = DateTime.Now;
                entity.Updater = entity.Creater;
                entity.UpdateTime = entity.CreateTime;
                bool exists = B_TbIconBLL.Instance.IconDetail(iconName) != null ? true : false;
                if (exists)
                {
                    return Content("{\"msg\":\"添加失败,图标名称已存在！\",\"success\":false}");
                }
                else
                {
                    int iconID = B_TbIconBLL.Instance.AddIcon(entity);
                    if (iconID > 0)
                    {
                        return Content("{\"msg\":\"添加成功！\",\"success\":true}");
                    }
                    else
                    {
                        return Content("{\"msg\":\"添加失败！\",\"success\":false}");
                    }
                }
            }
            catch (Exception ex)
            {
                return Content("{\"msg\":\"添加失败," + ex.Message + "\",\"success\":false}");
            }
        }


        #endregion

        #region 编辑

        public ActionResult EditPage()
        {
            return View();
        }


        public ContentResult EditIcon()
        {
            var sessionRes = base.GetSessionUser();
            string id = CheckRequest.GetString("Id");
            string iconName = CheckRequest.GetString("IconName");
            string iconCssInfo = CheckRequest.GetString("IconCssInfo");
            B_TbIcon entity = new B_TbIcon()
            {
                ID = id.ToInt(),
                IconName = iconName,
                IconCssInfo = iconCssInfo,
                Updater = sessionRes.FirstParam.AccountName,
                UpdateTime = DateTime.Now
            };
            if (B_TbIconBLL.Instance.IconDetail(iconName) != null)
            {
                return Content("{\"msg\":\"修改失败,图标名称已存在！\",\"success\":false}");
            }
            else
            {
                int result = B_TbIconBLL.Instance.UpdateIcon(entity);
                if (result > 0)
                {
                    return Content("{\"msg\":\"修改成功！\",\"success\":true}");
                }
                else
                {
                    return Content("{\"msg\":\"修改失败！\",\"success\":false}");
                }
            }
        }


        #endregion

        #region 删除
        public ContentResult DelIcon()
        {
            string ids = CheckRequest.GetString("IDs");
            List<int> delIDs = ids.Split(',').ToArray().Select<string, int>(s => Convert.ToInt32(s)).ToList();
            if (SundryHelper.IsNumericArray(ids.Split(',')))
            {
                int result = B_TbIconBLL.Instance.DelIcon(delIDs);
                if (result > 0)
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
        #endregion

        public ContentResult GetIconList()
        {
            string sort = CheckRequest.GetString("Sort");
            string order = CheckRequest.GetString("Order");
            string name = CheckRequest.GetString("IconName");
            if (string.IsNullOrEmpty(sort))
            {
                sort = "sort";
            }
            if (string.IsNullOrEmpty(order))
            {
                order = "asc";
            }
            int totalCount = 0;
            List<B_TbIcon> page = B_TbIconBLL.Instance.IconByPage(null, null, name, ref totalCount, sort, order);
            var pageJson = page.SerializeJSON();
            return Content("{\"total\": " + totalCount.ToString() + ",\"rows\":" + pageJson + "}");
        }

        public ActionResult GetLocalImgInfo()
        {
            FileInfo[] fs = (new DirectoryInfo(Server.MapPath("~/Content/themes/icon"))).GetFiles();
            string sb = "[";
            foreach (FileInfo file in fs)
            {
                string iconName = "icon-" + Path.GetFileNameWithoutExtension(file.Name);
                sb += "{\"id\":\"" + iconName + "\",\"text\":\"" + iconName + "\",\"iconCls\":\"" + iconName + "\"},";
            }
            sb = sb.Trim(",".ToCharArray());
            sb += "]";
            return Content(sb.ToString());
        }
    }
}