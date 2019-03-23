using ChuanYe.BMS.DAL.Table;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChuanYe.BMS.DAL
{
    public class B_TbRoleMenuDAL
    {
        private SqlSugarClient db = SqlSugarFactory.GetInstance(InitKeyType.SystemTable);

        #region 构造单例
        private B_TbRoleMenuDAL() { }
        private static B_TbRoleMenuDAL _instance;
        public static B_TbRoleMenuDAL Instance
        {
            get
            {
                return _instance ?? (_instance = new B_TbRoleMenuDAL());
            }
        }
        #endregion

        #region CRUD
        public int AddRoleMenu(B_TbRoleMenu info)
        {
            int id = db.Insertable<B_TbRoleMenu>(info).With(SqlWith.UpdLock).ExecuteReturnIdentity();
            return id;
        }

        public B_TbRoleMenu RoleMenuDetail(int roleId, int menuId)
        {
            return db.Queryable<B_TbRoleMenu>().Where(p => p.RoleId == roleId && p.MenuId == menuId).First();
        }
        public B_TbRoleMenu RoleMenuDetail(int id)
        {
            return db.Queryable<B_TbRoleMenu>().Where(p => p.ID==id).First();
        }
      
        /// <summary>
        ///  根据角色获取其菜单信息
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns>List params: ID,RoleId,RoleName,MenuId,MenuName</returns>
        public dynamic RoleMenuList(int roleId)
        {

            var exp = Expressionable.Create<B_TbRoleMenu, B_TbRole, B_TbMenu>();
            exp = exp.And((rm, r, m) => rm.RoleId == roleId);
            var list = db.Queryable<B_TbRoleMenu, B_TbRole, B_TbMenu>(
              (rm, r, m) =>
               new object[]{
                       JoinType.Left,rm.RoleId == r.ID,
                       JoinType.Left,rm.MenuId == m.ID,
               })
               .Where(exp.ToExpression())
               .OrderBy((rm) => rm.ID, OrderByType.Desc)
               .Select((rm, r, m) => new { ID = rm.ID, RoleId = rm.RoleId, RoleName = r.RoleName, MenuId = rm.MenuId, MenuName = m.Name })
               .ToList();
            return list;
        }

        public void DelRoleMenu(int roleId)
        {
            db.Deleteable<B_TbRoleMenu>().With(SqlWith.RowLock).Where(it => roleId == it.RoleId).ExecuteCommand();
        }
        #endregion
    }
}
