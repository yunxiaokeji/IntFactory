
/* 
作者：Allen
日期：2015-9-13
示例:
    $(...).createCart(callback);
*/

define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot");
    require("plug/shoppingcart/style.css");
    (function ($) {
        $.fn.createCart = function (options) {
            var opts = $.extend({}, $.fn.createCart.defaults, options);
            return this.each(function () {
                var _this = $(this);
                $.fn.drawCart(_this, opts);
            })
        }
        $.fn.createCart.defaults = {
            ordertype: 0,
            guid: "",
            tid: ""
        };
        $.fn.drawCart = function (obj, opts) {
            Global.post("/ShoppingCart/GetShoppingCartCount", {
                ordertype: opts.ordertype,
                guid: opts.guid
            }, function (data) {
                doT.exec("plug/shoppingcart/shoppingcart.html", function (templateFun) {
                    var innerText = templateFun([]);
                    innerText = $(innerText);

                    //产品数量
                    innerText.find(".totalcount").html(data.Quantity);

                    //展开购物车
                    innerText.find(".cart-header").click(function () {
                        if ($("#shopping-cart .totalcount").html() > 0) {
                            $(this).hide();
                            $.fn.drawCartProduct(innerText, opts);
                        }
                    });

                    //关闭购物车
                    innerText.find(".btnclose").click(function () {
                        obj.find(".cart-header").show();
                        obj.find(".cart-mainbody").hide();
                    });
                    obj.append(innerText);
                });
            });
        }
        //加载购物车明细
        $.fn.drawCartProduct = function (obj, opts) {
            obj.find(".cart-mainbody").show();
            obj.find(".cart-product-list").empty();
            Global.post("/ShoppingCart/GetShoppingCart", {
                ordertype: opts.ordertype,
                guid: opts.guid
            }, function (data) {
                doT.exec("plug/shoppingcart/product-list.html", function (templateFun) {
                    if (data.Items.length > 0) {
                        var innerText = templateFun(data.Items);
                        innerText = $(innerText);

                        //详情页增加单据类型
                        innerText.find(".productname").each(function () {
                            $(this).attr("href", $(this).attr("href") + "&type=" + opts.ordertype + "&guid=" + opts.guid);
                        });

                        //删除产品
                        innerText.find(".ico-del").click(function () {
                            var _this = $(this);
                            confirm("确认从清单中移除此材料吗？", function () {
                                Global.post("/ShoppingCart/DeleteCart", {
                                    autoid: _this.data("id")
                                }, function (data) {
                                    if (!data.Status) {
                                        alert("系统异常，请重新操作！");
                                    } else {
                                        _this.parents("tr.item").remove();
                                        obj.find(".totalcount").html(obj.find(".totalcount").html() - 1);
                                    }
                                });
                            });
                        });

                        if (opts.ordertype == 1) {
                            obj.find(".btnconfirm").attr("href", "/Products/ConfirmPurchase").html("返回采购单");;
                        } else if (opts.ordertype == 4) {
                            obj.find(".btnconfirm").attr("href", "/Stock/CreateOverflow").html("返回报溢单");;
                        } else if (opts.ordertype == 11) { //订单
                            if (opts.tid) {
                                obj.find(".btnconfirm").attr("href", "/Task/Detail/" + opts.tid).html("返回任务详情");
                            } else {
                                obj.find(".btnconfirm").attr("href", "/Orders/OrderDetail/" + opts.guid);
                            }
                        }
                        obj.find(".cart-product-list").append(innerText);
                    } else {
                        $(".cart-header").show();
                        $(".cart-mainbody").hide();
                    }
                    
                });
            });
        }
    })(jQuery)
    module.exports = jQuery;
});