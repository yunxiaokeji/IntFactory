define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot");
    require("pager");

    var Params = {
        isMy: true,
        finishStatus: -1,
        keyWords:'',
        beginDate: '',
        endDate: '',
        pageSize: 20,
        pageIndex:1
    };

    var ObjectJS = {};

    ObjectJS.init = function (isMy) {
        if (isMy == "0")
        {
            Params.isMy = false;
            document.title = "所有任务";
            $(".header-title").html("所有任务");
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

        //进度状态查询
        require.async("dropdown", function () {
            var Types = [
                {
                    ID: 0,
                    Name: "进行中"
                },
                {
                    ID: 1,
                    Name: "已完成"
                }
            ];

            $("#taskStatus").dropdown({
                prevText: "进度状态-",
                defaultText: "所有",
                defaultValue: "-1",
                data: Types,
                dataValue: "ID",
                dataText: "Name",
                width: "120",
                onChange: function (data) {
                    Params.pageIndex = 1;
                    Params.finishStatus = data.value;
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
        $(".tr-header").after("<tr><td colspan='8'><div class='dataLoading'><img src='/modules/images/ico-loading.jpg'/><div></td></tr>");

        Global.post("/Task/GetTasks", Params, function (data) {
            $(".tr-header").nextAll().remove();

            if (data.Items.length > 0) {
                doT.exec("template/task/task-list.html", function (template) {
                    var innerhtml = template(data.Items);
                    innerhtml = $(innerhtml);

                    $(".tr-header").after(innerhtml);

                    //下拉事件
                    innerhtml.find(".dropdown").click(function () {
                        var _this = $(this);
                        var position = _this.find(".ico-dropdown").position();
                        $(".dropdown-ul li").data("id", _this.data("id"));
                        $(".dropdown-ul").css({ "top": position.top + 20, "left": position.left - 80 }).show().mouseleave(function () {
                            $(this).hide();
                        });
                        return false;
                    });
                });
            }
            else {
                $(".tr-header").after("<tr><td colspan='8'><div class='noDataTxt' >暂无数据!<div></td></tr>");
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