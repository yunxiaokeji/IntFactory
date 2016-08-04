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
                    var _save = $('<div class="hand btn-link mLeft10 btn-save-'+id+'" style="display:inline-block;" data-id="' + id + '">保存</div>');
                    var _cancel = $('<div class="hand btn-link mLeft10 btn-cancel-' + id + '" style="display:inline-block;" data-id="' + id + '">取消</div>');
                    var _input = $('<div style="display:inline-block;" class="mLeft10 swen-quantity-' + id + '"><input class="mLeft10 quantity" type="text" style="width:40px;" value="0" /></div>');

                    _cancel.click(function () {
                        $(".btn-save-" + id).remove();
                        $(".btn-cancel-" + id).remove();
                        $(".swen-quantity-" + id).remove();
                    });
                    _save.click(function () {
                        var models = [];
                        $(".swen-quantity-" + $(this).data('id') + " .quantity").each(function () {
                            var _this = $(this);
                            var model = {
                                ProductDetailID: _this.parents('tr').data('id'),
                                Quantity: _this.val()
                            };
                            models.push(model);
                        });
                        $(".btn-save-" + id).remove();
                        $(".btn-cancel-" + id).remove();
                        $(".swen-quantity-" + id).remove();
                    });
                    _input.find('.quantity').change(function () {
                        var _this = $(this);
                        if (!_this.val().isDouble() || _this.val() <= 0) {
                            _this.val(0);
                        }
                    });
                    $(".btn-swen-box-" + id).append(_save).append(_cancel);
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
        DoT.exec("template/orders/sewn-goods.html", function (template) {
            var innerText = template(Common.OrderGoods);

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
                                    Common.getGetGoodsDoc("navSewnDoc", 11);
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
    };

    module.exports = ObjectJS;
});