
define(function (require, exports, module) {
    var Upload = require("upload"), ProductIco, ImgsIco,
        Global = require("global"),
        Verify = require("verify"), VerifyObject, DetailsVerify, editor,
        doT = require("dot"),
        Easydialog = require("easydialog");
    require("pager");
    require("switch");
    var Params = {
        PageIndex: 1,
        keyWords: "",
        totalCount: 0,
        CategoryID: "",
        ProviderID: "",
        BeginPrice: "",
        EndPrice: "",
        IsPublic: -1,
        OrderBy: "p.CreateTime desc",
        IsAsc: false
    };
    var CacheCategorys = [];
    var CacheChildCategorys = [];
    var Product = {};

    //列表页初始化
    Product.initList = function (url) {
        var _self = this;
        _self.url = url;
        _self.getChildCategory("");;
        _self.bindListEvent();
    }
    //获取分类信息和下级分类
    Product.getChildCategory = function (pid) {
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

        Params.CategoryID = pid;
        _self.getList();
    }
    //绑定下级分类
    Product.bindChildCagegory = function (pid) {
        var _self = this;
        var length = CacheChildCategorys[pid].length;
        if (length > 0) {
            $(".category-child").show();
            for (var i = 0; i < length; i++) {
                var _ele = $(" <li data-id='" + CacheChildCategorys[pid][i].CategoryID + "'>" + CacheChildCategorys[pid][i].CategoryName + "</li>");
                _ele.click(function () {
                    //处理分类MAP
                    var _map = $(" <li data-id='" + $(this).data("id") + "'>" + $(this).html() + "<span>></span></li>");
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

    //绑定列表页事件
    Product.bindListEvent = function () {
        var _self = this;
        $(".category-map li").click(function () {
            $(this).nextAll().remove();
            _self.getChildCategory($(this).data("id"));
        });

        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                Params.keyWords = keyWords;
                Product.getList();
            });
        });

        //切换状态
        $(".search-status li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                Params.PageIndex = 1;
                Params.IsPublic = _this.data("id");
                _self.getList();
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
                _self.getList();
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

            _self.getList();
        });
    }
    //获取产品列表
    Product.getList = function () {
        var _self = this;
        $("#product-items").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='9'><div class='dataLoading'><img src='/modules/images/ico-loading.jpg'/><div></td></tr>");

        Global.post("/Products/GetProductList", { filter: JSON.stringify(Params) }, function (data) {
            $("#product-items").nextAll().remove();

            if (data.Items.length > 0) {
                doT.exec("template/products/products.html", function (templateFun) {
                    var innerText = templateFun(data.Items);
                    innerText = $(innerText);
                    $("#product-items").after(innerText);

                    //绑定启用插件

                    innerText.find(".auditproduct").click(function () {
                        if (confirm("确认通过材料的公开申请吗？")) {
                            _self.editIsPublic($(this).data("id"), 2);
                        }
                    });
                    innerText.find(".cancelproduct").click(function () {
                        if (confirm("确认驳回材料的公开申请吗？")) {
                            _self.editIsPublic($(this).data("id"), 3);
                        }
                    });
                    innerText.find(".deleteproduct").click(function () {
                        if (confirm("确认取消材料的公开状态吗？")) {
                            _self.cancelIsPublic($(this).data("id"));
                        }
                    });
                    innerText.find("img").each(function () {
                        var _this = $(this);
                        _this.attr("src", _self.url + _this.attr("src"))
                    });

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
                    onChange: function (page) {
                        Params.PageIndex = page;
                        _self.getList();
                    }
                });
            }
            else
            {
                $(".tr-header").after("<tr><td colspan='9'><div class='noDataTxt' >暂无数据!<div></td></tr>");
            }
            

        });
    }
    //更改产品状态
    Product.editIsPublic = function (id, value) {
        var _self = this;
        Global.post("/Products/UpdateProductIsPublic", {
            productid: id,
            ispublic: value
        }, function (data) {
            if (data.Status) {
                _self.getList();
            }
        });
    }
    Product.cancelIsPublic = function (id) {
        var _self = this;
        Global.post("/Products/DeleteProductIsPublic", {
            productid: id
        }, function (data) {
            if (data.Status) {
                _self.getList();
            }
        });
    }
    module.exports = Product;
})