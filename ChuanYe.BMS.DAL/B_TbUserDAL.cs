using ChuanYe.BMS.DAL.Entity;
using ChuanYe.BMS.DAL.Table;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChuanYe.BMS.DAL
{
    public class B_TbUserDAL
    {
        private SqlSugarClient db = SqlSugarFactory.GetInstance(InitKeyType.SystemTable);

        #region 构造单例
        private B_TbUserDAL() { }
        private static B_TbUserDAL _instance;
        public static B_TbUserDAL Instance
        {
            get
            {
                return _instance ?? (_instance = new B_TbUserDAL());
            }
        }
        #endregion

        #region CRUD
        public int AddUser(B_TbUser info)
        {
            int  id= db.Insertable(info).With(SqlWith.UpdLock).ExecuteReturnIdentity();

            return id;
        }

        public B_TbUser UserDetail(string accountName)
        {
            return db.Queryable<B_TbUser>().Where(p => p.AccountName == accountName).First();
        }

        public B_TbUser UserDetail(int  id)
        {
            return db.Queryable<B_TbUser>().Where(p => p.ID==id).Single();
        }

        public int DelUser(List<int> ids)
        {
            return db.Deleteable<B_TbUser>().With(SqlWith.RowLock).Where(it => ids.Contains(it.ID)).ExecuteCommand();
        }

        public B_TbUser UserDetail(string accountName,string password)
        {
            return db.Queryable<B_TbUser>().Where(p => p.AccountName == accountName && p.Password== password).First();
        }

        public List<UserRoleEntity> UsersByPage(B_TbUser info,DateTime? startTime, DateTime? endTime, int isAble,
                              ref int totalCount, 
                              int pageSize = 20, int pageIndex = 1)
        {
            var exp = Expressionable.Create<B_TbUser,B_TbUserRole, B_TbRole>();
            if (!string.IsNullOrEmpty(info.AccountName))
            {
                exp = exp.And((u, ur, r) => u.AccountName.Contains(info.AccountName));
            }
            if (!string.IsNullOrEmpty(info.RealName))
            {
                exp = exp.And((u, ur, r) => u.RealName.Contains(info.RealName));
            }
            if (isAble != -1)
            {
                bool temp=isAble==1?true:false;
                exp = exp.And((u, ur, r) => u.IsAble==temp);
            }
            if (startTime != null)
            {
                exp = exp.And((u, ur, r) => u.CreateTime > startTime);
            }
            if (endTime != null)
            {
                exp = exp.And((u, ur, r) => u.CreateTime < endTime);
            }
            var list = db.Queryable<B_TbUser, B_TbUserRole, B_TbRole>((u, ur, r) =>
              new object[]{
                       JoinType.Left, u.ID==ur.UserId,
                       JoinType.Left, ur.RoleId==r.ID
                 })
                .Where(exp.ToExpression()).OrderBy((u, ur, r) => u.CreateTime, OrderByType.Asc)
                .Select((u, ur, r) => new UserRoleEntity() {
                    ID = u.ID,
                    AccountName = u.AccountName,
                    RealName=u.RealName,
                    Creater = u.Creater,
                    CreateTime = u.CreateTime,
                    Description = u.Description,
                    Email = u.Email,
                    IfChangePwd = u.IfChangePwd,
                    IsAble = u.IsAble,
                    MobilePhone = u.MobilePhone,
                    Password = null,
                    Position=u.Position,
                    RoleId=r.ID,
                    RoleName=r.RoleName,
                    Updater=u.Updater,
                    UpdateTime=u.UpdateTime,
                    WorkNumber=u.WorkNumber,
                })
                .ToPageList(pageIndex, pageSize, ref totalCount);
            return list;
        }

        public int UpdateUser(B_TbUser info)
        {
            return db.Updateable<B_TbUser>(info).With(SqlWith.UpdLock).IgnoreColumns(it => new {
                it.Creater,
                it.CreateTime,
                it.Password,
                it.WorkNumber,
            }).ExecuteCommand();
        }


        #endregion
    }
}
