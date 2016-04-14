using IntFactoryDAL;
using IntFactoryEntity;
using IntFactoryEnum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO; 

namespace IntFactoryBusiness
{

    public class CommonBusiness
    {
        #region Cache

        private static List<CityEntity> _citys;
        /// <summary>
        /// 城市
        /// </summary>
        public static List<CityEntity> Citys
        {
            get
            {
                if (_citys == null)
                {
                    DataTable dt = new CommonDAL().GetCitys();
                    _citys = new List<CityEntity>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        CityEntity model = new CityEntity();
                        model.FillData(dr);
                        _citys.Add(model);
                    }
                }
                return _citys;
            }
        }

        private static List<Menu> _clientMenus;
        /// <summary>
        /// 客户端菜单
        /// </summary>
        public static List<Menu> ClientMenus
        {
            get
            {
                if (_clientMenus == null)
                {
                    _clientMenus = new List<Menu>();
                    DataTable dt = new CommonDAL().GetMenus();
                    foreach (DataRow dr in dt.Rows)
                    {
                        Menu model = new Menu();
                        model.FillData(dr);
                        _clientMenus.Add(model);
                    }
                    foreach (var menu in _clientMenus)
                    {
                        menu.ChildMenus = _clientMenus.Where(m => m.PCode == menu.MenuCode).ToList();
                    }

                }
                return _clientMenus;
            }
            set
            {
                _clientMenus = value;
            }
        }

        private static List<Menu> _manageMenus;
        /// <summary>
        /// 后台端菜单
        /// </summary>
        public static List<Menu> ManageMenus
        {
            get
            {
                if (_manageMenus == null)
                {
                    _manageMenus = new List<Menu>();
                    DataTable dt = new CommonDAL().GetManageMenus();
                    foreach (DataRow dr in dt.Rows)
                    {
                        Menu model = new Menu();
                        model.FillData(dr);
                        _manageMenus.Add(model);
                    }
                    foreach (var menu in _manageMenus.Where(m => m.Layer == 3))
                    {
                        menu.ChildMenus = _manageMenus.Where(m => m.PCode == menu.MenuCode).ToList();
                    }
                    foreach (var menu in _manageMenus.Where(m => m.Layer == 2))
                    {
                        menu.ChildMenus = _manageMenus.Where(m => m.PCode == menu.MenuCode).ToList();
                    }
                    foreach (var menu in _manageMenus.Where(m => m.Layer == 1))
                    {
                        menu.ChildMenus = _manageMenus.Where(m => m.PCode == menu.MenuCode).ToList();
                    }

                }
                return _manageMenus;
            }
            set
            {
                _manageMenus = value;
            }
        }
        #endregion


        public static CityEntity GetCityByCode(string citycode)
        {
            if (string.IsNullOrEmpty(citycode))
            {
                return null;
            }
            return Citys.Where(m => m.CityCode == citycode).FirstOrDefault();
        }

        /// <summary>
        /// 修改表中某字段值
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columnName">字段名</param>
        /// <param name="columnValue">字段值</param>
        /// <param name="where">条件</param>
        /// <returns></returns>
        public static bool Update(string tableName, string columnName, object columnValue, string where)
        {
            int obj = CommonDAL.Update(tableName, columnName, columnValue.ToString(), where);
            return obj > 0;
        }

        /// <summary>
        /// 获取表中某字段值
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columnName">字段名</param>
        /// <param name="columnValue">字段值</param>
        /// <param name="where">条件</param>
        /// <returns></returns>
        public static object Select(string tableName, string columnName, string where)
        {
            object obj = CommonDAL.Select(tableName, columnName, where);
            return obj;
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="where">条件</param>
        /// <returns></returns>
        public static bool Delete(string tableName, string where)
        {
            int obj = CommonDAL.Delete(tableName, where);
            return obj > 0;
        }

        /// <summary>
        /// 获取分页数据集合
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columns">列明</param>
        /// <param name="condition">条件</param>
        /// <param name="key">主键，分页条件</param>
        /// <param name="orderColumn">排序字段</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="pageCount">当前页数</param>
        /// <param name="totalNum">总记录数</param>
        /// <param name="totalPage">总页数</param>
        /// <param name="isAsc">主键是否升序</param>
        /// <returns></returns>
        public static DataTable GetPagerData(string tableName, string columns, string condition, string key, string orderColumn, int pageSize, int pageIndex, out int totalNum, out int pageCount, bool isAsc)
        {
            int asc = 0;
            if (isAsc)
            {
                asc = 1;
            }
            return CommonDAL.GetPagerData(tableName, columns, condition, key, orderColumn, pageSize, pageIndex, out totalNum, out pageCount, asc);
        }
        
        /// <summary>
        /// 获取分页数据集合(默认降序)
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columns">列明</param>
        /// <param name="condition">条件</param>
        /// <param name="key">主键，分页条件</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="pageCount">当前页数</param>
        /// <param name="totalNum">总记录数</param>
        /// <param name="totalPage">总页数</param>
        /// <returns></returns>
        public static DataTable GetPagerData(string tableName, string columns, string condition, string key, int pageSize, int pageIndex, out int totalNum, out int pageCount)
        {
            return CommonDAL.GetPagerData(tableName, columns, condition, key, "", pageSize, pageIndex, out totalNum, out pageCount, 0);
        }
        
        /// <summary>
        /// 获取分页数据集合(默认降序)
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columns">列明</param>
        /// <param name="condition">条件</param>
        /// <param name="key">主键，分页条件</param>
        /// <param name="orderColumn">排序字段</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="pageCount">当前页数</param>
        /// <param name="totalNum">总记录数</param>
        /// <param name="totalPage">总页数</param>
        /// <returns></returns>
        public static DataTable GetPagerData(string tableName, string columns, string condition, string key, string orderColumn, int pageSize, int pageIndex, out int totalNum, out int pageCount)
        {
            return CommonDAL.GetPagerData(tableName, columns, condition, key, orderColumn, pageSize, pageIndex, out totalNum, out pageCount, 0);
        }

        /// <summary>
        /// 获取分页数据集合
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columns">列明</param>
        /// <param name="condition">条件</param>
        /// <param name="key">主键，分页条件</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="pageCount">当前页数</param>
        /// <param name="totalNum">总记录数</param>
        /// <param name="totalPage">总页数</param>
        /// <param name="isAsc">主键是否升序</param>
        /// <returns></returns>
        public static DataTable GetPagerData(string tableName, string columns, string condition, string key, int pageSize, int pageIndex, out int totalNum, out int pageCount, bool isAsc)
        {
            int asc = 0;
            if (isAsc)
            {
                asc = 1;
            }
            return CommonDAL.GetPagerData(tableName, columns, condition, key, "", pageSize, pageIndex, out totalNum, out pageCount, asc);
        }

        /// <summary>
        /// 获取枚举描述
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Enumtype"></param>
        /// <returns></returns>
        public static string GetEnumDesc<T>(T Enumtype)
        {
            if (Enumtype == null) throw new ArgumentNullException("Enumtype");
            if (!Enumtype.GetType().IsEnum) throw new Exception("参数类型不正确");
            return ((DescriptionAttribute)Enumtype.GetType().GetField(Enumtype.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false)[0]).Description;
        }

        /// <summary>
        /// 压缩图片
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="quality"></param>
        /// <param name="multiple"></param>
        /// <param name="outputFile"></param>
        /// <returns></returns>
        public static bool GetThumImage(string sourceFile, long quality, int width, string outputFile)  
        {  
            try  
            {  
                long imageQuality = quality;  
                Bitmap sourceImage = new Bitmap(sourceFile);  
                ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");  
                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;  
                EncoderParameters myEncoderParameters = new EncoderParameters(1);  
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, imageQuality);  
                myEncoderParameters.Param[0] = myEncoderParameter;  
                float xWidth = sourceImage.Width;  
                float yHeight = sourceImage.Height;
                if (xWidth > yHeight)
                {
                    xWidth = width * xWidth / yHeight;
                    yHeight = width;
                }
                else
                {
                    xWidth = width;
                    yHeight = width * yHeight / xWidth;
                }
                Bitmap newImage = new Bitmap((int)xWidth, (int)yHeight);  
                Graphics g = Graphics.FromImage(newImage);

                g.DrawImage(sourceImage, 0, 0, xWidth, yHeight);  
                g.Dispose();  
                newImage.Save(outputFile, myImageCodecInfo, myEncoderParameters);  
                return true;  
            }  
            catch  
            {  
                return false;  
            }  
        }   
  
        /**/  
        /// <summary>  
        /// 获取图片编码信息  
        /// </summary>  
        private static ImageCodecInfo GetEncoderInfo(String mimeType)  
        {  
            int j;  
            ImageCodecInfo[] encoders;  
            encoders = ImageCodecInfo.GetImageEncoders();  
            for (j = 0; j < encoders.Length; ++j)  
            {  
                if (encoders[j].MimeType == mimeType)  
                    return encoders[j];  
            }  
            return null;  
        } 
    }
}
