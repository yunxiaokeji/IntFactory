define(function (require, exports, module) {
    var Global = require("global");
    var Tip = require("tip");
    var DoT = require("dot");
    var EasyDialog = null;
    var Pager = require("pager");
    require("colormark");

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
        userID: "",
    }

    var ObjectJS = {};
    ObjectJS.orderFilter = -1;

    ObjectJS.init = function (orderLevel, taskLevel, remainDay, remainDate, orderMarks, tastMarks,currentUserID) {
        ObjectJS.remainDay = remainDay;
        ObjectJS.remainDate = remainDate;        
        ObjectJS.orderLevel = orderLevel;
        ObjectJS.taskLevel = taskLevel;
        ObjectJS.currentUserID = currentUserID;
        ObjectJS.orderMarks = JSON.parse(orderMarks.replace(/&quot;/g, '"'));
        ObjectJS.tastMarks = JSON.parse(tastMarks.replace(/&quot;/g, '"'));

        if (orderLevel == "0" && taskLevel == "0") {
            return;
        }

        if (orderLevel == "-1") {
            $(".report-title").html('所有订单');
            $("#chooseBranch").show();
        } else if (orderLevel == "0") {
            $(".report-title").remove();
            $(".report-title-task").addClass("hover");
            Paras.moduleType = 2;
        } else {

        }

        if (taskLevel == "-1") {
            $(".report-title-task").html('所有任务');
            $("#chooseBranch").show();
        } else if (taskLevel == 0) {
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

            if (!$(e.target).parents().hasClass("order-layer") && !$(e.target).hasClass("order-layer")) {
                $(".order-layer").animate({ right: "-505px" }, 200);
                $(".object-item").removeClass('looking-view');
            }
        });

        //关闭浮层
        $("#closeLayer").click(function () {
            $(".order-layer").animate({ right: "-505px" }, 200);
            $(".object-item").removeClass('looking-view');
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

                        if (ObjectJS.taskLevel == "-1") {
                            $("#chooseBranch").show();                            
                        } else {
                            $("#chooseBranch").hide();
                        }
                        
                    } else {
                        if (ObjectJS.orderLevel == "-1") {
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
        if (ObjectJS.orderLevel == "-1" || ObjectJS.taskLevel == "-1") {
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
            var taskTypes = [{ ID: "1", Name: "材料" }, { ID: "2", Name: "制版" }, { ID: "3", Name: "裁片/织机" }, { ID: "4", Name: "车缝/缝盘" }, { ID: "5", Name: "发货" }, { ID: "6", Name: "工价" }, { ID: "0", Name: "其他" }];
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
                            header:ObjectJS.remainDay>0?"授权快到期":"授权已超期",
                            content: innerHtml
                        }
                    });
                    $("#ExtendNow").click(function () {
                        window.open('/Auction/ExtendNow', '_target');
                    });

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
                    });
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

    //更改任务到期时间
    ObjectJS.updateTaskEndTime = function (taskid, maxhours) {
        if (maxhours == 0) {
            Easydialog = require("easydialog");
            DoT.exec("/template/task/set-endtime.html", function (template) {
                var innerHtml = template();
                Easydialog.open({
                    container: {
                        id: "show-model-setRole",
                        header: "设置任务到期时间",
                        content: innerHtml,
                        yesFn: function () {
                            if ($("#UpdateTaskEndTime").val() == "") {
                                alert("任务到期时间不能为空");
                                return false;
                            }
                            Global.post("/Task/UpdateTaskEndTime", {
                                id: taskid,
                                endTime: $("#UpdateTaskEndTime").val()
                            }, function (data) {
                                if (data.result == 0) {
                                    alert("操作无效");
                                }
                                else if (data.result == 2) {
                                    alert("任务已接受,不能操作");
                                }
                                else if (data.result == 3) {
                                    alert("没有权限操作");
                                }
                                else {
                                    alert("接受成功");
                                    $(".btn-accept[data-id=" + taskid + "]").next().html('结束日期：' + new Date($("#UpdateTaskEndTime").val()).toString('yyyy-MM-dd'));
                                    $(".btn-accept[data-id=" + taskid + "]").parents('ul').find('.accept-status').html('进行中').css({ "color": "#02C969" });
                                    $(".btn-accept[data-id=" + taskid + "]").parents('ul').find('.accept-date').html(new Date().toString('yyyy-MM-dd'));
                                    $(".btn-accept[data-id=" + taskid + "]").unbind().html("进行中").removeClass('btn').removeClass('btn-accept').css({ "color": "#02C969" });
                                }
                            });
                        }
                    }
                });

                var myDate = new Date();
                var minDate = myDate.toLocaleDateString();
                minDate = minDate + " 23:59:59"
                //更新任务到期日期
                var taskEndTime = {
                    elem: '#UpdateTaskEndTime',
                    format: 'YYYY/MM/DD hh:mm:ss',
                    min: minDate,
                    istime: true,
                    istoday: false
                };
                laydate(taskEndTime);
            });
        }
        else {
            Global.post("/Task/UpdateTaskEndTime", {
                id: taskid,
                endTime: ""
            }, function (data) {
                if (data.result == 0) {
                    alert("操作无效");
                }
                else if (data.result == 2) {
                    alert("任务已接受,不能操作");
                }
                else if (data.result == 3) {
                    alert("没有权限操作");
                }
                else {
                    $(".btn-accept[data-id=" + taskid + "]").next(0).html('--');
                    $(".btn-accept[data-id=" + taskid + "]").parents('ul').find('.accept-status').html('进行中').css({ "color": "#02C969" });
                    $(".btn-accept[data-id=" + taskid + "]").parents('ul').find('.accept-date').html(new Date().toString('yyyy-MM-dd'));
                    $(".btn-accept[data-id=" + taskid + "]").unbind().html("进行中").removeClass('btn').removeClass('btn-accept').css({ "color": "#02C969" });
                }
            });
        }
    }

    /*拼接列表数据*/
    ObjectJS.createDataListHtml = function (data) {
        var _self = this;

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
                items.currentUserID = ObjectJS.currentUserID;
                var innerText = template(items);
                innerText = $(innerText);
                if (Paras.moduleType == 2) {
                    /*任务讨论浮层*/
                    require.async("showtaskdetail", function () {
                        innerText.find('.show-task-reply').showtaskdetail();
                    });
                    innerText.find('.btn-accept').click(function () {
                        ObjectJS.updateTaskEndTime($(this).data('id'), $(this).data('maxhours'));
                    });
                }

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

                innerText.find(".mark").markColor({
                    isAll: false,
                    data: Paras.moduleType == 2 ? _self.tastMarks : _self.orderMarks,
                    onChange: function (obj, callback) {
                        _self.markOrdersOrTasks(obj.data("id"), obj.data("value"), callback, Paras.moduleType);
                    }
                });

                innerText.find(".view-detail").click(function () {
                    _self.getDetail($(this).data("id"), $(this).data('code'));
                    $('.object-item').removeClass('looking-view');
                    $(this).parents('.object-item').addClass('looking-view');
                    return false;
                });

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

    ObjectJS.markOrdersOrTasks = function (ids, mark, callback, type) {
        if (mark < 0) {
            alert("不能标记此选项!");
            return false;
        }
        var url = type == 2 ? "/Task/UpdateTaskColorMark" : "/Orders/UpdateOrderMark";
        Global.post(url, {
            ids: ids,
            mark: mark
        }, function (data) {
            if (data.result == "10001") {
                alert("您没有标记权限！");
                callback && callback(false);
            } else {
                callback && callback(data.status);
            }
        });
    }

    ObjectJS.getDetail = function (id, orderCode) {

        $(".order-layer-item").hide();
        if ($(".order-layer").css("right") == "-505px" || $(".order-layer").css("right") == "-505") {
            $(".order-layer").animate({ right: "0px" }, 200);
        }
        $(".order-layer").append("<div class='data-loading'><div>");

        if ($("#" + id).length > 0) {
            $(".order-layer").find(".data-loading").remove();
            $("#" + id).show();
        } else {
            $.get("/Orders/OrderLayer", { id: id }, function (html) {
                $(".order-layer").find(".data-loading").remove();
                $(".order-layer").append(html);

            });
        }
        var detail = "<a class='font14 mLeft5' href='/Orders/OrderDetail/" + id + "'>" + orderCode + "</a>";
        $(".order-layer").find('.layer-header').find('a').remove();
        $(".order-layer").find('.layer-header').append(detail);
    }

    module.exports= ObjectJS;
});