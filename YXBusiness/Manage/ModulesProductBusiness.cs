using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntFactoryEntity.Manage;
using IntFactoryDAL.Manage;
using System.Data;
namespace IntFactoryBusiness.Manage
{
    public class ModulesProductBusiness
    {
        #region  增
        public static bool InsertModulesProduct(ModulesProduct model)
        {
            string guid = Guid.NewGuid().ToString();
            return ModulesProductDAL.BaseProvider.InsertModulesProduct(guid, model.Period, model.PeriodQuantity, model.UserQuantity,
                model.Price, model.Description,model.Type,model.IsChild, model.CreateUserID);
        }
        #endregion

        #region  删
        public static bool DeleteModulesProduct(int id)
        {
            return ModulesProductDAL.BaseProvider.DeleteModulesProduct(id);
        }
        #endregion

        #region  查
        public static List<ModulesProduct> GetModulesProducts(string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            string sqlWhere = "p.ModulesID=m.ModulesID and p.Status<>9 ";
            //if (!string.IsNullOrEmpty(keyWords))
            //    sqlWhere += "p.PeriodQuantity";

            DataTable dt = CommonBusiness.GetPagerData("ModulesProduct as p,Modules as m ", " p.*,m.name ", sqlWhere, "p.AutoID", " p.UserQuantity asc,PeriodQuantity asc ", pageSize, pageIndex, out totalCount, out pageCount);
            List<ModulesProduct> list = new List<ModulesProduct>();
            ModulesProduct model;
            foreach (DataRow item in dt.Rows)
            { 
                model = new ModulesProduct();
                model.FillData(item);
                list.Add(model);
            }

            return list;
        }

        public static ModulesProduct GetModulesProductDetail(int id)
        {
            ModulesProduct model=new ModulesProduct();

            DataTable dt = ModulesProductDAL.BaseProvider.GetModulesProductDetail(id);
            model.FillData(dt.Rows[0]);

            return model;
        }


        public static Way GetBestWay(int quantity, List<ModulesProduct> list)
        {
            List<Way> ways = new List<Way>();

            foreach (var p in list)
            {
                Way model = new Way();
                model.TotalQuantity = p.UserQuantity;
                model.TotalMoney = p.Price;
                model.Products = new Dictionary<string, int>();
                model.Products.Add(p.ProductID, 1);

                if (model.TotalQuantity >= quantity)
                {
                    ways.Add(model);
                }
                else
                {
                    ways.AddRange(GetWays(quantity, model, list.Where(m => m.UserQuantity < quantity).ToList()));
                    break;
                }
            }

            return ways.OrderBy(m => m.TotalMoney).First();
        }

        public static List<Way> GetWays(int quantity, Way way, List<ModulesProduct> list)
        {
            List<Way> ways = new List<Way>();

            foreach (var p in list)
            {
                Way model = new Way() { TotalMoney = way.TotalMoney, TotalQuantity = way.TotalQuantity, Products = new Dictionary<string, int>(way.Products) };

                model.TotalQuantity += p.UserQuantity;
                model.TotalMoney += p.Price;
                if (model.Products.ContainsKey(p.ProductID))
                {
                    model.Products[p.ProductID] += 1;
                }
                else
                {
                    model.Products.Add(p.ProductID, 1);
                }

                if (model.TotalQuantity >= quantity)
                {
                    ways.Add(model);
                }
                else
                {
                    ways.AddRange(GetWays(quantity, model, list));
                    break;
                }
            }

            return ways;
        }
        #endregion

        #region  改
        public static bool UpdateModulesProduct(ModulesProduct model)
        {
            return ModulesProductDAL.BaseProvider.UpdateModulesProduct(model.AutoID,model.ModulesID, model.Period, model.PeriodQuantity, model.UserQuantity,
                model.Price, model.Description,model.Type,model.IsChild);
        }
        #endregion


    }

    public class Way
    {
        public int TotalQuantity { get; set; }
        public decimal TotalMoney { get; set; }
        public Dictionary<string, int> Products { get; set; }
    }


}
