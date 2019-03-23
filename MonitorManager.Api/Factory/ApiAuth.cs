
using ChuanYe.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Data;
using System.IO;
using MonitorManager.Model;
using MonitorManager.Models;
using MonitorManager.BLL;

namespace MonitorManager.Api.Factory
{
    /// <summary>
    ///   Auth
    /// </summary>
    public class ApiAuth : ActionFilterAttribute
    {
        /// <summary>
        /// 拦截器
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            //查找数据库中的所有密钥数据并缓存
            if (SundryHelper.GetCache<List<B_AppAuthor>>("appSecret") == null)
            {
                List<B_AppAuthor> authorList = AppAuthorBLL.Instance.GetAll();
                SundryHelper.SetCache<List<B_AppAuthor>>("appSecret", authorList);
            }
            ResultDtoBase result = new ResultDtoBase();
            try
            {
                
                //tk = actionContext.ActionArguments["obj"];
                var request = actionContext.Request;
                string timeStamp = HttpUtility.UrlDecode(request.Headers.GetValues("timestamp").FirstOrDefault());
                string appSecret = HttpUtility.UrlDecode(request.Headers.GetValues("appSecret").FirstOrDefault());
                string key = HttpUtility.UrlDecode(request.Headers.GetValues("key").FirstOrDefault());
                string clientToken = HttpUtility.UrlDecode(request.Headers.GetValues("token").FirstOrDefault());
                string data = string.Empty;

                //验证签名 
                //1.查找并验证AppSecret 是否存在
                List<B_AppAuthor> appModel = SundryHelper.GetCache<List<B_AppAuthor>>("appSecret");
                var appAuthor = appModel.Where(p => appSecret == p.AppSecret).FirstOrDefault();
                if (appAuthor == null)
                {
                    result.Status = 4031;
                    result.Message = "AppSecret 无效";
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, result.SerializeJSON());
                    return;
                }


                //2.获取数据json
                Stream stream = HttpContext.Current.Request.InputStream;
                string responseJson = string.Empty;
                StreamReader streamReader = new StreamReader(stream);
                data = streamReader.ReadToEnd();

                //3.验证安全
                string serverToken = GetAccessToken(appSecret,key,data,timeStamp);
                if (clientToken.ToLower() != serverToken.ToLower())
                {
                    result.Status = 4032;
                    result.Message = "安全验证失败，服务器Token:"+ serverToken.ToLower();
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, result.SerializeJSON());
                    return;
                }
            }
            catch (Exception ex)
            {
                result.Status = 4033;
                result.Message = "安全验证失败，缺少必要的参数";
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, result.SerializeJSON());
                return;
            }
            base.OnActionExecuting(actionContext);
        }

      
        private string GetAccessToken(string appSecret,string key, string data, string timeStamp)
        {
            string token = appSecret + "|" + key + "|" + data + "|" + timeStamp;
            return SundryHelper.MD5(token); 
        }
    }
}