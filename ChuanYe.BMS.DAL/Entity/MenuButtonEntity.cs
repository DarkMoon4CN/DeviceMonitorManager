using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChuanYe.BMS.DAL.Entity
{
    public class MenuButtonEntity
    {
        public int ID { get; set; }
        public int MenuId { get; set; }

        public string MenuName { get; set; }
        public int ButtonId { get; set; }

        public string ButtonName { get; set; }
    }
}
