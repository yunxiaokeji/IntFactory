define(function (require, exports, module) {
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

        ObjectJS.bindEvent();

        ObjectJS.getList();
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

        //过滤颜色标记
        $("#filterMark").markColor({
            isAll: true,
            onChange: function (obj, callback) {
                callback && callback(true);
                Params.pageIndex = 1;
                Params.colorMark = obj.data("value");
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

        //切换任务状态
        $(".search-returnstatus li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");

                Params.pageIndex = 1;
                Params.status = _this.data("id");
                ObjectJS.getList();
            }
        });

        //订单流程阶段、任务类型搜索
        require.async("dropdown", function () {
            var Types = [
                {
                    ID: "1",
                    Name: "打样"
                },
                {
                    ID: "2",
                    Name: "大货"
                }
            ];
            $("#orderType").dropdown({
                prevText: "订单类型-",
                defaultText: "全部",
                defaultValue: "-1",
                data: Types,
                dataValue: "ID",
                dataText: "Name",
                width: "120",
                onChange: function (data) {
                    Params.pageIndex = 1;
                    Params.orderType = data.value;
                    ObjectJS.getList();
                }
            });

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

        //时间段查询
        $("#btnSearch").click(function () {
            Params.pageIndex = 1;
            Params.beginDate = $("#BeginTime").val();
            Params.endDate = $("#EndTime").val();
            ObjectJS.getList();
        });

        //列表排序
        $(".orderby-column").click(function () {
            var _self = $(this);
            var isasc = _self.attr("data-isasc");
            var isactive = _self.attr("data-isactive");
            var orderbycloumn = _self.attr("data-orderbycloumn");

            $(".table-list td[data-isactive='1']").attr("data-isactive", 0).children().removeClass("hover");
            if (isactive == 1) {
                if (isasc == 1) {
                    _self.find(".orderby-asc").removeClass("hover");
                    _self.find(".orderby-desc").addClass("hover");
                }
                else {
                    _self.find(".orderby-desc").removeClass("hover");
                    _self.find(".orderby-asc").addClass("hover");
                }
                
                isasc = isasc == 1 ? 0 : 1;
            }
            else {
                if (isasc == 1) {
                    _self.find(".orderby-desc").removeClass("hover");
                    _self.find(".orderby-asc").addClass("hover");
                }
                else {
                    _self.find(".orderby-asc").removeClass("hover");
                    _self.find(".orderby-desc").addClass("hover");
                }

            }

            _self.attr( {"data-isasc": isasc,"data-isactive":1} );

            Params.isAsc = isasc;
            Params.taskOrderColumn = orderbycloumn;
            Params.pageIndex = 1;
            ObjectJS.getList();

        });
    }

    ObjectJS.getList = function () {
        $(".tr-header").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='10'><div class='data-loading'><div></td></tr>");

        Global.post("/Task/GetTasks", Params, function (data) {
            $(".tr-header").nextAll().remove();

            if (data.items.length > 0) {
                doT.exec("template/task/task-list.html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);

                    innerhtml.find(".mark").markColor({
                        isAll: false,
                        onChange: function (obj, callback) {
                            ObjectJS.markTasks(obj.data("id"), obj.data("value"), callback);
                        }
                    });

                    $(".tr-header").after(innerhtml);
                });
            }
            else {
                $(".tr-header").after("<tr><td colspan='10'><div class='nodata-txt' >暂无数据!<div></td></tr>");
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