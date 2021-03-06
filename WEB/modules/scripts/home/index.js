﻿define(function (require, exports, module) {
    var Global = require("global");
    var Tip = require("tip");
    var DoT = require("dot");
    var EasyDialog = null;

    var OrderListCache = null;
    var CacheArr = new Array();
    var IsLoadding = true;
    var IsLoaddingTwo = true;
    var GuidLineHeight = 0;//报表网格线对应的份数
    var ReportMinHeight = 10;//最低行高
    var ReportIndex = 0;
    var ReportAvgHeight = 0;//报表每一份对应的行高

    var Paras = {
        filterTime: '',
        filterType: -1,  //订单阶段-1全部 1已超期 2快到期 3进行中 4已完成
        moduleType: 1,//模块类型 1.订单  2.任务
        orderType: -1,
        taskType: -1,
        preFinishStatus: -1,//上级任务筛选
        filterTimeType: 1,//根据时间
        pageSize: 5,
        pageIndex: 1,
        userID:"",
    }

    var ObjectJS = {};
    ObjectJS.orderFilter = -1;
    ObjectJS.init = function (orderLevel, taskLevel, remainDay, remainDate) {
        ObjectJS.remainDay = remainDay;
        ObjectJS.remainDate = remainDate;
        ObjectJS.orderLevel = orderLevel;
        ObjectJS.taskLevel = taskLevel;
        
        if (orderLevel == 0 && taskLevel==0) {
            $(".main-content").hide();
        }

        if (orderLevel == 1) {
            $(".report-title").html('所有订单');
            $("#chooseBranch").show();
        }else if (orderLevel == 0) {
            $(".report-title").hide();
            $(".report-title-task").addClass("hover");
            Paras.moduleType = 2;
        } 

        if (taskLevel == 1) {
            $(".report-title-task").html('所有任务');
            $("#chooseBranch").show();
        }else if (taskLevel == 0) {
            $(".report-title-task").hide();            
        }
        
        ObjectJS.bindEvent();       
        ObjectJS.getReportList();
        ObjectJS.getNeedOrderList();
        ObjectJS.getTaskOrOrderEcceedCount();

        setTimeout(function () {
            ObjectJS.authorWarn();
        }, 500);
    };

    ObjectJS.bindEvent = function () {
       
        $(document).click(function (e) {
            if (!$(e.target).parents().hasClass('dropdown-items-modules') && !$(e.target).parents().hasClass('dropdown-module')) {
                $(".dropdown-items-modules").hide();
            }
        });

        /*根据时间段查询*/
        $(".select-time").addClass("usable");
        $(".select-time-left").click(function () {
            var _this = $(".time-now");
            if (Paras.filterTimeType == 1) {
                _this.html(_this.data("before"));
                $(".select-time-left").removeClass("usable").addClass("disable");

                Paras.filterTimeType = 0;
            } else if (Paras.filterTimeType == 2) {
                _this.html(_this.data("now"));
                $(".select-time-right").removeClass("disable").addClass("usable");

                Paras.filterTimeType = 1;                
            } else { 
                return;                
            }
            ObjectJS.getReportList();
            ObjectJS.getDataList();
        });

        $(".select-time-right").click(function () {
            var _this = $(".time-now");
            if (Paras.filterTimeType == 1) {
                _this.html(_this.data("after"));
                $(".select-time-right").removeClass("usable").addClass("disable");

                Paras.filterTimeType = 2;
            } else if (Paras.filterTimeType == 0) {
                _this.html(_this.data("now"));
                $(".select-time-left").removeClass("disable").addClass("usable");

                Paras.filterTimeType = 1;                
            } else {
                $(this).css("color", "#ccc !important");
                return;
            }

            ObjectJS.getReportList();
            ObjectJS.getDataList();
        });

        /*模块筛选*/
        $(".order-type span").click(function () {
            var _this = $(this);

            if (!_this.hasClass("hover")) {
                if (IsLoadding && IsLoaddingTwo) {                    
                    _this.addClass('hover').siblings().removeClass('hover');
                    Paras.moduleType = _this.data('id');
                    Paras.userID = '';
                    $(".choosebranch-text").html("人员筛选-全部");
                    
                    if (Paras.moduleType == 2) {
                        $(".task-status").show();
                        $("#taskType").show();                        

                        if (ObjectJS.taskLevel == 1) {
                            $("#chooseBranch").show();                            
                        } else {
                            $("#chooseBranch").hide();
                        }
                        
                    } else {
                        if (ObjectJS.orderLevel == 1) {
                            $("#chooseBranch").show();
                        } else {
                            $("#chooseBranch").hide();
                        }
                    }

                    $(".get-need").find("span:first").addClass("hover");
                    $(".get-ecceed").find("span:first").removeClass("hover");
                    ObjectJS.getReportList();
                    ObjectJS.getNeedOrderList();
                    ObjectJS.getTaskOrOrderEcceedCount();
                } else {
                    alert("数据加载中，请稍等 !");
                }
            }
        });

        /*负责人筛选*/
        if (ObjectJS.orderLevel == 1 || ObjectJS.taskLevel == 1) {
            require.async("choosebranch", function () {
                $("#chooseBranch").chooseBranch({
                    prevText: "人员筛选-",
                    defaultText: "全部",
                    defaultValue: "",
                    userid: "-1",
                    isTeam: false,
                    width: "130",
                    onChange: function (data) {
                        Paras.userID = data.userid;
                        Paras.pageIndex = 1;

                        ObjectJS.getReportList();
                        ObjectJS.getDataList();
                        ObjectJS.getTaskOrOrderEcceedCount();
                    }
                });
            });
        }

        /*订单类型选择*/
        require.async("dropdown", function () {
            var orderTypes = [{ ID: "1", Name: "打样" }, { ID: "2", Name: "大货" }];
            $("#orderType").dropdown({
                prevText: "订单类型-",
                defaultText: "全部",
                defaultValue: "-1",
                data: orderTypes,
                dataValue: "ID",
                dataText: "Name",
                width: "120",
                onChange: function (data) {
                    if (Paras.orderType != data.value) {
                        if (IsLoadding && IsLoaddingTwo) {
                            Paras.orderType = data.value;
                            Paras.pageIndex = 1;

                            ObjectJS.getReportList();
                            ObjectJS.getDataList();
                            ObjectJS.getTaskOrOrderEcceedCount();
                        } else {
                            alert("数据加载中，请稍等 !");
                        }
                    }
                }
            });
        });

        /*订单进行状态筛选*/
        $(".sum-list li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("active")) {
                _this.addClass("active").siblings().removeClass("active");

                ObjectJS.orderFilter = _this.data("orderfilter");
                ObjectJS.bindReport();
            }
        });

        /*上级任务状态筛选*/
        $(".task-status-ul li").click(function () {
            var _this = $(this), id = _this.data("taskstatusid");
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");

                Paras.preFinishStatus = id;;
                Paras.pageIndex = 1;
                ObjectJS.getDataList();
                ObjectJS.getTaskOrOrderEcceedCount();
            }           
        });

        /*任务模块选择*/
        require.async("dropdown", function () {
            var taskTypes = [{ ID: "1", Name: "材料" }, { ID: "2", Name: "制版" }, { ID: "3", Name: "裁剪" }, { ID: "4", Name: "车缝" }, { ID: "5", Name: "发货" }, { ID: "6", Name: "手工成本" }, { ID: "0", Name: "其他" }];
            $("#taskType").dropdown({
                prevText: "任务模块-",
                defaultText: "全部",
                defaultValue: "-1",
                data: taskTypes,
                dataValue: "ID",
                dataText: "Name",
                width: "110",
                onChange: function (data) {
                    if (Paras.taskType != data.value) {
                        if (IsLoadding && IsLoaddingTwo) {
                            Paras.taskType = data.value;
                            Paras.pageIndex = 1;

                            ObjectJS.getDataList();
                            ObjectJS.getTaskOrOrderEcceedCount();
                        } else {
                            alert("数据加载中，请稍等 !");
                        }
                    }
                }
            });
        });

        //获取需求订单或未接受任务
        $(".get-need").click(function () {
            if (IsLoadding && IsLoaddingTwo) {
                if (Paras.filterTime != "" || Paras.filterType != -1) {
                    $(this).find("span:first").addClass("hover");
                    $(".get-ecceed").find("span:first").removeClass("hover");
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
                if (Paras.filterTime != "" || Paras.filterType != 1) {
                    $(this).find("span:first").addClass("hover");
                    $(".get-need").find("span:first").removeClass("hover");
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

    //授权快到期提示
    ObjectJS.authorWarn = function () {
        if (ObjectJS.remainDay <= 20) {
            var authorWarn = Global.getCookie('authorWarn');
            if (authorWarn != "1") {
                var data = { remainDay: ObjectJS.remainDay, remainDate: ObjectJS.remainDate };
                EasyDialog = require('easydialog');
                DoT.exec("/template/home/author-page.html", function (template) {
                    var innerHtml = template(data);
                    EasyDialog.open({
                        container: {
                            id: "author-box",
                            header: "授权快到期",
                            content: innerHtml
                        }
                    });
                    $("#ExtendNow").click(function () {
                        window.open('/Auction/ExtendNow', '_target');
                    })

                    $('.no-hint').click(function () {
                        var _this = $(this);
                        if ($(".author-lump").hasClass('hover')) {
                            $(".author-lump").removeClass('hover');
                            Global.delCookie('authorWarn');
                        }
                        else {
                            $(".author-lump").addClass('hover');
                            Global.setCookie('authorWarn', '1');
                        }
                    })
                })
            }
        }
    }

    /*获取报表数据*/
    ObjectJS.getReportList = function () {
        $(".report-guid").nextAll().remove();
        var loadding = "<div class='data-loading'>";
        $(".report-guid").append(loadding);

        var data = CacheArr[Paras.filterTime + Paras.filterType + Paras.moduleType + Paras.orderType + Paras.filterTimeType+Paras.userID+Paras.taskType + "ReportList"];
        
        if (data == null) {
            Global.post("/Home/GetOrdersOrTasksReportData", {
                orderType: Paras.orderType,
                filterTimeType: Paras.filterTimeType,
                moduleType: Paras.moduleType,
                taskType: Paras.taskType,
                userID:Paras.userID
            }, function (data) {
                $(".report-guid").find('.data-loading').remove();

                CacheArr[Paras.filterTime + Paras.filterType + Paras.moduleType + Paras.orderType + Paras.filterTimeType + Paras.userID + Paras.taskType + "ReportList"] = data;
                OrderListCache = data.items;
                ObjectJS.bindReport();

                $("#totalSumCount").html(data.totalSumCount);
                $("#totalExceedCount").html(data.totalExceedCount);
                $("#totalFinishCount").html(data.totalFinishCount);
                $("#totalWarnCount").html(data.totalWarnCount);
                $("#totalWorkCount").html(data.totalWorkCount);
                $("#totalSumCount").html(data.totalSumCount);
            });
        } else {
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

            /*报表色块点击*/
            $(".report-item li").click(function () {
                $(".get-need").find("span:first").removeClass("hover");
                $(".get-ecceed").find("span:first").removeClass("hover");
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

    /*拼接报表柱状图形*/
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
                    html += '<li style="line-height:' + lineHeight + 'px;" class="' + classname + '" data-totalcount="' + item.totalCount + '" data-count="' + count + '" data-date="' + item.dateTime + '" data-type="' + type + '">' + count + '</li>';
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

    /*获取列表数据*/
    ObjectJS.getDataList = function () {        
        IsLoaddingTwo = false;
        var data = null;
        if (Paras.moduleType == 2) {
            $(".task-status").show();
            $("#taskType").show();
        }
        else {
            Paras.preFinishStatus = -1;
            $(".task-status").hide();
            $("#taskType").hide();
        }
        if (Paras.pageIndex == 1) {
            $(".order-layerbox").empty();
            data = CacheArr[Paras.filterTime + Paras.filterType + Paras.moduleType + Paras.orderType + Paras.userID + Paras.taskType + Paras.preFinishStatus + Paras.filterTimeType + "DataList"];
        }
        $(".order-layerbox").append("<div class='data-loading'></div>");

        if (data == null) {
            Global.post("/Home/GetOrdersOrTasksDataList", Paras, function (data) {
                IsLoaddingTwo = true;
                $('.data-loading').remove();

                if (Paras.pageIndex == 1) {
                    CacheArr[Paras.filterTime + Paras.filterType + Paras.moduleType + Paras.orderType + Paras.userID + Paras.taskType + Paras.preFinishStatus + Paras.filterTimeType + "DataList"] = data;
                }
                ObjectJS.createDataListHtml(data);
            })
        }
        else {
            ObjectJS.createDataListHtml(data);
        }
    }

    /*拼接列表数据*/
    ObjectJS.createDataListHtml = function (data) {
        $('.data-loading').remove();
       
        IsLoaddingTwo = true;
        
        var url = "";
        if (Paras.moduleType == 2) {
            url = "/template/home/index-task.html";
        } else {
            url = "/template/home/index-order.html";
        }
        var items = data.items;
        
        if (items.length == 0) {
            $(".order-layerbox").append("<div class='nodata-txt'>暂无数据!<div>");
            $(".load-box").hide();
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
                innerText.find(".customermark").Tip({
                    width: 60,
                    msg: "客户名称"
                });                
                innerText.find(".orderquantity").Tip({
                    width: 60,
                    msg: "下单数量"
                });
                innerText.find(".ordertype-title").Tip({
                    width: 60,
                    msg: "订单类型"
                })
            });
        }

        /*如果最后已到了最后一页则移除加载更多按钮*/
        if (data.pageCount == Paras.pageIndex) {
            $(".load-box").hide();
        } else {
            if (data.pageCount > 1) {
                $(".load-box").show();
            }
        }

        /*文字说明切换*/
        var reportTotalTtitle = "全部订单";        
        var totalEcceedTtitle = "需求订单总数:";

        if (ObjectJS.roleLevel==1) {
            if (Paras.moduleType == 2) {
                reportTotalTtitle = "全部任务";
                totalEcceedTtitle = "未接收任务总数:";
            }
        }
        $(".report-total-title").html(reportTotalTtitle); 
        
        var $listTitle = $(".list-title");
        if (Paras.filterTime != '') {
            $listTitle.html(data.showTime);
        } else {
            var listTitle = "需求订单总数";
            if (Paras.moduleType == 2) {
                listTitle = "未接受任务总数";
            }
            if (Paras.filterType == 1) {
                listTitle = "已超期订单总数";
                $(".list-total").css({ "background-color": "#f35353" });
            } else {
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
        var data = CacheArr[Paras.filterTime + Paras.filterType + Paras.moduleType + Paras.orderType + Paras.userID + Paras.taskType + "TaskOrOrderCount"];
        if (data == null) {
            Global.post("/Home/GetTaskOrOrderEcceedCount", Paras, function (data) {
                CacheArr[Paras.filterTime + Paras.filterType + Paras.moduleType + Paras.orderType + Paras.userID + Paras.taskType + "TaskOrOrderCount"] = data;
                
                var name = "超期订单总数:";
                var needname = "需求订单总数:";
                if (Paras.moduleType == 2) {
                    name = "超期任务总数:";
                    needname = "未接受任务总数:";
                }
                
                $(".total-ecceed").html(data.result).prev().html(name);
                $(".get-need").find("span:first").html(needname);
            });
        } else {
            var name = "超期订单总数:";
            var needname = "需求订单总数:";
            if (Paras.moduleType == 2) {
                name = "超期任务总数:";
                needname = "未接收任务总数:";
            }
            $(".total-ecceed").html(data.result).prev().html(name);
            $(".get-need").find("span:first").html(needname);
        }
    }

    module.exports= ObjectJS;
});