using ChuanYe.Utils;
using EntityFramework.Extensions;
using MonitorManager.Model;
using MonitorManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorManager.BLL
{
    public class AlarmConditionBLL
    {
        #region 构造单例
        private AlarmConditionBLL() { }
        private static AlarmConditionBLL _instance;
        public static AlarmConditionBLL Instance
        {
            get
            {
                return _instance ?? (_instance = new AlarmConditionBLL());
            }
        }
        #endregion

        public List<View_Alarm_LocalInfo_Item> AlarmConditionByPage(View_Alarm_LocalInfo_Item info, RequstPageBase page)
        {
            using (MonitorManagerEntities ef = new MonitorManagerEntities())
            {
                var where = PredicateExtensionses.True<View_Alarm_LocalInfo_Item>();
                if (!string.IsNullOrEmpty(info.STCD))
                {
                    where = where.And(p => p.STCD.Contains(info.STCD));
                }
                if (!string.IsNullOrEmpty(info.NiceName))
                {
                    where = where.And(p => p.NiceName.Contains(info.NiceName));
                }
                int startRow = page.PageSize * (page.PageIndex - 1);
                var query = ef.View_Alarm_LocalInfo_Item.Where(where);
                var result = query.OrderByDescending(o => o.TempID)
                    .Skip(startRow).Take(page.PageSize).ToList();
                page.TotalRows = query.Count();
                return result;
            }
        }



        public int AddAlarmCondition(AlarmCondition_Tab info)
        {
            try
            {
                using (MonitorManagerEntities ef = new MonitorManagerEntities())
                {
                    ef.AlarmCondition_Tab.Add(info);
                    ef.SaveChanges();
                    return 1;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public int UpDataAlarmCondition(AlarmCondition_Tab info)
        {
            try
            {
                using (MonitorManagerEntities ef = new MonitorManagerEntities())
                {
                    ef.AlarmCondition_Tab.Where(p => p.ItemID == info.ItemID 
                                                         && p.STCD == info.STCD 
                                                         && p.AlarmLevel==info.AlarmLevel).Update(
                                u => new AlarmCondition_Tab
                                {
                                     Condition=info.Condition,
                                     DATAVALUE=info.DATAVALUE,
                                     //AlarmAreaMin = info.AlarmAreaMin,
                                     //AlarmAreaMax = info.AlarmAreaMax,
                                     //DisplayAreaMin = info.DisplayAreaMin,
                                     //DisplayAreaMax = info.DisplayAreaMax,
                                }
                             );
                    return 1;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public List<AlarmCondition_Tab> GetAlarmCondition(string STCD, string itemID, int alarmLevel)
        {
            using (MonitorManagerEntities ef = new MonitorManagerEntities())
            {
                var where = PredicateExtensionses.True<AlarmCondition_Tab>();
                if (!string.IsNullOrEmpty(STCD))
                {
                    where = where.And(p => p.STCD.Contains(STCD));
                }
                if (!string.IsNullOrEmpty(itemID))
                {
                    where = where.And(p => p.ItemID.Contains(itemID));
                }
                if (alarmLevel>0)
                {
                    where = where.And(p => p.AlarmLevel==alarmLevel);
                }
                return ef.AlarmCondition_Tab.Where(where).ToList();
            }
        }

        public int DelAlarmCondition(AlarmCondition_Tab info)
        {
            try
            {
                using (MonitorManagerEntities ef = new MonitorManagerEntities())
                {
                     ef.AlarmCondition_Tab.Where(p => p.ItemID == info.ItemID
                                                         && p.STCD == info.STCD
                                                         && p.AlarmLevel == info.AlarmLevel).Delete();
                    return 1;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
          
        }

        public int DelAlarmCondition(string stcd)
        {
            try
            {
                using (MonitorManagerEntities ef = new MonitorManagerEntities())
                {
                    return ef.AlarmCondition_Tab.Where(p =>p.STCD == stcd).Delete();
                }
            }
            catch (Exception ex)
            {
                return 0;
            }

        }




    }
}
