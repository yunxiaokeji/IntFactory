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
    public class ManageSystemBusiness
    {
        private static List<M_Role> _cacheRoles;
        /// <summary>
        /// 缓存角色信息
        /// </summary>
        private static List<M_Role> Roles
        {
            get
            {
                if (_cacheRoles == null)
                {
                    _cacheRoles = new List<M_Role>();
                }
                return _cacheRoles;
            }
            set
            {
                _cacheRoles = value;
            }
        }

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
            if (dt.Rows.Count > 0)
            {
                item.FillData(dt.Rows[0]);
                if (!string.IsNullOrEmpty(item.PCode))
                    item.PCodeName = GetSystemMenu(item.PCode).Name;
            }

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


        /// <summary>
        /// 获取角色列表
        /// </summary>
        public static List<M_Role> GetRoles()
        {
            if (Roles.Count==0)
            {
                DataTable dt = new SystemDAL().GetRoles();
                List<M_Role> list = new List<M_Role>();
                foreach (DataRow dr in dt.Rows)
                {
                    M_Role model = new M_Role();
                    model.FillData(dr);
                    list.Add(model);
                }

                Roles = list;

                return list;
            }

            return Roles;
        }

        /// <summary>
        /// 根据ID获取角色
        /// </summary>
        /// <param name="roleid"></param>
        /// <returns></returns>
        public static M_Role GetRoleByIDCache(string roleid)
        {
            return GetRoles().Where(r => r.RoleID == roleid).FirstOrDefault();
        }

        /// <summary>
        /// 获取角色详情（权限明细）
        /// </summary>
        /// <param name="roleid"></param>
        /// <returns></returns>
        public static M_Role GetRoleByID(string roleid)
        {
            M_Role model = null;
            DataSet ds = SystemDAL.BaseProvider.GetRoleByID(roleid);
            if (ds.Tables.Contains("Role") && ds.Tables["Role"].Rows.Count > 0)
            {
                model = new M_Role();
                model.FillData(ds.Tables["Role"].Rows[0]);
                model.Menus = new List<Menu>();
                foreach (DataRow dr in ds.Tables["Menus"].Rows)
                {
                    Menu menu = new Menu();
                    menu.FillData(dr);
                    model.Menus.Add(menu);
                }
            }
            return model;
        }

        public string CreateRole(string name, string parentid, string description, string operateid)
        {
            string roleid = Guid.NewGuid().ToString();
            bool bl = SystemDAL.BaseProvider.CreateRole(roleid, name, parentid, description, operateid);
            if (bl)
            {
                //处理缓存
                var role = new M_Role()
                {
                    RoleID = roleid,
                    Name = name,
                    Description = description,
                    CreateTime = DateTime.Now,
                    CreateUserID = operateid,
                    Status = 1,
                    IsDefault = 0
                };
                Roles.Add(role);

                return roleid;
            }

            return "";
        }

        /// <summary>
        /// 编辑角色
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="name">名称</param>
        /// <param name="description">描述</param>
        /// <param name="operateid">操作人</param>
        /// <param name="ip">IP</param>
        /// <returns></returns>
        public bool UpdateRole(string roleid, string name, string description, string operateid)
        {
            bool bl = SystemDAL.BaseProvider.UpdateRole(roleid, name, description);
            if (bl)
            {
                //处理缓存
                var model = GetRoles().Where(d => d.RoleID == roleid).FirstOrDefault();
                model.Name = name;
                model.Description = description;
            }
            return bl;
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="operateid"></param>
        /// <param name="ip"></param>
        /// <param name="result">0 失败 1成功 10002 存在员工</param>
        /// <returns></returns>
        public bool DeleteRole(string roleid, string operateid, string ip, out int result)
        {
            bool bl = SystemDAL.BaseProvider.DeleteRole(roleid, out result);
            if (bl)
            {
               // var model = GetRoles().Where(d => d.RoleID == roleid).FirstOrDefault();
                Roles.RemoveAll(x => x.RoleID == roleid);
                //model.Status = 9;
            }
            return bl;
        }
        /// <summary>
        /// 编辑角色权限
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="permissions"></param>
        /// <param name="operateid"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public bool UpdateRolePermission(string roleid, string permissions, string operateid, string ip)
        {
            return SystemDAL.BaseProvider.UpdateRolePermission(roleid, permissions, operateid);
        }

        /// <summary>
        /// 编辑员工角色
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="roleid"></param>
        /// <param name="operateid"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public bool UpdateUserRole(string userid, string roleid, string operateid, string ip)
        {
                bool bl = SystemDAL.BaseProvider.UpdateUserRole(userid, roleid, operateid);

                return bl;
        }

    }
}
