
define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        ChooseUser = require("chooseuser"),
        moment = require("moment");
    require("daterangepicker");

    var Params = {
        beginTime: Date.now().toString().toDate("yyyy-MM-01"),
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

        //日期插件
        $("#iptCreateTime").daterangepicker({
            showDropdowns: true,
            empty: true,
            opens: "right",
            ranges: {
                '今天': [moment(), moment()],
                '昨天': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                '上周': [moment().subtract(6, 'days'), moment()],
                '本月': [moment().startOf('month'), moment().endOf('month')]
            }
        }, function (start, end, label) {
            Params.beginTime = start ? start.format("YYYY-MM-DD") : "";
            Params.endTime = end ? end.format("YYYY-MM-DD") : "";
            _self.getList();
        });

        $("#iptCreateTime").val(Params.beginTime + ' 至 ' + Params.endTime);

    }

    //获取列表
    ObjectJS.getList = function () {
        var _self = this;
        $("#userTotalRPT .tr-header").nextAll().remove();
        $("#userTotalRPT .tr-header").after("<tr><td colspan='8'><div class='data-loading'><div></td></tr>");
        Global.post("/Report/GetCustomerRateRPT", Params, function (data) {

            $("#userTotalRPT .tr-header").nextAll().remove();

            if (data.items.length > 0) {
                doT.exec("template/report/customerrate.html", function (templateFun) {
                    var innerText = templateFun(data.items);
                    innerText = $(innerText);
                    $("#userTotalRPT .tr-header").after(innerText);

                    $(".total-item td").each(function () {
                        var _this = $(this), _total = 0;
                        if (_this.data("class")) {
                            innerText.find("." + _this.data("class")).each(function () {
                                _total += $(this).html() * 1;
                            });
                            _this.html(_total.toFixed(0));
                        }
                    });
                    $(".t4").html(($(".t2").html() / $(".t1").html() * 100).toFixed(2) + "%");
                    $(".t5").html(($(".t3").html() / $(".t1").html() * 100).toFixed(2) + "%");
                });
            } else {
                $("#userTotalRPT .tr-header").after("<tr><td colspan='8'><div class='nodata-txt' >暂无数据!<div></td></tr>");
            }
        });
    }
    module.exports = ObjectJS;
})