define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot");
    require("pager");

    var Params = {
        isMy: true,
        beginDate: '',
        endDate: '',
        pageSize: 20,
        pageIndex:1
    };

    var ObjectJS = {};

    ObjectJS.init = function () {
        ObjectJS.bindEvent();

        ObjectJS.getList();
    }

    ObjectJS.bindEvent =function() {}

    ObjectJS.getList = function () {
        $(".tr-header").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='8'><div class='dataLoading'><img src='/modules/images/ico-loading.jpg'/><div></td></tr>");

        Global.post("/Orders/GetOrderPlans", Params, function (data) {
            $(".tr-header").nextAll().remove();

            if (data.Items.length > 0) {
                var eventsDatas = [];
                for (var i = 0; len = data.Items.length, i < len; i++) {
                    var item=data.Items[i];
                    var eventsData = {};
                    eventsData.id = item.OrderID;
                    eventsData.title = item.OrderID;
                    eventsData.start = item.OrderTime;

                    eventsDatas.push(eventsData);
                }

                $('#calendar').fullCalendar({
                    header: {
                        left: 'prev,next today',
                        center: 'title',
                        right: 'month,agendaWeek,agendaDay'
                    },
                    defaultDate: '2016-02-12',
                    editable: false,
                    eventLimit: true, // allow "more" link when too many events
                    events: eventsDatas,
                    eventClick: function (calEvent, jsEvent, view) {

                        alert('Event: ' + calEvent.id + "  " + calEvent.name);
                        //alert('Coordinates: ' + jsEvent.pageX + ',' + jsEvent.pageY);

                        // change the border color just for fun
                        //$(this).css('border-color', 'red');

                    }
                });

            }
            else {
                $(".tr-header").after("<tr><td colspan='8'><div class='noDataTxt' >暂无数据!<div></td></tr>");
            }

        });
    }

    module.exports = ObjectJS;
});