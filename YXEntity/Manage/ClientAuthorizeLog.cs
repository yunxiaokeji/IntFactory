/**  版本信息模板在安装目录下，可自行修改。
* ClientAuthorize.cs
*
* 功 能： N/A
* 类 名： ClientAuthorize
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2015/3/6 19:52:53   N/A    初版
*
* Copyright (c) 2012 Maticsoft Corporation. All rights reserved.
*┌──────────────────────────────────┐
*│　此技术信息为本公司机密信息，未经本公司书面同意禁止向第三方披露．　│
*│　版权所有：动软卓越（北京）科技有限公司　　　　　　　　　　　　　　│
*└──────────────────────────────────┘
*/
using System;
namespace IntFactoryEntity.Manage
{
	/// <summary>
	/// ModulesMenu:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
    public partial class ClientAuthorizeLog
	{
        public ClientAuthorizeLog()
		{}
		#region Model
		private int _autoid;
        private string _clientID;
        private string _orderID;
        private int _userquantity;
        private DateTime? _begintime = DateTime.Now;
        private DateTime? _endtime = DateTime.Now;
        private int _status;
        private int _systemtype;
		private DateTime? _createtime= DateTime.Now;
		private string _createuserid;
        private DateTime? _updatetime= DateTime.Now;
        //private string _updateuserID;
		/// <summary>
		/// 
		/// </summary>
		public int AutoID
		{
			set{ _autoid=value;}
			get{return _autoid;}
		}
		/// <summary>
		/// 
		/// </summary>
        public string ClientID
		{
            set { _clientID = value; }
            get { return _clientID; }
		}

        public string AgentID
        {
            set;
            get;
        }

		/// <summary>
		/// 
		/// </summary>
        public string OrderID
		{
            set { _orderID = value; }
            get { return _orderID; }
		}
        /// <summary>
        /// 
        /// </summary>
        public int Type
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public int UserQuantity
        {
            set { _userquantity = value; }
            get { return _userquantity; }
        }
		/// <summary>
		/// 
		/// </summary>
        public DateTime? BeginTime
		{
            set { _begintime = value; }
            get { return _begintime; }
		}
        /// <summary>
        /// 
        /// </summary>
        public DateTime? EndTime
        {
            set { _endtime = value; }
            get { return _endtime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Status
        {
            set { _status = value; }
            get { return _status; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int SystemType
        {
            set { _systemtype = value; }
            get { return _systemtype; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateTime
        {
            set { _createtime = value; }
            get { return _createtime; }
        }
		/// <summary>
		/// 
		/// </summary>
		public string CreateUserID
		{
			set{ _createuserid=value;}
			get{return _createuserid;}
		}
        /// <summary>
        /// 
        /// </summary>
        //public DateTime? UpdateTime
        //{
        //    set { _updatetime = value; }
        //    get { return _updatetime; }
        //}
        /// <summary>
        /// 
        /// </summary>
        //public string UpdateUserID
        //{
        //    set { _updateuserID = value; }
        //    get { return _updateuserID; }
        //}
		#endregion Model

        /// <summary>
        /// 填充数据
        /// </summary>
        /// <param name="dr"></param>
        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
	}
}

