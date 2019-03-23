using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChuanYe.BMS.DAL.Table
{
    public class B_TbUser
    {
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }

        [SugarColumn(IsNullable = false, Length = 512 )]
        public string AccountName { get; set; }

        [SugarColumn(IsNullable = false, Length = 512)]
        public string Password { get; set; }

        [SugarColumn( Length = 512)]
        public string RealName { get; set; }

        [SugarColumn(IsNullable = false, Length = 512)]
        public string MobilePhone { get; set; }

        [SugarColumn(IsNullable = false, Length = 512)]
        public string Email { get; set; }

        [SugarColumn(IsNullable = false)]
        public bool IsAble { get; set; }

        [SugarColumn(IsNullable = false)]
        public bool IfChangePwd { get; set; }

        [SugarColumn(IsNullable = true, Length = 512)]
        public string Description { get; set; }

        [SugarColumn(IsNullable = false, Length = 512)]
        public string Creater { get; set; }

        [SugarColumn(IsNullable = false)]
        public DateTime CreateTime { get; set; }

        [SugarColumn(IsNullable = false, Length = 512)]
        public string Updater { get; set; }

        [SugarColumn(IsNullable = false)]
        public DateTime UpdateTime { get; set; }


        [SugarColumn(IsNullable = true, Length = 512)]
        public string Position { get; set; }


        [SugarColumn(IsNullable = true, Length = 512)]
        public string WorkNumber { get; set; }

    }
}
