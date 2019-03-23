using EntityFramework.Extensions;
using MonitorManager.Model;
using MonitorManager.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace MonitorManager.BLL
{
    public class LocalInfoBLL
    {
        #region 构造单例
        private LocalInfoBLL() { }
        private static LocalInfoBLL _instance;
        public static LocalInfoBLL Instance
        {
            get
            {
                return _instance ?? (_instance = new LocalInfoBLL());
            }
        }
        #endregion

        private MonitorManagerEntities entities = new MonitorManagerEntities();
        public List<View_LocalInfo_YY_RTU_Basic> LocalInfoByPage(string niceName, string address, bool isIncomplete, RequstPageBase page)
        {
            var query = from a in entities.LocaInfo_Tab
                        join b in entities.YY_RTU_Basic on a.STCD equals b.STCD into temp
                        from c in temp.DefaultIfEmpty()
                        select new View_LocalInfo_YY_RTU_Basic
                        {
                            STCD = a.STCD,
                            NiceName = c.NiceName,
                            Latitude = a.Latitude,
                            Longitude = a.Longitude,
                            LocaManager = a.LocaManager,
                            Tel = a.Tel,
                            Altitude = a.Altitude,
                            Address = a.Address,
                            AddTime = a.AddTime,
                            Describe = a.Describe,
                            PassWord = c.PassWord,
                            STCDTemp = c.STCD,
                        };
            query = query.Where(p => string.IsNullOrEmpty(niceName) || p.NiceName.Contains(niceName));
            query = query.Where(p => string.IsNullOrEmpty(address) || p.Address.Contains(address));

            if (isIncomplete == true)
            {
                query = query.Where(p => string.IsNullOrEmpty(p.STCDTemp));
            }
            if (page.StartTime != null)
            {
                query = query.Where(p => p.AddTime > page.StartTime);
            }
            if (page.EndTime != null)
            {
                query = query.Where(p => p.AddTime < page.EndTime);
            }
            int startRow = page.PageSize * (page.PageIndex - 1);
            var result = query.OrderBy(o => o.STCDTemp).ThenByDescending(t => t.AddTime)
                .Skip(startRow).Take(page.PageSize).ToList();
            page.TotalRows = query.Count();
            return result;
        }

        public List<View_LocalInfo_YY_RTU_Basic> GetAll(string stcd=null)
        {
            var query = from a in entities.LocaInfo_Tab
                        join b in entities.YY_RTU_Basic on a.STCD equals b.STCD into temp
                        from c in temp.DefaultIfEmpty()
                        select new View_LocalInfo_YY_RTU_Basic
                        {
                            STCD = a.STCD,
                            NiceName = c.NiceName,
                            Latitude = a.Latitude,
                            Longitude = a.Longitude,
                            LocaManager = a.LocaManager,
                            Tel = a.Tel,
                            Altitude = a.Altitude,
                            Address = a.Address,
                            AddTime = a.AddTime,
                            Describe = a.Describe,
                            PassWord = c.PassWord,
                            STCDTemp = c.STCD,
                        };

            if (!string.IsNullOrEmpty(stcd))
            {
                query = query.Where(p => p.STCD == stcd);
            }
             var result = query.Where(p=>!string.IsNullOrEmpty(p.NiceName))
                           .OrderBy(o => o.STCDTemp).ThenByDescending(t => t.AddTime)
                           .ToList();
            return result;
        }

     

        public bool DelLocalInfo(List<string> delIDs)
        {
            //删除项目表
            entities.LocaInfo_Tab.Where(p => delIDs.Contains(p.STCD)).Delete();
            //删除项目关系表
            entities.YY_RTU_Basic.Where(p => delIDs.Contains(p.STCD)).Delete();
            //删除项目元素关系表
            entities.YY_RTU_BI.Where(p => delIDs.Contains(p.STCD)).Delete();
            return true;
        }

        public int AddLocalInfo(LocaInfo_Tab info, YY_RTU_Basic rel)
        {
            using (MonitorManagerEntities ef = new MonitorManagerEntities())
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    try
                    {
                        //实体表
                        ef.LocaInfo_Tab.Add(info);
                        ef.Entry<LocaInfo_Tab>(info).State = System.Data.Entity.EntityState.Added;
                        //关系表信息  
                        ef.YY_RTU_Basic.Add(rel);
                        ef.Entry<YY_RTU_Basic>(rel).State = System.Data.Entity.EntityState.Added;
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

        public int UpdateLocalInfo(LocaInfo_Tab info, YY_RTU_Basic rel)
        {
            try
            {
                entities.LocaInfo_Tab.Where(p => info.STCD == p.STCD).Update(
                        u => new LocaInfo_Tab
                        {
                            LocaManager = info.LocaManager,
                            Tel = info.Tel,
                            Address = info.Address,
                            Describe = info.Describe,
                            AddTime = DateTime.Now,
                            Latitude = info.Latitude,
                            Longitude = info.Longitude,
                            Altitude = info.Altitude,
                        }
                     );

                var exist = entities.YY_RTU_Basic.Where(p => info.STCD == p.STCD).FirstOrDefault();

                if (exist != null)
                {
                    entities.YY_RTU_Basic.Where(p => info.STCD == p.STCD).Update(
                            u => new YY_RTU_Basic
                            {
                                Latitude = rel.Latitude,
                                Longitude = rel.Longitude,
                                NiceName = rel.NiceName,
                                PassWord = rel.PassWord,
                            }
                         );
                }
                else
                {
                    entities.YY_RTU_Basic.Add(new YY_RTU_Basic
                    {
                        STCD = info.STCD,
                        Latitude = rel.Latitude,
                        Longitude = rel.Longitude,
                        NiceName = rel.NiceName,
                        PassWord = rel.PassWord,
                    });
                    entities.SaveChanges();
                }
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        public List<YY_RTU_BI> GetLocalInfoItemRel(string stcd)
        {
            return entities.YY_RTU_BI.Where(p => p.STCD == stcd).ToList();
        }

        public int SetLocalInfoItem(string stcd, List<string> itemIDs)
        {
            //清理之前配置的元素
            entities.YY_RTU_BI.Where(p => p.STCD == stcd).Delete();
            using (MonitorManagerEntities ef = new MonitorManagerEntities())
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    try
                    {
                        foreach (var item in itemIDs)
                        {
                            if (item == "0")
                            {
                                continue;
                            }
                            YY_RTU_BI rel = new YY_RTU_BI();
                            rel.STCD = stcd;
                            rel.ItemID = item;
                            ef.YY_RTU_BI.Add(rel);
                            ef.Entry<YY_RTU_BI>(rel).State = System.Data.Entity.EntityState.Added;
                        }
                        ef.SaveChanges();
                        //提交事务
                        transaction.Complete();
                        return 1;
                    }
                    catch (Exception ex)
                    {
                        return 0;
                    }
                }
            }
        }

        public int AddLocalInfoImage(Image_Tab info)
        {
            entities.Image_Tab.Add(info);
            entities.SaveChanges();
            return 1;
        }

        public List<Image_Tab> GetLocalInfoImageBySTCD(string stcd)
        {
           return entities.Image_Tab.Where(p => p.STCD == stcd).OrderByDescending(o=>o.AddTime).ToList();
        }

        public List<Image_Tab> GetLocalInfoImageByID(string imageID)
        {
            return entities.Image_Tab.Where(p => p.ImageID == imageID).OrderByDescending(o => o.AddTime).ToList();
        }


        public int DelImage(string  imageID)
        {
            return entities.Image_Tab.Where(p => p.ImageID == imageID).Delete();
        }

        public int DelImage(List<string> stcds)
        {
            return entities.Image_Tab.Where(p => stcds.Contains(p.STCD)).Delete();
        }
    }
}
