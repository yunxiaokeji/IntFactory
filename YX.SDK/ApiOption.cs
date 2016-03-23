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
        getToken,

        [Description("member.get")]
        memberDetail,

        [DescriptionAttribute("erp.manufacture.pullFentGoodsCodes")]
        pullFentGoodsCodes,

        [Description("erp.manufacture.pullFentDataList")]
        pullFentDataList,

        [Description("erp.manufacture.batchUpdateFent")]
        batchUpdateFent,

        [Description("erp.manufacture.pullBulkGoodsCodes")]
        pullBulkGoodsCodes,

        [Description("erp.manufacture.pullBulkDataList")]
        pullBulkDataList,

        [Description("erp.manufacture.batchUpdateBulk")]
        batchUpdateBulk,

        [Description("erp.manufacture.pushProductionPlan")]
        pushProductionPlan
    }
}
