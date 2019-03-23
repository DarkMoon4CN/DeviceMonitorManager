using ChuanYe.BMS.DAL.Entity;
using ChuanYe.BMS.DAL.Table;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChuanYe.BMS.DAL
{
    public class B_TbUserRoleDAL
    {
        private SqlSugarClient db = SqlSugarFactory.GetInstance(InitKeyType.SystemTable);

        #region 构造单例
        private B_TbUserRoleDAL() { }
        private static B_TbUserRoleDAL _instance;
        public static B_TbUserRoleDAL Instance
        {
            get
            {
                return _instance ?? (_instance = new B_TbUserRoleDAL());
            }
        }
        #endregion

        #region CRUD
        public int AddUserRole(B_TbUserRole info)
        {
            int id = db.Insertable<B_TbUserRole>(info).With(SqlWith.UpdLock).ExecuteReturnIdentity();
            return id;
        }

        public B_TbUserRole UserRoleDetail(int userId, int roleId)
        {
            return db.Queryable<B_TbUserRole>().Where(p => p.UserId == userId &&  p.RoleId == roleId).First();
        }
        public B_TbUserRole UserRoleDetail(int id)
        {
            return db.Queryable<B_TbUserRole>().Where(p => p.ID==id).First();
        }

        public int DelUserRoleByUserIDs(List<int> userIds)
        {
            return db.Deleteable<B_TbUserRole>().Where(p =>userIds.Contains(p.UserId)).ExecuteCommand();
        }


        /// <summary>
        /// 根据用户id 返回用户权限信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List params: ID,UserId,AccountName,UserNameRoleId,RoleName </returns>
        public UserRoleEntity UserRoleDetailByUserId(int userId)
        {
            var exp= Expressionable.Create<B_TbUserRole, B_TbUser,B_TbRole>();
            exp = exp.And((ur, u, r) => ur.UserId == userId);
            var data = db.Queryable<B_TbUserRole, B_TbUser, B_TbRole>(
              (ur, u, r) =>
               new object[]{
                       JoinType.Left,ur.UserId == u.ID,
                       JoinType.Left,ur.RoleId == r.ID,
               })
               .Where(exp.ToExpression())
               .OrderBy((ur) => ur.ID, OrderByType.Desc)
               .Select((ur, u, r) => new UserRoleEntity { ID = ur.ID, RoleId = ur.RoleId, RoleName = r.RoleName, UserId = u.ID, AccountName = u.AccountName})
               .First();
            return data;
        }

       



        /// <summary>
        ///  根据用户ID 获取菜单信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List params: ID,RoleId,UserId,Name</returns>
        public dynamic UserRoleMenuList(int userId,int parentId)
        {
            var exp = Expressionable.Create<B_TbRoleMenu,B_TbMenu,B_TbUserRole>();
            exp = exp.And((rm, m, ur) => ur.UserId == userId && m.ParentId == parentId);
            var list = db.Queryable<B_TbRoleMenu, B_TbMenu, B_TbUserRole>(
           (rm, m, ur) =>
            new object[]{
                       JoinType.Left,rm.MenuId == m.ID,
                       JoinType.Left,rm.RoleId == ur.ID,
            })
            .Where(exp.ToExpression())
            .OrderBy((rm, m, ur) => m.Sort, OrderByType.Asc)
            .Select((rm, m, ur) => new {  ID = rm.ID, RoleId = ur.RoleId, UserId = ur.ID,MenuId=m.ID,Name=m.Name,m.LinkAddress,m.ParentId,m.Code,m.Icon})
            .ToList();
            return list;
        }


        /// <summary>
        ///  根据角色ID 获取菜单信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List params: ID,RoleId,UserId,Name</returns>
        public dynamic UserRoleMenuList2(int roleId, int parentId)
        {
            var exp = Expressionable.Create<B_TbRoleMenu, B_TbMenu,B_TbRole>();
            exp = exp.And((rm, m, u) => rm.RoleId == roleId && m.ParentId == parentId);
            var list = db.Queryable<B_TbRoleMenu, B_TbMenu, B_TbRole>(
           (rm, m, u) =>
            new object[]{
                       JoinType.Left,rm.MenuId == m.ID,
                       JoinType.Left,rm.RoleId == u.ID,
            })
            .Where(exp.ToExpression())
            .OrderBy((rm, m, u) => m.Sort, OrderByType.Asc)
            .Select((rm, m, u) => new { ID = rm.ID, RoleId = u.ID,MenuId = m.ID, Name = m.Name, m.LinkAddress, m.ParentId, m.Code, m.Icon })
            .ToList();
            return list;
        }





        /// <summary>
        /// 根据权限ID 获取用户列表
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public dynamic UserListByRoleId(int roleId)
        {
            var exp = Expressionable.Create<B_TbUser,B_TbUserRole>();
            exp = exp.And((u, r) => r.RoleId==roleId);
            var list = db.Queryable<B_TbUser, B_TbUserRole>(
             (u, r) =>
              new object[]{
                        JoinType.Left,u.ID == r.UserId,
              })
              .Where(exp.ToExpression())
              .OrderBy((u, r) => r.ID, OrderByType.Asc)
              .ToList();
            return list;
        }



        /// <summary>
        /// 根据权限ID 获取用户列表 分页
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<B_TbUser> UserListByRoleId( DateTime? startTime, DateTime? endTime, int roleId , ref int totalCount,
                                     int pageSize = 20, int pageIndex = 1)
        {
            var exp = Expressionable.Create<B_TbUser, B_TbUserRole>();
            if (roleId > 0)
            {
                exp = exp.And((u, r) => r.RoleId == roleId);
            }
            if (startTime != null)
            {
                exp = exp.And((u, r) => u.UpdateTime > startTime);
            }
            if (endTime != null)
            {
                exp = exp.And((u, r) => u.UpdateTime < endTime);
            }

            var list = db.Queryable<B_TbUser, B_TbUserRole>(
             (u, r) =>
              new object[]{
                        JoinType.Left,u.ID == r.UserId,
              })
              .Where(exp.ToExpression())
              .OrderBy((u, r) => u.UpdateTime, OrderByType.Asc)
              .ToPageList(pageIndex, pageSize, ref totalCount);
            return list;
        }
        #endregion
    }
}
