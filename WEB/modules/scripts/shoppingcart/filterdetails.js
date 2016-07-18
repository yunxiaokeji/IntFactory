
define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot");
    require("pager");
    require("cart");

    var ObjectJS = {};
    //添加页初始化
    ObjectJS.init = function (model, detailid, ordertype, guid,tid) {
        var _self = this;
        _self.detailid = detailid;
        _self.ordertype = ordertype;
        _self.guid = guid;
        model = JSON.parse(model.replace(/&quot;/g, '"'));
        _self.productid = model.ProductID;
        $("#productStockQuantity").text(model.StockIn - model.LogicOut);

        _self.bindDetail(model);
        _self.bindEvent(model);

        if (ordertype && ordertype > 0) {
            $(".content-body").createCart({
                ordertype: ordertype,
                guid: guid,
                tid:tid
            });
            $(".choose-div").show();
        }
        $(".product-name").css("width", $(".content-body").width() - 500);
    }
    //绑定事件
    ObjectJS.bindEvent = function (model) {
        var _self = this;

        //选择规格
        $("#saleattr li.value").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.addClass("hover");
                _this.siblings().removeClass("hover");
                for (var i = 0, j = model.ProductDetails.length; i < j; i++) {

                    var bl = true, vales = model.ProductDetails[i].ProductDetailID, unitid = model.ProductDetails[i].UnitID;
                    $(".salesattr li.hover").each(function () {
                        if (vales.indexOf($(this).data("id")) < 0) {
                            bl = false;
                        }
                    });

                    if (bl) {
                        $("#addcart").prop("disabled", false).removeClass("addcartun");
                        _self.detailid = model.ProductDetails[i].ProductDetailID;
                        if ($("#small").hasClass("hover")) {
                            $("#price").html("￥" + model.ProductDetails[i].Price.toFixed(2));
                        } else {
                            $("#price").html("￥" + model.ProductDetails[i].BigPrice.toFixed(2));
                        }
                        if (model.ProductDetails[i].ImgS) {
                            $("#productimg").attr("src", model.ProductDetails[i].ImgS);
                        } else {
                            $("#productimg").attr("src", model.ProductDetails[i].ProductImage);
                        }
                        $("#productStockQuantity").text(model.ProductDetails[i].StockIn - model.ProductDetails[i].LogicOut);
                        return;
                    } else {
                        $("#addcart").prop("disabled", true).addClass("addcartun");
                    }
                }
            }
        });

        //产品数量
        $("#quantity").blur(function () {
            if (!$(this).val().isDouble()) {
                $(this).val("1");
            } else if ($(this).val() < 0) {
                $(this).val("1");
            }
        });
        //+1
        $("#quantityadd").click(function () {
            $("#quantity").val($("#quantity").val() * 1 + 1);
        });
        //-1
        $("#quantityreduce").click(function () {
            if ($("#quantity").val() != "1") {
                $("#quantity").val($("#quantity").val() * 1 - 1);
            }
        });

        //加入购物车
        $("#addcart").click(function () {
            var cart = $("#shopping-cart").offset();
            var temp = $("<div style='width:30px;height:30px;'><img style='width:30px;height:30px;' src='" + $("#productimg").attr("src") + "' /></div>");
            temp.offset({ top: $(this).offset().top + 20, left: $(this).offset().left + 100 });
            temp.css("position", "absolute");
            $("body").append(temp);
            temp.animate({ top: cart.top, left: cart.left }, 500, function () {
                temp.remove();
                var remark = "";
                $("#saleattr ul.salesattr").each(function () {
                    var _this = $(this);
                    remark += "[" + _this.find(".attrkey").html() + _this.find("li.hover").html() + "]";
                });
                Global.post("/ShoppingCart/AddShoppingCart", {
                    productid: _self.productid,
                    detailsid: _self.detailid,
                    quantity: $("#quantity").val(),
                    unitid: $("#unit li.hover").data("id"),
                    isBigUnit: $("#unit li.hover").data("value"),
                    ordertype: _self.ordertype,
                    guid: _self.guid,
                    remark: remark
                }, function (data) {
                    if (data.Status) {
                        $("#quantity").val("1");
                        $("#shopping-cart .totalcount").html($("#shopping-cart .totalcount").html() * 1 + 1);
                    }
                });
            });
        });

        //切换模块
        $(".show-nav-ul li").click(function () {
            var _this = $(this);
            _this.siblings().removeClass("hover");
            _this.addClass("hover");
            $(".nav-partdiv").hide();
            $("#" + _this.data("id")).show();

            if (_this.data("id") == "useLogs" && (!_this.data("first") || _this.data("first") == 0)) {
                _this.data("first", "1");
                _self.getUseLogs(1);
            } 
        });

    }
    //绑定信息
    ObjectJS.bindDetail = function (model) {
        var _self = this;
        _self.productid = model.ProductID;
        //绑定子产品详情
        for (var i = 0, j = model.ProductDetails.length; i < j; i++) {
            if (model.ProductDetails[i].ProductDetailID == _self.detailid) {
                $(".attr-item").find("li[data-id='" + model.ProductDetails[i].ProductDetailID + "']").addClass("hover");
                $("#price").html("￥" + model.ProductDetails[i].Price.toFixed(2));
                if (model.ProductDetails[i].ImgS) {
                    $("#productimg").attr("src", model.ProductDetails[i].ImgS);
                }
                $("#productStockQuantity").text(model.ProductDetails[i].StockIn - model.ProductDetails[i].LogicOut);
                break;
            }
        }
        $("#description").html(decodeURI(model.Description));
    }

    //获取使用记录
    ObjectJS.getUseLogs = function (pages) {
        var _self = this;
        $(".use-table-list .tr-header").nextAll().remove();
        $(".use-table-list .tr-header").after("<tr class='list-item'><td colspan='5'><div class='data-loading'></div><td></tr>");
        Global.post("/Products/GetProductUseLogs", {
            productid: _self.productid,
            pageindex: pages
        }, function (data) {
            $(".data-loading").parents('tr').remove();
            if (data.items.length > 0) {
                doT.exec("template/products/uselogs.html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);
                    $(".use-table-list .tr-header").after(innerhtml);
                });
            } else {
                $(".use-table-list .tr-header").after("<tr><td colspan='5'><div class='nodata-txt' >暂无记录!<div></td></tr>");
            }
            $("#pagerUseLogs").paginate({
                total_count: data.totalCount,
                count: data.pageCount,
                start: pages,
                display: 5,
                border: true,
                border_color: '#fff',
                text_color: '#333',
                background_color: '#fff',
                border_hover_color: '#ccc',
                text_hover_color: '#000',
                background_hover_color: '#efefef',
                rotate: true,
                images: false,
                mouse: 'slide',
                float: "left",
                onChange: function (page) {
                    _self.getUseLogs(page);
                }
            });
        });
    }

    module.exports = ObjectJS;
})