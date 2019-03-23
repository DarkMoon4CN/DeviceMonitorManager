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
    public class B_TbButtonBLL
    {
        #region 构造单例
        private B_TbButtonBLL() { }
        private static B_TbButtonBLL _instance;
        public static B_TbButtonBLL Instance
        {
            get
            {
                return _instance ?? (_instance = new B_TbButtonBLL());
            }
        }

        #endregion

        private B_TbRoleMenuButtonDAL roleMenuButtonDAL = B_TbRoleMenuButtonDAL.Instance;

        private B_TbButtonDAL buttonDAL = B_TbButtonDAL.Instance;

        public List<RoleMenuButtonEntity> RoleMenuButtonList(int roleId, int menuId = 0)
        {
            return roleMenuButtonDAL.RoleMenuButtonList(roleId, menuId);
        }
        public int RemoveMenuButtonByRoleId(int roleId)
        {
            return roleMenuButtonDAL.RemoveMenuButtonByRoleId(roleId);
        }



        public List<B_TbButton> ButtonByPage(DateTime? startTime, DateTime? endTime, string keyword, ref int totalCount,
                                    string orderbyKey = "ID", string descOrAsc = "asc", int pageSize = 20, int pageIndex = 1)
        {
            return buttonDAL.ButtonByPage(startTime, endTime, keyword, ref totalCount, orderbyKey, descOrAsc, pageSize, pageIndex);
        }


        public B_TbButton ButtonDetail(string name)
        {
            return buttonDAL.ButtonDetail(name);
        }

        public List<B_TbButton> GetButtonByExist(string name)
        {
            return buttonDAL.GetButtonByExist(name);
        }


        public int AddButton(B_TbButton info)
        {
            return buttonDAL.AddButton(info);
        }

        public int UpdateButton(B_TbButton entity)
        {
            return buttonDAL.UpdateButton(entity);
        }

        public int DelButton(List<int> ids)
        {
            return buttonDAL.DelButton(ids);
        }

        public List<B_TbButton> GetButtonByIDs(List<int> ids)
        {
            return buttonDAL.GetButtonByIDs(ids);
        }

    }
}
