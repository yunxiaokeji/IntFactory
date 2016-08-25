define(function (require, exports, module) {
    var Global = null;
    var DoT = null;
    var Easydialog =null;
    var Common = require("scripts/task/ordergoods");
    var ChooseUser = require("chooseuser");
    var Controller = "Task";
    var ObjectJS = {};
    //裁剪
    ObjectJS.initCutoutDoc = function (orderid, taskid, global, doT, easydialog, taskDesc) {
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
        ObjectJS.taskDesc = taskDesc;
        Common.orderid = orderid;
        Common.taskid = taskid;
        Common.init(Global, DoT);

        if ($("#btnCutoutOrder").length == 1) {
            //裁剪录入
            $("#btnCutoutOrder").click(function () {
                ObjectJS.cutOutGoods();
            });
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
            Common.OrderGoods.taskDesc = ObjectJS.taskDesc;
            var innerText = template(Common.OrderGoods);
            Easydialog.open({
                container: {
                    id: "showCutoutGoods",
                    header: "大货单" + ObjectJS.taskDesc + "登记",
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
                                remark: $("#expressRemark").val().trim(),
                                ownerid: $("#showCutoutGoods .choose-owner").data('id')
                            },
                            function (data) {
                                if (data.id) {
                                    alert("" + ObjectJS.taskDesc + "登记成功!");
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
                                    alert("" + ObjectJS.taskDesc + "登记失败！", 2);
                                }
                            });
                        }
                        else {
                            alert("请输入" + ObjectJS.taskDesc + "数量", 2);
                            return false;
                        }
                    }
                }
            });
           
            //默认负责人选择当前登录用户
            $("#showCutoutGoods .owner-name").text($("#currentUser .username").text());

            $("#showCutoutGoods .choose-owner").click(function () {
                var _this = $(this);
                ChooseUser.create({
                    title: "更换负责人",
                    type: 1,
                    single: true,
                    callback: function (items) {
                        if (items.length > 0) {
                            if (_this.data("id") != items[0].id) {
                                _this.data("id", items[0].id);
                                _this.prev().text(items[0].name);
                            }
                            else {
                                alert("请选择不同人员进行更换!", 2);
                            }
                        }
                    }
                });
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

            $("#showCutoutGoods").find(".quantity").change(function () {
                var _this = $(this);
                if (_this.val() > _this.data("max")) {
                    _this.addClass("bRed");
                    confirm("输入数量大于下单数，是否继续？", function () { }, function () {
                        _this.val(_this.data("max"));
                    });
                } else {
                    _this.removeClass("bRed");
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
            });

        });
    };

    module.exports = ObjectJS;
});