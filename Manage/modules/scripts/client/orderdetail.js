

define(function (require, exports, module) {
    require("jquery"); 
    var Verify = require("verify"),
        Global = require("global"),
        doT = require("dot"),
        Easydialog = require("easydialog") ;
 
    var OrderDetail = {};

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
        //搜索
        require.async("dropdown", function () {
            var OrderStatus = [
                { ID: "0", Name: "未支付" },
                { ID: "1", Name: "已支付" }
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
                    OrderDetail.Params.pageIndex = 1;
                    OrderDetail.Params.status = parseInt(data.value);
                    OrderDetail.getClientOrderAccounts();
                }
            });            
        });

    };
 
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
                $("#iblPayStatus").text(item.PayStatus == 0 ? "未支付" : item.PayStatus == 1 ? "已支付" : "部分付款" );
                $("#lblAmount").text(item.Amount);
                $("#lblPayFee").text(item.PayFee);                
                $("#lblCreateTime").text(item.CreateTime.toDate("yyyy-MM-dd"));
                $('#lblCreateUser').text(item.CreateUser == null ? "--" : item.CreateUser.Name);
                $('#lblCheckUser').text(item.CheckUser == null ? "--" : item.CheckUser.Name);
                $('#lblCheckTime').text(item.CheckUser.Name != null ? item.CheckTime.toDate("yyyy-MM-dd") : "--");

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
                var innerText = template(model);
                innerText = $(innerText);
                $("#clientorderaccount-header").after(innerText);
            });
        });
    }; 
    module.exports = OrderDetail;
});