
using System;
namespace IntFactoryEntity
{
	[Serializable]
	public partial class AttrValue
	{
		public AttrValue()
		{}

        [Property("Lower")]
        public string ValueID { get; set; }

        public string ValueName { get; set; }

        [Property("Lower")]
        public string AttrID { get; set; }


        public int Sort { get; set; }

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

