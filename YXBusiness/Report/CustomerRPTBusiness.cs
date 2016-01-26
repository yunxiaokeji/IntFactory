using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntFactoryEntity;
using System.Data;

using IntFactoryDAL;
using IntFactoryEnum;

namespace IntFactoryBusiness
{
    public class CustomerRPTBusiness
    {
        public static CustomerRPTBusiness BaseBusiness = new CustomerRPTBusiness();

        public List<SourceScaleEntity> GetCustomerSourceScale(string begintime, string endtime, string UserID, string TeamID, string agentid, string clientid)
        {
            List<SourceScaleEntity> list = new List<SourceScaleEntity>();
            int total = 0;
            DataTable dt = CustomerRPTDAL.BaseProvider.GetCustomerSourceScale(begintime, endtime, UserID, TeamID, agentid, clientid);
            foreach (DataRow dr in dt.Rows)
            {
                SourceScaleEntity model = new SourceScaleEntity();
                model.FillData(dr);
                total += model.Value;
                list.Add(model);
            }
            foreach (var model in list)
            {
                if (total > 0)
                {
                    model.Scale = (Convert.ToDecimal(model.Value) / total * 100).ToString("f2") + "%";
                }
                else
                {
                    model.Scale = "0.00%";
                }
            }
            return list;
        }

        public List<SourceDateEntity> GetCustomerSourceDate(EnumDateType type, string begintime, string endtime, string UserID, string TeamID, string agentid, string clientid)
        {
            List<SourceDateEntity> list = new List<SourceDateEntity>();

            DataSet ds = CustomerRPTDAL.BaseProvider.GetCustomerSourceDate((int)type, begintime, endtime, UserID, TeamID, agentid, clientid);
            var sources = SystemBusiness.BaseBusiness.GetCustomSources(agentid, clientid);
            foreach (var source in sources)
            {
                SourceDateEntity model = new SourceDateEntity();
                model.Name = source.SourceName;
                model.Items = new List<SourceDateItem>();
                DataRow[] drs = ds.Tables["SourceData"].Select("SourceID='" + source.SourceID + "'");
                foreach (DataRow dr in ds.Tables["DateName"].Rows)
                {
                    SourceDateItem item = new SourceDateItem();
                    item.Name = dr[0].ToString();
                    if (drs.Where(m => m["Name"].ToString() == item.Name).Count() > 0)
                    {
                        item.Value = Convert.ToInt32(drs.Where(m => m["Name"].ToString() == item.Name).FirstOrDefault()["Value"]);
                    }
                    else
                    {
                        item.Value = 0;
                    }
                    model.Items.Add(item);
                }
                list.Add(model);
            }

            return list;
        }

        public List<ReportCommonEntity> GetCustomerStageRate(string begintime, string endtime, string agentid, string clientid)
        {
            List<ReportCommonEntity> list = new List<ReportCommonEntity>();
            DataSet ds = CustomerRPTDAL.BaseProvider.GetCustomerStageRate(begintime, endtime, agentid, clientid);

            var stages = SystemBusiness.BaseBusiness.GetCustomStages(agentid, clientid);
            int total = 0, prev = 0;
            foreach (var stage in stages)
            {
                ReportCommonEntity model = new ReportCommonEntity();

                model.name = stage.StageName;
                model.iValue = 0;
                model.desc = "";

                foreach (DataRow dr in ds.Tables["Data"].Select("StageID='" + stage.StageID + "'"))
                {
                    model.desc += CommonBusiness.GetEnumDesc((EnumCustomStatus)Convert.ToInt32(dr["Status"])) + "：" + dr["Value"].ToString();

                    model.iValue += Convert.ToInt32(dr["Value"]);
                }
                total += model.iValue;

                list.Add(model);
            }


            if (total > 0)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    list[i].iValue += prev;
                    prev = list[i].iValue;

                    list[i].value = (Convert.ToDecimal(list[i].iValue) / total * 100).ToString("f2");
                    list[i].name += list[i].iValue;
                    if (list[i].desc.Length > 0)
                    {
                        list[i].name += " (" + list[i].desc + ") ";
                    }
                }

               
            }
            return list;
        }

        /// <summary>
        /// 获取客户分布统计
        /// </summary>
        /// <param name="type">1:按地区；2、按行业；3、按规模</param>
        public List<DateJson> GetCustomerReport(int type, string begintime, string endtime, string UserID, string TeamID, string agentid, string clientid)
        {
            List<DateJson> list = new List<DateJson>();

            DataTable dt = CustomerRPTDAL.BaseProvider.GetCustomerReport(type, begintime, endtime, UserID, TeamID, agentid, clientid);
            foreach (DataRow dr in dt.Rows)
            {
                DateJson model = new DateJson();
                model.name = dr["name"].ToString();
                model.value = int.Parse(dr["value"].ToString());

                list.Add(model);
            }

            if (type == 1)
            {
                if (CommonBusiness.Citys != null)
                {
                    List<CityEntity> provinces = CommonBusiness.Citys.FindAll(m => m.Level == 1);
                    foreach (CityEntity c in provinces)
                    {
                        DateJson item = list.Find(m => m.name == c.Name);
                        if (item == null)
                        {
                            DateJson model = new DateJson();
                            model.name = c.Name.Replace("市", "").Replace("省", "").Replace("特别行政区", "").Replace("壮族自治区", "").Replace("回族自治区", "").Replace("维吾尔自治区", "").Replace("自治区", "");
                            model.value = 0;
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

        public List<StageCustomerEntity> GetUserCustomers(string userid, string teamid, string begintime, string endtime, string agentid, string clientid)
        {
            List<StageCustomerEntity> list = new List<StageCustomerEntity>();

            DataSet ds = CustomerRPTDAL.BaseProvider.GetUserCustomers(userid, teamid, begintime, endtime, agentid, clientid);

            DataTable dt = ds.Tables["Users"];

            var stages = SystemBusiness.BaseBusiness.GetCustomStages(agentid, clientid);

            

            if (!string.IsNullOrEmpty(userid))
            {
                #region 统计个人

                StageCustomerEntity model = new StageCustomerEntity();
                model.Name = OrganizationBusiness.GetUserByUserID(userid, agentid).Name;
                model.Stages = new List<StageCustomerItem>();
                foreach (var stage in stages)
                {
                    StageCustomerItem item = new StageCustomerItem();
                    item.Name = stage.StageName;

                    var drs = dt.Select("StageID='" + item.StageID + "'");  
                    if (drs.Count() > 0)
                    {
                        item.Count = Convert.ToInt32(drs[0]["Value"]);
                    }
                    else
                    {
                        item.Count = 0;
                    }
                    model.Stages.Add(item);
                }
                list.Add(model);

                #endregion
            }
            else if (!string.IsNullOrEmpty(teamid))
            {
                #region 统计团队
                var team = SystemBusiness.BaseBusiness.GetTeamByID(teamid, agentid);
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
                        }
                        else
                        {
                            childItem.Count = 0;
                        }

                        if (model.Stages.Where(m => m.StageID == stage.StageID).Count() > 0)
                        {
                            model.Stages.Where(m => m.StageID == stage.StageID).FirstOrDefault().Count += childItem.Count;
                        }
                        else 
                        {
                            StageCustomerItem item = new StageCustomerItem();
                            item.Name = stage.StageName;
                            item.StageID = stage.StageID;
                            item.Count = childItem.Count;
                            model.Stages.Add(item);
                        }

                        childModel.Stages.Add(childItem);
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
                            }
                            else
                            {
                                childItem.Count = 0;
                            }

                            if (model.Stages.Where(m => m.StageID == stage.StageID).Count() > 0)
                            {
                                model.Stages.Where(m => m.StageID == stage.StageID).FirstOrDefault().Count += childItem.Count;
                            }
                            else
                            {
                                StageCustomerItem item = new StageCustomerItem();
                                item.Name = stage.StageName;
                                item.StageID = stage.StageID;
                                item.Count = childItem.Count;
                                model.Stages.Add(item);
                            }

                            childModel.Stages.Add(childItem);
                        }
                        model.ChildItems.Add(childModel);
                    }
                    list.Add(model);
                }

                #endregion
            }
            

            return list;
        }

    }
}
