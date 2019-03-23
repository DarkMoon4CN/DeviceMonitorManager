using ChuanYe.BMS.BLL;
using ChuanYe.BMS.DAL.Table;
using ChuanYe.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class ButtonController : Web.Factory.BaseController
    {
        // GET: Button
        public ActionResult Index()
        {
            string mid = CheckRequest.GetString("menuId");//左侧菜单  Home/Index  92行
            ViewBag.MenuId = mid;
            return View();
        }


        public JsonResult GetAuthPageButton()
        {
             ResultDto<dynamic> result = new ResultDto<dynamic>();
            try
            {
                var sessionRes = base.GetSessionUser();
                int roleId = sessionRes.SecondParam.RoleId;
                string mid = CheckRequest.GetString("menuId");//左侧菜单  Home/Index  92行
                int menuId = string.IsNullOrEmpty(mid) ? 0 : mid.ToInt();
                var rmbList = B_TbButtonBLL.Instance.RoleMenuButtonList(roleId, menuId);
                result.FirstParam = rmbList;
                result.Status = 1;
                result.Message = "Requst Is Success";
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = "Server Error!!";
                LoggerHelper.Info("ButtonController/GetAuthPageButton :" + ex);
            }
            return Json(result);
        }

        public ContentResult GetAllButtonInfo()
        {
            string sort = CheckRequest.GetString("sort");
            string order = CheckRequest.GetString("order");
            string name = CheckRequest.GetString("name");
            if (string.IsNullOrEmpty(sort))
            {
                sort = "sort";
            }
            if (string.IsNullOrEmpty(order))
            {
                order = "asc";
            }

            int totalCount = 0;
            var page = B_TbButtonBLL.Instance.ButtonByPage(null, null, name, ref totalCount, sort, order);
            var pageJson = page.SerializeJSON();

            return Content("{\"total\": " + totalCount.ToString() + ",\"rows\":" + pageJson + "}");
        }


        #region 增加
        public ActionResult AddPage()
        {
            return View();
        }

        public ContentResult AddButton()
        {
            var sessionRes = base.GetSessionUser();
            string name = CheckRequest.GetString("Name");
            string code = CheckRequest.GetString("Code");
            string icon = CheckRequest.GetString("Icon");
            string sort = CheckRequest.GetString("Sort");
            string des = CheckRequest.GetString("Description");
            string accountName = sessionRes.FirstParam.AccountName;

            B_TbButton entity = new B_TbButton()
            {
                Name = name,
                Code = code,
                Creater = accountName,
                CreateTime = DateTime.Now,
                Icon = icon,
                Updater = accountName,
                UpdateTime = DateTime.Now,
                Description = des,
                Sort = sort.ToInt()
            };

            int buttonId = B_TbButtonBLL.Instance.AddButton(entity);
            if (buttonId > 0)
            {
                return Content("{\"msg\":\"添加成功！\",\"success\":true}");
            }
            else
            {
                return Content("{\"msg\":\"添加失败！\",\"success\":false}");
            }
        }

        #endregion

        #region 删除
        public ContentResult DelButton()
        {
            string ids = CheckRequest.GetString("IDs");
            List<int> delIDs = ids.Split(',').ToArray().Select<string, int>(s => Convert.ToInt32(s)).ToList();
            if (SundryHelper.IsNumericArray(ids.Split(',')))
            {
                int result = B_TbButtonBLL.Instance.DelButton(delIDs);
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

        #region 编辑
        public ActionResult EditPage()
        {
            return View();
        }

        public ContentResult EditButton()
        {
            var sessionRes = base.GetSessionUser();
            string id = CheckRequest.GetString("Id");
            string name = CheckRequest.GetString("Name");
            string code = CheckRequest.GetString("Code");
            string icon = CheckRequest.GetString("Icon");
            string sort = CheckRequest.GetString("Sort");
            string des = CheckRequest.GetString("Description");
           
            B_TbButton entity = new B_TbButton()
            {
                ID = id.ToInt(),
                Name = name,
                Code = code,
                Icon=icon,
                Updater = sessionRes.FirstParam.AccountName,
                UpdateTime = DateTime.Now,
                Sort = sort.ToInt(),
                Description = des
            };
            var exist = B_TbButtonBLL.Instance.GetButtonByExist(name);
            if (exist != null && exist.Count>2)
            {
                return Content("{\"msg\":\"修改失败,按钮名称已存在！\",\"success\":false}");
            }
            else
            {
                int result = B_TbButtonBLL.Instance.UpdateButton(entity);
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

    }
}