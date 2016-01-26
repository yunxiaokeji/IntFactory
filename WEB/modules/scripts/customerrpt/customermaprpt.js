define(function (require, exports, module) {
    var Global = require("global"),
        ChooseUser = require("chooseuser"),
        ec = require("echarts/echarts");
    require("echarts/chart/pie");
    require("echarts/chart/map");
    var Params = {
        type:1,
        beginTime: "",
        endTime: "",
        UserID: "",
        TeamID: "",
        AgentID: ""
    };

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function () {
        var _self = this;

        _self.customermapChart = ec.init(document.getElementById('customermapRPT'));

        _self.bindEvent();
    }
    ObjectJS.bindEvent = function () {
        var _self = this;

        $(".search-type li").click(function () {
            var _this = $(this);

            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
            }

            Params.type = _this.data("type");

            $(".source-box").hide();
            $("#" + _this.data("id")).show();

            if (!_self.customerindustryRPT) {
                _self.customerindustryRPT = ec.init(document.getElementById('customerindustryRPT'));
            }

            if (Params.type == 1) {
                _self.customermap();
            }
            else if (Params.type == 2) {
                _self.customerindustry();
            }
            else if (Params.type == 3) {
                _self.customerindustry();
            }

        });

        require.async("choosebranch", function () {
            $("#chooseBranch").chooseBranch({
                prevText: "人员-",
                defaultText: "全部",
                defaultValue: "",
                userid: "-1",
                isTeam: true,
                width: "180",
                onChange: function (data) {
                    Params.UserID = data.userid;
                    Params.TeamID = data.teamid;
                    $("#btnSearch").click();
                }
            });
        });

        $("#beginTime").val(new Date().setMonth(new Date().getMonth() - 6).toString().toDate("yyyy-MM-dd"));
        $("#endTime").val(Date.now().toString().toDate("yyyy-MM-dd"));

        Params.beginTime = $("#beginTime").val().trim();
        Params.endTime = $("#endTime").val().trim();

        $("#btnSearch").click(function () {
            Params.beginTime = $("#beginTime").val().trim();
            Params.endTime = $("#endTime").val().trim();

            if (Params.type == 1) {
                _self.customermap();
            }
            else if (Params.type == 2) {
                _self.customerindustry();
            }
            else if (Params.type == 3) {
                _self.customerindustry();
            }
        });

        ObjectJS.customermap();

    }
    //客户地区分布统计
    ObjectJS.customermap = function () {
        var _self = this;
        _self.customermapChart.showLoading({
            text: "数据正在努力加载...",
            x: "center",
            y: "center",
            textStyle: {
                color: "red",
                fontSize: 14
            },
            effect: "spin"
        });

        Global.post("/CustomerRPT/GetCustomerReport", Params, function (data) {
            var maxCount = 0;
            var dataItems = [];
            var selDataItems = [];
            var selNameItems = [];
            var total = 0;
            for (var i = 0; len = data.items.length, i < len; i++)
            {
                maxCount = parseInt(data.items[0].value);
                total += data.items[i].value;
                if (i < 3)
                {
                    selDataItems.push({name: data.items[i].name,value:data.items[i].value });
                    selNameItems.push(data.items[i].name);
                    data.items[i].selected = true;
                }
                dataItems.push(data.items[i]);
            }

            option = {
                title: {
                    text: '客户地区占比',
                    subtext: "合计：" + total + "(100.00%)"
                },
                tooltip: {
                    trigger: 'item'
                },
                legend: {
                    x: 'right',
                    selectedMode: false,
                    data: selNameItems
                },
                dataRange: {
                    orient: 'horizontal',
                    min: 0,
                    max: maxCount,
                    text: ['高', '低'],           // 文本，默认为数值文本
                    splitNumber: 0
                },
                toolbox: {
                    show: true,
                    orient: 'vertical',
                    x: 'right',
                    y: 'center',
                    feature: {
                        mark: { show: true },
                        dataView: { show: true, readOnly: false },
                        restore: { show: true },
                        saveAsImage: { show: true }
                    }
                },
                series: [
                    {
                        name: '客户地区占比',
                        type: 'map',
                        mapType: 'china',
                        mapLocation: {
                            x: 'left'
                        },
                        selectedMode: 'multiple',
                        itemStyle: {
                            normal: { label: { show: true } },
                            emphasis: { label: { show: true } }
                        },
                        data:dataItems
                    },
                    {
                        name: '客户地区分布',
                        type: 'pie',
                        roseType: 'area',
                        tooltip: {
                            trigger: 'item',
                            formatter: "{a} <br/>{b} : {c} ({d}%)"
                        },
                        center: [document.getElementById('customermapRPT').offsetWidth - 250, 225],
                        radius: [30, 120],
                        data:selDataItems,
                        animation: false
                    }
                ]
            };
            var ecConfig = require('echarts/config');
            _self.customermapChart.on(ecConfig.EVENT.MAP_SELECTED, function (param) {
                var selected = param.selected;
                var mapSeries = option.series[0];
                var data = [];
                var legendData = [];
                var name;
                for (var p = 0, len = mapSeries.data.length; p < len; p++) {
                    name = mapSeries.data[p].name;
                    if (selected[name]) {
                        data.push({
                            name: name,
                            value: mapSeries.data[p].value
                        });
                        legendData.push(name);
                    }
                }
                option.legend.data = legendData;
                option.series[1].data = data;
                _self.customermapChart.setOption(option, true);
            })
            _self.customermapChart.hideLoading();
            _self.customermapChart.setOption(option);
        });
    }
 
    //客户行业分布统计
    ObjectJS.customerindustry = function () {
        var _self = this;
        _self.customerindustryRPT.showLoading({
            text: "数据正在努力加载...",
            x: "center",
            y: "center",
            textStyle: {
                color: "red",
                fontSize: 14
            },
            effect: "spin"
        });

        Global.post("/CustomerRPT/GetCustomerReport", Params, function (data) {
            var title = [], items = [],total=0,titleRPT='';

            _self.customerindustryRPT.clear();

            var ExtentArr = ["0-49人", "50-99人", "100-199人", "200-499人", "500-999人", "1000+人"];
            titleRPT = "客户行业占比";
            if (Params.type == 3)
                titleRPT = "客户规模占比";

            for (var i = 0, j = data.items.length; i < j; i++) {
                total += data.items[i].value;
                if (Params.type == 3)
                    data.items[i].name = ExtentArr[parseInt(data.items[i].name) - 1];
            }

            for (var i2 = 0, j2 = data.items.length; i2 < j2; i2++) {
                data.items[i2].name = data.items[i2].name + "：" + data.items[i2].value + "(" + ( (data.items[i2].value / total)*100 ).toFixed(2) + "%)";
                title.push(data.items[i2].name);
                items.push(data.items[i2]);
            }


            option = {
                title: {
                    text: titleRPT,
                    subtext: "合计：" + total + "(100.00%)",
                    x: 'center'
                },
                tooltip: {
                    trigger: 'item',
                    formatter: "{a} <br/>{b} : {c} ({d}%)"
                },
                legend: {
                    orient: 'vertical',
                    x: 'left',
                    data: title
                },
                calculable: true,
                toolbox: {
                    show: true,
                    orient: 'vertical',
                    x: 'right',
                    y: 'center',
                    feature: {
                        mark: { show: true },
                        dataView: { show: true, readOnly: false },
                        restore: { show: true },
                        saveAsImage: { show: true }
                    }
                },
                series: [
                    {
                        name: titleRPT,
                        type: 'pie',
                        radius: '55%',
                        center: ['50%', 245],
                        data: items
                    }
                ]
            };


            _self.customerindustryRPT.hideLoading();
            _self.customerindustryRPT.setOption(option);
            title = []; items = [];

        });
    }

    module.exports = ObjectJS;
});