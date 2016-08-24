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

        public SyncTaskRecord()
        {
        }

        public void OnStart()
        {
            DateTime LuckTime = DateTime.Now.AddMinutes(-3);
            TimeSpan span = LuckTime - DateTime.Now;
            if (span < TimeSpan.Zero)
            {
                span = LuckTime.AddMinutes(6) - DateTime.Now;
            }
            //按需传递的状态或者对象。   
            object state = new object();
            //定义计时器   
            syncTimer = new Timer(
                new TimerCallback(AuditWorking), state,
                span, TimeSpan.FromTicks(TimeSpan.TicksPerMinute));
        }

        public void AuditWorking(object state)
        {
            int totalcount = 0, pagecount = 0;
            List<OtherSyncTaskRecord> list = LogBusiness.BaseBusiness.GetSyncTaskRecord(1, 0, "", "", Int32.MaxValue, 1,
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
                LogBusiness.UpdateOtherRecord(obj.AutoID, status,string.IsNullOrEmpty(errormsg) ? "" : errormsg);
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
            infohead += DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            if (!Directory.Exists(@"d:\log\" + directoryName))
            {
                Directory.CreateDirectory(@"d:\log\" + directoryName);
            }

            fs = new FileStream(@"d:\log\" + directoryName + "\\" + fileName + fileExtention, FileMode.OpenOrCreate,
                FileAccess.Write);

            StreamWriter sw = new StreamWriter(fs);
            sw.BaseStream.Seek(0, SeekOrigin.End);
            sw.WriteLine("SyncOtherOrderRecord: Service Started" + infohead + str + "\n");

            sw.Flush();
            sw.Close();
            fs.Close();
        }
    }
}