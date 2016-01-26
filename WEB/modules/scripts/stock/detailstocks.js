define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot");
    require("pager");

    var CacheDetails = [];

    var Params = {
        WareID: "",
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

        Global.post("/System/GetAllWareHouses", {}, function (data) {
            require.async("dropdown", function () {
                $("#wares").dropdown({
                    prevText: "仓库-",
                    defaultText: "全部",
                    defaultValue: "",
                    data: data.items,
                    dataValue: "WareID",
                    dataText: "Name",
                    width: "180",
                    onChange: function (data) {
                        Params.PageIndex = 1;
                        Params.WareID = data.value;
                        _self.getList();
                    }
                });
            });
        });
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
        $(".tr-header").after("<tr><td colspan='9'><div class='dataLoading'><img src='/modules/images/ico-loading.jpg'/><div></td></tr>");

        Global.post("/Stock/GetDetailStocks", Params, function (data) {
            _self.bindList(data);
        });
    }
    //加载列表
    ObjectJS.bindList = function (data) {
        var _self = this;
        $(".tr-header").nextAll().remove();

        if (data.items.length > 0) {
            doT.exec("template/stock/batchstocks.html", function (template) {
                var innerhtml = template(data.items);
                innerhtml = $(innerhtml);

                $(".tr-header").after(innerhtml);

            });
        }
        else {
            $(".tr-header").after("<tr><td colspan='9'><div class='noDataTxt' >暂无数据!<div></td></tr>");
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

    module.exports = ObjectJS;
});