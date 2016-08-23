using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using IntFactoryBusiness;
using IntFactoryEntity;
using CloudSales.Sdk.Business;
using CloudSales.Sdk.EDJEntity;

namespace YXERP.Common
{
    public class SyncTaskRecord
    {  
        private static Timer syncTimer = null;   
        public void OnStart(string[] args)
        {
            DateTime LuckTime = DateTime.Now.Date.Add(new TimeSpan(7, 0, 0));
            TimeSpan span = LuckTime - DateTime.Now;
            if (span < TimeSpan.Zero)
            {
                span = LuckTime.AddDays(1d) - DateTime.Now;
            }
            //按需传递的状态或者对象。   
            object state = new object();
            //定义计时器   
            syncTimer = new Timer(
            new TimerCallback(AuditWorking), state,
            span, TimeSpan.FromTicks(TimeSpan.TicksPerDay));   
        }

        public  void AuditWorking(object state)
        {
            int totalcount = 0,pagecount=0;
            List<OtherSyncTaskRecord> list = LogBusiness.BaseBusiness.GetSyncTaskRecord(1, 0, "","", Int32.MaxValue, 1,
                ref totalcount, ref pagecount);
            foreach (var obj  in list)
            {
                int status = 2;
                OperateResult result=  TaskOrderBusiness.BaseBusiness.AddStockPartIn(obj.Content);
                if (result.error_code == 0)
                {
                    if (result.result == 1)
                    {
                        status = 1;
                    }
                }
                LogBusiness.UpdateOtherRecord(obj.AutoID, status,string.IsNullOrEmpty(result.error_message)?"":result.error_message );
            }

        }
    }
}