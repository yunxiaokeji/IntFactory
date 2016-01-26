using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IntFactoryDAL.Manage;
using IntFactoryEntity.Manage;
using IntFactoryEntity;
using System.Data;

namespace IntFactoryBusiness.Manage
{
    public class IndustryBusiness
    {
        #region Cache

        private static List<Industry> _industrys;

        /// <summary>
        /// 行业
        /// </summary>
        private static List<Industry> Industrys
        {
            get
            {
                if (_industrys == null)
                {
                    _industrys = new List<Industry>();
                }
                return _industrys;
            }
            set
            {
                _industrys = value;
            }
        }

        #endregion

        #region 查询

        /// <summary>
        /// 获取行业列表
        /// </summary>
        /// <param name="clientid"></param>
        /// <returns></returns>
        public static List<Industry> GetIndustrys()
        {
            if (IndustryBusiness.Industrys != null && IndustryBusiness.Industrys.Count > 0)
            {
                return IndustryBusiness.Industrys;
            }
            else
            {
                List<Industry> list = new List<Industry>();
                DataTable dt = new IndustryDAL().GetIndustrys();
                foreach (DataRow item in dt.Rows)
                {
                    Industry model = new Industry();
                    model.FillData(item);
                    list.Add(model);
                }
                IndustryBusiness.Industrys = list;
                return list;
            }
        }

        public static Industry GetIndustryDetail(string id)
        {
            var list = GetIndustrys();
            if (list.Where(m => m.IndustryID == id).Count() > 0)
            {
                return list.Where(m => m.IndustryID == id).FirstOrDefault();
            }
            Industry model = new Industry();
            DataTable dt = new IndustryDAL().GetIndustryDetail(id);
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
                IndustryBusiness.Industrys.Add(model);
            }

            return model;
        }
        #endregion

        #region 添加

        /// <summary>
        /// 添加行业
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="description">描述</param>
        /// <param name="userid">操作人</param>
        /// <param name="clientid">客户端ID</param>
        /// <returns>行业ID</returns>
        public static string InsertIndustry(string name, string description, string userid, string clientid)
        {
            string id = new IndustryDAL().InsertIndustry(name, description, userid, clientid);
            //处理缓存
            if (!string.IsNullOrEmpty(id))
            {
                IndustryBusiness.Industrys.Add(new Industry()
                {
                    IndustryID = id,
                    Name = name,
                    CreateUserID = userid,
                    ClientID = clientid,
                    CreateTime = DateTime.Now,
                    Description = description
                });
            }
            return id;
        }

        #endregion

        #region 添加

        /// <summary>
        /// 编辑行业
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="description">描述</param>
        public static bool UpdateIndustry(string id, string name, string description)
        {
            bool flag = new IndustryDAL().UpdateIndustry(id,name, description);
            //处理缓存
            if (flag)
            {
                Industry industry = IndustryBusiness.Industrys.Find(m => m.IndustryID == id);
                industry.Name = name;
                industry.Description = description; 
            }

            return flag;
        }

        #endregion
    }
}
