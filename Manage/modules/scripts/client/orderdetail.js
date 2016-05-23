

define(function (require, exports, module) {
    require("jquery"); 
    var Verify = require("verify"),
        Global = require("global"),
        doT = require("dot"),
        Easydialog = require("easydialog") ;
    var VerifyObject;
    var OrderDetail = {};
    OrderDetail.Account = {};
    OrderDetail.Params = {
        pageIndex: 1,
        pageSize: 20,
        status: -1,
        type: -1,
        payType: -1,
        orderID: '',
        clientID: '', 
        keyWords: ''
    }; 
    //客户详情初始化
    OrderDetail.detailInit = function (id) {
        OrderDetail.Params.clientID = id;
        OrderDetail.detailEvent();
        if (id) {
            OrderDetail.getClientOrderDetail(id);
        }
    }
    //绑定事件
    OrderDetail.detailEvent = function () {
        //客户设置菜单
        $(".search-tab li").click(function () {
            $(this).addClass("hover").siblings().removeClass("hover");
            var index = $(this).data("index");
            $(".content-body div[name='navContent']").hide().eq(parseInt(index)).show();
            if (index == 0) {
                OrderDetail.getClientOrders(); 
            }  
        }); 
        $('#addAccount').click(function () {
            if (OrderDetail.Account.PayStatus == 4) { alert('此单已全额退款，不能新增账目。');return;}
                var _self = this; 
                doT.exec("template/client/order-accountedit.html", function (template) {
                    var innerText = template(null);
                    Easydialog.open({
                        container: {
                            id: "show-model-detail",
                            header: "新增账目",
                            content: innerText,
                            yesFn: function () {
                                if (!VerifyObject.isPass()) {
                                    return false;
                                }
                                var modules = [];
                                var entity = {
                                    ClientID: OrderDetail.Params.clientID,
                                    OrderID: OrderDetail.Params.orderID,
                                    AlipayNo: $("#alipayNo").val(),
                                    PayType: $("#payType").val(),
                                    Type: $("#type").val(),
                                    RealAmount: $("#realAmount").val(),                                     
                                    Remark: $("#remark").val()
                                };
                                OrderDetail.saveModel(entity);
                            },
                            callback: function () { }
                        }
                    }); 
                    VerifyObject = Verify.createVerify({
                        element: ".verify",
                        emptyAttr: "data-empty",
                        verifyType: "data-type",
                        regText: "data-text"
                    });
                    if (OrderDetail.Account.PayStatus == 0) {
                        $('#type').html('<option value="1">收款</option>');
                    } else if (OrderDetail.Account.PayStatus == 1 || OrderDetail.Account.PayStatus>2) {
                        $('#type').html('<option value="2">退款</option>');
                    } 
                });
        });
    };
    OrderDetail.saveModel = function (model) { 
        var item = OrderDetail.Account;
        if($('#type').val()==2){
            if ((item.PayFee - item.RefundFee) < parseFloat($('#realAmount').val())) {
                alert("退款金额不能超过【支付金额 - 已退款金额】.");
                return;
            }
        }
        var regAmount = /^-?\d+\.?\d{0,4}$/;
        if (!regAmount.test($('#realAmount').val())) {
            alert('【金额】格式输入不正确!'); return;
        }
        var _self = this;
        Global.post("/Client/AddOrderAccount", { orderAccount: JSON.stringify(model) }, function (data) {
            if (data.Result == "1") {
                OrderDetail.getClientOrderDetail(model.OrderID);
            } else   {
                alert("操作失败!");
            }
        });
    }
    //客户详情
    OrderDetail.getClientOrderDetail = function (id) {
        Global.post("/Client/GetClientOrderDetail", { orderid: id }, function (data) {
            if (data.Result == "1") {
                var item = data.Item;
                $("#lblCode").html(item.ClientCode);
                $("#lblName").text(item.CompanyName);
                $("#lblType").text(item.Type == 1 ? "购买系统" : item.Type == 2 ? "购买人数" : "续费");
                $("#lblSourceType").text(item.SourceType==0?"客户下单":'系统新建');
                $("#lblUserQuantity").text(item.UserQuantity);
                $("#lblYears").text(item.Years);
                $("#lblStatus").text(item.Status == 0 ?"未审核": item.Status==1?"已审核":"已关闭");
                $("#lblPayStatus").text(item.PayStatus == 0 ? "未支付" : item.PayStatus == 1 ? "已支付" : item.PayStatus == 2 ? "部分付款" : item.PayStatus == 3 ? "部分退款" : "全额退款");
                $("#lblAmount").text(item.Amount);
                $("#lblPayFee").text(item.PayFee);
                $("#lblReFundFee").text(item.ReFundFee);
                $("#lblCreateTime").text(item.CreateTime.toDate("yyyy-MM-dd"));
                $('#lblCreateUser').text(item.CreateUser == null ? "--" : item.CreateUser.Name);
                $('#lblCheckUser').text(item.CheckUser == null ? "--" : item.CheckUser.Name);
                $('#lblCheckTime').text(item.CheckUser.Name != null ? item.CheckTime.toDate("yyyy-MM-dd") : "--");
                OrderDetail.Account = item;
                OrderDetail.Params.clientID = item.ClientID;
                OrderDetail.Params.orderID = item.OrderID;
                OrderDetail.getClientOrderAccounts();

            } else if (data.Result == "2") {
                alert("登陆账号已存在!");
                $("#loginName").val("");
            }
        });
    };
 
    //获取客户订单账目列表
    OrderDetail.getClientOrderAccounts = function () {
        var _self = this;
        $("#clientorderaccount-header").nextAll().remove();
 
        Global.post("/Client/GetClientOrderAccount", OrderDetail.Params, function (data) {
            doT.exec("template/client/order-account.html?3", function (templateFun) {
                var innerText = templateFun(data.Items);
                innerText = $(innerText);
                $("#clientorderaccount-header").after(innerText);
                $("#tb-clientOrderAccount a.checkOrderAccount").bind("click", function () {
                    if (confirm("确定审核通过吗?")) {
                        Global.post("/Client/CheckOrderAccount", { id: $(this).data("id"), OrderID: $(this).data("orderid") }, function (data) {
                            OrderDetail.validateResult(data);
                        });
                    }
                });
                $("#tb-clientOrderAccount a.deleteOrderAccount").bind("click", function () {
                    if (confirm("确定删除?")) {
                        Global.post("/Client/CloseOrderAccount", { id: $(this).data("id") }, function (data) {
                            OrderDetail.validateResult(data);
                        });
                    }
                });
                $("#tb-clientOrderAccount a.viewRemark").bind("click", function () {
                    Easydialog.open({
                        container: {
                            id: "show-model-detail",
                            header: "查看备注",
                            content: '<ul class="easydialog-create"><li><span class="width80">备注：</span><textarea >' + $(this).data("remark") + '</textarea></li></ul>',
                            yesFn: function () {
                               
                            },
                            callback: function () { }
                        }
                    });
                    $(".easyDialog_footer button.btn_highlight").remove();
                });
            });
        });
    };

    OrderDetail.validateResult = function (data) {
        if (data.Result == 1) {
            OrderDetail.getClientOrderAccounts();
        } else if (data.Result == 1001) {
            alert("订单已被处理,操作失败,请刷新页面查看.");
        } else if (data.Result == 1002) {
            alert("订单已被删除,操作失败,请刷新页面查看.");
        } else {
            alert("操作失败");
        }
    };
    module.exports = OrderDetail;
});