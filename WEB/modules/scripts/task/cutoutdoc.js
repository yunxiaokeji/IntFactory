define(function (require, exports, module) {
    var Global = null;
    var DoT = null;
    var Easydialog =null;
    var Common = require("scripts/task/ordergoods");

    var Controller = "Task";
    var ObjectJS = {};
    //裁剪
    ObjectJS.initCutoutDoc = function (orderid, taskid, global, doT, easydialog) {
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

        if ($("#btnCutoutOrder").length == 1) {
            //裁剪录入
            $("#btnCutoutOrder").click(function () {
                ObjectJS.cutOutGoods();
            });
            //获取订单大货明细
            Common.getOrderGoods();
        }
    }

    //裁剪记录
    ObjectJS.getCutoutDoc = function () {
        Common.getGetGoodsDoc("navCutoutDoc", 1);
    }

    //裁剪录入
    ObjectJS.cutOutGoods = function () {
        var _self = this;
        DoT.exec("template/orders/cutoutgoods.html", function (template) {
            var innerText = template(Common.OrderGoods);
            Easydialog.open({
                container: {
                    id: "showCutoutGoods",
                    header: "大货单裁剪登记",
                    content: innerText,
                    yesFn: function () {
                        var details = ""
                        $("#showCutoutGoods .list-item").each(function () {
                            var _this = $(this);
                            var quantity = _this.find(".quantity").val();
                            if (quantity > 0) {
                                details += _this.data("id") + "-" + quantity + ",";
                            }
                        });

                        if (details.length > 0 || $("#showCutoutGoods .check").hasClass("ico-checked")) {
                            Global.post("/" + Controller + "/CreateOrderCutOutDoc", {
                                orderid: _self.orderid,
                                taskid: _self.taskid,
                                doctype: 1,
                                isover: $("#showCutoutGoods .check").hasClass("ico-checked") ? 1 : 0,
                                expressid: "",
                                expresscode: "",
                                details: details,
                                remark: $("#expressRemark").val().trim()
                            },
                            function (data) {
                                if (data.id) {
                                    alert("裁剪登记成功!");
                                    
                                    if ($("#showCutoutGoods .check").hasClass("ico-checked")) {
                                        $("#btnCutoutOrder").remove();
                                    }
                                    Common.getGetGoodsDoc("navCutoutDoc", 1);
                                    Common.getOrderGoods();
                                }
                                else if (data.result == "10001") {
                                    alert("您没有操作权限!")
                                }
                                else {
                                    alert("裁剪登记失败！");
                                }
                            });
                        }
                        else {
                            alert("请输入裁剪数量");
                            return false;
                        }
                    }
                }
            });

            $("#showCutoutGoods .check").click(function () {
                var _this = $(this);
                if (!_this.hasClass("ico-checked")) {
                    _this.addClass("ico-checked").removeClass("ico-check");
                }
                else {
                    _this.addClass("ico-check").removeClass("ico-checked");
                }
            });

            $("#showCutoutGoods").find(".quantity").blur(function () {
                var _this = $(this);
                if (!_this.val()) {
                    _this.val("0");
                }
            });

            $("#showCutoutGoods").find(".quantity").keyup(function () {
                var _this = $(this);
                if (!_this.val()) {
                    return;
                }
                if (!_this.val().isInt() || _this.val() <= 0) {
                    _this.val("0");
                }
                else if (_this.val() > _this.data("max")) {
                    confirm("输入数量大于下单数，是否继续？", function () { }, function () {
                        _this.val(_this.data("max"));
                    });
                }
            });

        });
    };

    module.exports = ObjectJS;
});