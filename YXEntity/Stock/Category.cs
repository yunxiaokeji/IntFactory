/**  版本信息模板在安装目录下，可自行修改。
* Category.cs
*
* 功 能： N/A
* 类 名： Category
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2015/5/3 13:38:16   N/A    初版
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
	/// Category:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
    [Serializable]
    public partial class Category
    {
        public Category()
        { }

        [Property("Lower")] 
        public string CategoryID { get; set; }


        public string CategoryCode { get; set; }

        public string CategoryName { get; set; }

        [Property("Lower")]
        public string PID { get; set; }

        [Property("Lower")]
        public string PIDList { get; set; }

        public int Layers { get; set; }

        public string SaleAttr { get; set; }

        public List<ProductAttr> SaleAttrs { get; set; }

        [Property("Lower")]
        public string AttrList { get; set; }

        public List<ProductAttr> AttrLists { get; set; }


        public int Status { get; set; }

        public string Description { get; set; }

        public int CategoryType { get; set; }

        public List<Category> ChildCategory { get; set; }

        /// <summary>
        /// 填充数据
        /// </summary>
        /// <param name="dr"></param>
        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }

    }


    public partial class OrderCategory
    {
        [Property("Lower")] 
        public string CategoryID { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}

