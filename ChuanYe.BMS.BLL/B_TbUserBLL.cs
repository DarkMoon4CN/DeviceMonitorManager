using ChuanYe.BMS.DAL;
using ChuanYe.BMS.DAL.Entity;
using ChuanYe.BMS.DAL.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChuanYe.BMS.BLL
{
    public class B_TbUserBLL
    {
        #region 构造单例
        private B_TbUserBLL() { }
        private static B_TbUserBLL _instance;
        public static B_TbUserBLL Instance
        {
            get
            {
                return _instance ?? (_instance = new B_TbUserBLL());
            }
        }
        #endregion

        private B_TbUserDAL userDAL = B_TbUserDAL.Instance;
        private B_TbUserRoleDAL userRoleDAL = B_TbUserRoleDAL.Instance;

        public B_TbUser Login(string accountName, string password)
        {
            return userDAL.UserDetail(accountName, password);
        }

        public B_TbUser UserDetail(int id)
        {
            return userDAL.UserDetail(id);
        }

        public int AddUser(B_TbUser info)
        {
            return userDAL.AddUser(info);
        }

        public int DelUser(List<int> ids)
        {
            return userDAL.DelUser(ids);
        }

        public int DelUserRoleByUserIDs(List<int> userIds)
        {
            return userRoleDAL.DelUserRoleByUserIDs(userIds);
        }

        public dynamic UserRoleMenuList(int userId,int parentId)
        {
            return userRoleDAL.UserRoleMenuList(userId,parentId);
        }

        public dynamic UserRoleMenuList2(int roleId, int parentId)
        {
            return userRoleDAL.UserRoleMenuList2(roleId, parentId);
        }


        public UserRoleEntity UserRoleList(int userId)
        {
            return userRoleDAL.UserRoleDetailByUserId(userId);
        }

        public List<B_TbUser> UserListByRoleId(DateTime? startTime, DateTime? endTime, int roleId, ref int totalCount,
                                  int pageSize = 20, int pageIndex = 1)
        {
            return userRoleDAL.UserListByRoleId(startTime, endTime, roleId, ref totalCount, pageSize, pageIndex);
        }
        public dynamic UserListByRoleId(int roleId)
        {
            return userRoleDAL.UserListByRoleId(roleId);
        }
        public List<UserRoleEntity> UsersByPage(B_TbUser info,DateTime? startTime, DateTime? endTime,  int isAble,
                             ref int totalCount,
                             int pageSize = 20, int pageIndex = 1)
        {
            return userDAL.UsersByPage(info,startTime, endTime,  isAble, ref totalCount, pageSize, pageIndex);
        }

        public int UpdateUser(B_TbUser info)
        {
            return userDAL.UpdateUser(info);
        }

        public int AddUserRole(B_TbUserRole info)
        {
            return userRoleDAL.AddUserRole(info);
        }
        public B_TbUserRole UserRoleDetail(int userId, int roleId)
        {
            return userRoleDAL.UserRoleDetail(userId, roleId);
        }
    }
}
