using ChuanYe.BMS.DAL;
using ChuanYe.BMS.DAL.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChuanYe.BMS.BLL
{
    public class B_TbRoleBLL
    {
        #region 构造单例
        private B_TbRoleBLL() { }
        private static B_TbRoleBLL _instance;
        public static B_TbRoleBLL Instance
        {
            get
            {
                return _instance ?? (_instance = new B_TbRoleBLL());
            }
        }

        #endregion

        private B_TbRoleDAL roleDAL = B_TbRoleDAL.Instance;

        private B_TbRoleMenuDAL roleMenuDAL = B_TbRoleMenuDAL.Instance;

        public int AddRoleMenu(B_TbRoleMenu info)
        {
           return roleMenuDAL.AddRoleMenu(info);
        }

        public B_TbRole RoleDetail(string name)
        {
            return roleDAL.RoleDetail(name);
        }



        public List<B_TbRole> RoleByPage(DateTime? startTime, DateTime? endTime, string keyword, ref int totalCount,
                              string orderbyKey = "ID", string descOrAsc = "asc", int pageIndex = 1, int pageSize = 20)
        {
            return roleDAL.RoleByPage(startTime, endTime, keyword, ref totalCount, orderbyKey, descOrAsc, pageIndex,pageSize);
        }

        public int AddRole(B_TbRole info)
        {
            return roleDAL.AddRole(info);
        }

        public int UpdateRole(B_TbRole info)
        {
            return roleDAL.UpdateRole(info);
        }

        public int DelRole(List<int> ids)
        {
            return roleDAL.DelRole(ids);
        }

        public void DelRoleMenuByRoleID(int roleId)
        {
             roleMenuDAL.DelRoleMenu(roleId);
        }

      
    }
}
