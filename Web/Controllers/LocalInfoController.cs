using ChuanYe.Utils;
using MonitorManager.BLL;
using MonitorManager.Model;
using MonitorManager.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.IO;

namespace Web.Controllers
{
    public class LocalInfoController : Web.Factory.BaseController
    {
        // GET: LocalInfo
        public ActionResult Index()
        {
            string mid = CheckRequest.GetString("menuId");//左侧菜单  Home/Index  92行
            ViewBag.MenuId = mid;
            return View();
        }

        public ContentResult GetLocalInfoList()
        {
            string page = CheckRequest.GetString("page");
            string rows = CheckRequest.GetString("rows");
            string niceName = CheckRequest.GetString("niceName");
            string address = CheckRequest.GetString("address");
            string isIncomplete = CheckRequest.GetString("isIncomplete");
            string adddatestart = CheckRequest.GetString("adddatestart");
            string adddateend = CheckRequest.GetString("adddateend");

            DateTime? startTime = string.IsNullOrEmpty(adddatestart) == true ? new Nullable<DateTime>() : adddatestart.ToDateTime();
            DateTime? endTime = string.IsNullOrEmpty(adddateend) == true ? new Nullable<DateTime>() : adddateend.ToDateTime();
            RequstPageBase pager = new RequstPageBase();
            pager.StartTime = startTime;
            pager.EndTime = endTime;
            pager.PageIndex = string.IsNullOrEmpty(page)?1:page.ToInt() ;
            pager.PageSize= string.IsNullOrEmpty(rows) ? 20: rows.ToInt();
            bool temp_IsIncomplete = !string.IsNullOrEmpty(isIncomplete)&& isIncomplete=="1" ?true:false ;
           
            var result = LocalInfoBLL.Instance.LocalInfoByPage(niceName,address, temp_IsIncomplete,pager);
            var pageJson = result.SerializeJSON();
            return Content("{\"total\": " + pager.TotalRows.ToString() + ",\"rows\":" + pageJson + "}");
        }

        public ContentResult DelLocalInfo()
        {
            string ids = CheckRequest.GetString("IDs");
            List<string> delIDs = ids.Split(',').ToArray().ToList();
            #region 移除图片
            for (  int i = 0; i < delIDs.Count; i++)
            {
                var iamgePathList = LocalInfoBLL.Instance.GetLocalInfoImageBySTCD(delIDs[i]);
                foreach (var item in iamgePathList)
                {
                    string path = Server.MapPath(item.ImagePath);
                    System.IO.FileInfo file = new FileInfo(path);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }
            }
            LocalInfoBLL.Instance.DelImage(delIDs);
            #endregion

            bool result = LocalInfoBLL.Instance.DelLocalInfo(delIDs);
            if (result)
            {
                return Content("{\"msg\":\"删除成功！\",\"success\":true}");
            }
            else
            {
                return Content("{\"msg\":\"删除失败！\",\"success\":false}");
            }
          
        }

        public ActionResult AddPage()
        {
            return View();
        }
        public ContentResult AddLocalInfo()
        {
            string STCD = CheckRequest.GetString("STCD");
            string NiceName = CheckRequest.GetString("NiceName");
            string Latitude = CheckRequest.GetString("Latitude");
            string Longitude = CheckRequest.GetString("Longitude");
            string LocaManager = CheckRequest.GetString("LocaManager");
            string Tel = CheckRequest.GetString("Tel");
            string Altitude = CheckRequest.GetString("Altitude");
            string Address = CheckRequest.GetString("Address");
            string Describe = CheckRequest.GetString("Describe");
            string PassWord = "123456";

            LocaInfo_Tab info = new LocaInfo_Tab()
            {
                STCD = STCD,
                LocaManager = LocaManager,
                Latitude = !string.IsNullOrEmpty(Altitude)? Latitude.ToDecimal():new  System.Nullable<decimal>(),
                Longitude = !string.IsNullOrEmpty(Longitude) ? Longitude.ToDecimal() : new System.Nullable<decimal>(),
                Tel = Tel,
                Altitude = !string.IsNullOrEmpty(Altitude) ? Altitude.ToDecimal() : new System.Nullable<decimal>(),
                Address = Address,
                Describe = Describe,
                AddTime=DateTime.Now
            };

            YY_RTU_Basic rel = new YY_RTU_Basic()
            {
                STCD=STCD,
                Latitude = !string.IsNullOrEmpty(Altitude) ? Latitude.ToDecimal() : new System.Nullable<decimal>() ,
                Longitude = !string.IsNullOrEmpty(Longitude) ? Longitude.ToDecimal() : new System.Nullable<decimal>(),
                NiceName=NiceName,
                PassWord=PassWord,
            };

            int state = LocalInfoBLL.Instance.AddLocalInfo(info,rel);
            if (state > 0)
            {
                return Content("{\"msg\":\"添加成功！\",\"success\":true}");
            }
            else
            {
                return Content("{\"msg\":\"添加失败！\",\"success\":false}");
            }
        }
        public ActionResult EditPage()
        {
            return View();
        }
        public ContentResult EditLocalInfo() {
            string STCD = CheckRequest.GetString("STCD");
            string NiceName = CheckRequest.GetString("NiceName");
            string Latitude = CheckRequest.GetString("Latitude");
            string Longitude = CheckRequest.GetString("Longitude");
            string LocaManager = CheckRequest.GetString("LocaManager");
            string Tel = CheckRequest.GetString("Tel");
            string Altitude = CheckRequest.GetString("Altitude");
            string Address = CheckRequest.GetString("Address");
            string Describe = CheckRequest.GetString("Describe");
            string PassWord ="123456";

            LocaInfo_Tab info = new LocaInfo_Tab()
            {
                STCD = STCD,
                LocaManager = LocaManager,
                Latitude = !string.IsNullOrEmpty(Altitude) ? Latitude.ToDecimal() : new System.Nullable<decimal>(),
                Longitude = !string.IsNullOrEmpty(Longitude) ? Longitude.ToDecimal() : new System.Nullable<decimal>(),
                Tel = Tel,
                Altitude = !string.IsNullOrEmpty(Altitude) ? Altitude.ToDecimal() : new System.Nullable<decimal>(),
                Address = Address,
                Describe = Describe,
                AddTime = DateTime.Now
            };

            YY_RTU_Basic rel = new YY_RTU_Basic()
            {
                STCD = STCD,
                Latitude = !string.IsNullOrEmpty(Altitude) ? Latitude.ToDecimal() : new System.Nullable<decimal>(),
                Longitude = !string.IsNullOrEmpty(Longitude) ? Longitude.ToDecimal() : new System.Nullable<decimal>(),
                NiceName = NiceName,
                PassWord = PassWord,
            };

            int state = LocalInfoBLL.Instance.UpdateLocalInfo(info, rel);
            if (state > 0)
            {
                return Content("{\"msg\":\"修改成功！\",\"success\":true}");
            }
            else
            {
                return Content("{\"msg\":\"修改失败！\",\"success\":false}");
            }
        }

        public ActionResult SetItemPage() {
            return View();
        }

        public ContentResult GetAllItem()
        {
            var page = ItemBLL.Instance.GetALLItem();
            List<dynamic> keyValue = new List<dynamic>();
            foreach (var item in page)
            {
                keyValue.Add(new { id = item.ItemID, text = item.ItemName });
            }
            var result = new { id = 0, text = "全选", children = keyValue };
            return Content("[" + result.SerializeJSON() + "]");
        }

        public ContentResult GetLocalInfoItemBySTCD()
        {
            string stcd = CheckRequest.GetString("STCD");  
            List<string> itemIDs = LocalInfoBLL.Instance.GetLocalInfoItemRel(stcd).Select(s => s.ItemID).ToList();
            string itemIDsStr = string.Join(",", itemIDs);
            return Content(itemIDsStr);
        }

        public ContentResult SetLocalInfoItem()
        {
            string STCD = CheckRequest.GetString("STCD");
            string ItemIDs = CheckRequest.GetString("ItemIDs");

            List<string> ids = new List<string>();
            if (!string.IsNullOrEmpty(ItemIDs))
            {
                ids = ItemIDs.Split(',').ToArray().ToList();
            }
            int state= LocalInfoBLL.Instance.SetLocalInfoItem(STCD, ids);

            if (state > 0)
            {
                return Content("{\"msg\":\"修改成功！\",\"data\":\""+STCD+"\",\"success\":true}");
            }
            else
            {
                return Content("{\"msg\":\"修改失败！\",\"success\":false}");
            }
        }

        public ActionResult SetLocalInfoIamgePage()
        {
            return View();
        }

        public ContentResult SetLocalInfoImage() {

            try
            {
                string STCD =(string)Request["STCD"];
                HttpPostedFileBase file = Request.Files["txtFile"];
                if (file != null)
                {
                    string[] suffixTemp = file.FileName.Split('.');
                    string suffix = suffixTemp[suffixTemp.Length-1];
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
                    var path = Path.Combine(Request.MapPath("~/Upload"), fileName + "." + suffix);
                    file.SaveAs(path);
                    LocalInfoBLL.Instance.AddLocalInfoImage(new Image_Tab {
                        ImageID=System.Guid.NewGuid().ToString(),
                        AddTime = DateTime.Now,
                        ImagePath = "/Upload/" + fileName + "." + suffix,
                        Remark = string.Empty,
                        STCD = STCD,
                    });
                    return Content("{\"msg\":\"上传成功！\",\"data\":\"" + STCD + "\",\"success\":true}");
                }
                else {
                    return Content("{\"msg\":\"文件不能为空！\",\"success\":true}");
                }
           
            }
            catch (Exception ex)
            {
                return Content("{\"msg\":\"上传失败！\",\"success\":false}");
            }
         

        }

        public ContentResult GetLocalInfoImage()
        {
            string STCD = CheckRequest.GetString("STCD");
            var data=LocalInfoBLL.Instance.GetLocalInfoImageBySTCD(STCD);
            var result = data.Select(p=>new { ImageID = p.ImageID, ImagePath = p.ImagePath});
            return Content(result.SerializeJSON());
        }

        public ContentResult DelImage()
        {
            string ImageID = CheckRequest.GetString("ImageID");
            if (!string.IsNullOrEmpty(ImageID))
            {
                var iamgePathList = LocalInfoBLL.Instance.GetLocalInfoImageByID(ImageID);
                foreach (var item in iamgePathList)
                {
                    string path = Server.MapPath(item.ImagePath);
                    System.IO.FileInfo file = new FileInfo(path);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }
                int delRow = LocalInfoBLL.Instance.DelImage(ImageID);
                if (delRow>0)
                {
                    return Content("{\"msg\":\"删除成功！\",\"success\":true}");
                }
                else
                {
                    return Content("{\"msg\":\"删除失败！\",\"success\":false}");
                }
            }
            else
            {
                return Content("{\"msg\":\"数据传输失败！\",\"success\":false}");
            }
        }
    }
}