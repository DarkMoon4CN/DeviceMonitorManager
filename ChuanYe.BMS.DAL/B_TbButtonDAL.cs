using ChuanYe.BMS.DAL.Table;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChuanYe.BMS.DAL
{
    public class B_TbButtonDAL
    {
        private SqlSugarClient db = SqlSugarFactory.GetInstance(InitKeyType.SystemTable);

        #region 构造单例
        private B_TbButtonDAL() { }
        private static B_TbButtonDAL _instance;
        public static B_TbButtonDAL Instance
        {
            get
            {
                return _instance ?? (_instance = new B_TbButtonDAL());
            }
        }
        #endregion

        #region CRUD
        public int AddButton(B_TbButton info)
        {
            int id = db.Insertable(info).With(SqlWith.UpdLock).ExecuteReturnIdentity();
            return id;
        }

        public B_TbButton ButtonDetail(string  name)
        {
            return db.Queryable<B_TbButton>().Where(p => p.Name == name).First();
        }

        public List<B_TbButton> GetButtonByExist(string name)
        {
            return db.Queryable<B_TbButton>().Where(p => p.Name == name).ToList();
        }

        public B_TbButton ButtonDetail(int  id)
        {
            return db.Queryable<B_TbButton>().Where(p => p.ID==id).Single();
        }


        public List<B_TbButton> ButtonByPage(DateTime? startTime, DateTime? endTime, string keyword, ref int totalCount,
                                    string orderbyKey="ID", string descOrAsc="asc", int pageSize = 20, int pageIndex = 1)
        {
            var exp = Expressionable.Create<B_TbButton>();
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
            var list=db.Queryable<B_TbButton>()
                .Where(exp.ToExpression()).OrderBy(orderbyKey +" "+descOrAsc)
                .ToPageList(pageIndex, pageSize, ref totalCount);
            return list;
        }

        public int DelButton(List<int> ids)
        {
            return db.Deleteable<B_TbButton>().With(SqlWith.RowLock).Where(it => ids.Contains(it.ID)).ExecuteCommand();
        }

        public int UpdateButton(B_TbButton entity)
        {
            return db.Updateable<B_TbButton>(entity).With(SqlWith.UpdLock).IgnoreColumns(it => new { it.Creater, it.CreateTime }).ExecuteCommand();
        }


        public List<B_TbButton> GetButtonByIDs(List<int> ids)
        {
            return db.Queryable<B_TbButton>().Where(p => ids.Contains(p.ID)).ToList();
        }

        #endregion
    }
}
