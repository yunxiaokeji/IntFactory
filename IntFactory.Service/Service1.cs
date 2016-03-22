﻿using System;
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
using IntFactoryEnum;
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
            DownAliOrdersTimer.Interval =60 * 1000;
            DownAliOrdersTimer.Elapsed += new System.Timers.ElapsedEventHandler(DownAliOrdersEvent); //到达时间的时候执行事件；   
            DownAliOrdersTimer.AutoReset = true;   //设置是执行一次（false）还是一直执行(true)；   
            DownAliOrdersTimer.Enabled = true;     //是否执行System.Timers.Timer.Elapsed事件；   

            UpdateAliOrdersTimer.Interval = 60 * 1000;
            UpdateAliOrdersTimer.Elapsed += new System.Timers.ElapsedEventHandler(UpdateAliOrdersEvent); //到达时间的时候执行事件；   
            UpdateAliOrdersTimer.AutoReset = true;   //设置是执行一次（false）还是一直执行(true)；   
            UpdateAliOrdersTimer.Enabled = true;     //是否执行System.Timers.Timer.Elapsed事件；   

            // TODO: 在此处添加代码以启动服务。
            string state = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + " 下载阿里订单服务启动";
            WriteLog(state);
            DownAliOrdersTimer.Start();

            state = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "    更新阿里订单服务启动";
            WriteLog(state, 2);
            UpdateAliOrdersTimer.Start();
        }

        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
            string state = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + " 下载阿里订单服务停止";
            WriteLog(state);
            DownAliOrdersTimer.Stop();

            state = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "    更新阿里订单服务停止";
            WriteLog(state, 2);
            UpdateAliOrdersTimer.Stop();
        }


        public void DownAliOrdersEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            string state = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "     DownAliOrdersEvent开启";
            WriteLog(state);

            bool flag= ExecuteDownAliOrdersPlan();

            state = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "    DownAliOrdersEvent结果:" + (flag ? "成功" : "失败")+"\n";
            WriteLog(state);
        }

        public void UpdateAliOrdersEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            string state = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "     UpdateAliOrdersEvent开启";
            WriteLog(state,2);

            bool flag=ExecuteUpdateAliOrders();

            state = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "    UpdateAliOrdersEvent结果:" + (flag ? "成功" : "失败") + "\n";
            WriteLog(state, 2);
        }

        /// <summary>
        /// 执行下载阿里订单计划
        /// </summary>
        /// <returns></returns>
        public bool ExecuteDownAliOrdersPlan()
        {
            int successCount, total;
            //获取阿里订单下载计划列表
            var list = AliOrderBusiness.BaseBusiness.GetAliOrderDownloadPlans();

            foreach (var item in list)
            {
                string error;

                //下载阿里打样订单
                var gmtFentEnd = DateTime.Now;
                bool flag =AliOrderBusiness.DownFentOrders(item.FentSuccessEndTime, gmtFentEnd, item.Token, item.RefreshToken,
                    item.UserID, item.AgentID, item.ClientID, out successCount, out total, out error);

                //新增阿里打样订单下载日志
                AliOrderBusiness.BaseBusiness.AddAliOrderDownloadLog( EnumOrderType.ProofOrder, flag, AlibabaSdk.AliOrderDownType.Auto, item.FentSuccessEndTime, gmtFentEnd,
                    successCount, total, item.AgentID, item.ClientID, error);

                //添加服务日志
                string state = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "    ClientID:"+item.ClientID+" 下载打样订单结果:" + (flag ? "成功" : "失败");
                if (!flag)
                    state += "  原因："+error;
                WriteLog(state); 


                //下载阿里大货订单列表
                var gmtBulkEnd = DateTime.Now;
                flag = AliOrderBusiness.DownBulkOrders(item.BulkSuccessEndTime, gmtBulkEnd, item.Token, item.RefreshToken,
                    item.UserID, item.AgentID, item.ClientID, out successCount, out total, out error);

                //新增阿里大货订单下载日志
                AliOrderBusiness.BaseBusiness.AddAliOrderDownloadLog(EnumOrderType.LargeOrder, flag, AlibabaSdk.AliOrderDownType.Auto, item.BulkSuccessEndTime, gmtBulkEnd,
                    successCount, total, item.AgentID, item.ClientID, error);

                //添加服务日志
                state = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "    ClientID:" + item.ClientID + " 下载大货订单结果:" + (flag ? "成功" : "失败");
                if (!flag)
                    state += "  原因：" + error;
                WriteLog(state); 
            }

            return true;
        }

        /// <summary>
        /// 执行更新阿里订单
        /// </summary>
        /// <returns></returns>
        public bool ExecuteUpdateAliOrders()
        {
            //获取阿里订单下载计划列表
            var list = AliOrderBusiness.BaseBusiness.GetAliOrderDownloadPlans();

            foreach (var item in list)
            {
                List<string> failGodesCodes;
                bool flag = false;
                //批量更新打样订单
                flag=AliOrderBusiness.UpdateAliFentOrders(item.ClientID, item.Token, item.RefreshToken, out failGodesCodes);

                //添加服务日志
                string state = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "    ClientID:" + item.ClientID + " 更新打样订单结果:" + (flag ? "成功" : "失败");
                if (!flag)
                    state += "  不成功订单编码：" + string.Join(",", failGodesCodes.ToArray());
                WriteLog(state,2); 

                //批量更新大货订单
                flag=AliOrderBusiness.UpdateAliBulkOrders(item.ClientID, item.Token, item.RefreshToken, out failGodesCodes);

                //添加服务日志
                state = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "    ClientID:" + item.ClientID + " 更新大货订单结果:" + (flag ? "成功" : "失败");
                if (!flag)
                    state += "  不成功订单编码：" + string.Join(",", failGodesCodes.ToArray());
                WriteLog(state,2); 
            }

            return true;
        }

        /// <summary>
        /// 添加服务日志
        /// </summary>
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
