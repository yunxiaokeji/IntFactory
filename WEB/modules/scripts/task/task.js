define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot");
    require("pager");

    var Params = {
        isMy: true,
        beginDate: '',
        endDate: '',
        pageSize: 20,
        pageIndex:1
    };

    var ObjectJS = {};

    ObjectJS.init = function () {
        ObjectJS.bindEvent();

        ObjectJS.getList();
    }

    ObjectJS.bindEvent =function() {}

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