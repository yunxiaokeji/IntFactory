define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot");
    require("pager");
    require("mark");

    var Params = {
        isMy: true,//是否获取我的任务
        userID: "",
        isParticipate:0,//是否获取我参与任务
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
        taskOrderColumn: 0,//0:创建时间；2：到期时间
        isAsc:0,
        pageSize: 10,
        pageIndex:1
    };

    var ObjectJS = {};
    ObjectJS.isLoading = true;
    ObjectJS.showType = "list";

    ObjectJS.init = function (isMy, nowDate) {
        Params.beginDate = nowDate;
        Params.endDate = nowDate;
        Params.pageSize = ($(".content-body").width() / 300).toFixed(0) * 3;

        if (isMy == 2) {
            Params.isParticipate = 1;
            document.title = "参与任务";
            $(".header-title").html("参与任务");
        }
        else if (isMy == 0)
        {
            Params.isMy = false;
            document.title = "所有任务";
            $(".header-title").html("所有任务");

            //人员筛选
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

        ObjectJS.bindEvent();

        ObjectJS.getProcess();

        //获取任务列表
        if (Params.isParticipate != 1) {
            ObjectJS.getList();
        }

    }

    ObjectJS.bindEvent = function () {
        //切换任务阶段
        $(".search-stages li").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");

                Params.pageIndex = 1;
                Params.finishStatus = _this.data("id");
                ObjectJS.getList();
            }
        });

        if (Params.isParticipate == 1) {
            $(".search-stages li").eq(2).click();
        }

        //关键字查询 任务编码、订单编码、任务标题
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                if (!ObjectJS.isLoading) {
                    return;
                }
                Params.pageIndex = 1;
                Params.keyWords = keyWords;
                ObjectJS.getList();
            });
        });
        
        //切换颜色标记
        $(".search-item-color li").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");

                Params.pageIndex = 1;
                Params.colorMark = _this.data("id");
                ObjectJS.getList();
            }
        });

        //切换订单类型
        $(".search-ordertype .item").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");

                $(".search-process .item").removeClass('hover').eq(0).addClass('hover');
                $(".search-stage .item").removeClass('hover').eq(0).addClass('hover').nextAll().remove();
                Params.orderType = _this.data("id");
                if (Params.orderType != -1) {
                    $(".search-process .item[data-type='" + Params.orderType + "']").show();
                    $(".search-process .item[data-type!='" + Params.orderType + "']:gt(0)").hide();
                }
                else {
                    $(".search-process li").show();
                }

                Params.orderProcessID = '-1';
                Params.orderStageID = '-1';
                $(".search-stage").hide();
                ObjectJS.getList();
            }
        });

        //切换任务显示方式(列表或者卡片式)
        $(".search-sort .task-tabtype i").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            var _this = $(this);
            if (!_this.hasClass('checked')) {
                _this.addClass('checked').siblings().removeClass('checked');

                ObjectJS.showType = _this.data('type');
                ObjectJS.getList();
            }
        });

        //时间段查询
        $("#btnSearch").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            Params.pageIndex = 1;
            Params.beginDate = $("#BeginTime").val();
            Params.endDate = $("#EndTime").val();
            ObjectJS.getList();
        });

        //列表排序
        $(".sort-item").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            var _self = $(this);
            if (!_self.hasClass("hover")) {
                _self.addClass("hover").siblings().removeClass("hover");
            }
            var isasc = _self.data("isasc");
            var isactive = _self.attr("data-isactive");
            var orderbycloumn = _self.data("orderbycloumn");

            $(".search-sort li[data-isactive='1']").data("isactive", 0).find("span").removeClass("hover");

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
            _self.data("isasc",isasc).attr("data-isactive",1);

            Params.isAsc = isasc;
            Params.taskOrderColumn = orderbycloumn;
            Params.pageIndex = 1;
            ObjectJS.getList();

        });
    }

    //获取订单流程
    ObjectJS.getProcess = function () {
        ObjectJS.isLoading = false;
        Global.post("/Task/GetOrderProcess", null, function (data) {
            var items = data.items;
            var content = "<li class='item hover' data-id='-1' data-type='-1'>全部</li>";
            for (var i = 0; i < items.length; i++) {
                var item=items[i];
                content += "<li data-type=" + item.ProcessType + " data-id=" + item.ProcessID + " class='item'>" + item.ProcessName + "</li>";
            }
            content = $(content);
            $(".search-process").append(content);

            content.click(function () {
                var _this = $(this);
                if (!_this.hasClass("hover")) {
                    _this.siblings().removeClass("hover");
                    _this.addClass("hover");

                    Params.orderProcessID = _this.data('id');
                    Params.orderStageID = "-1";
                    Params.pageIndex = 1;
                    $(".search-stage").hide();
                    ObjectJS.getList();

                    ObjectJS.getStage();
                }
            });
            ObjectJS.isLoading = true;
        });
    }

    //获取订单阶段
    ObjectJS.getStage = function () {
        $(".search-stage").show();
        ObjectJS.isLoading = false;
        Global.post("/Task/GetOrderStages", { id: Params.orderProcessID }, function (data) {
            var items = data.items;
            var content = "<li class='item hover' data-id='-1'>全部</li>";
            for (var i = 0; i < items.length; i++) {
                content += "<li data-id=" + items[i].StageID + " class='item'>" + items[i].StageName + "</li>";
            }
            content = $(content);
            $(".search-stage .column-name").nextAll().remove();
            $(".search-stage").append(content);
            content.click(function () {
                var _this = $(this);
                if (!_this.hasClass("hover")) {
                    _this.siblings().removeClass("hover");
                    _this.addClass("hover");
                    Params.orderStageID = _this.data('id');
                    Params.pageIndex = 1;
                    ObjectJS.getList();
                }
            });
            ObjectJS.isLoading = true;
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
        $(".content-body").find('.nodata-txt').remove();
        ObjectJS.isLoading = false;

        Global.post("/Task/GetTasks", Params, function (data) {
            $(".tr-header").nextAll().remove();

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
                    }
                    if (Params.finishStatus == 1 || Params.finishStatus == -1) {
                        for (var i = 0; i < data.items.length; i++) {
                            var item = data.items[i];
                            if (item.FinishStatus == 1) {
                                ObjectJS.showTime(item, data.isWarns[i], data.endTimes[i], showtype);
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

            ObjectJS.isLoading = true;
        });
    }

    //任务到期时间倒计时
    ObjectJS.showTime = function (item, isWarn,endTime,showType) {
        if (ObjectJS.status == 8) {
            return;
        }
        if (item.FinishStatus != 1) {
            return;
        }

        var endtime = item.EndTime.toDate("yyyy/MM/dd hh:mm:ss");
        var num = item.TaskID;
        var time_end = (new Date(endtime)).getTime();
        var time_start = new Date().getTime(); //设定当前时间
        // 计算时间差 
        var time_distance = time_end - time_start;
        var overplusTime = false;

        if (time_distance < 0) {
            if (!overplusTime) {
                if (showType == "card") {
                    $(".overplusTime-" + num + "").html("超期：");
                    $(".overplusTime-" + num + "").parents('.picbox').find(".hint-layer").show();
                    $(".overplusTime-" + num + "").parents('.picbox').find(".hint-msg").html('已超期').css({ "background-color": "rgba(237,0,0,0.7)", "background-color": "rgba(237,0,0,0.7)" }).show();
                }
                else {
                    var $list_picbox=$(".table-list .list-item[data-taskid='" + item.TaskID + "']");
                    $list_picbox.find(".hint-msg").html("已超期").css({ "background-color": "rgba(237,0,0,0.7)", "background-color": "rgba(237,0,0,0.7)" }).show();
                }
            }

            overplusTime = true;
            time_distance = time_start - time_end;
        }
        else {
            if (isWarn == 1) {
                if (!overplusTime) {
                    if (showType == "card") {
                        $(".overplusTime-" + num + "").html("剩余：");
                        $(".overplusTime-" + num + "").parents('.picbox').find(".hint-layer").show();
                        $(".overplusTime-" + num + "").parents('.picbox').find(".hint-msg").html('快到期').show().css({ "background-color": "rgba(255,165,0,0.7)", "background-color": "rgba(255,165,0,0.7)" });
                    }
                    else {
                        var $list_picbox = $(".table-list .list-item[data-taskid='" + item.TaskID + "']");
                        $list_picbox.find(".hint-msg").html("快到期").css({ "background-color": "rgba(255,165,0,0.7)", "background-color": "rgba(255,165,0,0.7)" }).show();
                    }
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
        //var int_second = Math.floor(time_distance / 1000)
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
        //if (int_second < 10) {
        //    int_second = "0" + int_second;
        //}
        // 显示时间 
        $(".time-d-" + num + "").html(int_day);
        $(".time-h-" + num + "").html(int_hour);
        $(".time-m-" + num + "").html(int_minute);
        //$(".time-s-" + num + "").html(int_second);
    }

    //任务颜色标记
    ObjectJS.markTasks = function (ids, mark, callback) {
        if (mark < 0) {
            alert("不能标记此选项!");
            return false;
        }
        ObjectJS.isLoading = false;
        Global.post("/Task/UpdateTaskColorMark", {
            ids: ids,
            mark: mark
        }, function (data) {
            if (data.result == "10001") {
                alert("您没有标记任务的权限！");
                callback && callback(false);
            }
            else {
                callback && callback(data.result);
            }
            ObjectJS.isLoading = true;
        });
    }

    module.exports = ObjectJS;
});