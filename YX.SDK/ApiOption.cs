using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace AlibabaSdk
{
    public enum ApiOption
    {
        [Description("getToken")]
        accessToken=0,

        [Description("member.get")]
        memberDetail=1,

        [Description("erp.manufacture.pullFentGoodsCodes")]
        pullFentGoodsCodes = 2,

        [Description("erp.manufacture.pullFentDataList")]
        pullFentDataList = 3,

        [Description("erp.manufacture.batchUpdateFent")]
        batchUpdateFent = 4,

        [Description("erp.manufacture.pullBulkGoodsCodes")]
        pullBulkGoodsCodes = 5,

        [Description("erp.manufacture.pullBulkDataList")]
        pullBulkDataList = 6,

        [Description("erp.manufacture.batchUpdateBulk")]
        batchUpdateBulk = 7,

        [Description("erp.manufacture.pushProductionPlan")]
        pushProductionPlan = 8
    }
}
