using IntFactoryDAL;
using IntFactoryEntity.HelpCenter;
using IntFactoryEnum.HelpCenter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactoryBusiness
{
    public class HelpCenterBusiness
    {
        public static HelpCenterBusiness BaseBusiness = new HelpCenterBusiness();

        #region 查询
        public TypeEntity GetTypesByTypeID(string typeID)
        {
            DataTable ds = HelpCenterDAL.BaseProvider.GetTypesByTypeID(typeID);
            TypeEntity model = new TypeEntity();
            foreach (DataRow dr in ds.Rows)
            {
                model.FillData(dr);
            }
            return model;
        }

        public List<TypeEntity> GetTypesByModuleType(ModuleTypeEnum moduleType)
        {
            List<TypeEntity> list = new List<TypeEntity>();
            DataTable dt = HelpCenterDAL.BaseProvider.GetTypesByModuleType((int)moduleType);
            foreach (DataRow dr in dt.Rows)
            {
                TypeEntity model = new TypeEntity();
                model.FillData(dr);
                list.Add(model);
            }

            return list;
        }

        public List<TypeEntity> GetTypes(int types, string keyWords, string beginTime, string endTime, string orderBy, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            List<TypeEntity> list = new List<TypeEntity>();
            DataSet ds = HelpCenterDAL.BaseProvider.GetTypes(types, keyWords, beginTime, endTime, orderBy, pageSize, pageIndex, ref totalCount, ref pageCount);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                TypeEntity model = new TypeEntity();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        public List<TypeEntity> GetTypes()
        {
            List<TypeEntity> list = new List<TypeEntity>();
            DataSet ds = HelpCenterDAL.BaseProvider.GetTypeList();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                TypeEntity model = new TypeEntity();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        public List<ContentEntity> GetContents(int moduleType, string typeID, string keyWords, string beginTime, string endTime, string orderBy, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            List<ContentEntity> list = new List<ContentEntity>();
            DataSet ds = HelpCenterDAL.BaseProvider.GetContents(moduleType, typeID, keyWords, beginTime, endTime, orderBy, pageSize, pageIndex, ref totalCount, ref pageCount);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ContentEntity model = new ContentEntity();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        public ContentEntity GetContentByContentID(string contentID)
        {
            ContentEntity item = new ContentEntity();
            DataTable dt = HelpCenterDAL.BaseProvider.GetContentByContentID(contentID);
            foreach (DataRow dr in dt.Rows)
            {
                item.FillData(dr);
            }

            return item;
        }

        #endregion


        #region 添加
        public int InsertType(string name, string remark, int moduleType, string img, string userID)
        {
            var typeID = Guid.NewGuid().ToString().ToLower();
            return HelpCenterDAL.BaseProvider.InsertType(typeID, name, remark, moduleType, img, userID);
        }

        public int InsertContent(string typeID, string sort, string title, string keyWords,string img, string content, string userID)
        {
            var contentID = Guid.NewGuid().ToString().ToLower();
            return HelpCenterDAL.BaseProvider.InsertContent(contentID, typeID, sort, title, keyWords,img, content, userID);
        }

        #endregion


        #region 编辑

        public bool UpdateType(string typeID, string name,string remark, string icon, int moduleType)
        {
            return HelpCenterDAL.BaseProvider.UpdateType(typeID, name,remark, icon, moduleType);
        }

        public bool UpdateContent(string contentID, string title, string sort, string keyWords, string content, string typeID)
        {
            return HelpCenterDAL.BaseProvider.UpdateContent(contentID, title, sort, keyWords, content, typeID);
        }
        #endregion


        #region 删除
        public int DeleteType(string typeID)
        {
            return HelpCenterDAL.BaseProvider.DeleteType(typeID);
        }

        public bool DeleteContent(string contentID)
        {
            return HelpCenterDAL.BaseProvider.DeleteContent(contentID);
        }
        #endregion
    }
}
