define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Easydialog = require("easydialog");

    var ObjectJS = {};
    ObjectJS.isLoading = true;

    ObjectJS.init = function (orderid) {
        ObjectJS.bindEvent(orderid);
    };

    ObjectJS.bindEvent = function (orderid) {

        $(".price-range-set").click(function () {            
            $(".center-range").empty();
            $("#bfe_overlay").show();
            $(".price-range").show();
            ObjectJS.getPriceRange(orderid);
        });

        $(".add-price-range").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            $(".no-data").remove();
            doT.exec("template/orders/addpricerange.html", function (template) {
                var innerText = template({});                
                innerText = $(innerText);

                $(".center-range").append(innerText);
                
                ObjectJS.updateAndAddPriceRange(innerText, ".save-price-range", orderid);

                innerText.find(".cancel-price-range").click(function () {
                    $(this).parent().parent().remove();
                });

                
            });
        });

        $(".close_btn,#bfe_overlay").click(function () {
            $("#bfe_overlay").hide();
            $(".price-range").hide();
        });
    };

    ObjectJS.getPriceRange = function (orderid) {
        ObjectJS.isLoading = false;
        $(".center-range").append('<div class="data-loading"><div>');
        Global.post("/Orders/GetOrderPriceRanges", { orderid: orderid }, function (data) {
            $(".data-loading").remove();
            if (data.items.length>0) {
                doT.exec("template/orders/pricerangge.html", function (template) {
                    var innerText = template(data.items);
                    innerText = $(innerText);
                    $(".center-range").append(innerText);

                    innerText.find(".update").click(function () {
                        var _this = $(this).parent().parent();
                        _this.find(".update,.delete,.txt").hide();
                        _this.find(".save,.cancel,input").show();                        
                    });

                    innerText.find(".delete").click(function () {
                        var _this = $(this).parent().parent();
                        var rangeid = _this.data("rangeid");

                        var confirmMsg = "确定删除此价格区间?";
                        confirm(confirmMsg, function () {
                            Global.post("/Orders/DeleteOrderPriceRange", { rangeid: rangeid }, function (data) {
                                if (data.status > 0) {
                                    _this.fadeOut(400,function () {
                                        _this.remove();
                                    });
                                } else {
                                    alert("删除失败");
                                }
                            })
                        });                        
                    });

                    ObjectJS.updateAndAddPriceRange(innerText, ".save",orderid);

                    innerText.find(".cancel").click(function () {
                        var _this = $(this).parent().parent();
                        _this.find(".update,.delete,.txt").show();
                        _this.find(".save,.cancel,input").hide();
                    });
                });
            } else {
                $(".center-range").append('<div class="center no-data mTop50">暂无数据</div>');
            }
            ObjectJS.isLoading = true;
        })
    }
    
    ObjectJS.updateAndAddPriceRange = function (innerText,save,orderid) {
        innerText.find(save).click(function () {
            var _this = $(this).parent().parent();
            var rangeid = _this.data("rangeid");
            var minnumber = _this.find(".min-number").val();
            var maxnumber = _this.find(".max-number").val();
            var price = _this.find(".price").val();
            if (!minnumber.isDouble() || !minnumber.isInt() || Number(minnumber) <= 0) {
                alert("起始数量不能为非整数或者不小于零的数字");
                return;
            }
            if (!maxnumber.isDouble() || !maxnumber.isInt() || Number(maxnumber) <= 0) {
                alert("终止数量不能为非整数数字不小于零的数字");
                return;
            }
            if (!price.isDouble() || price < 0) {
                alert("价格不能为非数字或者小于零");
                return;
            }            
            if (Number(minnumber)>=Number(maxnumber)) {
                alert("起始数量不能大于或等于终止数量");
                return;
            }
            var model = {
                RangeID: rangeid,
                MinQuantity: minnumber,
                MaxQuantity: maxnumber,
                Price: price,
                OrderID: orderid
            };               
            Global.post("/Orders/OrderPriceRange", {
                model: JSON.stringify(model)
            }, function (obj) {
                if (obj.status) {
                    if (save ==".save") {
                        _this.find(".min-number").next().html(minnumber);
                        _this.find(".max-number").next().html(maxnumber);
                        _this.find(".price").next().html(price);

                        _this.find(".update,.delete,.txt").show();
                        _this.find(".save,.cancel,input").hide();
                    } else {
                        $(".center-range").empty();
                        ObjectJS.getPriceRange(orderid);
                            
                    }
                } else {
                    alert("添加失败");
                }
            });            
        });
    };

    module.exports = ObjectJS;
})