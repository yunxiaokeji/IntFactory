using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using IntFactoryBusiness;
using IntFactoryEntity;
using CloudSales.Sdk.Business;
using CloudSales.Sdk.EDJEntity;
using System.IO;

namespace YXERP.Common
{
    public class SyncTaskRecord
    {
        private static Timer syncTimer = null;
        private static string runTime = System.Configuration.ConfigurationManager.AppSettings["SyncTaskTime"] ?? "1";
        /// <summary>
        /// 设置时间 Hour  Minute  Second
        /// </summary>
        private static string runType = System.Configuration.ConfigurationManager.AppSettings["SyncTaskTimeType"] ?? "hour";
        public SyncTaskRecord()
        {
        }

        public void OnStart()
        {
            //按需传递的状态或者对象。   
            object state = new object();
            //定义计时器   
            long type=TimeSpan.TicksPerMinute;
            TimeSpan delayTime = new TimeSpan(0, Convert.ToInt32(runTime), 0);
            switch (runType.ToLower())
            { 
                case"minute":
                    delayTime= new TimeSpan(0, Convert.ToInt32(runTime), 0); 
                    break;
                case"hour":
                    delayTime = new TimeSpan(Convert.ToInt32(runTime),0, 0);
                    type=TimeSpan.TicksPerHour;
                    break;
                case"second":
                    delayTime = new TimeSpan(0, 0, Convert.ToInt32(runTime));
                    type=TimeSpan.TicksPerSecond;
                    break;
            }
            syncTimer = new Timer(new TimerCallback(AuditWorking), state, 0, (long)delayTime.TotalMilliseconds);
            //syncTimer = new Timer(
            //    new TimerCallback(AuditWorking), state,
            //    delayTime, TimeSpan.FromTicks(type));
        }

        public void AuditWorking(object state)
        {
            var hour = DateTime.Now.Hour;
            if (hour > 6)
            {
                WriteLog("-------------EDJOrderPush-----Begin-----------------", 1);
                int totalcount = 0, pagecount = 0;
                List<OtherSyncTaskRecord> list = LogBusiness.BaseBusiness.GetSyncTaskRecord(1, 0, "", "", Int32.MaxValue,
                    1,
                    ref totalcount, ref pagecount);
                foreach (var obj  in list)
                {
                    int status = 2;
                    string errormsg = "";
                    try
                    {
                        OperateResult result = TaskOrderBusiness.BaseBusiness.AddStockPartIn(obj.Content);
                        if (result.error_code == 0)
                        {
                            if (result.result == 1)
                            {
                                status = 1;
                            }
                        }
                        errormsg = result.error_message;
                        WriteLog(string.Format("系统订单ID：{0},外部单据:{1}推送成功。", obj.OrderID, obj.OtherSysID), 1);
                    }
                    catch (Exception ex)
                    {
                        errormsg = ex.ToString();
                        WriteLog(string.Format("系统订单ID：{0},外部单据:{1}" + errormsg, obj.OrderID, obj.OtherSysID), 2);
                    }
                    LogBusiness.UpdateOtherRecord(obj.AutoID, status, string.IsNullOrEmpty(errormsg) ? "" : errormsg);
                }
                WriteLog("-------------EDJOrderPush-----End-----------------", 1);
            }
        }

        public void OnStop()
        {
            if (syncTimer != null)
            {
                syncTimer.Dispose();
            }
        }

        // <summary>
        /// 添加服务日志
        /// </summary>
        public void WriteLog(string str, int logType = 1)
        {
            string fileName = DateTime.Now.ToString("yyyy-MM-dd");
            string fileExtention = ".txt";
            string directoryName = "SyncOtherRecord";
            string infohead = "[Info] ";
            FileStream fs = null;
            if (logType == 2)
            {
                infohead = "[Error] ";
            }
            string path =  System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "log\\" + directoryName;
            infohead += DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            fs = new FileStream(path + "\\" + fileName + fileExtention, FileMode.OpenOrCreate,
                FileAccess.Write);

            StreamWriter sw = new StreamWriter(fs);
            sw.BaseStream.Seek(0, SeekOrigin.End);
            sw.WriteLine("SyncOtherOrderRecord: AuditWorking " + infohead + str + "\n");

            sw.Flush();
            sw.Close();
            fs.Close();
        }
    }
}