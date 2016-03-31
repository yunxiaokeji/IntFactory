
define(function (require, exports, module) {
    var City = require("city"), CityObject,
        Upload = require("upload"), ProductIco, ImgsIco,
        Global = require("global"),
        Verify = require("verify"), VerifyObject, DetailsVerify, editor, VerifyProdiver,
        doT = require("dot"),
        Easydialog = require("easydialog");
    require("pager");
    require("switch");
    require("autocomplete");
    var Params = {
        PageIndex: 1,
        keyWords: "",
        totalCount: 0,
        CategoryID: "",
        ProviderID: "",
        BeginPrice: "",
        EndPrice: "",
        OrderBy: "p.CreateTime desc",
        IsAsc: false
    };
    var CacheCategorys = [];
    var CacheChildCategorys = [];
    var Product = {};
    //添加页初始化
    Product.init = function (Editor, type, guid, tid) {
        var _self = this;
        _self.type = type;
        _self.guid = guid;
        _self.tid = tid;
        editor = Editor;
        _self.bindEvent();
    }
    
    //绑定事件
    Product.bindEvent = function () {
        var _self = this;
        ProductIco = Upload.createUpload({
            element: "#productIco",
            buttonText: "选择图片",
            className: "",
            data: { folder: '', action: 'add', oldPath: "" },
            success: function (data, status) {
                if (data.Items.length > 0) {
                    _self.ProductImage = data.Items[0];
                    $("#productImg").attr("src", data.Items[0]);
                } else {
                    alert("只能上传jpg/png/gif类型的图片，且大小不能超过10M！");
                }
            }
        });
        $("#btnSaveProduct").on("click", function () {

            if (!VerifyObject.isPass()) {
                return;
            }
            Product.savaProduct();
        });

        VerifyObject = Verify.createVerify({
            element: ".verify",
            emptyAttr: "data-empty",
            verifyType: "data-type",
            regText: "data-text"
        });

        //编码是否重复
        $("#productCode").blur(function () {
            var _this = $(this);
            if (_this.val().trim() != "") {
                Global.post("/Products/IsExistsProductCode", {
                    code: _this.val()
                }, function (data) {
                    if (data.Status) {
                        _this.val("");
                        alert("材料编码已存在,请重新输入!");
                        _this.focus();
                    }
                });
            }
        })

        $("#productName").focus();

        //选择材料供应商
        $("#prodiver").autocomplete({
            url: "/Products/GetProviders",
            params: {
                pageIndex: 1,
                pageSize: 5
            },
            width: "180",
            asyncCallback: function (data, response) {
                response($.map(data.items, function (item) {
                    return {
                        text: item.Name + "(联系人：" + item.Contact + ")",
                        name: item.Name,
                        id: item.ProviderID
                    }
                }));
            },
            select: function (item) {

            }
        });

        //更改价格同步子产品
        $("#price").change(function () {
            $(".child-product-table").find(".price,.bigprice").val($("#price").val());
        });

        //快速添加供应商
        $("#createProdiver").click(function () {
            doT.exec("template/products/provider_detail.html", function (template) {
                var html = template([]);
                Easydialog.open({
                    container: {
                        id: "show-model-detail",
                        header: "添加供应商",
                        content: html,
                        yesFn: function () {
                            if (!VerifyObject.isPass("#show-model-detail")) {
                                return false;
                            }
                            var entity = {
                                ProviderID:  "",
                                Name: $("#providerName").val().trim(),
                                Contact: $("#contact").val().trim(),
                                MobileTele: $("#mobiletele").val().trim(),
                                CityCode: CityObject.getCityCode(),
                                Address: $("#address").val().trim(),
                                Remark: $("#description").val().trim()
                            };
                            Global.post("/Products/SavaProviders", { entity: JSON.stringify(entity) }, function (data) {
                                if (data.status) {
                                    $("#prodiver").data("id", data.model.ProviderID);
                                    $("#prodiver").data("name", data.model.Name);
                                    $("#prodiver").find("input").val(data.model.Name);
                                } else {
                                    alert("供应商添加失败！")
                                }
                            });
                        },
                        callback: function () {

                        }
                    }
                });
                VerifyProdiver = Verify.createVerify({
                    element: ".verify",
                    emptyAttr: "data-empty",
                    verifyType: "data-type",
                    regText: "data-text"
                });


                $("#providerName").focus();

                CityObject = City.createCity({
                    elementID: "city"
                });
            });
        });

        //组合子产品
        $(".productsalesattr .attritem").click(function () {
            var bl = false, details = [], isFirst = true;
            $(".productsalesattr").each(function () {
                bl = false;
                var _attr = $(this), attrdetail = details;
                //组合规格
                _attr.find("input:checked").each(function () {
                    bl = true;
                    var _value = $(this);
                    //首个规格
                    if (isFirst) {
                        var model = {};
                        model.ids = _attr.data("id") + ":" + _value.val();
                        model.saleAttr = _attr.data("id");
                        model.attrValue = _value.val();
                        model.names = "[" + _attr.data("text") + ":" + _value.data("text") + "]";
                        model.layer = 1;
                        model.guid = Global.guid();
                        details.push(model);
                    }else {
                        for (var i = 0, j = attrdetail.length; i < j; i++) {
                            if (attrdetail[i].ids.indexOf(_value.data("id")) < 0) {
                                var model = {};
                                model.ids = attrdetail[i].ids + "," + _attr.data("id") + ":" + _value.val();
                                model.saleAttr = attrdetail[i].saleAttr + "," + _attr.data("id");
                                model.attrValue = attrdetail[i].attrValue + "," + _value.val();
                                model.names = attrdetail[i].names + "[" + _attr.data("text") + ":" + _value.data("text") + "]";
                                model.layer = attrdetail[i].layer + 1;
                                model.guid = Global.guid();
                                details.push(model);
                            }
                        }
                    }
                });
                isFirst = false;
            });
            //选择所有属性
            if (bl) {
                var layer = $(".productsalesattr").length, items = [];
                for (var i = 0, j = details.length; i < j; i++) {
                    var model = details[i];
                    if (model.layer == layer) {
                        items.push(model);
                    }
                }
                $(".child-product-li").empty();
                //加载子产品
                doT.exec("template/products/product_child_add_list.html", function (templateFun) {
                    var innerText = templateFun(items);
                    innerText = $(innerText);
                    $(".child-product-li").append(innerText);

                    innerText.find(".price,.bigprice").val($("#price").val());

                    innerText.find(".upload-child-img").each(function () {
                        var _this = $(this);
                        Upload.createUpload({
                            element: "#" + _this.attr("id"),
                            buttonText: "选择图片",
                            className: "",
                            data: { folder: '', action: 'add', oldPath: "" },
                            success: function (data, status) {
                                if (data.Items.length > 0) {
                                    _this.siblings("img").attr("src", data.Items[0]);
                                } else {
                                    alert("只能上传jpg/png/gif类型的图片，且大小不能超过10M！");
                                }
                            }
                        });
                    })

                    //价格必须大于0的数字
                    innerText.find(".price,.bigprice").change(function () {
                        var _this = $(this);
                        if (!_this.val().isDouble() || _this.val() <= 0) {
                            _this.val($("#price").val());
                        }
                    });

                    //绑定启用插件
                    innerText.find(".ico-del").click(function () {
                        var _this = $(this);
                        confirm("确认删除此规格吗？", function () {
                            _this.parents("tr.list-item").remove();
                        })
                    });
                });
            }
        });
    }
    //保存产品
    Product.savaProduct = function () {

        var _self = this, attrlist = "", valuelist = "", attrvaluelist = "";

        if (!$("#prodiver").data("id")) {
            alert("请选择材料供应商!");
            return;
        };
       
        var bl = true;
        $(".product-attr").each(function () {
            var _this = $(this);
            attrlist += _this.data("id") + ",";
            valuelist += _this.find("select").val() + ",";
            attrvaluelist += _this.data("id") + ":" + _this.find("select").val() + ",";
            if (!_this.find("select").val()) {
                bl = false;
            }
        });

        if (!bl) {
            alert("属性尚未设置值!");
            return false;
        }

        var Product = {
            ProductID: _self.ProductID,
            ProductCode: $("#productCode").val().trim(),
            ProductName: $("#productName").val().trim(),
            GeneralName: '',//$("#generalName").val().trim(),
            IsCombineProduct: 0,
            ProdiverID: $("#prodiver").data("id"),
            BigUnitID: $("#smallUnit").val().trim(),//$("#bigUnit").val().trim(),
            SmallUnitID: $("#smallUnit").val().trim(),
            BigSmallMultiple: 1,
            CategoryID: $("#categoryID").val(),
            Status: $("#status").prop("checked") ? 1 : 0,
            IsPublic: $("#isPublic").prop("checked") ? 1 : 0,
            AttrList: attrlist,
            ValueList: valuelist,
            AttrValueList: attrvaluelist,
            CommonPrice: $("#price").val(),//$("#commonprice").val(),
            Price: $("#price").val(),
            Weight: 0,//$("#weight").val(),
            IsNew: 0,//$("#isNew").prop("checked") ? 1 : 0,
            IsRecommend: 0,//$("#isRecommend").prop("checked") ? 1 : 0,
            IsAllow: 1,//$("#isAllow").prop("checked") ? 1 : 0,
            IsAutoSend: 0, //$("#isAutoSend").prop("checked") ? 1 : 0,
            EffectiveDays: 0,//$("#effectiveDays").val(),
            DiscountValue:1,
            ProductImage: _self.ProductImage,
            ShapeCode: $("#shapeCode").val(),
            Description: encodeURI(editor.getContent())
        };

        //快捷添加子产品
        if (!_self.ProductID) {
            var details = [];
            $(".child-product-table .list-item").each(function () {
                var _this = $(this);
                var modelDetail = {
                    DetailsCode: _this.find(".code").val(),
                    ShapeCode: "",
                    ImgS: _this.find("img").attr("src"),
                    SaleAttr: _this.data("attr"),
                    AttrValue: _this.data("value"),
                    SaleAttrValue: _this.data("attrvalue"),
                    Weight: 0,
                    Price: _this.find(".price").val(),
                    BigPrice: _this.find(".price").val(),//(Product.SmallUnitID != Product.BigUnitID ? _this.find(".bigprice").val() : _this.find(".price").val()) * Product.BigSmallMultiple,
                    Remark: _this.find(".remark").val(),
                    Description: _this.data("desc")
                };
                details.push(modelDetail);
            });
            Product.ProductDetails = details;
        }
        Global.post("/Products/SavaProduct", {
            product: JSON.stringify(Product)
        }, function (data) {
            if (data.ID.length > 0) {
                if (_self.type == "11") {
                    confirm("材料添加成功，是否返回选择材料页面？", function () {
                        location.href = "/Customer/ChooseMaterial?id=" + _self.guid + "&tid=" + _self.tid;
                    }, function () {
                        location.href = "/Products/ProductDetail/" + data.ID;
                    });
                    
                } else {
                    location.href = "/Products/ProductDetail/" + data.ID;
                }
            }
        });
    }
    //列表页初始化
    Product.initList = function () {
        var _self = this;
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

        //供应商
        //require.async("dropdown", function () {
        //    Global.post("/Products/GetAllProviders", {}, function (data) {
        //        $("#prodiver").dropdown({
        //            prevText: "供应商-",
        //            defaultText: "全部",
        //            defaultValue: "",
        //            data: data.items,
        //            dataValue: "ProviderID",
        //            dataText: "Name",
        //            width: "180",
        //            onChange: function (data) {
        //                Params.PageIndex = 1;
        //                Params.ProviderID = data.value;
        //                _self.getList();
        //            }
        //        });
        //    })
        //});

        //按时间排序
        $(".orderby-new").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.addClass("hover");
                _this.siblings().removeClass("hover");
                Params.OrderBy = "p.CreateTime desc";
                Params.IsAsc = false;
                Params.PageIndex = 1;
                _self.getList();
            }
        });

        //按销量排序
        $(".orderby-sales").click(function () {
            var _this = $(this);
            $(".orderby-price").find(".xia").removeClass("xia-hover");
            $(".orderby-price").find(".shang").removeClass("shang-hover");
            if (!_this.hasClass("hover")) {
                _this.addClass("hover");
                _this.siblings().removeClass("hover");
                Params.OrderBy = "p.SaleCount desc";
                Params.IsAsc = false;
                Params.PageIndex = 1;
                _self.getList();
            }
        });

        //按价格排序
        $(".orderby-price").click(function () {
            var _this = $(this);

            if (!_this.hasClass("hover")) {
                _this.addClass("hover");
                _this.siblings().removeClass("hover");
                Params.IsAsc = true;
                Params.PageIndex = 1;
            } else {
                Params.IsAsc = !Params.IsAsc;
            }
            if (Params.IsAsc) {
                _this.find(".shang").addClass("shang-hover");
                _this.find(".xia").removeClass("xia-hover");
                Params.OrderBy = "p.Price";
            } else {
                _this.find(".shang").removeClass("shang-hover");
                _this.find(".xia").addClass("xia-hover");
                Params.OrderBy = "p.Price desc";
            }
            _self.getList();
        });
    }
    //获取产品列表
    Product.getList = function () {
        var _self = this;
        $("#product-items").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='9'><div class='data-loading'><div></td></tr>");

        Global.post("/Products/GetProductList", { filter: JSON.stringify(Params) }, function (data) {
            $("#product-items").nextAll().remove();

            if (data.Items.length > 0) {
                doT.exec("template/products/products.html", function (templateFun) {
                    var innerText = templateFun(data.Items);
                    innerText = $(innerText);
                    $("#product-items").after(innerText);

                    //绑定启用插件
                    innerText.find(".status").switch({
                        open_title: "点击上架",
                        close_title: "点击下架",
                        value_key: "value",
                        change: function (data, callback) {
                            _self.editStatus(data, data.data("id"), data.data("value"), callback);
                        }
                    });
                    innerText.find(".isnew").switch({
                        open_title: "设为新品",
                        close_title: "取消新品",
                        value_key: "value",
                        change: function (data, callback) {
                            _self.editIsNew(data, data.data("id"), data.data("value"), callback);
                        }
                    });
                    innerText.find(".isrecommend").switch({
                        open_title: "点击推荐",
                        close_title: "取消推荐",
                        value_key: "value",
                        change: function (data, callback) {
                            _self.editIsRecommend(data, data.data("id"), data.data("value"), callback);
                        }
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
                $(".tr-header").after("<tr><td colspan='9'><div class='nodata-txt' >暂无数据!<div></td></tr>");
            }
            

        });
    }
    //更改产品状态
    Product.editStatus = function (obj, id, status, callback) {
        var _self = this;
        Global.post("/Products/UpdateProductStatus", {
            productid: id,
            status: status ? 0 : 1
        }, function (data) {
            !!callback && callback(data.Status);
        });
    }
    //更改产品是否新品
    Product.editIsNew = function (obj, id, status, callback) {
        var _self = this;
        Global.post("/Products/UpdateProductIsNew", {
            productid: id,
            isnew: !status
        }, function (data) {
            !!callback && callback(data.Status);
        });
    }
    //更改产品是否推荐
    Product.editIsRecommend = function (obj, id, status, callback) {
        var _self = this;
        Global.post("/Products/UpdateProductIsRecommend", {
            productid: id,
            isRecommend: !status
        }, function (data) {
            !!callback && callback(data.Status);
        });
    }
    //初始化编辑页数据
    Product.initEdit = function (model, Editor) {
        var _self = this;
        editor = Editor;
        model = JSON.parse(model.replace(/&quot;/g, '"'));
        _self.bindDetailEvent(model);
        _self.bindDetail(model);
        _self.getChildList(model);
    }
    //获取详细信息
    Product.bindDetail = function (model) {
        var _self = this;
        _self.ProductID = model.ProductID;
        $("#productName").val(model.ProductName);
        $("#productCode").val(model.ProductCode);
        //$("#generalName").val(model.GeneralName);
        $("#shapeCode").val(model.ShapeCode);

        //截取绑定属性值
        var list = model.AttrValueList.split(',');
        for (var i = 0, j = list.length; i < j; i++) {
            $("#" + list[i].split(':')[0]).val(list[i].split(':')[1]);
        }

        $("#prodiver").val(model.ProdiverID);
        $("#smallUnit").val(model.SmallUnitID);
        //$("#bigUnit").val(model.BigUnitID);

        //$("#bigSmallMultiple").val(model.BigSmallMultiple);
        //$("#commonprice").val(model.CommonPrice);
        $("#price").val(model.Price);
        //$("#weight").val(model.Weight);
        //$("#effectiveDays").val(model.EffectiveDays);

        $("#status").prop("checked", model.Status == 1);
        $("#isPublic").prop("checked", (model.IsPublic > 0 && model.IsPublic < 3));
        //$("#isNew").prop("checked", model.IsNew == 1);
        //$("#isRecommend").prop("checked", model.IsRecommend == 1);
        //$("#isAllow").prop("checked", model.IsAllow == 1);
        //$("#isAutoSend").prop("checked", model.IsAutoSend == 1);
        $("#productImg").attr("src", model.ProductImage);
        
        _self.ProductImage = model.ProductImage;
        
        editor.ready(function () {
            editor.setContent(decodeURI(model.Description));
        });
    }
    //详情页事件
    Product.bindDetailEvent = function (model) {
        var _self = this;

        //保存产品信息
        $("#btnSaveProduct").on("click", function () {
            if (!VerifyObject.isPass()) {
                return;
            }
            Product.savaProduct();
        });

        VerifyObject = Verify.createVerify({
            element: ".verify",
            emptyAttr: "data-empty",
            verifyType: "data-type",
            regText: "data-text"
        });
        //编辑图片
        ProductIco = Upload.createUpload({
            element: "#productIco",
            buttonText: "更换图片",
            className: "",
            data: { folder: '', action: 'add', oldPath: model.ProductImage },
            success: function (data, status) {
                if (data.Items.length > 0) {
                    _self.ProductImage = data.Items[0];
                    $("#productImg").attr("src", data.Items[0] + "?" + Global.guid());
                } else {
                    alert("只能上传jpg/png/gif类型的图片，且大小不能超过10M！");
                }
            }
        });
        //切换内容
        $(".search-tab li").click(function () {
            var _this = $(this);
            _this.addClass("hover");
            _this.siblings().removeClass("hover");
            $("#productinfo").hide();
            $("#childproduct").hide();
            $("#" + _this.data("id") + "").removeClass("hide").show();
        });

        $("#addDetails").on("click", function () {
            $(".search-tab li").eq(0).removeClass("hover");
            $(".search-tab li").eq(1).addClass("hover");
            $("#productinfo").hide();
            $("#childproduct").removeClass("hide").show();
            _self.showTemplate(model, "");
        });

        //选择材料供应商
        $("#prodiver").autocomplete({
            defaultText: model.Providers ? model.Providers.Name : "",
            defaultValue: model.ProdiverID,
            url: "/Products/GetProviders",
            params: {
                pageIndex: 1,
                pageSize: 5
            },
            width: "180",
            asyncCallback: function (data, response) {
                response($.map(data.items, function (item) {
                    return {
                        text: item.Name + "(联系人：" + item.Contact + ")",
                        name: item.Name,
                        id: item.ProviderID
                    }
                }));
            },
            select: function (item) {

            }
        });
    }
    //子产品列表
    Product.getChildList = function (model) {
        var _self = this;
        $("#header-items").nextAll().remove();
        doT.exec("template/products/productdetails_list.html", function (templateFun) {
            var innerText = templateFun(model.ProductDetails);
            innerText = $(innerText);
            $("#header-items").after(innerText);

            //绑定启用插件
            innerText.find(".status").switch({
                open_title: "点击启用",
                close_title: "点击禁用",
                value_key: "value",
                change: function (data, callback) {
                    _self.editDetailsStatus(data, data.data("id"), data.data("value"), callback);
                }
            });

            innerText.find(".ico-edit").click(function () {
                _self.showTemplate(model, $(this).data("id"));
            });
        });
    }
    //更改子产品状态
    Product.editDetailsStatus = function (obj, id, status, callback) {
        var _self = this;
        Global.post("/Products/UpdateProductDetailsStatus", {
            productdetailid: id,
            status: status ? 0 : 1
        }, function (data) {
            !!callback && callback(data.Status);
        });
    }
    //添加/编辑子产品
    Product.showTemplate = function (model, id) {
        var _self = this, count = 1;
        doT.exec("template/products/productdetails_add.html", function (templateFun) {

            var html = templateFun(model.Category.SaleAttrs);

            Easydialog.open({
                container: {
                    id: "productdetails-add-div",
                    header: !id ? "添加材料子规格" : "编辑材料子规格",
                    content: html,
                    yesFn: function () {

                        if (!DetailsVerify.isPass()) {
                            return false;
                        }

                        var attrlist = "", valuelist = "", attrvaluelist = "", desc = "";

                        $(".productattr").each(function () {
                            var _this = $(this);
                            attrlist += _this.data("id") + ",";
                            valuelist += _this.find("select").val() + ",";
                            attrvaluelist += _this.data("id") + ":" + _this.find("select").val() + ",";
                            desc += "[" + _this.find("span").html() + _this.find("select option:selected").text() + "]";
                        });

                        var Model = {
                            ProductDetailID: id,
                            ProductID: model.ProductID,
                            DetailsCode: $("#detailsCode").val().trim(),
                            ShapeCode: "",//$("#shapeCode").val().trim(),
                            UnitID: $("#unitid").val(),
                            SaleAttr: attrlist,
                            AttrValue: valuelist,
                            SaleAttrValue: attrvaluelist,
                            Price: $("#detailsPrice").val(),
                            BigPrice: $("#detailsPrice").val(),//(model.SmallUnitID != model.BigUnitID ? $("#bigPrice").val() : $("#detailsPrice").val()) * model.BigSmallMultiple,
                            Weight: 0,
                            ImgS: _self.ImgS,
                            Remark: $("#detailsRemark").val(),
                            Description: desc
                        };
                        Global.post("/Products/SavaProductDetail", {
                            product: JSON.stringify(Model)
                        }, function (data) {
                            if (data.ID.length > 0) {
                                Global.post("/Products/GetProductByID", {
                                    productid: model.ProductID,
                                }, function (data) {
                                    _self.getChildList(data.Item);
                                });
                            }
                        });
                    },
                    callback: function () {

                    }
                }
            });

            //绑定单位
            $("#unitName").text(model.SmallUnit.UnitName)
            if (model.SmallUnitID != model.BigUnitID) {
                $("#bigName").text(model.BigUnit.UnitName);
                $("#bigquantity").text(model.BigSmallMultiple);
            } else {
                $("#bigpriceli").hide();
            }

            if (!id) {
                $("#detailsPrice").val(model.Price);
                $("#bigPrice").val(model.Price);
            } else {
                var detailsModel;
                for (var i = 0, j = model.ProductDetails.length; i < j; i++) {
                    if (id == model.ProductDetails[i].ProductDetailID) {
                        detailsModel = model.ProductDetails[i];
                    }
                }
                $("#detailsPrice").val(detailsModel.Price);
                $("#bigPrice").val(detailsModel.BigPrice / model.BigSmallMultiple);
                $("#detailsCode").val(detailsModel.DetailsCode);
                _self.ImgS = detailsModel.ImgS;
                $("#imgS").attr("src", detailsModel.ImgS);
                $("#detailsRemark").val(detailsModel.Remark);
                var list = detailsModel.SaleAttrValue.split(',');
                for (var i = 0, j = list.length; i < j; i++) {
                    $("#" + list[i].split(':')[0]).val(list[i].split(':')[1]).prop("disabled", true);
                }

            }

            ImgsIco = Upload.createUpload({
                element: "#imgSIco",
                buttonText: "选择图片",
                className: "",
                data: { folder: '/Content/tempfile/', action: 'add', oldPath: _self.ImgS },
                success: function (data, status) {
                    if (data.Items.length > 0) {
                        _self.ImgS = data.Items[0];
                        $("#imgS").attr("src", data.Items[0] + "?" + count++);
                    } else {
                        alert("只能上传jpg/png/gif类型的图片，且大小不能超过10M！");
                    }
                }
            });

            DetailsVerify = Verify.createVerify({
                element: ".verify",
                emptyAttr: "data-empty",
                verifyType: "data-type",
                regText: "data-text"
            });
        });
    }

    module.exports = Product;
})