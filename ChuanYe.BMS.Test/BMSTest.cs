using ChuanYe.BMS.DAL;
using ChuanYe.BMS.DAL.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChuanYe.BMS.Test
{
    public class BMSTest
    {
        public static void Init() {

            Console.WriteLine("开始设置....");

            #region 初始化表结构
            Console.WriteLine("初始化表结构....");
            InitCreateTable.Instance.Init();
            #endregion

            #region  初始化用户（超级管理员 Admin）
            Console.WriteLine("初始化用户（超级管理员 Admin)....");
            B_TbUser user = new B_TbUser()
            {
                AccountName = "admin",
                Password = "21232f297a57a5a743894a0e4a801fc3",
                RealName = "limeng",
                Email = "limeng8989@sina.com",
                Description = "初始化用户，禁止删除",
                Position = "100000",
                WorkNumber = "1",
                Creater = "系统",
                CreateTime = DateTime.Now,
                Updater = "系统",
                UpdateTime = DateTime.Now,
                MobilePhone = "13520655503",
                IfChangePwd = true,
                IsAble = true,

            };
            int userId = 0;
            if (B_TbUserDAL.Instance.UserDetail(user.AccountName) == null)
            {
                userId = B_TbUserDAL.Instance.AddUser(user);
            }
            #endregion

            #region 初始化角色
            B_TbRole role = new B_TbRole()
            {
                RoleName = "超级管理员",
                Description = string.Empty,
                Creater = user.Creater,
                CreateTime = user.CreateTime,
                Updater = user.Updater,
                UpdateTime = user.UpdateTime,
            };
            int roleId = 0;
            if (B_TbRoleDAL.Instance.RoleDetail(role.RoleName) == null)
            {
                roleId = B_TbRoleDAL.Instance.AddRole(role);
            }
            #endregion

            #region 初始化菜单
            Console.WriteLine("初始化 菜单管理 模块....");

            #region 系统管理 菜单 
            B_TbMenu menu = new B_TbMenu()
            {
                Name = "系统管理",
                ParentId = 0,
                Code = "system",
                Sort = 1,
                Icon = "",
                LinkAddress = "",
                Creater = user.AccountName,
                CreateTime = user.CreateTime,
                Updater = user.Updater,
                UpdateTime = user.UpdateTime,
                MenuType = 0,
            };

            int parentId = 0;
            if (B_TbMenuDAL.Instance.MenuDetail(menu.Name) == null)
            {
                parentId = B_TbMenuDAL.Instance.AddMenu(menu);
            }
            #endregion

            #region 用户管理 菜单
            B_TbMenu userMenu = new B_TbMenu()
            {
                Name = "用户管理",
                ParentId = parentId,
                Code = "user",
                Sort = 1,
                Icon = "",
                LinkAddress = "/User/Index",
                Creater = user.AccountName,
                CreateTime = user.CreateTime,
                Updater = user.Updater,
                UpdateTime = user.UpdateTime,
                MenuType = 0,
            };
            int userMenuId = 0;
            if (B_TbMenuDAL.Instance.MenuDetail(userMenu.Name) == null)
            {
                userMenuId = B_TbMenuDAL.Instance.AddMenu(userMenu);
            }
            #endregion

            #region 角色管理 菜单
            B_TbMenu roleMenu = new B_TbMenu()
            {
                Name = "角色管理",
                ParentId = parentId,
                Code = "role",
                Sort = 2,
                Icon = "",
                LinkAddress = "/Role/Index",
                Creater = user.AccountName,
                CreateTime = user.CreateTime,
                Updater = user.Updater,
                UpdateTime = user.UpdateTime,
                MenuType = 0,
            };
            int roleMenuId = 0;
            if (B_TbMenuDAL.Instance.MenuDetail(roleMenu.Name) == null)
            {
                roleMenuId = B_TbMenuDAL.Instance.AddMenu(roleMenu);
            }
            #endregion

            #region 菜单管理 菜单
            B_TbMenu menuMenu = new B_TbMenu()
            {
                Name = "菜单管理",
                ParentId = parentId,
                Code = "menu",
                Sort = 3,
                Icon = "",
                LinkAddress = "/Menu/Index",
                Creater = user.AccountName,
                CreateTime = user.CreateTime,
                Updater = user.Updater,
                UpdateTime = user.UpdateTime,
                MenuType = 0,
            };
            int menuMenuId = 0;
            if (B_TbMenuDAL.Instance.MenuDetail(menuMenu.Name) == null)
            {
                menuMenuId = B_TbMenuDAL.Instance.AddMenu(menuMenu);
            }
            #endregion

            #region 按钮管理 菜单
            B_TbMenu buttonMenu = new B_TbMenu()
            {
                Name = "按钮管理",
                ParentId = parentId,
                Code = "button",
                Sort = 4,
                Icon = "",
                LinkAddress = "/Button/Index",
                Creater = user.AccountName,
                CreateTime = user.CreateTime,
                Updater = user.Updater,
                UpdateTime = user.UpdateTime,
                MenuType = 0,
            };
            int buttonMenuId = 0;
            if (B_TbMenuDAL.Instance.MenuDetail(buttonMenu.Name) == null)
            {
                buttonMenuId = B_TbMenuDAL.Instance.AddMenu(buttonMenu);
            }
            #endregion

            #region 图标管理 菜单
            B_TbMenu iconMenu = new B_TbMenu()
            {
                Name = "图标管理",
                ParentId = parentId,
                Code = "Icon",
                Sort = 5,
                Icon = "",
                LinkAddress = "/Icon/Index",
                Creater = user.AccountName,
                CreateTime = user.CreateTime,
                Updater = user.Updater,
                UpdateTime = user.UpdateTime,
                MenuType = 0,
            };
            int iconMenuId = 0;
            if (B_TbMenuDAL.Instance.MenuDetail(iconMenu.Name) == null)
            {
                iconMenuId = B_TbMenuDAL.Instance.AddMenu(iconMenu);
            }
            #endregion

            #endregion

            #region 初始化按钮
            Console.WriteLine("初始化 按钮 模块....");

            #region 查询
            B_TbButton searchButton = new B_TbButton()
            {
                Name = "查询",
                Code = "search",
                Icon = "icon-eye",
                Sort = 1,
                Description = string.Empty,
                Updater = user.Updater,
                UpdateTime = user.UpdateTime,
                Creater = user.Creater,
                CreateTime = user.CreateTime,
            };
            int searchButtonId = 0;
            if (B_TbButtonDAL.Instance.ButtonDetail(searchButton.Name) == null)
            {
                searchButtonId = B_TbButtonDAL.Instance.AddButton(searchButton);
            }
            #endregion

            #region 报废
            B_TbButton scrapButton = new B_TbButton()
            {
                Name = "报废",
                Code = "scrap",
                Icon = "icon-delete",
                Sort = 2,
                Description = string.Empty,
                Updater = user.Updater,
                UpdateTime = user.UpdateTime,
                Creater = user.Creater,
                CreateTime = user.CreateTime,
            };
            int scrapButtonId = 0;
            if (B_TbButtonDAL.Instance.ButtonDetail(scrapButton.Name) == null)
            {
                scrapButtonId = B_TbButtonDAL.Instance.AddButton(scrapButton);
            }
            #endregion

            #region 添加
            B_TbButton addButton = new B_TbButton()
            {
                Name = "添加",
                Code = "add",
                Icon = "icon-add",
                Sort = 3,
                Description = string.Empty,
                Updater = user.Updater,
                UpdateTime = user.UpdateTime,
                Creater = user.Creater,
                CreateTime = user.CreateTime,
            };
            int addButtonId = 0;
            if (B_TbButtonDAL.Instance.ButtonDetail(addButton.Name) == null)
            {
                addButtonId = B_TbButtonDAL.Instance.AddButton(addButton);
            }
            #endregion

            #region 修改
            B_TbButton editButton = new B_TbButton()
            {
                Name = "修改",
                Code = "edit",
                Icon = "icon-edit",
                Sort = 4,
                Description = string.Empty,
                Updater = user.Updater,
                UpdateTime = user.UpdateTime,
                Creater = user.Creater,
                CreateTime = user.CreateTime,
            };
            int editButtonId = 0;
            if (B_TbButtonDAL.Instance.ButtonDetail(editButton.Name) == null)
            {
                editButtonId = B_TbButtonDAL.Instance.AddButton(editButton);
            }
            #endregion

            #region 删除
            B_TbButton deleteButton = new B_TbButton()
            {
                Name = "删除",
                Code = "delete",
                Icon = "icon-delete",
                Sort = 5,
                Description = string.Empty,
                Updater = user.Updater,
                UpdateTime = user.UpdateTime,
                Creater = user.Creater,
                CreateTime = user.CreateTime,
            };

            int deleteButtonId = 0;
            if (B_TbButtonDAL.Instance.ButtonDetail(deleteButton.Name) == null)
            {
                deleteButtonId = B_TbButtonDAL.Instance.AddButton(deleteButton);
            }
            #endregion

            #region 导出
            B_TbButton exportButton = new B_TbButton()
            {
                Name = "导出",
                Code = "export",
                Icon = "icon-page_excel",
                Sort = 6,
                Description = string.Empty,
                Updater = user.Updater,
                UpdateTime = user.UpdateTime,
                Creater = user.Creater,
                CreateTime = user.CreateTime,
            };
            if (B_TbButtonDAL.Instance.ButtonDetail(exportButton.Name) == null)
            {
                B_TbButtonDAL.Instance.AddButton(exportButton);
            }


            #endregion

            #region 导入
            B_TbButton importButton = new B_TbButton()
            {
                Name = "导出",
                Code = "import",
                Icon = "icon-import",
                Sort = 7,
                Description = string.Empty,
                Updater = user.Updater,
                UpdateTime = user.UpdateTime,
                Creater = user.Creater,
                CreateTime = user.CreateTime,
            };
            if (B_TbButtonDAL.Instance.ButtonDetail(importButton.Name) == null)
            {
                B_TbButtonDAL.Instance.AddButton(importButton);
            }

            #endregion

            #region 角色设置
            B_TbButton setuserroleButton = new B_TbButton()
            {
                Name = "角色设置",
                Code = "setuserrole",
                Icon = "icon-user_key",
                Sort = 8,
                Description = string.Empty,
                Updater = user.Updater,
                UpdateTime = user.UpdateTime,
                Creater = user.Creater,
                CreateTime = user.CreateTime,
            };
            if (B_TbButtonDAL.Instance.ButtonDetail(setuserroleButton.Name) == null)
            {
                B_TbButtonDAL.Instance.AddButton(setuserroleButton);
            }
            #endregion

            #region 部门设置
            B_TbButton setuserdeptButton = new B_TbButton()
            {
                Name = "部门设置",
                Code = "setuserdept",
                Icon = "icon-group",
                Sort = 9,
                Description = string.Empty,
                Updater = user.Updater,
                UpdateTime = user.UpdateTime,
                Creater = user.Creater,
                CreateTime = user.CreateTime,
            };
            if (B_TbButtonDAL.Instance.ButtonDetail(setuserdeptButton.Name) == null)
            {
                B_TbButtonDAL.Instance.AddButton(setuserdeptButton);
            }
            #endregion

            #region 角色授权
            B_TbButton roleauthorizeButton = new B_TbButton()
            {
                Name = "角色授权",
                Code = "roleauthorize",
                Icon = "icon-key",
                Sort = 10,
                Description = string.Empty,
                Updater = user.Updater,
                UpdateTime = user.UpdateTime,
                Creater = user.Creater,
                CreateTime = user.CreateTime,
            };
            int roleauthorizeButtonId = 0;
            if (B_TbButtonDAL.Instance.ButtonDetail(roleauthorizeButton.Name) == null)
            {
                roleauthorizeButtonId = B_TbButtonDAL.Instance.AddButton(roleauthorizeButton);
            }
            #endregion

            #region 分配按钮
            B_TbButton setmenuButton = new B_TbButton()
            {
                Name = "分配按钮",
                Code = "setmenu",
                Icon = "icon-link",
                Sort = 11,
                Description = string.Empty,
                Updater = user.Updater,
                UpdateTime = user.UpdateTime,
                Creater = user.Creater,
                CreateTime = user.CreateTime,
            };

            int setmenuButtonId = 0;
            if (B_TbButtonDAL.Instance.ButtonDetail(setmenuButton.Name) == null)
            {
                setmenuButtonId = B_TbButtonDAL.Instance.AddButton(setmenuButton);
            }
            #endregion

            #region 全部展开
            B_TbButton expandallButton = new B_TbButton()
            {
                Name = "全部展开",
                Code = "expandall",
                Icon = "icon-expand",
                Sort = 12,
                Description = string.Empty,
                Updater = user.Updater,
                UpdateTime = user.UpdateTime,
                Creater = user.Creater,
                CreateTime = user.CreateTime,
            };
            if (B_TbButtonDAL.Instance.ButtonDetail(expandallButton.Name) == null)
            {
                B_TbButtonDAL.Instance.AddButton(expandallButton);
            }
            #endregion

            #region 全部隐藏
            B_TbButton collapseallButton = new B_TbButton()
            {
                Name = "全部隐藏",
                Code = "collapseall",
                Icon = "icon-expand",
                Sort = 13,
                Description = string.Empty,
                Updater = user.Updater,
                UpdateTime = user.UpdateTime,
                Creater = user.Creater,
                CreateTime = user.CreateTime,
            };
            if (B_TbButtonDAL.Instance.ButtonDetail(collapseallButton.Name) == null)
            {
                B_TbButtonDAL.Instance.AddButton(collapseallButton);
            }
            #endregion

            #region 表数据查询
            B_TbButton seltabdataButton = new B_TbButton()
            {
                Name = "表数据查询",
                Code = "seltabdata",
                Icon = "icon-search",
                Sort = 14,
                Description = string.Empty,
                Updater = user.Updater,
                UpdateTime = user.UpdateTime,
                Creater = user.Creater,
                CreateTime = user.CreateTime,
            };
            if (B_TbButtonDAL.Instance.ButtonDetail(seltabdataButton.Name) == null)
            {
                B_TbButtonDAL.Instance.AddButton(seltabdataButton);
            }
            #endregion


            #endregion

            #region 初始化图标

            B_TbIcon searchIcons = new B_TbIcon()
            {
                Creater = user.Creater,
                CreateTime = user.CreateTime,
                IconCssInfo = "icon-search",
                IconName = "search",
                Updater = user.Creater,
                UpdateTime = user.CreateTime,
            };

            B_TbIcon addIcons = new B_TbIcon()
            {
                Creater = user.Creater,
                CreateTime = user.CreateTime,
                IconCssInfo = "icon-add",
                IconName = "add",
                Updater = user.Creater,
                UpdateTime = user.CreateTime,
            };

            B_TbIcon editIcons = new B_TbIcon()
            {
                Creater = user.Creater,
                CreateTime = user.CreateTime,
                IconCssInfo = "icon-edit",
                IconName = "edit",
                Updater = user.Creater,
                UpdateTime = user.CreateTime,
            };
            B_TbIcon deleteIcons = new B_TbIcon()
            {
                Creater = user.Creater,
                CreateTime = user.CreateTime,
                IconCssInfo = "icon-delete",
                IconName = "delete",
                Updater = user.Creater,
                UpdateTime = user.CreateTime,
            };
            B_TbIcon importIcons = new B_TbIcon()
            {
                Creater = user.Creater,
                CreateTime = user.CreateTime,
                IconCssInfo = "icon-import",
                IconName = "import",
                Updater = user.Creater,
                UpdateTime = user.CreateTime,
            };
            B_TbIcon keyIcons = new B_TbIcon()
            {
                Creater = user.Creater,
                CreateTime = user.CreateTime,
                IconCssInfo = "icon-key",
                IconName = "key",
                Updater = user.Creater,
                UpdateTime = user.CreateTime,
            };
            B_TbIcon userKeyIcons = new B_TbIcon()
            {
                Creater = user.Creater,
                CreateTime = user.CreateTime,
                IconCssInfo = "icon-user_key",
                IconName = "user_key",
                Updater = user.Creater,
                UpdateTime = user.CreateTime,
            };
            B_TbIconDAL.Instance.AddIcon(searchIcons);
            B_TbIconDAL.Instance.AddIcon(addIcons);
            B_TbIconDAL.Instance.AddIcon(editIcons);
            B_TbIconDAL.Instance.AddIcon(deleteIcons);
            B_TbIconDAL.Instance.AddIcon(importIcons);
            B_TbIconDAL.Instance.AddIcon(keyIcons);
            B_TbIconDAL.Instance.AddIcon(userKeyIcons);

            #endregion

            #region 初始化 用户与角色关系
            Console.WriteLine("初始化  用户与角色关系....");
            B_TbUserRoleDAL.Instance.AddUserRole(new B_TbUserRole() { UserId = userId, RoleId = roleId });
            #endregion

            #region 初始化 角色菜单关系
            Console.WriteLine("初始化  角色菜单关系....");
            B_TbRoleMenuDAL.Instance.AddRoleMenu(new B_TbRoleMenu() { RoleId = roleId, MenuId = parentId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbRoleMenuDAL.Instance.AddRoleMenu(new B_TbRoleMenu() { RoleId = roleId, MenuId = userMenuId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbRoleMenuDAL.Instance.AddRoleMenu(new B_TbRoleMenu() { RoleId = roleId, MenuId = roleMenuId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbRoleMenuDAL.Instance.AddRoleMenu(new B_TbRoleMenu() { RoleId = roleId, MenuId = menuMenuId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbRoleMenuDAL.Instance.AddRoleMenu(new B_TbRoleMenu() { RoleId = roleId, MenuId = buttonMenuId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbRoleMenuDAL.Instance.AddRoleMenu(new B_TbRoleMenu() { RoleId = roleId, MenuId = iconMenuId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            #endregion

            #region 初始化 菜单按钮关系
            Console.WriteLine("初始化  菜单按钮关系....");

            #region 菜单管理赋予权限
            //默认给菜单管理  提供增删改查功能(菜单可用权限)
            B_TbMenuButtonDAL.Instance.AddMenuButton(new B_TbMenuButton() { MenuId = menuMenuId, ButtonId = addButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbMenuButtonDAL.Instance.AddMenuButton(new B_TbMenuButton() { MenuId = menuMenuId, ButtonId = deleteButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbMenuButtonDAL.Instance.AddMenuButton(new B_TbMenuButton() { MenuId = menuMenuId, ButtonId = editButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbMenuButtonDAL.Instance.AddMenuButton(new B_TbMenuButton() { MenuId = menuMenuId, ButtonId = searchButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbMenuButtonDAL.Instance.AddMenuButton(new B_TbMenuButton() { MenuId = menuMenuId, ButtonId = setmenuButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });


            //默认给菜单管理提供  增删改查功能(角色下可用的菜单按钮权限)
            B_TbRoleMenuButtonDAL.Instance.AddRoleMenuButton(new B_TbRoleMenuButton() { RoleId = roleId, MenuId = menuMenuId, ButtonId = addButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbRoleMenuButtonDAL.Instance.AddRoleMenuButton(new B_TbRoleMenuButton() { RoleId = roleId, MenuId = menuMenuId, ButtonId = deleteButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbRoleMenuButtonDAL.Instance.AddRoleMenuButton(new B_TbRoleMenuButton() { RoleId = roleId, MenuId = menuMenuId, ButtonId = editButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbRoleMenuButtonDAL.Instance.AddRoleMenuButton(new B_TbRoleMenuButton() { RoleId = roleId, MenuId = menuMenuId, ButtonId = searchButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbRoleMenuButtonDAL.Instance.AddRoleMenuButton(new B_TbRoleMenuButton() { RoleId = roleId, MenuId = menuMenuId, ButtonId = setmenuButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            #endregion

            #region 用户管理赋予权限
            //默认给用户管理  提供增删改查功能(用户可用权限)
            B_TbMenuButtonDAL.Instance.AddMenuButton(new B_TbMenuButton() { MenuId = userMenuId, ButtonId = addButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbMenuButtonDAL.Instance.AddMenuButton(new B_TbMenuButton() { MenuId = userMenuId, ButtonId = deleteButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbMenuButtonDAL.Instance.AddMenuButton(new B_TbMenuButton() { MenuId = userMenuId, ButtonId = editButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbMenuButtonDAL.Instance.AddMenuButton(new B_TbMenuButton() { MenuId = userMenuId, ButtonId = searchButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });

            //默认给用户管理提供  增删改查功能(用户下可用的菜单按钮权限)
            B_TbRoleMenuButtonDAL.Instance.AddRoleMenuButton(new B_TbRoleMenuButton() { RoleId = roleId, MenuId = userMenuId, ButtonId = addButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbRoleMenuButtonDAL.Instance.AddRoleMenuButton(new B_TbRoleMenuButton() { RoleId = roleId, MenuId = userMenuId, ButtonId = deleteButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbRoleMenuButtonDAL.Instance.AddRoleMenuButton(new B_TbRoleMenuButton() { RoleId = roleId, MenuId = userMenuId, ButtonId = editButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbRoleMenuButtonDAL.Instance.AddRoleMenuButton(new B_TbRoleMenuButton() { RoleId = roleId, MenuId = userMenuId, ButtonId = searchButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });

            #endregion



            #region 角色管理赋予权限
            //角色管理赋予权限  提供增删改查功能(菜单可用权限)
            B_TbMenuButtonDAL.Instance.AddMenuButton(new B_TbMenuButton() { MenuId = roleMenuId, ButtonId = addButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbMenuButtonDAL.Instance.AddMenuButton(new B_TbMenuButton() { MenuId = roleMenuId, ButtonId = deleteButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbMenuButtonDAL.Instance.AddMenuButton(new B_TbMenuButton() { MenuId = roleMenuId, ButtonId = editButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbMenuButtonDAL.Instance.AddMenuButton(new B_TbMenuButton() { MenuId = roleMenuId, ButtonId = searchButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbMenuButtonDAL.Instance.AddMenuButton(new B_TbMenuButton() { MenuId = roleMenuId, ButtonId = roleauthorizeButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });


            //角色管理赋予权限  增删改查功能(角色下可用的菜单按钮权限)
            B_TbRoleMenuButtonDAL.Instance.AddRoleMenuButton(new B_TbRoleMenuButton() { RoleId = roleId, MenuId = roleMenuId, ButtonId = addButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbRoleMenuButtonDAL.Instance.AddRoleMenuButton(new B_TbRoleMenuButton() { RoleId = roleId, MenuId = roleMenuId, ButtonId = deleteButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbRoleMenuButtonDAL.Instance.AddRoleMenuButton(new B_TbRoleMenuButton() { RoleId = roleId, MenuId = roleMenuId, ButtonId = editButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbRoleMenuButtonDAL.Instance.AddRoleMenuButton(new B_TbRoleMenuButton() { RoleId = roleId, MenuId = roleMenuId, ButtonId = searchButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbRoleMenuButtonDAL.Instance.AddRoleMenuButton(new B_TbRoleMenuButton() { RoleId = roleId, MenuId = roleMenuId, ButtonId = roleauthorizeButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            #endregion

            #region 按钮管理赋予权限
            //默认给按钮管理 提供增删改查功能
            B_TbMenuButtonDAL.Instance.AddMenuButton(new B_TbMenuButton() { MenuId = buttonMenuId, ButtonId = addButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbMenuButtonDAL.Instance.AddMenuButton(new B_TbMenuButton() { MenuId = buttonMenuId, ButtonId = deleteButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbMenuButtonDAL.Instance.AddMenuButton(new B_TbMenuButton() { MenuId = buttonMenuId, ButtonId = editButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbMenuButtonDAL.Instance.AddMenuButton(new B_TbMenuButton() { MenuId = buttonMenuId, ButtonId = searchButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });

            //默认给菜单管理提供  增删改查功能(角色下可用的菜单按钮权限)
            B_TbRoleMenuButtonDAL.Instance.AddRoleMenuButton(new B_TbRoleMenuButton() { RoleId = roleId, MenuId = buttonMenuId, ButtonId = addButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbRoleMenuButtonDAL.Instance.AddRoleMenuButton(new B_TbRoleMenuButton() { RoleId = roleId, MenuId = buttonMenuId, ButtonId = deleteButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbRoleMenuButtonDAL.Instance.AddRoleMenuButton(new B_TbRoleMenuButton() { RoleId = roleId, MenuId = buttonMenuId, ButtonId = editButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbRoleMenuButtonDAL.Instance.AddRoleMenuButton(new B_TbRoleMenuButton() { RoleId = roleId, MenuId = buttonMenuId, ButtonId = searchButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            #endregion

            #region 图标管理赋予权限

            B_TbMenuButtonDAL.Instance.AddMenuButton(new B_TbMenuButton() { MenuId = iconMenuId, ButtonId = addButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbMenuButtonDAL.Instance.AddMenuButton(new B_TbMenuButton() { MenuId = iconMenuId, ButtonId = deleteButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbMenuButtonDAL.Instance.AddMenuButton(new B_TbMenuButton() { MenuId = iconMenuId, ButtonId = editButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbMenuButtonDAL.Instance.AddMenuButton(new B_TbMenuButton() { MenuId = iconMenuId, ButtonId = searchButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });

            B_TbRoleMenuButtonDAL.Instance.AddRoleMenuButton(new B_TbRoleMenuButton() { RoleId = roleId, MenuId = iconMenuId, ButtonId = addButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbRoleMenuButtonDAL.Instance.AddRoleMenuButton(new B_TbRoleMenuButton() { RoleId = roleId, MenuId = iconMenuId, ButtonId = deleteButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbRoleMenuButtonDAL.Instance.AddRoleMenuButton(new B_TbRoleMenuButton() { RoleId = roleId, MenuId = iconMenuId, ButtonId = editButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });
            B_TbRoleMenuButtonDAL.Instance.AddRoleMenuButton(new B_TbRoleMenuButton() { RoleId = roleId, MenuId = iconMenuId, ButtonId = searchButtonId, Updater = user.Updater, UpdateTime = user.UpdateTime });

            #endregion

            #endregion

            Console.WriteLine("完成设置....");

            Console.ReadKey();
        }
    }
}
