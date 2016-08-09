
define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot");
    require("pager");
    require("cart");
    require("dropdown");

    var ObjectJS = {};

    //添加页初始化
    ObjectJS.init = function (model, detailid, ordertype, guid, tid, depotItem) {
        var _self = this;
        model = JSON.parse(model.replace(/&quot;/g, '"'));
        var depots = JSON.parse(depotItem.replace(/&quot;/g, '"'));

        _self.detailid = detailid;
        _self.ordertype = ordertype;
        _self.guid = guid;
        
        _self.model = model;
        _self.productid = model.ProductID;
        $("#productStockQuantity").text(model.StockIn - model.LogicOut);

        _self.depotID = depots[0].DepotID;
        _self.depots = depots;
        
        _self.bindDetail(model);
        _self.bindEvent(model);

        if (ordertype && ordertype > 0) {
            $(".content-body").createCart({
                ordertype: ordertype,
                guid: guid,
                tid: tid
            });
            $(".choose-div").show();
        }

        _self.showDepotStock();
        $(".product-name").css("width", $(".content-body").width() - 500);
    }

    //绑定事件
    ObjectJS.bindEvent = function (model) {
        var _self = this;

        $(".depot-dropdown").dropdown({
            prevText: "货位－",
            defaultText: _self.depots[0].DepotCode,
            defaultValue: _self.depots[0].DepotID,
            data: _self.depots,
            dataText: "DepotCode",
            dataValue: "DepotID",
            width: 120,
            onChange: function (data) {
                _self.depotID = data.value;
                _self.showDepotStock();
            }
        });
       
        //选择规格
        $("#saleattr li.value").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {

                _this.addClass("hover");
                _this.siblings().removeClass("hover");

                for (var i = 0, j = model.ProductDetails.length; i < j; i++) {
                    if (model.ProductDetails[i].ProductDetailID == _this.data("id")) {
                        _self.detailid = model.ProductDetails[i].ProductDetailID;
                        $("#price").html("￥" + model.ProductDetails[i].Price.toFixed(2));
                        if (model.ProductDetails[i].ImgS) {
                            $("#productimg").attr("src", model.ProductDetails[i].ImgS);
                        } else {
                            $("#productimg").attr("src", model.ProductDetails[i].ProductImage);
                        }
                        $("#productStockQuantity").text(model.ProductDetails[i].StockIn - model.ProductDetails[i].LogicOut);
                        
                        ObjectJS.showDepotStock();
                        return;
                    } 
                }
            }
        });

        $(document).click(function (e) {
            //隐藏下拉
            if (!$(e.target).parents().hasClass("dropdown-ul") && !$(e.target).parents().hasClass("dropdown") && !$(e.target).hasClass("dropdown")) {
                $(".dropdown-ul").hide();
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

        //加入购物车
        $("#addcart").click(function () {
            if (!_self.detailid) {
                alert("请选择材料规格");
                return;
            }
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
                    unitid: $("#small").data("id"),
                    ordertype: _self.ordertype,
                    depotid: _self.depotID || "",
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

        //单据类型为0可进行报损、报溢、加入采购单操作
        if (_self.ordertype == 0) {
            //下拉事件
            $(".dropdown").click(function () {
                var _this = $(this);
                var position = _this.position();
                $(".dropdown-ul").css({ "top": position.top+28, "left": position.left-2 }).show().mouseleave(function () {
                    $(this).hide();
                });
                _self.docid = _this.data("id");
            });

            //采购
            $("#purchases,#damaged,#overflow").click(function () {
                $("#addDoc").html($(this).text().trim()).show();
                $("#addDocAndBack").show();
                $("#addcart").hide();
                $(".dropdown-ul").hide();
                _self.saveType = $(this).data('type');
                $(".choose-div").show();
                $("#quantity").val("1");
            });

            $("#addDoc").click(function () {
                var _this = $(this);
                if (!_self.detailid) {
                    alert("请选择材料规格");
                    return;
                }
                if (($("#quantity").val() * 1) <= 0) {
                    alert("数量必须大于0");
                    return false;
                }
                ObjectJS.addDoc(false);
            });

            //加入并返回单据详情
            $("#addDocAndBack").click(function () {
                var _this = $(this);
                if (!_self.detailid) {
                    alert("请选择材料规格");
                    return;
                }
                if (($("#quantity").val() * 1) <= 0) {
                    alert("数量必须大于0");
                    return false;
                }
                ObjectJS.addDoc(true);
            });
        }
    }

    //获取货位库存
    ObjectJS.showDepotStock = function () {
        var _self = this;
        $("#productDepotQuantity").text("0.00");
        if (_self.detailid) {
            for (var i = 0, j = _self.model.ProductDetails.length; i < j; i++) {
                if (_self.model.ProductDetails[i].ProductDetailID == _self.detailid) {
                    var model = _self.model.ProductDetails[i];
                    for (var ii = 0, jj = model.DetailStocks.length; ii < jj; ii++) {
                        if (model.DetailStocks[ii].DepotID == _self.depotID) {
                            $("#productDepotQuantity").text(model.DetailStocks[ii].StockIn - model.DetailStocks[ii].StockOut);
                            return;
                        }
                    }
                }
            }
        }
    };

    //快捷添加单据（ordertype为0时才会有）
    ObjectJS.addDoc = function (isBack) {
        var _self = ObjectJS;
        var remark = "";
        Global.post("/ShoppingCart/AddShoppingCart", {
            productid: _self.productid,
            detailsid: _self.detailid,
            quantity: $("#quantity").val() * 1,
            unitid: $("#small").data("id"),
            ordertype: _self.saveType,
            depotid: _self.depotID || "",
            guid: '',
            remark: remark
        }, function (data) {
            if (data.Status) {
                if (!isBack) {
                    alert("添加成功");
                } else {
                    var href = "";
                    if (_self.saveType == 1) {
                        href = "/Products/ConfirmPurchase";
                    } else if (_self.saveType == 3) {
                        href = "/Stock/CreateDamaged";
                    } else if (_self.saveType == 4) {
                        href = "/Stock/CreateOverflow";
                    }
                    location.href = href;
                }
            } else {
                alert("网络异常，请重试");
            }
        });
    };

    //绑定信息
    ObjectJS.bindDetail = function (model) {
        var _self = this;
        _self.productid = model.ProductID;
        //绑定子产品详情
        for (var i = 0, j = model.ProductDetails.length; i < j; i++) {
            if (model.ProductDetails[i].ProductDetailID == _self.detailid) {
                var productdetailid = model.ProductDetails[i].ProductDetailID;
                $(".attr-item").find("li[data-id='" + productdetailid + "']").addClass("hover");

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