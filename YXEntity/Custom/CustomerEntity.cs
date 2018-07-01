using System;
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

        public int SourceType { get; set; }

        [Property("Lower")]
        public string SourceID { get; set; }

        [Property("Lower")]
        public string OwnerID { get; set; }

        public Users Owner { get; set; }

        public int Status { get; set; }

        public int Mark { get; set; }

        public List<ContactEntity> Contacts { get; set; }

        public DateTime CreateTime { get; set; }

        [Property("Lower")]
        public string CreateUserID { get; set; }

        public Users CreateUser { get; set; }

        [Property("Lower")]
        public string ClientID { get; set; }

        [Property("Lower")]
        public string YXAgentID { get; set; }

        [Property("Lower")]
        public string YXClientID { get; set; }

        [Property("Lower")]
        public string YXClientCode { get; set; }

        public string FirstName
        {
            get;
            set;
            //get
            //{
            //    if (!string.IsNullOrEmpty(this.Name))
            //        return Net.Sourceforge.Pinyin4j.PinyinHelper.ToHanyuPinyinString(this.Name, new Net.Sourceforge.Pinyin4j.Format.HanyuPinyinOutputFormat(), " ").ToCharArray()[0].ToString().ToUpper();
            //    else
            //        return string.Empty;
            //}
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

        public int DemandCount { get; set; }

        public int DYCount { get; set; }

        public int DHCount { get; set; }

        public List<CustomerMemberEntity> Members { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }

    public class CustomerMemberEntity
    {
        public string CustomerID { get; set; }

        public string MemberID { get; set; }

        public string Name { get; set; }

        public string ClientID { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
