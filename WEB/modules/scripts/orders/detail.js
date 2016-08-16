define(function (require, exports, module) {
    var City = require("city"),  CityInvoice,
        Verify = require("verify"), VerifyInvoice,
        Global = require("global"),
        doT = require("dot"),
        Upload = require("upload"),
        ChooseUser = require("chooseuser"),
        ChooseFactory = require("choosefactory"),
        ChooseOrder = require("chooseorder"),
        ChooseProcess = require("chooseprocess"),
        ChooseCustomer = require("choosecustomer"),
        Sortable = require("sortable"),
        Easydialog = require("easydialog");
    require("pager");
    require("colormark");

    var ObjectJS = {}, CacheItems = [];

    ObjectJS.init = function (orderid, status, model, list) {
        var _self = this;
        _self.orderid = orderid;
        _self.status = status;
        _self.isLoading = false;
        _self.model = JSON.parse(model.replace(/&quot;/g, '"'));
        if (list) {
            _self.ColorList = JSON.parse(list.replace(/&quot;/g, '"'));
        }       
        _self.bindStyle(_self.model);
        _self.bindEvent();
        _self.getAmount();
        _self.bingCache();
    }

    ObjectJS.bindStyle = function (model) {

        var _self = this;

        $(".sample-report").click(function () {
            if ($(this).data("type") == "1") {
                window.open("/Orders/FentOrderReport/" + model.OrderID);
            } else {
                window.open("/Orders/PlateMakingProcess/" + model.OrderID);
            }            
        });

        if (model.Platemaking) {
            $("#navEngravingInfo").html(decodeURI(model.Platemaking));  
            $("#navEngravingInfo .ico-dropdown").remove();
            $("#navEngravingInfo tr").each(function () {
                $(this).find("td").last().remove();
            });
        } else {           
            $("#navEngravingInfo").html("<div class='nodata-txt'>暂无制版信息<div>");
        }
        //样图
        _self.bindOrderImages(model.OrderImages);       
    }

    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;

        $(document).click(function (e) {
            //隐藏下拉
            if (!$(e.target).parents().hasClass("dropdown") && !$(e.target).hasClass("dropdown")) {
                $(".dropdown-ul").hide();
            }
        });

        //更多操作
        $("#btnOperateTask").click(function () {
            var _this = $(this);
            var position = _this.position();
            $("#ddlOperateTask").css({ "top": position.top + 30, "right":20 }).show().mouseleave(function () {
                $(this).hide();
            });
            return false;
        });

        //更多操作
        $("#btnOperateMore").click(function () {
            var _this = $(this);
            var position = _this.position();
            $("#ddlOperateOrder").css({ "top": position.top + 30, "right": 20 }).show().mouseleave(function () {
                $(this).hide();
            });
            return false;
        });

        //开始订单
        $("#btnBeginOrder").click(function () {
            if (!_self.model.IntGoodsCode) {
                alert("款式编码不能为空");
                return;
            }
            if (!_self.model.GoodsName) {
                alert("款式名称不能为空");
                return;
            }
            if (!_self.model.CategoryID) {
                alert("需求单尚未绑定订单类别，不能转为订单");
                return;
            }
            if (!_self.model.ProcessID) {
                alert("需求单尚未绑定流程，不能转为订单");
                return;
            }
            doT.exec("template/orders/sure_plan_time.html", function (template) {
                var innerText = template(_self.model);
                Easydialog.open({
                    container: {
                        id: "show_sure_plan_time",
                        header: "需求单转为订单",
                        content: innerText,
                        yesText: "确认转为订单",
                        yesFn: function () {
                            var time = $("#iptPlanTime").val().trim();
                            if (!time) {
                                alert("请确认交货日期！");
                                return false;
                            }
                            _self.updateOrderBegin(time);
                        },
                        callback: function () {

                        }
                    }
                });
                laydate({
                    elem: '#iptPlanTime',
                    format: 'YYYY-MM-DD',
                    min: laydate.now(),
                    max: "",
                    istime: false,
                    istoday: true
                });
                var date = (new Date(_self.model.PlanTime.toDate("yyyy-MM-dd")).getTime() - new Date().getTime()) < 0 ? new Date().toString('yyyy-MM-dd') : _self.model.PlanTime.toDate("yyyy-MM-dd");
                $("#iptPlanTime").val(_self.model.PlanTime.toDate("yyyy-MM-dd") == "2040-01-01" ? "" : date);
            });
        });

        //更换品类流程
        $(".btn-change-process-category").click(function () {
            if (_self.isLoading) {
                alert("数据处理中，请稍后");
                return false;
            }
            _self.isLoading = true;

            var _this = $(this);
            ChooseProcess.create({
                title: "更换品类流程",
                type: _self.model.OrderType,
                categoryid: _self.model.BigCategoryID,
                isClearOther: _self.model.OrderType == 2 && _self.model.OriginalID,
                callback: function (items) {
                    if (items.length > 0) {
                        if (_this.data("processid") != items[0].id) {
                            Global.post("/Orders/UpdateOrderProcess", {
                                categoryid: items[0].categoryid,
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
            _self.isLoading = false;
        });

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
                            } else if (price > 100) {
                                confirm("利润比例设置大于100%，确认继续吗？", function () {
                                    _self.updateProfitPrice(price);
                                }, function () {
                                    return false;
                                });
                            } else {
                                _self.updateProfitPrice(price);
                            }
                           
                        },
                        callback: function () {

                        }
                    }
                });

                $("#iptProfitPrice").focus();
                $("#iptProfitPrice").val($("#profitPrice").html())
            });
        });

        //设置订单折扣
        $("#updateOrderDiscount").click(function () {
            doT.exec("template/orders/update_order_discount.html", function (template) {
                var innerText = template();
                Easydialog.open({
                    container: {
                        id: "show-updateOrderDiscount",
                        header: "设置订单价格",
                        content: innerText,
                        yesFn: function () {
                            var price = $("#newPrice").val().trim();
                            if (!price.isDouble() || price < 0) {
                                alert("价格必须为不小于0的数字！");
                                return false;
                            }
                            _self.updateOrderDiscount($("#iptDiscount").val().trim(), price);
                        },
                        callback: function () {

                        }
                    }
                });
                $("#iptDiscount").focus();
                $("#iptDiscount").val((_self.model.Discount).toFixed(3)).data("value", (_self.model.Discount).toFixed(3));
                $("#iptOriginalPrice").text(_self.model.OriginalPrice.toFixed(2));
                $("#newPrice").val(_self.model.FinalPrice.toFixed(2)).data("value", _self.model.FinalPrice.toFixed(2));
                $("#iptDiscount").change(function () {
                    var _this = $(this), _discount = $(this).val()
                    if (!_discount.isDouble() || _discount < 0) {
                        _this.val(_this.data("value"));
                    } else if (_discount > 1) {
                        confirm("折扣大于1会导致价格大于样衣报价，确认继续吗？", function () {
                            $("#newPrice").val((_self.model.OriginalPrice * _discount).toFixed(2));
                            _this.data("value", _this.val());
                        }, function () {
                            _this.val(_this.data("value"));
                        });
                    } else {
                        $("#newPrice").val((_self.model.OriginalPrice * _discount).toFixed(2));
                        _this.data("value", _discount);
                    }
                });
                //金额
                $("#newPrice").change(function () {
                    var _this = $(this);
                    if (!_this.val().isDouble() || _this.val() < 0) {
                        alert("价格必须为不小于0的数字");
                        _this.val(_this.data("value"));
                    } else if (_this.val() > _self.model.FinalPrice) {
                        confirm("大货价格大于样衣报价，确认继续吗？", function () {
                            if (_self.model.OriginalPrice == 0) {
                                $("#iptDiscount").val("1.00");
                            } else {
                                $("#iptDiscount").val((_this.val() / _self.model.OriginalPrice).toFixed(2));
                            }
                            _this.data("value", _this.val());
                        }, function () {
                            _this.val(_this.data("value"));
                        });
                    } else {
                        if (_self.model.OriginalPrice == 0) {
                            $("#iptDiscount").val("1.00");
                        } else {
                            $("#iptDiscount").val((_this.val() / _self.model.OriginalPrice).toFixed(2));
                        }
                        _this.data("value", _this.val());
                    }

                });
            });
        });

        //设置总金额
        $("#updateOrderTotalMoney").click(function () {
            doT.exec("template/orders/update_order_totalmoney.html", function (template) {
                var innerText = template();
                Easydialog.open({
                    container: {
                        id: "show-updateOrderTotalMoney",
                        header: "设置订单总金额",
                        content: innerText,
                        yesFn: function () {
                            var price = $("#newTotalMoney").val().trim();
                            if (!price.isDouble() || price < 0) {
                                alert("总金额必须为不小于0的数字！");
                                return false;
                            }
                            _self.UpdateOrderTotalMoney($("#newTotalMoney").val().trim());
                        },
                        callback: function () {

                        }
                    }
                });
                $("#newTotalMoney").focus().val(_self.model.TotalMoney);

            });
        });

        //确认大货明细
        $("#confirmDHOrder").click(function () {
            if (!_self.model.OriginalID) {
                confirm("您尚未绑定打样单，确认大货明细后不能再绑定打样单，且订单类别不可变更，确认继续操作吗？", function () {
                    _self.createDHOrder(true);
                });
            } else {
                _self.createDHOrder(true);
            }
        });

        if ($(".repeatorder-times").length > 0) {
            require.async("tip", function () {
                $(".repeatorder-times").Tip({
                    width: "100",
                    msg: "第" + $(".repeatorder-times").data('turntime') + "次翻单"
                });
            })
        }

        //任务详情
        require.async("showtaskdetail", function () {
            $(".task-item[data-limit='']").showtaskdetail();
        });

        //更换客户
        $("#changeCustomer").click(function () {
            ChooseCustomer.create({
                title: "绑定客户",
                isAll: true,
                callback: function (items) {
                    if (items.length > 0) {
                        Global.post("/Orders/UpdateOrderCustomer", {
                            orderid: _self.orderid,
                            customerid: items[0].id,
                            name: items[0].name
                        }, function (data) {
                            if (data.status) {
                                alert("绑定客户成功！", location.href)
                            } else {
                                alert("订单已完成，不能更换客户");
                            }
                        });
                    }
                }
            });
        });

        //绑定打样单
        $("#bindOriginalOrder").click(function () {
            var _this = $(this);
            ChooseOrder.create({
                title: "绑定打样订单",
                categoryid: _self.model.OrderGoods.length > 0 ? _self.model.CategoryID : "",
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
                                    _this.data("userid", items[0].id);
                                    $("#lblOwner").text(items[0].name);
                                }
                            });
                        }
                        else {
                            alert("请选择不同人员进行更换!");
                        }
                    }
                }
            });
        });

        //更改订单状态
        $("#changeOrderStatus").click(function () {
            var _this = $(this);
            if (_self.status == 2) {
                doT.exec("template/orders/sureprice.html", function (template) {
                    var innerText = template(_self.model);
                    Easydialog.open({
                        container: {
                            id: "show-surequantity",
                            header: "订单核价",
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

                    $("#iptFinalPrice").val((($("#productMoney").text() * 1 + $("#lblCostMoney").text() * 1) * (1 + $("#profitPrice").text() / 100)).toFixed(2))
                });
            } //大货下单
            else if (_self.status == 3) {
                _self.createDHOrder(false);
            }//发货
            else if (_self.status == 5) {
                _self.sendGoods();
                
            }//交易结束
            else if (_self.status == 6) {
                confirm("确认标记大货单交易结束吗？", function () {
                    _self.updateOrderStatus(7);
                });
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

        //裁剪录入
        $("#btnCutoutOrder").click(function () {
            _self.cutOutGoods($(this));
        });

        //车缝录入
        $("#btnSewnOrder").click(function () {
            _self.sewnGoods($(this));
        });

        //发货录入
        $("#btnSendDYOrder").click(function () {
            _self.sendOrders($(this));
        });

        //发货
        $("#btnSendOrder").click(function () {
            _self.sendGoods($(this));
        });

        //付款登记
        $("#addPay").click(function () {
            _self.addPay();
        });

        //转移工厂
        $("#btnchangeclient").click(function () {
            var _this = $(this);
            ChooseFactory.create({
                title: "订单委托-选择工厂",
                callback: function (items) {
                    if (items.length > 0) {
                        if (_self.model.ClientID != items[0].id) {
                            Global.post("/Orders/UpdateOrderClient", {
                                orderid:_self.model.OrderID,
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

        //退回委托
        $("#btnreturn").click(function () {
            confirm("委托退回后不能撤销，确认退回委托吗？", function () {
                Global.post("/Orders/ApplyReturnOrder", {
                    orderid: _self.model.OrderID,
                }, function (data) {
                    if (data.status) {
                        alert("委托退回成功!", location.href);
                    } else {
                        alert("委托退回失败，请稍后重试")
                    }
                });
            });
        });

        //删除需求单
        $("#btndelete").click(function () {
            confirm("需求单删除后不可恢复，确认删除吗？", function () {
                _self.deleteOrder();
            });
        });

        //终止订单
        $("#btnOverOrder").click(function () {
            confirm("订单终止后不可恢复，请谨慎操作，确认终止吗？", function () {
                _self.overOrder();
            });
        });

        //绑定分类
        $("#changeOrderCategory").click(function () {

            if (_self.isLoading) {
                alert("数据处理中，请稍后");
                return false;
            }
            _self.isLoading = true;
            Global.post("/System/GetClientOrderCategorys", {}, function (data) {
                doT.exec("template/orders/choose-order-category.html", function (template) {
                    var innerText = template(data.items);

                    Easydialog.open({
                        container: {
                            id: "bindOrderCategoryBox",
                            header: "更换订单类别",
                            content: innerText,
                            yesFn: function () {
                                if ($("#bindOrderCategoryBox li .ico-radiobox.hover").length == 0) {
                                    alert("请选择类别！");
                                    return false;
                                }
                                var _hover = $("#bindOrderCategoryBox li .ico-radiobox.hover").parent();
                                confirm("确认绑定类别“" + _hover.data("name") + "”吗？", function () {
                                    Global.post("/Orders/UpdateOrderCategoryID", {
                                        orderid: _self.orderid,
                                        pid: _hover.data("pid"),
                                        categoryid: _hover.data("id"),
                                        name: _hover.data("name")
                                    }, function (data) {
                                        if (data.status) {
                                            alert("订单品类绑定成功!", location.href);
                                        } else {
                                            alert("订单品类绑定失败，请刷新页面重试！");
                                        }
                                    });
                                });
                            },
                            callback: function () {

                            }
                        }
                    });

                    $("#bindOrderCategoryBox li").click(function () {
                        var _this = $(this).find(".ico-radiobox");
                        if (!_this.hasClass("hover")) {
                            $("#bindOrderCategoryBox li .ico-radiobox").removeClass("hover");
                            _this.addClass("hover");
                        }
                    });

                    _self.isLoading = false;
                });
            });
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

        //订单联系人新建客户
        $("#createOrderCustomer").click(function () {
            if (!_self.model.MobileTele) {
                alert("联系方式不能为空");
                return;
            }
            confirm("确认把订单联系人信息创建为新客户吗？", function () {
                Global.post("/Orders/CreateOrderCustomer", {
                    orderid: _self.orderid 
                }, function (data) {
                    if (data.status) {
                        alert('客户创建成功', location.href);
                    } else if (data.result == 2) {
                        alert('订单已绑定客户', location.href);
                    } else if (data.result == 3) {
                        alert('联系方式不能为空', location.href);
                    } else if (data.result == 4) {
                        alert('联系方式已存在客户，您可以选择绑定客户');
                    } else {
                        alert("客户创建失败");
                    }
                });
            });
        });

        //添加成本
        $("#addOtherCost").click(function () {
            _self.addOtherCosts($(this));
        })

        //车缝退回操作
        $("#btnSaveSwen").click(function () {
            var id = ObjectJS.docID;
            if ($(".btn-save-" + id).length <= 0) {
                var _save = $('<div class="hand btn-link mLeft10 btn-save-' + id + '" style="display:inline-block;" data-id="' + id + '">保存</div>');
                var _cancel = $('<div class="hand btn-link mLeft10 btn-cancel-' + id + '" style="display:inline-block;" data-id="' + id + '">取消</div>');
                var _input = $('<div style="display:inline-block;" class="mLeft10 swen-quantity-' + id + '"><input class="mLeft10 quantity" type="text" style="width:40px;" value="0" /></div>');

                _cancel.click(function () {
                    $(".btn-save-" + id).remove();
                    $(".btn-cancel-" + id).remove();
                    $(".swen-quantity-" + id).remove();
                });
                _save.click(function () {
                    var _thisBtn = $(this);
                    if (_thisBtn.data('isSubmit') != 1) {
                        var details = "";
                        $(".swen-quantity-" + $(this).data('id') + " .quantity").each(function () {
                            var _this = $(this);
                            if (_this.val() > 0) {
                                details += _this.parents('tr').data('id') + "|" + _this.val() + ",";
                            }
                        });
                        if (details.length > 0) {
                            _thisBtn.text("保存中...");
                            _thisBtn.data('isSubmit', 1);
                            Global.post("/Task/CreateGoodsDocReturn", {
                                orderID: ObjectJS.orderid,
                                taskID: '',
                                docType: 6,
                                details: details,
                                originalID: id
                            }, function (data) {
                                _thisBtn.text("保存");
                                _thisBtn.data('isSubmit', 0);
                                if (data.result == 1) {
                                    alert("车缝退回成功");
                                    $(".swen-quantity-" + id).each(function () {
                                        var quantity = ($(this).prev().text() * 1) + ($(this).find('input').val() * 1);
                                        $(this).prev().text(quantity);
                                    });
                                    $(".btn-save-" + id).remove();
                                    $(".btn-cancel-" + id).remove();
                                    $(".swen-quantity-" + id).remove();
                                } else if (data.result == 2) {
                                    alert("退回数不能多于车缝数");
                                } else {
                                    alert("网络繁忙，请重试");
                                }
                            });
                        } else {
                            alert("请输入退回数量");
                        }
                    }
                });
                _input.find('.quantity').change(function () {
                    var _this = $(this);
                    if (!_this.val().isDouble() || _this.val() * 1 <= 0) {
                        _this.val(0);
                        return false;
                    }
                    var swenTotal = _this.parents('tr').find('.swen-total').text() * 1;
                    var swenQuantity = _this.val() * 1 + _this.parent().prev().text() * 1;
                    if (swenTotal < swenQuantity) {
                        alert("退回数不能多于车缝数");
                        _this.val(0);
                        return false;
                    }
                    return false;
                });
                $(".btn-swen-box-" + id).append(_save).append(_cancel);
                $(".input-swen-box-" + id).append(_input);
            }
        });

        //编辑数量
        $("#tab11 .quantity").change(function () {
            if (ObjectJS.isLoading) {
                return;
            }

            if ($(this).val().isDouble() && $(this).val() > 0) {
                ObjectJS.editQuantity($(this));
            } else {
                $(this).val($(this).data("value"));
            }
        });

        //切换模块
        $(".module-tab li").click(function () {
            var _this = $(this);
            _this.siblings().removeClass("hover");
            _this.addClass("hover");
            $(".nav-partdiv").hide();
            $("#" + _this.data("id")).show();

            if (_this.data("id") == "navLog" && (!_this.data("first") || _this.data("first") == 0)) {
                _this.data("first", "1");
                require.async("logs", function () {
                    $("#navLog").getObjectLogs({
                        guid: _self.orderid,
                        type: 2, /*1 客户 2订单 10任务 */
                        pageSize: 10
                    });
                });
            } else if (_this.data("id") == "tab12" && (!_this.data("first") || _this.data("first") == 0)) {
                _this.data("first", "1");
                _self.getPlateMakings();
            } else if (_this.data("id") == "tab11" && (!_this.data("first") || _this.data("first") == 0)) {

            } else if (_this.data("id") == "tab15" && (!_this.data("first") || _this.data("first") == 0)) {
                _this.data("first", "1");
                _self.getSendDoc();
            } else if (_this.data("id") == "tab13" && (!_this.data("first") || _this.data("first") == 0)) {
                _this.data("first", "1");
                _self.getCutoutDoc();
            } else if (_this.data("id") == "tab14" && (!_this.data("first") || _this.data("first") == 0)) {
                _this.data("first", "1");
                _self.getSewnDoc();
            } else if (_this.data("id") == "tab16" && (!_this.data("first") || _this.data("first") == 0)) {
                _this.data("first", "1");
                _self.getCosts();
            } else if (_this.data("id") == "navPays" && (!_this.data("first") || _this.data("first") == 0)) {
                _this.data("first", "1");
                _self.getPays();
            } else if (_this.data("id") == "navDHOrder" && (!_this.data("first") || _this.data("first") == 0)) {
                _this.data("first", "1");
                _self.getDHOrders(_self.orderid, 1);
            }
        });
        $(".module-tab li").first().click();
    }

    //加载缓存
    ObjectJS.bingCache = function () {
        var _self = this;
        if (_self.status == 3 || _self.status == 0 || _self.status == 4) {
            Global.post("/Products/GetOrderCategoryDetailsByID", {
                categoryid: _self.model.CategoryID,
                orderid: (_self.model.OrderType == 1 ? _self.orderid : _self.model.OriginalID)
            }, function (data) {
                _self.categoryAttrs = data.Model;
            });
        }

        Global.post("/Plug/GetExpress", {}, function (data) {
            _self.express = data.items;
        });
    }

    //获取制版工艺说明
    ObjectJS.getPlateMakings = function () {
        var _self = this;

        $(".tb-plates").html('');
        $(".tb-plates").html("<tr><td colspan='5'><div class='data-loading'><div></td></tr>");
        Global.post("/Task/GetPlateMakings", {
            orderID: _self.model.OrderType == 1 ? _self.model.OrderID : _self.model.OriginalID,
            taskID: ""
        }, function (data) {
            $(".tb-plates").html('');
            if (data.items.length > 0) {
                doT.exec("template/task/platematring-orderdatail.html", function (template) {
                    PlateMakings = data.items;
                    var html = template(data.items);
                    html = $(html);
                    html.find(".dropdown").remove();
                    $(".tb-plates").append(html);
                });
            }
            else {
                $(".tb-plates").append("<tr><td colspan='5'><div class='nodata-txt'>暂无工艺说明</div></td></tr>");
            }
        });
    }

    //添加成本
    ObjectJS.addOtherCosts = function (btnObject) {
        var _self = this;
        doT.exec("template/orders/add-order-cost.html", function (template) {
            var innerText = template({});

            Easydialog.open({
                container: {
                    id: "addOrderCostBox",
                    header: btnObject.data("name"),
                    content: innerText,
                    yesFn: function () {
                        if (!$("#iptCostPrice").val() || $("#iptCostPrice").val() * 1 <= 0) {
                            alert("价格必须为大于0的数字！");
                            return false;
                        }
                        if (!$("#iptCostDescription").val()) {
                            alert("描述不能为空！");
                            return false;
                        };
                        Global.post("/Orders/CreateOrderCost", {
                            orderid: _self.model.OrderType == 1 ? _self.orderid : _self.model.OriginalID,
                            price: $("#iptCostPrice").val(),
                            remark: $("#iptCostDescription").val()
                        }, function (data) {
                            if (data.status) {
                                alert("保存成功！");
                                $("#lblCostMoney").text(($("#lblCostMoney").text() * 1 + $("#iptCostPrice").val() * 1).toFixed(2));
                                _self.getCosts();
                            } else {
                                alert("成本添加失败，请刷新页面重试！");
                            }
                        });
                    },
                    callback: function () {

                    }
                }
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

    //绑定样式图
    ObjectJS.bindOrderImages = function (orderimages) {
        var _self = this;        
        var images = orderimages.split(",");
        _self.images = images;
        
        for (var i = 0; i < images.length; i++) {
            if (images[i]) {
                if (i == 0) {
                    $("#orderImage").attr("src", images[i].split("?")[0]);
                }
                var img = $('<li class="' + (i == 0 ? 'hover' : "") + '"><img src="' + images[i] + '" /></li>');
                $(".order-imgs-list").append(img);                
            }
        }
        $(".order-imgs-list img").parent().click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");

                $("#orderImage").attr("src", _this.find("img").attr("src").split("?")[0]);

                if ($("#orderImage").width() > $("#orderImage").height()) {
                    $("#orderImage").css("width", 350);
                } else {
                    $("#orderImage").css("height", 350);
                }
            }
        });
        if ($("#orderImage").data("self") == 1) {
            var setimage = $('<li title="编辑样图" class="edit-orderimages">+</li>');
            $(".order-imgs-list").append(setimage);

            setimage.click(function () {
                doT.exec("template/orders/edit-orderimages.html", function (template) {
                    var innerText = template(_self.images);
                    Easydialog.open({
                        container: {
                            id: "show-order-images",
                            header: "更换订单样图(拖动排序)",
                            content: innerText,
                            yesFn: function () {
                                var newimages = "";
                                $("#show-order-images img").each(function () {
                                    newimages += $(this).attr("src") + ",";                                 
                                });                                
                                Global.post("/Orders/UpdateOrderImages", {
                                    orderid: _self.orderid,
                                    images: newimages
                                }, function (data) {
                                    if (data.status) {
                                        $(".order-imgs-list img").parent().remove();
                                        _self.images = newimages.split(",");
                                        for (var i = 0; i < _self.images.length; i++) {
                                            if (_self.images[i]) {
                                                $(".edit-orderimages").before($('<li class="' + (i == 0 ? 'hover' : "") + '"><img src="' + _self.images[i] + '" /></li>'));
                                            }
                                        }
                                        $(".order-imgs-list img").parent().click(function () {
                                            var _this = $(this);
                                            if (!_this.hasClass("hover")) {
                                                _this.siblings().removeClass("hover");
                                                _this.addClass("hover");
                                                $("#orderImage").attr("src", _this.find("img").attr("src").split("?")[0]);
                                            }
                                        });
                                    }
                                });
                            },
                            callback: function () {

                            }
                        }
                    });

                    var uploader = Upload.uploader({
                        browse_button: 'addOrderImages',
                        file_path: "/Content/UploadFiles/Task/",
                        picture_container: "order-imgs-box",
                        file_container: "reply-files",
                        maxQuantity: 10,
                        maxSize: 5,
                        draggable:true,
                        successItems: '.order-imgs-box li:not(:last-child)',
                        fileType: 1,
                        dragdrop: false,
                        init: {
                            
                        }
                    });
                    
                    $("#show-order-images .order-imgs-list").sortable();

                    $("#show-order-images .ico-delete").click(function () {
                        $(this).parent().remove();
                    });
                });
            });
        }

        //图片放大功能
        var width = document.documentElement.clientWidth, height = document.documentElement.clientHeight;
        $("#orderImage").click(function () {
            if ($(this).attr("src")) {
                $(".enlarge-image-bgbox,.enlarge-image-box").fadeIn();
                $(".right-enlarge-image,.left-enlarge-image").css({ "top": height / 2 - 80 })

                $(".enlarge-image-item").append('<img id="enlargeImage" src="' + $(this).attr("src") + '"/>');
                $('#enlargeImage').smartZoom({ 'containerClass': 'zoomableContainer' });

                $(".close-enlarge-image").unbind().click(function () {
                    $(".enlarge-image-bgbox,.enlarge-image-box").fadeOut();
                    $(".enlarge-image-item").empty()
                });
                $(".enlarge-image-bgbox").unbind().click(function () {
                    $(".enlarge-image-bgbox,.enlarge-image-box").fadeOut();
                    $(".enlarge-image-item").empty();
                });
                $(".zoom-botton").unbind().click(function (e) {
                    var scaleToAdd = 0.8;
                    if (e.target.id == 'zoomOutButton')
                        scaleToAdd = -scaleToAdd;
                    $('#enlargeImage').smartZoom('zoom', scaleToAdd);
                    return false;
                });

                $(".left-enlarge-image").unbind().click(function () {
                    var ele = $(".order-imgs-list .hover").prev();
                    if (ele && ele.find("img").attr("src")) {
                        var _img = ele.find("img");
                        $(".order-imgs-list .hover").removeClass("hover");
                        ele.addClass("hover");
                        //$("#enlargeImage").attr("src", _img.attr("src"));
                        $("#orderImage").attr("src", _img.attr("src").split("?")[0]);
                        $(".enlarge-image-item").empty();
                        $(".enlarge-image-item").append('<img id="enlargeImage" src="' + _img.attr("src").split("?")[0] + '"/>');
                        $('#enlargeImage').smartZoom({ 'containerClass': 'zoomableContainer' });
                    }
                });

                $(".right-enlarge-image").unbind().click(function () {
                    var ele = $(".order-imgs-list .hover").next();
                    if (ele && ele.find("img").attr("src")) {
                        var _img = ele.find("img");
                        $(".order-imgs-list .hover").removeClass("hover");
                        ele.addClass("hover");
                        //$("#enlargeImage").attr("src", _img.attr("src"));
                        $("#orderImage").attr("src", _img.attr("src"));
                        $(".enlarge-image-item").empty();
                        $(".enlarge-image-item").append('<img id="enlargeImage" src="' + _img.attr("src").split("?")[0] + '"/>');
                        $('#enlargeImage').smartZoom({ 'containerClass': 'zoomableContainer' });
                    }
                });
            }
        });

        
        if ($("#orderImage").width() > $("#orderImage").height()) {
            $("#orderImage").css("width", 350);
        } else {
            $("#orderImage").css("height", 350);
        }
    }

    //大货下单
    ObjectJS.createDHOrder = function (isExists) {
        var _self = this;
        doT.exec("template/orders/surequantity.html", function (template) {
            var innerText = template(_self.categoryAttrs);            
            Easydialog.open({
                container: {
                    id: "show-surequantity",
                    header: !isExists ? "大货下单" : "确认大货明细",
                    content: innerText,
                    yesFn: function () {
                        var orderModel = {};
                        orderModel.OrderID = _self.orderid;
                        orderModel.OrderGoods = [];
                        $(".child-product-table .quantity").each(function () {
                            var _this = $(this);
                            if (_this.val() > 0) {
                                var item = CacheItems[_this.data("remark")];
                                orderModel.OrderGoods.push({
                                    SaleAttr: item.saleAttr,
                                    AttrValue: item.attrValue,
                                    SaleAttrValue: item.ids,
                                    Quantity: _this.val(),
                                    XRemark: item.xRemark,
                                    YRemark: item.yRemark,
                                    XYRemark: item.xyRemark,
                                    Remark: item.names
                                });
                            }
                        });

                        Global.post("/Orders/CreateDHOrder", {
                            entity: JSON.stringify(orderModel),
                            ordertype: _self.model.OrderType,
                            discount: $("#iptOrderDiscount").val().trim(),
                            price: $("#iptOrderNewPrice").val().trim()
                        }, function (data) {
                            if (data.id) {
                                alert("大货下单成功!", "/Orders/OrderDetail/" + data.id);
                            } else {
                                alert("大货下单失败，请刷新页面重试！");
                            }
                        });
                        

                    },
                    callback: function () {

                    }
                }
            });
            
            $("#lblOrderFinalPrice").text(_self.model.FinalPrice);
            $("#iptOrderNewPrice").val(_self.model.FinalPrice).data("value", _self.model.FinalPrice);
            //折扣
            $("#iptOrderDiscount").change(function () {
                var _this = $(this);
                if (!_this.val().isDouble() || _this.val() < 0) {
                    alert("下单折扣必须为不小于0的数字");
                    _this.val(_this.data("value"));
                } else if (_this.val() > 1) {
                    confirm("下单折扣大于1会导致大货价格大于样衣报价，确认继续吗？", function () {
                        $("#iptOrderNewPrice").val((_self.model.FinalPrice * _this.val()).toFixed(2));
                        _this.data("value", _this.val());
                    }, function () {
                        _this.val(_this.data("value"));
                    });
                } else {
                    $("#iptOrderNewPrice").val((_self.model.FinalPrice * _this.val()).toFixed(2));
                    _this.data("value", _this.val());
                }

            });
            
            //金额
            $("#iptOrderNewPrice").change(function () {
                var _this = $(this);
                if (!_this.val().isDouble() || _this.val() < 0) {
                    alert("价格必须为不小于0的数字");
                    _this.val(_this.data("value"));
                } else if (_this.val() > _self.model.FinalPrice) {
                    confirm("大货价格大于样衣报价，确认继续吗？", function () {
                        if (_self.model.FinalPrice == 0) {
                            $("#iptOrderDiscount").val(1);
                        } else {
                            $("#iptOrderDiscount").val((_this.val() / _self.model.FinalPrice).toFixed(2));
                        }
                        _this.data("value", _this.val());
                    }, function () {
                        _this.val(_this.data("value"));
                    });
                } else {
                    if (_self.model.FinalPrice == 0) {
                        $("#iptOrderDiscount").val(1);
                    } else {
                        $("#iptOrderDiscount").val((_this.val() / _self.model.FinalPrice).toFixed(2));
                    }
                    _this.data("value", _this.val());
                }

            });
            innerText = $(innerText);

            //自定义产品
            $('#show-surequantity').find('.change-attr').change(function () {
                var _this = $(this);
                var isContinue = true;
                _this.parents('.productsalesattr').find('.check-box').each(function () {
                    if (!_this.val().trim()) {
                        alert("自定义规格不能为空");
                        isContinue = false;
                        return false;
                    }
                    if (_this.val().trim() == $(this).text().trim()) {
                        alert("该规格已存在");
                        isContinue = false;
                        return false;
                    }
                });
                if (isContinue) {
                    var checkBoxHtml = $('<span class="mRight10 hand check-box"><span class="checkbox iconfont mTop3" data-attrid="' + _this.data('id') + '"  data-type="' + _this.data('type') + '" data-text="' + _this.val().trim() + '" data-id="|"></span> ' + _this.val().trim() + '</span>');
                    checkBoxHtml.click(function () {
                        var _this = $(this).find(".checkbox");
                        if (_this.hasClass("hover")) {
                            _this.removeClass("hover");
                        } else {
                            _this.addClass("hover");
                        }

                        var bl = false, details = [], isFirst = true, xattr = [], yattr = [];
                        $(".productsalesattr").each(function () {
                            bl = false;
                            var _attr = $(this), attrdetail = details;
                            //组合规格
                            _attr.find(".checkbox.hover").each(function () {
                                bl = true;
                                var _value = $(this);
                                //首个规格
                                if (isFirst) {
                                    var model = {};
                                    model.ids = _attr.data("id") + ":" + _value.data("id");
                                    model.saleAttr = _attr.data("id");
                                    model.attrValue = _value.data("id");
                                    model.xRemark = _value.data("type") == 1 ? ("【" + _value.data("text") + "】") : "";
                                    model.yRemark = _value.data("type") == 2 ? ("【" + _value.data("text") + "】") : "";
                                    model.xyRemark = "【" + _value.data("text") + "】";
                                    model.names = "【" + _attr.data("text") + "：" + _value.data("text") + "】";
                                    model.layer = 1;
                                    details.push(model);

                                } else {
                                    for (var i = 0, j = attrdetail.length; i < j; i++) {
                                        if (attrdetail[i].ids.indexOf(_value.data("attrid")) < 0) {
                                            var model = {};
                                            model.ids = attrdetail[i].ids + "," + _attr.data("id") + ":" + _value.data("id");
                                            model.saleAttr = attrdetail[i].saleAttr + "," + _attr.data("id");
                                            model.attrValue = attrdetail[i].attrValue + "," + _value.data("id");
                                            model.xRemark = attrdetail[i].xRemark + (_value.data("type") == 1 ? ("【" + _value.data("text") + "】") : "");
                                            model.yRemark = attrdetail[i].yRemark + (_value.data("type") == 2 ? ("【" + _value.data("text") + "】") : "");
                                            model.xyRemark = attrdetail[i].xyRemark + "【" + _value.data("text") + "】";
                                            model.names = attrdetail[i].names + "【" + _attr.data("text") + "：" + _value.data("text") + "】";
                                            model.layer = attrdetail[i].layer + 1;
                                            details.push(model);
                                        }
                                    }
                                }
                                //处理二维表
                                if (_value.data("type") == 1 && xattr.indexOf("【" + _value.data("text") + "】") < 0) {
                                    xattr.push("【" + _value.data("text") + "】");
                                } else if (_value.data("type") == 2 && yattr.indexOf("【" + _value.data("text") + "】") < 0) {
                                    yattr.push("【" + _value.data("text") + "】");
                                }

                            });
                            isFirst = false;
                        });
                        $("#childGoodsQuantity").empty();
                        //选择所有属性
                        if (bl) {
                            var layer = $(".productsalesattr").length, items = [];
                            for (var i = 0, j = details.length; i < j; i++) {
                                var model = details[i];
                                if (model.layer == layer) {
                                    items.push(model);
                                    CacheItems[model.xyRemark] = model;
                                }
                            }
                            var tableModel = {};
                            tableModel.xAttr = xattr;
                            tableModel.yAttr = yattr;
                            tableModel.items = items;

                            //加载子产品
                            doT.exec("template/orders/orders_child_list.html", function (templateFun) {
                                var innerText = templateFun(tableModel);
                                innerText = $(innerText);
                                $("#childGoodsQuantity").append(innerText);
                                //数量必须大于0的数字
                                innerText.find(".quantity").change(function () {
                                    var _this = $(this);
                                    if (!_this.val().isInt() || _this.val() <= 0) {
                                        _this.val("0");
                                    }

                                    var total = 0;
                                    $(".child-product-table .tr-item").each(function () {
                                        var _tr = $(this), totaly = 0;
                                        if (!_tr.hasClass("total")) {
                                            _tr.find(".quantity").each(function () {
                                                var _this = $(this);
                                                if (_this.val() > 0) {
                                                    totaly += _this.val() * 1;
                                                }
                                            });
                                            _tr.find(".total-y").text(totaly);
                                        } else {
                                            _tr.find(".total-y").each(function () {
                                                var _td = $(this), totalx = 0;
                                                $(".child-product-table .quantity[data-x='" + _td.data("x") + "']").each(function () {
                                                    var _this = $(this);
                                                    if (_this.val() > 0) {
                                                        totalx += _this.val() * 1;
                                                    }
                                                });
                                                total += totalx;
                                                _td.text(totalx);
                                            });
                                            _tr.find(".total-xy").text(total);
                                        }
                                    });
                                });
                            });
                        }
                    });
                    _this.parent().before(checkBoxHtml);
                    checkBoxHtml.click();
                    _this.val('');
                }
            });

            ///组合产品
            $("#createOrderQuantity").find(".check-box").click(function () {
                var _this = $(this).find(".checkbox");
                if (_this.hasClass("hover")) {
                    _this.removeClass("hover");
                } else {
                    _this.addClass("hover");
                }

                var bl = false, details = [], isFirst = true, xattr = [], yattr = [];
                $(".productsalesattr").each(function () {
                    bl = false;
                    var _attr = $(this), attrdetail = details;
                    //组合规格
                    _attr.find(".checkbox.hover").each(function () {
                        bl = true;
                        var _value = $(this);
                        //首个规格
                        if (isFirst) {
                            var model = {};
                            model.ids = _attr.data("id") + ":" + _value.data("id");
                            model.saleAttr = _attr.data("id");
                            model.attrValue = _value.data("id");
                            model.xRemark = _value.data("type") == 1 ? ("【" + _value.data("text") + "】") : "";
                            model.yRemark = _value.data("type") == 2 ? ("【" + _value.data("text") + "】") : "";
                            model.xyRemark = "【" + _value.data("text") + "】";
                            model.names = "【" + _attr.data("text") + "：" + _value.data("text") + "】";
                            model.layer = 1;
                            details.push(model);

                        } else {
                            for (var i = 0, j = attrdetail.length; i < j; i++) {
                                if (attrdetail[i].ids.indexOf(_value.data("attrid")) < 0) {
                                    var model = {};
                                    model.ids = attrdetail[i].ids + "," + _attr.data("id") + ":" + _value.data("id");
                                    model.saleAttr = attrdetail[i].saleAttr + "," + _attr.data("id");
                                    model.attrValue = attrdetail[i].attrValue + "," + _value.data("id");
                                    model.xRemark = attrdetail[i].xRemark + (_value.data("type") == 1 ? ("【" + _value.data("text") + "】") : "");
                                    model.yRemark = attrdetail[i].yRemark + (_value.data("type") == 2 ? ("【" + _value.data("text") + "】") : "");
                                    model.xyRemark = attrdetail[i].xyRemark + "【" + _value.data("text") + "】";
                                    model.names = attrdetail[i].names + "【" + _attr.data("text") + "：" + _value.data("text") + "】";
                                    model.layer = attrdetail[i].layer + 1;
                                    details.push(model);
                                }
                            }
                        }
                        //处理二维表
                        if (_value.data("type") == 1 && xattr.indexOf("【" + _value.data("text") + "】") < 0) {
                            xattr.push("【" + _value.data("text") + "】");
                        } else if (_value.data("type") == 2 && yattr.indexOf("【" + _value.data("text") + "】") < 0) {
                            yattr.push("【" + _value.data("text") + "】");
                        }

                    });
                    isFirst = false;
                });
                $("#childGoodsQuantity").empty();
                //选择所有属性
                if (bl) {
                    var layer = $(".productsalesattr").length, items = [];
                    for (var i = 0, j = details.length; i < j; i++) {
                        var model = details[i];
                        if (model.layer == layer) {
                            items.push(model);
                            CacheItems[model.xyRemark] = model;
                        }
                    }
                    var tableModel = {};
                    tableModel.xAttr = xattr;
                    tableModel.yAttr = yattr;
                    tableModel.items = items;

                    //加载子产品
                    doT.exec("template/orders/orders_child_list.html", function (templateFun) {
                        var innerText = templateFun(tableModel);
                        innerText = $(innerText);
                        $("#childGoodsQuantity").append(innerText);
                        //数量必须大于0的数字
                        innerText.find(".quantity").change(function () {
                            var _this = $(this);
                            if (!_this.val().isInt() || _this.val() <= 0) {
                                _this.val("0");
                            }

                            var total = 0;
                            $(".child-product-table .tr-item").each(function () {
                                var _tr = $(this), totaly = 0;
                                if (!_tr.hasClass("total")) {
                                    _tr.find(".quantity").each(function () {
                                        var _this = $(this);
                                        if (_this.val() > 0) {
                                            totaly += _this.val() * 1;
                                        }
                                    });
                                    _tr.find(".total-y").text(totaly);
                                } else {
                                    _tr.find(".total-y").each(function () {
                                        var _td = $(this), totalx = 0;
                                        $(".child-product-table .quantity[data-x='" + _td.data("x") + "']").each(function () {
                                            var _this = $(this);
                                            if (_this.val() > 0) {
                                                totalx += _this.val() * 1;
                                            }
                                        });
                                        total += totalx;
                                        _td.text(totalx);
                                    });
                                    _tr.find(".total-xy").text(total);
                                }
                            });
                        });
                    });
                }
            });
        });
    }

    //裁剪录入
    ObjectJS.cutOutGoods = function (btnObject) {
        var _self = this;
        
        doT.exec("template/orders/cutoutgoods.html", function (template) {
            /*弹出层列表显示信息*/
            var items = _self.model.OrderGoods
            items.taskDesc = btnObject.data("name");

            var innerText = template(items);
            Easydialog.open({
                container: {
                    id: "showCutoutGoods",
                    header: btnObject.data("name"),
                    content: innerText,
                    yesFn: function () {
                        var details = ""
                        $("#showCutoutGoods .list-item").each(function () {
                            var _this = $(this);
                            var quantity = _this.find(".quantity").val();
                            if (quantity > 0) {
                                details += _this.data("id") + "-" + quantity + ",";
                            }
                        });
                        if (details.length > 0) {
                            Global.post("/Orders/CreateOrderCutOutDoc", {
                                orderid: _self.orderid,
                                doctype: 1,
                                isover: 0,
                                expressid: "",
                                expresscode: "",
                                details: details,
                                remark: $("#expressRemark").val().trim(),
                                ownerid: $("#showCutoutGoods .choose-owner").data('id')
                            }, function (data) {
                                if (data.id) {
                                    alert("数据录入成功!", location.href);
                                } else if (data.result == "10001") {
                                    alert("您没有操作权限!")
                                } else {
                                    alert("裁片登记失败！");
                                }
                            });
                        } else {
                            alert("请输入数量");
                            return false;
                        }
                    },
                    callback: function () {

                    }
                }
            });

            //默认负责人选择当前登录用户
            $("#showCutoutGoods .owner-name").text($("#currentUser .username").text());

            $("#showCutoutGoods .choose-owner").click(function () {
                var _this = $(this);
                ChooseUser.create({
                    title: "更换负责人",
                    type: 1,
                    single: true,
                    callback: function (items) {
                        if (items.length > 0) {
                            if (_this.data("id") != items[0].id) {
                                _this.data("id", items[0].id);
                                _this.prev().text(items[0].name);
                            }
                            else {
                                alert("请选择不同人员进行更换!");
                            }
                        }
                    }
                });
            });
            $("#showCutoutGoods").find(".quantity").change(function () {
                var _this = $(this);
                if (_this.val() > _this.data("max")) {
                    confirm("输入数量大于下单数，是否继续？", function () { }, function () {
                        _this.val(_this.data("max"));
                    });
                }
            });
            $("#showCutoutGoods").find(".quantity").keyup(function () {
                var _this = $(this);
                if (!_this.val()) {
                    return;
                }
                if (!_this.val().isInt() || _this.val() <= 0) {
                    _this.val("0");
                } 
            });
        });
    };

    //车缝录入
    ObjectJS.sewnGoods = function (btnObject) {
        var _self = this;
        doT.exec("template/orders/sewn-goods.html", function (template) {
            var innerText = template(_self.model.OrderGoods);
            Easydialog.open({
                container: {
                    id: "showSewnGoods",
                    header: btnObject.data("name"),
                    content: innerText,
                    yesFn: function () {
                        var details = "", bl = true;
                        $("#showSewnGoods .list-item").each(function () {
                            var _this = $(this);
                            var quantity = _this.find(".quantity").val();
                            if (quantity > 0) {
                                if (quantity > _this.find(".quantity").data("max")) {
                                    bl = false;
                                }
                                details += _this.data("id") + "-" + quantity + ",";
                            }
                        });
                        if (!bl) {
                            alert("数量输入过大");
                            return false;
                        }
                        if (details.length > 0) {
                            Global.post("/Orders/CreateOrderSewnDoc", {
                                orderid: _self.orderid,
                                doctype: 11,
                                isover: 0,
                                expressid: "",
                                expresscode: "",
                                details: details,
                                remark: $("#expressRemark").val().trim(),
                                ownerid:$("#showSewnGoods .choose-owner").data('id')
                            }, function (data) {
                                if (data.id) {
                                    alert("数据录入成功!", location.href);
                                } else if (data.result == "10001") {
                                    alert("您没有操作权限!")
                                } else {
                                    alert("缝制登记失败！");
                                }
                            });
                        } else {
                            alert("请输入数量");
                            return false;
                        }
                    },
                    callback: function () {

                    }
                }
            });
            //默认负责人选择当前登录用户
            $("#showSewnGoods .owner-name").text($("#currentUser .username").text());

            $("#showSewnGoods .choose-owner").click(function () {
                var _this = $(this);
                ChooseUser.create({
                    title: "更换负责人",
                    type: 1,
                    single: true,
                    callback: function (items) {
                        if (items.length > 0) {
                            if (_this.data("id") != items[0].id) {
                                _this.data("id", items[0].id);
                                _this.prev().text(items[0].name);
                            }
                            else {
                                alert("请选择不同人员进行更换!");
                            }
                        }
                    }
                });
            });
            $("#showSewnGoods").find(".quantity").change(function () {
                var _this = $(this);
                if (_this.val() > _this.data("max")) {
                    _this.addClass("bRed");
                } else {
                    _this.removeClass("bRed");
                }
            });
            $("#showSewnGoods").find(".quantity").keyup(function () {
                var _this = $(this);
                if (!_this.val()) {
                    return;
                }
                if (!_this.val().isInt() || _this.val() <= 0) {
                    _this.val("0");
                } 
            });
        });
    };

    //发货
    ObjectJS.sendGoods = function (btnObject) {
        var _self = this;
        doT.exec("template/orders/sendordergoods.html", function (template) {
            var innerText = template(_self.model.OrderGoods);
            
            Easydialog.open({
                container: {
                    id: "showSendOrderGoods",
                    header: btnObject.data("name"),
                    content: innerText,
                    yesFn: function () {

                        var details = "", bl = true;
                        $("#showSendOrderGoods .list-item").each(function () {
                            var _this = $(this);
                            var quantity = _this.find(".quantity").val();
                            if (quantity > 0) {
                                if (quantity > _this.find(".quantity").data("max")) {
                                    bl = false;
                                }
                                details += _this.data("id") + "-" + quantity + ",";
                            }
                        });

                        if (!bl) {
                            alert("数量输入过大");
                            return false;
                        }

                        if (details.length > 0) {
                            if (!$("#expressid").data("id") || !$("#expressCode").val()) {
                                alert("请完善快递信息!");
                                return false;
                            }
                            Global.post("/Orders/CreateOrderSendDoc", {
                                orderid: _self.orderid,
                                doctype: 2,
                                isover: 0,
                                expressid: $("#expressid").data("id"),
                                expresscode: $("#expressCode").val(),
                                details: details,
                                remark: $("#expressRemark").val().trim()
                            }, function (data) {
                                if (data.id) {
                                    alert("数据录入成功!", location.href);
                                } else if (data.result == "10001") {
                                    alert("您没有操作权限!")
                                } else {
                                    alert("数据录入失败！");
                                }
                            });
                        } else {
                            alert("请输入数量");
                            return false;
                        }

                    },
                    callback: function () {

                    }
                }
            });
            //快递公司
            require.async("dropdown", function () {
                var dropdown = $("#expressid").dropdown({
                    prevText: "",
                    defaultText: "请选择",
                    defaultValue: "",
                    data: _self.express,
                    dataValue: "ExpressID",
                    dataText: "Name",
                    width: "180",
                    isposition: true,
                    onChange: function (data) {

                    }
                });
            });

            $("#showSendOrderGoods").find(".quantity").change(function () {
                var _this = $(this);
                if (_this.val() > _this.data("max")) {
                    _this.addClass("bRed");
                } else {
                    _this.removeClass("bRed");
                }
            });
            $("#showSendOrderGoods").find(".quantity").keyup(function () {
                var _this = $(this);
                if (!_this.val()) {
                    return;
                }
                if (!_this.val().isInt() || _this.val() <= 0) {
                    _this.val("0");
                } 
            });
        });
    };

    //打样单发货
    ObjectJS.sendOrders = function (btnObject) {
        var _self = this;
        doT.exec("template/orders/send_orders.html", function (template) {
            var innerText = template(_self.model.OrderGoods);

            Easydialog.open({
                container: {
                    id: "showSendOrderGoods",
                    header: btnObject.data("name"),
                    content: innerText,
                    yesFn: function () {
                        if (!$("#expressid").data("id") || !$("#expressCode").val()) {
                            alert("请完善快递信息!");
                            return false;
                        }

                        Global.post("/Orders/CreateOrderSendDoc", {
                            orderid: _self.orderid,
                            doctype: 2,
                            isover: 0,
                            expressid: $("#expressid").data("id"),
                            expresscode: $("#expressCode").val(),
                            details: "",
                            remark: $("#expressRemark").val().trim()
                        }, function (data) {
                            if (data.id) {
                                alert("发货成功!", location.href);
                            } else if (data.result == "10001") {
                                alert("您没有操作权限!")
                            } else {
                                alert("发货失败！");
                            }
                        });
                    },
                    callback: function () {

                    }
                }
            });
            //快递公司
            require.async("dropdown", function () {
                var dropdown = $("#expressid").dropdown({
                    prevText: "",
                    defaultText: "请选择",
                    defaultValue: "",
                    data: _self.express,
                    dataValue: "ExpressID",
                    dataText: "Name",
                    width: "180",
                    isposition: true,
                    onChange: function (data) {

                    }
                });
            });
        });
    };

    //删除订单
    ObjectJS.deleteOrder = function () {
        var _self = this;
        Global.post("/Orders/DeleteOrder", { orderid: _self.orderid }, function (data) {
            if (data.status) {
                location.href = "/Orders/Orders";
            } else {
                alert("需求单删除失败，可能因为单据状态已改变，请刷新页面后重试！");
            }
        });
    }

    //终止订单
    ObjectJS.overOrder = function () {
        var _self = this;
        Global.post("/Orders/UpdateOrderOver", { orderid: _self.orderid }, function (data) {
            if (data.status) {
                location.href = location.href;
            } else if (data.result = "10001") {
                alert("您没有操作权限！");
            } else {
                alert("需求单删除失败，可能因为单据状态已改变，请刷新页面后重试！");
            }
        });
    }

    //设置利润比例
    ObjectJS.updateProfitPrice = function (profit) {
        var _self = this;
        Global.post("/Orders/UpdateProfitPrice", {
            orderid: _self.orderid,
            profit: profit / 100
        }, function (data) {
            if (data.status) {
                $("#profitPrice").text((profit * 1).toFixed(2));
                _self.model.ProfitPrice = profit / 100;
            } else {
                alert("利润比例设置失败，可能因为订单状态已改变，请刷新页面后重试！");
            }
        });
    }

    //设置折扣
    ObjectJS.updateOrderDiscount = function (discount, price) {
        var _self = this;
        Global.post("/Orders/UpdateOrderDiscount", {
            orderid: _self.orderid,
            discount: discount,
            price: price
        }, function (data) {
            if (data.status) {
                location.href = location.href;
            } else {
                alert("折扣设置失败，可能因为订单状态已改变，请刷新页面后重试！");
            }
        });
    }

    //设置总金额
    ObjectJS.UpdateOrderTotalMoney = function (totalMoney) {
        var _self = this;
        Global.post("/Orders/UpdateOrderTotalMoney", {
            orderid: _self.orderid,
            totalMoney: totalMoney
        }, function (data) {
            if (data.status) {
                location.href = location.href;
            } else {
                alert("总金额设置失败，可能因为订单状态已改变，请刷新页面后重试！");
            }
        });
    }

    //汇总
    ObjectJS.getAmount = function () {
        //订单明细汇总
        $(".total-item td").each(function () {
            var _this = $(this), _total = 0;
            if (_this.data("class")) {
                $("." + _this.data("class")).each(function () {
                    _total += $(this).html() * 1;
                });
                if (_this.data("class") == "moneytotal") {
                    _this.html(_total.toFixed(2));
                } else {
                    _this.html(_total);
                }
            }
        });
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
                            IntGoodsCode: $("#iptGoodsCode").val().trim(),
                            GoodsName: $("#iptGoodsName").val().trim(),
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
                            } else if (data.result == 3) {
                                alert("订单信息编辑失败，款式编码已存在！");
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

    //开始订单
    ObjectJS.updateOrderBegin = function (time) {
        var _self = this;
        Global.post("/Orders/UpdateOrderBegin", {
            orderid: _self.orderid,
            time: time ? time : ""
        }, function (data) {
            if (!data.status) {
                alert(data.errinfo);
            } else {
                location.href = location.href;
            }
        });
    }

    //更改订单状态
    ObjectJS.updateOrderStatus = function (status, time, price) {
        var _self = this;
        Global.post("/Orders/UpdateOrderStatus", {
            orderid: _self.orderid,
            status: status,
            time: time ? time : "",
            price: price ? price : 0
        }, function (data) {
            if (!data.status) {
                alert(data.errinfo);
            } else {
                location.href = location.href;
            }
        });
    }

    //登记付款
    ObjectJS.addPay = function () {
        var _self = this;
        doT.exec("template/finance/orderpay-detail.html", function (template) {
            var innerText = template();
            Easydialog.open({
                container: {
                    id: "show-pays-detail",
                    header: "收款登记",
                    content: innerText,
                    yesFn: function () {
                        if (!VerifyPay.isPass()) {
                            return false;
                        }
                        var entity = {
                            BillingID: _self.orderid,
                            Type: $("#billType").val(),
                            PayType: $("#billPaytype").val(),
                            PayMoney: $("#billPaymoney").val().trim(),
                            PayTime: $("#billPaytime").val().trim(),
                            Remark: $("#billRemark").val().trim()
                        };
                        confirm("请核对金额和日期是否正确，提交后不可修改，确认提交吗？", function () {
                            Easydialog.close();
                            Global.post("/Finance/SaveOrderBillingPay", { entity: JSON.stringify(entity) }, function (data) {
                                if (data.status) {
                                    alert("登记成功!");
                                    if (entity.Type == 2) {
                                        $("#infoPayMoney").html(($("#infoPayMoney").html() * 1 + entity.PayMoney * 1).toFixed(2));
                                    } else {
                                        $("#infoPayMoney").html(($("#infoPayMoney").html() * 1 - entity.PayMoney * 1).toFixed(2));
                                    }
                                    _self.getPays()
                                } else {
                                    alert("登记失败,请稍后重试!");
                                }
                            });
                        });
                        return false;

                    },
                    callback: function () {

                    }
                }
            });

            $("#billPaymoney").focus();

            laydate({
                elem: '#billPaytime',
                format: 'YYYY-MM-DD',
                min: '1900-01-01',
                max: laydate.now(),
                istime: false,
                istoday: true
            });
            $("#billPaytime").val(Date.now().toString().toDate("yyyy-MM-dd"));

            VerifyPay = Verify.createVerify({
                element: ".verify",
                emptyAttr: "data-empty",
                verifyType: "data-type",
                regText: "data-text"
            });
        });
    }

    //绑定支付列表
    ObjectJS.getPays = function () {
        var _self = this;
        $("#navPays .tr-header").nextAll().remove();
        $("#navPays .tr-header").after("<tr><td colspan='10'><div class='data-loading' ><div></td></tr>");
        Global.post("/Finance/GetOrderBillingPays", {
            orderid: _self.orderid
        }, function (data) {
            $("#navPays .tr-header").nextAll().remove();
            if (data.items.length > 0) {
                doT.exec("template/finance/billingpays.html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);

                    $("#navPays .tr-header").after(innerhtml);
                });
            } else {
                $("#navPays .tr-header").after("<tr><td colspan='10'><div class='nodata-txt' >暂无数据!<div></td></tr>");
            }
        }); 
    }

    //发货记录
    ObjectJS.getSendDoc = function () {
        var _self = this, _box = $("#tab15 .tr-header");
        _box.nextAll().remove();
        _box.after("<tr><td colspan='10'><div class='data-loading' ><div></td></tr>");
        Global.post("/Orders/GetGoodsDocByOrderID", {
            orderid: _self.orderid,
            taskid:'',
            type: 2
        }, function (data) {
            _box.nextAll().remove();
            if (data.items.length > 0) {
                $("#tab15 .table-header").hide();
                var templateInner = "senddocs";
                if (ObjectJS.model.OrderType == 1)
                {
                    templateInner = "senddydocs";
                }
                doT.exec("template/orders/" + templateInner + ".html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);

                    _box.after(innerhtml);

                    if (templateInner == "senddocs") {
                        var total = 0;
                        innerhtml.find('.cut1').each(function () {
                            var _this = $(this);
                            total += parseInt(_this.text());
                        });
                        innerhtml.find('.total-count').html(total);
                    }
                });
            } else {
                $("#tab15 .table-header").show();
                _box.after("<tr><td colspan='10'><div class='nodata-txt' >暂无数据!<div></td></tr>");
            }
        });
    }

    //裁剪记录
    ObjectJS.getCutoutDoc = function () {
        var _self = this, _box = $("#tab13 .tr-header");
        _box.nextAll().remove();
        _box.after("<tr><td colspan='10'><div class='data-loading' ><div></td></tr>");
        Global.post("/Orders/GetGoodsDocByOrderID", {
            orderid: _self.orderid,
            taskid: '',
            type: 1
        }, function (data) {
            _box.nextAll().remove();
            if (data.items.length > 0) {
                $("#tab13 .table-header").hide();
                doT.exec("template/orders/cutoutdoc.html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);
                    _box.after(innerhtml);
                    var total = 0;
                    innerhtml.find('.cut1').each(function () {
                        var _this = $(this);
                        total += parseInt(_this.text());
                    });
                    innerhtml.find('.total-count').html(total);
                });
            } else {
                $("#tab13 .table-header").show();
                _box.after("<tr><td colspan='10'><div class='nodata-txt' >暂无数据!<div></td></tr>");
            }
        });

    }

    //缝制记录
    ObjectJS.getSewnDoc = function () {
        var _self = this, _box = $("#tab14 .tr-header");
        _box.nextAll().remove();
        _box.after("<tr><td colspan='10'><div class='data-loading' ><div></td></tr>");
        Global.post("/Orders/GetGoodsDocByOrderID", {
            orderid: _self.orderid,
            taskid: '',
            type: 11
        }, function (data) {
            _box.nextAll().remove();
            if (data.items.length > 0) {
                $("#tab14 .table-header").hide();
                doT.exec("template/orders/cutoutdoc.html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);
                    innerhtml.find(".ico-dropdown").click(function () {
                        var _this = $(this);
                        ObjectJS.docID = _this.data('id');
                        var position = _this.position();
                        $("#setReturnSewn li").data("columnname", _this.data("columnname"));
                        $("#setReturnSewn").css({ "top": position.top + 20, "left": position.left - 70 }).show().mouseleave(function () {
                            $(this).hide();
                        });
                        return false;
                    });
                    var total = 0;
                    innerhtml.find('.cut1').each(function () {
                        var _this = $(this);
                        total += parseInt(_this.text());
                    });
                    innerhtml.find('.total-count').html(total);

                    _box.after(innerhtml);

                });
            } else {
                $("#tab14 .table-header").show();
                _box.after("<tr><td colspan='10'><div class='nodata-txt' >暂无数据!<div></td></tr>");
            }
        });
    }

    //其他成本
    ObjectJS.getCosts = function () {
        var _self = this, _box = $("#tab16 .tr-header");
        _box.nextAll().remove();
        _box.after("<tr><td colspan='10'><div class='data-loading' ><div></td></tr>");
        Global.post("/Orders/GetOrderCosts", {
            orderid: _self.model.OrderType == 1 ? _self.orderid : _self.model.OriginalID
        }, function (data) {
            _box.nextAll().remove();
            if (data.items.length > 0) {
                doT.exec("template/orders/orderCosts.html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);

                    innerhtml.find(".cost-price").each(function () {
                        $(this).text($(this).text() * $("#tab16").data("quantity"))
                    });

                    _box.after(innerhtml);
                    innerhtml.find(".ico-del").click(function () {
                        var _this = $(this);
                        confirm("删除后不可恢复，确认删除吗？", function () {
                            Global.post("/Orders/DeleteOrderCost", {
                                orderid: _self.model.OrderType == 1 ? _self.orderid : _self.model.OriginalID,
                                autoid: _this.data("id")
                            }, function (data) {
                                if (data.status) {
                                    $("#lblCostMoney").text(($("#lblCostMoney").text() - _this.parents("tr").find(".cost-price").text()).toFixed(2));
                                    _this.parents("tr").first().remove();
                                }
                            });
                        });
                    });

                });
            } else {
                _box.after("<tr><td colspan='10'><div class='nodata-txt' >暂无数据!</div></td></tr>");
            }
        });
    }

    //获取大货订单
    ObjectJS.getDHOrders = function (originalid, page) {
        var _self = this;
        $("#navDHOrder .tr-header").nextAll().remove();
        $("#navDHOrder .tr-header").after("<tr><td colspan='12'><div class='data-loading' ><div></td></tr>");
        Global.post("/Orders/GetOrdersByOriginalID", {
            originalid: originalid,
            ordertype: 2,
            pagesize: 10,
            pageindex: page
        }, function (data) {
            $("#navDHOrder .tr-header").nextAll().remove();
            if (data.items.length > 0) {
                doT.exec("template/orders/orders_originalid.html", function (template) {
                    var innerhtml = template(data.items);
                    
                    innerhtml = $(innerhtml);
                    innerhtml.find(".mark").markColor({
                        isAll: false,
                        data: _self.ColorList,
                        onChange: function (obj, callback) {
                            _self.markOrders(obj.data("id"), obj.data("value"), callback);
                        }
                    });

                    $("#navDHOrder .tr-header").after(innerhtml);
                });
            } else {
                $("#navDHOrder .tr-header").after("<tr><td colspan='12'><div class='nodata-txt' >暂无订单!<div></td></tr>");
            }
            $("#pagerOrders").paginate({
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
                    _self.getDHOrders(originalid, page);
                }
            });
        });
    }

    //标记订单
    ObjectJS.markOrders = function (ids, mark, callback) {
        if (mark < 0) {
            alert("不能标记此选项!");
            return false;
        }
        Global.post("/Orders/UpdateOrderMark", {
            ids: ids,
            mark: mark
        }, function (data) {
            if (data.result == "10001") {
                alert("您没有标记订单的权限！");
                callback && callback(false);
            } else {
                callback && callback(data.status);
            }
        });
    }

    //更改消耗量
    ObjectJS.editQuantity = function (ele) {
        var _self = this;
        ObjectJS.isLoading = true;

        Global.post("/Orders/UpdateProductQuantity", {
            orderid: _self.orderid,
            autoid: ele.data("id"),
            name: ele.data("name"),
            quantity: ele.val()
        }, function (data) {
            if (!data.status) {
                ele.val(ele.data("value"));
                alert("当前订单状态,不能进行修改");
            }
            else {
                ele.data("value", ele.val());
                _self.getProductAmount();
            }
            ObjectJS.isLoading = false;
        });
    }

    ObjectJS.getProductAmount = function () {
        var amount = 0;
        $("#tab11 .cart-item .moneytotal").each(function () {
            var _this = $(this);
            _this.html(((_this.prevAll(".tr-quantity").find("input").val() * 1) * _this.prevAll(".tr-price").find("label").text()).toFixed(3));
            amount += _this.html() * 1;
        });

        $("#tab11 .total-item .moneytotal").html(amount.toFixed(3));
        $("#productMoney").text(amount.toFixed(3));
    }
    module.exports = ObjectJS;
})