define(function (require, exports, module) {
    var Global = require("global"),
        ec = require("echarts/echarts");
    require("echarts/chart/pie");
    require("echarts/chart/line");
    require("echarts/chart/bar");
    var Params = {
        searchType: "clientsActionRPT",
        dateType: 3,
        beginTime: "",
        endTime: ""
    };

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function () {
        var _self = this;
        _self.clientsChart = ec.init(document.getElementById('clientsActionRPT'));
        _self.bindEvent();
    }
    ObjectJS.bindEvent = function () {
        var _self = this;

        $("#beginTime").val(new Date().setMonth(new Date().getMonth() - 1).toString().toDate("yyyy-MM-dd"));
        $("#endTime").val(Date.now().toString().toDate("yyyy-MM-dd"));

        $(".search-type li").click(function () {
            var _this = $(this);

            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");

                Params.searchType = _this.data("id");
                Params.dateType = _this.data("type");

                $(".source-box").hide();
                $("#" + _this.data("id")).show();

                if (!_self.clientsChart) {
                    _self.clientsChart = ec.init(document.getElementById('clientsActionRPT'));
                }
                if (_this.data("begintime")) {
                    $("#beginTime").val(_this.data("begintime"));
                } else {
                    if (Params.dateType == 3) {
                        $("#beginTime").val(new Date().setFullYear(new Date().getFullYear() - 1).toString().toDate("yyyy-MM-dd"));
                    } else if (Params.dateType == 2) {
                        $("#beginTime").val(new Date().setMonth(new Date().getMonth() - 3).toString().toDate("yyyy-MM-dd"));
                    }
                    else if (Params.dateType == 1) {
                        $("#beginTime").val(new Date().setDate(new Date().getDay() - 15).toString().toDate("yyyy-MM-dd"));
                    }
                }
                if (_this.data("endtime")) {
                    $("#endTime").val(_this.data("endtime"));
                } else {
                    $("#endTime").val(Date.now().toString().toDate("yyyy-MM-dd"));
                }
                $("#btnSearch").click();
            }

        });

        $("#btnSearch").click(function () {
            Params.beginTime = $("#beginTime").val().trim();
            Params.endTime = $("#endTime").val().trim();
            if (!Params.beginTime || !Params.endTime) {
                alert("开始日期与结束日期不能为空！");
                return;
            }
            if (Params.beginTime > Params.endTime) {
                alert("开始日期不能大于结束日期！");
                return;
            }
            _self.sourceDate()
            $(".search-type .hover").data("begintime", Params.beginTime).data("endtime", Params.endTime);
        });
        $("#btnSearch").click();
    }
    //按时间周期
    ObjectJS.sourceDate = function () {
        var _self = this;
        _self.clientsChart.showLoading({
            text: "数据正在努力加载...",
            x: "center",
            y: "center",
            textStyle: {
                color: "red",
                fontSize: 14
            },
            effect: "spin"
        });
        Global.post("/Report/GetClientsAgentActionReport", Params, function (data) {
            var title = [], items = [], datanames = [];
            _self.clientsChart.clear();
            if (data.items.length == 0) {
                _self.clientsChart.hideLoading();
                _self.clientsChart.showLoading({
                    text: "暂无数据",
                    x: "center",
                    y: "center",
                    textStyle: {
                        color: "red",
                        fontSize: 14
                    },
                    effect: "bubble"
                });
                return;
            }
            for (var i = 0, j = data.items.length; i < j; i++) {
                title.push(data.items[i].Name);
                var _items = [];
                for (var ii = 0, jj = data.items[i].Items.length; ii < jj; ii++) {
                    if (i == 0) {
                        datanames.push(data.items[i].Items[ii].Name);
                    }
                    _items.push(data.items[i].Items[ii].Value);
                }
                items.push({
                    name: data.items[i].Name,
                    type: 'line',
                    stack: '总量',
                    data: _items
                });
            }
            option = {
                tooltip: {
                    trigger: 'axis'
                },
                legend: {
                    data: title
                },
                toolbox: {
                    show: true,
                    feature: {
                        dataView: {
                            show: true,
                            readOnly: false,
                            optionToContent: function (opt) {
                                var axisData = opt.xAxis[0].data;
                                var series = opt.series;
                                var table = '<table class="table-list"><tr class="tr-header">'
                                             + '<td>时间</td>';
                                for (var i = 0, l = series.length; i < l; i++) {
                                    table += '<td>' + series[i].name + '</td>'
                                }
                                table += '</tr>';
                                for (var i = 0, l = axisData.length; i < l; i++) {
                                    table += '<tr>'
                                    + '<td class="center">' + axisData[i] + '</td>'
                                    for (var ii = 0, ll = series.length; ii < ll; ii++) {
                                        table += '<td class="center">' +( series[ii].data[i]|| 0) + '</td>';
                                    }

                                    table += '</tr>';
                                }
                                table += '</table>';
                                return table;
                            }
                        },
                        magicType: { show: true, type: ['line', 'bar'] },
                        restore: { show: true },
                        saveAsImage: { show: true }
                    }
                },
                xAxis: [
                    {
                        type: 'category',
                        boundaryGap: false,
                        data: datanames
                    }
                ],
                yAxis: [
                    {
                        type: 'value'
                    }
                ],
                series: items
            };
            _self.clientsChart.hideLoading();
            _self.clientsChart.setOption(option);
        });
    }
    module.exports = ObjectJS;
});