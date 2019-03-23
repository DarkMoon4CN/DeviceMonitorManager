using ChuanYe.Utils;
using MonitorManager.Model;
using MonitorManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorManager.BLL
{
    public class AlarmInfoBLL
    {
        #region 构造单例
        private AlarmInfoBLL() { }
        private static AlarmInfoBLL _instance;
        public static AlarmInfoBLL Instance
        {
            get
            {
                return _instance ?? (_instance = new AlarmInfoBLL());
            }
        }
        #endregion

        public List<View_AlarmInfo_LocalInfo_Item> AlarmConditionByPage(View_AlarmInfo_LocalInfo_Item info, RequstPageBase page)
        {
            using (MonitorManagerEntities ef = new MonitorManagerEntities())
            {
                var where = PredicateExtensionses.True<View_AlarmInfo_LocalInfo_Item>();
                if (!string.IsNullOrEmpty(info.STCD))
                {
                    where = where.And(p => p.STCD.Contains(info.STCD));
                }
                if (!string.IsNullOrEmpty(info.NiceName))
                {
                    where = where.And(p => p.NiceName.Contains(info.NiceName));
                }
                if (!string.IsNullOrEmpty(info.ItemID))
                {
                    where = where.And(p => p.ItemID.Contains(info.ItemID));
                }
                if (!string.IsNullOrEmpty(info.ItemName))
                {
                    where = where.And(p => p.ItemName.Contains(info.ItemName));
                }
                if (info.AlarmLevel>0)
                {
                    where = where.And(p => p.AlarmLevel== info.AlarmLevel);
                }
                if (page.StartTime != null)
                {
                    where = where.And(p => p.AlarmTime > page.StartTime);
                }
                if (page.EndTime != null)
                {
                    where = where.And(p => p.AlarmTime < page.EndTime);
                }

                int startRow = page.PageSize * (page.PageIndex - 1);
                var query = ef.View_AlarmInfo_LocalInfo_Item.Where(where);
                var result = query.OrderByDescending(o => o.AlarmTime)
                    .Skip(startRow).Take(page.PageSize).ToList();
                page.TotalRows = query.Count();
                return result;
            }
        }

        public int AlarmInfoCountFor24Hours(string STCD)
        {
            using (MonitorManagerEntities ef = new MonitorManagerEntities())
            {
                var where = PredicateExtensionses.True<View_AlarmInfo_LocalInfo_Item>();
                var startTime = DateTime.Now.AddHours(-24);
                var endTime = DateTime.Now;
                where = where.And(p => p.AlarmTime > startTime);
                where = where.And(p => p.AlarmTime < endTime);
                if (!string.IsNullOrEmpty(STCD))
                {
                    where = where.And(p => p.STCD==STCD);
                }
                var query = ef.View_AlarmInfo_LocalInfo_Item.Where(where);
                return  query.Count();
            }
        }

    }
}
