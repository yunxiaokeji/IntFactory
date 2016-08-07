define(function (require, exports, module) {
    var Global = null;
    var DoT = null;
    var Easydialog = null;
    var Common = require("scripts/task/ordergoods");

    var ObjectJS = {};
    var Controller = "Task";
    //车缝
    ObjectJS.initSewnDoc = function (orderid, taskid, global, doT, easydialog,taskDesc) {
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

            //车缝退回操作
            $("#btnSaveSwen").click(function () {
                var id = Common.docID;
                if ($(".btn-save-" + id).length <= 0) {
                    var _save = $('<div class="hand btn-link right mLeft10 btn-save-'+id+'" data-id="' + id + '">保存</div>');
                    var _cancel = $('<div class="hand btn-link right mLeft10 btn-cancel-' + id + '" data-id="' + id + '">取消</div>');
                    var _input = $('<div class="right mLeft10 swen-quantity-' + id + '"><input class="mLeft10 quantity" type="text" style="width:40px;" value="0" /></div>');

                    _cancel.click(function () {
                        $(".btn-save-" + id).remove();
                        $(".btn-cancel-" + id).remove();
                        $(".swen-quantity-" + id).remove();
                    });

                    _save.click(function () {
                        var _thisBtn = $(this);
                        if (_thisBtn.data('isSubmit') != 1) {
                            var details = "";
                            $(".swen-quantity-" + $(this).data('id') + " .quantity").each(function () {
                                var _this = $(this);
                                if (_this.val() > 0) {
                                    details += _this.parents('tr').data('id') + "|" + _this.val() + ",";
                                }
                            });
                            if (details.length > 0) {
                                _thisBtn.text("保存中...");
                                _thisBtn.data('isSubmit', 1);
                                Global.post("/Task/CreateGoodsDocReturn", {
                                    orderID: orderid,
                                    taskID: taskid,
                                    docType: 6,
                                    details: details,
                                    originalID: id
                                }, function (data) {
                                    _thisBtn.text("保存");
                                    _thisBtn.data('isSubmit', 0);
                                    if (data.result == 1) {
                                        alert("" + ObjectJS.taskDesc + "退回成功");
                                        $(".swen-quantity-" + id).each(function () {
                                            var quantity = ($(this).prev().text() * 1) + ($(this).find('input').val() * 1);
                                            $(this).prev().text(quantity.toFixed(2));
                                        });
                                        $(".btn-save-" + id).remove();
                                        $(".btn-cancel-" + id).remove();
                                        $(".swen-quantity-" + id).remove();
                                    } else if (data.result==2) {
                                        alert("退回数不能多于" + ObjectJS.taskDesc + "数");
                                    } else {
                                        alert("网络繁忙，请重试");
                                    }
                                });
                            } else {
                                alert("请输入退回数量");
                            }
                        }
                    });
                    _input.find('.quantity').change(function () {
                        var _this = $(this);
                        if (!_this.val().isDouble() || _this.val() * 1 <= 0) {
                            _this.val(0);
                            return false;
                        }
                        var swenTotal = _this.parents('tr').find('.swen-total').text() * 1;
                        var swenQuantity = _this.val() * 1 + _this.parent().prev().text() * 1;
                        if (swenTotal < swenQuantity) {
                            alert("退回数不能多于" + ObjectJS.taskDesc + "数");
                            _this.val(0);
                            return false;
                        }
                        return false;
                    });
                    $(".btn-swen-box-" + id).append(_cancel).append(_save);
                    $(".input-swen-box-" + id).append(_input);
                }
            });
        }
    }

    //车缝记录
    ObjectJS.getSewnDoc = function () {
        Common.getGetGoodsDoc("navSewnDoc", 11, ObjectJS.taskDesc);
    }

    //车缝录入
    ObjectJS.sewnGoods = function () {
        var _self = this;
        Global.post("/Task/GetOrderGoods", { id: ObjectJS.orderid }, function (data) {
            DoT.exec("template/orders/sewn-goods.html", function (template) {
                var innerText = template(data.list);
                Easydialog.open({
                    container: {
                        id: "showSewnGoods",
                        header: "大货单" + ObjectJS.taskDesc + "登记",
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
                                        alert("" + ObjectJS.taskDesc + "登记成功!");

                                        if ($("#showSewnGoods .check").hasClass("ico-checked")) {
                                            $("#btnSewnOrder").remove();
                                        }
                                        Common.getGetGoodsDoc("navSewnDoc", 11, ObjectJS.taskDesc);
                                        Common.getOrderGoods();
                                    }
                                    else if (data.result == "10001") {
                                        alert("您没有操作权限!")
                                    }
                                    else {
                                        alert("" + ObjectJS.taskDesc + "登记失败！");
                                    }
                                });
                            }
                            else {
                                alert("请输入" + ObjectJS.taskDesc + "数量");
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
        });
    };

    module.exports = ObjectJS;
});