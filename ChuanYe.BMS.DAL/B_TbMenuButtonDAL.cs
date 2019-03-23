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
    public class B_TbMenuButtonDAL
    {
        private SqlSugarClient db = SqlSugarFactory.GetInstance(InitKeyType.SystemTable);

        #region 构造单例
        private B_TbMenuButtonDAL() { }
        private static B_TbMenuButtonDAL _instance;
        public static B_TbMenuButtonDAL Instance
        {
            get
            {
                return _instance ?? (_instance = new B_TbMenuButtonDAL());
            }
        }
        #endregion

        #region CRUD
        public int AddMenuButton(B_TbMenuButton info)
        {
            int id = db.Insertable<B_TbMenuButton>(info).With(SqlWith.UpdLock).ExecuteReturnIdentity();
            return id;
        }

        public B_TbMenuButton MenuButtonDetail(int menuId, int buttonId)
        {
            return db.Queryable<B_TbMenuButton>().Where(p => p.MenuId == menuId && p.ButtonId == buttonId).First();
        }
        public B_TbMenuButton MenuButtonDetail(int id )
        {
            return db.Queryable<B_TbMenuButton>().Where(p => p.ID==id).First();
        }

        public int RemoveMenuButtonByMid(int mid)
        {
            return db.Deleteable<B_TbMenuButton>().With(SqlWith.RowLock).Where(it => mid==it.MenuId).ExecuteCommand();
        }
        public int RemoveMenuButtonByMids(List<int> mids)
        {
            return db.Deleteable<B_TbMenuButton>().With(SqlWith.RowLock).Where(it => mids.Contains(it.MenuId)).ExecuteCommand();
        }

        public List<MenuButtonEntity> MenuButtonByPage(DateTime? startTime, DateTime? endTime, string keyword, ref int totalCount, int pageSize = 20, int pageIndex = 1)
        {
            var exp = Expressionable.Create<B_TbMenuButton, B_TbMenu, B_TbButton>();
            if (startTime != null)
            {
                exp = exp.And((mb, m, b) => mb.UpdateTime > startTime);
            }
            if (endTime != null)
            {
                exp = exp.And((mb, m, b) => mb.UpdateTime < endTime);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                exp = exp.And((mb, m, b) => m.Name.Contains(keyword) || b.Name.Contains(keyword));
            }

            var list = db.Queryable<B_TbMenuButton, B_TbMenu, B_TbButton>(
               (mb, m, b) =>
               new object[]{
                   JoinType.Left,mb.MenuId == m.ID,
                   JoinType.Left,mb.ButtonId == b.ID,
               })
               .Where(exp.ToExpression())
               .OrderBy((mb)=>mb.ID,OrderByType.Desc)
               .Select((mb, m, b) => new MenuButtonEntity { ID=mb.ID,MenuId=mb.MenuId, MenuName=m.Name,ButtonId=b.ID,ButtonName=b.Name })
               .ToPageList(pageIndex, pageSize,ref totalCount);
           
            return list;

        }

        public List<B_TbMenuButton> GetMenuButtonByMid(int menuId)
        {
            return db.Queryable<B_TbMenuButton>().Where(p => p.MenuId == menuId ).ToList();
        }
        #endregion
    }
}
