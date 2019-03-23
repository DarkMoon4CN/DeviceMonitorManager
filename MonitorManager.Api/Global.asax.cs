using MonitorManager.Api.Controllers;
using MonitorManager.BLL;
using MonitorManager.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace MonitorManager.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// 告警配置 30分钟重新获取一次
        /// </summary>
        public static List<AlarmCondition_Tab> ALARM_CONDIATION = new List<AlarmCondition_Tab>();
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            string path = ConfigurationManager.AppSettings["WatcherPath"].ToString();
            var t = Task.Factory.StartNew(() =>
            {
                Watcher(path, "*.*");
            });

            var ac = Task.Factory.StartNew(()=> {
                while (true)
                {
                    lock (this)
                    {
                        ALARM_CONDIATION = AlarmConditionBLL.Instance.GetAlarmCondition(null, null, -1);
                    }
                    System.Threading.Thread.Sleep(1800);

                }
            });
        }

        protected void Watcher(string path, string filter)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.Filter = filter;
            watcher.Created += new FileSystemEventHandler(OnProcess);
            watcher.EnableRaisingEvents = true;
            watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess
                                   | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;
            watcher.IncludeSubdirectories = true;

        }

        private static void OnProcess(object source, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                OnCreated(source, e);
            }
        }

        private static void OnCreated(object source, FileSystemEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("文件新建事件处理逻辑 {0}  {1}  {2}", e.ChangeType, e.FullPath, e.Name);
            try
            {
                #region  写入数据库
                if (e.FullPath.IndexOf(@"\") != -1)
                {
                    string[] splitString = e.FullPath.Split('\\');
                    string fileName = splitString[splitString.Length - 1];
                    if (fileName.IndexOf(".smp") != -1)
                    {
                        System.Text.StringBuilder buffer = new System.Text.StringBuilder();
                        FileStream fs = new FileStream(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        byte[] bytes = new byte[fs.Length];
                        fs.Read(bytes, 0, bytes.Length);
                        fs.Close();
                        if (bytes != null && bytes.Length > 326)
                        {
                            byte[] temp = bytes.Skip(322).Take(4).ToArray();
                            var flag = System.Text.Encoding.Default.GetString(temp);
                            double[] ptRes = new double[1024];
                            if (flag == "data")
                            {
                                int i = 0;
                                int start = 326;
                                while (i < 1024)
                                {
                                    byte[] temp2 = bytes.Skip(start).Take(4).ToArray();
                                    var flag2 = System.BitConverter.ToSingle(temp2, 0);
                                    System.Diagnostics.Debug.WriteLine("第 {0} -> 数据 {1} ", i + 1, flag2);
                                    ptRes[i] = flag2;
                                    i++;
                                    start += 4;
                                }
                            }
                            //FFT逻辑
                            double[] fftRes = FFT(ptRes);
                            var fileNameSplit = fileName.Split('_');
                            string stcd = fileNameSplit[0];
                            string itemID = fileNameSplit[1];
                            string yyyyMMdd = fileNameSplit[fileNameSplit.Length - 1];
                            string HHmmdd = fileNameSplit[fileNameSplit.Length];
                            YY_HS_DATA_AUTO info = new YY_HS_DATA_AUTO()
                            {
                                STCD = stcd,
                                ItemID = itemID,
                                TM = (yyyyMMdd + " " + HHmmdd).ToDateTime(),
                                DATAVALUE = string.Join("|", ptRes),
                                FFT = string.Join("|", fftRes),
                            };
                            YY_HS_DATA_AUTO_BLL.Instance.AddYY_HS_DATA_AUTO(info);
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                ChuanYe.Utils.LoggerHelper.Error(ex.Message);
            }
        }



        private static double[] FFT(double[] ptRes)
        {
            FFT fft = new FFT();
            ComplexList list = new ComplexList(ptRes);
            int k = 0;
            ComplexList l = fft.fft_core(list, ref k, 1);

            Complex[] c = new Complex[ptRes.Length];
            for (int i = 0; i < ptRes.Length; i++)
            {
                c[i] = new Complex(ptRes[i]);
            }
            Complex[] cc = fft.fft_frequency(c, ptRes.Length);

            double[] fft_Sqrt = new double[401];
            if (cc.Length == 1024)
            {
                for (int m = 0; m < 401; m++)
                {
                    var temp=Math.Pow(cc[m].Re, 2) + Math.Pow(cc[m].Im, 2);
                    var fft_temp = Math.Sqrt(temp);
                    fft_Sqrt[m] = fft_temp;
                }
            }
            return fft_Sqrt;
        }
    }
}
