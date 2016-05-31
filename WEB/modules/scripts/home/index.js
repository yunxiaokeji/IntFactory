define(function (require, exports, module) {
    var Global = require("global");

    var HeightCount = 0;
    var GuidCount = 0;
    var MinHeight = 0;
    var ObjectJS = {};

    ObjectJS.init = function () {
        ObjectJS.getOrdersByPlanTime();
    };

    ObjectJS.getOrdersByPlanTime = function () {
        Global.post("/Home/GetOrdersByPlanTime", {}, function (data) {
            ObjectJS.bindReport(data);
        });
    }

    ObjectJS.bindReport = function (data) {
        //var items = [];
        //for (var i = 0; i < 10; i++) {
        //    var item = {
        //        Date: "6月"+(i+1)+"日",
        //        ExpireCount: 20+i*10,
        //        FinishCount: 20,
        //        WorkCount: 50+i*8,
        //        ExceedCount: 10 + i * 5,
        //        TotalCount: 100+i*23
        //    };
        //    items.push(item);
        //}

        var MaxTotalCount = 0;
        var items=data.items;
        for (var j = 0; j < items.length; j++) {
            var item=items[j];
            if (item.totalCount > MaxTotalCount) {
                MaxTotalCount = item.totalCount;
            }
        }
        
        HeightCount =300/MaxTotalCount;
        GuidCount = parseInt(MaxTotalCount % 5 == 0 ? MaxTotalCount / 5 : (MaxTotalCount / 5+1) );
        for (var l = 0; l < items.length; l++) {
            ObjectJS.getReportHtml(items[l], l);
        }

        $(".report-guid ul li:not(:last)").css("height", (GuidCount * HeightCount) + "px");
        for (var h = 0; h<5; h++) {
            $(".report-guid ul li").eq(h).find(".guid-count").html( GuidCount * (4 - h) );
        }

        $("#totalSumCount").html(data.totalSumCount);
        $("#totalExceedCount").html(data.totalExceedCount);
        $("#totalFinishCount").html(data.totalFinishCount);
        $("#totalWarnCount").html(data.totalWarnCount);
        $("#totalWorkCount").html(data.totalWorkCount);
        $("#totalSumCount").html(data.totalSumCount);
    }

    ObjectJS.getReportHtml = function (item, index) {
        if (item.totalCount > 0) {
            var html = '';
            html += '<div class="report-item" style="left:' + (75 * index) + 'px">';
            html += '    <ul>';
            if (item.exceedCount > 0) {
                html += '        <li style="line-height:' + (item.exceedCount * HeightCount + MinHeight) + 'px;" class="item-exceed">' + item.exceedCount + '</li>';
            }
            if (item.warnCount > 0) {
                html += '         <li style="line-height:' + (item.warnCount * HeightCount + MinHeight) + 'px;" class="item-expire">' + item.warnCount + '</li>';
            }
            if (item.finishCount > 0) {
                html += '         <li style="line-height:' + (item.finishCount * HeightCount + MinHeight) + 'px;" class="item-finish">' + item.finishCount + '</li>';
            }
            if (item.workCount > 0) {
                html += '        <li style="line-height:' + (item.workCount * HeightCount + MinHeight) + 'px;" class="item-work">' + item.workCount + '</li>';
            }
            html += '    </ul>';
            html += '    <div class="item-date">' + item.date + '</div>';
            html += '</div>';
            html = $(html);
            $(".index-report-content").append(html);
            html.fadeIn(500);
        }
    }

    
    module.exports= ObjectJS;
});