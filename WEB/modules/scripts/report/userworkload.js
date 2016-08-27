
define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot");

    var Params = {
        beginTime: new Date().setMonth(new Date().getMonth() - 1).toString().toDate("yyyy-MM-dd"),
        endTime: Date.now().toString().toDate("yyyy-MM-dd"),
        UserID: "",
        TeamID: ""
    };

    var ObjectJS = {};

    //列表页初始化
    ObjectJS.init = function () {
        var _self = this;

        _self.getList();
        _self.bindEvent();
    }

    //绑定列表页事件
    ObjectJS.bindEvent = function () {
        var _self = this;
    }

    //获取列表
    ObjectJS.getList = function () {
        var _self = this;
        $("#userTotalRPT .tr-header").nextAll().remove();
        $("#userTotalRPT .tr-header").after("<tr><td colspan='8'><div class='data-loading'><div></td></tr>");
        Global.post("/UserReport/GetUserWorkLoad", Params, function (data) {

            $("#userTotalRPT .tr-header").nextAll().remove();

            if (data.items.length > 0) {
                doT.exec("template/report/user-workload.html", function (templateFun) {
                    var innerText = templateFun(data.items);
                    innerText = $(innerText);
                    $("#userTotalRPT .tr-header").after(innerText);
                });
            } else {
                $("#userTotalRPT .tr-header").after("<tr><td colspan='8'><div class='nodata-txt' >暂无数据!<div></td></tr>");
            }
        });
    }
    module.exports = ObjectJS;
})