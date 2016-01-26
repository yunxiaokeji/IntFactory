define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot");
    require("pager");

    var CacheDetails = [];

    var Params = {
        Keywords: "",
        PageIndex: 1,
        PageSize: 20
    };

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function () {
        var _self = this;
        _self.getList();
        _self.bindEvent();
    }

    //绑定事件
    ObjectJS.bindEvent = function (type) {
        var _self = this;

        //关键字搜索
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                Params.PageIndex = 1;
                Params.Keywords = keyWords;
                _self.getList();
            });
        });     
    }

    //获取列表
    ObjectJS.getList = function () {
        var _self = this;
        $(".tr-header").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='8'><div class='dataLoading'><img src='/modules/images/ico-loading.jpg'/><div></td></tr>");

        Global.post("/Stock/GetProductStocks", Params, function (data) {
            _self.bindList(data);
        });
    }
    //加载列表
    ObjectJS.bindList = function (data) {
        var _self = this;
        $(".tr-header").nextAll().remove();
        if (data.items.length > 0) {
            doT.exec("template/stock/stocks.html", function (template) {
                var innerhtml = template(data.items);
                innerhtml = $(innerhtml);

                //展开明细
                innerhtml.find(".dropdown").click(function () {
                    var _this = $(this);
                    if (!_this.data("first") || _this.data("first") == 0) {
                        _this.data("first", 1).data("status", "open");
                        if (CacheDetails[_this.data("id")]) {
                            _self.bindDetails(CacheDetails[_this.data("id")], _this.parent())
                        } else {
                            Global.post("/Stock/GetProductDetailStocks", {
                                productid: _this.data("id")
                            }, function (details) {
                                CacheDetails[_this.data("id")] = details.items;
                                _self.bindDetails(details.items, _this.parent());
                            });
                        }
                    } else {
                        if (_this.data("status") == "open") {
                            _this.data("status", "close");
                            _this.parent().nextAll("tr[data-pid='" + _this.data("id") + "']").hide();
                        } else {
                            _this.data("status", "open");
                            _this.parent().nextAll("tr[data-pid='" + _this.data("id") + "']").show();
                        }
                    }
                });

                $(".tr-header").after(innerhtml);

            });
        }
        else {
            $(".tr-header").after("<tr><td colspan='8'><div class='noDataTxt' >暂无数据!<div></td></tr>");
        }

        $("#pager").paginate({
            total_count: data.totalCount,
            count: data.pageCount,
            start: Params.PageIndex,
            display: 5,
            border: true,
            border_color: '#fff',
            text_color: '#333',
            background_color: '#fff',
            border_hover_color: '#ccc',
            text_hover_color: '#000',
            background_hover_color: '#efefef',
            rotate: true,
            images: false,
            mouse: 'slide',
            onChange: function (page) {
                Params.PageIndex = page;
                _self.getList();
            }
        });
    }

    ObjectJS.bindDetails = function (items, ele) {
        var _self = this;

        doT.exec("template/stock/detailstocks.html", function (template) {
            var innerhtml = template(items);
            innerhtml = $(innerhtml);
            ele.after(innerhtml);
        });
    }

    module.exports = ObjectJS;
});