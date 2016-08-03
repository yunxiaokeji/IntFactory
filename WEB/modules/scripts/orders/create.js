define(function (require, exports, module) {
    var Upload = require("upload"), ProductIco, ImgsIco,
        Global = require("global"),
        doT = require("dot"),
        City = require("city"), CityObject,
        Verify = require("verify"), VerifyObject,
        moment = require("moment");

    var ObjectJS = {}, CacheCategory = [];
    //初始化
    ObjectJS.init = function (customerid,clientid,categoryitem) {
        var _self = this;
        _self.customerid = customerid;
        _self.clientid = clientid;

        if ($(".category-item").length == 1) {
            $(".category-item").addClass("hover");
        }

        if (categoryitem != null) {
            var categoryitems = JSON.parse(categoryitem.replace(/&quot;/g, '"'));
            ObjectJS.categoryitems = categoryitems;
            _self.bigCategoryValue = _self.categoryitems[0].CategoryID;
            _self.categoryValue = "";
        }

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
        //切换类型
        $(".customtype").click(function () {
            var _this = $(this);
            if (!_this.hasClass("ico-checked")) {
                $(".customtype").removeClass("ico-checked").addClass("ico-check");
                _this.addClass("ico-checked").removeClass("ico-check");
            }
        });
    }

    //确认下单明细
    ObjectJS.showAttrForOrder = function () {
        var _self = this;
        $(".productsalesattr").remove();
        if ($(".ico-radiobox.hover").data('type') == 2) {
            doT.exec("template/orders/createorder-checkattr.html", function (template) {
                var innerhtml = template(CacheCategory[_self.categoryValue.trim()]);
                innerhtml = $(innerhtml);

                //组合产品
                innerhtml.find(".check-box").click(function () {
                    var _this = $(this).find(".checkbox");
                    if (_this.hasClass("hover")) {
                        _this.removeClass("hover");
                    } else {
                        _this.addClass("hover");
                    }

                    var bl = false, details = [], isFirst = true;
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
                            }
                        }
                        console.log(items);
                        return;
                       
                        //加载子产品
                        doT.exec("template/orders/orders_child_list.html", function (templateFun) {
                            var innerText = templateFun(items);
                            innerText = $(innerText);
                            $("#childGoodsQuantity").append(innerText);
                            //数量必须大于0的数字
                            innerText.find(".quantity").change(function () {
                                var _this = $(this);
                                if (!_this.val().isInt() || _this.val() <= 0) {
                                    _this.val("1");
                                }
                            });
                        });

                    }
                });

                $("#checkOrderType").after(innerhtml);
            });          
        }
    };

    //绑定小品类
    ObjectJS.bindCategory = function (item) {
        var _self = this;
        var isOnce = true;

        $('.ordercategory').empty();
        Global.post("/Home/GetChildOrderCategorysByID", { categoryid: item.value, clientid: _self.clientid }, function (data) {
            var items = data.Items;

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
            Remark: $("#remark").val().trim()
        };
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