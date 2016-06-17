define(function (require, exports, module) {
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
        moduleType: 1,//模块类型 1.订单  2.任务
        orderType: -1,
        pageSize: 5,
        pageIndex: 1,
        preFinishStatus: -1
    }

    var ObjectJS = {};
    
    ObjectJS.orderFilter = -1;
    var GuidLineHeight = 0;//报表网格线对应的份数
    var ReportMinHeight = 10;//最低行高
    var ReportIndex = 0;
    ObjectJS.init = function (orderLevel, roleLevel) {
        ObjectJS.orderLevel = orderLevel;
    var ReportAvgHeight = 0;//报表每一份对应的行高
        ObjectJS.roleLevel = roleLevel;

        //没有订单权限
        if (orderLevel == 0) {
            Paras.moduleType = 2;
            $('.order-type').find('span:first-child').remove();
            $('.order-type').find('span:last-child').addClass('hover').css({ "border-left": "1px solid #cecece", "border-right": "1px solid #cecece" });
        }

        ObjectJS.bindEvent();       
        ObjectJS.getReportList();
        ObjectJS.getNeedOrderList();
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

        $(document).click(function (e) {

            if (!$(e.target).parents().hasClass('dropdown-items-modules') && !$(e.target).parents().hasClass('dropdown-module')) {
                $(".dropdown-items-modules").hide();
            }

        })

        //订单模块筛选
        $(".order-type span").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover"))
            {
                if (IsLoadding && IsLoaddingTwo)
                {
                    //上级任务进行进度筛选
                    require.async("dropdown", function () {
                        var taskTypes = [{ ID: "-1", Name: "全部" }, { ID: "0", Name: "未接收" }, { ID: "1", Name: "进行中" }, { ID: "2", Name: "已完成" },{ ID: "9", Name: "无上级" }];
                        $("#taskType").dropdown({
                            prevText: "上级任务进度-",
                            defaultText: "全部",
                            defaultValue: "-1",
                            data: taskTypes,
                            dataValue: "ID",
                            dataText: "Name",
                            width: 150,
                            onChange: function (data) {
                                if (Paras.preFinishStatus != data.value) {
                                    if (IsLoadding && IsLoaddingTwo) {
                                        Paras.preFinishStatus = data.value;
                                        Paras.pageIndex = 1;

                                        ObjectJS.getDataList();
                                        ObjectJS.getTaskOrOrderEcceedCount();
                                    }
                                    else {
                                        alert("数据加载中，请稍等 !");
                                    }
                                }
                            }
                        });
                    });
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

                            ObjectJS.getReportList();
                            ObjectJS.getNeedOrderList();
                            ObjectJS.getTaskOrOrderEcceedCount();
                        }
                        else {
                            alert("数据加载中，请稍等 !");
                        }
                    }
                }
            });
        });

        //获取需球订单或未接受任务
        $(".get-need").click(function () {
            if (IsLoadding && IsLoaddingTwo) {
                if (Paras.filterTime != "" || Paras.filterType != -1) {
                    ObjectJS.getNeedOrderList();
                }
            }
            else {
                alert("数据加载中，请稍等 !");
            }
           
        });

        //获取所有已超期订单或任务
        $(".get-ecceed").click(function () {
            if (IsLoadding && IsLoaddingTwo) {
                if (Paras.filterTime != "" || Paras.filterType!=1) {
                    ObjectJS.getEcceedOrderList();
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

            $(".report-guid ul li:not(:last)").css("height", (GuidLineHeight * ReportAvgHeight - 1.2) + "px");
            for (var h = 0; h < 6; h++) {
                $(".report-guid ul li").eq(h).find(".guid-count").html(GuidLineHeight * (5 - h));
            }

            ReportIndex = 0;
            for (var l = 0; l < OrderListCache.length; l++) {
                ObjectJS.createReportHtml(OrderListCache[l]);
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

            //报表色块点击
            $(".report-item li").click(function () {
                var _this = $(this);
                if ((Paras.filterType != _this.data('type') || Paras.filterTime != _this.data('date')) || !_this.hasClass('checked')) {
                    if (IsLoadding && IsLoaddingTwo) {
                        var backgroundColor = _this.data('type') == 1 ? "#f35353" : _this.data('type') == 2 ? "#ffa200" : _this.data('type') == 3 ? "#49b3f5" : "#2F73B8";
                        $(".report-item li").removeClass('checked').css({ "box-shadow": "none" });
                        _this.addClass('checked').css({ "box-shadow": "2px 2px 10px " + backgroundColor });
                        $(".list-header .list-total").css("background-color", backgroundColor);
                        $(".order-layerbox .layer-lump").nextAll().remove();

                        Paras.filterType = _this.data('type');
                        Paras.filterTime = _this.data('date');
                        ObjectJS.getOrdersByTypeAndTime();
                    }
                    else {
                        alert("数据加载中，请稍等 !");
                    }
                }
            });
        }
        else {
            ReportIndex = 0;
            for (var l = 0; l < OrderListCache.length; l++) {
                ObjectJS.createReportHtml(OrderListCache[l]);
            }
        }

    }

    //拼接报表柱状图形
    ObjectJS.createReportHtml = function (item) {
        var lineHeight = 0;
        var countArr = [item.exceedCount, item.warnCount, item.workCount, item.finishCount];
        var classNameArr = ["item-exceed", "item-warn", "item-work", "item-finish"];
        var html = '';
        html += '<div class="report-item" style="left:' + (75 * ReportIndex) + 'px">';
        html += '    <ul>';
        
        if(item.totalCount>0){
            for (var i = 0; i < 4; i++) {
                var count = countArr[i];
                var classname = classNameArr[i];
                var type=i+1;
                if (count>0 && (ObjectJS.orderFilter == -1 || ObjectJS.orderFilter == type)) {
                    lineHeight = count * ReportAvgHeight;
                    lineHeight = lineHeight < 10 ? ReportMinHeight : lineHeight;

                    html += '<li style="line-height:' + lineHeight + 'px;" class="' + classname + '" data-totalcount="' + item.totalCount + '" data-count="' + count + '" data-date="' + item.date + '" data-type="' + type + '">' + count + '</li>';
                }
            }
        }

        html += '    </ul>';
        html += '    <div class="item-date">' + item.date + '</div>';
        html += '</div>';
        html = $(html);

        $(".index-report-content").append(html);
        html.fadeIn(500);
        ReportIndex++;
    }

    ObjectJS.getNeedOrderList = function () {
        Paras.filterTime = '';
        Paras.filterType = -1;
        Paras.pageIndex = 1;
        ObjectJS.getDataList();
    }

    ObjectJS.getEcceedOrderList = function () {
        Paras.filterTime = '';
        Paras.filterType = 1;
        Paras.pageIndex = 1;
        ObjectJS.getDataList();
    }

    ObjectJS.getOrdersByTypeAndTime = function () {
        Paras.pageIndex = 1;
        ObjectJS.getDataList();
    }

    //获取列表数据
    ObjectJS.getDataList = function () {
        IsLoaddingTwo = false;
        var data = null;
        if (Paras.moduleType == 2) {
            $("#taskType").show();
        }
        else {
            Paras.preFinishStatus = -1;
            $("#taskType").hide();
        }
        if (Paras.pageIndex == 1) {
            $(".order-layerbox .layer-lump").nextAll().remove();
            data = CacheArr[Paras.filterTime + Paras.filterType + Paras.moduleType + Paras.orderType + Paras.preFinishStatus + "DataList"];
        }
        $(".order-layerbox").append("<div class='data-loading'></div>");


        if (data == null) {
            Global.post("/Home/GetOrdersByTypeAndTime", Paras, function (data) {
                IsLoaddingTwo = true;
                $('.data-loading').remove();

                if (Paras.pageIndex == 1) {
                    CacheArr[Paras.filterTime + Paras.filterType + Paras.moduleType + Paras.orderType + Paras.preFinishStatus + "DataList"] = data;
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
        $('.data-loading').remove();
        IsLoaddingTwo = true;
        
        var url = "";
        if (Paras.moduleType == 2) {
            url = "/template/home/index-task.html";

            if (Paras.filterTime == '' && Paras.filterType==-1) {
                url = "/template/home/task-list.html";
            }
        }
        else {
            url = "/template/home/index-order.html";

            if (Paras.filterTime == '' && Paras.filterType == -1) {
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
        if (Paras.moduleType == 2) {
            reportTitle = "我的任务";
            if (ObjectJS.roleLevel == 1) {
                reportTitle = "所有任务";
            }
            reportTotalTtitle = "全部任务";
            totalEcceedTtitle = "未接收任务总数:";
        }
        else {
            if (ObjectJS.roleLevel == 1) {
                reportTitle = "所有订单";
            }
        }
        $(".report-title").html(reportTitle);
        $(".report-total-title").html(reportTotalTtitle);
        if (Paras.filterType == -1) {
            $(".ordertotal .total-need").prev().html(totalEcceedTtitle);
        }
        
        var $listTitle = $(".list-title");
        if (Paras.filterTime != '') {
            $listTitle.html(data.showTime);
        }
        else {
            var listTitle = "需求单";
            if (Paras.moduleType == 2) {
                listTitle = "未接受";
            }
            if (Paras.filterType == 1) {
                listTitle = "已超期";
                $(".list-total").css({ "background-color": "#f35353" });
            }
            else {
                $(".list-total").css({ "background-color": "#49b3f5" });
            }
            $listTitle.html(listTitle);

            if (Paras.filterType == -1) {
                $(".total-need").html(data.getNeedTotalCount);
            }
        }
        $(".list-total").html(data.getNeedTotalCount);
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