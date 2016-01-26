﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class CustomerEntity
    {
        [Property("Lower")]
        public string CustomerID { get; set; }

        public string Name { get; set; }

        public int Type { get; set; }

        [Property("Lower")]
        public string IndustryID { get; set; }

        public Industry Industry { get; set; }

        public int Extent { get; set; }
        public string ExtentStr { get; set; }

        public string CityCode { get; set; }

        public CityEntity City { get; set; }

        public string Address { get; set; }

        public string ContactName { get; set; }

        public string MobilePhone { get; set; }

        public string OfficePhone { get; set; }

        public string Email { get; set; }

        public string Jobs { get; set; }

        public DateTime Birthday { get; set; }

        public int Age { get; set; }

        public int Sex { get; set; }

        public int Education { get; set; }

        public string Description { get; set; }

        [Property("Lower")]
        public string SourceID { get; set; }
        public CustomSourceEntity Source { get; set; }

        [Property("Lower")]
        public string ActivityID { get; set; }

        public ActivityEntity Activity { get; set; }

        [Property("Lower")]
        public string StageID { get; set; }

        public CustomStageEntity Stage { get; set; }

        [Property("Lower")]
        public string OwnerID { get; set; }

        public Users Owner { get; set; }

        public int Status { get; set; }

        public int Mark { get; set; }

        public List<ContactEntity> Contacts { get; set; }

        public DateTime AllocationTime { get; set; }

        public DateTime OrderTime { get; set; }

        public DateTime CreateTime { get; set; }

        [Property("Lower")]
        public string CreateUserID { get; set; }

        public Users CreateUser { get; set; }

        [Property("Lower")]
        public string AgentID { get; set; }

        [Property("Lower")]
        public string ClientID { get; set; }

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

        /// <summary>
        /// 编辑加载列表用
        /// </summary>
        public List<Industry> Industrys { get; set; }
        /// <summary>
        /// 编辑加载列表用
        /// </summary>
        public List<ExtentEntity> Extents { get; set; }


        public int ReplyTimes { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
