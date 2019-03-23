using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChuanYe.BMS.DAL
{
    public class SqlSugarFactory
    {
        public static string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["connStr"].ToString();

        public static InitKeyType keyType = InitKeyType.SystemTable;
        public static  SqlSugarClient GetInstance(InitKeyType myKeyType= InitKeyType.SystemTable)
        {
            keyType = myKeyType;
            return _instance;
        }
        private static SqlSugarClient _instance
        {
            get
            {
                return  new SqlSugarClient(new ConnectionConfig()
                {
                    ConnectionString = connStr,
                    DbType = DbType.SqlServer,        
                    IsAutoCloseConnection = true,       
                    InitKeyType = keyType,
                    MoreSettings = new ConnMoreSettings { IsAutoRemoveDataCache=true, IsWithNoLockQuery= true },
                });
           }
        }
       
    }
}

