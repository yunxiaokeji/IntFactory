define(function (require, exports, module) {
    var Global = null;
    var DoT = null;

    var ObjectJS = {};

    ObjectJS.init = function (global, doT) {
        if (Global == null) {
            Global = require("global");
        }
        else {
            Global = global;
        }
        if (doT == null) {
            Global = require("dot");
        }
        else {
            DoT = doT;
        }
    };

    //获取订单明细
    ObjectJS.getGetGoodsDoc = function (id, type, taskDesc) {
        var _self = this;
        var $tr_header = $("#" + id + " .tr-header");
        $tr_header.nextAll().remove();
        $tr_header.after("<tr><td colspan='10'><div class='data-loading' ><div></td></tr>");
        Global.post("/Orders/GetGoodsDocByOrderID", {
            orderid: _self.orderid,
            taskid: _self.taskid,
            type: type==22?2:type
        }, function (data) {
            $tr_header.nextAll().remove();
            if (data.items.length > 0) {
                $tr_header.hide();
                var templateHtml = "template/orders/cutoutdoc.html";
                if (type == 2) {
                    $tr_header.show();
                    templateHtml = "template/orders/senddydocs.html"
                }
                else if (type == 22) {
                    templateHtml = "template/orders/senddocs.html";
                }

                DoT.exec(templateHtml, function (template) {
                    data.items.taskDesc = taskDesc;
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);

                    /*车缝退回操作*/
                    if ($("#btnSewnOrder").length == 1 && type == 11) {
                        innerhtml.find(".ico-dropdown").click(function () {
                            var _this = $(this);
                            ObjectJS.docID = _this.data('id');
                            var position = _this.position();
                            $("#setReturnSewn li").data("columnname", _this.data("columnname"));
                            $("#setReturnSewn").css({ "top": position.top + 20, "left": position.left - 70 }).show().mouseleave(function () {
                                $(this).hide();
                            });
                            return false;
                        });
                    } else {
                        /*无权限删除车缝退回操作*/
                        innerhtml.find(".ico-dropdown").remove();
                    }
                    $tr_header.after(innerhtml);

                    var total = 0;
                    innerhtml.find('.cut1').each(function () {
                        var _this = $(this);
                        total += parseInt(_this.text());
                    });
                    innerhtml.find('.total-count').html(total);
                });
            }
            else {
                $tr_header.show();
                $tr_header.after("<tr><td colspan='10'><div class='nodata-txt' >暂无数据!<div></td></tr>");
            }
        });
    };

    ObjectJS.getOrderGoods = function () {
        $("#navGoods .table-header").nextAll().remove();
        $("#navGoods .table-header").after($("<tr><td colspan='8'><div class='data-loading'></div></td></tr>"));
        Global.post("/Task/GetOrderGoods", { id: ObjectJS.orderid }, function (data) {
            ObjectJS.OrderGoods = data.list;
            $("#navGoods .table-header").nextAll().remove();
            if (data.list.length > 0) {
                DoT.exec("template/task/task-ordergoods.html", function (template) {
                    var innerHtml = template(data.list);
                    innerHtml = $(innerHtml);
                    $("#navGoods .table-header").after(innerHtml);
                    ObjectJS.getAmount();
                });
            } else {
                $("#navGoods .table-header").after($("<tr><td colspan='8'><div class='nodata-txt'>暂无明细</div></td></tr>"));
            }
        });
    }

    //汇总
    ObjectJS.getAmount = function () {
        //订单明细汇总
        $(".total-item td").each(function () {
            var _this = $(this), _total = 0;
            if (_this.data("class")) {
                $("." + _this.data("class")).each(function () {
                    _total += $(this).html() * 1;
                });
                if (_this.data("class") == "moneytotal") {
                    _this.html(_total.toFixed(2));
                } else {
                    _this.html(_total);
                }
            }
        });
    }

    //加载快递公司列表
    ObjectJS.getExpress = function () {
        Global.post("/Plug/GetExpress", {}, function (data) {
            ObjectJS.express = data.items;
        });
    }

    module.exports = ObjectJS;
});