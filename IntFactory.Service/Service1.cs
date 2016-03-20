using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

using IntFactoryBusiness;
namespace IntFactory.Service
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        System.Timers.Timer DownAliOrdersTimer = new System.Timers.Timer();
        System.Timers.Timer UpdateAliOrdersTimer = new System.Timers.Timer();
        protected override void OnStart(string[] args)
        {
            DownAliOrdersTimer.Interval =10 * 1000;
            DownAliOrdersTimer.Elapsed += new System.Timers.ElapsedEventHandler(DownAliOrdersEvent); //到达时间的时候执行事件；   
            DownAliOrdersTimer.AutoReset = true;   //设置是执行一次（false）还是一直执行(true)；   
            DownAliOrdersTimer.Enabled = true;     //是否执行System.Timers.Timer.Elapsed事件；   

            DownAliOrdersTimer.Interval = 10 * 1000;
            DownAliOrdersTimer.Elapsed += new System.Timers.ElapsedEventHandler(UpdateAliOrdersEvent); //到达时间的时候执行事件；   
            DownAliOrdersTimer.AutoReset = true;   //设置是执行一次（false）还是一直执行(true)；   
            DownAliOrdersTimer.Enabled = true;     //是否执行System.Timers.Timer.Elapsed事件；   

            // TODO: 在此处添加代码以启动服务。
            string state = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "启动";
            WriteLog(state);
            DownAliOrdersTimer.Start();
            UpdateAliOrdersTimer.Start();
        }

        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
            string state = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "停止";
            WriteLog(state);

            DownAliOrdersTimer.Stop();
            UpdateAliOrdersTimer.Stop();
        }

        public void DownAliOrdersEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            string state = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "DownAliOrdersEvent";
            WriteLog(state);

            AliOrderBusiness.ExecuteDownAliOrdersPlan();
        }

        public void UpdateAliOrdersEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            string state = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "UpdateAliOrdersEvent";
            WriteLog(state,2);

            AliOrderBusiness.ExecuteUpdateAliOrders();
        }  


        public void WriteLog(string str,int logType=1)  
        {
            string fileName = DateTime.Now.ToString("yyyy-MM-dd");
            string fileExtention = ".txt";
            string directoryName="downaliorders";
            FileStream fs = null;
            if (logType == 2)
                directoryName = "updatealiorders";

            if (!Directory.Exists(@"c:\log\" + directoryName))
            {
                Directory.CreateDirectory(@"c:\log\" + directoryName);
            }

            fs = new FileStream(@"c:\log\"+directoryName+"\\" + fileName + fileExtention, FileMode.OpenOrCreate, FileAccess.Write);

            StreamWriter sw = new StreamWriter(fs);
            sw.BaseStream.Seek(0, SeekOrigin.End);
            sw.WriteLine("WindowsService: Service Started" + str + "\n");

            sw.Flush();
            sw.Close();
            fs.Close();
        }

    }
}
