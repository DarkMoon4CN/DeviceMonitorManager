using ChuanYe.Utils;
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
    public class YY_DATA_AUTO_BLL
    {

        #region 构造单例
        private YY_DATA_AUTO_BLL() { }
        private static YY_DATA_AUTO_BLL _instance;
        public static YY_DATA_AUTO_BLL Instance
        {
            get
            {
                return _instance ?? (_instance = new YY_DATA_AUTO_BLL());
            }
        }
        #endregion

        public List<View_YY_DATA_AUTO> Page(View_YY_DATA_AUTO info, RequstPageBase page)
        {
            using (MonitorManagerEntities ef = new MonitorManagerEntities())
            {
                var where = PredicateExtensionses.True<View_YY_DATA_AUTO>();

                if (!string.IsNullOrEmpty(info.STCD))
                {
                    where = where.And(p => p.STCD == info.STCD);
                }
                if (!string.IsNullOrEmpty(info.ItemName))
                {
                    where = where.And(p => p.ItemName.Contains(info.ItemName));
                }
                if (!string.IsNullOrEmpty(info.ItemID))
                {
                    where = where.And(p => p.ItemID == info.ItemID);
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
                var query = ef.View_YY_DATA_AUTO.Where(where);
                var result = query.OrderByDescending(o => o.TM)
                    .Skip(startRow).Take(page.PageSize).ToList();
                page.TotalRows = query.Count();
                return result;
            }
        }


        public List<View_YY_DATA_AUTO> Page2(View_YY_DATA_AUTO info, RequstPageBase page)
        {
            using (MonitorManagerEntities ef = new MonitorManagerEntities())
            {
                var where = PredicateExtensionses.True<YY_DATA_AUTO>();
                if (!string.IsNullOrEmpty(info.STCD))
                {
                    where = where.And(p => p.STCD == info.STCD);
                }
                if (!string.IsNullOrEmpty(info.ItemID))
                {
                    where = where.And(p => p.ItemID == info.ItemID);
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
                var query = ef.YY_DATA_AUTO.Where(where);
                var data = query.OrderByDescending(o => o.TM).Skip(startRow).Take(page.PageSize).ToList();
                var linq = from a in data
                           join b in ef.YY_RTU_Basic on a.STCD equals b.STCD
                           into c
                           from d in c.DefaultIfEmpty()
                           join e in ef.YY_RTU_ITEM on a.ItemID equals e.ItemID
                           into f
                           from g in f.DefaultIfEmpty()
                           select new View_YY_DATA_AUTO
                           {
                               STCD=a.STCD,
                               ItemID=g.ItemID,
                               TM=a.TM,
                               DOWNDATE=a.DOWNDATE,
                               NFOINDEX=a.NFOINDEX,
                               DATAVALUE=a.DATAVALUE,
                               CorrectionVALUE=a.CorrectionVALUE,
                               DATATYPE=a.DATATYPE,
                               STTYPE=a.STTYPE,
                               NiceName=d.NiceName,
                               ItemName=g.ItemName,
                               Units=g.Units,
                               ItemCode=g.ItemCode,
                               ItemDecimal=g.ItemDecimal,
                               ItemInteger=g.ItemInteger,
                               PlusOrMinus=g.PlusOrMinus
                           };
                page.TotalRows = query.Count();
                return linq.ToList();
            }
        }


        public List<YY_DATA_AUTOTabSearchResponse> GetDataForOne(string stcd, List<string> itemIDs, string itemTypeID, DateTime? startTime,DateTime? endTime)
        {
            using (MonitorManagerEntities ef = new MonitorManagerEntities())
            {
                var where = PredicateExtensionses.True<View_LocaInfo_YY_RTU_ITEM>();
                if (!string.IsNullOrEmpty(stcd))
                {
                    where = where.And(p => p.STCD == stcd);
                }
                if (itemIDs!=null && itemIDs.Count >0)
                {
                    where = where.And(p => itemIDs.Contains(p.ItemID));
                }
                if (!string.IsNullOrEmpty(itemTypeID))
                {
                    where = where.And(p => p.ItemTypeID==itemTypeID);
                }
                var data = ef.View_LocaInfo_YY_RTU_ITEM.Where(where);
                var linq = from a in data
                           join b in ef.YY_DATA_AUTO on new { a.STCD, a.ItemID } equals new { b.STCD, b.ItemID }
                           into c
                           from d in c.DefaultIfEmpty()
                           select new YY_DATA_AUTOTabSearchResponse
                           {
                               STCD= a.STCD,
                               ItemID=  a.ItemID,
                               ItemName= a.ItemName,
                               ItemTypeID=a.ItemTypeID,
                               ItemTypeName=a.ItemType,
                               DATAVALUE= d.DATAVALUE,
                               TM=d.TM,
                               Units=a.Units,
                               ItemTypeIndex=a.ItemTypeIndex,
                               ItemIndex = a.ItemIndex,
                               //AlarmsLevels 字段由外部逻辑二次填充
                           };
                if (startTime != null && startTime == endTime)
                {
                    linq = linq.Where(p => p.TM == startTime);
                }
                else
                {
                    if (startTime != null)
                    {
                        linq = linq.Where(p => p.TM >= startTime);
                    }
                    if (endTime != null)
                    {
                        linq = linq.Where(p => p.TM <= endTime);
                    }
                }
                return linq.OrderBy(o=>o.ItemIndex).ToList();
            }
        }


        public List<YY_DATA_AUTOTabSearchResponse> GetLastDataByTM(string stcd, List<string> itemIDs, string itemTypeID, DateTime? endTIme)
        {
            using (MonitorManagerEntities ef = new MonitorManagerEntities())
            {
                var where = PredicateExtensionses.True<View_LocaInfo_YY_RTU_ITEM>();
                if (!string.IsNullOrEmpty(stcd))
                {
                    where = where.And(p => p.STCD == stcd);
                }
                if (itemIDs != null && itemIDs.Count > 0)
                {
                    where = where.And(p => itemIDs.Contains(p.ItemID));
                }
                if (!string.IsNullOrEmpty(itemTypeID))
                {
                    where = where.And(p => p.ItemTypeID == itemTypeID);
                }
                var data = ef.View_LocaInfo_YY_RTU_ITEM.Where(where);
                var linq = from a in data
                           join b in ef.YY_DATA_AUTO on new { a.STCD, a.ItemID } equals new { b.STCD, b.ItemID }
                           into c
                           let d = c.Where(p=>p.TM <=endTIme).OrderByDescending(o => o.TM).FirstOrDefault()
                           select new YY_DATA_AUTOTabSearchResponse
                           {
                               STCD = a.STCD,
                               ItemID = a.ItemID,
                               ItemName = a.ItemName,
                               ItemTypeID = a.ItemTypeID,
                               ItemTypeName = a.ItemType,
                               DATAVALUE = d.DATAVALUE,
                               TM = d.TM,
                               Units = a.Units,
                               ItemTypeIndex = a.ItemTypeIndex,
                               ItemIndex = a.ItemIndex
                               //AlarmsLevels 字段由外部逻辑二次填充
                           };
                return linq.ToList();
            }
        }


        public List<YY_DATA_AUTOTabSearchResponse> GetDataForOneByPage(string stcd, List<string> itemIDs, string itemTypeID,RequstPageBase page)
        {
            using ( MonitorManagerEntities ef = new MonitorManagerEntities())
            {
                var where = PredicateExtensionses.True<View_LocaInfo_YY_RTU_ITEM>();
                if (!string.IsNullOrEmpty(stcd))
                {
                    where = where.And(p => p.STCD == stcd);
                }
                if (itemIDs != null && itemIDs.Count > 0)
                {
                    where = where.And(p => itemIDs.Contains(p.ItemID));
                }
                if (!string.IsNullOrEmpty(itemTypeID))
                {
                    where = where.And(p => p.ItemTypeID == itemTypeID);
                }
                var data = ef.View_LocaInfo_YY_RTU_ITEM.Where(where);
                var linq = from a in data
                           join b in ef.YY_DATA_AUTO on new { a.STCD, a.ItemID } equals new { b.STCD, b.ItemID }
                           into c
                           from d in c.DefaultIfEmpty()
                           select new YY_DATA_AUTOTabSearchResponse
                           {
                               STCD = a.STCD,
                               ItemID = a.ItemID,
                               ItemName = a.ItemName,
                               ItemTypeID = a.ItemTypeID,
                               ItemTypeName = a.ItemType,
                               DATAVALUE = d.DATAVALUE,
                               TM = d.TM,
                               Units = a.Units,
                               ItemTypeIndex = a.ItemTypeIndex,
                               ItemIndex=a.ItemIndex
                               //AlarmsLevels 字段由外部逻辑二次填充
                           };
                if (page.StartTime != null && page.StartTime == page.EndTime)
                {
                    linq = linq.Where(p => p.TM == page.StartTime);
                }
                else
                {
                    if (page.StartTime != null)
                    {
                        linq = linq.Where(p => p.TM >= page.StartTime);
                    }
                    if (page.EndTime != null)
                    {
                        linq = linq.Where(p => p.TM <= page.EndTime);
                    }
                }
                page.TotalRows = linq.Count();
                int startRow = page.PageSize * (page.PageIndex - 1);
                return linq.OrderBy(o=>o.ItemIndex).Skip(startRow).Take(page.PageSize).ToList();
            }
        }




        public List<YY_DATA_AUTOTabSearchResponse> GetDataForOneByDateTime(string stcd, List<string> itemIDs, string itemTypeID, DateTime? startTime, DateTime? endTime)
        {
            using (MonitorManagerEntities ef = new MonitorManagerEntities())
            {
                var where = PredicateExtensionses.True<View_LocaInfo_YY_RTU_ITEM>();
                if (!string.IsNullOrEmpty(stcd))
                {
                    where = where.And(p => p.STCD == stcd);
                }
                if (itemIDs != null && itemIDs.Count > 0)
                {
                    where = where.And(p => itemIDs.Contains(p.ItemID));
                }
                if (!string.IsNullOrEmpty(itemTypeID))
                {
                    where = where.And(p => p.ItemTypeID == itemTypeID);
                }
                var data = ef.View_LocaInfo_YY_RTU_ITEM.Where(where);
                var linq = from a in data
                           join b in ef.YY_DATA_AUTO on new { a.STCD, a.ItemID } equals new { b.STCD, b.ItemID }
                           into c
                           from d in c.DefaultIfEmpty()
                           select new YY_DATA_AUTOTabSearchResponse
                           {
                               STCD = a.STCD,
                               ItemID = a.ItemID,
                               ItemName = a.ItemName,
                               ItemTypeID = a.ItemTypeID,
                               ItemTypeName = a.ItemType,
                               DATAVALUE = d.DATAVALUE,
                               TM = d.TM,
                               Units = a.Units,
                               ItemTypeIndex = a.ItemTypeIndex,
                               ItemIndex = a.ItemIndex
                           };
                if (startTime != null && startTime == endTime)
                {
                    linq = linq.Where(p => p.TM == startTime);
                }
                else
                {
                    if (startTime != null)
                    {
                        linq = linq.Where(p => p.TM >= startTime);
                    }
                    if (endTime != null)
                    {
                        linq = linq.Where(p => p.TM <= endTime);
                    }
                }
                return linq.OrderBy(o => o.ItemIndex).ToList();
            }
        }

        public int Add(List<YY_DATA_AUTO> infos)
        {
            using (MonitorManagerEntities ef = new MonitorManagerEntities())
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    try
                    {
                        foreach (var info in infos)
                        {
                            var exist=ef.YY_DATA_AUTO.Where(p => p.STCD == info.STCD && p.ItemID == info.ItemID && p.TM == info.TM).FirstOrDefault();
                            if (exist != null)
                            {
                                ef.Entry<YY_DATA_AUTO>(exist).State = System.Data.Entity.EntityState.Modified;
                            }
                            else {
                                ef.YY_DATA_AUTO.Add(info);
                                ef.Entry<YY_DATA_AUTO>(info).State = System.Data.Entity.EntityState.Added;
                            }
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


    }
}
