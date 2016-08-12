
/*
    --选择客户插件--
    --引用
    choosecustomer = require("choosecustomer");
    choosecustomer.create({});
*/
define(function (require, exports, module) {
    var $ = require("jquery"),
        Global = require("global"),
        doT = require("dot"),
        Easydialog = require("easydialog");

    require("plug/chooseprocess/style.css");

    var PlugJS = function (options) {
        var _this = this;
        _this.setting = $.extend([], _this.default, options);
        _this.init();
    }

    //默认参数
    PlugJS.prototype.default = {
        title:"选择流程", //标题
        type: 1,
        categoryid: "",
        isClearOther: false,
        callback: null   //回调
    };

    PlugJS.prototype.init = function () {

        var _self = this;

        Global.post("/Orders/GetClientProcessCategory", {}, function (data) {
            doT.exec("/plug/chooseprocess/chooseprocess.html", function (template) {
                var innerHtml = template(data.items);

                Easydialog.open({
                    container: {
                        id: "choose-customer-add",
                        header: _self.setting.title,
                        content: innerHtml,
                        yesFn: function () {
                            var list = [];
                            $(".customerlist-all .customerlist-items .checkbox").each(function () {
                                var _this = $(this);
                                if (_this.hasClass("hover")) {
                                    var model = {
                                        id: _this.data("id"),
                                        name: _this.data("name"),
                                        categoryid: _this.data("categoryid")
                                    };
                                    if (!_self.setting.categoryid || _self.setting.categoryid == _this.data("categoryid")) {
                                        list.push(model);
                                    }
                                }
                            })
                            _self.setting.callback && _self.setting.callback(list);
                        },
                        callback: function () {

                        }
                    }
                });
                //绑定事件
                _self.bindEvent();
            });
        });
    };

    //绑定事件
    PlugJS.prototype.bindEvent = function () {
        var _self = this;
        Global.post("/System/GetOrderProcess", {
            type: _self.setting.type
        }, function (data) {
            $(".customerlist-all .customerlist-items").empty();

            doT.exec("/plug/chooseprocess/process.html", function (template) {
                var innerHtml = template(data.items);
                innerHtml = $(innerHtml);
                innerHtml.click(function () {
                    var _this = $(this).find(".checkbox");
                    if (!_this.hasClass("hover")) {
                        $(".customerlist-all .customerlist-items .checkbox").removeClass("hover");
                        _this.addClass("hover");
                    }
                });
                $(".customerlist-all .customerlist-items").append(innerHtml);

                

                if (_self.setting.isClearOther) {
                    $(".customerlist-all .customerlist-items li[data-id!='" + _self.setting.categoryid + "']").remove();
                    $(".chooseprocess-header .process-category[data-id!='" + _self.setting.categoryid + "']").remove();
                }

                if (_self.setting.categoryid) {
                    $(".chooseprocess-header .process-category[data-id='" + _self.setting.categoryid + "']").click();
                }
            });

            $(".chooseprocess-header .process-category").click(function () {
                var _this = $(this);
                if (!_this.hasClass("hover")) {
                    $(".chooseprocess-header .process-category").removeClass("hover");
                    _this.addClass("hover");

                    $(".customerlist-all .customerlist-items .checkbox").removeClass("hover");
                    $(".customerlist-all .customerlist-items li").hide();
                    $(".customerlist-all .customerlist-items li[data-id='" + _this.data("id") + "']").show();
                }
            });
        });
    }

    exports.create = function (options) {
        return new PlugJS(options);
    }
});