
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
        stages.find("li").width(width / stages.find("li").length - 20);

        if (_self.status != 0 && _self.status != 3){
            $("#changeProcess").hide();
        }

        if (model.OrderType == 1 && _self.status == 0) {
            $("#changeOrderStatus").html("开始打样");
        } else if (model.OrderType == 2 && (_self.status == 0 || _self.status == 3)) {
            $("#changeOrderStatus").html("开始生产");
        } else if (_self.status == 1) {
            $("#changeOrderStatus").html("打样完成");
        } else if (_self.status == 2) {
            $("#changeOrderStatus").html("合价完成");
        } else if (_self.status == 3) {
            $("#changeOrderStatus").html("开始生产");
        } else if (_self.status == 4) {
            $("#changeOrderStatus").html("生产完成");
        } else if (_self.status == 5) {
            $("#changeOrderStatus").html("交易结束");
        }

    }
    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;

        $("#btnreturn,#btnconfirm,#btndelete,#updateOrderInfo").hide();

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

        //转移拥有者
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

        //编辑单价
        $(".price").change(function () {
            var _this = $(this);
            if (_this.val().isDouble() && _this.val() > 0) {
                _self.editPrice(_this);
            } else {
                _this.val(_this.data("value"));
            }
        });

        //编辑数量
        $(".quantity").change(function () {
            if ($(this).val().isDouble() && $(this).val() > 0) {
                _self.editQuantity($(this));
            } else {
                $(this).val($(this).data("value"));
            }
        });

        //编辑损耗
        $(".loss").change(function () {
            if ($(this).val().isDouble() && $(this).val() >= 0) {
                _self.editLoss($(this));
            } else {
                $(this).val($(this).data("value"));
            }
        });

        //删除产品
        $(".ico-del").click(function () {
            var _this = $(this);
            confirm("确认从清单中移除此材料吗？", function () {
                Global.post("/Orders/DeleteProduct", {
                    orderid: _self.orderid,
                    autoid: _this.data("id"),
                    name: _this.data("name")
                }, function (data) {
                    if (!data.status) {
                        alert("系统异常，请重新操作！");
                    } else {
                        _this.parents("tr.item").remove();
                        _self.getAmount();
                    }
                });
            });
        });

        $("#changeOrderStatus").click(function () {
            //开始打样
            if (_self.model.OrderType == 1 && _self.status == 0) {
                confirm("开始打样后流程不可变更，确认开始打样吗？", function () {
                    _self.updateOrderStatus(1);
                });
            } //开始生产
            else if (_self.model.OrderType == 2 && (_self.status == 0 || _self.status == 3)) {
                confirm("开始生产后流程不可变更，确认开始生产吗？", function () {
                    _self.updateOrderStatus(4);
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
    //更改材料单价
    ObjectJS.editPrice = function (ele) {
        var _self = this;
        Global.post("/Orders/UpdateOrderPrice", {
            orderid: _self.orderid,
            autoid: ele.data("id"),
            name: ele.data("name"),
            price: ele.val()
        }, function (data) {
            if (!data.status) {
                ele.val(ele.data("value"));
                alert("价格修改失败，可能因为订单状态已改变，请刷新页面后重试！");
            } else {
                ele.parent().nextAll(".amount").html((ele.parent().prevAll(".tr-quantity").find("label").text() * ele.val()).toFixed(2));
                ele.data("value", ele.val());
                _self.getAmount();
            }
        });
    }

    //更改消耗量
    ObjectJS.editQuantity = function (ele) {
        var _self = this;
        Global.post("/Orders/UpdateProductQuantity", {
            orderid: _self.orderid,
            autoid: ele.data("id"),
            name: ele.data("name"),
            quantity: ele.val()
        }, function (data) {
            if (!data.status) {
                ele.val(ele.data("value"));
                alert("系统异常，请重新操作！");
            } else {
                ele.data("value", ele.val());
                _self.getAmount();
            }
        });
    }

    //更改损耗量
    ObjectJS.editLoss = function (ele) {
        var _self = this;
        Global.post("/Orders/UpdateProductLoss", {
            orderid: _self.orderid,
            autoid: ele.data("id"),
            name: ele.data("name"),
            quantity: ele.val()
        }, function (data) {
            if (!data.status) {
                ele.val(ele.data("value"));
                alert("系统异常，请重新操作！");
            } else {
                ele.data("value", ele.val());
                _self.getAmount();
            }
        });
    }

    //计算总金额
    ObjectJS.getAmount = function () {
        var amount = 0;
        $(".amount").each(function () {
            var _this = $(this);
            _this.html(((_this.prevAll(".tr-quantity").find("input").val() * 1 + _this.prevAll(".tr-loss").find("input").val() * 1) * _this.prevAll(".tr-price").find("label").text()).toFixed(2));
            amount += _this.html() * 1;
        });
        $("#amount").text(amount.toFixed(2));
        $("#totalMoney").text((amount * $("#planQuantity").text()).toFixed(2));
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
                            TypeID: $("#orderType").val().trim(),
                            ExpressType: $("#expressType").val().trim(),
                            PostalCode: $("#postalcode").val().trim(),
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
    ObjectJS.updateOrderStatus = function (status) {
        var _self = this;
        Global.post("/Orders/UpdateOrderStatus", {
            orderid: _self.orderid,
            status: status
        }, function (data) {
            if (!data.status) {
                alert("系统异常，请重新操作！");
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

    //删除订单
    ObjectJS.deleteOrder = function () {
        var _self = this;
        Global.post("/Orders/DeleteOrder", { orderid: _self.orderid }, function (data) {
            if (data.status) {
                location.href = location.href;
            } else {
                alert("订单删除失败，可能因为订单状态已改变，请刷新页面后重试！");
            }
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