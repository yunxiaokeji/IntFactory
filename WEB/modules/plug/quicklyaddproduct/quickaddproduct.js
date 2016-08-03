define(function (require, exports, module) {
    var Verify = require("verify"), VerifyObject;
    var Easydialog = require("easydialog");
    var dot = require("dot");
    require("autocomplete");
    require("menu");
    require("dropdown");
    var Global = require("global");
    var cacheUnit;
    var PlugJS = function (options) {
        var _this = this;
        _this.setting = $.extend([], _this.default, options);
        _this.init();
    }

    //默认参数
    PlugJS.prototype.default = {
        title: "快捷添加材料", //标题
        callback: null   //回调
    };

    PlugJS.prototype.init = function () {
        var _self = this;
        dot.exec("plug/quicklyaddproduct/product_qulickly_add.html", function (template) {
            var innerHtml = template();
            Easydialog.open({
                container: {
                    id: "qulicklyAddMaterial",
                    header: _self.setting.title,
                    content: innerHtml,
                    yesFn: function () {
                        if (!VerifyObject.isPass()) {
                            return false;
                        }
                        if ($(".autocomplete-text").val() && !$("#prodiver").data("id")) {
                            alert("请重新选择材料供应商!");
                            return false;
                        }
                        if (!_self.categoryID && $("#productMenuChange").val()) {
                            alert("材料类别选择有误，请重新选择");
                            return false;
                        }
                        var Product = {
                            ProductID: '',
                            ProductCode: $("#productCode").val().trim(),
                            ProductName: $("#productName").val().trim(),
                            GeneralName: '',//$("#generalName").val().trim(),
                            IsCombineProduct: 0,
                            ProviderID: $("#prodiver").data("id"),
                            UnitID: _self.unitID,//$("#bigUnit").val().trim(),
                            BigSmallMultiple: 1,
                            CategoryID: _self.categoryID ? _self.categoryID : "",
                            Status: 1,
                            IsPublic: 0,
                            AttrList: '',
                            ValueList: '',
                            AttrValueList: '',
                            CommonPrice: $("#layerPrice").val(),//$("#commonprice").val(),
                            Price: $("#layerPrice").val(),
                            Weight: 0,//$("#weight").val(),
                            IsNew: 0,//$("#isNew").prop("checked") ? 1 : 0,
                            IsRecommend: 0,//$("#isRecommend").prop("checked") ? 1 : 0,
                            IsAllow: 1,//$("#isAllow").prop("checked") ? 1 : 0,
                            IsAutoSend: 0, //$("#isAutoSend").prop("checked") ? 1 : 0,
                            EffectiveDays: 0,//$("#effectiveDays").val(),
                            DiscountValue: 1,
                            ProductImage: '',
                            ShapeCode: '',
                            Description: ''
                        };
                        //快捷添加子产品
                        var details = [];
                        var saleAttr = "", attrValue = "", saleAttrValue = "", desc = $("#iptRemark").val().trim(), isNull = false;
                        $(".attr-item").each(function () {
                            var _this = $(this);
                            saleAttr += _this.find('.salesattr').data("id") + ",";
                            attrValue += _this.find(".drop-item").data("id") + ",";
                            saleAttrValue += _this.find('.salesattr').data("id") + ":" + _this.find(".drop-item").data("id") + ",";
                            if (_this.find(".drop-item").data("id") == "|" && !_this.find(".drop-item").parent().next().val()) {
                                isNull = true;
                                alert("请输入自定义规格");
                                return false;
                            }
                        });
                        if (isNull) {
                            return false;
                        }
                        var modelDetail = {
                            DetailsCode: '',
                            ShapeCode: "",
                            ImgS: '',
                            SaleAttr: saleAttr,
                            AttrValue: attrValue,
                            SaleAttrValue: saleAttrValue,
                            Weight: 0,
                            Price: $("#layerPrice").val(),
                            BigPrice: $("#layerPrice").val(),//(Product.SmallUnitID != Product.BigUnitID ? _this.find(".bigprice").val() : _this.find(".price").val()) * Product.BigSmallMultiple,
                            Remark: '',
                            Description: desc
                        };
                        details.push(modelDetail);
                        Product.ProductDetails = details;
                        confirm("材料分类：" + $("#chooseCategory").val() + "，选择后不能更改！", function () {
                            Global.post("/Products/SavaProduct", {
                                product: JSON.stringify(Product)
                            }, function (data) {
                                if (data.result == 1) {
                                    alert("添加成功");
                                    Easydialog.close();
                                } else {
                                    alert("网络异常，请重试");
                                }
                                _self.setting.callback && _self.setting.callback((data));
                            });
                        });
                        return false;
                    }
                }
            });
            _self.bindEvent();
        });
        
    };

    //绑定事件
    PlugJS.prototype.bindEvent = function () {
        var _self = this;
        $("#chooseCategory").chooseMenu({
            isInit: true,
            onCategroyChange: function (items) {
                var id = items[items.length - 1].id;
                _self.categoryID = id;
                if (id) {
                    $(".attr-item").remove();
                    Global.post("/Products/GetCategoryDetailsByID", { categoryid: id }, function (data) {
                        var sales = data.Model.SaleAttrs;
                        if (sales) {
                            var _desc = "";
                            for (var i = 0; i < sales.length; i++) {
                                var sale = sales[i];
                                var html = $("<li class='attr-item'></li>"),
                                    salehtml = $('<span class="width80 salesattr" data-id="' + sale.AttrID + '">' + sale.AttrName + '：</span>'),
                                    drophtml = $('<span class="mLeft3 mRight10"><span class="drop-item" data-id="' + sale.AttrValues[0].ValueID + '" data-name="' + sale.AttrValues[0].ValueName + '" id="dropdown-attr' + sale.AttrID + '"></span></span>'),
                                    customizehtml = $('<input type="text" class="customize width70 mTop2 hide" data-id="' + sale.AttrID + '" placeholder="" />');
                                html.append(salehtml).append(drophtml).append(customizehtml);
                                $("#qulicklyAddMaterial").find('.remark-box').before(html);

                                //自定义文本
                                customizehtml.keyup(function () {
                                    var _descCustomize = "";
                                    $(".attr-item").each(function () {
                                        var _this = $(this);
                                        if (_this.find('.drop-item').data('id') != "|") {
                                            _descCustomize += "【" + _this.find(".salesattr").text() + _this.find('.drop-item').data('name') + "】";
                                        } else {
                                            _descCustomize += "【" + _this.find(".salesattr").text() + _this.find(".drop-item").parent().next().val().trim() + "】";
                                        }
                                    });
                                    $("#iptRemark").val(_descCustomize);
                                });

                                sale.AttrValues.push({
                                    ValueName: "自定义",
                                    ValueID: "|"
                                });
                                var attrValues = sale.AttrValues;
                                /*第一次读取获取描述*/
                                _desc += "【" + sale.AttrName + "：" + attrValues[0].ValueName + "】";
                                $("#dropdown-attr" + sale.AttrID + "").dropdown({
                                    prevText: sale.AttrName + "-",
                                    defaultText: attrValues[0].ValueName,
                                    defaultValue: attrValues[0].ValueID,
                                    data: attrValues,
                                    dataText: "ValueName",
                                    dataValue: "ValueID",
                                    isposition: true,
                                    width: 100,
                                    onChange: function (attrValue) {
                                        var ele = attrValue.element;
                                        if (attrValue.value == '|') {
                                            ele.parent().next().show()
                                        } else {
                                            ele.parent().next().hide();
                                        }
                                        ele.data('name', attrValue.text);
                                        ele.data('id', attrValue.value);
                                        var description = "";
                                        $(".attr-item").each(function () {
                                            var _this = $(this);
                                            if (_this.find('.drop-item').data('id') == '|') {
                                                description += "【" + _this.find('.salesattr').text() + _this.find('.customize').val() + "】";
                                            } else {
                                                description += "【" + _this.find('.salesattr').text() + _this.find('.drop-item').data('name') + "】";
                                            }
                                        });
                                        $("#iptRemark").val(description);
                                    }
                                });
                            }
                            $("#iptRemark").val(_desc);
                            //有规格不能自动输入
                            if ($(".attr-item").length > 0) {
                                $("#iptRemark").prop("disabled", "disabled").css({ "background-color": "#fff", "border": "none" });
                            } else {
                                $("#iptRemark").prop("disabled", false).css({ "background-color": "#fff", "border": "1px solid #ccc" });
                            }
                        } else {
                            $("#qulicklyAddMaterial").find(".attr-item").hide();
                            $("#iptRemark").prop("disabled", "false").css({ "background-color": "#fff", "border": "1px solid #ccc" });
                        }
                    });
                } else {
                    $("#qulicklyAddMaterial").find(".attr-item").hide();
                    $("#iptRemark").prop("disabled", false).css({ "background-color": "#fff", "border": "1px solid #ccc" });
                }
            }
        });
        //选择材料供应商
        $("#prodiver").autocomplete({
            url: "/Products/GetProviders",
            params: {
                pageIndex: 1,
                pageSize: 5
            },
            width: "190",
            isposition: true,
            asyncCallback: function (data, response) {
                response($.map(data.items, function (item) {
                    return {
                        text: item.Name + "(联系人：" + (item.Contact || "--") + ")",
                        name: item.Name,
                        id: item.ProviderID
                    }
                }));
            },
            select: function (item) {

            }
        });

        $(".autocomplete-text").change(function () {
            if ($(this).val().trim() == "") {
                $("#prodiver").data('id', '');
            }
        });

        //编码是否重复
        $("#productCode").change(function () {
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
        });

        //绑定单位
        if (!cacheUnit) {
            Global.post("/Products/GetUnits", null, function (data) {
                cacheUnit = data.items;
                _self.unitID = cacheUnit[0].UnitID;
                $("#Units").dropdown({
                    prevText: "单位-",
                    defaultText: cacheUnit[0].UnitName,
                    defaultValue: cacheUnit[0].UnitID,
                    data: cacheUnit,
                    dataText: "UnitName",
                    dataValue: "UnitID",
                    isposition:true,
                    width: 100,
                    onChange: function (unitData) {
                        _self.unitID = unitData.value;
                    }
                });
            });
        } else {
            _self.unitID = cacheUnit[0].UnitID;
            $("#Units").dropdown({
                prevText: "单位-",
                defaultText: cacheUnit[0].UnitName,
                defaultValue: cacheUnit[0].UnitID,
                data: cacheUnit,
                dataText: "UnitName",
                dataValue: "UnitID",
                isposition: true,
                width: 100,
                onChange: function (unitData) {
                    _self.unitID = unitData.value;
                }
            });
        }
        VerifyObject = Verify.createVerify({
            element: ".verify",
            emptyAttr: "data-empty",
            verifyType: "data-type",
            regText: "data-text"
        });
    }

    exports.create = function (options) {
        return new PlugJS(options);
    };

});