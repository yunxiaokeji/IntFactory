
define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot");
    require("pager");

    var Params = {
        pageIndex: 1,
        totalCount: 0,
        docID: ""
    };
    var ObjectJS = {};
    //初始化
    ObjectJS.init = function (docID) {
        var _self = this;
        Params.docID = docID;
        _self.bindEvent();
        
    }
    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;
        $(".tab-nav-ul li").click(function () {
            var _this = $(this);
            _this.siblings().removeClass("hover");
            _this.addClass("hover");
            $(".nav-partdiv").hide();
            $("#" + _this.data("id")).show();

            if (_this.data("id") == "navLog" && (!_this.data("first") || _this.data("first") == 0)) {
                _this.data("first", "1");
                _self.getList();
            } 
        });
    }
    //获取单据列表
    ObjectJS.getList = function () {
        var _self = this;
        $(".log-body").empty();
        var url = "/Purchase/GetStorageDocLog",
            template = "template/purchase/storagedocaction.html";
        

        Global.post(url, Params, function (data) {
            doT.exec(template, function (templateFun) {
                var innerText = templateFun(data.Items);
                innerText = $(innerText);
                $(".log-body").append(innerText);

            });
            $("#pager").paginate({
                total_count: data.TotalCount,
                count: data.PageCount,
                start: Params.pageIndex,
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
                float: "left",
                onChange: function (page) {
                    Params.pageIndex = page;
                    _self.getList();
                }
            });
        });
    }

    module.exports = ObjectJS;
})