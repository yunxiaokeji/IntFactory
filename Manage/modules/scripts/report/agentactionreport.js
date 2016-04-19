

define(function (require, exports, module) {

    require("jquery");
    var Global = require("global"),
        doT = require("dot");

    var AgentActionReport = {};
   
    AgentActionReport.Params = {
        keyword: "",
        startDate: "",
        endDate:""
    };


    //列表初始化
    AgentActionReport.init = function () {
        AgentActionReport.bindEvent();
        AgentActionReport.bindData();
    };

    //绑定事件
    AgentActionReport.bindEvent = function () {
        //关键字查询
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                AgentActionReport.Params.pageIndex = 1;
                AgentActionReport.Params.keyWords = keyWords;
                AgentActionReport.bindData();
            });
        });
    };

    $("#SearchList").click(function () {
        if ($("#BeginTime").val() != '' || $("#EndTime").val() != '') {
            AgentActionReport.Params.pageIndex = 1;
            AgentActionReport.Params.beginDate = $("#BeginTime").val();
            AgentActionReport.Params.endDate = $("#EndTime").val();
            AgentActionReport.bindData();
        }
    });
    //绑定数据
    AgentActionReport.bindData = function () {
        $(".tr-header").nextAll().remove();

        Global.post("/Report/GetAgentActionReports", AgentActionReport.Params, function (data) {
            doT.exec("template/report/agentactionreport-list.html?3", function (templateFun) {
                var innerText = templateFun(data.Items);
                innerText = $(innerText);
                $(".tr-header").after(innerText);
            });

        });
    }

    module.exports = AgentActionReport;
});