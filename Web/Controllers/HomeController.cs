using ChuanYe.BMS.BLL;
using ChuanYe.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Web.Models;
using ChuanYe.BMS.DAL.Table;
using ChuanYe.BMS.DAL.Entity;

namespace Web.Controllers
{
    public class HomeController : Web.Factory.BaseController
    {
        // GET: Home
        public ActionResult Index()
        {
            string id= SundryHelper.GetCookie("UserID");
            int userId = string.IsNullOrEmpty(id) || !CheckRequest.IsNumber(id) ? 0 : id.ToInt();
            var  user= B_TbUserBLL.Instance.UserDetail(userId);
            if (user == null)
            {
                return RedirectToAction("Index", "Login");
            }

            #region 用户session 相关设置
            var userRole=B_TbUserBLL.Instance.UserRoleList(user.ID);
            ResultDto<B_TbUser, UserRoleEntity> userEnityRes = new ResultDto<B_TbUser,UserRoleEntity>();
            userEnityRes.Status = 1;
            userEnityRes.FirstParam = user;
            userEnityRes.SecondParam = userRole;
            base.SetSessionUser(userEnityRes);
            #endregion

            ViewBag.RealName = user.RealName;
            ViewBag.TimeView = DateTime.Now.ToLongDateString();
            ViewBag.DayDate = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek);
            return View();
        }

        public JsonResult GetTreeByEasyui()
        {
            var sessionRes= base.GetSessionUser();
            ResultDto<dynamic> result = new ResultDto<dynamic>();
            string pid = CheckRequest.GetString("parentId");
            int parentId = string.IsNullOrEmpty(pid) ? 0 : pid.ToInt();
            if (sessionRes!=null &&sessionRes.Status!=0)
            {
                var list=B_TbUserBLL.Instance.UserRoleMenuList2(sessionRes.SecondParam.RoleId, parentId);
                result.FirstParam = list;
                result.Status = 1;
                result.Message = " Requst Is Success ";
                
            }
            else
            {
                result.Status = sessionRes.Status;
                result.Message = sessionRes.Message;
            }
            return Json(result);
        }


        public ActionResult LogOut()
        {
            base.ClearUserSession();
            return RedirectToRoute(new { Controller = "Login", Action = "Index" });
        }
    }
}