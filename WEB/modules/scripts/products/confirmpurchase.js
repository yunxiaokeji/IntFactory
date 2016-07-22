
define(function (require, exports, module) {
    var Global = require("global"),
        Easydialog = require("easydialog"),
        ChooseProduct = require("chooseproduct");
    require("dropdown");
    var ObjectJS = {};
    //添加页初始化
    ObjectJS.init = function (guid, wares) {
        var _self = this;
        _self.guid = guid;
        _self.wares = JSON.parse(wares.replace(/&quot;/g, '"'));
        _self.wareid = _self.wares[0].WareID;
        _self.bindEvent(guid);
        _self.getAmount();
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

        //添加产品
        $("#btnChooseProduct").click(function () {
            ChooseProduct.create({
                title: "选择采购材料",
                type: 1, //1采购 2出库 3报损 4报溢 5调拨
                wareid: guid,
                callback: function (products) {
                    if (products.length > 0) {
                        var entity = {}, items = [];
                        entity.guid = guid;
                        entity.type = 1;
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

                        Global.post("/ShoppingCart/AddShoppingCartBatchIn", { entity: JSON.stringify(entity) }, function (data) {
                            if (data.status) {
                                location.href = location.href;
                            }
                        });
                    }
                }
            });
        });
        //编辑批次
        $(".batch").change(function () {
            var _this = $(this);
            Global.post("/ShoppingCart/UpdateCartBatch", {
                autoid: _this.data("id"),
                batch: _this.val().trim()
            }, function (data) {
                if (!data.status) {
                    _this.val(_this.data("value"));
                    alert("系统异常，请重新操作！");
                } else {
                    _this.data("value", _this.val());
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
        //编辑单价
        $(".price").change(function () {
            var _this = $(this);
            if (_this.val().isDouble() && _this.val() > 0) {

                Global.post("/ShoppingCart/UpdateCartPrice", {
                    autoid: _this.data("id"),
                    price: _this.val()
                }, function (data) {
                    if (!data.Status) {
                        _this.val(_this.data("value"));
                        alert("价格编辑失败，请刷新页面后重试！");
                    } else {
                        _this.parent().nextAll(".amount").html((_this.parent().nextAll(".tr-quantity").find("input").val() * _this.val()).toFixed(2));
                        _this.data("value", _this.val());
                        _self.getAmount();
                    }
                });

               
            } else {
                _this.val(_this.data("value"));
            }
        });
        //删除产品
        $(".ico-del").click(function () {
            var _this = $(this);
            confirm("确认从采购清单中移除此材料吗？", function () {
                Global.post("/ShoppingCart/DeleteCart", {
                    autoid: _this.data("id")
                }, function (data) {
                    if (!data.Status) {
                        alert("系统异常，请重新操作！");
                    } else {
                        _this.parents("tr.item").remove();
                        _self.getAmount();
                    }
                });
            });
        });

        $("#btnconfirm").click(function () {
            var bl = false;

            $(".cart-item").each(function () {
                bl = true;
            });
            if (!bl) {
                alert("请选择采购材料！");
                return;
            }
            confirm("采购单提交后不可编辑，确认提交吗？", function () {
                _self.submitOrder();
            });
        });

    }
    //计算总金额
    ObjectJS.getAmount = function () {
        var amount = 0;
        $(".amount").each(function () {
            var _this = $(this);
            _this.html((_this.prevAll(".tr-quantity").find("input").val() * _this.prevAll(".tr-price").find("input").val()).toFixed(2));
            amount += _this.html() * 1;
        });
        $("#amount").text(amount.toFixed(2));
    }
    //更改数量
    ObjectJS.editQuantity = function (ele) {
        var _self = this;
        Global.post("/ShoppingCart/UpdateCartQuantity", {
            autoid: ele.data("id"),
            quantity: ele.val(),
            wareid: _self.guid
        }, function (data) {
            if (!data.Status) {
                ele.val(ele.data("value"));
                alert("系统异常，请重新操作！");
            } else {
                ele.parent().nextAll(".amount").html((ele.parent().prevAll(".tr-price").find("input").val() * ele.val()).toFixed(2));
                ele.data("value", ele.val());
                _self.getAmount();
            }
        });
    }

    //保存
    ObjectJS.submitOrder = function () {
        var _self = this, bl = false;
        var totalamount = 0, list = [];

        $(".cart-item").each(function () {
            bl = true;
        });
        if (!bl) {
            alert("请选择采购材料！");
            return;
        }
        Global.post("/Purchase/SubmitPurchase", {
            wareid: _self.guid,
            remark: $("#remark").val().trim()
        }, function (data) {
            if (data.status) {
                location.href = "/Products/Purchases";
            } else {
                location.href = location.href;
            }
        });
    }

    module.exports = ObjectJS;
})