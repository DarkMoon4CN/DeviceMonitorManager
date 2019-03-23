using ChuanYe.BMS.BLL;
using ChuanYe.BMS.DAL.Table;
using ChuanYe.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Factory;

namespace Web.Controllers
{
    public class MenuController : BaseController
    {
        // GET: Menu
        public ActionResult Index()
        {
            string mid = CheckRequest.GetString("menuId");//左侧菜单  Home/Index  92行
            ViewBag.MenuId = mid;
            return View();
        }

        public ContentResult GetMenuList()
        {
            string sort = CheckRequest.GetString("Sort");
            string order = CheckRequest.GetString("Order");
            string name = CheckRequest.GetString("Name");
            if (string.IsNullOrEmpty(sort))
            {
                sort = "sort";
            }
            if (string.IsNullOrEmpty(order))
            {
                order = "asc";
            }
            int totalCount = 0;
            List<B_TbMenu> page = B_TbMenuBLL.Instance.MenuByPage(null, null, name, ref totalCount, sort, order);
            var pageJson = page.SerializeJSON();
            return Content("{\"total\": " + totalCount.ToString() + ",\"rows\":" + pageJson + "}");
        }

        public ContentResult GetAllButton()
        {
            int totalCount = 0;
            var page = B_TbButtonBLL.Instance.ButtonByPage(null, null, null, ref totalCount,"UpdateTime", "asc", int.MaxValue,1);
            List<dynamic> keyValue = new List<dynamic>();
            foreach (var item in page)
            {
                keyValue.Add(new { id = item.ID, text = item.Name });
            }
            var result = new { id = 0, text = "全选", children = keyValue };
            return Content("["+result.SerializeJSON()+"]");
        }

        #region 增加
        public ActionResult AddPage()
        {
            return View();
        }

        public ContentResult AddMenu()
        {
            var sessionRes = base.GetSessionUser();
            string name = CheckRequest.GetString("Name");
            string code = CheckRequest.GetString("Code");
            string icon = CheckRequest.GetString("Icon");
            string sort = CheckRequest.GetString("Sort");
            string menuType = CheckRequest.GetString("menuType");
            string linkAddr = CheckRequest.GetString("LinkAddress");
            int pid = 1;

            string accountName = sessionRes.FirstParam.AccountName;

            B_TbMenu entity = new B_TbMenu()
            {
                Name = name,
                Code = code,
                Creater = accountName,
                CreateTime = DateTime.Now,
                Icon = icon,
                Updater = accountName,
                UpdateTime = DateTime.Now,
                LinkAddress = linkAddr,
                MenuType = menuType.ToInt(),
                ParentId = pid.ToInt(),
                Sort = sort.ToInt()
            };
            int menuId = B_TbMenuBLL.Instance.AddMenu(entity);

            //默认给自己角色赋予菜单权限
            int roleMenuId=B_TbRoleBLL.Instance.AddRoleMenu(new B_TbRoleMenu() {
                   RoleId = sessionRes.SecondParam.RoleId, MenuId = menuId, Updater = entity.Updater, UpdateTime = entity.UpdateTime });
           if (menuId > 0)
            {
                return Content("{\"msg\":\"添加成功！\",\"success\":true}");
            }
            else
            {
                return Content("{\"msg\":\"添加失败！\",\"success\":false}");
            }
        }

        #endregion

        #region 编辑

        public ActionResult EditPage()
        {
            return View();
        }


        public ContentResult EditMenu()
        {
            var sessionRes = base.GetSessionUser();
            string id= CheckRequest.GetString("ID");
            string name = CheckRequest.GetString("Name");
            string code = CheckRequest.GetString("Code");
            string icon = CheckRequest.GetString("Icon");
            string sort = CheckRequest.GetString("Sort");
            string linkAddr = CheckRequest.GetString("LinkAddress");
            int pid = 1;

            string accountName = sessionRes.FirstParam.AccountName;

            B_TbMenu entity = new B_TbMenu()
            {
                ID=id.ToInt(),
                Name = name,
                Code = code,
                Creater = accountName,
                CreateTime = DateTime.Now,
                Icon = icon,
                Updater = accountName,
                UpdateTime = DateTime.Now,
                LinkAddress = linkAddr,
                ParentId = pid.ToInt(),
                Sort = sort.ToInt()
            };

            var exist = B_TbMenuBLL.Instance.GetMenu(name);
            if (exist.Count >= 2)
            {
                return Content("{\"msg\":\"修改失败,菜单名名称已存在！\",\"success\":false}");
            }
            else
            {
                int result = B_TbMenuBLL.Instance.UpdateMenu(entity);
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
        public ContentResult DelMenu()
        {
            string ids = CheckRequest.GetString("IDs");
            List<int> delIDs = ids.Split(',').ToArray().Select<string, int>(s => Convert.ToInt32(s)).ToList();
            if (SundryHelper.IsNumericArray(ids.Split(',')))
            {
                B_TbMenuBLL.Instance.RemoveMenuButtonByMids(delIDs);
                int result = B_TbMenuBLL.Instance.DelMenu(delIDs);
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

        #region 分配按钮

        public ActionResult SetMenuButtonPage()
        {
            return View();
        }

        public ContentResult SetMenuButton()
        {
            try {
                var sessionRes = base.GetSessionUser();
                string mid = CheckRequest.GetString("MID");
                string bids = CheckRequest.GetString("BIDs");
                List<int> btempId = bids.Split(',').ToArray().Select<string, int>(s => Convert.ToInt32(s)).ToList();
                List<B_TbButton> buttons = B_TbButtonBLL.Instance.GetButtonByIDs(btempId);
                B_TbMenuBLL.Instance.RemoveMenuButtonByMid(mid.ToInt());
                foreach (var item in buttons)
                {
                    B_TbMenuBLL.Instance.AddMenuButton(
                         new B_TbMenuButton() { MenuId = mid.ToInt(), ButtonId = item.ID, Updater = sessionRes.FirstParam.AccountName, UpdateTime = DateTime.Now });
                }
                return Content("{\"msg\":\"分配按钮成功！\",\"success\":true}");
            }
            catch {
                return Content("{\"msg\":\"分配按钮失败！\",\"success\":false}");
            }
        }
        public ContentResult GetMenuButtonByMenuID()
        {
            string mid = CheckRequest.GetString("MID");  //菜单id
            List<int> buttonIDs = B_TbMenuBLL.Instance.GetMenuButtonByMid(mid.ToInt()).Select(s=>s.ButtonId).ToList();
            string buttonIdsStr = string.Join(",", buttonIDs);
            return Content(buttonIdsStr);
        }
        #endregion
    }
}