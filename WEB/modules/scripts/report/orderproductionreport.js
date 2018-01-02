
define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        ChooseUser = require("chooseuser"),
        moment = require("moment");
    require("daterangepicker");

    var Params = {
        beginTime: Date.now().toString().toDate("yyyy-MM-01"),
        endTime: Date.now().toString().toDate("yyyy-MM-dd"),
        TimeType: 1,
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

        var timeitems = [];
        timeitems.push({ ID: "1", Name: "下单日期" });
        timeitems.push({ ID: "2", Name: "完成日期" });
        require.async("dropdown", function () {
            $("#ddlTimeType").dropdown({
                prevText: "",
                defaultText: "下单日期",
                defaultValue: "1",
                data: timeitems,
                dataValue: "ID",
                dataText: "Name",
                width: "120",
                isposition: true,
                onChange: function (data) {
                    Params.TimeType = data.value;
                    _self.getList();
                }
            });
        });

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

        require.async("choosebranch", function () {
            $("#chooseBranch").chooseBranch({
                prevText: "负责人-",
                defaultText: "全部",
                defaultValue: "",
                userid: "-1",
                isTeam: true,
                width: "170",
                onChange: function (data) {
                    Params.UserID = data.userid;
                    Params.TeamID = data.teamid;
                    _self.getList();
                }
            });
        });

        //关键字搜索
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                Params.keyWords = keyWords;
                _self.getList();
            });
        });
    }

    //获取列表
    ObjectJS.getList = function () {
        var _self = this;
        $("#userTotalRPT .tr-header").nextAll().remove();
        $("#userTotalRPT .tr-header").after("<tr><td colspan='20'><div class='data-loading'><div></td></tr>");
        Global.post("/Report/GetOrderProductionRPT", Params, function (data) {

            $("#userTotalRPT .tr-header").nextAll().remove();

            if (data.items.length > 0) {
                doT.exec("template/report/orderproduction.html", function (templateFun) {
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
                });
            } else {
                $("#userTotalRPT .tr-header").after("<tr><td colspan='208'><div class='nodata-txt' >暂无数据!<div></td></tr>");
            }
        });
    }
    module.exports = ObjectJS;
})