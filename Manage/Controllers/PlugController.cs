using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IntFactoryBusiness;

using Qiniu.Auth;
using Qiniu.IO;
using Qiniu.IO.Resumable;
using Qiniu.RS;
using Qiniu.RPC;
using Qiniu.Conf;
namespace YXManage.Controllers
{
    public class PlugController : BaseController
    {
        /// <summary>
        /// 根据cityCode获取下级地区列表
        /// </summary>
        /// <param name="pcode"></param>
        /// <returns></returns>
        public JsonResult GetCityByPCode(string cityCode)
        {
            var list = CommonBusiness.Citys.Where(c => c.PCode == cityCode);
            JsonDictionary.Add("Items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetToken()
        {
            Config.Init();
            //普通上传,只需要设置上传的空间名就可以了,第二个参数可以设定token过期时间
            PutPolicy put = new PutPolicy(YXManage.Common.Common.QNBucket, 3600);
            //调用Token()方法生成上传的Token
            string upToken = put.Token();
            JsonDictionary.Add("uptoken", upToken);
            JsonDictionary.Add("qnBucket", YXManage.Common.Common.QNBucket);
            JsonDictionary.Add("qnDomianUrl", YXManage.Common.Common.QNDomianUrl);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        public int DeleteAttachment(string key)
        {
            Config.Init();
            //实例化一个RSClient对象，用于操作BucketManager里面的方法
            RSClient client = new RSClient();
            CallRet ret = client.Delete(new EntryPath(YXManage.Common.Common.QNBucket, key));

            return ret.OK ? 1 : 0;
        }

        public bool UploadAttachment(string key)
        {
            Config.Init();
            IOClient target = new IOClient();
            PutExtra extra = new PutExtra();
            //普通上传,只需要设置上传的空间名就可以了,第二个参数可以设定token过期时间
            PutPolicy put = new PutPolicy(YXManage.Common.Common.QNBucket, 3600);

            //调用Token()方法生成上传的Token
            string upToken = put.Token();
            //上传文件的路径
            String filePath = string.Empty;

            //调用PutFile()方法上传
            PutRet ret = target.PutFile(upToken, key, filePath, extra);


            return ret.OK;
        }

    }
}
