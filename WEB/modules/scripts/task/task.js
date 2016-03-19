define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot");
    require("pager");

    var Params = {
        isMy: true,
        userID: "",
        orderType:-1,
        finishStatus:0,
        keyWords:'',
        beginDate: '',
        endDate: '',
        pageSize: 20,
        pageIndex:1
    };

    var ObjectJS = {};

    ObjectJS.init = function (isMy, nowDate) {
        Params.beginDate = nowDate;
        Params.endDate = nowDate;

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
                    userID: "-1",
                    isTeam: false,
                    width: "180",
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

        //订单类型搜索
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

        });

        //时间段查询
        $("#btnSearch").click(function () {
            Params.pageIndex = 1;
            Params.beginDate = $("#BeginTime").val();
            Params.endDate = $("#EndTime").val();
            ObjectJS.getList();
        });

    }

    ObjectJS.getList = function () {
        $(".tr-header").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='9'><div class='dataLoading'><img src='/modules/images/ico-loading.jpg'/><div></td></tr>");

        Global.post("/Task/GetTasks", Params, function (data) {
            $(".tr-header").nextAll().remove();

            if (data.Items.length > 0) {
                doT.exec("template/task/task-list.html", function (template) {
                    var innerhtml = template(data.Items);
                    innerhtml = $(innerhtml);

                    $(".tr-header").after(innerhtml);
                });
            }
            else {
                $(".tr-header").after("<tr><td colspan='9'><div class='noDataTxt' >暂无数据!<div></td></tr>");
            }

            $("#pager").paginate({
                total_count: data.TotalCount,
                count: data.PageCount,
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

    module.exports = ObjectJS;
});