﻿define(function (require, exports, module) {
    var Upload = require("upload"), ProductIco, ImgsIco,
        Global = require("global"),
        City = require("city"), CityObject,
        Verify = require("verify"), VerifyObject,
        moment = require("moment");

    var ObjectJS = {}, CacheCategory = [];
    //初始化
    ObjectJS.init = function (customerid,clientid,categoryitem) {
        var _self = this;
        _self.customerid = customerid;
        _self.clientid = clientid;
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

    //绑定小品类
    ObjectJS.bindCategory = function (item) {
        var _self = this;
        var isOnce = true;

        $('.ordercategory').empty();
        Global.post("/Home/GetChildOrderCategorysByID", { categoryid: item.value, clientid: _self.clientid }, function (data) {
            var items = data.Items;

            if (isOnce) {
                _self.categoryValue = items[0].CategoryID;
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
            BigCategoryID: _self.bigCategoryValue.trim(),
            CategoryID: _self.categoryValue.trim(),
            CityCode: CityObject.getCityCode(),
            ExpressCode: $("#expressCode").val().trim(),
            Address: $("#address").val().trim(),
            OrderImage: images,
            PlanPrice: $("#planPrice").val().trim(),
            PlanQuantity: $("#planQuantity").val().trim(),
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