using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Data;
namespace MonitorManager.Api
{
    public class ItemDataHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string connectionId= Context.ConnectionId;
            return base.OnDisconnected(stopCalled);
        }

        public void ClientRegister(int userid,string stcd,string itemids)
        {
            string connectionId = Context.ConnectionId;
            RegisterEntity exist = clientList.Where(p => p.ConnectionId == connectionId).FirstOrDefault();
                List<string> tempItemIDs = null;
                if (!string.IsNullOrEmpty(itemids))
                {
                    tempItemIDs = itemids.Split(',').ToArray().ToList();
                }
                if (exist != null)
                {
                   
                    //更新参数
                    exist.UserID = userid.ToInt();
                    exist.Condition = new SearchCondition()
                    {
                        STCD = stcd,
                        ItemIDs = tempItemIDs
                    };
                }
                else
                {
                    clientList.Add(new RegisterEntity() {
                         UserID=userid.ToInt(),
                         ConnectionId=connectionId,
                         Condition=new SearchCondition() {
                             STCD = stcd,
                             ItemIDs = tempItemIDs
                         }   
                    });
                }
            Clients.Client(connectionId).RegisterResult(new RegisterEntity()
            {
                UserID = userid.ToInt(),
                ConnectionId = connectionId,
                Condition = new SearchCondition()
                {
                    STCD = stcd,
                    ItemIDs = tempItemIDs
                }
            }.SerializeJSON());
        }

        public static List<RegisterEntity> clientList = new List<RegisterEntity>();
    }

    public class RegisterEntity
    {
         public int UserID { get; set; }

         public string ConnectionId { get; set; }
         
         public SearchCondition Condition { get; set; }
    }

    public class SearchCondition
    {
          public string STCD{ get; set; }
          public List<string> ItemIDs { get; set; }
    }
}