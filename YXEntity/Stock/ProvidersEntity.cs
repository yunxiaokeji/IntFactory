﻿/**  版本信息模板在安装目录下，可自行修改。
* Brand.cs
*
* 功 能： N/A
* 类 名： Brand
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2015/5/3 13:38:15   N/A    初版
*
* Copyright (c) 2012 Maticsoft Corporation. All rights reserved.
*┌──────────────────────────────────┐
*│　此技术信息为本公司机密信息，未经本公司书面同意禁止向第三方披露．　│
*│　版权所有：动软卓越（北京）科技有限公司　　　　　　　　　　　　　　│
*└──────────────────────────────────┘
*/
using System;
namespace IntFactoryEntity
{
	/// <summary>
	/// Brand:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class ProvidersEntity
	{
        public ProvidersEntity()
		{}
		#region Model
		private int _autoid;
		private string _brandid;
		private string _name;
		private string _citycode="";
		private int _status=1;
		private string _remark="";
		private string _createuserid;
		private DateTime? _createtime= DateTime.Now;
		private DateTime? _updatetime= DateTime.Now;
		private string _operateip="";
		private string _clientid;
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
        [Property("Lower")]
        public string ProviderID
		{
			set{ _brandid=value;}
			get{return _brandid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Name
		{
			set{ _name=value;}
			get{return _name;}
		}

        public string Contact { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string CityCode
		{
			set{ _citycode=value;}
			get{return _citycode;}
		}

        public string Address { get; set; }

        public CityEntity City { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public int Status
		{
			set{ _status=value;}
			get{return _status;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Remark
		{
			set{ _remark=value;}
			get{return _remark;}
		}

        [Property("Lower")] 
		public string CreateUserID
		{
			set{ _createuserid=value;}
			get{return _createuserid;}
		}
        public Users CreateUser { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public DateTime? CreateTime
		{
			set{ _createtime=value;}
			get{return _createtime;}
		}

        public string MobileTele { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string OperateIP
		{
			set{ _operateip=value;}
			get{return _operateip;}
		}
        public string AgentID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Property("Lower")] 
		public string ClientID
		{
			set{ _clientid=value;}
			get{return _clientid;}
		}
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

