define(function (require, exports, module) {
    var Global = require("global"),
        City = require("city"), CityObject,
        Verify = require("verify"), VerifyObject;


    var ObjectJS = {};
    //初始化
    ObjectJS.init = function (activityid) {
        var _self = this;
        _self.bindEvent(activityid);
    }

    //绑定事件
    ObjectJS.bindEvent = function (activityid) {
        var _self = this;
        //保存
        $("#btnSave").click(function () {
            if (!VerifyObject.isPass()) {
                return false;
            }
            _self.saveModel(activityid);
        });

        if (activityid) {
            $("#source option[data-code='Source-Activity']").prop("selected", true);
            $("#source").prop("disabled", true);

            Global.post("/Customer/GetActivityBaseInfoByID", { activityid: activityid }, function (data) {
                if (data.model.Name) {
                    $("#activityName").text("活动：" + data.model.Name);
                }
            })
        }

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
                if (_this.data("type") == 1) {
                    $(".company").show();
                } else {
                    $(".company").hide();
                }
            }
        });

        $("#name").focus();
    }

    //保存实体
    ObjectJS.saveModel = function (activityid) {
        var _self = this;

        var model = {
            Name: $("#name").val().trim(),
            Type: $("#companyCustom").hasClass("ico-checked") ? 1 : 0,
            IndustryID: $("#industry").val().trim(),
            ActivityID: activityid,
            SourceID: $("#source").val().trim(),
            Extent: $("#extent").val().trim(),
            CityCode: CityObject.getCityCode(),
            Address: $("#address").val().trim(),
            ContactName: $("#contactName").val().trim(),
            MobilePhone: $("#contactMobile").val().trim(),
            Email: $("#email").val().trim(),
            Jobs: $("#jobs").val().trim(),
            Description: $("#remark").val().trim()
        };
        Global.post("/Customer/SaveCustomer", { entity: JSON.stringify(model) }, function (data) {
            if (data.model.CustomerID) {
                confirm("客户保存成功,是否继续添加客户?", function () {
                    location.href = location.href;
                }, function () {
                    location.href = "/Customer/MyCustomer";
                })
                
            } else {
                alert("网络异常,请稍后重试!");
            }
        });
    }

    module.exports = ObjectJS;
});