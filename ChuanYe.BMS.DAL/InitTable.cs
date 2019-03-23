using ChuanYe.BMS.DAL.Table;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChuanYe.BMS.DAL
{
    public class InitCreateTable
    {
        private  SqlSugarClient db= SqlSugarFactory.GetInstance(InitKeyType.Attribute);


        #region 构造单例
        private InitCreateTable() { }
        private static InitCreateTable _instance;
        public static InitCreateTable Instance
        {
            get
            {
                return _instance ?? (_instance = new InitCreateTable());
            }
        }
        #endregion

        public void  Init()
        {
            db.CodeFirst.InitTables(typeof(B_TbUser),typeof(B_TbMenu),typeof(B_TbRole),typeof(B_TbButton),
                                    typeof(B_TbRoleMenu), typeof(B_TbMenuButton), typeof(B_TbRoleMenuButton),typeof(B_TbUserRole),typeof(B_TbIcon));
        }
     
    }
}
