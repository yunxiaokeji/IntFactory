﻿define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot");
    require("pager");
    require("mark");

    var Params = {
        isMy: true,
        userID: "",
        isParticipate:0,
        taskType: -1,
        colorMark: -1,
        status: 1,
        finishStatus:0,
        keyWords:"",
        beginDate: "",
        endDate: "",
        orderType: -1,
        orderProcessID: "-1",
        orderStageID: "-1",
        taskOrderColumn: 0,
        isAsc:0,
        pageSize: 10,
        pageIndex:1
    };

    var ObjectJS = {};

    ObjectJS.init = function (isMy, nowDate) {
        Params.beginDate = nowDate;
        Params.endDate = nowDate;
        Params.pageSize = ($(".content-body").width() / 280).toFixed(0) * 3;

        ObjectJS.showType = "list";
        if (isMy == 2) {
            Params.isParticipate = 1;

        }

        if (isMy == "0")
        {
            Params.isMy = false;
            document.title = "所有任务";
            $(".header-title").html("所有任务");

            require.async("choosebranch", function () {
                $("#chooseBranch").chooseBranch({
                    prevText: "人员-",
                    defaultText: "全部",
                    defaultValue: "",
                    userid: "-1",
                    isTeam: false,
                    width: "140",
                    onChange: function (data) {
                        Params.pageIndex = 1;
                        Params.userID = data.userid;
                        ObjectJS.getList();
                    }
                });
            });
        }
        else if (isMy == "2") {
            document.title = "参与任务";
            $(".header-title").html("参与任务");
        }

        ObjectJS.bindEvent();

        ObjectJS.getList();

        if (Params.isParticipate == 1) {
            $(".search-stages li").eq(2).click();
        }
    }

    ObjectJS.bindEvent = function () {
        //关键字查询
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                Params.pageIndex = 1;
                Params.keyWords = keyWords;
                ObjectJS.getList();
            });
        });

        //切换颜色标记
        $(".search-item-color li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");

                Params.pageIndex = 1;
                Params.colorMark = _this.data("id");
                ObjectJS.getList();
            }
        });

        //切换阶段
        $(".search-stages li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");

                Params.pageIndex = 1;
                Params.finishStatus = _this.data("id");
                ObjectJS.getList();
            }
        });


        //切换订单类型
        $(".search-ordertype .item").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");

                Params.pageIndex = 1;
                Params.orderType = _this.data("id");
                ObjectJS.getList();
            }
        });

        //订单流程阶段搜索
        require.async("dropdown", function () {

            Global.post("/Task/GetOrderProcess", null, function (data) {
                $("#orderProcess").dropdown({
                    prevText: "订单流程-",
                    defaultText: "全部",
                    defaultValue: "-1",
                    data: data.items,
                    dataValue: "ProcessID",
                    dataText: "ProcessName",
                    width: "140",
                    onChange: function (data) {
                        Params.orderProcessID = data.value;
                        Params.orderStageID = "-1";
                        Params.pageIndex = 1;
                        ObjectJS.getList();

                        Global.post("/Task/GetOrderStages", { id: data.value }, function (data) {

                            $("#orderStage").dropdown({
                                prevText: "流程阶段-",
                                defaultText: "全部",
                                defaultValue: "-1",
                                data: data.items,
                                dataValue: "StageID",
                                dataText: "StageName",
                                width: "140",
                                onChange: function (data) {
                                    Params.orderStageID = data.value;
                                    Params.pageIndex = 1;
                                    ObjectJS.getList();
                                }
                            });

                        });

                    }
                });

            });

        });

        //切换任务显示方式(列表或者卡片式)
        //.search-sort   .search-header .task-tabtype span
        $(".search-sort .task-tabtype i").click(function () {
            var _this = $(this);
            ObjectJS.showType = _this.data('type');
            _this.addClass('checked').siblings().removeClass('checked');
            ObjectJS.getList();
        });

        //时间段查询
        $("#btnSearch").click(function () {
            Params.pageIndex = 1;
            Params.beginDate = $("#BeginTime").val();
            Params.endDate = $("#EndTime").val();
            ObjectJS.getList();
        });

        //列表排序
        $(".sort-item").click(function () {
            var _self = $(this);
            if (!_self.hasClass("hover")) {
                _self.addClass("hover").siblings().removeClass("hover");
            }
            var isasc = _self.data("isasc");
            var isactive = _self.data("isactive");
            var orderbycloumn = _self.data("orderbycloumn");

            $(".search-sort li[data-isactive='1']").data("isactive", 0).children().removeClass("hover");
            if (isactive == 1) {
                if (isasc == 1) {
                    _self.find(".asc").removeClass("hover");
                    _self.find(".desc").addClass("hover");
                }
                else {
                    _self.find(".desc").removeClass("hover");
                    _self.find(".asc").addClass("hover");
                }
                
                isasc = isasc == 1 ? 0 : 1;
            }
            else {
                if (isasc == 1) {
                    _self.find(".desc").removeClass("hover");
                    _self.find(".asc").addClass("hover");
                }
                else {
                    _self.find(".asc").removeClass("hover");
                    _self.find(".desc").addClass("hover");
                }

            }
            _self.data({ "isasc": isasc, "isactive": 1 });
            Params.isAsc = isasc;
            Params.taskOrderColumn = orderbycloumn;
            Params.pageIndex = 1;
            ObjectJS.getList();

        });
    }

    ObjectJS.getList = function () {
        var showtype = ObjectJS.showType;
        $(".tr-header").nextAll().remove();
        if (showtype == "list") {
            $(".task-items").hide();
            $(".table-list").show();
            $(".tr-header").after("<tr><td colspan='10'><div class='data-loading'><div></td></tr>");
        }
        else {
            $(".table-list").hide();
            $(".task-items").show();
            $(".task-items").html("<div class='data-loading'><div>");
        }
    
        Global.post("/Task/GetTasks", Params, function (data) {
            $(".tr-header").nextAll().remove();
            $(".content-body").find('.nodata-txt').remove();
            if (data.items.length > 0) {
                doT.exec("template/task/task-"+showtype+".html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);
                    
                    innerhtml.find(".mark").markColor({
                        isAll: false,
                        onChange: function (obj, callback) {
                            ObjectJS.markTasks(obj.data("id"), obj.data("value"), callback);
                        }

                    });

                    innerhtml.find(".picbox img").each(function () {
                        if ($(this).width() > $(this).height()) {
                            $(this).css("width", 248);
                        } else if ($(this).width() < $(this).height()) {
                            $(this).css("height", 248);
                        } else {
                            $(this).css("height", 248);
                        }
                    });

                    if (showtype == "list") {
                        $(".table-list").append(innerhtml);
                    }
                    else {
                        $(".task-items").html(innerhtml);
                        if (Params.finishStatus == 1 || Params.finishStatus == -1) {

                            for (var i = 0; i < data.items.length; i++) {
                                if (data.items[i].FinishStatus == 1) {
                                    
                                    ObjectJS.showTime(data.items[i], data.isWarns[i],data.endTimes[i]);
                                }
                            }
                        }
                    }   
                });
               
            }
            else {
                if (showtype == "list") {
                    $(".table-list").after("<div class='nodata-txt' >暂无数据!<div>");
                }
                else {
                    $(".task-items").html("<div class='nodata-txt' >暂无数据!<div>");
                }
            }

            $("#pager").paginate({
                total_count: data.totalCount,
                count: data.pageCount,
                start: Params.pageIndex,
                display: 5,
                images: false,
                mouse: 'slide',
                onChange: function (page) {
                   $(".tr-header").nextAll().remove();
                   Params.pageIndex = page;
                   ObjectJS.getList();
                }
            });

        });
    }

    //任务到期时间倒计时
    ObjectJS.showTime = function (item, isWarn,endTime) {

        var endtime = item.EndTime.toDate("yyyy/MM/dd hh:mm:ss");
        var num = item.TaskID;
        if (ObjectJS.status == 8) {
            return;
        }

        if (endTime == "未设置") {
            return;
        }

        if (ObjectJS.finishStatus == 2) {
            return;
        }
        debugger;
        var time_end = (new Date(endtime)).getTime();

        var time_start = new Date().getTime(); //设定当前时间

        // 计算时间差 
        var time_distance = time_end - time_start;
        var overplusTime = false;
        if (time_distance < 0) {
            if (!overplusTime) {
                $(".overplusTime-" + num + "").html("超期时间：");

            }
            overplusTime = true;
            time_distance = time_start - time_end;
        }
        else {
            if (isWarn == 1) {
                if (!overplusTime) {

                }
                overplusTime = true;
            }
        }

        // 天
        var int_day = Math.floor(time_distance / 86400000)
        time_distance -= int_day * 86400000;
        // 时
        var int_hour = Math.floor(time_distance / 3600000)
        time_distance -= int_hour * 3600000;
        // 分
        var int_minute = Math.floor(time_distance / 60000)
        time_distance -= int_minute * 60000;
        // 秒 
        var int_second = Math.floor(time_distance / 1000)
        // 时分秒为单数时、前面加零 
        if (int_day < 10) {
            int_day = "0" + int_day;
        }
        if (int_hour < 10) {
            int_hour = "0" + int_hour;
        }
        if (int_minute < 10) {
            int_minute = "0" + int_minute;
        }
        if (int_second < 10) {
            int_second = "0" + int_second;
        }
        // 显示时间 
        $(".time-d-" + num + "").html(int_day);
        $(".time-h-" + num + "").html(int_hour);
        $(".time-m-" + num + "").html(int_minute);
        $(".time-s-" + num + "").html(int_second);
    }

    //任务颜色标记
    ObjectJS.markTasks = function (ids, mark, callback) {
        if (mark < 0) {
            alert("不能标记此选项!");
            return false;
        }
        Global.post("/Task/UpdateTaskColorMark", {
            ids: ids,
            mark: mark
        }, function (data) {
            if (data.result == "10001") {
                alert("您没有标记任务的权限！");
                callback && callback(false);
            } else {
                callback && callback(data.result);
            }
        });
    }

    module.exports = ObjectJS;
});