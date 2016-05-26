define(function (require, exports, module) {
    var Global = null;
    var DoT = null;
    var Easydialog = null;
    var Common = require("scripts/task/ordergoods");

    var ObjectJS = {};
    var Controller = "Task";
    //车缝
    ObjectJS.initSewnDoc = function (orderid, taskid, global, doT, easydialog) {
        if (global == null) {
            Global = require("global");
        }
        else {
            Global = global;
        }
        if (doT == null) {
            DoT = require("dot");
        }
        else {
            DoT = doT;
        }
        if (easydialog == null) {
            Easydialog = require("easydialog");
        }
        else {
            Easydialog = easydialog;
        }
        ObjectJS.orderid = orderid;
        ObjectJS.taskid = taskid;
        Common.orderid =orderid;
        Common.taskid = taskid;
        Common.init(Global, DoT);

        if ($("#btnSewnOrder").length == 1) {
            //车缝录入
            $("#btnSewnOrder").click(function () {
                ObjectJS.sewnGoods();
            });
            //获取订单大货明细
            Common.getOrderGoods();
        }
    }

    //车缝记录
    ObjectJS.getSewnDoc = function () {
        Common.getGetGoodsDoc("navSewnDoc", 11);
    }

    //车缝录入
    ObjectJS.sewnGoods = function () {
        var _self = this;
        DoT.exec("template/orders/sewn-goods.html", function (template) {
            var innerText = template(Common.OrderGoods);

            Easydialog.open({
                container: {
                    id: "showSewnGoods",
                    header: "大货单车缝登记",
                    content: innerText,
                    yesFn: function () {
                        var details = ""
                        $("#showSewnGoods .list-item").each(function () {
                            var _this = $(this);
                            var quantity = _this.find(".quantity").val();
                            if (quantity > 0) {
                                details += _this.data("id") + "-" + quantity + ",";
                            }
                        });

                        if (details.length > 0) {
                            Global.post("/" + Controller + "/CreateOrderSewnDoc", {
                                orderid: _self.orderid,
                                taskid: _self.taskid,
                                doctype: 11,
                                isover: $("#showSewnGoods .check").hasClass("ico-checked") ? 1 : 0,
                                expressid: "",
                                expresscode: "",
                                details: details,
                                remark: $("#expressRemark").val().trim()
                            }, function (data) {
                                if (data.id) {
                                    alert("车缝登记成功!");
                                    
                                    if ($("#showSewnGoods .check").hasClass("ico-checked")) {
                                        $("#btnSewnOrder").remove();
                                    }
                                    Common.getGetGoodsDoc("navSewnDoc", 11);
                                    Common.getOrderGoods();
                                }
                                else if (data.result == "10001") {
                                    alert("您没有操作权限!")
                                }
                                else {
                                    alert("车缝登记失败！");
                                }
                            });
                        }
                        else {
                            alert("请输入车缝数量");
                            return false;
                        }
                    },
                    callback: function () {

                    }
                }
            });

            $("#showSewnGoods .check").click(function () {
                var _this = $(this);
                if (!_this.hasClass("ico-checked")) {
                    _this.addClass("ico-checked").removeClass("ico-check");
                }
                else {
                    _this.addClass("ico-check").removeClass("ico-checked");
                }
            });

            $("#showSewnGoods").find(".quantity").blur(function () {
                var _this = $(this);
                if (!_this.val()) {
                    _this.val("0");
                }
            });
            $("#showSewnGoods").find(".quantity").keyup(function () {
                var _this = $(this);
                if (!_this.val()) {
                    return;
                }
                if (!_this.val().isInt() || _this.val() <= 0) {
                    _this.val("0");
                }
                else if (_this.val() > _this.data("max")) {
                    _this.val(_this.data("max"));
                    alert("输入车缝数量过大");
                }
            });

        });
    };

    module.exports = ObjectJS;
});