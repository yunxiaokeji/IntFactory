

define(function (require, exports, module) {

    require("jquery");
    require("pager");
    var Global = require("global"),
        Easydialog = require("easydialog"),
        doT = require("dot");

    var Order = {};

    Order.Params = {
        pageIndex: 1,
        pageSize: 20,
        status: -1,
        type: -1,
        beginDate: '',
        endDate: '',
        keyWords: '',
        agentID: '',
        clientID: ''
    };


    //列表初始化
    Order.init = function () {
        Order.bindEvent();
        Order.bindData();
    };

    //绑定事件
    Order.bindEvent = function () {
        //关键字查询
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                Order.Params.pageIndex = 1;
                Order.Params.keyWords = keyWords;
                Order.bindData();
            });
        });
    };

    //搜索
    require.async("dropdown", function () {
        var OrderStatus = [
            {
                ID: "0",
                Name: "未支付"
            },
            {
                ID: "1",
                Name: "已支付"
            },
            {
                ID: "9",
                Name: "已关闭"
            }
        ];
        $("#OrderStatus").dropdown({
            prevText: "订单状态-",
            defaultText: "所有",
            defaultValue: "-1",
            data: OrderStatus,
            dataValue: "ID",
            dataText: "Name",
            width: "120",
            onChange: function (data) {
                $("#clientOrders").nextAll().remove();
                Order.Params.beginDate = $("#orderBeginTime").val();
                Order.Params.endDate = $("#orderEndTime").val();
                Order.Params.pageIndex = 1;
                Order.Params.status = parseInt(data.value);
                Order.bindData();
            }
        });

        var OrderTypes = [
            {
                ID: "4",
                Name: "试用"
            },
            {
                ID: "1",
                Name: "购买系统"
            },
            {
                ID: "2",
                Name: "购买人数"
            },
            {
                ID: "3",
                Name: "续费"
            }
        ];
        $("#OrderTypes").dropdown({
            prevText: "订单类型-",
            defaultText: "所有",
            defaultValue: "-1",
            data: OrderTypes,
            dataValue: "ID",
            dataText: "Name",
            width: "120",
            onChange: function (data) {
                $("#clientOrders").nextAll().remove();
                Order.Params.beginDate = $("#orderBeginTime").val();
                Order.Params.endDate = $("#orderEndTime").val();
                Order.Params.pageIndex = 1;
                Order.Params.type = parseInt(data.value);
                Order.bindData();
            }
        });

        $("#SearchClientOrders").click(function () {
            Order.Params.pageIndex = 1;
            Order.Params.beginDate = $("#orderBeginTime").val();
            Order.Params.endDate = $("#orderEndTime").val();
            Order.bindData();
        });

    });

    //绑定数据
    Order.bindData = function () {
        $(".tr-header").nextAll().remove();

        Global.post("/Client/GetClientOrders", Order.Params, function (data) {
            doT.exec("template/client/agent-orders.html?3", function (templateFun) {
                var innerText = templateFun(data.Items);
                innerText = $(innerText);
                $(".tr-header").after(innerText);
                innerText.find(".deleteOrder").click(function () {
                    if (confirm("确定删除?")) {
                        Global.post("/Client/CloseClientOrder", { id: $(this).data("id") }, function (data) {
                            if (data.Result == 1) {
                                Order.bindData();
                            }else {
                                alert("关闭失败");
                            }
                        });
                    }
                });
                innerText.find(".editOrder").click(function () {
                    var id = $(this).data("id");
                    var amount = $(this).data("amount");
                    var html = "<input type='text' value='" + amount + "' id='txt-orderAmount' />";
                    Easydialog.open({
                        container: {
                            id: "show-model-feedback",
                            header: "修改订单支付金额",
                            content: html,
                            yesFn: function () {
                                if (confirm("确定修改价格吗?")) {
                                    Global.post("/Client/UpdateOrderAmount", { id: id, amount: $("#txt-orderAmount").val() }, function (data) {
                                        if (data.Result == 1) {
                                            Order.bindData();
                                        }else {
                                            alert("修改失败");
                                        }
                                    });
                                }
                            },
                            callback: function () {
                            }
                        }
                    });
                });
                innerText.find(".examineOrder").click(function () {
                    if (confirm("确定审核通过吗?")) {
                        alert($(this).data("agentid"));
                        Global.post("/Client/PayOrderAndAuthorizeClient", { id: $(this).data("id"), agentID: $(this).data("agentid") }, function (data) {
                            if (data.Result == 1) {
                                Order.bindData();
                            }else {
                                alert("审核失败");
                            }
                        });
                    }
                });
            });

            $("#pager").paginate({
                total_count: data.TotalCount,
                count: data.PageCount,
                start: Order.Params.pageIndex,
                display: 5,
                border: true,
                rotate: true,
                images: false,
                mouse: 'slide',
                onChange: function (page) {
                    Order.Params.pageIndex = page;
                    Order.bindData();
                }
            });
        });
    }

    module.exports = Order;
});