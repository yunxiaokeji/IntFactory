

define(function (require, exports, module) {

    require("jquery");
    require("pager");
    var Global = require("global"),
        doT = require("dot");

    var AgentActionReport = {};
   
    AgentActionReport.Params = {
        pageIndex: 1,
        pageSize: 15,
        keyword: "",
        startDate: "",
        endDate: "",
        orderBy: "SUM(a.CustomerCount) desc"
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
                AgentActionReport.Params.keyword = keyWords;
                AgentActionReport.bindData();
            });
        });
    };

    $("#SearchList").click(function () {       
        AgentActionReport.Params.pageIndex = 1;
        AgentActionReport.Params.startDate = $("#BeginTime").val();
        AgentActionReport.Params.endDate = $("#EndTime").val();
        AgentActionReport.bindData();
       
    });
    //排序
    $(".td-span").click(function () {
        var _this = $(this);
        if (_this.hasClass("hover")) {
            if (_this.find(".asc").hasClass("hover")) {
                $(".td-span").find(".asc").removeClass("hover");
                $(".td-span").find(".desc").removeClass("hover");
                _this.find(".desc").addClass("hover");
                AgentActionReport.Params.orderBy = _this.data("column") + " desc ";
            } else {
                $(".td-span").find(".desc").removeClass("hover");
                $(".td-span").find(".asc").removeClass("hover");
                _this.find(".asc").addClass("hover");
                AgentActionReport.Params.orderBy = _this.data("column") + " asc ";
            }
        } else {
            $(".td-span").removeClass("hover");
            $(".td-span").find(".desc").removeClass("hover");
            $(".td-span").find(".asc").removeClass("hover");
            _this.addClass("hover");            
            _this.find(".desc").addClass("hover");
            AgentActionReport.Params.orderBy = _this.data("column") + " desc ";
        }
        AgentActionReport.Params.PageIndex = 1;
        AgentActionReport.bindData();
    });

    //绑定数据
    AgentActionReport.bindData = function () {
        $(".tr-header").nextAll().remove();

        Global.post("/Report/GetAgentActionReports", AgentActionReport.Params, function (data) {
            doT.exec("template/report/agentactionreport-list.html?3", function (templateFun) {
                var innerText = "";
                if (data.Items.length == 0) {
                    innerText='<tr><td colspan="11" style="text-align:center;font-size:18px;color:#333;"> 暂无数据 </td></tr>';
                } else {
                    innerText=templateFun(data.Items);
                    innerText = $(innerText);
                }
                $(".tr-header").after(innerText);
            });
            $("#pager").paginate({
                total_count: data.TotalCount,
                count: data.PageCount,
                start: AgentActionReport.Params.pageIndex,
                display: 5,
                border: true,
                rotate: true,
                images: false,
                mouse: 'slide',
                onChange: function (page) {
                    AgentActionReport.Params.pageIndex = page;
                    AgentActionReport.bindData();
                }
            });
        });
    }

    module.exports = AgentActionReport;
});