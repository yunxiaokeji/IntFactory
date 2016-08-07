
define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot");
    require("pager");
    require("cart");
    require("dropdown");

    var ObjectJS = {};

    var cacheDepot = [];

    //添加页初始化
    ObjectJS.init = function (model, detailid, ordertype, guid, tid, depotItem) {
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
                tid: tid
            });
            $(".choose-div").show();
        }

        //通过报损单进入初始化数据
        if (ordertype == 3) {
            //初始化货位数据
            var depots = JSON.parse(depotItem.replace(/&quot;/g, '"'));
            if (depots.length > 0) {
                _self.depotID = depots[0].DepotID;
                $(".depot-dropdown").dropdown({
                    prevText: "货位－",
                    defaultText: depots[0].DepotCode,
                    defaultValue: depots[0].DepotID,
                    data: depots,
                    dataText: "DepotCode",
                    dataValue: "DepotID",
                    width: 120,
                    onChange: function (data) {
                        _self.depotID = data.value;
                    }
                });
                $(".choose-div").show();
            } else {
                $(".choose-div").hide();
            }
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

                //通过报损单进入，切换规格显示不同货位
                if (_self.ordertype == 3) {
                    if (!cacheDepot[_this.data('id')]) {
                        Global.post("/Stock/GetDeoptByProductDetailID", { did: _this.data('id') }, function (data) {
                            var depots = data.depots;
                            cacheDepot[_this.data('id')] = depots;
                            if (depots.length > 0) {
                                _self.depotID = depots[0].DepotID;
                                $(".depot-dropdown").dropdown({
                                    prevText: "货位－",
                                    defaultText: depots[0].DepotCode,
                                    defaultValue: depots[0].DepotID,
                                    data: depots,
                                    dataText: "DepotCode",
                                    dataValue: "DepotID",
                                    width: 120,
                                    isposition: true,
                                    onChange: function (data) {
                                        _self.depotid = data.value;
                                    }
                                });
                                $(".choose-div").show();
                            } else {
                                $(".choose-div").hide();
                            }
                        });
                    } else {
                        var depots = cacheDepot[_this.data('id')];
                        if (depots.length > 0) {
                            _self.depotID = depots[0].DepotID;
                            $(".depot-dropdown").dropdown({
                                prevText: "货位－",
                                defaultText: depots[0].DepotCode,
                                defaultValue: depots[0].DepotID,
                                data: depots,
                                dataText: "DepotCode",
                                dataValue: "DepotID",
                                width: 120,
                                isposition: true,
                                onChange: function (data) {
                                    _self.depotid = data.value;
                                }
                            });
                            $(".choose-div").show();
                        } else {
                            $(".choose-div").hide();
                        }
                    }
                }

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
                        //不带type进入材料详情可直接添加报损、报溢、采购单
                        if (_self.ordertype == 0 && _self.saveType == 3) {
                            ObjectJS.getDepotsByID(_this.data('id'));
                        }
                        return;
                    } else {
                        $("#addcart").prop("disabled", true).addClass("addcartun");
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
                    depotid: _self.depotID ? _self.depotID : "",
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

        //产品数量
        $(".quantity").blur(function () {
            if (!$(this).val().isDouble()) {
                $(this).val("1");
            } else if ($(this).val() < 0) {
                $(this).val("1");
            }
        });

        //填写库存数量
        $(".stock-count").blur(function () {
            if (!$(this).val().isDouble()) {
                $(this).val("1");
            } else if ($(this).val() < 0) {
                $(this).val("1");
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
                $("#addDoc").html($(this).text().trim());
                $("#addDocAndBack").html($(this).text().trim() + "并返回");
                if ($(this).data('type') == 3) {
                    if ($("#productStockQuantity").text() * 1 <= 0) {
                        $(".quickly-add-stock").hide();
                    } else {
                        if ($(".sales-item li.hover").data('id')) {
                            ObjectJS.getDepotsByID($(".sales-item li.hover").data('id'));
                        }
                        $(".quickly-add-stock").show();
                        $(".damaged-dropdown").show();
                    }
                } else {
                    $(".quickly-add-stock").show();
                    $(".damaged-dropdown").hide();
                }
                $(".dropdown-ul").hide();
                _self.saveType = $(this).data('type');
            });

            //加入单据
            $("#addDoc").click(function () {
                var _this = $(this);
                if ($(".sales-item li").hasClass('hover')) {
                    if (($(".stock-count").val() * 1) <= 0) {
                        alert("数量必须大于0");
                        return false;
                    }
                    ObjectJS.addDoc(_this.data('type'));
                } else {
                    alert("请选择材料规格");
                }
            });

            //加入并返回单据详情
            $("#addDocAndBack").click(function () {
                var _this = $(this);
                if ($(".sales-item li").hasClass('hover')) {
                    if (($(".stock-count").val() * 1) <= 0) {
                        alert("数量必须大于0");
                        return false;
                    }
                    ObjectJS.addDoc(_this.data('type'));
                } else {
                    alert("请选择材料规格");
                }
            });
        }
    }

    ObjectJS.getDepotsByID = function (id) {
        var _self = ObjectJS;
        if (!cacheDepot[id]) {
            Global.post("/Stock/GetDeoptByProductDetailID", { did: id }, function (data) {
                var depots = data.depots;
                cacheDepot[id] = depots;
                if (depots.length > 0 && $("#productStockQuantity").text() * 1>0) {
                    _self.depotID = depots[0].DepotID;
                    $(".damaged-dropdown").dropdown({
                        prevText: "货位－",
                        defaultText: depots[0].DepotCode,
                        defaultValue: depots[0].DepotID,
                        data: depots,
                        dataText: "DepotCode",
                        dataValue: "DepotID",
                        width: 120,
                        onChange: function (data) {
                            _self.depotID = data.value;
                        }
                    });
                    $(".quickly-add-stock").show();
                } else {
                    $(".quickly-add-stock").hide();
                }
            });
        } else {
            var depots = cacheDepot[id];
            if (depots.length > 0 && $("#productStockQuantity").text() * 1 > 0) {
                _self.depotID = depots[0].DepotID;
                $(".damaged-dropdown").dropdown({
                    prevText: "货位－",
                    defaultText: depots[0].DepotCode,
                    defaultValue: depots[0].DepotID,
                    data: depots,
                    dataText: "DepotCode",
                    dataValue: "DepotID",
                    width: 120,
                    onChange: function (data) {
                        _self.depotID = data.value;
                    }
                });
                $(".quickly-add-stock").show();
            } else {
                $(".quickly-add-stock").hide();
            }
        }
    };

    //快捷添加单据（ordertype为0时才会有）
    ObjectJS.addDoc = function (type) {
        var _self = ObjectJS;
        var remark = "";
        $("#saleattr ul.salesattr").each(function () {
            var _this = $(this);
            remark += "[" + _this.find(".attrkey").html() + _this.find("li.hover").html() + "]";
        });
        Global.post("/ShoppingCart/AddShoppingCart", {
            productid: _self.productid,
            detailsid: _self.detailid,
            quantity: $(".stock-count").val() * 1,
            unitid: $("#unit li.hover").data("id"),
            isBigUnit: $("#unit li.hover").data("value"),
            ordertype: _self.saveType,
            depotid: _self.saveType == 3 ? _self.depotID : "",
            guid: '',
            remark: remark
        }, function (data) {
            if (data.Status) {
                if (type == 1) {
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

                if (_self.ordertype == 0) {
                    if (!cacheDepot[productdetailid]) {
                        Global.post("/Stock/GetDeoptByProductDetailID", { did: productdetailid }, function (data) {
                            var depots = data.depots;
                            cacheDepot[productdetailid] = depots;
                            if (depots.length > 0) {
                                _self.depotID = depots[0].DepotID;
                                $(".damaged-dropdown").dropdown({
                                    prevText: "货位－",
                                    defaultText: depots[0].DepotCode,
                                    defaultValue: depots[0].DepotID,
                                    data: depots,
                                    dataText: "DepotCode",
                                    dataValue: "DepotID",
                                    width: 120,
                                    onChange: function (data) {
                                        _self.depotID = data.value;
                                    }
                                });
                                $(".damaged").show();
                            } else {
                                $(".damaged").hide();
                            }
                        });
                    } else {
                        var depots = cacheDepot[productdetailid];
                        if (depots.length > 0) {
                            _self.depotID = depots[0].DepotID;
                            $(".damaged-dropdown").dropdown({
                                prevText: "货位－",
                                defaultText: depots[0].DepotCode,
                                defaultValue: depots[0].DepotID,
                                data: depots,
                                dataText: "DepotCode",
                                dataValue: "DepotID",
                                width: 120,
                                onChange: function (data) {
                                    _self.depotID = data.value;
                                }
                            });
                            $(".damaged").show();
                        } else {
                            $(".damaged").hide();
                        }
                    }
                }

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