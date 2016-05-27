define(function (require, exports, module) {
    require("echarts/chart/pie");
    require("echarts/chart/line");
    require("echarts/chart/bar");
    require("daterangepicker");

    var Global = require("global"),
        ec = require("echarts/echarts"),
        moment = require("moment");
  
    var Params = {
        searchType: "clientsActionRPT",
        dateType: 1,
        beginTime: new Date().setDate(new Date().getDate() - 15).toString().toDate("yyyy-MM-dd"),
        endTime: Date.now().toString().toDate("yyyy-MM-dd")
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
        //日期插件
        $("#rptBeginTime").daterangepicker({
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
            Params.pageIndex = 1;
            Params.startDate = start ? start.format("YYYY-MM-DD") : '';
            Params.endDate = end ? end.format("YYYY-MM-DD") : '';
            _self.sourceDate();
        });
        $("#rptBeginTime").val(Params.beginTime + ' 至 ' + Params.endTime);
        $(".search-type li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover"); 
                Params.dateType = _this.data("type"); 
                if (!_self.clientsChart) {
                    _self.clientsChart = ec.init(document.getElementById('clientsActionRPT'));
                } 
                if (Params.dateType == 3) {
                    Params.beginTime = new Date().setFullYear(new Date().getFullYear() - 1).toString().toDate("yyyy-MM-dd");
                } else if (Params.dateType == 2) {
                    Params.beginTime = new Date().setMonth(new Date().getMonth() - 3).toString().toDate("yyyy-MM-dd");
                }
                else if (Params.dateType == 1) { 
                    Params.beginTime = new Date().setDate(new Date().getDate() - 15).toString().toDate("yyyy-MM-dd");
                }
                Params.endTime = Date.now().toString().toDate("yyyy-MM-dd");
                $("#rptBeginTime").val(Params.beginTime + ' 至 ' + Params.endTime);
                _self.sourceDate();
            }            
        });
        _self.sourceDate(); 
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