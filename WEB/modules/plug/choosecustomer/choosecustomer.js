
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

    require("plug/choosecustomer/style.css");

    var PlugJS = function (options) {
        var _this = this;
        _this.setting = $.extend([], _this.default, options);
        _this.init();
    }

    //默认参数
    PlugJS.prototype.default = {
        title:"选择客户", //标题
        isAll: false,
        callback: null   //回调
    };

    PlugJS.prototype.init = function () {

        var _self = this;

        doT.exec("/plug/choosecustomer/choosecustomer.html", function (template) {
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
                                    name: _this.data("name"),
                                    city: _this.data("city"),
                                    mobile: _this.data("mobile"),
                                    address: _this.data("address")
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
                    $(".customerlist-all .customerlist-items").append("<li class='data-loading'></li>");
                    Global.post("/Customer/GetCustomersByKeywords", {
                        keywords: keyWords,
                        isAll: _self.setting.isAll ? 1 : 0
                    }, function (data) {
                        $(".customerlist-all .customerlist-items").empty();
                        if (data.items.length > 0) {
                            doT.exec("/plug/choosecustomer/customers.html", function (template) {
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
                        } else {
                            $(".customerlist-all .customerlist-items").append("<li class='nodata-txt'>找不到客户</li>");
                        }
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