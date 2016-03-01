
define(function (require, exports, module) {
    var City = require("city"),  CityInvoice,
        Verify = require("verify"), VerifyInvoice,
        Global = require("global"),
        doT = require("dot"),
        ChooseUser = require("chooseuser"),
        ChooseProcess = require("chooseprocess"),
        Easydialog = require("easydialog");
    require("pager");
    var ObjectJS = {};

    ObjectJS.init = function (orderid, status, model) {
        var _self = this;
        _self.orderid = orderid;
        _self.status = status;
        _self.model = JSON.parse(model.replace(/&quot;/g, '"'));
        _self.bindEvent();
        _self.getAmount();

        _self.bindStyle(_self.model);

        $("#addInvoice").hide();
    }

    ObjectJS.bindStyle = function (model) {
        var _self = this;
        var stages = $(".stage-items"), width = stages.width();

        stages.find("li .leftbg").first().removeClass("leftbg");
        stages.find("li .rightbg").last().removeClass("rightbg");
        stages.find("li,a").width(width / stages.find("li").length - 15);

        if (_self.status != 0 && _self.status != 4) {
            $("#changeProcess").hide();
        }

        if (_self.status >= 7) {
            $("#changeOrderStatus,#updateOrderInfo").hide();
        }

        if (_self.status == 0) {
            $("#changeOrderStatus").html("转为订单");
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
            $("#changeOrderStatus").html("交易完成");
        }

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
    }
    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;

        $("#btnreturn").hide();
        if (_self.status > 0) {
            $("#btndelete").hide();//#btnreturn,

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
            $("#btndelete").show();//#btnreturn,

            //删除需求单
            $("#btndelete").click(function () {
                confirm("需求单删除后不可恢复，确认删除吗？", function () {
                    _self.deleteOrder();
                });
            });
            //转移工厂
            $("#btnchangeclient").click(function () {
                doT.exec("template/orders/changeclient.html", function (template) {
                    var innerText = template();
                    Easydialog.open({
                        container: {
                            id: "show-changeclient",
                            header: "需求单转移工厂",
                            content: innerText,
                            yesFn: function () {
                                var clientcode = $("#produceQuantity").val().trim();
                                if (!clientcode || clientcode.length < 6) {
                                    alert("工厂编码不能小于6位字符！");
                                    return false;
                                }
                                
                            },
                            callback: function () {

                            }
                        }
                    });

                    $("#produceQuantity").focus();
                    $("#produceQuantity").val($("#planQuantity").html())

                });
            });

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

        //转移拥有者
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
                            alert("请选择不同人员进行转移!");
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
                confirm("转为订单后不可撤销且不能变更流程，确认转为订单吗？", function () {
                    _self.updateOrderStatus(4);
                });
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
                doT.exec("template/orders/surequantity.html", function (template) {
                    var innerText = template();
                    Easydialog.open({
                        container: {
                            id: "show-surequantity",
                            header: "确认大货数量",
                            content: innerText,
                            yesFn: function () {
                                var quantity = $("#produceQuantity").val().trim();
                                if (!quantity.isInt() || quantity <= 0) {
                                    alert("大货数量必须为正整数！");
                                    return false;
                                }
                                _self.updateOrderStatus(4, quantity, 0);
                            },
                            callback: function () {

                            }
                        }
                    });

                    $("#produceQuantity").focus();
                    $("#produceQuantity").val($("#planQuantity").html())
                });
            }//开始生产
            else if (_self.status == 4) {
                doT.exec("template/orders/surequantity.html", function (template) {
                    var innerText = template();
                    Easydialog.open({
                        container: {
                            id: "show-surequantity",
                            header: "确认大货数量",
                            content: innerText,
                            yesFn: function () {
                                var quantity = $("#produceQuantity").val().trim();
                                if (!quantity.isInt() || quantity <= 0) {
                                    alert("大货数量必须为正整数！");
                                    return false;
                                }
                                _self.updateOrderStatus(5, quantity, 0);
                            },
                            callback: function () {

                            }
                        }
                    });

                    $("#produceQuantity").focus();
                    $("#produceQuantity").val($("#planQuantity").html())
                });
            }else {
                confirm("操作后不可撤销，确认" + _this.html() + "吗？", function () {
                    _self.updateOrderStatus(_self.status * 1 + 1);
                });
            }
            
        });

        //编辑订单信息
        $("#updateOrderInfo").click(function () {
            _self.editOrder(_self.model);
        });

        Global.post("/Finance/GetOrderBillByID", { id: _self.orderid }, function (data) {
            var model = data.model;
            if (model.BillingID) {
                $("#infoPaymoney").text(model.PayMoney.toFixed(2));
                _self.getPays(model.BillingPays, true);
                _self.getInvoices(model.BillingInvoices, true);

                //申请开票
                if (model.InvoiceStatus == 0) {
                    _self.billingid = model.BillingID;
                    $("#addInvoice").click(function () {
                        _self.addInvoice();
                    });
                }
            }
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

            $("#addInvoice").hide();

            if (_this.data("id") == "navLog" && (!_this.data("first") || _this.data("first") == 0)) {
                _this.data("first", "1");
                _self.getLogs(1);
            } else if (_this.data("id") == "navInvoices" && _self.billingid) {
                $("#addInvoice").show();
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

    //转移工厂
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
                        Global.post("/Opportunitys/EditOrder", { entity: JSON.stringify(entity) }, function (data) {
                            if (data.status) {
                                location.href = location.href;
                            } else {
                                alert("订单信息编辑失败，请刷新页面重试！");
                            }
                        })
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
                alert("订单状态有变更，请重新操作！");
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
                _self.getInvoices([data.item], false)
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

    //绑定开票列表
    ObjectJS.getInvoices = function (items, empty) {
        var _self = this;
        if (empty) {
            $("#navInvoices .tr-header").nextAll().remove();
        }
        doT.exec("template/finance/billinginvoices.html", function (template) {
            var innerhtml = template(items);
            innerhtml = $(innerhtml);

            innerhtml.find(".ico-dropdown").each(function () {
                if ($(this).data("status") == 1) {
                    $(this).remove();
                }
            });
            innerhtml.find(".dropdown").click(function () {
                var _this = $(this);
                if ($(this).data("status") != 1) {
                    var position = _this.find(".ico-dropdown").position();
                    $(".dropdown-ul li").data("id", _this.data("id")).data("billingid", _this.data("billingid"));
                    $(".dropdown-ul").css({ "top": position.top + 20, "left": position.left - 50 }).show().mouseleave(function () {
                        $(this).hide();
                    });
                }
                return false;
            });

            $("#navInvoices .tr-header").after(innerhtml);
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

    module.exports = ObjectJS;
})