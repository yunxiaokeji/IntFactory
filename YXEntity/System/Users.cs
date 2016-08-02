/**  版本信息模板在安装目录下，可自行修改。
* Users.cs
*
* 功 能： N/A
* 类 名： Users
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2015/4/8 19:58:56   N/A    初版
*
* Copyright (c) 2012 Maticsoft Corporation. All rights reserved.
*┌──────────────────────────────────┐
*│　此技术信息为本公司机密信息，未经本公司书面同意禁止向第三方披露．　│
*│　版权所有：动软卓越（北京）科技有限公司　　　　　　　　　　　　　　│
*└──────────────────────────────────┘
*/
using System;
using System.Collections.Generic;
namespace IntFactoryEntity
{
	/// <summary>
	/// Users:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Users
	{
		public Users()
		{}



        public List<Menu> Menus { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public int AutoID{ set; get; }
		/// <summary>
		/// 
		/// </summary>
        [Property("Lower")]
		public string UserID{ set; get; }


        public string LoginName { get; set; }


		public string LoginPWD{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public string Name{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public string Email{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public string MobilePhone{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public string OfficePhone{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public string CityCode{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public string Address{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime Birthday{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public int? Age{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public int? Sex{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public int? Education{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public string Jobs{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public string Avatar{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        [Property("Lower")] 
		public string ParentID{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public int? Allocation{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public int? Status{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public string Description{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime EffectTime{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime TurnoverTime{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime CreateTime{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        [Property("Lower")] 
		public string CreateUserID{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        [Property("Lower")] 
		public string ClientID{ set; get; }

        public string FirstName 
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Name))
                    return Net.Sourceforge.Pinyin4j.PinyinHelper.ToHanyuPinyinString(this.Name, new Net.Sourceforge.Pinyin4j.Format.HanyuPinyinOutputFormat(), " ").ToCharArray()[0].ToString().ToUpper();
                else
                    return string.Empty;
            }
        }
        [Property("Lower")]
        public string TeamID { get; set; }

        public Users CreateUser { get; set; }

        [Property("Lower")] 
        public string DepartID { get; set; }

        public Department Department { get; set; }

        [Property("Lower")] 
        public string RoleID { get; set; }

        public Role Role { get; set; }

        public List<Users> ChildUsers { get; set; }

        public string LogGUID { get; set; }

        public Manage.Clients Client { get; set; }

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

