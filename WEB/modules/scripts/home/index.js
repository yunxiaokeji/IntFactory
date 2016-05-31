define(function (require, exports, module) {
    var Global = require("global");

    var HeightCount = 0;
    var GuidCount = 0;
    var MinHeight = 0;
    var ObjectJS = {};

    ObjectJS.init = function () {
        ObjectJS.bindReport();
    };

    ObjectJS.bindReport = function () {
        var items = [];
        for (var i = 0; i < 10; i++) {
            var item = {
                Date: "6月"+(i+1)+"日",
                ExpireCount: 20+i*10,
                FinishCount: 20,
                WorkCount: 50+i*8,
                ExceedCount: 10 + i * 5,
                TotalCount: 100+i*23
            };
            items.push(item);
        }

        var MaxTotalCount = 0;
        for (var j = 0; j < items.length; j++) {
            var item=items[j];
            if (item.TotalCount > MaxTotalCount) {
                MaxTotalCount = item.TotalCount;
            }
        }

        HeightCount =300/MaxTotalCount;
        GuidCount =parseInt( MaxTotalCount / 5);
        for (var l = 0; l < items.length; l++) {
            ObjectJS.getReportHtml(items[l], l);
        }

        for (var h = 0; h<5; h++) {
            $(".report-guid ul li").eq(h).find(".guid-count").html( GuidCount * (4 - h) );
        }

    }

    ObjectJS.getReportHtml = function (item, index) {
        var html = '';
        html += '<div class="report-item" style="left:' + (75 * index) + 'px">';
        html += '    <ul>';
        if (item.ExceedCount > 0) {
            html += '        <li style="line-height:' + (item.ExceedCount * HeightCount + MinHeight) + 'px;" class="item-exceed">' + item.ExceedCount + '</li>';
        }
        if (item.ExpireCount > 0) {
            html += '         <li style="line-height:' + (item.ExpireCount * HeightCount + MinHeight) + 'px;" class="item-expire">' + item.ExpireCount + '</li>';
        }
        if (item.FinishCount > 0) {
            html += '         <li style="line-height:' + (item.FinishCount * HeightCount + MinHeight) + 'px;" class="item-finish">' + item.FinishCount + '</li>';
        }
        if (item.WorkCount > 0) {
            html += '        <li style="line-height:' + (item.WorkCount * HeightCount + MinHeight) + 'px;" class="item-work">' + item.WorkCount + '</li>';
        }
        html += '    </ul>';
        html += '    <div class="item-date">' + item.Date + '</div>';
        html += '</div>';
        html = $(html);
        $(".index-report-content").append(html);
        html.fadeIn(500);
    }

    module.exports= ObjectJS;
});