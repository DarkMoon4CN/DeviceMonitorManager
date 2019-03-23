using MonitorManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorManager.BLL
{
    public class AppAuthorBLL
    {
        #region 构造单例
        private AppAuthorBLL() { }
        private static AppAuthorBLL _instance;
        public static AppAuthorBLL Instance
        {
            get
            {
                return _instance ?? (_instance = new AppAuthorBLL());
            }
        }
        #endregion


        public List<B_AppAuthor> GetAll()
        {
            using (MonitorManagerEntities ef = new MonitorManagerEntities())
            {
                return ef.B_AppAuthor.ToList();
            }
        }
    }
}
