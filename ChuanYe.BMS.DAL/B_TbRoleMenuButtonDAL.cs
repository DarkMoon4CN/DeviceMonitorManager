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
    public class B_TbRoleMenuButtonDAL
    {
        private SqlSugarClient db = SqlSugarFactory.GetInstance(InitKeyType.SystemTable);

        #region 构造单例
        private B_TbRoleMenuButtonDAL() { }
        private static B_TbRoleMenuButtonDAL _instance;
        public static B_TbRoleMenuButtonDAL Instance
        {
            get
            {
                return _instance ?? (_instance = new B_TbRoleMenuButtonDAL());
            }
        }
        #endregion

        #region CRUD
        public int AddRoleMenuButton(B_TbRoleMenuButton info)
        {
            int id = db.Insertable<B_TbRoleMenuButton>(info).With(SqlWith.UpdLock).ExecuteReturnIdentity();
            return id;
        }

        public B_TbRoleMenuButton RoleMenuButtonDetail(int roleId,int menuId, int buttonId=0)
        {
            var exp = Expressionable.Create<B_TbRoleMenuButton>();
            if (buttonId != 0)
            {
                exp = exp.And(p=>p.ButtonId == buttonId);
            }
            exp = exp.And(p => p.RoleId == roleId && p.MenuId == menuId);
            return db.Queryable<B_TbRoleMenuButton>().Where(exp.ToExpression()).First();
        }
        public B_TbRoleMenuButton RoleMenuButtonDetail(int id)
        {
            return db.Queryable<B_TbRoleMenuButton>().Where(p => p.ID==id).First();
        }

        public List<RoleMenuButtonEntity> RoleMenuButtonList(int roleId, int menuId=0)
        {
            var exp = Expressionable.Create<B_TbRoleMenuButton, B_TbButton>();
            
            if (menuId != 0)
            {
                exp = exp.And((rmb,b) => rmb .MenuId== menuId);
            }
            exp = exp.And((rmb, b) => rmb.RoleId == roleId);
            var list = db.Queryable<B_TbRoleMenuButton, B_TbButton>((rmb, b) =>
              new object[]{
                       JoinType.Left, rmb.ButtonId==b.ID
              }).Where(exp.ToExpression())
             .OrderBy((rmb) => rmb.ID, OrderByType.Desc)
             .Select((rmb, b) => new RoleMenuButtonEntity { ID = b.ID, RoleId = rmb.RoleId, Code = b.Code, Icon = b.Icon, MenuId = rmb.MenuId,ButtonId = rmb.ButtonId, ButtonName = b.Name })
             .ToList();
            return list;
        }
        public int RemoveMenuButtonByRoleId(int roleId)
        {
            return db.Deleteable<B_TbRoleMenuButton>().With(SqlWith.RowLock).Where(it => it.RoleId== roleId).ExecuteCommand();
        }

        #endregion
    }
}
