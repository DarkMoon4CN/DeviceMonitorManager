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
    public class YY_HS_DATA_AUTO_BLL
    {
        #region 构造单例
        private YY_HS_DATA_AUTO_BLL() { }
        private static YY_HS_DATA_AUTO_BLL _instance;
        public static YY_HS_DATA_AUTO_BLL Instance
        {
            get
            {
                return _instance ?? (_instance = new YY_HS_DATA_AUTO_BLL());
            }
        }
        #endregion


        public int AddYY_HS_DATA_AUTO(YY_HS_DATA_AUTO info)
        {
            using (MonitorManagerEntities ef = new MonitorManagerEntities())
            {
                ef.YY_HS_DATA_AUTO.Add(info);
                ef.SaveChanges();
                return 1;
            }
        }

        public List<YY_HS_DATA_AUTO> YY_HS_DATA_AUTOByPage(YY_HS_DATA_AUTO info, RequstPageBase page)
        {
            using (MonitorManagerEntities ef = new MonitorManagerEntities())
            {
                var where = PredicateExtensionses.True<YY_HS_DATA_AUTO>();
                if (!string.IsNullOrEmpty(info.STCD))
                {
                    where = where.And(p => p.STCD.Contains(info.STCD));
                }
                if (!string.IsNullOrEmpty(info.ItemID))
                {
                    where = where.And(p => p.ItemID.Contains(info.ItemID));
                }
                if (page.StartTime != null)
                {
                    where = where.And(p => p.TM > page.StartTime);
                }
                if (page.EndTime != null)
                {
                    where = where.And(p => p.TM < page.EndTime);
                }

                int startRow = page.PageSize * (page.PageIndex - 1);
                var query = ef.YY_HS_DATA_AUTO.Where(where);
                var result = query.OrderBy(o => o.TM)
                    .Skip(startRow).Take(page.PageSize).ToList();
                page.TotalRows = query.Count();
                return result;
            }
        }


    }
}
