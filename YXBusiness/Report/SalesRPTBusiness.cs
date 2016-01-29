using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;

using IntFactoryEntity;
using IntFactoryDAL;

namespace IntFactoryBusiness
{
    public class SalesRPTBusiness
    {
        public static SalesRPTBusiness BaseBusiness = new SalesRPTBusiness();

        public List<TypeOrderEntity> GetUserOrders(string userid, string teamid, string begintime, string endtime, string agentid, string clientid)
        {
            List<TypeOrderEntity> list = new List<TypeOrderEntity>();

            DataSet ds = SalesRPTDAL.BaseProvider.GetUserOrders(userid, teamid, begintime, endtime, agentid, clientid);

            DataTable dt = ds.Tables["Users"];

            var types = SystemBusiness.BaseBusiness.GetOrderTypes(agentid, clientid);



            if (!string.IsNullOrEmpty(userid))
            {
                #region 统计个人

                TypeOrderEntity model = new TypeOrderEntity();
                model.Name = OrganizationBusiness.GetUserByUserID(userid, agentid).Name;
                model.Types = new List<TypeOrderItem>();
                foreach (var type in types)
                {
                    TypeOrderItem item = new TypeOrderItem();
                    item.Name = type.TypeName;

                    var drs = dt.Select("TypeID='" + item.TypeID + "'");
                    if (drs.Count() > 0)
                    {
                        item.Money = Convert.ToDecimal(drs[0]["TotalMoney"]);
                        item.Count = Convert.ToInt32(drs[0]["Value"]);
                    }
                    else
                    {
                        item.Money = 0;
                        item.Count = 0;
                    }
                    model.Types.Add(item);
                }
                list.Add(model);

                #endregion
            }
            else if (!string.IsNullOrEmpty(teamid))
            {
                #region 统计团队
                var team = SystemBusiness.BaseBusiness.GetTeamByID(teamid, agentid);
                TypeOrderEntity model = new TypeOrderEntity();

                model.Name = team.TeamName;
                model.GUID = team.TeamID;

                model.Types = new List<TypeOrderItem>();
                model.ChildItems = new List<TypeOrderEntity>();

                //遍历成员
                foreach (var user in team.Users)
                {
                    TypeOrderEntity childModel = new TypeOrderEntity();
                    childModel.GUID = user.UserID;
                    childModel.Name = user.Name;
                    childModel.PID = team.TeamID;
                    childModel.Types = new List<TypeOrderItem>();

                    //遍历阶段
                    foreach (var type in types)
                    {
                        TypeOrderItem childItem = new TypeOrderItem();
                        childItem.Name = type.TypeName;

                        var drs = dt.Select("TypeID='" + type.TypeID + "' and OwnerID='" + user.UserID + "'");

                        if (drs.Count() > 0)
                        {
                            childItem.Money = Convert.ToDecimal(drs[0]["TotalMoney"]);
                            childItem.Count = Convert.ToInt32(drs[0]["Value"]);
                        }
                        else
                        {
                            childItem.Money = 0;
                            childItem.Count = 0;
                        }

                        if (model.Types.Where(m => m.TypeID == type.TypeID).Count() > 0)
                        {
                            model.Types.Where(m => m.TypeID == type.TypeID).FirstOrDefault().Count += childItem.Count;
                            model.Types.Where(m => m.TypeID == type.TypeID).FirstOrDefault().Money += childItem.Money;
                        }
                        else
                        {
                            TypeOrderItem item = new TypeOrderItem();
                            item.Name = type.TypeName; ;
                            item.TypeID = type.TypeID;
                            item.Count = childItem.Count;
                            item.Money = childItem.Money;
                            model.Types.Add(item);
                        }

                        childModel.Types.Add(childItem);
                    }
                    model.ChildItems.Add(childModel);
                }
                list.Add(model);

                #endregion
            }
            else
            {
                #region 统计所有
                var teams = SystemBusiness.BaseBusiness.GetTeams(agentid);
                foreach (var team in teams)
                {
                    TypeOrderEntity model = new TypeOrderEntity();

                    model.Name = team.TeamName;
                    model.GUID = team.TeamID;

                    model.Types = new List<TypeOrderItem>();
                    model.ChildItems = new List<TypeOrderEntity>();

                    //遍历成员
                    foreach (var user in team.Users)
                    {
                        TypeOrderEntity childModel = new TypeOrderEntity();
                        childModel.GUID = user.UserID;
                        childModel.Name = user.Name;
                        childModel.PID = team.TeamID;
                        childModel.Types = new List<TypeOrderItem>();

                        //遍历阶段
                        foreach (var type in types)
                        {
                            TypeOrderItem childItem = new TypeOrderItem();
                            childItem.Name = type.TypeName;

                            var drs = dt.Select("TypeID='" + type.TypeID + "' and OwnerID='" + user.UserID + "'");

                            if (drs.Count() > 0)
                            {
                                childItem.Money = Convert.ToDecimal(drs[0]["TotalMoney"]);
                                childItem.Count = Convert.ToInt32(drs[0]["Value"]);
                            }
                            else
                            {
                                childItem.Money = 0;
                                childItem.Count = 0;
                            }

                            if (model.Types.Where(m => m.TypeID == type.TypeID).Count() > 0)
                            {
                                model.Types.Where(m => m.TypeID == type.TypeID).FirstOrDefault().Count += childItem.Count;
                                model.Types.Where(m => m.TypeID == type.TypeID).FirstOrDefault().Money += childItem.Money;
                            }
                            else
                            {
                                TypeOrderItem item = new TypeOrderItem();
                                item.Name = type.TypeName; ;
                                item.TypeID = type.TypeID;
                                item.Count = childItem.Count;
                                item.Money = childItem.Money;
                                model.Types.Add(item);
                            }

                            childModel.Types.Add(childItem);
                        }
                        model.ChildItems.Add(childModel);
                    }
                    list.Add(model);
                }

                #endregion
            }


            return list;
        }

        public List<OrderMapItem> GetOrderMapReport(int type, string begintime, string endtime, string UserID, string TeamID, string agentid, string clientid)
        {
            List<OrderMapItem> list = new List<OrderMapItem>();

            DataTable dt = SalesRPTDAL.BaseProvider.GetOrderMapReport(type, begintime, endtime, UserID, TeamID, agentid, clientid);
            foreach (DataRow dr in dt.Rows)
            {
                OrderMapItem model = new OrderMapItem();
                model.name = dr["name"].ToString();
                model.value = int.Parse(dr["value"].ToString());
                model.total_money = decimal.Parse(dr["total_money"].ToString());

                list.Add(model);
            }

            if (type == 1)
            {
                if (CommonBusiness.Citys != null)
                {
                    List<CityEntity> provinces = CommonBusiness.Citys.FindAll(m => m.Level == 1);
                    foreach (CityEntity c in provinces)
                    {
                        OrderMapItem item = list.Find(m => m.name == c.Name);
                        if (item == null)
                        {
                            OrderMapItem model = new OrderMapItem();
                            model.name = c.Name.Replace("市", "").Replace("省", "").Replace("特别行政区", "").Replace("壮族自治区", "").Replace("回族自治区", "").Replace("维吾尔自治区", "").Replace("自治区", "");
                            model.value = 0;
                            model.total_money = 0;
                            list.Add(model);
                        }
                        else
                        {
                            item.name = c.Name.Replace("市", "").Replace("省", "").Replace("特别行政区", "").Replace("壮族自治区", "").Replace("回族自治区", "").Replace("维吾尔自治区", "").Replace("自治区", "");
                        }
                    }
                }
            }

            return list;
        }

        public List<ReportCommonEntity> GetOpportunityStageRate(string begintime, string endtime, string UserID, string TeamID, string agentid, string clientid, out decimal forecast)
        {
            forecast = 0;

            List<ReportCommonEntity> list = new List<ReportCommonEntity>();
            DataSet ds = SalesRPTDAL.BaseProvider.GetOpportunityStageRate(begintime, endtime, UserID, TeamID, agentid, clientid);

            var stages = SystemBusiness.BaseBusiness.GetOrderStages("", agentid, clientid);
            decimal total = 0, prev = 0;
            foreach (var stage in stages)
            {
                ReportCommonEntity model = new ReportCommonEntity();

                model.name = stage.StageName;
                
                DataRow[] drs = ds.Tables["Data"].Select("StageID='" + stage.StageID + "'");
                if (drs.Count() > 0)
                {
                    model.dValue = Convert.ToDecimal(drs[0]["Value"]);
                }
                else
                {
                    model.dValue = 0;
                }
                model.desc = "当前：" + model.dValue.ToString("f2") + "*" + (stage.Probability * 100).ToString("f2") + "%";

                total += model.dValue;

                forecast += model.dValue * stage.Probability;

                list.Add(model);
            }


            if (total > 0)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    list[i].dValue += prev;
                    prev = list[i].dValue;

                    list[i].value = (list[i].dValue / total * 100).ToString("f2");
                    list[i].name += list[i].dValue.ToString("f2");
                    list[i].name += " (" + list[i].desc + ") ";
                }
            }
            else 
            {
                return new List<ReportCommonEntity>();
            }
            return list;
        }

        public List<StageCustomerEntity> GetUserOpportunitys(string userid, string teamid, string begintime, string endtime, string agentid, string clientid)
        {
            List<StageCustomerEntity> list = new List<StageCustomerEntity>();

            DataSet ds = SalesRPTDAL.BaseProvider.GetUserOpportunitys(userid, teamid, begintime, endtime, agentid, clientid);

            DataTable dt = ds.Tables["Users"];

            var stages = SystemBusiness.BaseBusiness.GetOrderStages("", agentid, clientid);

            #region 统计所有
            var teams = SystemBusiness.BaseBusiness.GetTeams(agentid);
            foreach (var team in teams)
            {
                StageCustomerEntity model = new StageCustomerEntity();

                model.Name = team.TeamName;
                model.GUID = team.TeamID;

                model.Stages = new List<StageCustomerItem>();
                model.ChildItems = new List<StageCustomerEntity>();

                //遍历成员
                foreach (var user in team.Users)
                {
                    StageCustomerEntity childModel = new StageCustomerEntity();
                    childModel.GUID = user.UserID;
                    childModel.Name = user.Name;
                    childModel.PID = team.TeamID;
                    childModel.Stages = new List<StageCustomerItem>();

                    //遍历阶段
                    foreach (var stage in stages)
                    {
                        StageCustomerItem childItem = new StageCustomerItem();
                        childItem.Name = stage.StageName;

                        var drs = dt.Select("StageID='" + stage.StageID + "' and OwnerID='" + user.UserID + "'");

                        if (drs.Count() > 0)
                        {
                            childItem.Count = Convert.ToInt32(drs[0]["Value"]);
                            childItem.Money = Convert.ToDecimal(drs[0]["TotalMoney"]);
                        }
                        else
                        {
                            childItem.Count = 0;
                            childItem.Money = 0;
                        }

                        if (model.Stages.Where(m => m.StageID == stage.StageID).Count() > 0)
                        {
                            model.Stages.Where(m => m.StageID == stage.StageID).FirstOrDefault().Count += childItem.Count;
                            model.Stages.Where(m => m.StageID == stage.StageID).FirstOrDefault().Money += childItem.Money;
                        }
                        else
                        {
                            StageCustomerItem item = new StageCustomerItem();
                            item.Name = stage.StageName;
                            item.StageID = stage.StageID;
                            item.Count = childItem.Count;
                            item.Money = childItem.Money;
                            model.Stages.Add(item);
                        }

                        childModel.Stages.Add(childItem);
                    }
                    model.ChildItems.Add(childModel);
                }
                list.Add(model);
            }

            #endregion


            return list;
        }
    }
}
