using ChuanYe.BMS.BLL;
using ChuanYe.BMS.DAL;
using ChuanYe.BMS.DAL.Entity;
using ChuanYe.BMS.DAL.Table;
using ChuanYe.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace Web.Controllers
{
    public class RoleController : Factory.BaseController
    {

        // GET: Role
        public ActionResult Index()
        {
            string mid = CheckRequest.GetString("menuId");//左侧菜单  Home/Index  92行
            ViewBag.MenuId = mid;
            return View();
        }

        public ContentResult GetRoleList()
        {
            string sort = CheckRequest.GetString("sort");
            string order = CheckRequest.GetString("order");
            string page = CheckRequest.GetString("page");
            string rows = CheckRequest.GetString("rows");
            string name = CheckRequest.GetString("roleName");
            if (string.IsNullOrEmpty(sort))
            {
                sort = "sort";
            }
            if (string.IsNullOrEmpty(order))
            {
                order = "asc";
            }
            int pageIndex = string.IsNullOrEmpty(page) == true ? 1 : page.ToInt();
            int pageSize = string.IsNullOrEmpty(rows) == true ? 20 : rows.ToInt();

            int totalCount = 0;
            var result = B_TbRoleBLL.Instance.RoleByPage(null, null, name, ref totalCount, sort, order, pageIndex, pageSize);
            var json = result.SerializeJSON();

            return Content("{\"total\": " + totalCount.ToString() + ",\"rows\":" + json + "}");
        }

        public ContentResult GetRoleUserList()
        {
            string rid = CheckRequest.GetString("roleId");
            int totalCount = 0;
            var page = B_TbUserBLL.Instance.UserListByRoleId(null, null, rid.ToInt(), ref totalCount);
            var pageJson = page.SerializeJSON();
            return Content("{\"total\": " + totalCount.ToString() + ",\"rows\":" + pageJson + "}");
        }


        #region 增加
        public ActionResult AddPage()
        {
            return View();
        }

        public ContentResult AddRole()
        {
            var sessionRes = base.GetSessionUser();
            string roleName = CheckRequest.GetString("RoleName");
            string description = CheckRequest.GetString("Description");
            string accountName = sessionRes.FirstParam.AccountName;
            B_TbRole entity = new B_TbRole() {
                Creater = accountName,
                CreateTime = DateTime.Now,
                Updater = accountName,
                UpdateTime = DateTime.Now,
                Description = description,
                RoleName = roleName,
            };
            var existEntity = B_TbRoleBLL.Instance.RoleDetail(roleName);
            if (existEntity == null)
            {
                int rid = B_TbRoleBLL.Instance.AddRole(entity);
                if (rid > 0)
                {
                    return Content("{\"msg\":\"添加成功！\",\"success\":true}");
                }
                else
                {
                    return Content("{\"msg\":\"添加失败！\",\"success\":false}");
                }
            }
            else {
                return Content("{\"msg\":\"添加失败，存在相同的角色名！\",\"success\":false}");
            }
        }

        #endregion

        #region 编辑
        public ActionResult EditPage()
        {
            return View();
        }

        public ContentResult EditRole()
        {
            var sessionRes = base.GetSessionUser();
            string accountName = sessionRes.FirstParam.AccountName;
            string id = CheckRequest.GetString("ID");
            string roleName = CheckRequest.GetString("RoleName");
            string description = CheckRequest.GetString("Description");

            B_TbRole entity = new B_TbRole()
            {
                ID = id.ToInt(),
                Updater = accountName,
                UpdateTime = DateTime.Now,
                Description = description,
                RoleName = roleName,
            };
            var existEntity = B_TbRoleBLL.Instance.RoleDetail(roleName);
            if (existEntity != null && existEntity.ID != id.ToInt())
            {
                return Content("{\"msg\":\"修改失败,按钮名称已存在！\",\"success\":false}");
            }
            else
            {
                int result = B_TbRoleBLL.Instance.UpdateRole(entity);
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
        public ContentResult DelRole()
        {
            string ids = CheckRequest.GetString("IDs");
            List<int> delIDs = ids.Split(',').ToArray().Select<string, int>(s => Convert.ToInt32(s)).ToList();
            if (SundryHelper.IsNumericArray(ids.Split(',')))
            {
                int result = B_TbRoleBLL.Instance.DelRole(delIDs);
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

        #region 角色授权

        public ActionResult RoleauthorizePage()
        {
            return View();
        }


        public ContentResult GetAllRoleMenuButton()
        {
            string roleId = CheckRequest.GetString("roleid");
            int rid =CheckRequest.IsNumber(roleId)==true?roleId.ToInt():0;
            //页面上可用按钮权限
            int totalCount = 0;
            List<MenuButtonEntity> tmbList = B_TbMenuBLL.Instance.MenuButtonByPage(null, null, string.Empty, ref totalCount, int.MaxValue, 1);
            List<B_TbMenu> menuList = B_TbMenuBLL.Instance.MenuByPage(null, null, string.Empty, ref totalCount, "ID", "asc", int.MaxValue, 1);
            var roleMenuList = B_TbButtonBLL.Instance.RoleMenuButtonList(rid);

            //一级目录
            B_TbMenu firstlevelMenu = menuList.Where(p => p.ParentId == 0 && p.MenuType==0).FirstOrDefault();
            //二级目录
            List<B_TbMenu> secondLevelMenu = menuList.Where(p => p.ParentId > 0).ToList();


            //一级目录返回体
            List<dynamic> firstLevelRes = new List<dynamic>();
            //二级目录返回体
            List<dynamic> secondLevelRes = new List<dynamic>();
            foreach (var menu in secondLevelMenu)
            {
                List<ButtonTree> btList = new List<ButtonTree>();
                var tempTmbList = tmbList.Where(p => p.MenuId == menu.ID).ToList();
                foreach (var tmb in tempTmbList)
                {
                    var exist = roleMenuList.Where(p => p.RoleId == rid && p.MenuId == tmb.MenuId && p.ButtonId == tmb.ButtonId).FirstOrDefault();
                    btList.Add(new ButtonTree() {
                        id = tmb.ID,
                        text = tmb.ButtonName,
                        Checked = exist != null ? true : false,
                        attributes = new RoleMenuButtonTreeAttribute() {
                                 buttonid=tmb.ButtonId,
                                 menuid=tmb.MenuId
                            },
                    });
                }
                if (btList != null && btList.Count > 0)
                {
                    secondLevelRes.Add(new
                    {
                        id = menu.ID,
                        text = menu.Name,
                        state = "closed",
                        attributes =
                                new RoleMenuButtonTreeAttribute()
                                {
                                    buttonid = 0,
                                    menuid = menu.ID
                                },
                        children = btList,
                    });
                }
            }

            firstLevelRes.Add(new {
                id = firstlevelMenu.ID,
                text = firstlevelMenu.Name,
                state = "open",
                attributes =
                           new RoleMenuButtonTreeAttribute() {
                                 buttonid=0,
                                 menuid= firstlevelMenu.ID

                           },
                children = secondLevelRes,
            });
            var menuJson = JsonConvert.SerializeObject(firstLevelRes);
            return Content(menuJson);
        }

        public ContentResult SetRoleMenuButton()
        {
            try
            {
                var sessionRes = base.GetSessionUser();
                string accountName = sessionRes.FirstParam.AccountName;
                string roleId = CheckRequest.GetString("RoleId");
                int rid = roleId.ToInt();
                string menuButtonId = CheckRequest.GetString("MenuButtonId");
                string[] tempArray = menuButtonId.Split(',');
                B_TbButtonBLL.Instance.RemoveMenuButtonByRoleId(roleId.ToInt());
                List<int> menuIds = new List<int>();

                for (int i = 0; i < tempArray.Length; i++)
                {
                    var temp = tempArray[i];
                    if (!string.IsNullOrEmpty(temp))
                    {
                        string[] mbArray = temp.Split(' ');
                        int menuId = mbArray[0].ToInt();
                        if (!menuIds.Contains(menuId))
                        {
                            menuIds.Add(menuId);
                        }
                        int buttonId = mbArray[1].ToInt();
                        B_TbRoleMenuButtonDAL.Instance.AddRoleMenuButton(new B_TbRoleMenuButton()
                        {
                            ButtonId = buttonId,
                            MenuId = menuId,
                            RoleId = rid,
                            Updater = accountName,
                            UpdateTime = DateTime.Now
                        });
                    }
                }
                //清理角色与菜单关系
                B_TbRoleBLL.Instance.DelRoleMenuByRoleID(rid);
                //赋予角色与菜单根节点
                menuIds.Add(1);
                for (int i = 0; i < menuIds.Count; i++)
                {
                     B_TbRoleBLL.Instance.AddRoleMenu(new B_TbRoleMenu() {
                         RoleId=rid,
                         MenuId=menuIds[i],
                         Updater=sessionRes.FirstParam.AccountName,
                         UpdateTime=DateTime.Now
                    });
                }
             

                return Content("{\"msg\":\"授权成功！\",\"success\":true}");
            }
            catch
            {
                return Content("{\"msg\":\"授权失败！\",\"success\":false}");
            }
        }
        #endregion
    }

    #region 授权返回对象体
    internal class ButtonTree {
        public int id { get; set; }

        public string text { get; set; }
        [JsonProperty(PropertyName = "checked")]
        public bool Checked { get; set; }

        public RoleMenuButtonTreeAttribute attributes { get; set; }
    }

    internal class RoleMenuButtonTreeAttribute {
        public int menuid { get; set; }
        public int buttonid { get; set; }
    }
    #endregion 

}