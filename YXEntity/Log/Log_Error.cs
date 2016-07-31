﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity.Log
{
    /// <summary>
    /// 错误日志
    /// </summary>
    public class Log_Error
    {
        public int AutoID { get; set; }

        [Property("Lower")]
        public string UserID { get; set; }

        public string Message { get; set; }

        public int SystemType { get; set; }

        public DateTime CreateTime { get; set; }

        public string OpreateIP { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
