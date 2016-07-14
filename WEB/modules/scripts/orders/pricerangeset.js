define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Easydialog = require("easydialog");

    var ObjectJS = {};
    ObjectJS.init = function (orderid) {
        ObjectJS.bindEvent(orderid);
    };

    ObjectJS.bindEvent = function (orderid) {
        $(".price-range-set").click(function () {            
            $(".center-head").nextAll().remove();
            $("#bfe_overlay").show();
            $(".price-range").show();
            ObjectJS.getPriceRange(orderid);
        });

        $(".add-price-range").click(function () {
            $(".no-data").remove();
            $(".center-range").append('<li><div class="li-range"><input class="min-number" type="text" maxlength="10" value="" /><span class="hide">22</span></div><div class="li-range"><input class="max-number" type="text" maxlength="10" value="" /><span class="hide">122</span></div><div class="li-range"><input class="price" type="text" maxlength="10" value="" /><span class="hide">3</span></div><div class="operation li-range"><div class="save-price-range btn mLeft10">保存</div><div class="cancel-price-range btn mLeft10">取消</div></div></li>');
            $(".save-price-range").click(function () {
                var _this = $(this).parent().parent();
                var minnumber = _this.find(".min-number").val();
                var maxnumber = _this.find(".max-number").val();
                var price = _this.find(".price").val();

                if (!minnumber.isDouble()||minnumber<0) {
                    alert("起始数量不能为非数字或者小于零");
                    return;
                }
                if (!maxnumber.isDouble() || maxnumber < 0) {
                    alert("终止数量不能为非数字或者小于零");
                    return;
                }
                if (!price.isDouble() || price < 0) {
                    alert("价格不能为非数字或者小于零");
                    return;
                }

                if (minnumber == "" || maxnumber == "" || price == "") {
                    alert("内容不能为空");
                } else {
                    var model = {
                        MinQuantity: minnumber,
                        MaxQuantity: maxnumber,
                        Price: price,
                        OrderID: orderid
                    };
                    Global.post("/Orders/AddOrderPriceRange", {
                        model: JSON.stringify(model)
                    }, function (data) {
                        if (data.status) {
                            $(".center-head").nextAll().remove();
                            ObjectJS.getPriceRange(orderid);
                        } else {
                            alert("添加失败");
                        }
                    });
                }
            });

            $(".cancel-price-range").click(function () {
                $(this).parent().parent().remove();
            });
        });

        $(".close_btn").click(function () {
            $("#bfe_overlay").hide();
            $(".price-range").hide();
        });
    };

    ObjectJS.getPriceRange = function (orderid) {
        $(".center-range").append('<div class="data-loading"><div>');
        Global.post("/Orders/GetOrderPriceRanges", { orderid: orderid }, function (data) {
            $(".data-loading").remove();
            if (data.items.length>0) {
                doT.exec("template/orders/pricerangge.html", function (template) {
                    var html = template(data.items);
                    html = $(html);
                    $(".center-range").append(html);

                    html.find(".update").click(function () {
                        var _this = $(this).parent().parent();
                        _this.find(".update,.delete,.txt").hide();
                        _this.find(".save,.cancel,input").show();
                    });

                    html.find(".delete").click(function () {
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

                    html.find(".save").click(function () {
                        var _this = $(this).parent().parent();                        
                        var rangeid = _this.data("rangeid");
                        var minnumber = _this.find(".min-number").val();
                        var maxnumber = _this.find(".max-number").val();
                        var price = _this.find(".price").val();
                        if (!minnumber.isDouble() || minnumber < 0) {
                            alert("起始数量不能为非数字或者小于零");
                            return;
                        }
                        if (!maxnumber.isDouble() || maxnumber < 0) {
                            alert("终止数量不能为非数字或者小于零");
                            return;
                        }
                        if (!price.isDouble() || price < 0) {
                            alert("价格不能为非数字或者小于零");
                            return;
                        }
                        if (minnumber == "" || maxnumber == "" || price == "") {
                            alert("内容不能为空");
                        } else {
                            var model = {
                                RangeID: rangeid,
                                MinQuantity: minnumber,
                                MaxQuantity: maxnumber,
                                Price: price,
                            };
                            Global.post("/Orders/UpdateOrderPriceRange", {
                                model: JSON.stringify(model)
                            }, function (obj) {
                                if (obj.status) {
                                    _this.find(".min-number").next().html(minnumber);
                                    _this.find(".max-number").next().html(maxnumber);
                                    _this.find(".price").next().html(price);

                                    _this.find(".update,.delete,.txt").show();
                                    _this.find(".save,.cancel,input").hide();                                    
                                } else {
                                    alert("添加失败");
                                }
                            });
                        }
                    });

                    html.find(".cancel").click(function () {
                        var _this = $(this).parent().parent();
                        _this.find(".update,.delete,.txt").show();
                        _this.find(".save,.cancel,input").hide();
                    });
                });
            } else {
                $(".center-range").append('<div class="center no-data">暂无数据</div>');
            }
        })
    }

    module.exports = ObjectJS;
})