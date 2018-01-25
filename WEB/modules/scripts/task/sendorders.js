define(function (require, exports, module) {
    var Global = null;
    var DoT = null;
    var Easydialog =null;
    var Common = require("scripts/task/ordergoods");

    var ObjectJS = {};
    var Controller = "Task";
    //大货单发货
    ObjectJS.initSendOrders = function (orderid, taskid, global, doT, easydialog, task) {
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

        var userid = $("#hideCurrentUserID").val();//登录者
        var isTaskOwner = task.Owner.UserID == userid;
        Common.isTaskOwner = isTaskOwner;

        if ($("#btnSendOrder").length == 1) {
            //大货单发货
            $("#btnSendOrder").click(function () {
                ObjectJS.sendGoods();
            });
            //获取订单大货明细
            Common.getOrderGoods();
            //加载快递公司列表
            Common.getExpress();
            //发货退回操作
            $("#btnSaveSwen").click(function () {
                var id = Common.docID;
                if ($(".btn-save-" + id).length <= 0) {
                    var _save = $('<div class="hand btn-link right mLeft10 btn-save-' + id + '" data-id="' + id + '">保存</div>');
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
                                    docType: 22,
                                    details: details,
                                    originalID: id
                                }, function (data) {
                                    _thisBtn.text("保存");
                                    _thisBtn.data('isSubmit', 0);
                                    if (data.result == 1) {
                                        var totalReturnCount = 0;
                                        $(".swen-quantity-" + id).each(function () {
                                            var quantity = ($(this).prev().text() * 1) + ($(this).find('input').val() * 1);
                                            totalReturnCount += quantity * 1;
                                            $(this).prev().text(quantity);
                                        });
                                        var objTotal = $(".input-swen-box-" + id).eq(0).next();
                                        objTotal.text(objTotal.data('quantity') * 1 - totalReturnCount * 1);

                                        $(".btn-save-" + id).remove();
                                        $(".btn-cancel-" + id).remove();
                                        $(".swen-quantity-" + id).remove();

                                        alert("退回成功");
                                    } else if (data.result == 2) {
                                        alert("退回数不能多于" + ObjectJS.taskDesc + "数"), 2;
                                    } else {
                                        alert("网络繁忙，请重试", 2);
                                    }
                                });
                            } else {
                                alert("请输入退回数量", 2);
                            }
                        }
                    });
                    _input.find('.quantity').change(function () {
                        var _this = $(this);
                        if (!_this.val().isInt() || _this.val() * 1 <= 0) {
                            _this.val(0);
                            return false;
                        }
                        var swenTotal = _this.parents('tr').find('.swen-total').text() * 1;
                        var swenQuantity = _this.val() * 1 + _this.parent().prev().text() * 1;
                        if (swenTotal < swenQuantity) {
                            _this.showTipLayer({
                                content: "退回数不能多于" + ObjectJS.taskDesc + "数"
                            });
                            _this.val(0);
                            return false;
                        }
                        return false;
                    });
                    $(".btn-swen-box-" + id).append(_cancel).append(_save);
                    $(".input-swen-box-" + id).append(_input);
                }
                $(this).parent().hide();
            });
        }
    }

    //大货单发货记录
    ObjectJS.getSendDoc = function () {
        Common.getGetGoodsDoc("navSendDoc", 22, "", Common.isTaskOwner);
    }

    //大货单发货
    ObjectJS.sendGoods = function () {
        var _self = this;
        DoT.exec("template/orders/sendordergoods.html", function (template) {
            var innerText = template(Common.OrderGoods);

            Easydialog.open({
                container: {
                    id: "showSendOrderGoods",
                    header: "大货单发货",
                    content: innerText,
                    yesFn: function () {
                        var othersysid = $('#YXOrderID').val();
                        var jsonparas = {};
                        jsonparas.orderid = othersysid;
                        var details = "", bl = true ,guge = $('#GoodsID').val()+":", nums = "";
                        $("#showSendOrderGoods .list-item").each(function () {
                            var _this = $(this);
                            var quantity = _this.find(".quantity").val();
                            if (quantity > 0) {
                                if (quantity > _this.find(".quantity").data("max")) {
                                    bl = false;
                                }
                                details += _this.data("id") + "-" + quantity + ",";
                                nums += quantity + ",";
                                guge += _this.data("remark") + ",";
                            }
                        });
                        jsonparas.remarks = guge.replace("【", "[").replace("】", "]");
                        jsonparas.nums = nums;
                        if (!bl) {
                            alert("数量输入过大", 2);
                            return false;
                        }

                        if (details.length > 0) {
                            if ((!$("#expressid").data("id") || !$("#expressCode").val())
                             && $("#expressid").data('id') != 'fd090820-541a-4a7a-a2c5-00ed5e72f84f') {
                                alert("请完善快递信息!", 2);
                                return false;
                            }

                            Global.post("/" + Controller + "/CreateOrderSendDoc", {
                                orderid: _self.orderid,
                                taskid: _self.taskid,
                                doctype: 2,
                                isover: 0,
                                expressid: $("#expressid").data("id"),
                                expresscode: $("#expressCode").val(),
                                details: details,
                                remark: $("#expressRemark").val().trim(),
                                othersysid: othersysid,
                                jsonparas: JSON.stringify(jsonparas)
                            }, function (data) {
                                if (data.id) {
                                    alert("发货成功!");

                                    if ($("#showSendOrderGoods .check").hasClass("ico-checked")) {
                                        $("#btnSendOrder").remove();
                                    }
                                    Common.getGetGoodsDoc("navSendDoc", 22, "", Common.isTaskOwner);
                                    Common.getOrderGoods();
                                }
                                else if (data.result == "10001") {
                                    alert("您没有操作权限!", 2)
                                }
                                else {
                                    alert("发货失败！", 2);
                                }
                            });
                        } else {
                            alert("请输入数量", 2);
                            return false;
                        }
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

            $("#showSendOrderGoods .check").click(function () {
                var _this = $(this);
                if (!_this.hasClass("ico-checked")) {
                    _this.addClass("ico-checked").removeClass("ico-check");
                }
                else {
                    _this.addClass("ico-check").removeClass("ico-checked");
                }
            });

            $("#showSendOrderGoods").find(".quantity").blur(function () {
                var _this = $(this);
                if (_this.val() > _this.data("max")) {
                    _this.showTipLayer({
                        content: "发货数不能大于完成数",
                        zIndex: 9999,
                        isposition: true
                    });
                    _this.addClass("bRed");
                } else {
                    _this.removeClass("bRed");
                }
            });
            $("#showSendOrderGoods").find(".quantity").keyup(function () {
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