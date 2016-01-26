using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class SourceScaleEntity
    {
        public string SourceID { get; set; }

        public string Name { get; set; }

        public int Value { get; set; }

        public string Scale { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }

    public class SourceDateEntity
    {
        public string SourceID { get; set; }

        public string Name { get; set; }

        public List<SourceDateItem> Items { get; set; }
    }

    public class SourceDateItem
    {
        public string Name { get; set; }

        public int Value { get; set; }
    }

    public class DateJson
    {
        public string name { get; set; }

        public int value { get; set; }
    }

    public class ReportCommonEntity
    {
        public string name { get; set; }

        public int iValue { get; set; }

        public decimal dValue { get; set; }

        public string desc { get; set; }

        public object value { get; set; }
    }

}
