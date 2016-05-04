using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IntFactoryBusiness;
using IntFactoryEntity;
using IntFactoryEntity.Manage;
using IntFactoryBusiness.Manage;
using YXERP.Common.Alipay;
using System.Collections.Specialized;
using System.Data;
namespace YXERP.Controllers
{
    public class AuctionController : Controller
    {
        Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();

        IntFactoryEntity.Users CurrentUser
        {
            get
            {
                if (Session["ClientManager"] == null)
                {
                    return null;
                }
                else
                {
                    return (IntFactoryEntity.Users)Session["ClientManager"];
                }
            }
        }

        Agents CurrentAgent
        {
            get
            {
                if (CurrentUser == null)
                {
                    return null;
                }
                else
                {
                    return AgentsBusiness.GetAgentDetail(CurrentUser.AgentID);
                }


            }
        }

        Clients CurrentClient
        {
            get
            {
                if (CurrentUser == null)
                {
                    return null;
                }
                else
                {
                    return ClientBusiness.GetClientDetail(CurrentUser.ClientID);
                }


            }
        }
        #region view
        /// <summary>
        /// 购买系统
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult BuyNow(string id)
        {
            if (Session["ClientManager"] == null)
            {
                return Redirect("/Home/Login");
            }
            else
            {
                if (CurrentAgent.AuthorizeType == 1)
                {
                    return Redirect("/Auction/BuyUserQuantity");
                }

                ViewBag.Discount = GetBuyDiscount();

                id = id ?? string.Empty;
                int result= GetClientOrderInfo(ref id);

                if (result == 2)
                {
                    return Redirect("/Auction/BuyNow/" + id);
                }
                else if (result == -2)
                {
                    Response.Write("<script>alert('订单已支付');location.href='/Home/Index';</script>");
                    Response.End();
                }
                else if (result == -1)
                {
                    Response.Write("<script>alert('订单不存在');location.href='/Home/Index';</script>");
                    Response.End();
                }
            }

            return View();
        }

        /// <summary>
        /// 购买人数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult BuyUserQuantity(string id)
        {
            if (Session["ClientManager"] == null)
            {
                return Redirect("/Home/Login");
            }
            else
            {
                if (CurrentAgent.AuthorizeType == 0)
                {
                    return Redirect("/Auction/BuyNow");
                }

                if ((CurrentAgent.EndTime - DateTime.Now).Days <= 31)
                {
                    return Redirect("/Auction/ExtendNow");
                }

                int remainderMonths = (CurrentAgent.EndTime.Year - DateTime.Now.Year) * 12 + (CurrentAgent.EndTime.Month - DateTime.Now.Month) - 1;
                if (CurrentAgent.EndTime.Day >= DateTime.Now.Day)
                    remainderMonths += 1;
                ViewBag.RemainderMonths = remainderMonths;

                ViewBag.CurrentAgent = CurrentAgent;

                ViewBag.Discount = GetBuyDiscount();

                id = id ?? string.Empty;
                int result = GetClientOrderInfo(ref id);

                if (result == 2)
                {
                    return Redirect("/Auction/BuyUserQuantity/" + id);
                }
                else if (result == -2)
                {
                    Response.Write("<script>alert('订单已支付');location.href='/Home/Index';</script>");
                    Response.End();
                }
                else if (result == -1)
                {
                    Response.Write("<script>alert('订单不存在');location.href='/Home/Index';</script>");
                    Response.End();
                }

            }

            return View();
        }

        /// <summary>
        /// 续费
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ExtendNow(string id)
        {
            if (Session["ClientManager"] == null)
            {
                return Redirect("/Home/Login");
            }
            else
            {
                if (CurrentAgent.AuthorizeType == 0)
                {
                    return Redirect("/Auction/BuyNow");
                }

                if ((CurrentAgent.EndTime - DateTime.Now).Days > 31)
                {
                    return Redirect("/Auction/BuyUserQuantity");
                }

                int Days = (CurrentAgent.EndTime - DateTime.Now).Days;
                ViewBag.Days = Days;
                ViewBag.UserQuantity = CurrentAgent.UserQuantity;
                ViewBag.CurrentAgent = CurrentAgent;

                ViewBag.Discount = 10;
                if (!string.IsNullOrEmpty(CurrentClient.AliMemberID))
                    ViewBag.Discount = 8.8;

                id = id ?? string.Empty;
                int result = GetClientOrderInfo(ref id);

                if (result == 2)
                {
                    return Redirect("/Auction/ExtendNow/" + id);
                }
                else if (result == -2)
                {
                    Response.Write("<script>alert('订单已支付');location.href='/Home/Index';</script>");
                    Response.End();
                }
                else if (result == -1)
                {
                    Response.Write("<script>alert('订单不存在');location.href='/Home/Index';</script>");
                    Response.End();
                }
            }

            return View();
        }

        /// <summary>
        /// 跳转支付宝进行订单支付
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GoAlipayPay(string id)
        {
            string url = string.Empty;
            if (Session["ClientManager"] == null)
            {
                return Redirect("/Home/Login");
            }
            else
            {
                ClientOrder order = ClientOrderBusiness.GetClientOrderInfo(id);
                if (order != null && !string.IsNullOrEmpty(order.OrderID))
                {
                    //订单未支付
                    if (order.Status == 0)
                    {
                        url = ToPayOrderUrl(order.UserQuantity, order.Years, order.OrderID, order.RealAmount.ToString(), 1);
                        return Redirect(url);
                    }
                }

            }

            return View();
        }

        /// <summary>
        /// 获取客户订单信息
        /// </summary>
        /// <param name="id"></param>
        public int GetClientOrderInfo(ref string id)
        {
            ViewBag.OrderID = id ?? string.Empty;

            if (!string.IsNullOrEmpty(id))
            {
                ClientOrder order = ClientOrderBusiness.GetClientOrderInfo(id);
                if (order != null && !string.IsNullOrEmpty(order.OrderID))
                {
                    //订单已支付
                    if (order.Status == 1)
                    {
                        return -2;
                    }
                    else
                    {
                        ViewBag.RealAmount = decimal.Round(order.RealAmount, 2);
                    }
                }
                //订单不存在
                else
                {
                    return -1;
                }
            }
            else
            {
                int pageCount = 0;
                int totalCount = 0;
                List<ClientOrder> list = ClientOrderBusiness.GetClientOrders(0, -1, string.Empty, string.Empty, CurrentUser.AgentID, CurrentUser.ClientID, int.MaxValue, 1, ref totalCount, ref pageCount);

                if (list.Count > 0) {
                    id = list[0].OrderID;
                    return 2;
                }
            }

            return 1;

        }

        /// <summary>
        /// 支付宝扣款成功跳转页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Success()
        {
            SortedDictionary<string, string> sPara = GetRequestGet();

            if (sPara.Count > 0)//判断是否有带返回参数
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.Verify(sPara, Request.QueryString["notify_id"], Request.QueryString["sign"]);

                if (verifyResult)//验证成功
                {
                    //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
                    //获取支付宝的通知返回参数，可参考技术文档中页面跳转同步通知参数列表

                    //商户订单号

                    string out_trade_no = Request.QueryString["out_trade_no"];

                    //支付宝交易号

                    string trade_no = Request.QueryString["trade_no"];

                    //交易状态
                    string trade_status = Request.QueryString["trade_status"];


                    if (Request.QueryString["trade_status"] == "TRADE_FINISHED" || Request.QueryString["trade_status"] == "TRADE_SUCCESS")
                    {
                        //判断该笔订单是否在商户网站中已经做过处理
                        //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                        //如果有做过处理，不执行商户的业务程序
                    }
                }
            }
            return View();
        }

        /// <summary>
        /// 接受支付宝扣款通知
        /// </summary>
        public void Notify()
        {
            SortedDictionary<string, string> sPara = GetRequestPost();

            if (sPara.Count > 0)//判断是否有带返回参数
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.Verify(sPara, Request.Form["notify_id"], Request.Form["sign"]);


                if (verifyResult)//验证成功
                {
                    //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
                    //获取支付宝的通知返回参数，可参考技术文档中服务器异步通知参数列表

                    //商户订单号
                    string out_trade_no = Request.Form["out_trade_no"];

                    //支付宝交易号

                    string trade_no = Request.Form["trade_no"];

                    //交易状态
                    string trade_status = Request.Form["trade_status"];


                    if (Request.Form["trade_status"] == "TRADE_FINISHED")
                    {
                        //判断该笔订单是否在商户网站中已经做过处理
                        //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                        //请务必判断请求时的total_fee、seller_id与通知时获取的total_fee、seller_id为一致的
                        //如果有做过处理，不执行商户的业务程序

                        //注意：
                        //退款日期超过可退款期限后（如三个月可退款），支付宝系统发送该交易状态通知
                    }
                    else if (Request.Form["trade_status"] == "TRADE_SUCCESS")
                    {
                        //判断该笔订单是否在商户网站中已经做过处理
                        //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                        //请务必判断请求时的total_fee、seller_id与通知时获取的total_fee、seller_id为一致的
                        //如果有做过处理，不执行商户的业务程序

                        //注意：
                        //付款完成后，支付宝系统发送该交易状态通知

                        //获取订单详情
                        ClientOrder order = ClientOrderBusiness.GetClientOrderInfo(out_trade_no);
                        if (order != null && !string.IsNullOrEmpty(order.OrderID))
                        {
                            decimal total_fee = decimal.Parse(Request.Form["total_fee"]);
                            if (order.RealAmount == total_fee)
                            {
                                //订单支付及后台客户授权
                                bool flag = ClientOrderBusiness.PayOrderAndAuthorizeClient(order.OrderID);

                                if (flag)
                                {
                                    AgentsBusiness.UpdatetAgentCache(order.AgentID);
                                    Response.Write("success");  //请不要修改或删除
                                }
                            }
                        }
                    }

                }
            }

        }

        #endregion

        #region ajax
        /// <summary>
        /// 获取产品列表
        /// </summary>
        public JsonResult GetProductList()
        {
            int pageCount = 0;
            int totalCount = 0;

            List<ModulesProduct> list = ModulesProductBusiness.GetModulesProducts(string.Empty, int.MaxValue, 1, ref totalCount, ref pageCount);

            JsonDictionary.Add("Items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 根据人数、年数获取最佳产品组合
        /// </summary>
        public JsonResult GetBestWay(int quantity, int years, int type)
        {
            int remainderMonths = 12;//剩余月份
            float discount = 1F;
            int pageCount = 0;
            int totalCount = 0;

            //购买人数
            if (type == 2)
            {
                remainderMonths = (CurrentAgent.EndTime.Year - DateTime.Now.Year) * 12 + (CurrentAgent.EndTime.Month - DateTime.Now.Month) - 1;
                if (CurrentAgent.EndTime.Day >= DateTime.Now.Day)
                    remainderMonths += 1;

                years = remainderMonths / 12 == 0 ? 1 : remainderMonths / 12;
                JsonDictionary.Add("PeriodQuantity", years);
            }

            List<ModulesProduct> list = ModulesProductBusiness.GetModulesProducts(string.Empty, int.MaxValue, 1, ref totalCount, ref pageCount);
            var way = ModulesProductBusiness.GetBestWay(quantity, list.OrderByDescending(m => m.UserQuantity).Where(m => m.PeriodQuantity == years).ToList());

            List<Dictionary<string, string>> products = new List<Dictionary<string, string>>();
            foreach (var p in way.Products)
            {
                Dictionary<string, string> product = new Dictionary<string, string>();
                product.Add("id", p.Key);
                product.Add("count", p.Value.ToString());
                products.Add(product);
            }

            JsonDictionary.Add("Items", products);
            JsonDictionary.Add("TotalMoney", way.TotalMoney);
            JsonDictionary.Add("TotalQuantity", way.TotalQuantity);


            //购买人数
            if (type == 2)
            {
                float remainderYears = (float)remainderMonths / (12 * years);
                JsonDictionary.Add("Amount", (float.Parse(way.TotalMoney.ToString()) * remainderYears).ToString("f2"));
            }

            if (!string.IsNullOrEmpty(CurrentUser.MDUserID))
            {
                if (type == 1 || type == 2)
                {
                    discount = 0.5F;
                }
                else
                {
                    discount = 0.88F;
                }

            }
            else
            {
                if (type == 1 || type == 2)
                {
                    discount = 0.5F;
                }
            }
            JsonDictionary.Add("Discount", discount);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 根据人数、年数生成客户订单
        /// </summary>
        public JsonResult AddClientOrder(int quantity, int years, int type)
        {
            int remainderMonths = 12;//剩余月份

            //购买人数
            if (type == 2)
            {
                remainderMonths = (CurrentAgent.EndTime.Year - DateTime.Now.Year) * 12 + (CurrentAgent.EndTime.Month - DateTime.Now.Month) - 1;
                if (CurrentAgent.EndTime.Day >= DateTime.Now.Day)
                    remainderMonths += 1;

                years = remainderMonths / 12 == 0 ? 1 : remainderMonths / 12;
            }

            int pageCount = 0;
            int totalCount = 0;
            List<ModulesProduct> list = ModulesProductBusiness.GetModulesProducts(string.Empty, int.MaxValue, 1, ref totalCount, ref pageCount);

            //获取订单产品的最佳组合
            var way = ModulesProductBusiness.GetBestWay(quantity, list.OrderByDescending(m => m.UserQuantity).Where(m => m.PeriodQuantity == years).ToList());

            //获取订单参数
            ClientOrder model = new ClientOrder();
            model.UserQuantity = way.TotalQuantity;
            model.Type = type;
            model.Years = years;
            model.Amount = way.TotalMoney;

            decimal discount = 1M;
            if (!string.IsNullOrEmpty(CurrentUser.MDUserID))
            {
                if (type == 1 || type == 2)
                {
                    discount = 0.50M;
                }
                else
                {
                    discount = 0.88M;
                }

            }
            else
            {
                if (type == 1 || type == 2)
                {
                    discount = 0.5M;
                }
            }

            model.RealAmount = way.TotalMoney * discount;

            //购买人数
            float remainderYears = 1;
            if (type == 2)
            {
                remainderYears = (float)remainderMonths / (12 * years);
                model.Amount = decimal.Parse((float.Parse(model.Amount.ToString()) * remainderYears).ToString("f2"));
                model.RealAmount = decimal.Parse((float.Parse(model.RealAmount.ToString()) * remainderYears).ToString("f2"));
            }
            model.AgentID = CurrentUser.AgentID;
            model.ClientID = CurrentUser.ClientID;
            model.CreateUserID = CurrentUser.UserID;

            model.Details = new List<ClientOrderDetail>();
            foreach (var p in way.Products)
            {
                ClientOrderDetail detail = new ClientOrderDetail();
                detail.ProductID = p.Key;
                detail.Qunatity = p.Value;
                detail.CreateUserID = CurrentUser.CreateUserID;
                detail.Price = list.Find(m => m.ProductID == p.Key).Price;
                //购买人数
                if (type == 2)
                {
                    detail.Price = decimal.Parse((float.Parse(detail.Price.ToString()) * remainderYears).ToString("f2"));
                }
                model.Details.Add(detail);
            }

            string orderID = ClientOrderBusiness.AddClientOrder(model);
            JsonDictionary.Add("ID", orderID);
            JsonDictionary.Add("RealAmount", model.RealAmount);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取支付宝支付订单的url
        /// </summary>
        /// <returns></returns>
        public JsonResult ToPayOrder(int userQuantity, int years, string orderID, string realAmount, int type)
        {
            if (!string.IsNullOrEmpty(orderID))
            {

                string productUrl = "http://edj.yunxiaokeji.com/Home/Login";
                string name = "年";
                if (type == 2)
                    name = "月";
                string orderTitle = "您购买了二当家 人数:" + userQuantity + "人   时间:" + years + name + "   " + "金额:" + realAmount;
                string orderDes = "";
                decimal realAmount2 = decimal.Parse(realAmount);
                string amount = decimal.Round(realAmount2, 2).ToString();

                JsonDictionary.Add("AlipayUrl", GetAlipayUrl(productUrl, orderTitle, orderDes, amount, orderID));
            }

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }

        /// <summary>
        /// 获取支付宝支付订单的url
        /// </summary>
        public string ToPayOrderUrl(int userQuantity, int years, string orderID, string realAmount, int type)
        {
            if (!string.IsNullOrEmpty(orderID))
            {

                string productUrl = "http://edj.yunxiaokeji.com/Home/Login";
                string name = "年";
                if (type == 2)
                    name = "月";
                string orderTitle = "您购买了二当家 人数:" + userQuantity + "人   时间:" + years + name + "   " + "金额:" + realAmount;
                string orderDes = "";
                decimal realAmount2 = decimal.Parse(realAmount);
                string amount = decimal.Round(realAmount2, 2).ToString();

                return GetAlipayUrl(productUrl, orderTitle, orderDes, amount, orderID);
            }

            return string.Empty;

        }

        /// <summary>
        /// 获取支付宝付款页面url
        /// </summary>
        public string GetAlipayUrl(string productUrl, string orderTitle, string orderDes, string amount, string orderID)
        {

            ////////////////////////////////////////////请求参数////////////////////////////////////////////

            //支付类型
            string payment_type = "1";
            //必填，不能修改
            //服务器异步通知页面路径
            string notify_url = Common.Common.AlipayNotifyPage;
            //需http://格式的完整路径，不能加?id=123这类自定义参数

            //页面跳转同步通知页面路径
            //string return_url = Common.Common.AlipaySuccessPage;
            string return_url = string.Empty;
            //需http://格式的完整路径，不能加?id=123这类自定义参数，不能写成http://localhost/

            //商户订单号
            string out_trade_no = orderID;
            //商户网站订单系统中唯一订单号，必填

            //订单名称
            string subject = orderTitle;
            //必填

            //付款金额
            string total_fee = amount;
            //string total_fee = "0.01";
            //必填

            //订单描述
            string body = orderDes;

            //商品展示地址
            string show_url = productUrl;
            //需以http://开头的完整路

            //防钓鱼时间戳
            string anti_phishing_key = "";
            //若要使用请调用类文件submit中的query_timestamp函数

            //客户端的IP地址
            string exter_invoke_ip = "";
            //非局域网的外网IP地址，如：221.0.0.1


            ////////////////////////////////////////////////////////////////////////////////////////////////

            //把请求参数打包成数组
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
            sParaTemp.Add("partner", Config.Partner);
            sParaTemp.Add("seller_email", Config.Seller_email);
            sParaTemp.Add("_input_charset", Config.Input_charset.ToLower());
            sParaTemp.Add("service", "create_direct_pay_by_user");
            sParaTemp.Add("payment_type", payment_type);
            sParaTemp.Add("notify_url", notify_url);
            sParaTemp.Add("return_url", return_url);
            sParaTemp.Add("out_trade_no", out_trade_no);
            sParaTemp.Add("subject", subject);
            sParaTemp.Add("total_fee", total_fee);
            sParaTemp.Add("body", body);
            sParaTemp.Add("show_url", show_url);
            sParaTemp.Add("anti_phishing_key", anti_phishing_key);
            sParaTemp.Add("exter_invoke_ip", exter_invoke_ip);

            //建立请求
            return Submit.BuildRequest(sParaTemp, "get", "确认");
        }

        public JsonResult GetOrderInfo(string id)
        {
            int result = 0;
            if (!string.IsNullOrEmpty(id))
            {
                ClientOrder order = ClientOrderBusiness.GetClientOrderInfo(id);
                if (order != null && !string.IsNullOrEmpty(order.OrderID))
                {
                    //订单已支付
                    if (order.Status == 0)
                    {
                        result = -2;
                    }
                    else
                    {
                        result = 1;
                        JsonDictionary.Add("type", order.Type);
                        JsonDictionary.Add("nowUserCount", CurrentAgent.UserQuantity);
                        JsonDictionary.Add("nowEndTime", CurrentAgent.EndTime.ToString("yyyy-MM-dd"));

                        if (order.Type == 1 || order.Type == 3)
                        {
                            JsonDictionary.Add("preUserCount", 0);
                            JsonDictionary.Add("preEndTime", CurrentAgent.EndTime.AddYears(-order.Years).ToString("yyyy-MM-dd"));
                        }
                        else if (order.Type == 2) {
                            JsonDictionary.Add("preUserCount", CurrentAgent.UserQuantity-order.UserQuantity);
                            JsonDictionary.Add("preEndTime", CurrentAgent.EndTime.ToString("yyyy-MM-dd"));
                        }
                    

                    }
                }
                //订单不存在
                else
                {
                    result = -1;
                }
            }
            else
            {
                result = 2;
            }

            JsonDictionary.Add("result", result);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }

        #endregion

        #region common
        /// <summary>
        /// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public SortedDictionary<string, string> GetRequestPost()
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.Form;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.Form[requestItem[i]]);
            }

            return sArray;
        }

        /// <summary>
        /// 获取支付宝GET过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public SortedDictionary<string, string> GetRequestGet()
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.QueryString;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.QueryString[requestItem[i]]);
            }

            return sArray;
        }

        public float GetBuyDiscount() {
            float discount = 5;
            if (!string.IsNullOrEmpty(CurrentClient.AliMemberID))
            {
                discount = 5;
            }

            return discount;
        }
        #endregion

    }
}
