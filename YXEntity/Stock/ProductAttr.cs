
using System;
using System.Collections.Generic;
namespace IntFactoryEntity
{
	[Serializable]
	public partial class ProductAttr
	{
		public ProductAttr()
		{}

        [Property("Lower")]
        public string AttrID { get; set; }

        public string AttrName { get; set; }

        public string Description { get; set; }

        public int Type { get; set; }

        /// <summary>
        /// 属性值
        /// </summary>
        public List<AttrValue> AttrValues { get; set; }

        [Property("Lower")]
        public string CategoryID { get; set; }

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

