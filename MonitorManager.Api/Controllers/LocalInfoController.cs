using MonitorManager.Api.Factory;
using MonitorManager.Api.Models;
using MonitorManager.BLL;
using MonitorManager.Model;
using MonitorManager.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace MonitorManager.Api.Controllers
{
    [EnableCors("*", "*", "*")]
    //[ApiAuth]
    public class LocalInfoController : ApiController
    {

        

        /// <summary>
        /// 项目列表
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultDto<dynamic,dynamic> LocalInfoByPage(LocalInfoSearchRequest obj)
        {
            ResultDto<dynamic,dynamic> result = new ResultDto<dynamic,dynamic>();
            try
            {
                RequstPageBase page = new RequstPageBase();
                page.StartTime = obj.StartTime;
                page.EndTime = obj.EndTime;
                page.PageIndex = obj.PageIndex == 0 ? 1 : obj.PageIndex;
                page.PageSize = obj.PageSize == 0 ? 20 : obj.PageSize;
                result.FirstParam=LocalInfoBLL.Instance.LocalInfoByPage(obj.NiceName, obj.Address, false, page);
                result.SecondParam = page.TotalRows;
                result.Status = 1;
                result.Message = "Requst Is Success";
            }
            catch(Exception ex)
            {
                result.Status = 500;
                result.Message = "/LocalInfo/LocalInfoByPage Server Error";
                ChuanYe.Utils.LoggerHelper.Error(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 左侧树形结构 级别依次为：项目，分类，元素
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResultDto<dynamic> GetLocalInfoTree()
        {
            ResultDto<dynamic> result = new ResultDto<dynamic>();
           
            try
            {
                string imageServer = ConfigurationManager.AppSettings["ImageServer"].ToString();
                RequstPageBase page = new RequstPageBase();
                page.PageIndex = 1;
                page.PageSize = int.MaxValue;
                //项目
                var localInfoList = LocalInfoBLL.Instance.LocalInfoByPage(null, null, false, page);
                List<dynamic> localData = new List<dynamic>();
                foreach (var local in localInfoList)
                {
                    var images = LocalInfoBLL.Instance.GetLocalInfoImageBySTCD(local.STCD);
                    if (!string.IsNullOrEmpty(imageServer))
                    {
                        foreach (var item in images)
                        {
                            item.ImagePath = imageServer + item.ImagePath;
                        }
                    }
                    //项目，元素
                    var localInfoItemList = ItemBLL.Instance.LocalInfoItemByPage(
                        new View_LocaInfo_YY_RTU_ITEM  { STCD = local.STCD}, page).ToList();
                    //告警总条数
                    int alarmInfoCount = AlarmInfoBLL.Instance.AlarmInfoCountFor24Hours(local.STCD);
                    //获取项目下所有的告警信息
                    var alarmAllList = AlarmConditionBLL.Instance.GetAlarmCondition(local.STCD,null, 0);
                    var itemIDs = localInfoItemList.Select(s => s.ItemID).Distinct().ToList();
                    //获取上下限
                    var ecList = ItemBLL.Instance.GetElementsChartsInfo(itemIDs);

                    //获取分类
                    var itemTypeList = localInfoItemList.Select(s => new YY_ITEMTYPE()
                    {
                        ItemTypeID = s.ItemTypeID,
                        ItemType = s.ItemType,
                        ItemTypeIndex = s.ItemTypeIndex
                    });
                     itemTypeList = itemTypeList.GroupBy(g=>g.ItemTypeID)
                                   .Select(s=> new YY_ITEMTYPE()
                                   {
                                       ItemTypeID = s.Key,
                                       ItemType=s.FirstOrDefault().ItemType,
                                       ItemTypeIndex = s.FirstOrDefault().ItemTypeIndex,
                                   }).OrderByDescending(o=>o.ItemTypeIndex).ToList();
                    //获取元素
                    List<dynamic> itemTypeData = new List<dynamic>();
                    foreach (var it in itemTypeList)
                    {
                        var item = localInfoItemList.Where(p => it.ItemTypeID == p.ItemTypeID).ToList();
                        List<dynamic> itemList = new List<dynamic>();
                        foreach (var t2 in item)
                        {
                            var  ec=ecList.Where(p => p.ItemID == t2.ItemID).FirstOrDefault();
                            var alarmTempList = alarmAllList.Where(p => p.ItemID == t2.ItemID).ToList();
                            List<dynamic> alarmList = new List<dynamic>();

                            //自定义分组标记 2019/02/13 补充需求
                            var groupData = GetTypeNameGroup(it.ItemType, t2.ItemName);
                            foreach (var t3 in alarmTempList)
                            {
                                alarmList.Add(new
                                {
                                    t3.STCD,
                                    t3.ItemID,
                                    t3.DATAVALUE,
                                    t3.Condition,
                                    ConditionName = GetConditionName(t3.Condition),
                                    t3.AlarmLevel
                                });
                            }
                            itemList.Add(new {
                                ItemCode = t2.ItemCode,
                                ItemDecimal = t2.ItemDecimal.Value,
                                ItemID = t2.ItemID,
                                ItemInteger = t2.ItemInteger.Value,
                                ItemName = t2.ItemName,
                                PlusOrMinus = t2.PlusOrMinus.Value,
                                Units = t2.Units,
                                AlarmAreaMin = ec != null ? ec.AlarmAreaMin : null,
                                AlarmAreaMax = ec != null ? ec.AlarmAreaMax : null,
                                DisplayAreaMin = ec != null ? ec.DisplayAreaMin : null,
                                DisplayAreaMax = ec != null ? ec.DisplayAreaMax : null,
                                Alarms = alarmList,
                                GroupID= groupData!=null? groupData.GroupID:0,
                                GroupName= groupData!=null? groupData.GroupName: string.Empty,
                                ItemIndex=t2.ItemIndex
                            });

                        }
                       itemTypeData.Add(new {
                           ItemTypeID = it.ItemTypeID,
                           ItemType = it.ItemType,
                           ItemTypeIndex = it.ItemTypeIndex,
                           Children= itemList.OrderBy(p=>p.ItemIndex).ToList()
                       });
                    }
                    localData.Add(new {
                        STCD = local.STCD,
                        LocaManager = local.LocaManager,
                        Longitude= local.Longitude,
                        Latitude = local.Latitude,
                        Altitude= local.Altitude,
                        Address = local.Address,
                        AddTime= local.AddTime,
                        Describe = local.Describe,
                        NiceName = local.NiceName,
                        PassWord = local.PassWord,
                        STCDTemp = local.STCDTemp,
                        Images= images,
                        Children = itemTypeData.OrderBy(o=>o.ItemTypeIndex),
                        AlarmInfoCount= alarmInfoCount,
                    });
                }
                result.FirstParam = localData;
                result.Status = 1;
                result.Message = "Requst Is Success";
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = "/LocalInfo/GetLocalInfoTree Server Error";
                ChuanYe.Utils.LoggerHelper.Error(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 根据项目STCD获取图片信息
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultDto<dynamic> GetImageBySTCD(LocalInfoImageRequest obj)
        {
            ResultDto<dynamic> result = new ResultDto<dynamic>();
            try
            {
                string imageServer = ConfigurationManager.AppSettings["ImageServer"].ToString();
                
                if (obj != null && !string.IsNullOrEmpty(obj.STCD))
                {
                    var data = LocalInfoBLL.Instance.GetLocalInfoImageBySTCD(obj.STCD);

                    if (!string.IsNullOrEmpty(imageServer))
                    {
                        foreach (var item in data)
                        {
                            item.ImagePath = imageServer + item.ImagePath;
                        }

                    }
                    result.FirstParam = data;
                    result.Status = 1;
                    result.Message = "Requst Is Success";
                }
                else
                {
                    result.Status = 0;
                    result.Message = "STCD Is Empty";
                }
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = "/LocalInfo/GetImageBySTCD Server Error";
                ChuanYe.Utils.LoggerHelper.Error(ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 批量增加数据
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultDto<dynamic> AddOrUpDateYY_DATA_AUTO(YY_DATA_AUTOSRequest obj)
        {
            ResultDto<dynamic> result = new ResultDto<dynamic>();
            try
            {
                if (obj == null)
                {
                    result.Status = 0;
                    result.Message = "List Is Empty";
                    return result;
                }
                //深度复制
                var infos = obj.List.SerializeJSON().DeserializeJSON<List<YY_DATA_AUTO>>();
                var state = YY_DATA_AUTO_BLL.Instance.Add(infos);
                result.FirstParam = state;
                result.Status = 1;
                result.Message = "Requst Is Success";
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = "/YY_DATA_AUTO/AddOrUpDateYY_DATA_AUTO Server Error";
                ChuanYe.Utils.LoggerHelper.Error(ex.Message);
            }
            return result;
        }

        private string GetConditionName(int condition)
        {
            if (condition == 1)
            {
                return "大于";
            }
            else if (condition == 2)
            {
                return "小于";
            }
            else if (condition == 3)
            {
                return "等于";
            }
            else if (condition == 4)
            {
                return "大于等于";
            }
            else if (condition == 5)
            {
                return "小于等于";
            }
            else if (condition == 6)
            {
                return "时长超过";
            }
            return null;
        }


        private TypeNameGroupInfo GetTypeNameGroup(string itemTypeName, string itemName)
        {
            TypeNameGroupInfo info = new TypeNameGroupInfo();
            if (itemTypeName.Contains("钢结构"))
            {
                if (itemName.Contains("动应变"))
                {
                    info.GroupID = 1001;
                    info.GroupName = "动应变";
                }
                else if (itemName.Contains("静应变"))
                {
                    info.GroupID = 1002;
                    info.GroupName = "静应变";
                }
                else
                {
                    info.GroupID = 1000;
                    info.GroupName = "其他";
                }
            }
            else if (itemTypeName.Contains("起升开闭机构"))  //减速箱
            {
                if (itemName.Contains("振动波形"))
                {
                    info.GroupID = 2001;
                    info.GroupName = "振动波形";
                }
                else if (itemName.Contains("电机振动"))
                {
                    info.GroupID = 2002;
                    info.GroupName = "振动";
                }
                else if (itemName.Contains("电机温度"))
                {
                    info.GroupID = 2003;
                    info.GroupName = "温度";
                }
                else if (itemName.Contains("电流") || itemName.Contains("电压") || itemName.Contains("转速"))
                {
                    info.GroupID = 2004;
                    info.GroupName = "电流电压";
                }
                else
                {
                    info.GroupID = 2000;
                    info.GroupName = "其他";
                }
            }
            else if (itemTypeName.Contains("俯仰机构")) //卷筒
            {
                if (itemName.Contains("振动波形") || itemName.Contains("减速箱振动波形"))
                {
                    info.GroupID = 3001;
                    info.GroupName = "振动波形";
                }
                else if (itemName.Contains("振动"))
                {
                    info.GroupID = 3002;
                    info.GroupName = "振动";
                }
                else if (itemName.Contains("温度"))
                {
                    info.GroupID = 3003;
                    info.GroupName = "温度";
                }
                else if (itemName.Contains("电流") || itemName.Contains("电压") || itemName.Contains("转速"))
                {
                    info.GroupID = 3004;
                    info.GroupName = "电流电压";
                }
                else
                {
                    info.GroupID = 3000;
                    info.GroupName = "其他";
                }

            }
                return info;
        } 
    }
}
