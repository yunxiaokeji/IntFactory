using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntFactoryEntity.Manage;
using System.Data;
using IntFactoryDAL.Manage;
namespace IntFactoryEntity.Manage
{
    public class SystemBusiness
    {
        public static List<SystemMenu> GetSystemMenus() {
            List<SystemMenu> list = new List<SystemMenu>();
            DataTable dt = SystemDAL.BaseProvider.GetSystemMenus();
            SystemMenu model;

            foreach (DataRow item in dt.Rows)
            {
                model = new SystemMenu();
                model.FillData(item);

                if (!string.IsNullOrEmpty(model.PCode))
                    model.PCodeName = GetSystemMenu(model.PCode).Name;

                list.Add(model);
            }

            return list;
        }

        public static SystemMenu GetSystemMenu(string menuCode)
        {

            SystemMenu item = new SystemMenu();
            DataTable dt = SystemDAL.BaseProvider.GetSystemMenu(menuCode);
            item.FillData(dt.Rows[0]);
            if (!string.IsNullOrEmpty(item.PCode))
                item.PCodeName = GetSystemMenu(item.PCode).Name;

            return item;
        }


        public static bool AddSystemMenu(SystemMenu systemMenu)
        {
            return SystemDAL.BaseProvider.AddSystemMenu(systemMenu.MenuCode,systemMenu.Name,systemMenu.Controller,systemMenu.View,systemMenu.PCode,systemMenu.Sort,systemMenu.Layer,systemMenu.Type);
        }

        public static bool UpdateSystemMenu(SystemMenu systemMenu)
        {
            return SystemDAL.BaseProvider.UpdateSystemMenu(systemMenu.MenuCode, systemMenu.Name, systemMenu.Controller, systemMenu.View, systemMenu.Sort);
        }

        public static bool DeleteSystemMenu(string menuCode)
        {
            return SystemDAL.BaseProvider.DeleteSystemMenu(menuCode);
        }
    }
}
