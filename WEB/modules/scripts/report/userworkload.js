
define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        ChooseUser = require("chooseuser"),
        moment = require("moment");
    require("daterangepicker");

    var Params = {
        beginTime: Date.now().toString().toDate("yyyy-MM-01"),
        endTime: Date.now().toString().toDate("yyyy-MM-dd"),
        docType: 11,
        UserID: "",
        TeamID: "",
        SearchType: 1
    };

    var ObjectJS = {};

    //列表页初始化
    ObjectJS.init = function (docType) {
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
            if (Params.SearchType == 1) {
                _self.getList();
            } else {
                _self.getProcessList();
            }
        });

        $("#iptCreateTime").val(Params.beginTime + ' 至 ' + Params.endTime);

        $(".search-reporttype .item").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                Params.SearchType = _this.data("id")

                _this.siblings().removeClass("hover");
                _this.addClass("hover");

                $(".table-list-rpt").hide();
                $("#" + _this.data("nav")).show();

                Params.PageIndex = 1;
                if (Params.SearchType == 1) {
                    _self.getList();
                } else {
                    _self.getProcessList();
                }
            }
        });

        require.async("choosebranch", function () {
            $("#chooseBranch").chooseBranch({
                prevText: "人员-",
                defaultText: "全部",
                defaultValue: "",
                userid: "-1",
                isTeam: true,
                width: "170",
                onChange: function (data) {
                    Params.UserID = data.userid;
                    Params.TeamID = data.teamid;
                    if (Params.SearchType == 1) {
                        _self.getList();
                    } else {
                        _self.getProcessList();
                    }
                }
            });
        });
    }

    //获取列表
    ObjectJS.getList = function () {
        var _self = this;
        $("#userTotalRPT .tr-header").nextAll().remove();
        $("#userTotalRPT .tr-header").after("<tr><td colspan='8'><div class='data-loading'><div></td></tr>");
        Global.post("/Report/GetUserWorkLoad", Params, function (data) {

            $("#userTotalRPT .tr-header").nextAll().remove();

            if (data.items.length > 0) {
                doT.exec("template/report/user-workload.html", function (templateFun) {
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
                $("#userTotalRPT .tr-header").after("<tr><td colspan='8'><div class='nodata-txt' >暂无数据!<div></td></tr>");
            }
        });
    }
    ObjectJS.getProcessList = function () {
        var _self = this;
        $("#userProcessRPT").empty()
        $("#userProcessRPT").append("<tr><td colspan='100'><div class='data-loading'><div></td></tr>");
        Global.post("/Report/GetUserSewnProcess", Params, function (data) {

            var obj = {};
            obj.items = data.items;
            obj.process = data.process;
            $("#userProcessRPT").empty()

            if (data.items.length > 0) {
                doT.exec("template/report/user-sewnprocess-rpt.html", function (templateFun) {
                    var innerText = templateFun(obj);
                    innerText = $(innerText);

                    $("#userProcessRPT").append(innerText);

                    $(".total-item td").each(function () {
                        var _this = $(this), _total = 0;
                        if (_this.data("class")) {
                            innerText.find("." + _this.data("class")).each(function () {
                                _total += $(this).html() * 1;
                            });
                            _this.html(_total.toFixed(2));
                        }
                    });
                });
            } else {
                $("#userProcessRPT").append("<tr><td colspan='100'><div class='nodata-txt' >暂无数据!<div></td></tr>");
            }
        });
    }
    module.exports = ObjectJS;
})