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
    public class B_TbMenuBLL
    {
        #region 构造单例
        private B_TbMenuBLL() { }
        private static B_TbMenuBLL _instance;
        public static B_TbMenuBLL Instance
        {
            get
            {
                return _instance ?? (_instance = new B_TbMenuBLL());
            }
        }
        #endregion

        private B_TbMenuDAL menuDAL = B_TbMenuDAL.Instance;
        private B_TbMenuButtonDAL menuButtonDAL = B_TbMenuButtonDAL.Instance;
       
        public int AddMenu(B_TbMenu info)
        {
            return menuDAL.AddMenu(info);
        }


        public B_TbMenu MenuDetail(string name)
        {
            return menuDAL.MenuDetail(name);
        }

        public List<B_TbMenu> GetMenu(string name)
        {
            return menuDAL.GetMenu(name);
        }


        public int UpdateMenu(B_TbMenu info)
        {
            return menuDAL.UpdateMenu(info);
        }


        public List<B_TbMenu> MenuByPage(DateTime? startTime, DateTime? endTime, string keyword, ref int totalCount,
                                  string orderbyKey = "ID", string descOrAsc = "asc", int pageSize = 20, int pageIndex = 1)
        {
            return menuDAL.MenuByPage(startTime,endTime,keyword,ref totalCount,orderbyKey,descOrAsc,pageSize,pageIndex);
        }

        public int DelMenu(List<int> delIDs)
        {
            return menuDAL.DelMenu(delIDs);
        }

        public int AddMenuButton(B_TbMenuButton info)
        {
            return menuButtonDAL.AddMenuButton(info);
        }

        public int RemoveMenuButtonByMid(int mid)
        {
            return menuButtonDAL.RemoveMenuButtonByMid(mid);
        }

        public int RemoveMenuButtonByMids(List<int> mids)
        {
            return menuButtonDAL.RemoveMenuButtonByMids(mids);
        }

        public List<B_TbMenuButton> GetMenuButtonByMid(int mid)
        {
            return menuButtonDAL.GetMenuButtonByMid(mid);
        }

        public List<MenuButtonEntity> MenuButtonByPage(DateTime? startTime, DateTime? endTime, string keyword, ref int totalCount, int pageSize = 20, int pageIndex = 1)
        {
           return menuButtonDAL.MenuButtonByPage(startTime, endTime, keyword, ref totalCount, pageSize, pageIndex);
        }
    }
}
