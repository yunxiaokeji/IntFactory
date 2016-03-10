
define(function (require, exports, module) {
    var City = require("city"),  CityInvoice,
        Verify = require("verify"), VerifyInvoice,
        Global = require("global"),
        doT = require("dot"),
        ChooseUser = require("chooseuser"),
        ChooseFactory = require("choosefactory"),
        ChooseOrder = require("chooseorder"),
        ChooseProcess = require("chooseprocess"),
        Easydialog = require("easydialog");
    require("pager");
    var ObjectJS = {};

    ObjectJS.init = function (orderid, status, model) {
        var _self = this;
        _self.orderid = orderid;
        _self.status = status;
        _self.mark = 1;
        _self.model = JSON.parse(model.replace(/&quot;/g, '"'));
        _self.bindEvent();
        _self.getAmount();

        _self.bindStyle(_self.model);
        _self.initTalkReply();
    }

    ObjectJS.bindStyle = function (model) {

        var _self = this;
        var stages = $(".stage-items"), width = stages.width();

        stages.find("li .leftbg").first().removeClass("leftbg");
        stages.find("li .rightbg").last().removeClass("rightbg");
        stages.find("li,a").width(width / stages.find("li").length - 15);

        //转移工厂按钮
        if (_self.status != 0 || !!model.EntrustClientID) {
            $("#btnchangeclient").hide();
            $("#btndelete").hide();
        } else {
            //转移工厂
            $("#btnchangeclient").click(function () {
                var _this = $(this);
                ChooseFactory.create({
                    title: "订单委托-选择工厂",
                    callback: function (items) {
                        if (items.length > 0) {
                            if (model.ClientID != items[0].id) {
                                Global.post("/Orders/UpdateOrderClient", {
                                    orderid: model.OrderID,
                                    clientid: items[0].id,
                                    name: items[0].name
                                }, function (data) {
                                    if (data.status) {
                                        alert("订单委托成功!", location.href);
                                    }
                                });
                            } else {
                                alert("请选择不同工厂进行委托!");
                            }
                        }
                    }
                });
            });

            //删除需求单
            $("#btndelete").click(function () {
                confirm("需求单删除后不可恢复，确认删除吗？", function () {
                    _self.deleteOrder();
                });
            });
        }

        //回退委托
        if (_self.status == 0 && !!model.EntrustClientID) {
            $("#btnreturn").show();
            $("#btnreturn").click(function () {
                confirm("委托退回后不能撤销，确认退回委托吗？", function () {
                    Global.post("/Orders/ApplyReturnOrder", {
                        orderid: model.OrderID,
                    }, function (data) {
                        if (data.status) {
                            alert("委托退回成功!", location.href);
                        } else {
                            alert("委托退回失败，请稍后重试")
                        }
                    });
                });
            });
        } else {
            $("#btnreturn").hide();
        }

        //转移流程
        if (_self.status != 0 && _self.status != 4) {
            $("#changeProcess").hide();
        }

        if (_self.status >= 7) {
            $("#changeOrderStatus,#updateOrderInfo").hide();
        }

        if (_self.status == 0 && _self.model.OrderType == 1) {
            $("#changeOrderStatus").html("转为订单");
        } else if (_self.status == 0 && _self.model.OrderType == 2) {
            $("#changeOrderStatus").html("绑定打样款号");
        } else if (_self.status == 1) {
            $("#changeOrderStatus").html("完成打样");
        } else if (_self.status == 2) {
            $("#changeOrderStatus").html("完成合价");
        } else if (_self.status == 3) {
            $("#changeOrderStatus").html("大货下单");
        } else if (_self.status == 4) {
            $("#changeOrderStatus").html("开始生产");
        } else if (_self.status == 5) {
            $("#changeOrderStatus").html("完成生产");
        } else if (_self.status == 6) {
            $("#changeOrderStatus").html("订单发货");
        }

        //状态
        $(".status-items li").each(function () {
            var _this = $(this), status = _this.data("status");
            if (status <= _self.status) {
                _this.find("span").css("color", "#3D90F0");
                _this.find(".status-bg,.status-line").css("background-color", "#3D90F0");
            }
        });

        for (var i = 0; i < model.OrderStatus.length; i++) {
            $(".status-items li[data-status='" + model.OrderStatus[i].Status + "']").find(".status-time-text").html(model.OrderStatus[i].CreateTime.toDate("yyyy-MM-dd"));
        }

        if (model.Platemaking) {
            $("#navEngravingInfo").html(decodeURI(model.Platemaking));
            $("#navEngravingInfo .ico-dropdown").remove();
            $("#navEngravingInfo tr").each(function () {
                $(this).find("td").last().remove();
            });
        }
        if (model.PlateRemark) {
            $("#navEngravingRemark").html(decodeURI(model.PlateRemark));
        }

        var images = model.OrderImages.split(",");
        
        for (var i = 0; i < images.length; i++) {
            if (images[i]) {
                var img = $('<li class="' + (i == 0 ? 'hover' : "") + '"><img src="' + images[i] + '" /></li>');
                $(".order-imgs-list").append(img);
                img.click(function () {
                    var _this = $(this);
                    if (!_this.hasClass("hover")) {
                        _this.siblings().removeClass("hover");
                        _this.addClass("hover");
                        $("#orderImage").attr("src", _this.find("img").attr("src"));
                    }
                });
            }
        }
    }
    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;

        if (_self.status > 0) {
            if (_self.status >= 3) {
                $("#updateProfitPrice").hide();
            } else {
                //设置利润比例
                $("#updateProfitPrice").click(function () {
                    doT.exec("template/orders/updateProfitPrice.html", function (template) {
                        var innerText = template();
                        Easydialog.open({
                            container: {
                                id: "show-updateProfitPrice",
                                header: "设置利润比例",
                                content: innerText,
                                yesFn: function () {
                                    var price = $("#iptProfitPrice").val().trim();
                                    if (!price.isDouble() || price < 0) {
                                        alert("利润比例必须为不小于0的数字！");
                                        return false;
                                    }
                                    _self.updateProfitPrice(price);
                                },
                                callback: function () {

                                }
                            }
                        });

                        $("#iptProfitPrice").focus();
                        $("#iptProfitPrice").val($("#profitPrice").html())
                    });
                });
            }
        } else { //需求单
            $("#updateProfitPrice").hide();
        }
        
        if (_self.status == 0 || _self.status == 4) {
            //更换流程
            $("#changeProcess").click(function () {
                var _this = $(this);
                ChooseProcess.create({
                    title: "更换流程",
                    type: _self.model.OrderType,
                    callback: function (items) {
                        if (items.length > 0) {
                            if (_this.data("processid") != items[0].id) {
                                Global.post("/Orders/UpdateOrderProcess", {
                                    processid: items[0].id,
                                    orderid: _this.data("id"),
                                    name: items[0].name
                                }, function (data) {
                                    if (data.status) {
                                        location.href = location.href;
                                    }
                                });
                            } else {
                                alert("请选择不同流程进行更换!");
                            }
                        }
                    }
                });
            });
        }

        //确认大货明细
        $("#confirmDHOrder").click(function () {
            _self.createDHOrder(true);
        });

        //绑定打样单
        $("#bindOriginalOrder").click(function () {
            var _this = $(this);
            ChooseOrder.create({
                title: "绑定打样订单",
                callback: function (items) {
                    if (items.length > 0) {
                        Global.post("/Orders/UpdateOrderOriginalID", {
                            orderid: _self.orderid,
                            originalorderid: items[0].id,
                            name: items[0].name
                        }, function (data) {
                            if (data.status) {
                                alert("绑定打样订单成功!", location.href);
                            }
                        });
                    }
                }
            });
        });

        //更换负责人
        $("#changeOwner").click(function () {
            var _this = $(this);
            ChooseUser.create({
                title: "更换负责人",
                type: 1,
                single: true,
                callback: function (items) {
                    if (items.length > 0) {
                        if (_this.data("userid") != items[0].id) {
                            Global.post("/Orders/UpdateOrderOwner", {
                                userid: items[0].id,
                                ids: _this.data("id")
                            }, function (data) {
                                if (data.status) {
                                    $("#lblOwner").text(items[0].name);
                                }
                            });
                        } else {
                            alert("请选择不同人员进行更换!");
                        }
                    }
                }
            });
        });

        //更改订单状态
        $("#changeOrderStatus").click(function () {
            var _this=$(this);
            //开始打样
            if (_self.model.OrderType == 1 && _self.status == 0) {
                confirm("转为订单后不可撤销且不能变更流程，确认转为订单吗？", function () {
                    _self.updateOrderStatus(1);
                });
            } //开始大货(无)
            else if (_self.model.OrderType == 2 && _self.status == 0) {
                $("#bindOriginalOrder").click();
            }//合价完成
            else if (_self.status == 2) {
                doT.exec("template/orders/sureprice.html", function (template) {
                    var innerText = template();
                    Easydialog.open({
                        container: {
                            id: "show-surequantity",
                            header: "确认打样价格",
                            content: innerText,
                            yesFn: function () {
                                var price = $("#iptFinalPrice").val().trim();
                                if (!price.isDouble() || price <= 0) {
                                    alert("价格必须为大于0的数字！");
                                    return false;
                                }
                                _self.updateOrderStatus(3, 0, price);
                            },
                            callback: function () {

                            }
                        }
                    });

                    $("#iptFinalPrice").focus();
                    $("#iptFinalPrice").val(($("#amount").text() * (1 + $("#profitPrice").text() / 100)).toFixed(2))
                });
            } //大货下单
            else if (_self.status == 3) {
                _self.createDHOrder(false);
            }//开始生产
            else if (_self.status == 4) {
                confirm("开始生产后不可撤销且不能变更流程，确认开始生产吗？", function () {
                    _self.updateOrderStatus(_self.status * 1 + 1);
                });
            }//发货
            else if (_self.status == 6) {
                _self.sendGoods();
            } else {
                confirm("操作后不可撤销，确认" + _this.html() + "吗？", function () {
                    _self.updateOrderStatus(_self.status * 1 + 1);
                });
            }
            
        });

        //编辑订单信息
        $("#updateOrderInfo").click(function () {
            _self.editOrder(_self.model);
        });

        //删除发票信息
        $("#deleteInvoice").click(function () {
            var _this = $(this);
            confirm("撤销后不可恢复，确认撤销此开票申请吗？", function () {
                Global.post("/Finance/DeleteBillingInvoice", {
                    id: _this.data("id"),
                    billingid: _this.data("billingid")
                }, function (data) {
                    if (data.status) {
                        var ele = $("#navInvoices tr[data-id='" + _this.data("id") + "']");
                        ele.remove();
                    } else {
                        alert("申请已通过审核，不能删除!");
                    }
                });
            });
        });

        //切换模块
        $(".tab-nav-ul li").click(function () {
            var _this = $(this);
            _this.siblings().removeClass("hover");
            _this.addClass("hover");
            $(".nav-partdiv").hide();
            $("#" + _this.data("id")).show();

            if (_this.data("id") == "navLog" && (!_this.data("first") || _this.data("first") == 0)) {
                _this.data("first", "1");
                _self.getLogs(1);
            } else if (_this.data("id") == "navEngraving" || _this.data("id") == "navProducts") {
                if (_this.data("mark")) {
                    $("#navOrderTalk").show();
                    _self.mark = _this.data("mark");
                    _self.getTaskReplys(1);
                }
            } 
        });
    }
    //大货下单
    ObjectJS.createDHOrder = function (isExists) {
        var _self = this;
        Global.post("/Products/GetOrderCategoryDetailsByID", {
            categoryid: _self.model.CategoryID,
            orderid: (_self.model.OrderType == 1 ? _self.orderid : _self.model.OriginalID)
        }, function (data) {
            doT.exec("template/orders/surequantity.html", function (template) {
                var innerText = template(data.Model);
                Easydialog.open({
                    container: {
                        id: "show-surequantity",
                        header: "确认大货明细",
                        content: innerText,
                        yesFn: function () {
                            
                            var details = [];
                            $(".child-product-table .list-item").each(function () {
                                var _this = $(this);
                                var modelDetail = {
                                    SaleAttr: _this.data("attr"),
                                    AttrValue: _this.data("value"),
                                    SaleAttrValue: _this.data("attrvalue"),
                                    Quantity: _this.find(".quantity").val(),
                                    Description: _this.data("name")
                                };
                                details.push(modelDetail);
                            });
                            if (details.length > 0) {
                                var orderModel = {};
                                orderModel.OrderID = _self.orderid;
                                orderModel.OrderGoods = details;
                                Global.post("/Orders/CreateDHOrder", {
                                    entity: JSON.stringify(orderModel),
                                    originalid: (!isExists ? "" : _self.model.OriginalID)
                                }, function (data) {
                                    if (data.id) {
                                        alert("大货下单成功!", "/Customer/OrderDetail/" + data.id);
                                    } else {
                                        alert("大货下单失败，请刷新页面重试！");
                                    }
                                });
                            }
                        },
                        callback: function () {

                        }
                    }
                });
                //组合下单规格
                $(".productsalesattr .attritem").click(function () {
                    var bl = false, details = [], isFirst = true;
                    $(".productsalesattr").each(function () {
                        bl = false;
                        var _attr = $(this), attrdetail = details;
                        //组合规格
                        _attr.find("input:checked").each(function () {
                            bl = true;
                            var _value = $(this);
                            //首个规格
                            if (isFirst) {
                                var model = {};
                                model.ids = _attr.data("id") + ":" + _value.val();
                                model.saleAttr = _attr.data("id");
                                model.attrValue = _value.val();
                                model.names = _attr.data("text") + ":" + _value.data("text");
                                model.layer = 1;
                                model.guid = Global.guid();
                                details.push(model);
                            } else {
                                for (var i = 0, j = attrdetail.length; i < j; i++) {
                                    if (attrdetail[i].ids.indexOf(_value.data("id")) < 0) {
                                        var model = {};
                                        model.ids = attrdetail[i].ids + "," + _attr.data("id") + ":" + _value.val();
                                        model.saleAttr = attrdetail[i].saleAttr + "," + _attr.data("id");
                                        model.attrValue = attrdetail[i].attrValue + "," + _value.val();
                                        model.names = attrdetail[i].names + "," + _attr.data("text") + ":" + _value.data("text");
                                        model.layer = attrdetail[i].layer + 1;
                                        model.guid = Global.guid();
                                        details.push(model);
                                    }
                                }
                            }
                        });
                        isFirst = false;
                    });
                    //选择所有属性
                    if (bl) {
                        var layer = $(".productsalesattr").length, items = [];
                        for (var i = 0, j = details.length; i < j; i++) {
                            var model = details[i];
                            if (model.layer == layer) {
                                items.push(model);
                            }
                        }
                        $("#childGoodsQuantity").empty();
                        //加载子产品
                        doT.exec("template/orders/orders_child_list.html", function (templateFun) {
                            var innerText = templateFun(items);
                            innerText = $(innerText);
                            $("#childGoodsQuantity").append(innerText);

                            Easydialog.toPosition();

                            //价格必须大于0的数字
                            innerText.find(".quantity").change(function () {
                                var _this = $(this);
                                if (!_this.val().isInt() || _this.val() <= 0) {
                                    _this.val("1");
                                }
                            });

                            //绑定启用插件
                            innerText.find(".ico-del").click(function () {
                                var _this = $(this);
                                confirm("确认删除此规格吗？", function () {
                                    _this.parents("tr.list-item").remove();
                                })
                            });
                        });

                    }
                });
            });
        });
    }

    ObjectJS.sendGoods = function () {
        var _self = this;
        doT.exec("template/orders/sendordergoods.html", function (template) {
            var innerText = template(_self.model.OrderGoods);
            
            Easydialog.open({
                container: {
                    id: "show-sendordergoods",
                    header: "大货单发货",
                    content: innerText,
                    yesFn: function () {
                        var details = ""
                        $("#orderSendGoods .list-item").each(function () {
                            var _this = $(this);
                            var quantity = _this.find(".quantity").val();
                            if (quantity > 0) {
                                details += _this.data("id") + "-" + quantity + ",";
                            }
                        });
                        if (details.length > 0) {
                            Global.post("/Orders/SendOrderGoods", {
                                orderid: _self.orderid,
                                expresscode: $("#expressCode").val(),
                                details: details
                            }, function (data) {
                                if (data.id) {
                                    alert("发货成功!", location.href);
                                } else {
                                    alert("发货成功！");
                                }
                            });
                        }
                    },
                    callback: function () {

                    }
                }
            });
            $("#orderSendGoods").find(".quantity").change(function () {
                var _this = $(this);
                if (!_this.val().isInt() || _this.val() <= 0) {
                    _this.val("0");
                }
            });
        });
    };

    //绑定账单
    ObjectJS.getOrderBills = function () {
        Global.post("/Finance/GetOrderBillByID", { id: _self.orderid }, function (data) {
            var model = data.model;
            if (model.BillingID) {
                $("#infoPaymoney").text(model.PayMoney.toFixed(2));
                _self.getPays(model.BillingPays, true);

                //申请开票
                if (model.InvoiceStatus == 0) {
                    _self.billingid = model.BillingID;
                    $("#addInvoice").click(function () {
                        _self.addInvoice();
                    });
                }
            }
        });
    }

    //删除订单
    ObjectJS.deleteOrder = function () {
        var _self = this;
        Global.post("/Orders/DeleteOrder", { orderid: _self.orderid }, function (data) {
            if (data.status) {
                location.href = "/Customer/Orders";
            } else {
                alert("需求单删除失败，可能因为单据状态已改变，请刷新页面后重试！");
            }
        });
    }

    //转移工厂
    ObjectJS.changeOrderClient = function (code) {
        var _self = this;
        Global.post("/Orders/UpdateOrderClient", {
            orderid: _self.orderid,
            clientcode: code
        }, function (data) {
            if (data.status) {
                location.href = "/Customer/Orders";
            } else {
                alert("需求单转移失败，可能因为单据状态已改变，请刷新页面后重试！");
            }
        });
    }

    //设置利润比例
    ObjectJS.updateProfitPrice = function (profit) {
        var _self = this;
        Global.post("/Orders/UpdateProfitPrice", {
            orderid: _self.orderid,
            profit: profit
        }, function (data) {
            if (data.status) {
                location.href = location.href;
            } else {
                alert("利润比例设置失败，可能因为订单状态已改变，请刷新页面后重试！");
            }
        });
    }

    //计算总金额
    ObjectJS.getAmount = function () {
        var amount = 0;
        $(".amount").each(function () {
            var _this = $(this);
            _this.html(((_this.prevAll(".tr-quantity").find("label").text() * 1 + _this.prevAll(".tr-loss").find("label").text() * 1) * _this.prevAll(".tr-price").find("label").text()).toFixed(2));
            amount += _this.html() * 1;
        });
        $("#amount").text(amount.toFixed(2));
    }

    //编辑信息
    ObjectJS.editOrder = function (model) {
        var _self = this;
        doT.exec("template/orders/order-detail.html", function (template) {
            var innerText = template(model);
            Easydialog.open({
                container: {
                    id: "show-model-detail",
                    header: "编辑订单信息",
                    content: innerText,
                    yesFn: function () {
                        var entity = {
                            OrderID: _self.orderid,
                            PersonName: $("#personName").val().trim(),
                            MobileTele: $("#mobileTele").val().trim(),
                            CityCode: CityObj.getCityCode(),
                            Address: $("#address").val().trim(),
                            TypeID: "",//$("#orderType").val().trim(),
                            ExpressType: 0,//$("#expressType").val().trim(),
                            PostalCode: "",//$("#postalcode").val().trim(),
                            Remark: $("#remark").val().trim()
                        };
                        Global.post("/Orders/EditOrder", { entity: JSON.stringify(entity) }, function (data) {
                            if (data.status) {
                                location.href = location.href;
                            } else {
                                alert("订单信息编辑失败，请刷新页面重试！");
                            }
                        });
                    },
                    callback: function () {

                    }
                }
            });

            CityObj = City.createCity({
                cityCode: model.CityCode,
                elementID: "city"
            });

            $("#orderType").val(model.TypeID);

            $("#extent").val(model.Extent);

            $("#industry").val(model.IndustryID);
        });
    }

    //登记发票
    ObjectJS.addInvoice = function () {
        var _self = this;
        doT.exec("template/finance/orderinvoice-detail.html", function (template) {
            var innerText = template();
            Easydialog.open({
                container: {
                    id: "show-invoice-detail",
                    header: "开票申请",
                    content: innerText,
                    yesFn: function () {
                        if (!VerifyInvoice.isPass()) {
                            return false;
                        }
                        var entity = {
                            BillingID: _self.billingid,
                            Type:1,
                            CustomerType: $("#invoicetype").val(),
                            InvoiceMoney: $("#invoicemoney").val().trim(),
                            InvoiceTitle: $("#invoicetitle").val().trim(),
                            CityCode: CityInvoice.getCityCode(),
                            Address: $("#invoiceaddress").val().trim(),
                            PostalCode: $("#invoicepostalcode").val().trim(),
                            ContactName: $("#contactname").val().trim(),
                            ContactPhone: $("#contactmobile").val().trim(),
                            Remark: $("#remark").val().trim()
                        };
                        if (entity.InvoiceMoney <= 0) {
                            alert("开票金额必须大于0！");
                            return false;
                        }
                        confirm("提交后不能更改，确认提交吗？", function () {
                            _self.saveOrderInvoice(entity);
                        })
                        return false;
                    },
                    callback: function () {

                    }
                }
            });

            $("#invoicemoney").focus();

            $("#invoicemoney").val(_self.model.TotalMoney.toFixed(2));
            $("#invoicetitle").val(_self.model.Customer.Name);
            $("#invoiceaddress").val(_self.model.Address);
            $("#invoicepostalcode").val(_self.model.PostalCode);
            $("#contactname").val(_self.model.PersonName);
            $("#contactmobile").val(_self.model.MobileTele);

            CityInvoice = City.createCity({
                elementID: "invoicecity",
                cityCode: _self.model.CityCode
            });

            VerifyInvoice = Verify.createVerify({
                element: ".verify",
                emptyAttr: "data-empty",
                verifyType: "data-type",
                regText: "data-text"
            });
        });
    }

    //更改订单状态
    ObjectJS.updateOrderStatus = function (status, quantity, price) {
        var _self = this;
        Global.post("/Orders/UpdateOrderStatus", {
            orderid: _self.orderid,
            status: status,
            quantity: quantity ? quantity : 0,
            price: price ? price : 0
        }, function (data) {
            if (!data.status) {
                alert(data.errinfo);
            } else {
                location.href = location.href;
            }
        });
    }

    //保存发票信息
    ObjectJS.saveOrderInvoice = function (model) {
        Easydialog.close();
        var _self = this;
        Global.post("/Finance/SaveBillingInvoice", { entity: JSON.stringify(model) }, function (data) {
            if (data.item.InvoiceID) {
                $("#addInvoice").hide();
            } else {
                alert("已提交过申请，不能重复操作！");
            }
        });
    }

    //绑定支付列表
    ObjectJS.getPays = function (items, empty) {
        var _self = this;
        if (empty) {
            $("#navPays .tr-header").nextAll().remove();
        }
        doT.exec("template/finance/billingpays.html", function (template) {
            var innerhtml = template(items);
            innerhtml = $(innerhtml);

            $("#navPays .tr-header").after(innerhtml);
        });
    }

    //获取日志
    ObjectJS.getLogs = function (page) {
        var _self = this;
        $("#orderLog").empty();
        Global.post("/Orders/GetOrderLogs", {
            orderid: _self.orderid,
            pageindex: page
        }, function (data) {

            doT.exec("template/common/logs.html", function (template) {
                var innerhtml = template(data.items);
                innerhtml = $(innerhtml);
                $("#orderLog").append(innerhtml);
            });
            $("#pagerLogs").paginate({
                total_count: data.totalCount,
                count: data.pageCount,
                start: page,
                display: 5,
                border: true,
                border_color: '#fff',
                text_color: '#333',
                background_color: '#fff',
                border_hover_color: '#ccc',
                text_hover_color: '#000',
                background_hover_color: '#efefef',
                rotate: true,
                images: false,
                mouse: 'slide',
                float: "left",
                onChange: function (page) {
                    _self.getLogs(page);
                }
            });
        });
    }

    //初始化任务讨论列表
    ObjectJS.initTalkReply = function () {
        var _self = this;

        $("#btnSaveTalk").click(function () {
            var txt = $("#txtContent");

            if (txt.val().trim()) {
                var model = {
                    GUID: _self.orderid,
                    StageID: "",
                    mark: _self.mark,
                    Content: txt.val().trim(),
                    FromReplyID: "",
                    FromReplyUserID: "",
                    FromReplyAgentID: ""
                };
                _self.saveTaskReply(model);

                txt.val("");
            }

        });

        ObjectJS.getTaskReplys(1);

    }

    //获取任务讨论列表
    ObjectJS.getTaskReplys = function (page) {
        var _self = this;
        $("#replyList").empty();
        $("#replyList").html("<tr><td colspan='2'><div class='dataLoading'><img src='/modules/images/ico-loading.jpg'/><div></td></tr>");
        Global.post("/Opportunitys/GetReplys", {
            guid: _self.orderid,
            stageid: "",
            mark: _self.mark,
            pageSize: 10,
            pageIndex: page
        }, function (data) {
            $("#replyList").empty();
            if (data.items.length > 0) {
                doT.exec("template/customer/replys.html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);

                    $("#replyList").html(innerhtml);

                    innerhtml.find(".btn-reply").click(function () {
                        var _this = $(this), reply = _this.nextAll(".reply-box");
                        reply.slideDown(500);
                        reply.find("textarea").focus();
                        reply.find("textarea").blur(function () {
                            if (!$(this).val().trim()) {
                                reply.slideUp(200);
                            }
                        });
                    });

                    innerhtml.find(".save-reply").click(function () {
                        var _this = $(this);
                        if ($("#Msg_" + _this.data("replyid")).val().trim()) {
                            var entity = {
                                GUID: _this.data("id"),
                                StageID: "",
                                Mark: _self.mark,
                                Content: $("#Msg_" + _this.data("replyid")).val().trim(),
                                FromReplyID: _this.data("replyid"),
                                FromReplyUserID: _this.data("createuserid"),
                                FromReplyAgentID: _this.data("agentid")
                            };

                            ObjectJS.saveTaskReply(entity);
                        }

                        $("#Msg_" + _this.data("replyid")).val('');
                        $(this).parent().slideUp(100);
                    });

                });
            }
            else {
                $("#replyList").html("<tr><td colspan='2'><div class='noDataTxt' >暂无评论!<div></td></tr>");
            }

            $("#pagerReply").paginate({
                total_count: data.totalCount,
                count: data.pageCount,
                start: page,
                display: 5,
                border: true,
                rotate: true,
                images: false,
                mouse: 'slide',
                float: "left",
                onChange: function (page) {
                    _self.getTaskReplys(page);
                }
            });
        });
    }

    //保存任务讨论
    ObjectJS.saveTaskReply = function (model) {
        var _self = this;

        Global.post("/Opportunitys/SavaReply", { entity: JSON.stringify(model) }, function (data) {
            doT.exec("template/customer/replys.html", function (template) {
                var innerhtml = template(data.items);
                innerhtml = $(innerhtml);

                $("#replyList .noDataTxt").parent().parent().remove();

                $("#replyList").prepend(innerhtml);

                innerhtml.find(".btn-reply").click(function () {
                    var _this = $(this), reply = _this.nextAll(".reply-box");
                    reply.slideDown(500);
                    reply.find("textarea").focus();
                    reply.find("textarea").blur(function () {
                        if (!$(this).val().trim()) {
                            reply.slideUp(200);
                        }
                    });
                });

                innerhtml.find(".save-reply").click(function () {
                    var _this = $(this);
                    if ($("#Msg_" + _this.data("replyid")).val().trim()) {
                        var entity = {
                            GUID: _this.data("id"),
                            Content: $("#Msg_" + _this.data("replyid")).val().trim(),
                            FromReplyID: _this.data("replyid"),
                            FromReplyUserID: _this.data("createuserid"),
                            FromReplyAgentID: _this.data("agentid")
                        };
                        ObjectJS.saveTaskReply(entity);

                    }
                    $("#Msg_" + _this.data("replyid")).val('');
                    $(this).parent().slideUp(100);
                });

            });
        });
    }

    module.exports = ObjectJS;
})