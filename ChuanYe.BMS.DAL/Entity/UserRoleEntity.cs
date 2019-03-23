using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChuanYe.BMS.DAL.Entity
{
    public class UserRoleEntity
    {

        public int ID { get; set; }

        public int UserId { get; set; }

        public string RealName { get; set; }

        public string  AccountName{get;set;}

        public string Password { get; set; }

        public string Email { get; set; }
        public string MobilePhone { get; set; }

        public bool IsAble { get; set; }

        public bool IfChangePwd { get; set; }

        public string Description { get; set; }

        public string Creater { get; set; }
        public DateTime CreateTime { get; set; }

        public string Updater { get; set; }

        public DateTime UpdateTime { get; set; }

        public string Position { get; set; }

        public string WorkNumber { get; set; }

        public int RoleId { get; set; }

        public string RoleName { get; set; }
      
    }
}
