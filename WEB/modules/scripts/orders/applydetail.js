
define(function (require, exports, module) {
    var City = require("city"), CityObj,
        Global = require("global"),
        ChooseUser = require("chooseuser");

    var ObjectJS = {};
    //添加页初始化
    ObjectJS.init = function (orderid, status, model) {
        var _self=this;
        _self.orderid = orderid;
        _self.status = status;
        _self.model = JSON.parse(model.replace(/&quot;/g, '"'));
        _self.bindEvent();
        _self.getAmount();
    }
    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;

        //编辑数量
        $(".quantity").change(function () {
            if ($(this).val().isInt() && $(this).val() > 0) {
                _self.editQuantity($(this));
            } else {
                $(this).val($(this).data("value"));
            }
        });

        //提交订单
        $("#btnconfirm").click(function () {
            confirm("退货申请提交后不能撤销，确认提交吗？", function () {
                _self.submitOrder();
            });
            
        });        
    }
    //更改数量
    ObjectJS.editQuantity = function (ele) {
        var _self = this;
        var quantity = ele.val();
        if (quantity > ele.data("max")) {
            quantity = ele.data("max");
            ele.val(ele.data("max"));
        }

        Global.post("/Orders/UpdateReturnQuantity", {
            orderid: _self.orderid,
            autoid: ele.data("id"),
            name: ele.data("name"),
            quantity: quantity
        }, function (data) {
            if (!data.status) {
                ele.val(ele.data("value"));
                alert("订单状态已变更，请刷新页面后重试！");
            } else {
                ele.data("value", quantity);
            }
        });
    }

    //保存
    ObjectJS.submitOrder = function () {
        var _self = this;
        var totalamount = 0, bl = false;
        //单据明细
        $("input.quantity").each(function () {
            if ($(this).val() > 0) {
                bl = true;
            }
        });
        if (!bl) {
            alert("退货数量不能全部为0！")
            return;
        }
        Global.post("/Orders/ApplyReturnProduct", { orderid: _self.orderid }, function (data) {
            if (data.status) {
                location.href = "/Orders/Detail/" + _self.orderid;
            } else {
                location.href = location.href;
            }
        })
    }
    module.exports = ObjectJS;
})