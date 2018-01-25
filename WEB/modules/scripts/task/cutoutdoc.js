define(function (require, exports, module) {
    var Global = null;
    var DoT = null;
    var Easydialog =null;
    var Common = require("scripts/task/ordergoods");
    var ChooseUser = require("chooseuser");
    var Controller = "Task";
    var ObjectJS = {};
    require("tiplayer");
    //裁剪
    ObjectJS.initCutoutDoc = function (orderid, taskid, global, doT, easydialog, taskDesc,task) {
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
        
        var userid = $("#hideCurrentUserID").val();//登录者
        var isTaskOwner = task.Owner.UserID == userid;
        Common.isTaskOwner = isTaskOwner;

        if ($("#btnCutoutOrder").length == 1) {
            //裁剪录入
            $("#btnCutoutOrder").click(function () {
                ObjectJS.cutOutGoods();
            });
            Common.getOrderGoods();

            //裁片退回操作
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
                                    docType: 21,
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

    //裁剪记录
    ObjectJS.getCutoutDoc = function () {
        console.log(Common.isTaskOwner);
        Common.getGetGoodsDoc("navCutoutDoc", 1, ObjectJS.taskDesc, Common.isTaskOwner);
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
                                    Common.getGetGoodsDoc("navCutoutDoc", 1, ObjectJS.taskDesc, Common.isTaskOwner);
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
                    _this.showTipLayer({
                        content: "输入数量大于下单数",
                        zIndex: 9999,
                        isposition: true
                    });
                } else {
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