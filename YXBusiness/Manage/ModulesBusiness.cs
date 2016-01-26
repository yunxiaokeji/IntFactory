﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using IntFactoryDAL.Manage;
using IntFactoryEntity.Manage;

namespace IntFactoryBusiness
{
    public class ModulesBusiness
    {
        /// <summary>
        /// 获取模块列表
        /// </summary>
        /// <returns></returns>
        public static List<Modules> GetModules()
        {
            List<Modules> list = new List<Modules>();
            DataTable dt = new ModulesDAL().GetModules();
            foreach (DataRow dr in dt.Rows)
            {
                Modules model = new Modules();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        public static List<Modules> GetModulesByClientID(string clientID)
        {
            List<Modules> list = new List<Modules>();
            DataTable dt = new ModulesDAL().GetModulesByClientID(clientID);
            foreach (DataRow dr in dt.Rows)
            {
                Modules model = new Modules();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }
    }
}
