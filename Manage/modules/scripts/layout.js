/*
*布局页JS
*/
define(function (require, exports, module) {
    var $ = require("jquery"),
        doT = require("dot"),
        Global = require("global");

    var LayoutObject = {};
    //初始化数据
    LayoutObject.init = function () {
        LayoutObject.bindStyle();
        LayoutObject.bindEvent();
    }

    //绑定事件
    LayoutObject.bindEvent = function () {
        var _self = this;
        //调整浏览器窗体
        $(window).resize(function () {
            LayoutObject.bindStyle();
        });

        $(document).click(function (e) {

            if (!$(e.target).parents().hasClass("currentuser") && !$(e.target).hasClass("currentuser")) {
                $(".dropdown-userinfo").fadeOut("1000");
            }

            if (!$(e.target).parents().hasClass("companyname") && !$(e.target).hasClass("companyname")) {
                $(".dropdown-companyinfo").fadeOut("1000");
            }
        });

        //折叠收藏
        $("#choosemodules").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.addClass("hover");
                $(".bottom-body").css("height", "0");
            } else {
                $(".bottom-body").css("height", "40px");
                _this.removeClass("hover");
            }
        });

        $(".controller-box").click(function () {
            var _this = $(this);
            if (!_this.hasClass("select")) {
                _self.setRotateR(_this.find(".open"), 0, 90);
                _this.addClass("select");
                $(this).next().slideUp(200);
            } else {
                _self.setRotateL(_this.find(".open"), 90, 0);
                _this.removeClass("select");
                $(this).next().slideDown(200);
            }
        });

        //登录信息展开
        $("#currentUser").click(function () {
            $(".dropdown-userinfo").fadeIn("1000");
        });


        //一级菜单图标事件处理
        $("#modulesMenu a").mouseenter(function () {
            var _this = $(this).find("img");
            _this.attr("src", _this.data("hover"));
        });

        $("#modulesMenu a").mouseleave(function () {
            if (!$(this).hasClass("select")) {
                var _this = $(this).find("img");
                _this.attr("src", _this.data("ico"));
            }
        });

        $("#modulesMenu .select img").attr("src", $("#modulesMenu .select img").data("hover"));

    }
    //旋转按钮（顺时针）
    LayoutObject.setRotateR = function (obj, i, v) {
        var _self = this;
        if (i < v) {
            i += 3;
            setTimeout(function () {
                obj.css("transform", "rotate(" + i + "deg)");
                _self.setRotateR(obj, i, v);
            }, 5)
        }
    }
    //旋转按钮(逆时针)
    LayoutObject.setRotateL = function (obj, i, v) {
        var _self = this;
        if (i > v) {
            i -= 3;
            setTimeout(function () {
                obj.css("transform", "rotate(" + i + "deg)");
                _self.setRotateL(obj, i, v);
            }, 5)
        }
    }
    //绑定元素定位和样式
    LayoutObject.bindStyle = function () {

    }

    module.exports = LayoutObject;
})