define(function (require, exports, module) {
    var Global = require("global");
    var Tip = require("tip");
    var DoT = require("dot");

    var OrderListCache = null;
    
    var IsLoadding = true;
    var IsLoaddingTwo = true;

    var Paras = {
        filterTime: '', 
        filterType: 1,  //订单阶段-1全部 1已超期 2快到期 3进行中 4已完成
        userID: '',
        moduleType:1,//模块类型 1.订单  2.任务
        orderType: -1,
        pageSize:10,
        pageIndex:1
    }

    var ObjectJS = {};
    ObjectJS.orderFilter = -1;

    ObjectJS.init = function (level, userID) {
        //level   3和0移除订单按钮
        if (level == 2 || level == 0) {
            Paras.userID = userID;
        }

        if (level == 0 || level == 3) {
            Paras.moduleType = 2;
            $(".list-header").find("span").eq(0).data('isget', '0');
            $('.order-type').find('span:first-child').remove();
            $('.order-type').find('span:last-child').addClass('hover').css({ "border-left": "1px solid #cecece", "border-right": "1px solid #cecece" });
        }

        ObjectJS.bindEvent();
        ObjectJS.getReportList();
        ObjectJS.getDataList();
        ObjectJS.getTaskOrOrderCount();
    };

    ObjectJS.bindEvent = function () {
        //订单进行状态筛选
        $(".sum-list li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("active")) {
                _this.addClass("active").siblings().removeClass("active");

                ObjectJS.orderFilter = _this.data("orderfilter");
                ObjectJS.bindReport();
            }
        });

        //订单模块筛选
        $(".order-type span").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover"))
            {
                if (IsLoadding && IsLoaddingTwo)
                {
                    _this.addClass('hover').siblings().removeClass('hover');
                    $(".list-header").find("span").eq(0).data('isget', '1');

                    Paras.moduleType = _this.data('id');
                    Paras.filterTime ='';
                    Paras.filterType = 1;
                    Paras.pageIndex = 1;

                    ObjectJS.getReportList();
                    ObjectJS.getTaskOrOrderCount();
                    ObjectJS.getDataList();
                }
                else {
                    alert("数据加载中，请稍等 !");
                }
            }

        });

        //订单类型选择
        require.async("dropdown", function () {
            var orderTypes = [{ ID: "1", Name: "打样" }, { ID: "2", Name: "大货" }];
            $("#orderType").dropdown({
                prevText: "订单类型-",
                defaultText: "全部",
                defaultValue: "-1",
                data: orderTypes,
                dataValue: "ID",
                dataText: "Name",
                width: "110",
                onChange: function (data) {
                    if (IsLoadding && IsLoaddingTwo) {
                        Paras.orderType = data.value;
                        Paras.pageIndex = 1;
                        ObjectJS.getDataList();
                        ObjectJS.getReportList();
                    }
                    else {
                        alert("数据加载中，请稍等 !");
                    }
                }
            });
        });

    }

    //获取报表数据
    ObjectJS.getReportList = function () {
        $(".report-guid").nextAll().remove();
        var loadding = "<div class='data-loading'>";
        $(".report-guid").append(loadding);

        IsLoadding = false;
        var action = Paras.moduleType == 1 ? "GetOrdersByPlanTime" : "GetTasksByEndTime";
        Global.post("/Home/" + action, { userID: Paras.userID,orderType:Paras.orderType }, function (data) {
            $(".report-guid").find('.data-loading').remove();
            IsLoadding = true;

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
        //        //exceedCount: 3,
        //        //warnCount: 1,
        //        //workCount: 1,
        //        finishCount: 9,
        //        totalCount: 9
        //    };
        //    items.push(item);
        //}
        //OrderListCache = items;

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
        ReportAvgHeight = 220 / (maxTotalCount + (GuidLineHeight - guidLineHeight) * 5);

        for (var l = 0; l < OrderListCache.length; l++) {
            ObjectJS.createReportHtml(OrderListCache[l], l);
        }

        $(".report-guid ul li:not(:last)").css("height", (GuidLineHeight * ReportAvgHeight - 1.25) + "px");
        for (var h = 0; h<6; h++) {
            $(".report-guid ul li").eq(h).find(".guid-count").html(GuidLineHeight * (5 - h));
        }
        
        $(".index-report-content .report-item li").each(function () {
            var _this = $(this);
            var type = _this.data("type");
            var showMsg = Paras.moduleType == 1 ? "订单" : "任务";
            _this.Tip({
                width: 160,
                msg: _this.data("date") + "      " + (type == 1 ? "已超期" + showMsg : type == 2 ? "快到期" + showMsg : type == 3 ? "进行中" + showMsg : "已完成" + showMsg) + "：" + _this.data("count")
            });
        });

        $(".report-item li").click(function () {
            if (IsLoadding && IsLoaddingTwo) {
                var _this = $(this);
                Paras.filterType = _this.data('type');
                Paras.filterTime = _this.data('date');
                $(".order-layerbox .layer-lump").nextAll().remove();
                $(".list-header .list-total").css("background-color", _this.data('type') == 1 ? "#f35353" : _this.data('type') == 2 ? "#ffa200" : _this.data('type') == 3 ? "#49b3f5" : "#2F73B8");

                ObjectJS.getDataList();
            }
            else {
                alert("数据加载中，请稍等 !");
            }
        });
    }

    //拼接报表柱状图形
    ObjectJS.createReportHtml = function (item, index) {
        if (item.totalCount > 0) {
            var lineHeight = 0;
            var isShow = false;
            var html = '';
            html += '<div class="report-item" style="left:' + (75 * index) + 'px">';
            html += '    <ul>';
            if (item.exceedCount > 0 && (ObjectJS.orderFilter == -1 || ObjectJS.orderFilter == 1)) {
                lineHeight = item.exceedCount * ReportAvgHeight;
                lineHeight = lineHeight < 10 ? ReportMinHeight : lineHeight;
                isShow = true;

                html += '<li style="line-height:' + lineHeight + 'px;" class="item-exceed" data-count="' + item.exceedCount + '" data-date="'+item.date+'" data-type="1">' + item.exceedCount + '</li>';
            }
            if (item.warnCount > 0 && (ObjectJS.orderFilter == -1 || ObjectJS.orderFilter == 2)) {
                lineHeight = item.warnCount * ReportAvgHeight;
                lineHeight = lineHeight < 10 ? ReportMinHeight : lineHeight;
                isShow = true;

                html += '<li style="line-height:' + lineHeight + 'px;" class="item-warn" data-count="' + item.warnCount + '" data-date="' + item.date + '" data-type="2">' + item.warnCount + '</li>';
            }
            if (item.workCount > 0 && (ObjectJS.orderFilter == -1 || ObjectJS.orderFilter == 3)) {
                lineHeight = item.workCount * ReportAvgHeight;
                lineHeight = lineHeight < 10 ? ReportMinHeight : lineHeight;
                isShow = true;

                html += '<li style="line-height:' + lineHeight + 'px;" class="item-work" data-count="' + item.workCount + '" data-date="' + item.date + '" data-type="3">' + item.workCount + '</li>';
            }
            if (item.finishCount > 0 && (ObjectJS.orderFilter == -1 || ObjectJS.orderFilter == 4)) {
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

    //获取数据
    ObjectJS.getDataList = function (addStatus) {
        var loadding = "<div class='data-loading'>";

        //如果addStatus传入参数则不移除列表数据而是累加数据
        if (addStatus === undefined) {
            $(".order-layerbox").find('.layer-lump').nextAll().remove();
        }
        $(".order-layerbox").append(loadding);

        IsLoaddingTwo = false;
        var moduleType = Paras.moduleType;
        var url = "/template/home/index-order.html";
        if (moduleType == 2) {
            url = "/template/home/index-task.html";
        }
        Global.post("/Home/GetOrdersByTypeAndTime", Paras, function (data) {
            IsLoaddingTwo = true;
            var items = data.items;
            //如果最后已到了最后一页则移除加载更多按钮
            if (data.items.length < 10 || data.pageCount==Paras.pageIndex) {
                $(".load-more").remove();
            }
            else {
                if ($(".load-more").length == 0) {
                    $(".load-box").append('<span class="load-more font14 hand">加载更多</span>');

                    //加载完毕绑定加载更多事件
                    $('.load-more').unbind().click(function () {
                        if (IsLoadding && IsLoaddingTwo) {
                            Paras.pageIndex++;
                            ObjectJS.getDataList('1');
                        }
                        else {
                            alert("数据加载中，请稍等 !");
                        }
                    });
                }
            }

            if (items.length == 0) {
                var nodata = "<div class='nodata-txt'>暂无数据!<div>";
                $(".order-layerbox").append(nodata);
            }
            else {
                DoT.exec(url, function (template) {
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

                    });

                    innerText.find('.layer-line').css({ width: 0, left: 160 });
                });
            }

            //切换模块显示任务或订单描述
            var orderMsg = "任务";
            var totalEcceed = "超期任务总数:";
            if ($(".order-type span.hover").data('id') == 1) {
                orderMsg = "订单";
                totalEcceed = "超期订单总数:";
            }
            $(".order-msg").html(orderMsg);
            $(".ordertotal .total-ecceed").siblings().html(totalEcceed);

            $(".order-layerbox").find('.data-loading').remove();

            //判断是否选择时间没有列表时间则显示已超期
            var timeHtml = $(".list-header").find("span").eq(0);
            if (timeHtml.data('isget') != 1) {
                timeHtml.html(data.showTime);
            }
            else {
                timeHtml.html('已超期');
                $(".list-total").css({ "background-color": "#f35353" });
                timeHtml.data('isget', 0);
            }
            $(".list-total").html(data.getTotalCount);
            $(".total-ecceed").html(data.getTotalCount);

        })
    }

    //获取统计总数
    ObjectJS.getTaskOrOrderCount = function () {
        Global.post("/Home/GetOrderOrTaskCount", Paras,
            function (data) {
            if (Paras.moduleType == 1) {
                $(".total-need").siblings().html('需求订单总数:');
            }
            else {
                $(".total-need").siblings().html('未接受任务总数:');
            }

            $(".total-need").html(data.result);
        });
    }

    module.exports= ObjectJS;
});