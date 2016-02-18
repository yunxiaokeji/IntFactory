define(function (require, exports, module) {
    var Upload = require("upload"), ProductIco, ImgsIco,
        Global = require("global"),
        City = require("city"), CityObject,
        Verify = require("verify"), VerifyObject;


    var ObjectJS = {};
    //初始化
    ObjectJS.init = function () {
        var _self = this;
        _self.bindEvent();
    }

    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;
        //保存
        $("#btnSave").click(function () {
            if (!VerifyObject.isPass()) {
                return false;
            }
            _self.saveModel();
        });

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

        VerifyObject = Verify.createVerify({
            element: ".verify",
            emptyAttr: "data-empty",
            verifyType: "data-type",
            regText: "data-text"
        });
        CityObject = City.createCity({
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

    //保存实体
    ObjectJS.saveModel = function (activityid) {
        var _self = this;

        var model = {
            PersonName: $("#name").val().trim(),
            OrderType: $("#companyCustom").hasClass("ico-checked") ? 1 : 2,
            CategoryID: $("#ordercategory").val().trim(),
            CityCode: CityObject.getCityCode(),
            Address: $("#address").val().trim(),
            OrderImage: _self.ProductImage,
            PlanPrice: $("#planPrice").val().trim(),
            PlanQuantity: $("#planQuantity").val().trim(),
            MobileTele: $("#contactMobile").val().trim(),
            Remark: $("#remark").val().trim()
        };
        Global.post("/Orders/CreateOrder", { entity: JSON.stringify(model) }, function (data) {
            if (data.id) {
                location.href = "/Customer/Orders";
                
            } else {
                alert("网络异常,请稍后重试!");
            }
        });
    }

    module.exports = ObjectJS;
});