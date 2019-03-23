using ChuanYe.BMS.DAL.Table;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChuanYe.BMS.DAL
{
    public class B_TbIconDAL
    {
        private SqlSugarClient db = SqlSugarFactory.GetInstance(InitKeyType.SystemTable);

        #region 构造单例
        private B_TbIconDAL() { }
        private static B_TbIconDAL _instance;
        public static B_TbIconDAL Instance
        {
            get
            {
                return _instance ?? (_instance = new B_TbIconDAL());
            }
        }
        #endregion


        #region CRUD
        public int AddIcon(B_TbIcon info)
        {
            int id = db.Insertable(info).With(SqlWith.UpdLock).ExecuteReturnIdentity();
            return id;
        }

        public B_TbIcon IconDetail(string name)
        {
            return db.Queryable<B_TbIcon>().Where(p => p.IconName == name).First();
        }

        public List<B_TbIcon> IconByPage(DateTime? startTime, DateTime? endTime, string keyword, ref int totalCount,
                               string orderbyKey = "ID", string descOrAsc = "asc", int pageSize = 20, int pageIndex = 1)
        {
            var exp = Expressionable.Create<B_TbIcon>();
            if (!string.IsNullOrEmpty(keyword))
            {
                exp = exp.And(p => p.IconName.Contains(keyword));
            }
            if (startTime != null)
            {
                exp = exp.And(p => p.UpdateTime > startTime);
            }
            if (endTime != null)
            {
                exp = exp.And(p => p.UpdateTime < endTime);
            }
            var list = db.Queryable<B_TbIcon>()
                .Where(exp.ToExpression()).OrderBy(orderbyKey + " " + descOrAsc)
                .ToPageList(pageIndex, pageSize, ref totalCount);
            return list;
        }

        public int DelIcon(List<int> ids)
        {
            return db.Deleteable<B_TbIcon>().With(SqlWith.RowLock).Where(it=> ids.Contains(it.ID)).ExecuteCommand();
        }

        public int UpdateIcon(B_TbIcon entity)
        {
             //db.Updateable<B_TbIcon>(entity).WhereColumns(it=>new{it.ID}).ExecuteCommand();
            return db.Updateable<B_TbIcon>(entity).With(SqlWith.UpdLock).IgnoreColumns(it => new { it.Creater, it.CreateTime }).ExecuteCommand();
            
        }




        #endregion
    }
}
