using ChuanYe.Utils;
using EntityFramework.Extensions;
using MonitorManager.Model;
using MonitorManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace MonitorManager.BLL
{
    public class ItemBLL
    {

        #region 构造单例
        private ItemBLL() { }
        private static ItemBLL _instance;
        public static ItemBLL Instance
        {
            get
            {
                return _instance ?? (_instance = new ItemBLL());
            }
        }
        #endregion


        public List<View_LocaInfo_YY_RTU_ITEM> LocalInfoItemByPage(View_LocaInfo_YY_RTU_ITEM info, RequstPageBase page)
        {
            using (MonitorManagerEntities ef = new MonitorManagerEntities())
            {
                var where = PredicateExtensionses.True<View_LocaInfo_YY_RTU_ITEM>();
                if (!string.IsNullOrEmpty(info.STCD))
                {
                    where = where.And(p => p.STCD == info.STCD);
                }
                if (!string.IsNullOrEmpty(info.ItemName))
                {
                    where = where.And(p => p.ItemName.Contains(info.ItemName));
                }
                int startRow = page.PageSize * (page.PageIndex - 1);
                var query = ef.View_LocaInfo_YY_RTU_ITEM.Where(where);
                var result = query.OrderByDescending(o => o.STCD).ThenByDescending(t => t.ItemIndex)
                    .Skip(startRow).Take(page.PageSize).ToList();
                page.TotalRows = query.Count();
                return result;
            }
        }

        public List<View_YY_RTU_ITEM_Alarm> ItemAlarmByPage(View_YY_RTU_ITEM_Alarm info, RequstPageBase page)
        {

            using (MonitorManagerEntities ef = new MonitorManagerEntities())
            {
                var where = PredicateExtensionses.True<View_YY_RTU_ITEM_Alarm>();
                if (!string.IsNullOrEmpty(info.ItemName))
                {
                    where = where.And(p => p.ItemName.Contains(info.ItemName));
                }
                int startRow = page.PageSize * (page.PageIndex - 1);
                var query = ef.View_YY_RTU_ITEM_Alarm.Where(where);
                var result = query.OrderByDescending(o => o.TempID).ThenByDescending(t => t.ItemName)
                    .Skip(startRow).Take(page.PageSize).ToList();
                page.TotalRows = query.Count();
                return result;
            }
        }

        public List<ElementsChartsInfo_Tab> GetElementsChartsInfo(List<string> itemIDs)
        {
            using (MonitorManagerEntities ef = new MonitorManagerEntities())
            {
                var where = PredicateExtensionses.True<ElementsChartsInfo_Tab>();
                where = where.And(p => itemIDs.Contains(p.ItemID));
                return  ef.ElementsChartsInfo_Tab.Where(where).ToList();
            }
        }





        public List<View_YY_RTU_ITEM> ItemByPage(View_YY_RTU_ITEM info, RequstPageBase page)
        {

            using (MonitorManagerEntities ef = new MonitorManagerEntities())
            {
                var where = PredicateExtensionses.True<View_YY_RTU_ITEM>();
                if (!string.IsNullOrEmpty(info.ItemName))
                {
                    where = where.And(p => p.ItemName.Contains(info.ItemName));
                }
                int startRow = page.PageSize * (page.PageIndex - 1);
                var query = ef.View_YY_RTU_ITEM.Where(where);
                var result = query.OrderByDescending(o => o.ItemIndex).ThenByDescending(t => t.ItemName)
                    .Skip(startRow).Take(page.PageSize).ToList();
                page.TotalRows = query.Count();
                return result;
            }

           
        }

        public List<View_YY_RTU_ItemType> ItemTypeByPage(View_YY_RTU_ItemType info, RequstPageBase page)
        {
            using (MonitorManagerEntities ef = new MonitorManagerEntities())
            {
                var where = PredicateExtensionses.True<View_YY_RTU_ItemType>();
                if (!string.IsNullOrEmpty(info.ItemName))
                {
                    where = where.And(p => p.ItemName.Contains(info.ItemName));
                }
                if(!string.IsNullOrEmpty(info.ItemID))
                {
                    where = where.And(p => p.ItemID == info.ItemID);
                }
                int startRow = page.PageSize * (page.PageIndex - 1);
                var query = ef.View_YY_RTU_ItemType.Where(where);
                var result = query.OrderByDescending(o => o.ItemID).ThenBy(t => t.ItemIndex)
                    .Skip(startRow).Take(page.PageSize).ToList();
                page.TotalRows = query.Count();
                return result;
            }
        }

        public List<YY_RTU_ITEM> GetALLItem()
        {
           
            using (MonitorManagerEntities ef = new MonitorManagerEntities())
            {
                return ef.YY_RTU_ITEM.ToList();
            }
        }


        public List<View_LocaInfo_YY_RTU_ITEM> GetAll(string stcd)
        {
            using (MonitorManagerEntities ef = new MonitorManagerEntities())
            {
                var where = PredicateExtensionses.True<View_LocaInfo_YY_RTU_ITEM>();
                if (!string.IsNullOrEmpty(stcd))
                {
                    where = where.And(p => p.STCD == stcd);
                }
             
                var query = ef.View_LocaInfo_YY_RTU_ITEM.Where(where);
                var result = query.OrderByDescending(o => o.STCD).ThenByDescending(t => t.ItemIndex)
                    .ToList();
                return result;
            }
        }

        public List<View_LocaInfo_YY_RTU_ITEM> GetItem(string stcd,string itemTypeID,List<string> itemIDs)
        {
            using (MonitorManagerEntities ef = new MonitorManagerEntities())
            {
                var where = PredicateExtensionses.True<View_LocaInfo_YY_RTU_ITEM>();
                if (!string.IsNullOrEmpty(stcd))
                {
                    where = where.And(p => p.STCD == stcd);
                }
                if (!string.IsNullOrEmpty(itemTypeID))
                {
                    where = where.And(p => p.ItemTypeID == itemTypeID);
                }
                if (itemIDs != null && itemIDs.Count > 0)
                {
                    where = where.And(p => itemIDs.Contains(p.ItemID));
                }
                var query = ef.View_LocaInfo_YY_RTU_ITEM.Where(where);
                var result = query.OrderByDescending(o => o.STCD).ThenByDescending(t => t.ItemIndex)
                    .ToList();
                return result;
            }
        }

        public int  DelItem(List<string> delIDs)
        {
            try
            {
                using (MonitorManagerEntities ef = new MonitorManagerEntities())
                {
                    //删除项目与元素关系表
                    ef.YY_RTU_BI.Where(p => delIDs.Contains(p.ItemID)).Delete();
                    //删除分类与元素关系表
                    ef.YY_ITEM_TI.Where(p => delIDs.Contains(p.ItemID)).Delete();
                    //删除元素表
                    ef.YY_RTU_ITEM.Where(p => delIDs.Contains(p.ItemID)).Delete(); 
                }
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public List<YY_RTU_ITEM> GetItem(string itemID)
        {
            using (MonitorManagerEntities ef = new MonitorManagerEntities())
            {
                var result = ef.YY_RTU_ITEM.Where(p => p.ItemID == itemID).ToList();
                return result;
            }
                
        }
        
        public int UpdateItem(YY_RTU_ITEM info, ElementsChartsInfo_Tab rel)
        {
            try
            {
                using (MonitorManagerEntities ef = new MonitorManagerEntities())
                {
                    ef.YY_RTU_ITEM.Where(p => info.ItemID == p.ItemID).Update(
                           u => new YY_RTU_ITEM
                           {
                               Units = info.Units,
                               ItemName = info.ItemName,
                               ItemCode = info.ItemCode,
                               PlusOrMinus = info.PlusOrMinus,
                               ItemInteger = info.ItemInteger,
                               ItemDecimal = info.ItemDecimal,
                           }
                        );
                    if (rel != null)
                    {
                        var exist = ef.ElementsChartsInfo_Tab.Where(p => info.ItemID == p.ItemID).ToList();
                        if (exist.Count > 0)
                        {
                            ef.ElementsChartsInfo_Tab.Where(p => info.ItemID == p.ItemID).Update(
                                    u => new ElementsChartsInfo_Tab
                                    {
                                        AlarmAreaMin = rel.AlarmAreaMin,
                                        AlarmAreaMax = rel.AlarmAreaMax,
                                        DisplayAreaMin = rel.DisplayAreaMin,
                                        DisplayAreaMax = rel.DisplayAreaMax,
                                    }
                                 );
                        }
                        else
                        {
                            ef.ElementsChartsInfo_Tab.Add(new ElementsChartsInfo_Tab()
                            {
                                AlarmAreaMin = rel.AlarmAreaMin,
                                AlarmAreaMax = rel.AlarmAreaMax,
                                DisplayAreaMin = rel.DisplayAreaMin,
                                DisplayAreaMax = rel.DisplayAreaMax,
                                ItemID = rel.ItemID,
                            });
                            ef.SaveChanges();
                        }
                    }

                }
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int AddItem(YY_RTU_ITEM info, ElementsChartsInfo_Tab rel)
        {
            using (MonitorManagerEntities ef = new MonitorManagerEntities())
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    try
                    {
                        //实体表
                        ef.YY_RTU_ITEM.Add(info);
                        
                        ef.Entry<YY_RTU_ITEM>(info).State = System.Data.Entity.EntityState.Added;

                        //清理告警旧数据
                        var exist=ef.ElementsChartsInfo_Tab.Where(p => info.ItemID == p.ItemID).ToList();
                        foreach (var item in exist)
                        {
                            ef.ElementsChartsInfo_Tab.Where(p=>p.ItemID==item.ItemID).Delete();
                        }
                        //增加告警表数据
                        if (rel != null)
                        {
                            ef.ElementsChartsInfo_Tab.Add(rel);
                            ef.Entry<ElementsChartsInfo_Tab>(rel).State = System.Data.Entity.EntityState.Added;
                        }
                        ef.SaveChanges();
                        //提交事务
                        transaction.Complete();
                    }
                    catch (Exception ex)
                    {
                        return 0;
                    }
                }
            }
            return 1;
        }

        public List<YY_ITEMTYPE> GetItemType()
        {
            using (MonitorManagerEntities ef = new MonitorManagerEntities())
            {
                return ef.YY_ITEMTYPE.OrderByDescending(o => o.ItemTypeIndex).ToList();
            }
        }

        public int AddItemRel(YY_ITEM_TI info)
        {
            using (MonitorManagerEntities ef = new MonitorManagerEntities())
            {
                ef.YY_ITEM_TI.Where(p => p.ItemID == info.ItemID && p.ItemTypeID == info.ItemTypeID).Delete();
                ef.YY_ITEM_TI.Add(info);
                ef.SaveChanges();
                return 1;
            }
        }

        public int DelItemRel(string itemID,string itemTypeID)
        {
            using (MonitorManagerEntities ef = new MonitorManagerEntities())
            {
                return ef.YY_ITEM_TI.Where(p=>p.ItemID==itemID && p.ItemTypeID==itemTypeID).Delete();
            }
        }
    }
}
