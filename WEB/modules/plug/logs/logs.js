
/* 
作者：Allen
日期：2016-7-1
示例:
    $(...).getObjectLogs(options);
*/

define(function (require, exports, module) {
    require("plug/logs/style.css");
    var Global = require("global"),
        doT = require("dot");
    require("pager");

    (function ($) {
        $.fn.getObjectLogs = function (options) {
            var opts = $.extend({}, $.fn.getObjectLogs.defaults, options);
            return this.each(function () {
                var _this = $(this);
                $.fn.drawObjectLogs(_this, opts);
            })
        }

        $.fn.getObjectLogs.defaults = {
            guid: "",
            type: 1, /*1 客户 2订单 10任务 */
            subject: -1,
            pageSize: 10
        };

        $.fn.drawObjectLogs = function (obj, opts) {
            obj.empty();
            doT.exec("plug/logs/logs.html", function (template) {
                var innerhtml = template(opts);
                innerhtml = $(innerhtml);
                obj.append(innerhtml);
                opts.pageIndex = 1;
                $.fn.drawLogsItems(obj, opts);


                if (opts.type != 2 && opts.type != 10) {
                    $(".select-log-subject").hide();
                } else {
                    $(".select-log-subject .item").click(function () {
                        var _this = $(this);
                        if (!_this.hasClass("hover")) {
                            _this.siblings().removeClass("hover");
                            _this.addClass("hover");
                            opts.pageIndex = 1;
                            opts.subject = _this.data("id");
                            $.fn.drawLogsItems(obj, opts);
                        }
                    });
                }
            });
        }

        $.fn.drawLogsItems = function (obj, opts) {
            obj.find(".log-body").empty();
            obj.find(".log-body").append("<div class='data-loading'><div>");
            Global.post("/Plug/GetObjectLogs", opts, function (data) {
                obj.find(".log-body").empty();
                if (data.items.length > 0) {
                    doT.exec("plug/logs/logitem.html", function (template) {
                        var innerhtml = template(data.items);
                        innerhtml = $(innerhtml);
                        obj.find(".log-body").append(innerhtml);
                    });
                } else {
                    obj.find(".log-body").append("<div class='nodata-txt'>暂无日志<div>");
                }
                obj.find(".log-pager").paginate({
                    total_count: data.totalCount,
                    count: data.pageCount,
                    start: opts.pageIndex,
                    display: 5,
                    border: true,
                    border_color: '#fff',
                    text_color: '#333',
                    background_color: '#fff',
                    border_hover_color: '#ccc',
                    text_hover_color: '#000',
                    background_hover_color: '#efefef',
                    rotate: true,
                    images: false,
                    mouse: 'slide',
                    float: "left",
                    onChange: function (page) {
                        opts.pageIndex = page;
                        $.fn.drawLogsItems(obj, opts);
                    }
                });
            });
        }
    })(jQuery)
    module.exports = jQuery;
});