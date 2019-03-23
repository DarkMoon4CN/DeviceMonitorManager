using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChuanYe.BMS.DAL.Entity
{
    public class RoleMenuButtonEntity
    {
        public int ID { get; set; }
        public int RoleId { get; set; }

        public string Code { get; set; }

        public string Icon { get; set; }

        public int MenuId { get; set; }

        public int ButtonId { get; set; }

        /// <summary>
        /// button名字
        /// </summary>
        public string ButtonName { get; set; }

    }
}
