using ChuanYe.Utils;
using MonitorManager.Api.Factory;
using MonitorManager.BLL;
using MonitorManager.Model;
using MonitorManager.Models;
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
    public class HelloWorldController : ApiController
    {
        public ResultDto<dynamic,dynamic, dynamic> ApiAuth(ApiAuthReq obj)
        {
            ResultDto<dynamic, dynamic, dynamic> result = new ResultDto<dynamic, dynamic, dynamic>();
            try
            {
                //查询数据库中的 AppSecret
                if (SundryHelper.GetCache<List<B_AppAuthor>>("appSecret") == null)
                {
                    List<B_AppAuthor> authorList = AppAuthorBLL.Instance.GetAll();
                    SundryHelper.SetCache<List<B_AppAuthor>>("appSecret", authorList);
                }
                List<B_AppAuthor> appModel = SundryHelper.GetCache<List<B_AppAuthor>>("appSecret");
                var appAuthor = appModel.Where(p => obj.AppSecret==p.AppSecret).FirstOrDefault();
                if (appAuthor == null)
                {
                    result.Status = 4031;
                    result.Message = "数据库中没有相应的 appSecret,然而API送你一个： " + appModel.FirstOrDefault().AppSecret;
                    return result;
                }
                string allString = obj.AppSecret + "|" + obj.Key + "|" + obj.Data + "|" + obj.TimeStamp;
                string token=SundryHelper.MD5(allString);

                string tips = string.Empty;
                #region tips
                if (!string.IsNullOrEmpty(obj.AppSecret))
                {
                    tips += "AppSecret|";
                }
                else {
                    tips += "|";
                }
                if (!string.IsNullOrEmpty(obj.Key))
                {
                    tips += "Key|";
                }
                else
                {
                    tips += "|";
                }

                if (!string.IsNullOrEmpty(obj.Data))
                {
                    tips += "Data|";
                }
                else
                {
                    tips += "|";
                }

                if (!string.IsNullOrEmpty(obj.TimeStamp))
                {
                    tips += "TimeStamp";
                }
                #endregion
                result.FirstParam = tips;
                result.SecondParam =allString;
                result.ThirdParam = token;
                result.Status = 1;
                result.Message = "validate complete";
                return result;

            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = "/HelloWorld/TestApiAuth Server Error";
                ChuanYe.Utils.LoggerHelper.Error(ex.Message);
            }
            return result;
        }
     }
}
