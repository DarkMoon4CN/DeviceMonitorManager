using ChuanYe.BMS.BLL;
using ChuanYe.BMS.DAL.Table;
using ChuanYe.Utils;
using MonitorManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Web.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult CheckUserLogin()
        {
            var result = new ResultDtoBase();
            string serverCode = (string)Session["LoginValidateCode"];
            string clientCode=CheckRequest.GetString("code");
            if (clientCode!="0" &&(serverCode != clientCode && serverCode != null))
            {
                result.Status = 0;
                result.Message = "验证码不正确！";
            }
            else
            {
                string accountName = CheckRequest.GetString("accountName");
                string pwd = CheckRequest.GetString("password");
                string password = SundryHelper.MD5(pwd);
                B_TbUser temp = B_TbUserBLL.Instance.Login(accountName, password);
                if (temp == null)
                {
                    result.Status = 0;
                    result.Message = "账户或密码验证错误！";
                }
                else if (temp.IsAble == false)
                {
                    result.Status = 0;
                    result.Message = "用户已被禁用！";
                }
                else
                {
                    SundryHelper.WriteCookie("UserID", temp.ID.ToString(),480);
                    result.Status = 1;
                    result.Message = "登录成功！";
                }
            }
            return Json(result);
        }

        public JsonResult UserLoginOut()
        {
            var result = new ResultDtoBase();
            //清空cookie
            SundryHelper.WriteCookie("UserID",string.Empty, -1);
            result.Status = 1;
            result.Message = "退出成功!";
            return Json(result);
        }

        public ActionResult GetValidatorGraphics()
        {
            ValidateCodeImage vCode = new ValidateCodeImage();
            string code = vCode.RndNum(4);
            Session["LoginValidateCode"] = code;
            byte[] bytes = vCode.CreateValidateGraphic(code);
            return File(bytes, @"image/jpeg");
        }
    }
}