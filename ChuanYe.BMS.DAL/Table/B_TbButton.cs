using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChuanYe.BMS.DAL.Table
{
    public class B_TbButton
    {
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }

        [SugarColumn(IsNullable = false, Length = 512)]
        public string Name { get; set; }

        [SugarColumn(IsNullable = false, Length = 512)]
        public string Code { get; set; }


        [SugarColumn(IsNullable = true, Length = 512)]
        public string Icon { get; set; }

        [SugarColumn(IsNullable = false)]
        public int Sort { get; set; }

        [SugarColumn(IsNullable = false, Length = 512)]
        public string Creater { get; set; }

        [SugarColumn(IsNullable = false)]
        public DateTime CreateTime { get; set; }

        [SugarColumn(IsNullable = false, Length = 512)]
        public string Updater { get; set; }

        [SugarColumn(IsNullable = false)]
        public DateTime UpdateTime { get; set; }


        [SugarColumn(IsNullable = true, Length = 512)]
        public string Description { get; set; }
    }
}
