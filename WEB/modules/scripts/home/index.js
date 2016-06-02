define(function (require, exports, module) {
    var Global = require("global");
    var Tip = require("tip");
    var DoT = require("dot");

    var OrderListCache = null;
    var IsLoadding = false;
    var Paras = {
        orderFilter: -1,
        orderTime: new Date().getMonth().toString()+'.' + new Date().getDay().toString(),
        filterType: 1
    }

    var ObjectJS = {};
    ObjectJS.init = function () {
        ObjectJS.bindEvent();

        ObjectJS.getOrdersByPlanTime();
        ObjectJS.getOrdersByStatus();
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
        IsLoadding = true;
        if (IsLoadding) {
            Global.post("/Home/GetOrdersByPlanTime", {}, function (data) {
                IsLoadding = false;
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
        ReportAvgHeight = 280 / (maxTotalCount + (GuidLineHeight - guidLineHeight) * 4);

        for (var l = 0; l < OrderListCache.length; l++) {
            ObjectJS.createReportHtml(OrderListCache[l], l);
        }

        $(".report-guid ul li:not(:last)").css("height", (GuidLineHeight * ReportAvgHeight - 1.25) + "px");
        for (var h = 0; h<5; h++) {
            $(".report-guid ul li").eq(h).find(".guid-count").html(GuidLineHeight * (4 - h));
        }
        
        $(".index-report-content .report-item li").each(function () {
            var _this = $(this);
            var type = _this.data("type");
            _this.Tip({
                width: 160,
                msg: _this.data("date") + "      " + (type == 1 ? "已超期订单" : type == 2 ? "快到期订单" : type == 3 ? "进行中订单" : "已完成订单") + "：" + _this.data("count")
            });
        });

        $(".report-item li").click(function () {
            var _this = $(this);
            Paras.filterType = _this.data('type');
            Paras.orderTime = _this.data('date');
            $(".order-layerbox .layer-lump").nextAll().remove();

            $(".list-header .list-total").css("background-color", _this.data('type') == 1 ? "#f35353" : _this.data('type') == 2 ? "#ffa200" : _this.data('type') == 3 ? "#49b3f5" : "#2F73B8");
            ObjectJS.getOrdersByStatus();
        });
    }

    ObjectJS.createReportHtml = function (item, index) {
        if (item.totalCount > 0) {
            var lineHeight = 0;
            var isShow = false;
            var html = '';
            html += '<div class="report-item" style="left:' + (75 * index) + 'px">';
            html += '    <ul>';
            if (item.exceedCount > 0 && (Paras.orderFilter == -1 || Paras.orderFilter==1) ) {
                lineHeight = item.exceedCount * ReportAvgHeight;
                lineHeight = lineHeight < 10 ? ReportMinHeight : lineHeight;
                isShow = true;

                html += '<li style="line-height:' + lineHeight + 'px;" class="item-exceed" data-count="' + item.exceedCount + '" data-date="'+item.date+'" data-type="1">' + item.exceedCount + '</li>';
            }
            if (item.warnCount > 0 && (Paras.orderFilter == -1 || Paras.orderFilter == 2) ) {
                lineHeight = item.warnCount * ReportAvgHeight;
                lineHeight = lineHeight < 10 ? ReportMinHeight : lineHeight;
                isShow = true;

                html += '<li style="line-height:' + lineHeight + 'px;" class="item-warn" data-count="' + item.warnCount + '" data-date="' + item.date + '" data-type="2">' + item.warnCount + '</li>';
            }
            if (item.workCount > 0 && (Paras.orderFilter == -1 || Paras.orderFilter == 3) ) {
                lineHeight = item.workCount * ReportAvgHeight;
                lineHeight = lineHeight < 10 ? ReportMinHeight : lineHeight;
                isShow = true;

                html += '<li style="line-height:' + lineHeight + 'px;" class="item-work" data-count="' + item.workCount + '" data-date="' + item.date + '" data-type="3">' + item.workCount + '</li>';
            }
            if (item.finishCount > 0 && (Paras.orderFilter == -1 || Paras.orderFilter == 4) ) {
                lineHeight = item.finishCount * ReportAvgHeight;
                lineHeight = lineHeight < 10 ? ReportMinHeight : lineHeight;
                isShow = true;

                html += ' <li style="line-height:' + lineHeight + 'px;" class="item-finish" data-count="' + item.finishCount + '" data-date="' + item.date + '" data-type="4">' + item.finishCount + '</li>';
            }
            html += '    </ul>';
            html += '    <div class="item-date">' + item.date + '</div>';
            html += '</div>';
            html = $(html);

            if (isShow) {
                $(".index-report-content").append(html);
                html.fadeIn(500);
            }
        }

        
    }

    ObjectJS.getOrdersByStatus = function () {
        IsLoadding = true;
        if (IsLoadding) {
            var loadding = "<div class='center loadding'><img src='/modules/images/ico-loading.gif' style='width:30px;height:30px;' /></div>";
            $(".order-layerbox").append(loadding);
            Global.post("/Home/GetOrdersByTypeAndTime", Paras, function (data) {
                IsLoadding = false;
                $(".order-layerbox").find('.loadding').remove();
                var items = data.items;
                $(".list-total").html(items.length);

                var timeHtml = $(".list-header").find("span").eq(0);
                if (timeHtml.data('isget') != 1) {
                    timeHtml.html(data.showTime);
                } else {
                    timeHtml.data('isget', 0);
                }

                if (items.length == 0) {
                    var nodata = "<div class='center font14'>暂无数据</div>";
                    $(".order-layerbox").append(nodata);
                } else {
                    DoT.exec("/template/orders/index-order.html", function (template) {
                        var innerText = template(items);
                        innerText = $(innerText);

                        innerText.find('.order-progress-item').each(function () {
                            var _this = $(this);

                            _this.css({ "width": _this.data('width') });

                        });

                        $(".order-layerbox").append(innerText);

                        $(".order-layerbox").find('.progress-tip,.top-lump').each(function () {
                            var _this = $(this);

                            _this.css({ "left": (_this.parent().width() - _this.width()) / 2 });

                        })

                        //$(".order-layerbox").find('.top-lump').each(function () {
                        //    var _this = $(this);

                        //    _this.css({ "left": (_this.parent().width() - _this.width()) / 2 });
                        //})
                        innerText.find('.layer-line').css({ width: 0, left: 160 });



                    });
                }

            })
        }
    }

    module.exports= ObjectJS;
});