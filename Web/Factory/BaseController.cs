using ChuanYe.BMS.DAL.Entity;
using ChuanYe.BMS.DAL.Table;
using ChuanYe.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Factory
{
    public class BaseController : Controller
    {

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

            if (controllerName.ToLower() != "login" && controllerName.ToLower()!="home")
            {
                var sessionRes=GetSessionUser();
                if (sessionRes==null)
                {
                    filterContext.Result = RedirectToRoute(new { Controller = "Login", Action = "Index" });
                }
            }
            base.OnActionExecuting(filterContext);
        }

        public void SetSessionUser(ResultDto<B_TbUser, UserRoleEntity> entity)
        {
            Session["user"] = entity;
        }

        public ResultDto<B_TbUser,UserRoleEntity> GetSessionUser()
        {
            ResultDto<B_TbUser, UserRoleEntity> result = new ResultDto<B_TbUser, UserRoleEntity>();
            try
            {
               result = (ResultDto<B_TbUser, UserRoleEntity>)Session["user"];
            }
            catch (Exception ex)
            {
                result.Status = 0;
                result.Message = "session is null ！";
                LoggerHelper.Info(ex);
            }
            return result;
        }

        public void ClearUserSession()
        {
            SundryHelper.WriteCookie("UserID", string.Empty, -1);
            Session["user"] = null;
            Session.Remove("user");
            Session.Abandon();
        }

        
    }
}