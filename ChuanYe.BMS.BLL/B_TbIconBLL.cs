using ChuanYe.BMS.DAL;
using ChuanYe.BMS.DAL.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChuanYe.BMS.BLL
{
    public class B_TbIconBLL
    {
        #region 构造单例
        private B_TbIconBLL() { }
        private static B_TbIconBLL _instance;
        public static B_TbIconBLL Instance
        {
            get
            {
                return _instance ?? (_instance = new B_TbIconBLL());
            }
        }
        #endregion

        private B_TbIconDAL iconDAL = B_TbIconDAL.Instance;


        public int AddIcon(B_TbIcon info)
        {
            return iconDAL.AddIcon(info);
        }


        public B_TbIcon IconDetail(string name)
        {
            return iconDAL.IconDetail(name);
        }

        public List<B_TbIcon> IconByPage(DateTime? startTime, DateTime? endTime, string keyword, ref int totalCount,
                              string orderbyKey = "ID", string descOrAsc = "asc", int pageSize = 20, int pageIndex = 1)
        {

            return iconDAL.IconByPage(startTime, endTime, keyword, ref totalCount, orderbyKey, descOrAsc, pageSize, pageIndex);
        }

        public int UpdateIcon(B_TbIcon entity)
        {
            return iconDAL.UpdateIcon(entity);
        }

        public int DelIcon(List<int> ids)
        {
            return iconDAL.DelIcon(ids);
        }
    }
}
