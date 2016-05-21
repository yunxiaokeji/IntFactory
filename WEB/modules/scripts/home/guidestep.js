define(function (require, exports, module) {
    var Global = require("global"),
        City = require("city"), CityObject,
        Verify = require("verify"), VerifyObject;


    var ObjectJS = {};
    //初始化
    ObjectJS.initProcess = function () {
        var _self = this;

        _self.isPost = false;

        //选择流程
        $(".radiobox").click(function () {
            var _this = $(this);
            if (_this.find(".hover").length == 0) {
                _this.find(".ico-radiobox").addClass("hover");
                _this.siblings().find(".ico-radiobox").removeClass("hover");

                $(".process-items").hide();
                $(".process-items[data-id='" + _this.find(".ico-radiobox").data("value") + "']").show();
            }
        });
        //下一步
        $("#btnSubmit").click(function () {
            if (_self.isPost) {
                alert(Global.againSubmitText);
                return;
            }
            _self.isPost = true;
            var _this = $(".ico-radiobox.hover");
            confirm("您初始化流程选择的是“" + _this.data("name") + "”，确认下一步吗？", function () {
                Global.post("/Default/SetClientProcess", { type: _this.data("value") }, function (data) {
                    _self.isPost = false;
                    location.href = "/Default/Index";
                });
            }, function () {
                _self.isPost = false;
            });
        });
    }

    //分类
    ObjectJS.initCategory = function () {
        var _self = this;

        //选择大分类
        $(".items-header .ico-checkbox").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.addClass("hover");
                $("#" + _this.data("id") + " .ico-checkbox").addClass("hover");
            } else {
                _this.removeClass("hover");
                $("#" + _this.data("id") + " .ico-checkbox").removeClass("hover");
            }
        });
        //选择分类
        $(".child-item .ico-checkbox").click(function () {
            var _this = $(this), pEle = $(".items-header .ico-checkbox[data-id='" + _this.data("pid") + "']");
            if (!_this.hasClass("hover")) {
                _this.addClass("hover");

                if (!pEle.hasClass("hover")) {
                    pEle.addClass("hover");
                }
            } else {
                _this.removeClass("hover");
                if ($("#" + _this.data("pid") + " .hover").length == 0) {
                    pEle.removeClass("hover");
                }
            }
        });
        //下一步
        $("#btnSubmit").click(function () {
            if (_self.isPost) {
                alert(Global.againSubmitText);
                return;
            }
            var ids = "";
            $(".ico-checkbox.hover").each(function () {
                ids += $(this).data("id") + ",";
            });
            if (ids.length == 0) {
                alert("请选择订单品类");
                return;
            }
            _self.isPost = true;
            confirm("您可在进入系统后，通过“系统”菜单下“系统配置”的“订单品类”重新配置品类，确认下一步吗？", function () {
                Global.post("/Default/SetClientCategory", { ids: ids }, function (data) {
                    _self.isPost = false;
                    location.href = "/Default/SettingHelp";
                });
            }, function () {
                _self.isPost = false;
            });
        });
    }

    module.exports = ObjectJS;
});