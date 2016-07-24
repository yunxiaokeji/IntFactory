
define(function (require, exports, module) {
    var Global = require("global"),
        Easydialog = require("easydialog"),
        ChooseProduct = require("chooseproduct"),
        doT = require("dot");
    require("dropdown");
    var ObjectJS = {};
    //初始化
    ObjectJS.init = function (guid,wares) {
        var _self = this;
        _self.guid = guid;
        _self.wares = JSON.parse(wares.replace(/&quot;/g, '"'));
        _self.wareid = _self.wares[0].WareID;
        _self.bindEvent(guid);
    }
    //绑定事件
    ObjectJS.bindEvent = function (guid) {
        var _self = this;

        $("#chooseWare").dropdown({
            prevText: "全部-",//文本前缀
            defaultText: _self.wares[0].Name,
            defaultValue: _self.wares[0].WareID,
            data: _self.wares,
            dataValue: "WareID",
            dataText: "Name",
            width: "180",
            onChange: function (data) {
                _self.wareid = data.value;
            }
        });

        $("#btnChooseProduct").click(function () {
            ChooseProduct.create({
                title: "选择报损材料",
                type: 3, //1采购 2出库 3报损 4报溢 5调拨
                wareid: _self.wareid,
                callback: function (products) {
                    if (products.length > 0) {
                        var entity = {}, items = [];
                        entity.guid = guid;
                        entity.type = 3;
                        for (var i = 0; i < products.length; i++) {
                            items.push({
                                ProductID: products[i].pid,
                                ProductDetailID: products[i].did,
                                BatchCode: products[i].batch,
                                DepotID: products[i].depotid,
                                Description: products[i].remark,
                            });
                        }
                        entity.Products = items;

                        Global.post("/ShoppingCart/AddShoppingCartBatchOut", { entity: JSON.stringify(entity) }, function (data) {
                            if (data.status) {
                                location.href = location.href;
                            }
                        });
                    }
                }   
            });
        });

        //编辑数量
        $(".quantity").change(function () {
            if ($(this).val().isDouble() && $(this).val() > 0) {
                _self.editQuantity($(this));
            } else {
                $(this).val($(this).data("value"));
            }
        });
        //删除
        $(".ico-del").click(function () {
            var _this = $(this);
            confirm("确认删除此产品吗？", function () {
                Global.post("/ShoppingCart/DeleteCart", {
                    autoid: _this.data("id")
                }, function (data) {
                    if (!data.Status) {
                        alert("系统异常，请重新操作！");
                    } else {
                        _this.parents("tr.item").remove();
                    }
                });
            });
        });

        //提交订单
        $("#btnconfirm").click(function () {
            if ($(".cart-item").length == 0) {
                alert("请选择报损材料！");
                return;
            }
            confirm("报损单提交后不可编辑，确认提交吗？", function () {
                _self.submitOrder();
            });

        });
    }
    //更改数量
    ObjectJS.editQuantity = function (ele) {
        var _self = this;
        Global.post("/ShoppingCart/UpdateCartQuantity", {
            autoid: ele.data("id"),
            quantity: ele.val().trim(),
            guid: _self.guid
        }, function (data) {
            if (!data.Status) {
                ele.val(ele.data("value"));
                alert("系统异常，请重新操作！");
            } else {
                ele.data("value", ele.val());
            }
        });
    }
    //保存
    ObjectJS.submitOrder = function () {
        var _self = this;
        //单据明细
        if ($(".cart-item").length == 0) {
            alert("请选择报损材料！");
            return;
        }
        Global.post("/Stock/SubmitDamagedDoc", {
            wareid: _self.wareid,
            remark: $("#remark").val().trim()
        }, function (data) {
            if (data.status) {
                location.href = "/Stock/Damaged";
            } else {
                location.href = location.href;
            }
        })
    }

    module.exports = ObjectJS;
})