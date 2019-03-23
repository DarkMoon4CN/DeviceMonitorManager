using ChuanYe.BMS.DAL.Table;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChuanYe.BMS.DAL
{
    public class B_TbRoleDAL
    {
        private SqlSugarClient db = SqlSugarFactory.GetInstance(InitKeyType.SystemTable);

        #region 构造单例
        private B_TbRoleDAL() { }
        private static B_TbRoleDAL _instance;
        public static B_TbRoleDAL Instance
        {
            get
            {
                return _instance ?? (_instance = new B_TbRoleDAL());
            }
        }
        #endregion

        #region CRUD
        public int AddRole(B_TbRole info)
        {
            int id = db.Insertable(info).With(SqlWith.UpdLock).ExecuteReturnIdentity();
            return id;
        }

        public B_TbRole RoleDetail(string  name)
        {
            return db.Queryable<B_TbRole>().Where(p => p.RoleName == name).First();
        }

        public B_TbRole RoleDetail(int  id)
        {
            return db.Queryable<B_TbRole>().Where(p => p.ID==id).Single();
        }

        public List<B_TbRole> RoleByPage(DateTime? startTime, DateTime? endTime, string keyword, ref int totalCount,
                               string orderbyKey = "ID", string descOrAsc = "asc", int pageIndex = 1, int pageSize = 20)
        {
            var exp = Expressionable.Create<B_TbRole>();
            if (!string.IsNullOrEmpty(keyword))
            {
                exp = exp.And(p => p.RoleName.Contains(keyword));
            }
            if (startTime != null)
            {
                exp = exp.And(p => p.UpdateTime > startTime);
            }
            if (endTime != null)
            {
                exp = exp.And(p => p.UpdateTime < endTime);
            }
            var list = db.Queryable<B_TbRole>()
                .Where(exp.ToExpression()).OrderBy(orderbyKey + " " + descOrAsc)
                .ToPageList(pageIndex, pageSize, ref totalCount);
            return list;
        }

        public int DelRole(List<int> ids)
        {
            return db.Deleteable<B_TbRole>().With(SqlWith.RowLock).Where(it => ids.Contains(it.ID)).ExecuteCommand();
        }

        public int UpdateRole(B_TbRole info)
        {
            return db.Updateable<B_TbRole>(info).With(SqlWith.UpdLock).IgnoreColumns(it => new { it.Creater, it.CreateTime }).ExecuteCommand();
        }


        #endregion
    }
}
