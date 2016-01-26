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

namespace IntFactor.Service
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 在此处添加代码以启动服务。
            string state = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "启动";  
            WriteLog(state);

            //timer_downdata.Start();
        }

        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
            string state = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "停止";
            WriteLog(state);

            //timer_downdata.Stop();
        }

        private void timer_downdata_Tick(object sender, EventArgs e)
        {
            WriteLog(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            
        }

        public void WriteLog(string str)  
        { 
            //using (StreamWriter sw = File.AppendText(@"c:\service.txt"))  
            //{  
            //    sw.WriteLine(str);  
            //    sw.Flush();  
            //}

            FileStream fs = new FileStream(@"c:\xx.txt", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.BaseStream.Seek(0, SeekOrigin.End);
            sw.WriteLine("WindowsService: Service Started" + str + "\n");

            sw.Flush();
            sw.Close();
            fs.Close();
        }

    }
}
