define(function (require,exports,module) {
    var Global = require("global"),
        DoT = null,
        Easydialog = null;

    var Controller = "Task";
        
    var ObjectJS = {};
    ObjectJS.processes = [];

    ObjectJS.initProcessCosts = function (orderID, global, doT, orderType) {
        var _self = ObjectJS;

        if (global == null) {
            Global = require("global");
        }
        else {
            Global = global;
        }
        if (doT == null) {
            DoT = require("dot");
        }
        else {
            DoT = doT;
        }
        if (Easydialog == null) {
            Easydialog = require("easydialog");
        }
        else {
            Easydialog = easydialog;
        }

        _self.orderID = orderID;
        _self.orderType = orderType;

        _self.getCosts();
        //添加成本
        if ($("#addOtherCost").length == 1) {
            Global.post("/System/GetTaskProcess", {}, function (data) {
                _self.processes = data.items;
            });
            $("#addOtherCost").click(function () {
                _self.addOtherCosts();
            })
        }
    }

    //其他成本
    ObjectJS.getCosts = function () {
        var _self = this;
        $("#navCosts .tr-header").nextAll().remove();
        $("#navCosts .tr-header").after("<tr><td colspan='10'><div class='data-loading' ><div></td></tr>");
        Global.post("/Task/GetOrderCosts", {
            orderid: _self.orderID
        }, function (data) {
            $("#navCosts .tr-header").nextAll().remove();
            if (data.items.length > 0) {
                DoT.exec("template/task/task-costs.html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);

                    innerhtml.find(".cost-price").each(function () {
                        $(this).text($(this).text() * $("#navCosts").data("quantity"))
                    });

                    $("#navCosts .tr-header").after(innerhtml);

                    if ($("#addOtherCost").length == 1) {
                        if (_self.orderType == 1) {
                            innerhtml.find(".ico-del").click(function () {
                                var _this = $(this);
                                confirm("删除后不可恢复，确认删除吗？", function () {
                                    Global.post("/Task/DeleteOrderCost", {
                                        orderid: _self.orderID,
                                        autoid: _this.data("id")
                                    }, function (data) {
                                        if (data.status) {
                                            $("#lblCostMoney").text(($("#lblCostMoney").text() - _this.parents("tr").find(".cost-price").text()).toFixed(2));
                                            _this.parents("tr").first().remove();
                                        }
                                    });
                                }, "删除");
                            });
                        }
                    }
                    else {
                        innerhtml.find(".ico-del").remove();
                    }
                });
            } else {
                $("#navCosts .tr-header").after("<tr><td colspan='10'><div class='nodata-txt' >暂无数据!</div></td></tr>");
            }
        });
    }

    //添加成本
    ObjectJS.addOtherCosts = function () {
        var _self = this;
        DoT.exec("template/orders/add-order-cost.html", function (template) {
            var innerText = template();

            Easydialog.open({
                container: {
                    id: "addOrderCostBox",
                    header: "添加成本",
                    content: innerText,
                    yesFn: function () {
                        if (!$("#iptCostPrice").val() || $("#iptCostPrice").val() * 1 <= 0) {
                            alert("价格必须为大于0的数字！", 2);
                            return false;
                        }
                        if (!$("#iptCostDescription").val()) {
                            alert("描述不能为空！", 2);
                            return false;
                        };
                        Global.post("/Task/CreateOrderCost", {
                            orderid: _self.orderID,
                            price: $("#iptCostPrice").val(),
                            processid: $("#taskProcess").data("id"),
                            remark: $("#iptCostDescription").val()
                        }, function (data) {
                            if (data.status) {
                                alert("成本添加成功！");
                                $("#lblCostMoney").text(($("#lblCostMoney").text() * 1 + $("#iptCostPrice").val() * 1).toFixed(2));
                                _self.getCosts();
                            } else {
                                alert("成本添加失败，请刷新页面重试！", 2);
                            }
                        });
                    },
                    callback: function () {

                    }
                }
            });

            //工序选择
            require.async("dropdown", function () {
                $("#taskProcess").dropdown({
                    prevText: "工序-",
                    defaultText: "其他",
                    defaultValue: "",
                    data: _self.processes,
                    dataValue: "ProcessID",
                    dataText: "Name",
                    width: "180",
                    isposition: true,
                    onChange: function (data) {
                        if (data.value && !$("#iptCostDescription").val()) {
                            $("#iptCostDescription").val(data.text);
                        }
                    }
                });
            });

            $("#iptCostPrice").focus();
            $("#iptCostPrice").change(function () {
                var _this = $(this);
                if (!_this.val().isDouble() || _this.val() <= 0) {
                    _this.val("0");
                }
            });
        });
    }

    module.exports = ObjectJS;
})