
/*
    --选择客户插件--
    --引用
    choosecustomer = require("choosecustomer");
    choosecustomer.create({});
*/
define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Easydialog = require("easydialog");

    require("plug/chooseproduct/style.css");

    var PlugJS = function (options) {
        var _this = this;
        _this.setting = $.extend([], _this.default, options);
        _this.init();
    };

    //默认参数
    PlugJS.prototype.default = {
        title:"选择材料", //标题
        type:3, //1采购 2出库 3报损 4报溢 5调拨
        wareid: "",
        callback: null   //回调
    };

    PlugJS.prototype.init = function () {

        var _self = this, url = "plug/chooseproduct/chooseproductin.html";

        if (_self.setting.type == 3 || _self.setting.type == 5 || _self.setting.type == 7) {
            url = "plug/chooseproduct/chooseproductout.html"
        }
        doT.exec(url, function (template) {
            var innerHtml = template({});

            Easydialog.open({
                container: {
                    id: "choose-product-add",
                    header: _self.setting.title,
                    content: innerHtml,
                    yesFn: function () {
                        var list = [];
                        $(".product-all .product-items .check").each(function () {
                            var _this = $(this);
                            if (_this.hasClass("ico-checked")) {
                                var model = {
                                    pid: _this.data("pid"),
                                    did: _this.data("did"),
                                    batch: _this.data("batch"),
                                    depotid: _this.data("depotid"),
                                    remark: _this.data("remark")
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
        var _self = this, url = "/plug/chooseproduct/productin.html", posturl = "/Products/GetProductDetails", wareid = "";
        if (_self.setting.type == 3 || _self.setting.type == 5 || _self.setting.type == 7) {
            url = "/plug/chooseproduct/productout.html";
            posturl = "/Stock/GetProductsByKeywords";
            wareid = _self.setting.wareid;
        }
        //搜索
        require.async("search", function () {
            $("#chooseproductSearch").searchKeys(function (keyWords) {
                if (true) {
                    $(".product-all .product-items").empty();
                    $(".product-all .product-items").append('<div class="data-loading"></div>');
                    Global.post(posturl, {
                        wareid: wareid,
                        keywords: keyWords
                    }, function (data) {
                        $(".product-all .product-items").empty();
                        if (data.items.length > 0) {
                            doT.exec(url, function (template) {
                                var innerHtml = template(data.items);
                                innerHtml = $(innerHtml);
                                innerHtml.click(function () {
                                    var _this = $(this);
                                    if (!_this.find(".check").hasClass("ico-checked")) {
                                        _this.find(".check").removeClass("ico-check").addClass("ico-checked");
                                    } else {
                                        _this.find(".check").removeClass("ico-checked").addClass("ico-check");
                                    }
                                });
                                $(".product-all .product-items").append(innerHtml);
                            });
                        } else {
                            $(".product-all .product-items").append('<div class="nodata-txt">暂无数据</div>');
                        }
                    });
                } else {
                    $(".product-items").empty();
                }
            });
        });
    }

    exports.create = function (options) {
        return new PlugJS(options);
    }
});