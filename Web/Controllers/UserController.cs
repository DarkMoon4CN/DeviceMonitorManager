using ChuanYe.BMS.BLL;
using ChuanYe.BMS.DAL.Entity;
using ChuanYe.BMS.DAL.Table;
using ChuanYe.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class UserController : Factory.BaseController
    {
        // GET: User
        public ActionResult Index()
        {
            string mid = CheckRequest.GetString("menuId");//左侧菜单  Home/Index  92行
            ViewBag.MenuId = mid;
            return View();
        }

        public ContentResult GetUserList()
        {
            string page = CheckRequest.GetString("page");
            string rows = CheckRequest.GetString("rows");
            string isableStr = CheckRequest.GetString("isable");
            string accountName = CheckRequest.GetString("accountname");
            string username = CheckRequest.GetString("username");
            string adddatestart = CheckRequest.GetString("adddatestart");
            string adddateend = CheckRequest.GetString("adddateend");

            DateTime? startTime = string.IsNullOrEmpty(adddatestart) == true ? new Nullable<DateTime>() : adddatestart.ToDateTime();
            DateTime? endTime = string.IsNullOrEmpty(adddateend) == true ? new Nullable<DateTime>() : adddateend.ToDateTime();
            int isable = string.IsNullOrEmpty(isableStr) == true ? -1 : isableStr.ToInt();
            int pageIndex = string.IsNullOrEmpty(page) == true ? 1 : page.ToInt();
            int pageSize = string.IsNullOrEmpty(rows) == true ? 20 : rows.ToInt();

            B_TbUser entity = new B_TbUser() {
                AccountName = accountName,
                RealName = username,
            };
            int totalCount = 0;
            List<UserRoleEntity> result = B_TbUserBLL.Instance.UsersByPage(entity, startTime, endTime, isable, ref totalCount, pageSize, pageIndex);
            var pageJson = result.SerializeJSON();
            return Content("{\"total\": " + totalCount.ToString() + ",\"rows\":" + pageJson + "}");
        }

        public ActionResult AddPage()
        {
            return View();
        }
        public ContentResult AddUser()
        {
            try
            {
                var sessionRes = base.GetSessionUser();
                string accountName = CheckRequest.GetString("UserID");
                string userName = CheckRequest.GetString("UserName");
                string isable = CheckRequest.GetString("Isable");
                string ifchangepwd = CheckRequest.GetString("IfChangepwd");
                string description = CheckRequest.GetString("description");
                string mobilePhone = CheckRequest.GetString("MobilePhone");
                string post = CheckRequest.GetString("Post");
                string workNumber = CheckRequest.GetString("WorkNumber");
                string email = CheckRequest.GetString("Email");
                string password = SundryHelper.MD5("123456");

                B_TbUser entity = new B_TbUser() {
                    ID = 0,
                    AccountName = accountName,
                    RealName = userName,
                    IsAble = string.IsNullOrEmpty(isable) ? false : bool.Parse(isable),
                    IfChangePwd = string.IsNullOrEmpty(ifchangepwd) ? false : bool.Parse(ifchangepwd),
                    Description = description,
                    MobilePhone = mobilePhone,
                    Position = post,
                    WorkNumber = workNumber,
                    Password = password,
                    Email = email,
                    Creater = sessionRes.FirstParam.AccountName,
                    CreateTime = DateTime.Now,
                    Updater = sessionRes.FirstParam.AccountName,
                    UpdateTime = DateTime.Now
                };

                int userId = B_TbUserBLL.Instance.AddUser(entity);
                if (userId > 0)
                {
                    return Content("{\"msg\":\"添加成功！默认密码是【123456】！\",\"success\":true}");
                }
                else
                {
                    return Content("{\"msg\":\"添加失败！\",\"success\":false}");
                }
            }
            catch (Exception ex)
            {
                return Content("{\"msg\":\"添加失败," + ex.Message + "\",\"success\":false}");
            }
        }

        public ActionResult DelUser()
        {
            try
            {
                string ids = CheckRequest.GetString("ids");
                if (SundryHelper.IsNumericArray(ids.Split(',')))
                {
                    List<int> delIDs = ids.Split(',').ToArray().Select<string, int>(s => Convert.ToInt32(s)).ToList();

                    //清理角色关系
                    B_TbUserBLL.Instance.DelUserRoleByUserIDs(delIDs);
                    int result = B_TbUserBLL.Instance.DelUser(delIDs);

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
            catch (Exception ex)
            {
                return Content("{\"msg\":\"删除失败," + ex.Message + "\",\"success\":false}");
            }
        }


        public ActionResult EditPage() {
            return View();
        }

        public ContentResult EditUser() {
            try
            {
                var sessionRes = base.GetSessionUser();
                string accountName = CheckRequest.GetString("UserID");
                string userName = CheckRequest.GetString("UserName");
                string isable = CheckRequest.GetString("Isable");
                string ifchangepwd = CheckRequest.GetString("IfChangepwd");
                string description = CheckRequest.GetString("description");
                string mobilePhone = CheckRequest.GetString("MobilePhone");
                string post = CheckRequest.GetString("Post");
                string email = CheckRequest.GetString("Email");
                int id =Convert.ToInt32(CheckRequest.GetString("id"));
                B_TbUser entity = new B_TbUser()
                {
                    ID = id,
                    AccountName = accountName,
                    RealName = userName,
                    IsAble = string.IsNullOrEmpty(isable) ? false : bool.Parse(isable),
                    IfChangePwd = string.IsNullOrEmpty(ifchangepwd) ? false : bool.Parse(ifchangepwd),
                    Description = description,
                    MobilePhone = mobilePhone,
                    Position = post,
                    Email = email,
                    Updater = sessionRes.FirstParam.AccountName,
                    UpdateTime = DateTime.Now
                };
                int updateRow = B_TbUserBLL.Instance.UpdateUser(entity);
                if (updateRow>0)
                {
                    return Content("{\"msg\":\"修改成功！\",\"success\":true}");
                }
                else
                {
                    return Content("{\"msg\":\"修改失败！\",\"success\":true}");
                }

            }
            catch (Exception ex)
            {
                return Content("{\"msg\":\"修改失败," + ex.Message + "\",\"success\":false}");
            }
        }


        public ActionResult SetRolePage()
        {
            return View();
        }


        public ContentResult GetAllRole()
        {
            int totalCount = 0;
            List<B_TbRole> result=B_TbRoleBLL.Instance.RoleByPage(null, null, null, ref totalCount, "ID", "ASC", 1, int.MaxValue);
            string json = result.SerializeJSON();
            return Content(json);
        }

        public ContentResult SetRole()
        {
            string userIds = CheckRequest.GetString("UserIDs");
            string roleID = CheckRequest.GetString("RoleIDs");
            try
            {
                int rid = roleID.ToInt();
                List<int> uids = userIds.Split(',').ToArray().Select<string, int>(s => Convert.ToInt32(s)).ToList();

                //清理旧角色
                B_TbUserBLL.Instance.DelUserRoleByUserIDs(uids);
                for (int i = 0; i < uids.Count; i++)
                {
                    B_TbUserBLL.Instance.AddUserRole(new B_TbUserRole()
                    {
                        UserId = uids[i],
                        RoleId = rid
                    });
                }
                return Content("{\"msg\":\"设置成功！\",\"success\":true}");
            }
            catch
            {
                return Content("{\"msg\":\"设置失败！\",\"success\":true}");
            }
        }

    }
}