using ChuanYe.BMS.DAL.Table;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChuanYe.BMS.DAL
{
    public class B_TbMenuDAL
    {
        private SqlSugarClient db = SqlSugarFactory.GetInstance(InitKeyType.SystemTable);

        #region 构造单例
        private B_TbMenuDAL() { }
        private static B_TbMenuDAL _instance;
        public static B_TbMenuDAL Instance
        {
            get
            {
                return _instance ?? (_instance = new B_TbMenuDAL());
            }
        }
        #endregion

        #region CRUD

        public int AddMenu(B_TbMenu info)
        {
            int id = db.Insertable(info).With(SqlWith.UpdLock).ExecuteReturnIdentity();

            return id;
        }

        public B_TbMenu MenuDetail(string  name)
        {
            return db.Queryable<B_TbMenu>().Where(p => p.Name == name).First();
        }

        public List<B_TbMenu> GetMenu(string name)
        {
            return db.Queryable<B_TbMenu>().Where(p => p.Name == name).ToList();
        }



        public B_TbMenu MenuDetail(int  id)
        {
            return db.Queryable<B_TbMenu>().Where(p => p.ID==id).Single();
        }

        public List<B_TbMenu>MenuByPage(DateTime? startTime, DateTime? endTime, string keyword, ref int totalCount,
                                    string orderbyKey = "ID", string descOrAsc = "asc", int pageSize = 20, int pageIndex = 1)
        {
            var exp = Expressionable.Create<B_TbMenu>();
            if (!string.IsNullOrEmpty(keyword))
            {
                exp = exp.And(p => p.Name.Contains(keyword));
            }
            if (startTime != null)
            {
                exp = exp.And(p => p.UpdateTime > startTime);
            }
            if (endTime != null)
            {
                exp = exp.And(p => p.UpdateTime < endTime);
            }
            var list = db.Queryable<B_TbMenu>()
                .Where(exp.ToExpression()).OrderBy(orderbyKey + " " + descOrAsc)
                .ToPageList(pageIndex, pageSize, ref totalCount);
            return list;
        }

        public int DelMenu(List<int> ids)
        {
            return db.Deleteable<B_TbMenu>().With(SqlWith.RowLock).Where(it => ids.Contains(it.ID)).ExecuteCommand();
        }

        public int UpdateMenu(B_TbMenu entity)
        {
            return db.Updateable<B_TbMenu>(entity).With(SqlWith.UpdLock).IgnoreColumns(it => new { it.Creater, it.CreateTime,it.MenuType }).ExecuteCommand();

        }

        #endregion
    }
}
