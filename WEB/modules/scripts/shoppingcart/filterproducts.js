define(function (require, exports, module) {
    var Global = require("global"),
        Easydialog = require("easydialog"),
        doT = require("dot"),
        quicklyAdd = require("quicklyproduct");
    require("cart");
    require("pager");

    var CacheCategorys = [];
    var CacheChildCategorys = [];
    var CacheProduct = [];
    var cacheDepot = [];
    var Params = {
        CategoryID: "",
        BeginPrice: "",
        EndPrice: "",
        PageIndex: 1,
        keyWords: "",
        DocType: -1,
        IsPublic: -1,
        OrderBy: "pd.CreateTime desc",
        IsAsc:false
    }

    var ObjectJS = {};
    var isLoadding = false;
    //初始化
    ObjectJS.init = function (type, guid, tid) {
        var _self = this;
        _self.type = type;
        _self.guid = guid;
        _self.tid = tid;
        Params.DocType = type;

        Global.post("/System/GetDepotSeatsByWareID", {}, function (data) {
            _self.depots = data.Items;
            _self.getChildCategory("");
        });

        _self.bindEvent();
        $(".content-body").createCart({
            ordertype: type,
            guid: guid,
            tid: tid
        });
    }

    //获取分类信息和下级分类
    ObjectJS.getChildCategory = function (pid) {
        var _self = this;
        $("#category-child").empty();
        
        if (!CacheChildCategorys[pid]) {
            Global.post("/Products/GetChildCategorysByID", {
                categoryid: pid
            }, function (data) {
                CacheChildCategorys[pid] = data.Items;
                _self.bindChildCagegory(pid);
            });
        } else {
            _self.bindChildCagegory(pid);
        }
        if (!!pid) {
            if (!CacheCategorys[pid]) {
                Global.post("/Products/GetCategoryDetailsByID", {
                    categoryid: pid
                }, function (data) {
                    CacheCategorys[pid] = data.Model;
                    _self.bindCagegoryAttr(pid);
                });
            } else {
                _self.bindCagegoryAttr(pid);
            }
        } else {
            $("#attr-price").nextAll(".attr-item").remove();
        }

        Params.CategoryID = pid;
        _self.getProducts();
    }

    //绑定分类属性
    ObjectJS.bindCagegoryAttr = function (pid) {
        var _self = this;
        $("#attr-price").nextAll(".attr-item").remove();
        //属性
        doT.exec("template/shoppingcart/filter-attrs.html", function (templateFun) {
            var html = templateFun(CacheCategorys[pid].AttrLists);
            html = $(html);

            html.find(".value").data("type", 1);
            html.find(".value").click(function () {
                var _this = $(this);
                if (!_this.hasClass("hover")) {
                    _this.addClass("hover");
                    _this.siblings().removeClass("hover");

                    _self.getProducts();
                }
            });
            $("#attr-price").after(html);
        });
        //规格
        doT.exec("template/shoppingcart/filter-attrs.html", function (templateFun) {
            var html = templateFun(CacheCategorys[pid].SaleAttrs);
            html = $(html);

            html.find(".value").data("type", 2);
            html.find(".value").click(function () {
                var _this = $(this);
                if (!_this.hasClass("hover")) {
                    _this.addClass("hover");
                    _this.siblings().removeClass("hover");

                    _self.getProducts();
                }
            });
            $("#attr-price").after(html);
        });
    }

    //绑定下级分类
    ObjectJS.bindChildCagegory = function (pid) {
        var _self = this;
        var length = CacheChildCategorys[pid].length;
        if (length > 0) {
            $(".category-child").show();
            for (var i = 0; i < length; i++) {
                var _ele = $(" <li data-id='" + CacheChildCategorys[pid][i].CategoryID + "'>" + CacheChildCategorys[pid][i].CategoryName + "</li>");
                _ele.click(function () {
                    //处理分类MAP
                    var _map = $(" <li data-id='" + $(this).data("id") + "'><a href='javascript:void(0);'>" + $(this).html() + "</a></li>");
                    _map.click(function () {
                        $(this).nextAll().remove();
                        _self.getChildCategory($(this).data("id"));
                    })
                    $(".category-map").append(_map);
                    _self.getChildCategory($(this).data("id"));
                });
                $("#category-child").append(_ele);
            }
        } else {
            $(".category-child").hide();
        }
    }

    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;
        $(".category-map li").click(function () {
            $(this).nextAll().remove();
            _self.getChildCategory($(this).data("id"));
        });

        //绑定快捷添加材料
        $("#btnQulicklyAddMaterial").click(function () {
            quicklyAdd.create({
                callback: function (data) {
                    if (data.result) {
                        location.href = location.href;
                    } else {
                        alert("添加失败");
                    }
                }
            });
        });
        //搜索
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                Params.keyWords = keyWords;
                _self.getProducts();
            });
        });
        //类型筛选
        $("#attr-type .attrValues .type").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.addClass("hover");
                _this.siblings().removeClass("hover");
                Params.IsPublic = _this.data("id");
                _self.getProducts();
            }
        });
        //价格筛选
        $("#attr-price .attrValues .price").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.addClass("hover");
                _this.siblings().removeClass("hover");
                Params.BeginPrice = _this.data("begin");
                Params.EndPrice = _this.data("end");
                _self.getProducts();
                $("#beginprice").val("");
                $("#endprice").val("");
            }
        });
        //搜索价格区间
        $("#searchprice").click(function () {
            if (!!$("#beginprice").val() && !isNaN($("#beginprice").val())) {
                Params.BeginPrice = $("#beginprice").val();
                $("#attr-price .attrValues .price").removeClass("hover");
            } else if (!$("#beginprice").val()) {
                Params.BeginPrice = "";
            } else {
                $("#beginprice").val("");
            }

            if (!!$("#endprice").val() && !isNaN($("#endprice").val())) {
                Params.EndPrice = $("#endprice").val();
                $("#attr-price .attrValues .price").removeClass("hover");
            } else if (!$("#endprice").val()) {
                Params.EndPrice = "";
            } else {
                $("#endprice").val("");
            }

            _self.getProducts();
        });

        $(".sort-item").click(function () {
            var _this = $(this);
            if (_this.hasClass("hover")) {
                if (_this.find(".asc").hasClass("hover")) {
                    _this.find(".asc").removeClass("hover");
                    _this.find(".desc").addClass("hover");
                    Params.OrderBy = _this.data("column") + " desc ";
                } else {
                    _this.find(".desc").removeClass("hover");
                    _this.find(".asc").addClass("hover");
                    Params.OrderBy = _this.data("column") + " asc ";
                }
            } else {
                _this.addClass("hover").siblings().removeClass("hover");
                _this.siblings().find(".hover").removeClass("hover");
                _this.find(".desc").addClass("hover");
                Params.OrderBy = _this.data("column") + " desc ";
            }
            Params.PageIndex = 1;
            _self.getProducts();
        });

    }

    //绑定产品列表
    ObjectJS.getProducts = function (params) {
        var _self = this;
        var attrs = [];
        $(".filter-attr").each(function () {
            var _this = $(this), _value = _this.find(".hover");
            if (_value.data("id")) {
                var FilterAttr = {
                    AttrID: _this.data("id"),
                    ValueID: _value.data("id"),
                    Type: _value.data("type")
                };
                attrs.push(FilterAttr);
            }
        });

        var opt = $.extend({
            CategoryID: Params.CategoryID,
            PageIndex: Params.PageIndex,
            Keywords: Params.keyWords,
            BeginPrice: Params.BeginPrice,
            EndPrice: Params.EndPrice,
            OrderBy: Params.OrderBy,
            DocType: Params.DocType,
            IsPublic: Params.IsPublic,
            IsAsc: Params.IsAsc,
            Attrs: attrs
        }, params);
        $("#productlist").empty();
        $("#productlist").append("<div class='data-loading'></div>");
        Global.post("/ShoppingCart/GetProductListForShopping", { filter: JSON.stringify(opt) }, function (data) {
                $("#productlist").empty();
                if (data.Items.length > 0) {
                    doT.exec("template/shoppingcart/filter-products.html", function (templateFun) {
                        var html = templateFun(data.Items);
                        html = $(html);

                        //打开产品详情页
                        html.find(".productimg,.name").each(function () {
                            $(this).attr("href", $(this).attr("href") + "&type=" + _self.type + "&guid=" + _self.guid + "&tid=" + _self.tid);
                        });
                        //加入购物车
                        html.find(".btnAddCart").click(function () {
                            var _this = $(this);
                            _self.showDetail(_this.data("pid"), _this.data("did"));
                        });

                        $("#productlist").append(html);

                    });

                    $("#pager").paginate({
                        total_count: data.TotalCount,
                        count: data.PageCount,
                        start: Params.PageIndex,
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
                        float: "normal",
                        onChange: function (page) {
                            Params.PageIndex = page;
                            ObjectJS.getProducts();
                        }
                    });
                } else {
                    $("#productlist").append("<div class='nodata-txt'>暂无数据</div>");
                }
            });
    }

    //加入购物车
    ObjectJS.showDetail = function (pid, did) {
        if (isLoadding) {
            alert("数据加载中,请稍候");
            return false;
        }
        var _self = this;
        var objCache = {};
        //缓存产品信息
        if (!CacheProduct[pid]) {
            isLoadding = true;
            Global.post("/Products/GetProductByIDForDetails", {
                productid: pid,
                did: did,
                type: _self.type
            }, function (data) {
                isLoadding = false;
                CacheProduct[pid] = data.Item;
                doT.exec("template/shoppingcart/product-detail.html", function (templateFun) {
                    var html = templateFun(CacheProduct[pid]);
                    Easydialog.open({
                        container: {
                            id: "product-add-div",
                            header: "选择材料",
                            content: html,
                            callback: function () {

                            }
                        }
                    });
                    
                    _self.depotid = _self.depots[0].DepotID;
                    require.async("dropdown", function () {
                        $(".depot-dropdown").dropdown({
                            prevText: "货位-",
                            defaultText: _self.depots[0].DepotCode,
                            defaultValue: _self.depots[0].DepotID,
                            data: _self.depots,
                            dataText: "DepotCode",
                            dataValue: "DepotID",
                            width: 120,
                            isposition: true,
                            onChange: function (dataValue) {
                                _self.depotid = dataValue.value;
                                _self.showDepotStock(pid);
                            }
                        });
                    });

                    Easydialog.toPosition();
                    _self.bindDetail(CacheProduct[pid], did);
                    _self.bindDetailEvent(CacheProduct[pid], pid, did)
                });
            });
        } else {
            doT.exec("template/shoppingcart/product-detail.html", function (templateFun) {
                var html = templateFun(CacheProduct[pid]);
                Easydialog.open({
                    container: {
                        id: "product-add-div",
                        header: "选择材料",
                        content: html,

                        callback: function () {

                        }
                    }
                });
                _self.depotid = _self.depots[0].DepotID;
                require.async("dropdown", function () {
                    $(".depot-dropdown").dropdown({
                        prevText: "货位-",
                        defaultText: _self.depots[0].DepotCode,
                        defaultValue: _self.depots[0].DepotID,
                        data: _self.depots,
                        dataText: "DepotCode",
                        dataValue: "DepotID",
                        width: 120,
                        isposition: true,
                        onChange: function (dataValue) {
                            _self.depotid = dataValue.value;
                            _self.showDepotStock(pid);
                        }
                    });
                });
                Easydialog.toPosition();
                _self.bindDetail(CacheProduct[pid], did);
                _self.bindDetailEvent(CacheProduct[pid], pid, did)
            });
        }
    }

    //绑定加入购物车事件
    ObjectJS.bindDetailEvent = function (model, pid, did) {
        var _self = this;

        //选择规格
        $("#saleattr li.cart-value").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.addClass("hover");
                _this.siblings().removeClass("hover");
                for (var i = 0, j = model.ProductDetails.length; i < j; i++) {
                    if (_this.data("id") == model.ProductDetails[i].ProductDetailID) {
                        _self.detailid = model.ProductDetails[i].ProductDetailID;
                        $("#price").html("￥" + model.ProductDetails[i].Price.toFixed(2));
                        if (model.ProductDetails[i].ImgS) {
                            $("#productimg").attr("src", model.ProductDetails[i].ImgS);
                        } else {
                            $("#productimg").attr("src", model.ProductImage);
                        }
                        $("#productStockQuantity").text(model.ProductDetails[i].StockIn - model.ProductDetails[i].LogicOut);
                        _self.showDepotStock(pid);
                        return;
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
                Global.post("/ShoppingCart/AddShoppingCart", {
                    productid: pid,
                    detailsid: _self.detailid,
                    quantity: $("#quantity").val(),
                    unitid: $("#small").data("id"),
                    ordertype: _self.type,
                    depotid: _self.depotid || "",
                    guid: _self.guid,
                    remark: remark
                }, function (data) {
                    if (data.Status) {
                        Easydialog.close();
                        $("#shopping-cart .totalcount").html($("#shopping-cart .totalcount").html() * 1 + 1);
                    }
                });
            });
        });

    }

    //绑定信息
    ObjectJS.bindDetail = function (model, did) {
        var _self = this;
        _self.detailid = did;
        //绑定子产品详情
        for (var i = 0, j = model.ProductDetails.length; i < j; i++) {
            if (model.ProductDetails[i].ProductDetailID == did) {
                $(".cart-attr-item").find("li[data-id='" + model.ProductDetails[i].ProductDetailID + "']").addClass("hover");
                $("#price").html("￥" + model.ProductDetails[i].Price.toFixed(2));
                $("#productimg").attr("src", model.ProductDetails[i].ImgS);
                $("#productStockQuantity").text(model.ProductDetails[i].StockIn - model.ProductDetails[i].LogicOut);
                break;
            }
        }
    }

    //获取货位库存
    ObjectJS.showDepotStock = function (pid) {
        var _self = this;
        $("#productDepotQuantity").text("0.00");
        for (var i = 0, j = CacheProduct[pid].ProductDetails.length; i < j; i++) {
            if (CacheProduct[pid].ProductDetails[i].ProductDetailID == _self.detailid) {
                var model = CacheProduct[pid].ProductDetails[i];
                for (var ii = 0, jj = model.DetailStocks.length; ii < jj; ii++) {
                    if (model.DetailStocks[ii].DepotID == _self.depotid) {
                        $("#productDepotQuantity").text(model.DetailStocks[ii].StockIn - model.DetailStocks[ii].StockOut);
                        return;
                    }
                }
            }
        }
    };

    module.exports = ObjectJS;
});