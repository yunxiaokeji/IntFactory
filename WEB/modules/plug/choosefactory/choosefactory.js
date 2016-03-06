
/*
    --选择客户插件--
    --引用
    choosefactory = require("choosefactory");
    choosefactory.create({});
*/
define(function (require, exports, module) {
    var $ = require("jquery"),
        Global = require("global"),
        doT = require("dot"),
        Easydialog = require("easydialog");

    require("plug/choosefactory/style.css");

    var PlugJS = function (options) {
        var _this = this;
        _this.setting = $.extend([], _this.default, options);
        _this.init();
    }

    //默认参数
    PlugJS.prototype.default = {
        title:"选择工厂", //标题
        callback: null   //回调
    };

    PlugJS.prototype.init = function () {

        var _self = this;

        doT.exec("/plug/choosefactory/choosefactory.html", function (template) {
            var innerHtml = template({});

            Easydialog.open({
                container: {
                    id: "choose-customer-add",
                    header: _self.setting.title,
                    content: innerHtml,
                    yesFn: function () {
                        var list = [];
                        $(".customerlist-all .customerlist-items .check").each(function () {
                            var _this = $(this);
                            if (_this.hasClass("ico-checked")) {
                                var model = {
                                    id: _this.data("id"),
                                    name: _this.data("name")
                                };
                                list.push(model);
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
    };

    //绑定事件
    PlugJS.prototype.bindEvent = function () {
        var _self = this;
        //搜索
        require.async("search", function () {
            $("#choosecustomerSearch").searchKeys(function (keyWords) {
                if (keyWords) {
                    $(".customerlist-all .customerlist-items").empty();
                    Global.post("/Customer/GetClientByKeywords", {
                        keywords: keyWords
                    }, function (data) {
                        $(".customerlist-all .customerlist-items").empty();

                        doT.exec("/plug/choosefactory/factory.html", function (template) {
                            var innerHtml = template(data.items);
                            innerHtml = $(innerHtml);
                            innerHtml.click(function () {
                                var _this = $(this);
                                if (!_this.hasClass("ico-checked")) {
                                    _this.siblings().find(".check").removeClass("ico-checked").addClass("ico-check");
                                    _this.find(".check").removeClass("ico-check").addClass("ico-checked");
                                }
                            });
                            $(".customerlist-all .customerlist-items").append(innerHtml);
                        });
                    });
                    
                } else {
                    $(".customerlist-items").empty();
                }
            });
        });
    }

    exports.create = function (options) {
        return new PlugJS(options);
    }
});