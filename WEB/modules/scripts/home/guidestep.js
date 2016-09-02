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
        $(".check-category").click(function () {
            var _this = $(this);
            if (_this.find(".checkbox.hover").length == 0) {
                _this.find(".checkbox").addClass("hover");
                _this.parents(".process-box").addClass("hover");
            } else {
                _this.find(".checkbox").removeClass("hover");
                _this.parents(".process-box").removeClass("hover");
            }
        });
        //下一步
        $("#btnSubmit").click(function () {
            if (_self.isPost) {
                alert(Global.againSubmitText);
                return;
            }
            
            if ($(".checkbox.hover").length == 0) {
                alert("请至少选择一种品类", 2);
                return;
            }
            var ids = "", names = "";
            $(".checkbox.hover").each(function () {
                var _this = $(this);
                ids += _this.data("id") + "，";
                names += _this.data("name") + "，";
            });
            ids = ids.substr(0, ids.length - 1);
            confirm("您初始化品类选择的是“" + names.substr(0, names.length - 1) + "”，确认下一步吗？", function () {
                _self.isPost = true;
                Global.post("/Default/SetClientProcess", { ids: ids }, function (data) {
                    _self.isPost = false;
                    location.href = "/Default/Index";
                });
            },"确认", function () {
                _self.isPost = false;
            });
        });
    }

    //分类
    ObjectJS.initCategory = function () {
        var _self = this;

        //选择大分类
        $(".items-header .checkbox").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.addClass("hover");
                $("#" + _this.data("id") + " .checkbox").addClass("hover");
            } else {
                _this.removeClass("hover");
                $("#" + _this.data("id") + " .checkbox").removeClass("hover");
            }
        });
        //选择分类
        $(".child-item .checkbox").click(function () {
            var _this = $(this), pEle = $(".items-header .checkbox[data-id='" + _this.data("pid") + "']");
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
            $(".checkbox.hover").each(function () {
                ids += $(this).data("id") + ",";
            });
            if (ids.length == 0) {
                alert("请选择订单品类", 2);
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