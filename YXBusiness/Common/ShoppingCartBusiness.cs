using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntFactoryEntity;
using IntFactoryEnum;
using IntFactoryDAL;

namespace IntFactoryBusiness
{
    public class ShoppingCartBusiness
    {
        public static int GetShoppingCartCount(EnumDocType ordertype, string guid)
        {
            if (ordertype == EnumDocType.Order)
            {
                object obj = CommonBusiness.Select("OrderDetail", "count(0)", "[OrderID]='" + guid + "'");
                return Convert.ToInt32(obj);
            }
            else
            {
                object obj = CommonBusiness.Select("ShoppingCart", "count(0)", "ordertype=" + (int)ordertype + " and [GUID]='" + guid + "'");
                return Convert.ToInt32(obj);
            }
        }
        /// <summary>
        /// 获取购物车列表
        /// </summary>
        public static List<ProductDetail> GetShoppingCart(EnumDocType ordertype, string guid, string userid)
        {
            DataTable dt = ShoppingCartDAL.GetShoppingCart((int)ordertype, guid, userid);
            List<ProductDetail> list = new List<ProductDetail>();
            foreach (DataRow dr in dt.Rows)
            {
                ProductDetail model = new ProductDetail();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        /// <summary>
        /// 加入购物车
        /// </summary>
        /// <returns></returns>
        public static bool AddShoppingCart(string productid, string detailsid, int quantity, string unitid, int isBigUnit, EnumDocType ordertype, string remark, string guid, string userid, string operateip)
        {
            if (string.IsNullOrEmpty(guid))
            {
                guid = userid;
            }
            return ShoppingCartDAL.AddShoppingCart(productid, detailsid, quantity, unitid, isBigUnit, (int)ordertype, remark, guid, userid, operateip);
        }

        public static bool AddShoppingCartBatchOut(string productid, string detailsid, int quantity, string batchcode, string depotid, EnumDocType ordertype, string remark, string guid, string userid, string operateip)
        {
            if (string.IsNullOrEmpty(guid))
            {
                guid = userid;
            }
            return ShoppingCartDAL.AddShoppingCartBatchOut(productid, detailsid, quantity, (int)ordertype, batchcode, depotid, remark, guid, userid, operateip);
        }

        public static bool AddShoppingCartBatchIn(string productid, string detailsid, int quantity, EnumDocType ordertype, string remark, string guid, string userid, string operateip)
        {
            if (string.IsNullOrEmpty(guid))
            {
                guid = userid;
            }
            return ShoppingCartDAL.AddShoppingCartBatchIn(productid, detailsid, quantity, (int)ordertype, remark, guid, userid, operateip);
        }

        /// <summary>
        /// 编辑购物车产品数量
        /// </summary>
        /// <param name="autoid"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public static bool UpdateCartQuantity(string autoid, int quantity, string userid)
        {
            return ShoppingCartDAL.UpdateCartQuantity(autoid, quantity, userid);
        }

        public static bool UpdateCartBatch(string autoid, string batch, string userid)
        {
            return CommonBusiness.Update("ShoppingCart", "BatchCode", batch, "AutoID=" + autoid);
        }

        public static bool UpdateCartPrice(string autoid, decimal price, string userid)
        {
            return ShoppingCartDAL.UpdateCartPrice(autoid, price, userid);
        }

        /// <summary>
        /// 删除购物车记录
        /// </summary>
        /// <param name="autoid"></param>
        /// <param name="userid"></param>
        /// <param name="clientid"></param>
        /// <returns></returns>
        public static bool DeleteCart(string autoid, string userid)
        {
            return ShoppingCartDAL.DeleteCart(autoid, userid);
        }
    }
}
