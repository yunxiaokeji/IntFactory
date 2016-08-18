define(function (require, exports, module) {
    var Upload = require("upload"), ProductIco, ImgsIco,
        Global = require("global"),
        doT = require("dot"),
        City = require("city"), CityObject,
        Verify = require("verify"), VerifyObject,
        ChooseCustomer = require("choosecustomer"),
        moment = require("moment");
    require("chooseordercategory");
    var ObjectJS = {}, CacheCategory = [], CacheChildCategory = [], CacheItems = [];

    //初始化
    ObjectJS.init = function (customerid, clientid, categoryitem) {
        var _self = this;
        _self.customerid = customerid;
        _self.clientid = clientid;

        if ($(".category-item").length == 1) {
            $(".category-item").addClass("hover");
        }
        if (categoryitem != null) {
            var categoryitems = JSON.parse(categoryitem.replace(/&quot;/g, '"'));
            ObjectJS.categoryitems = categoryitems;
            for (var i = 0; i < categoryitems.length; i++) {
                CacheChildCategory[categoryitems[i].CategoryID] = categoryitems[i].ChildCategory;
            }
            _self.bigCategoryValue = _self.categoryitems[0].CategoryID;
            _self.categoryValue = "";
        }
        $("#chooseOrderCategory").chooseMenu({
            layer: 2,
            url: "/Orders/GetChildCategorysByID",
            data: ObjectJS.categoryitems,
            onHeaderChange: function (data) {
            },
            onCategroyChange:function(data){
            }
        });
        if (customerid) {
            Global.post("/Customer/GetCustomerByID", { customerid: customerid }, function (data) {
                if (data.model.CustomerID) {
                    $("#name").val(data.model.Name);
                    $("#contactMobile").val(data.model.MobilePhone);
                    $("#address").val(data.model.Address);
                    _self.bindEvent(data.model.CityCode);
                } else { }
            });
        } else {
            _self.bindEvent('');
        }
    }

    //绑定事件
    ObjectJS.bindEvent = function (citycode) {
        var _self = this;
        //保存
        $("#btnSave").click(function () {
            if (!VerifyObject.isPass()) {
                return false;
            }

            if ($(".category-item.hover").length != 1) {
                alert("请选择加工品类");
                return;
            }

            _self.saveModel();
        });
        
        //大品类下拉
        require.async("dropdown", function () {
            $(".bigcategory").dropdown({
                prevText: "",
                defaultText: _self.categoryitems[0].CategoryName,
                defaultValue: _self.categoryitems[0].CategoryID,
                data: _self.categoryitems,
                dataValue: "CategoryID",
                dataText: "CategoryName",
                width: 78,
                onChange: function (data) {
                    ObjectJS.bigCategoryValue = data.value;
                    ObjectJS.bindCategory(data);
                }
            });
        });

        ObjectJS.bindCategory({ value: _self.categoryitems[0].CategoryID });

        //切换类型
        $(".ico-radiobox").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                $(".ico-radiobox").removeClass("hover");
                _this.addClass("hover");

                _self.showAttrForOrder();
            }
        });

        //切换品类
        $(".category-item").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                $(".category-item").removeClass("hover");
                _this.addClass("hover");
            }
        });

        var uploader = Upload.uploader({
            browse_button: 'productIco',
            file_path: "/Content/UploadFiles/Order/",
            picture_container: "orderImages",
            image_view: "?imageView2/1/w/120/h/80",//缩略图大小
            maxQuantity: 5,
            maxSize: 5,
            successItems: '#orderImages li',
            fileType: 1,
            init: {}
        });

        VerifyObject = Verify.createVerify({
            element: ".verify",
            emptyAttr: "data-empty",
            verifyType: "data-type",
            regText: "data-text"
        });

        CityObject = City.createCity({
            cityCode: citycode,
            elementID: "city"
        });

        //更换客户
        $("#changeCustomer").click(function () {
            ChooseCustomer.create({
                title: "选择客户",
                isAll: true,
                callback: function (items) {
                    if (items.length > 0) {
                        $("#name").val(items[0].name);
                        $("#contactMobile").val(items[0].mobile);
                        $("#address").val(items[0].address);
                        if (items[0].city) {
                            CityObject.setValue(items[0].city + "");
                        }
                    }
                }
            });
        });
    }

    //确认下单明细
    ObjectJS.showAttrForOrder = function () {
        var _self = this;
        $(".productsalesattr").remove();
        $("#childGoodsQuantity").empty();
        CacheItems = [];
        if ($(".ico-radiobox.hover").data('type') == 2) {
            doT.exec("template/orders/createorder-checkattr.html", function (template) {
                var innerhtml = template(CacheCategory[_self.categoryValue.trim()]);
                innerhtml = $(innerhtml);

                //自定义产品
                innerhtml.find('.change-attr').change(function () {
                    var _this = $(this);
                    var isContinue = true;
                    _this.parents('.attr-box').find('.check-box').each(function () {
                        if (!_this.val().trim()) {
                            alert("自定义规格不能为空");
                            isContinue = false;
                            return false;
                        }
                        if (_this.val().trim() == $(this).text().trim()) {
                            alert("该规格已存在");
                            isContinue = false;
                            return false;
                        }
                    });
                    if (isContinue) {
                        var checkBoxHtml = $('<span class="mRight10 hand check-box"><span class="checkbox iconfont mTop3" data-attrid="' + _this.data('id') + '"  data-type="' + _this.data('type') + '" data-text="' + _this.val().trim() + '" data-id="|"></span> ' + _this.val().trim() + '</span>');
                        checkBoxHtml.click(function () {
                            var _this = $(this).find(".checkbox");
                            if (_this.hasClass("hover")) {
                                _this.removeClass("hover");
                            } else {
                                _this.addClass("hover");
                            }

                            var bl = false, details = [], isFirst = true, xattr = [], yattr = [];
                            $(".productsalesattr").each(function () {
                                bl = false;
                                var _attr = $(this), attrdetail = details;
                                //组合规格
                                _attr.find(".checkbox.hover").each(function () {
                                    bl = true;
                                    var _value = $(this);
                                    //首个规格
                                    if (isFirst) {
                                        var model = {};
                                        model.ids = _attr.data("id") + ":" + _value.data("id");
                                        model.saleAttr = _attr.data("id");
                                        model.attrValue = _value.data("id");
                                        model.xRemark = _value.data("type") == 1 ? ("【" + _value.data("text") + "】") : "";
                                        model.yRemark = _value.data("type") == 2 ? ("【" + _value.data("text") + "】") : "";
                                        model.xyRemark = "【" + _value.data("text") + "】";
                                        model.names = "【" + _attr.data("text") + "：" + _value.data("text") + "】";
                                        model.layer = 1;
                                        details.push(model);

                                    } else {
                                        for (var i = 0, j = attrdetail.length; i < j; i++) {
                                            if (attrdetail[i].ids.indexOf(_value.data("attrid")) < 0) {
                                                var model = {};
                                                model.ids = attrdetail[i].ids + "," + _attr.data("id") + ":" + _value.data("id");
                                                model.saleAttr = attrdetail[i].saleAttr + "," + _attr.data("id");
                                                model.attrValue = attrdetail[i].attrValue + "," + _value.data("id");
                                                model.xRemark = attrdetail[i].xRemark + (_value.data("type") == 1 ? ("【" + _value.data("text") + "】") : "");
                                                model.yRemark = attrdetail[i].yRemark + (_value.data("type") == 2 ? ("【" + _value.data("text") + "】") : "");
                                                model.xyRemark = attrdetail[i].xyRemark + "【" + _value.data("text") + "】";
                                                model.names = attrdetail[i].names + "【" + _attr.data("text") + "：" + _value.data("text") + "】";
                                                model.layer = attrdetail[i].layer + 1;
                                                details.push(model);
                                            }
                                        }
                                    }
                                    //处理二维表
                                    if (_value.data("type") == 1 && xattr.indexOf("【" + _value.data("text") + "】") < 0) {
                                        xattr.push("【" + _value.data("text") + "】");
                                    } else if (_value.data("type") == 2 && yattr.indexOf("【" + _value.data("text") + "】") < 0) {
                                        yattr.push("【" + _value.data("text") + "】");
                                    }

                                });
                                isFirst = false;
                            });
                            $("#childGoodsQuantity").empty();
                            //选择所有属性
                            if (bl) {
                                var layer = $(".productsalesattr").length, items = [];
                                for (var i = 0, j = details.length; i < j; i++) {
                                    var model = details[i];
                                    if (model.layer == layer) {
                                        items.push(model);
                                        CacheItems[model.xyRemark] = model;
                                    }
                                }
                                var tableModel = {};
                                tableModel.xAttr = xattr;
                                tableModel.yAttr = yattr;
                                tableModel.items = items;

                                //加载子产品
                                doT.exec("template/orders/orders_child_list.html", function (templateFun) {
                                    var innerText = templateFun(tableModel);
                                    innerText = $(innerText);
                                    $("#childGoodsQuantity").append(innerText);
                                    //数量必须大于0的数字
                                    innerText.find(".quantity").change(function () {
                                        var _this = $(this);
                                        if (!_this.val().isInt() || _this.val() <= 0) {
                                            _this.val("0");
                                        }

                                        var total = 0;
                                        $(".child-product-table .tr-item").each(function () {
                                            var _tr = $(this), totaly = 0;
                                            if (!_tr.hasClass("total")) {
                                                _tr.find(".quantity").each(function () {
                                                    var _this = $(this);
                                                    if (_this.val() > 0) {
                                                        totaly += _this.val() * 1;
                                                    }
                                                });
                                                _tr.find(".total-y").text(totaly);
                                            } else {
                                                _tr.find(".total-y").each(function () {
                                                    var _td = $(this), totalx = 0;
                                                    $(".child-product-table .quantity[data-x='" + _td.data("x") + "']").each(function () {
                                                        var _this = $(this);
                                                        if (_this.val() > 0) {
                                                            totalx += _this.val() * 1;
                                                        }
                                                    });
                                                    total += totalx;
                                                    _td.text(totalx);
                                                });
                                                _tr.find(".total-xy").text(total);
                                            }
                                        });
                                    });
                                });
                            }
                        });
                        _this.parent().before(checkBoxHtml);
                        checkBoxHtml.click();
                        $("#checkOrderType").after(innerhtml);
                        _this.val('');
                    }
                });

                //组合产品
                innerhtml.find(".check-box").click(function () {
                    var _this = $(this).find(".checkbox");
                    if (_this.hasClass("hover")) {
                        _this.removeClass("hover");
                    } else {
                        _this.addClass("hover");
                    }

                    var bl = false, details = [], isFirst = true, xattr = [], yattr = [];
                    $(".productsalesattr").each(function () {
                        bl = false;
                        var _attr = $(this), attrdetail = details;
                        //组合规格
                        _attr.find(".checkbox.hover").each(function () {
                            bl = true;
                            var _value = $(this);
                            //首个规格
                            if (isFirst) {
                                var model = {};
                                model.ids = _attr.data("id") + ":" + _value.data("id");
                                model.saleAttr = _attr.data("id");
                                model.attrValue = _value.data("id");
                                model.xRemark = _value.data("type") == 1 ? ("【" + _value.data("text") + "】") : "";
                                model.yRemark = _value.data("type") == 2 ? ("【" + _value.data("text") + "】") : "";
                                model.xyRemark = "【" + _value.data("text") + "】";
                                model.names = "【" + _attr.data("text") + "：" + _value.data("text") + "】";
                                model.layer = 1;
                                details.push(model);
                                
                            } else {
                                for (var i = 0, j = attrdetail.length; i < j; i++) {
                                    if (attrdetail[i].ids.indexOf(_value.data("attrid")) < 0) {
                                        var model = {};
                                        model.ids = attrdetail[i].ids + "," + _attr.data("id") + ":" + _value.data("id");
                                        model.saleAttr = attrdetail[i].saleAttr + "," + _attr.data("id");
                                        model.attrValue = attrdetail[i].attrValue + "," + _value.data("id");
                                        model.xRemark = attrdetail[i].xRemark + (_value.data("type") == 1 ? ("【" + _value.data("text") + "】") : "");
                                        model.yRemark = attrdetail[i].yRemark + (_value.data("type") == 2 ? ("【" + _value.data("text") + "】") : "");
                                        model.xyRemark = attrdetail[i].xyRemark + "【" + _value.data("text") + "】";
                                        model.names = attrdetail[i].names + "【" + _attr.data("text") + "：" + _value.data("text") + "】";
                                        model.layer = attrdetail[i].layer + 1;
                                        details.push(model);
                                    }
                                }
                            }
                            //处理二维表
                            if (_value.data("type") == 1 && xattr.indexOf("【" + _value.data("text") + "】") < 0) {
                                xattr.push("【" + _value.data("text") + "】");
                            } else if (_value.data("type") == 2 && yattr.indexOf("【" + _value.data("text") + "】") < 0) {
                                yattr.push("【" + _value.data("text") + "】");
                            }

                        });
                        isFirst = false;
                    });
                    $("#childGoodsQuantity").empty();
                    //选择所有属性
                    if (bl) {
                        var layer = $(".productsalesattr").length, items = [];
                        for (var i = 0, j = details.length; i < j; i++) {
                            var model = details[i];
                            if (model.layer == layer) {
                                items.push(model);
                                CacheItems[model.xyRemark] = model;
                            }
                        }
                        var tableModel = {};
                        tableModel.xAttr = xattr;
                        tableModel.yAttr = yattr;
                        tableModel.items = items;

                        //加载子产品
                        doT.exec("template/orders/orders_child_list.html", function (templateFun) {
                            var innerText = templateFun(tableModel);
                            innerText = $(innerText);
                            $("#childGoodsQuantity").append(innerText);
                            //数量必须大于0的数字
                            innerText.find(".quantity").change(function () {
                                var _this = $(this);
                                if (!_this.val().isInt() || _this.val() <= 0) {
                                    _this.val("0");
                                }

                                var total = 0;
                                $(".child-product-table .tr-item").each(function () {
                                    var _tr = $(this), totaly = 0;
                                    if (!_tr.hasClass("total")) {
                                        _tr.find(".quantity").each(function () {
                                            var _this = $(this);
                                            if (_this.val() > 0) {
                                                totaly += _this.val() * 1;
                                            }
                                        });
                                        _tr.find(".total-y").text(totaly);
                                    } else {
                                        _tr.find(".total-y").each(function () {
                                            var _td = $(this), totalx = 0;
                                            $(".child-product-table .quantity[data-x='" + _td.data("x") + "']").each(function () {
                                                var _this = $(this);
                                                if (_this.val() > 0) {
                                                    totalx += _this.val() * 1;
                                                }
                                            });
                                            total += totalx;
                                            _td.text(totalx);
                                        });
                                        _tr.find(".total-xy").text(total);
                                    }
                                });
                            });
                        });
                    }
                });

                $("#checkOrderType").after(innerhtml);
            });          
        }
    };

    //绑定小品类
    ObjectJS.bindCategory = function () {
        var _self = this;
        var isOnce = true;

        $('.ordercategory').empty();

        var items = CacheChildCategory[_self.bigCategoryValue];

        for (var i = 0; i < items.length; i++) {
            if (!CacheCategory[items[i].CategoryID]) {
                CacheCategory[items[i].CategoryID] = items[i];
            }
        }
        if (isOnce) {
            _self.categoryValue = items[0].CategoryID;
            _self.showAttrForOrder();
            isOnce = false;
        }

        require.async("dropdown", function () {
            $(".ordercategory").dropdown({
                prevText: "",
                defaultText: items[0].CategoryName,
                defaultValue: items[0].CategoryID,
                data: items,
                dataValue: "CategoryID",
                dataText: "CategoryName",
                width: 78,
                onChange: function (data) {
                    _self.categoryValue = data.value;
                    _self.showAttrForOrder();
                }
            });
        });

    }

    //保存实体
    ObjectJS.saveModel = function () {
        var _self = this;
        var images = "";
        $("#orderImages li").each(function () {
            var _this = $(this);            
            images += _this.data("server") + _this.data("filename") + ",";
        });

        var model = {
            CustomerID: _self.customerid,
            PersonName: $("#name").val().trim(),
            OrderType: $(".ico-radiobox.hover").data('type'),
            PlanTime: $("#iptCreateTime").val() == null ? "" : $("#iptCreateTime").val(),
            BigCategoryID: $(".category-item.hover").data("id"),
            CategoryID: _self.categoryValue.trim(),
            CityCode: CityObject.getCityCode(),
            ExpressCode: $("#expressCode").val().trim(),
            Address: $("#address").val().trim(),
            OrderImage: images,
            PlanPrice: $("#planPrice").val().trim(),
            PlanQuantity: 1,
            MobileTele: $("#contactMobile").val().trim(),
            Remark: $("#remark").val().trim(),
            OrderGoods:[]
        };

        //大货单遍历下单明细
        if ($(".ico-radiobox.hover").data('type') == 2) {
            $(".child-product-table .quantity").each(function () {
                var _this = $(this);
                if (_this.val() > 0) {
                    var item = CacheItems[_this.data("remark")];
                    model.OrderGoods.push({
                        SaleAttr: item.saleAttr,
                        AttrValue: item.attrValue,
                        SaleAttrValue: item.ids,
                        Quantity: _this.val(),
                        XRemark: item.xRemark,
                        YRemark: item.yRemark,
                        XYRemark: item.xyRemark,
                        Remark: item.names
                    });
                }
            });
        }

        Global.post("/Orders/CreateOrder", { entity: JSON.stringify(model) }, function (data) {
            if (data.id) {
                location.href = "/Orders/OrderDetail/" + data.id;
            } else {
                alert("网络异常,请稍后重试!");
            }
        });
    }

    module.exports = ObjectJS;
});