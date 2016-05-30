define(function (require, exports, module) {
    var Upload = require("upload"), ProductIco, ImgsIco,
        Global = require("global"),
        City = require("city"), CityObject,
        Verify = require("verify"), VerifyObject,
        moment = require("moment");
        require("daterangepicker");

    var ObjectJS = {}, CacheCategory = [];
    //初始化
    ObjectJS.init = function (customerid) {
        var _self = this;
        _self.customerid = customerid;
        
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
        
        //切换类型
        $(".ico-radiobox").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                $(".ico-radiobox").removeClass("hover");
                _this.addClass("hover");
            }
        });

        //日期插件
        $("#iptCreateTime").daterangepicker({
            showDropdowns: true,
            empty: true,
            opens: "right",
            ranges: {
                '今天': [moment(), moment()],
                '昨天': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                '上周': [moment().subtract(6, 'days'), moment()],
                '本月': [moment().startOf('month'), moment().endOf('month')]
            }
        }, function (start, end, label) {
            _self.beginTime = start ? start.format("YYYY-MM-DD") : "";
            _self.endTime = end ? end.format("YYYY-MM-DD") : "";
        });

        ProductIco = Upload.createUpload({
            element: "#productIco",
            buttonText: "选择图片",
            className: "",
            multiple: true,
            data: { folder: '', action: 'add', oldPath: "" },
            success: function (data, status) {
                if (data.Items.length > 0) {
                    for (var i = 0; i < data.Items.length; i++) {
                        if ($("#orderImages li").length < 5) {
                            var img = $('<li><img src="' + data.Items[i] + '" /><span class="ico-delete"></span> </li>');
                            $("#orderImages").append(img).fadeIn(300);
                            img.find(".ico-delete").click(function () {
                                $(this).parent().remove();
                                if ($("#orderImages li").length == 0) {
                                    $("#orderImages").hide();
                                }
                            });
                        }
                    }
                } else {
                    alert("只能上传jpg/png/gif类型的图片，且大小不能超过5M！");
                }
            }
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

        $("#bigcategory").change(function () {
            var _this = $(this);
            $("#ordercategory").empty();
            if (CacheCategory[_this.val()]) {
                for (var i = 0; i < CacheCategory[_this.val()].length; i++) {
                    $("#ordercategory").append("<option value=" + CacheCategory[_this.val()][i].CategoryID + ">" + CacheCategory[_this.val()][i].CategoryName + "</option>")
                }
            } else {
                Global.post("/Products/GetChildOrderCategorysByID", { categoryid: _this.val() }, function (data) {
                    CacheCategory[_this.val()] = data.Items;
                    for (var i = 0; i < CacheCategory[_this.val()].length; i++) {
                        $("#ordercategory").append("<option value=" + CacheCategory[_this.val()][i].CategoryID + ">" + CacheCategory[_this.val()][i].CategoryName + "</option>")
                    }
                });
            }
        });
        $("#bigcategory").change();
    }

    //保存实体
    ObjectJS.saveModel = function () {
        var _self = this;
        var images = "";
        $("#orderImages img").each(function () {
            images += $(this).attr("src") + ",";
        });

        var model = {
            CustomerID: _self.customerid,
            PersonName: $("#name").val().trim(),
            OrderType: $(".ico-radiobox.hover").data('type'),
            BigCategoryID: $("#bigcategory").val().trim(),
            CategoryID: $("#ordercategory").val().trim(),
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