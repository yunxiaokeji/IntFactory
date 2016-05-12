using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using IntFactoryEntity;
using YXERP.Models;
using IntFactoryBusiness;
using IntFactoryEnum;

namespace YXERP.Controllers
{
    public class InquireController : BaseController
    {
        //
        // GET: /Inquire/

        public ActionResult InquireOrder()
        {
            return View();
        }

        /// <summary>
        /// 发送手机验证码
        /// </summary>
        /// <param name="mobilePhone">手机号码</param>
        /// <returns></returns>
        public JsonResult SendMobileMessage(string mobilePhone)
        {
            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();
            Random rd = new Random();
            int code = rd.Next(100000, 1000000);

            bool flag = Common.MessageSend.SendMessage(mobilePhone, code);
            JsonDictionary.Add("Result", flag ? 1 : 0);

            if (flag)
            {
                Common.Common.SetCodeSession(mobilePhone, code.ToString());

                Common.Common.WriteAlipayLog(mobilePhone + " : " + code.ToString());

            }

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 验证手机验证码
        /// </summary>
        /// <param name="mobilePhone"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public JsonResult ValidateMobilePhoneCode(string mobilePhone, string code)
        {
            bool bl = Common.Common.ValidateMobilePhoneCode(mobilePhone, code);
            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();
            JsonDictionary.Add("Result", bl ? 1 : 0);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取客户订单状态
        /// </summary>
        /// <param name="mobilePhone"></param>
        /// <returns></returns>
        public JsonResult InquireOrderByPhone(string mobilePhone)
        {
            var list = OrdersBusiness.BaseBusiness.GetOrdersByMobilePhone(mobilePhone);
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}
