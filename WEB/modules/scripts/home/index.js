﻿define(function (require, exports, module) {
    var Global = require("global");
    var Tip = require("tip");
    var DoT = require("dot");

    var OrderListCache = null;
    var CacheArr = new Array();
    var IsLoadding = true;
    var IsLoaddingTwo = true;

    var Paras = {
        filterTime: '', 
        filterType: -1,  //订单阶段-1全部 1已超期 2快到期 3进行中 4已完成
        moduleType:1,//模块类型 1.订单  2.任务
        orderType: -1,
        pageSize:5,
        pageIndex:1
    }

    var ObjectJS = {};
    
    ObjectJS.orderFilter = -1;

    ObjectJS.init = function (orderLevel) {
        ObjectJS.orderLevel = orderLevel;

        //没有订单权限
        if (orderLevel == 0) {
            Paras.moduleType = 2;
            $('.order-type').find('span:first-child').remove();
            $('.order-type').find('span:last-child').addClass('hover').css({ "border-left": "1px solid #cecece", "border-right": "1px solid #cecece" });
        }

        ObjectJS.bindEvent();       
        ObjectJS.getReportList();
        ObjectJS.getDataList();
        ObjectJS.getTaskOrOrderEcceedCount();
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
                    Paras.moduleType = _this.data('id');

                    ObjectJS.getReportList();
                    ObjectJS.getNeedOrderList();
                    ObjectJS.getTaskOrOrderEcceedCount();
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
                    if (Paras.orderType != data.value) {
                        if (IsLoadding && IsLoaddingTwo) {
                            Paras.orderType = data.value;
                            Paras.pageIndex = 1;

                            ObjectJS.getDataList();
                            ObjectJS.getReportList();
                            ObjectJS.getTaskOrOrderEcceedCount();
                        }
                        else {
                            alert("数据加载中，请稍等 !");
                        }
                    }
                }
            });
        });

        //获取所有已超期订单或任务
        $(".get-need").click(function () {
            if (IsLoadding && IsLoaddingTwo) {
                if (Paras.filterTime != "") {
                    ObjectJS.getNeedOrderList();
                }
            }
            else {
                alert("数据加载中，请稍等 !");
            }
           
        });

        //加载完毕绑定加载更多事件
        $('.load-more').click(function () {
            if (IsLoadding && IsLoaddingTwo) {
                Paras.pageIndex++;
                ObjectJS.getDataList();
            }
            else {
                alert("数据加载中，请稍等 !");
            }
        });

    }

    //获取报表数据
    ObjectJS.getReportList = function () {
        $(".report-guid").nextAll().remove();
        var loadding = "<div class='data-loading'>";
        $(".report-guid").append(loadding);

        var data = CacheArr[Paras.filterTime + Paras.filterType + Paras.moduleType + Paras.orderType + "ReportList"];

        if (data == null) {
            IsLoadding = false;
            var action = Paras.moduleType == 1 ? "GetOrdersByPlanTime" : "GetTasksByEndTime";
            Global.post("/Home/" + action, {orderType: Paras.orderType }, function (data) {
                $(".report-guid").find('.data-loading').remove();
                IsLoadding = true;

                CacheArr[Paras.filterTime + Paras.filterType + Paras.moduleType + Paras.orderType + "ReportList"] = data;
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
        else {
            $(".report-guid").find('.data-loading').remove();
            OrderListCache = data.items;
            ObjectJS.bindReport();
            $("#totalSumCount").html(data.totalSumCount);
            $("#totalExceedCount").html(data.totalExceedCount);
            $("#totalFinishCount").html(data.totalFinishCount);
            $("#totalWarnCount").html(data.totalWarnCount);
            $("#totalWorkCount").html(data.totalWorkCount);
            $("#totalSumCount").html(data.totalSumCount);
        }
    }

    var ReportAvgHeight = 0;//报表每一份对应的行高
    var GuidLineHeight = 0;//报表网格线对应的份数
    var ReportMinHeight = 10;//最低行高
    var ReportIndex=0;
    ObjectJS.bindReport = function () {
        //var items = [];
        //for (var i = 0; i < 6; i++) {
        //    var item = {
        //        date: "6.1",
        //        exceedCount: 3,
        //        warnCount: 1,
        //        workCount: 0,
        //        finishCount: 100,
        //        totalCount: 104
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

        if (maxTotalCount > 0) {
            var guidLineHeight = maxTotalCount / 5;
            GuidLineHeight = parseInt(maxTotalCount % 5 == 0 ? guidLineHeight : (guidLineHeight + 1));
            ReportAvgHeight = 220 / (maxTotalCount + (GuidLineHeight - guidLineHeight) * 5);

            ReportIndex = 0;
            for (var l = 0; l < OrderListCache.length; l++) {
                ObjectJS.createReportHtml(OrderListCache[l]);
            }

            $(".report-guid ul li:not(:last)").css("height", (GuidLineHeight * ReportAvgHeight - 1.2) + "px");
            for (var h = 0; h < 6; h++) {
                $(".report-guid ul li").eq(h).find(".guid-count").html(GuidLineHeight * (5 - h));
            }

            $(".index-report-content .report-item li").each(function () {
                var _this = $(this);
                var type = _this.data("type");
                var totalCount = _this.data("totalcount");
                var count = _this.data("count");
                var showMsg = Paras.moduleType == 1 ? "订单" : "任务";
                var message = _this.data("date") + "&nbsp;&nbsp;&nbsp;" + (type == 1 ? "已超期" + showMsg : type == 2 ? "快到期" + showMsg : type == 3 ? "正常" + showMsg : "已完成" + showMsg) + "：" + count;
                message += "</br>总计：" + totalCount + "，占比：" + (count / totalCount * 100).toFixed(2) + "%";
                _this.Tip({
                    width: 160,
                    msg: message
                });
            });

            $(".report-item li").click(function () {
                var _this = $(this);
                if ((Paras.filterType != _this.data('type') || Paras.filterTime != _this.data('date')) || !_this.hasClass('checked')) {
                    if (IsLoadding && IsLoaddingTwo) {
                        Paras.filterType = _this.data('type');
                        Paras.filterTime = _this.data('date');

                        var backgroundColor = _this.data('type') == 1 ? "#f35353" : _this.data('type') == 2 ? "#ffa200" : _this.data('type') == 3 ? "#49b3f5" : "#2F73B8";
                        $(".report-item li").removeClass('checked').css({ "box-shadow": "none" });
                        _this.addClass('checked').css({ "box-shadow": "2px 2px 10px " + backgroundColor });
                        $(".list-header .list-total").css("background-color", backgroundColor);
                        $(".order-layerbox .layer-lump").nextAll().remove();

                        ObjectJS.getOrdersByTypeAndTime();
                    }
                    else {
                        alert("数据加载中，请稍等 !");
                    }
                }
            });
        }

    }

    //拼接报表柱状图形
    ObjectJS.createReportHtml = function (item) {
        if (item.totalCount > 0) {
            var lineHeight = 0;
            var isShow = false;
            var countArr = [item.exceedCount, item.warnCount, item.workCount, item.finishCount];
            var classNameArr = ["item-exceed", "item-warn", "item-work", "item-finish"];
            var html = '';
            html += '<div class="report-item" style="left:' + (75 * ReportIndex) + 'px">';
            html += '    <ul>';
            
            for (var i = 0; i < 4; i++) {
                var count = countArr[i];
                var classname = classNameArr[i];
                var type=i+1;
                if (count > 0 && (ObjectJS.orderFilter == -1 || ObjectJS.orderFilter == type)) {
                    lineHeight = count * ReportAvgHeight;
                    lineHeight = lineHeight < 10 ? ReportMinHeight : lineHeight;
                    isShow = true;

                    html += '<li style="line-height:' + lineHeight + 'px;" class="' + classname + '" data-totalcount="' + item.totalCount + '" data-count="' + count + '" data-date="' + item.date + '" data-type="' + type + '">' + count + '</li>';
                }
            }

            html += '    </ul>';
            html += '    <div class="item-date">' + item.date + '</div>';
            html += '</div>';
            html = $(html);

            if (isShow) {
                $(".index-report-content").append(html);
                html.fadeIn(500);

                ReportIndex++;
            }
        }
    }

    ObjectJS.getNeedOrderList = function () {
        Paras.filterTime = '';
        Paras.filterType = -1;
        Paras.pageIndex = 1;
        ObjectJS.getDataList();
    }

    ObjectJS.getOrdersByTypeAndTime = function () {
        Paras.pageIndex = 1;
        ObjectJS.getDataList();
    }

    //获取列表数据
    ObjectJS.getDataList = function () {
        var data = null;
        if (Paras.pageIndex == 1) {
            $(".order-layerbox").find('.layer-lump').nextAll().remove();
            data = CacheArr[Paras.filterTime + Paras.filterType + Paras.moduleType + Paras.orderType + "DataList"];
        }

        $(".order-layerbox").append("<div class='data-loading'></div>");

        IsLoaddingTwo = false;
        if (data == null) {
            Global.post("/Home/GetOrdersByTypeAndTime", Paras, function (data) {
                IsLoaddingTwo = true;
                $('.data-loading').remove();

                if (Paras.pageIndex == 1) {
                    CacheArr[Paras.filterTime + Paras.filterType + Paras.moduleType + Paras.orderType + "DataList"] = data;
                }
                ObjectJS.createDataListHtml(data);
            })
        }
        else {
            ObjectJS.createDataListHtml(data);
        }
    }

    //拼接列表数据
    ObjectJS.createDataListHtml = function (data) {
        IsLoaddingTwo = true;
        $('.data-loading').remove();

        var url = "";
        if (Paras.moduleType == 2) {
            url = "/template/home/index-task.html";

            if (Paras.filterTime == '') {
                url = "/template/home/task-list.html";
            }
        }
        else {
            url = "/template/home/index-order.html";

            if (Paras.filterTime == '') {
                url = "/template/home/customerorders.html";
            }
        }

        var items = data.items;
        if (items.length == 0) {
            $(".order-layerbox").append("<div class='nodata-txt'>暂无数据!<div>");
        }
        else {
            DoT.exec(url, function (template) {
                var innerText = template(items);
                innerText = $(innerText);
                $(".order-layerbox").append(innerText);

                innerText.find('.order-progress-item').each(function () {
                    var _this = $(this);
                    _this.css({ "width": _this.data('width') });
                });
                $(".order-layerbox").find('.progress-tip,.top-lump').each(function () {
                    var _this = $(this);
                    _this.css({ "left": (_this.parent().width() - _this.width()) / 2 });

                });
                innerText.find('.layer-line').css({ width: 0, left: "160px" });
            });
        }

        //如果最后已到了最后一页则移除加载更多按钮
        if (data.pageCount == Paras.pageIndex) {
            $(".load-box").hide();
        }
        else {
            if (data.pageCount > 1) {
                $(".load-box").show();
            }
        }

        //文字说明切换
        var reportTitle = "我的订单";
        var reportTotalTtitle = "全部订单";
        var totalEcceedTtitle = "需求订单总数:";
        if (Paras.moduleType == 2) 
        {
            reportTitle = "我的任务";
            reportTotalTtitle = "全部任务";
            totalEcceedTtitle = "未接收任务总数:";
        }
        $(".report-title").html(reportTitle);
        $(".report-total-title").html(reportTotalTtitle);
        $(".ordertotal .total-need").prev().html(totalEcceedTtitle);
        

        var $listTitle = $(".list-title");
        if (Paras.filterTime != '') {
            $listTitle.html(data.showTime);
        }
        else {
            var listTitle = "需求单";
            if (Paras.moduleType == 2) {
                listTitle = "未接受任务";
            }
            $listTitle.html(listTitle);

            $(".list-total").css({ "background-color": "#49b3f5" });
            $(".list-total").html(data.getNeedTotalCount);
            $(".total-need").html(data.getNeedTotalCount);
        }
    }

    //获取超期总数
    ObjectJS.getTaskOrOrderEcceedCount = function () {
        var data = CacheArr[Paras.filterTime + Paras.filterType + Paras.moduleType + Paras.orderType + "TaskOrOrderCount"];
        if (data == null) {
            Global.post("/Home/GetTaskOrOrderEcceedCount", Paras,
                function (data) {
                    CacheArr[Paras.filterTime + Paras.filterType + Paras.moduleType + Paras.orderType + "TaskOrOrderCount"] = data;

                    var name = "超期订单总数:";
                    if (Paras.moduleType == 2) {
                        name = "超期任务总数:";
                    }
                    $(".total-ecceed").html(data.result).prev().html(name);
                });
        }
        else {
            var name = "超期订单总数:";
            if (Paras.moduleType == 2) {
                name = "超期任务总数:";
            }
            $(".total-ecceed").html(data.result).prev().html(name);
        }
    }

    module.exports= ObjectJS;
});