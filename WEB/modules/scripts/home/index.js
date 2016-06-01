define(function (require, exports, module) {
    var Global = require("global");
    var Tip = require("tip");

    var OrderListCache = null;
    var Paras = {
        orderFilter:-1
    }
    var ObjectJS = {};

    ObjectJS.init = function () {
        ObjectJS.getOrdersByPlanTime();
        ObjectJS.bindEvent();
    };

    ObjectJS.bindEvent = function () {
        $(".sum-list li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("active")) {
                _this.addClass("active").siblings().removeClass("active");
                Paras.orderFilter = _this.data("orderfilter");
                ObjectJS.bindReport();
            }
        });

        $(".order-type span").click(function () {
            var _this = $(this);
            _this.addClass('hover').siblings().removeClass('hover');
        })

    }

    ObjectJS.getOrdersByPlanTime = function () {

        Global.post("/Home/GetOrdersByPlanTime", {}, function (data) {
            OrderListCache = data.items;
            ObjectJS.bindReport();

            $("#totalSumCount").html(data.totalSumCount);
            $("#totalExceedCount").html(data.totalExceedCount);
            $("#totalFinishCount").html(data.totalFinishCount);
            $("#totalWarnCount").html(data.totalWarnCount);
            $("#totalWorkCount").html(data.totalWorkCount);
            $("#totalSumCount").html(data.totalSumCount);
        });
    }

    var ReportAvgHeight = 0;//报表每一份对应的行高
    var GuidLineHeight = 0;//报表网格线对应的份数
    var ReportMinHeight = 0;//最低行高
    ObjectJS.bindReport = function () {
        //var items = [];
        //for (var i = 0; i < 1; i++) {
        //    var item = {
        //        date: "6.1",
        //        exceedCount: 3,
        //        warnCount: 1,
        //        workCount: 1,
        //        finishCount: 6,
        //        totalCount: 11
        //    };
        //    items.push(item);
        //}
        $(".report-guid").nextAll().remove();

        var maxTotalCount = 0;
        for (var j = 0; j < OrderListCache.length; j++) {
            var item = OrderListCache[j];
            if (item.totalCount > maxTotalCount) {
                maxTotalCount = item.totalCount;
            }
        }

        var guidLineHeight =maxTotalCount / 5;
        GuidLineHeight = parseInt(maxTotalCount % 5 == 0 ? guidLineHeight : (guidLineHeight + 1));
        ReportAvgHeight = 300 / (maxTotalCount + (GuidLineHeight - guidLineHeight) * 4);

        for (var l = 0; l < OrderListCache.length; l++) {
            ObjectJS.createReportHtml(OrderListCache[l], l);
        }

        $(".report-guid ul li:not(:last)").css("height", (GuidLineHeight * ReportAvgHeight - 1.25) + "px");
        for (var h = 0; h<5; h++) {
            $(".report-guid ul li").eq(h).find(".guid-count").html(GuidLineHeight * (4 - h));
        }

        $(".report-item li").each(function () {
            $(this).Tip({
                width: 300,
                msg: '/modules/plug/qqface/arclist/11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111'
            });
        });
    }

    ObjectJS.createReportHtml = function (item, index) {
        if (item.totalCount > 0) {
            var lineHeight = 0;
            var html = '';
            html += '<div class="report-item" style="left:' + (75 * index) + 'px">';
            html += '    <ul>';
            if (item.exceedCount > 0 && (Paras.orderFilter == -1 || Paras.orderFilter==1) ) {
                lineHeight = item.exceedCount * ReportAvgHeight;
                lineHeight = lineHeight < 10 ? ReportMinHeight : lineHeight;

                html += '<li style="line-height:' + lineHeight + 'px;" class="item-exceed">' + item.exceedCount + '</li>';
            }
            if (item.warnCount > 0 && (Paras.orderFilter == -1 || Paras.orderFilter == 2) ) {
                lineHeight = item.warnCount * ReportAvgHeight;
                lineHeight = lineHeight < 10 ? ReportMinHeight : lineHeight;

                html += '<li style="line-height:' + lineHeight + 'px;" class="item-warn">' + item.warnCount + '</li>';
            }
            if (item.workCount > 0 && (Paras.orderFilter == -1 || Paras.orderFilter == 3) ) {
                lineHeight = item.workCount * ReportAvgHeight;
                lineHeight = lineHeight < 10 ? ReportMinHeight : lineHeight;

                html += '<li style="line-height:' + lineHeight + 'px;" class="item-work">' + item.workCount + '</li>';
            }
            if (item.finishCount > 0 && (Paras.orderFilter == -1 || Paras.orderFilter == 4) ) {
                lineHeight = item.finishCount * ReportAvgHeight;
                lineHeight = lineHeight < 10 ? ReportMinHeight : lineHeight;

                html += ' <li style="line-height:' + lineHeight + 'px;" class="item-finish">' + item.finishCount + '</li>';
            }
            
            html += '    </ul>';
            html += '    <div class="item-date">' + item.date + '</div>';
            html += '</div>';
            html = $(html);
            $(".index-report-content").append(html);
            html.fadeIn(500);
        }
    }

    module.exports= ObjectJS;
});