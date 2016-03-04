define(function (require, exports, module) {
    var Global = require("global");

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function () {
        var _self = this;
        _self.bindEvent();
        _self.getList();
    }
    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;
        $(".ico-check").click(function () {
            var _this = $(this);
            if (!_this.hasClass("ico-checked")) {
                Global.post("/System/UpdateOrderCategory", {
                    categoryid: _this.data("id"),
                    pid: _this.data("pid"),
                    status: 1
                }, function (data) {
                    _this.addClass("ico-checked").removeClass("ico-check");
                });
                

            } else {
                Global.post("/System/UpdateOrderCategory", {
                    categoryid: _this.data("id"),
                    pid: _this.data("pid"),
                    status: 0
                }, function (data) {
                    _this.addClass("ico-check").removeClass("ico-checked");
                });
                
            }
        });
    }

    //获取列表
    ObjectJS.getList = function () {
        var _self = this;
        Global.post("/System/GetOrderCategorys", {}, function (data) {
            for (var i = 0; i < data.items.length; i++) {
                $(".ico-check[data-id='" + data.items[i].CategoryID + "']").addClass("ico-checked").removeClass("ico-check");
            };
           
        });
    }

    module.exports = ObjectJS;
});