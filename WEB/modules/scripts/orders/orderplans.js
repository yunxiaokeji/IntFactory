define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot");
    require("pager");

    var Params = {
        isMy: false,
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
        require.async("fullcalendar", function () {
            Global.post("/Orders/GetOrderPlans", Params, function (data) {

                if (data.Items.length > 0) {
                    var eventsDatas = [];

                    for (var i = 0; len = data.Items.length, i < len; i++) {
                        var item = data.Items[i];
                        var eventsData = {};
                        eventsData.id = item.OrderID;
                        eventsData.title = item.OrderID;
                        eventsData.start = item.OrderTime.toDate("yyyy-MM-dd");

                        eventsDatas.push(eventsData);
                    }
                   
                    $('#calendar').fullCalendar({
                        header: {
                            left: 'prev,next today',
                            center: 'title',
                            right: 'month,agendaWeek,agendaDay'
                        },
                        defaultDate: (new Date()).toLocaleDateString(),
                        editable: false,
                        eventLimit: true, // allow "more" link when too many events
                        events: eventsDatas,
                        eventClick: function (calEvent, jsEvent, view) {
                            var Easydialog = require("easydialog");
                            var html = '<ul class="">';
                            html +='<li>';
                            html +='<span class="width80">标题：</span>';
                            html +='<span>';
                            html += calEvent.id;
                            html += '</span>';
                            html +='</li>';
                            html += '</ul>';

                            Easydialog.open({
                                container: {
                                    id: "show-order-detail",
                                    header: "订单详情",
                                    content: html,
                                    yesFn: null,
                                    callback: function () {
                                    }
                                }
                            });

                        }
                    });
                }


            });

            

        });

        
    }

    module.exports = ObjectJS;
});