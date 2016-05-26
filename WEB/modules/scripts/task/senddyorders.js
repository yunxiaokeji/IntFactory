define(function (require, exports, module) {
    var Global = null;
    var DoT = null;
    var Easydialog = null;
    var Common = require("scripts/task/ordergoods");

    var ObjectJS = {};
    var Controller = "Task";
    //打样单发货
    ObjectJS.initSendDYOrders = function (orderid, taskid, global, doT, easydialog) {
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
        if (Easydialog == null) {
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

        if ($("#btnSendDYOrder").length == 1) {
            //打样单发货
            $("#btnSendDYOrder").click(function () {
                ObjectJS.sendDYGoods();
            });
            //获取订单大货明细
            Common.getOrderGoods();
            //加载快递公司列表
            Common.getExpress();
        }
    }

    //打样单发货记录
    ObjectJS.getSendDYDoc = function () {
        Common.getGetGoodsDoc("navSendDYDoc", 2);
    }

    //打样单发货
    ObjectJS.sendDYGoods = function () {
        var _self = this;
        DoT.exec("template/orders/send_orders.html", function (template) {
            var innerText = template(Common.OrderGoods);

            Easydialog.open({
                container: {
                    id: "showSendOrderGoods",
                    header: "打样单发货",
                    content: innerText,
                    yesFn: function () {
                        if (!$("#expressid").data("id") || !$("#expressCode").val()) {
                            alert("请完善快递信息!");
                            return false;
                        }

                        Global.post("/" + Controller + "/CreateOrderSendDoc", {
                            orderid: _self.orderid,
                            taskid: _self.taskid,
                            doctype: 2,
                            isover: 0,
                            expressid: $("#expressid").data("id"),
                            expresscode: $("#expressCode").val(),
                            details: "",
                            remark: $("#expressRemark").val().trim()
                        }, function (data) {
                            if (data.id) {
                                alert("发货成功!");

                                Common.getGetGoodsDoc("navSendDYDoc", 2);
                            }
                            else if (data.result == "10001") {
                                alert("您没有操作权限!")
                            }
                            else {
                                alert("发货失败！");
                            }
                        });
                    }
                }
            });

            //快递公司
            require.async("dropdown", function () {
                var dropdown = $("#expressid").dropdown({
                    prevText: "",
                    defaultText: "请选择",
                    defaultValue: "",
                    data: Common.express,
                    dataValue: "ExpressID",
                    dataText: "Name",
                    width: "180",
                    isposition: true,
                    onChange: function (data) {

                    }
                });
            });

        });
    };

    module.exports = ObjectJS;
});