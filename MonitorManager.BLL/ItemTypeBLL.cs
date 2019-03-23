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
    public class ItemTypeBLL
    {

        #region 构造单例
        private ItemTypeBLL() { }
        private static ItemTypeBLL _instance;
        public static ItemTypeBLL Instance
        {
            get
            {
                return _instance ?? (_instance = new ItemTypeBLL());
            }
        }
        #endregion

        public List<YY_ITEMTYPE> ItemTypeByPage(string itemTypeName, RequstPageBase page)
        {
            using (MonitorManagerEntities ef = new MonitorManagerEntities())
            {
                var where = PredicateExtensionses.True<YY_ITEMTYPE>();
                if (!string.IsNullOrEmpty(itemTypeName))
                {
                    where = where.And(p => p.ItemType.Contains(itemTypeName));
                }
                int startRow = page.PageSize * (page.PageIndex - 1);
                var query = ef.YY_ITEMTYPE.Where(where);
                var result = query.OrderBy(o => o.ItemTypeIndex)
                    .Skip(startRow).Take(page.PageSize).ToList();
                page.TotalRows = query.Count();
                return result;
            }
        }

        public List<YY_ITEMTYPE> GetItemTypeByName(string itemTypeName)
        {
            using (MonitorManagerEntities ef = new MonitorManagerEntities())
            {
                return ef.YY_ITEMTYPE.Where(p => p.ItemType == itemTypeName).ToList();
            }
        }

        public int AddItemType(YY_ITEMTYPE info)
        {
            using (MonitorManagerEntities ef = new MonitorManagerEntities())
            {
                ef.YY_ITEMTYPE.Add(info);
                ef.SaveChanges();
                return 1;
            }
        }

        public int UpdateItemType(YY_ITEMTYPE info)
        {
            try
            {
                using (MonitorManagerEntities ef = new MonitorManagerEntities())
                {
                    ef.YY_ITEMTYPE.Where(p => info.ItemTypeID == p.ItemTypeID).Update(
                      u => new YY_ITEMTYPE
                      {
                          ItemType = info.ItemType,
                          ItemTypeIndex = info.ItemTypeIndex
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

        public bool DelItemType(List<string> itemTypeIDs)
        {
            try
            {
                using (MonitorManagerEntities ef = new MonitorManagerEntities())
                {
                    ef.YY_ITEMTYPE.Where(p => itemTypeIDs.Contains(p.ItemTypeID)).Delete();
                    ef.YY_ITEM_TI.Where(p => itemTypeIDs.Contains(p.ItemTypeID)).Delete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
