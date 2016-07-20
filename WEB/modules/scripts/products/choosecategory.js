﻿/*
*布局页JS
*/
define(function (require, exports, module) {
    var $ = require("jquery"),
        Global = require("global"),
        doT = require("dot");

    var Category = {
        CategoryID: "",
        PID: ""
    };
    var ObjectJS = {};
    //初始化数据
    ObjectJS.init = function (type, guid, tid) {
        var _self = this;
        _self.type = type;
        _self.guid = guid;
        _self.tid = tid;

        ObjectJS.bindStyle();
        ObjectJS.bindEvent();
    }
    //绑定元素定位和样式
    ObjectJS.bindStyle = function () {
        var _height = document.documentElement.clientHeight - 270;
        $(".category-all").css("height", _height);
        $(".category-list").css("max-height", _height - 100);
    }
    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;
        //调整浏览器窗体
        $(window).resize(function () {
            ObjectJS.bindStyle();
        });
        _self.bindElementEvent($(".category-list li"));
    }
    //元素绑定事件
    ObjectJS.bindElementEvent = function (element) {
        var _self = this;
        ////鼠标悬浮
        //element.mouseover(function () {
        //    var _this = $(this);
        //    _this.find(".add-product").addClass("ico-add").html("");
        //});
        ////鼠标悬浮
        //element.mouseout(function () {
        //    var _this = $(this);
        //    _this.find(".add-product").removeClass("ico-add").html("");
        //});
        //编辑
        //element.find(".add-product").click(function () {
        //    var _this = $(this);
        //    location.href = "/Products/ProductAdd/" + _this.parent().data("id");
        //    return false;
        //});

        //点击
        element.click(function () {
            var _this = $(this);
            _this.siblings().removeClass("hover");
            _this.addClass("hover");
            _this.parents(".category-layer").nextAll().remove();

            if (_this.data("layer") == 3) {
                location.href = "/Products/ProductAdd/?id=" + _this.data("id") + "&type=" + _self.type + "&guid=" + _self.guid + "&tid=" + _self.tid;
                return false;
            }

            Global.post("/Products/GetChildCategorysByID", {
                categoryid: _this.data("id")
            }, function (data) {
                doT.exec("template/products/choose_category.html", function (templateFun) {
                    var html = templateFun(data.Items);
                    html = $(html);
                    //绑定添加事件
                    html.find(".category-header span").html(_this.find(".category-name").html());

                    _self.bindElementEvent(html.find("li"));

                    _this.parents(".category-layer").after(html);
                    _self.bindStyle();
                });
            });
        });
    }
    module.exports = ObjectJS;
})