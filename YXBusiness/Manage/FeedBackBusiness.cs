using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntFactoryDAL.Manage;
using IntFactoryEntity.Manage;
using System.IO;
using System.Web;
using System.Data;
namespace IntFactoryBusiness.Manage
{
    public class FeedBackBusiness
    {
        public static string FilePath = CloudSalesTool.AppSettings.Settings["UploadFilePath"];

        #region 增
        /// <summary>
        /// 新增建议反馈
        /// </summary>
        public static bool InsertFeedBack(FeedBack model)
        {
            string filePath = model.FilePath;
            if (!string.IsNullOrEmpty(filePath))
            {
                if (filePath.IndexOf("?") > 0)
                {
                    filePath = filePath.Substring(0, filePath.IndexOf("?"));
                }
                FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(filePath));
                filePath = FilePath + file.Name;
                if (file.Exists)
                {
                    file.MoveTo(HttpContext.Current.Server.MapPath(filePath));
                }
            }

            model.FilePath = filePath;
            return FeedBackDAL.BaseProvider.InsertFeedBack(model.Title, model.ContactName, model.MobilePhone, model.Type, model.FilePath, model.Remark, model.CreateUserID);
        }
        #endregion

        #region 查
        public static List<FeedBack> GetFeedBacks(string keywords, string beginDate, string endDate, int type, int status, int pageSize, int pageIndex, out int totalCount, out int pageCount)
        {
            string sqlWhere = "1=1";
            if (type != -1)
            {
                sqlWhere += " and type="+type;
            }

            if (status != -1)
            {
                sqlWhere += " and status="+status;
            }

            if (!string.IsNullOrEmpty(keywords))
            {
                sqlWhere += " and (Title like '%"+keywords+"%'";
                sqlWhere += " or ContactName like '%" + keywords + "%'";
                sqlWhere += " or MobilePhone like '%" + keywords + "%'";
                sqlWhere += " or Remark like '%" + keywords + "%' )";
            }

            if (!string.IsNullOrEmpty(beginDate))
            {
                sqlWhere += " and createtime>=" + beginDate;
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                sqlWhere += " and createtime<=" +DateTime.Parse(endDate).AddDays(1);
            }

            DataTable dt = CommonBusiness.GetPagerData("FeedBack", "*", sqlWhere, "AutoID", pageSize, pageIndex, out totalCount, out pageCount);
            
            List<FeedBack> list = new List<FeedBack>();
            FeedBack model;
            foreach (DataRow item in dt.Rows)
            {
                model = new FeedBack();
                model.FillData(item);
                list.Add(model);
            }

            return list;
        }

        public static FeedBack GetFeedBackDetail(string id)
        {
            DataTable dt = FeedBackDAL.BaseProvider.GetFeedBackDetail(id);
            FeedBack model= new FeedBack();
            if(dt.Rows.Count>0)
                model.FillData(dt.Rows[0]);

            return model;
        }
        #endregion

        #region 改
        public static bool UpdateFeedBackStatus(string id, int status)
        {
           return FeedBackDAL.BaseProvider.UpdateFeedBackStatus(id, status);
        }
        #endregion
    }
}
