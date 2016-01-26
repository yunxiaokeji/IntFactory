define(function (require, exports, module) {
    var Global = require("global"),
        City = require("city"), CityObject, CityContact,
        Verify = require("verify"), VerifyPay, VerifyInvoice,
        doT = require("dot"),
        Easydialog = require("easydialog");
    require("pager");

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function (id) {
        var _self = this;
        _self.billingid = id;
        Global.post("/Finance/GetPayableBillByID", { id: id }, function (data) {
            if (data.model.BillingID) {
                _self.bindInfo(data.model);
                _self.bindEvent(data.model);
            }
        });

        $("#addInvoice").hide();
    }

    //基本信息
    ObjectJS.bindInfo = function (model) {

        var _self = this;

        $("#infoBillingCode").html("账单编号：" + model.BillingCode);
        $("#infoSourceCode").attr("href", $("#infoSourceCode").attr("href") + "/" + model.DocID).html(model.DocCode);
        $("#infoTotalMoney").html(model.TotalMoney.toFixed(2));
        $("#infoPayMoney").html(model.PayMoney.toFixed(2));
        $("#infoInvoiceMoney").html(model.InvoiceMoney.toFixed(2));
        $("#infoCreateTime").html(model.CreateTime.toDate("yyyy-MM-dd hh:mm:ss"));
        $("#infoCreateUser").html(model.CreateUser ? model.CreateUser.Name : "--");

        _self.getPays(model.StorageBillingPays, true);
        _self.getInvoices(model.StorageBillingInvoices, true)
    }

    //绑定事件
    ObjectJS.bindEvent = function (model) {
        var _self = this;

        $(document).click(function (e) {
            //隐藏下拉
            if (!$(e.target).parents().hasClass("dropdown") && !$(e.target).hasClass("dropdown")) {
                $(".dropdown-ul").hide();
            }
        });
        //付款登记
        $("#addPay").click(function () {
            _self.addPay();
        });
        
        $("#addInvoice").click(function () {
            _self.addInvoice();
        });

        //切换模块
        $(".tab-nav-ul li").click(function () {
            var _this = $(this);
            _this.siblings().removeClass("hover");
            _this.addClass("hover");
            $(".nav-partdiv").hide();
            $("#" + _this.data("id")).show();

            if (_this.data("id") == "navInvoices") {
                $("#addPay").hide();
                $("#addInvoice").show();
            } else {
                $("#addPay").show();
                $("#addInvoice").hide();
            }
        });

        //删除发票信息
        $("#deleteInvoice").click(function () {
            var _this = $(this);
            confirm("删除后不可恢复，确认删除此开票记录吗？", function () {
                Global.post("/Finance/DeleteStorageBillingInvoice", {
                    id: _this.data("id"),
                    billingid: _self.billingid
                }, function (data) {
                    if (data.status) {
                        var ele = $("#navInvoices tr[data-id='" + _this.data("id") + "']");
                        $("#infoInvoiceMoney").html(($("#infoInvoiceMoney").html() * 1 - ele.find(".money").html() * 1).toFixed(2));
                        ele.remove();
                    } else {
                        alert("网络异常,请稍后重试!");
                    }
                });
            });
        });

    }
    //绑定支付列表
    ObjectJS.getPays = function (items, empty) {
        var _self = this;
        if (empty) {
            $("#navPays .tr-header").nextAll().remove();
        }
        doT.exec("template/finance/storagebillingpays.html", function (template) {
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
        doT.exec("template/finance/storagebillinginvoices.html", function (template) {
            var innerhtml = template(items);
            innerhtml = $(innerhtml);

            innerhtml.find(".dropdown").click(function () {
                var _this = $(this);
                var position = _this.find(".ico-dropdown").position();
                $(".dropdown-ul li").data("id", _this.data("id"));
                $(".dropdown-ul").css({ "top": position.top + 20, "left": position.left - 50 }).show().mouseleave(function () {
                    $(this).hide();
                });
                return false;
            });

            $("#navInvoices .tr-header").after(innerhtml);
        });
    }

    //登记付款
    ObjectJS.addPay = function () {
        var _self = this;
        doT.exec("template/finance/payable-detail.html", function (template) {
            var innerText = template();
            Easydialog.open({
                container: {
                    id: "show-pays-detail",
                    header: "付款登记",
                    content: innerText,
                    yesFn: function () {
                        if (!VerifyPay.isPass()) {
                            return false;
                        }
                        var entity = {
                            BillingID: _self.billingid,
                            Type: 1,
                            PayType: $("#paytype").val(),
                            PayTypeStr: $("#paytype option:selected").html(),
                            PayMoney: $("#paymoney").val().trim(),
                            PayTime: $("#paytime").val().trim(),
                            Remark: $("#remark").val().trim()
                        };
                        confirm("请核对金额和日期是否正确，提交后不可修改，确认提交吗？", function () {
                            _self.savePayablePay(entity);
                        });
                        return false;
                        
                    },
                    callback: function () {

                    }
                }
            });

            $("#paymoney").focus();

            laydate({
                elem: '#paytime',
                format: 'YYYY-MM-DD',
                min: '1900-01-01',
                max: laydate.now(),
                istime: false,
                istoday: true
            });
            $("#paytime").val(Date.now().toString().toDate("yyyy-MM-dd"));

            VerifyPay = Verify.createVerify({
                element: ".verify",
                emptyAttr: "data-empty",
                verifyType: "data-type",
                regText: "data-text"
            });
        });
    }

    //登记发票
    ObjectJS.addInvoice = function () {
        var _self = this;
        doT.exec("template/finance/invoice-detail.html", function (template) {
            var innerText = template();
            Easydialog.open({
                container: {
                    id: "show-invoice-detail",
                    header: "发票登记",
                    content: innerText,
                    yesFn: function () {
                        if (!VerifyInvoice.isPass()) {
                            return false;
                        }
                        var entity = {
                            BillingID: _self.billingid,
                            Type: $("#invoicetype").val(),
                            InvoiceMoney: $("#invoicemoney").val().trim(),
                            InvoiceCode: $("#invoicecode").val().trim(),
                            Remark: $("#remark").val().trim()
                        };
                        if (entity.InvoiceMoney <= 0) {
                            alert("开票金额必须大于0！");
                            return false;
                        }
                        _self.savePayableInvoice(entity);
                    },
                    callback: function () {

                    }
                }
            });

            $("#invoicemoney").focus();

            VerifyInvoice = Verify.createVerify({
                element: ".verify",
                emptyAttr: "data-empty",
                verifyType: "data-type",
                regText: "data-text"
            });
        });
    }
    //保存付款
    ObjectJS.savePayablePay = function (model) {
        Easydialog.close();
        var _self = this;
        Global.post("/Finance/SaveStorageBillingPay", { entity: JSON.stringify(model) }, function (data) {
            if (data.item) {
                $("#infoPayMoney").html(($("#infoPayMoney").html() * 1 + data.item.PayMoney).toFixed(2));
                _self.getPays([data.item], false)
            } else {
                alert("网络异常,请稍后重试!");
            }
        });
    }
    //保存发票信息
    ObjectJS.savePayableInvoice = function (model) {
        Easydialog.close();
        var _self = this;
        Global.post("/Finance/SaveStorageBillingInvoice", { entity: JSON.stringify(model) }, function (data) {
            if (data.item.InvoiceID) {
                $("#infoInvoiceMoney").html(($("#infoInvoiceMoney").html() * 1 + data.item.InvoiceMoney).toFixed(2));
                _self.getInvoices([data.item], false)
            } else {
                alert("网络异常,请稍后重试!");
            }
        });
    }

    module.exports = ObjectJS;
});