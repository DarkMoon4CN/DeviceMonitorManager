using ChuanYe.BMS.BLL;
using ChuanYe.BMS.DAL.Table;
using MonitorManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace MonitorManager.Api.Controllers
{
    [EnableCors("*", "*", "*")]
    public class UserController : ApiController
    {
        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultDto<dynamic> Login(LoginRequest obj)
        {
            ResultDto<dynamic> result = new ResultDto<dynamic>();
            try
            {
                string accountName = obj.AccountName;
                string pwd = obj.Password;
                B_TbUser data = B_TbUserBLL.Instance.Login(accountName, pwd);
                if (data == null)
                {
                    result.Status = 0;
                    result.Message = "AccountName Or Password is Error";
                }
                else if (data.IsAble == false)
                {
                    result.Status = 0;
                    result.Message = "User Status is Disable";
                }
                else
                {
                    result.Status = 1;
                    result.Message = "Requst Is Success";
                    data.Password = string.Empty;
                    result.FirstParam = data;
                }
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = "/User/Login Server Error";
                ChuanYe.Utils.LoggerHelper.Error(ex.Message);
            }
            return result;
        }
    }
}
