define(function (require, exports, module) {
    var Global = require("global"),
        City = require("city"), CityObject,
        Verify = require("verify"), VerifyObject;


    var ObjectJS = {};
    //初始化
    ObjectJS.initProcess = function () {
        var _self = this;

        //选择流程
        $(".radiobox").click(function () {
            var _this = $(this);
            if (_this.find("hover").length == 0) {
                _this.find(".ico-radiobox").addClass("hover");
                _this.siblings().find(".ico-radiobox").removeClass("hover");

                $(".process-items").hide();
                $(".process-items[data-id='" + _this.find(".ico-radiobox").data("value") + "']").show();
            }
        });
        //下一步
        $("#btnSubmit").click(function () {
            var _this = $(".ico-radiobox.hover");
            confirm("您初始化流程选择的是“" + _this.data("name") + "”，确认下一步吗？", function () {
                Global.post("/Default/SetClientProcess", { type: _this.data("value") }, function (data) {
                    location.href = "/Default/Index";
                });
            });
        });
    }

    module.exports = ObjectJS;
});