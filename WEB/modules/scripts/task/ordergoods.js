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
    ObjectJS.getGetGoodsDoc = function (id, type) {
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
                var templateHtml = "template/orders/cutoutdoc.html";
                if (type == 2) {
                    templateHtml = "template/orders/senddydocs.html"
                }
                else if (type == 22) {
                    templateHtml = "template/orders/senddocs.html";
                }

                DoT.exec(templateHtml, function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);
                    
                    if (type != 2) {
                        innerhtml.click(function () {
                            _self.getGoodsDocDetail(this, type == 22 ? 2 : 1);
                        });
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
                $tr_header.after("<tr><td colspan='10'><div class='nodata-txt' >暂无数据!<div></td></tr>");
            }
            /*有数据隐藏表头*/
            if (!$(".table-items-detail").find('div').hasClass('nodata-txt')) {
                $(".table-header").hide();
            } else {
                $(".table-header").show();
            }
        });
    };

    //获取单据明细
    ObjectJS.getGoodsDocDetail = function (item, type) {
        var _this = $(item), url = "";
        if (type == 1) {
            url = "template/orders/cutout-details.html";
        }
        else if (type == 2) {
            url = "template/orders/send-details.html";
        }

        if (!_this.data("first") || _this.data("first") == 0) {
            _this.data("first", 1).data("status", "open");

            Global.post("/Orders/GetGoodsDocDetail", {
                docid: _this.data("id")
            }, function (data) {
                DoT.exec(url, function (template) {
                    var innerhtml = template(data.model.Details);
                    innerhtml = $(innerhtml);
                    _this.after(innerhtml);
                });
            });
        }
        else {
            if (_this.data("status") == "open") {
                _this.data("status", "close");
                _this.nextAll("tr[data-pid='" + _this.data("id") + "']").hide();
            } else {
                _this.data("status", "open");
                _this.nextAll("tr[data-pid='" + _this.data("id") + "']").show();
            }
        }
    };

    //获取订单明细
    ObjectJS.getOrderGoods = function () {
        Global.post("/Task/GetOrderGoods", { id: ObjectJS.orderid }, function (data) {
            ObjectJS.OrderGoods = data.list;
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