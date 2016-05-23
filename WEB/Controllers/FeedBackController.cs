using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using IntFactoryEntity.Manage;
using IntFactoryBusiness.Manage;
namespace YXERP.Controllers
{
    public class FeedBackController :BaseController
    {
        //
        // GET: /FeedBack/

        #region ajax
        public ActionResult InsertFeedBack(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FeedBack model = serializer.Deserialize<FeedBack>(entity);
            model.CreateUserID = CurrentUser.UserID;
            
            bool flag= FeedBackBusiness.InsertFeedBack(model);
            JsonDictionary.Add("Result",flag?1:0);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion

    }
}
