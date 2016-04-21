define(function (require, exports, module) {
    var Global = require("global"),
        Easydialog = require("easydialog"),
        Verify = require("verify"), VerifyObject;

    var ObjectJS = {};

    ObjectJS.Params =
    {
        type: 1,//1:付费购买；2：购买人数;3:续费
        orderID: ''
    };

    //初始化
    ObjectJS.init = function (type, ClientOrdersCount, OrderID) {
        //有未完成的订单，请先处理掉
        if (parseInt(ClientOrdersCount) > 0) {
            alert("你有未完成的订单，请处理...");
            setTimeout(function () { location.href = "/System/Client/4" }, 1000);
        }

        var _self = this;
        ObjectJS.Params.type = type;

        VerifyObject = Verify.createVerify({
            element: ".verify",
            emptyAttr: "data-empty",
            verifyType: "data-type",
            regText: "data-text"
        });

        _self.bindEvent();

        //处理未完成的订单
        if (OrderID != "") {
            ObjectJS.Params.orderID = OrderID;
            if (OrderID == "-1") {
                alert("您的订单不存在，请核查");
                setTimeout(function () { location.href = "/System/Client/3" }, 1000);
            }
            else if (OrderID == "-2") {
                alert("您的订单已支付，请核查");
                setTimeout(function () { location.href = "/System/Client/3" }, 1000);
            }
            else {
                ObjectJS.RealAmount = $("#orderAmount").html();
                ObjectJS.OrderID = OrderID;
                $("#btn_backSelectPrpduct").hide();
                ObjectJS.sureOrder(true);
            }
        }
        else {
            _self.getList();
        }

        ObjectJS.Discount = 1;
    }

    //绑定事件
    ObjectJS.bindEvent = function () {
        //选择人数
        $("#UserCount").blur(function () {
            ObjectJS.getBestWay();
        });

        //选择年份
        $("#UserYear").change(function () {
            $(".productListTB td.tdBG2").each(function () {
                var id = $(this).data("id");
                var year = $(this).data("year");
                var productCount = $(this).data("productCount");

                var yearCount = $("#UserYear").val();
                if (year != yearCount) {
                    $(this).removeClass("tdBG2");
                    var $span = null;
                    if ($(this).find("span").length > 0) {
                        $span = $(this).find("span");
                        $(this).find("span").remove();
                    }
                    $(".productListTB td[data-id='" + id + "'][data-year='" + yearCount + "']").addClass("tdBG2").append($span).data("productCount", productCount);
                }

            });

            //统计产品的人数、金额
            ObjectJS.getTotalPrice();

        });

        //进入确认订单
        $("#btn_sureOrder").click(function () {
            ObjectJS.sureOrder(false);
        });

        if (ObjectJS.Params.orderID == '') {
            //返回选择产品
            $("#btn_backSelectPrpduct").click(function () {
                $(".productList").show();
                $(".productOrder").hide();

                $(".selectProduct").css("background", "url('/modules/images/auction/bg-select-product-active.png') center center");
                $(".selectProduct").children().eq(0).removeClass("stepIcoFinish");
                $(".selectProduct").children().eq(1).removeClass("stepDesFinish");

                $(".sureOrder").css("background", "url('/modules/images/auction/bg-sure-order.png') center center");
                $(".sureOrder").children().eq(0).removeClass("stepIcoActive");
                $(".sureOrder").children().eq(1).removeClass("stepDesActive");

            });
        }

        //进入支付订单
        $("#btn_payOrder").click(function () {
            $(".productOrder").hide();
            $(".payProductOrder").show();

            $(".sureOrder").css("background", "url('/modules/images/auction/bg-sure-order.png') center center");
            $(".sureOrder").children().eq(0).addClass("stepIcoFinish");
            $(".sureOrder").children().eq(1).addClass("stepDesFinish");

            $(".payOrder").css("background", "url('/modules/images/auction/bg-pay-order-active.png') center center");
            $(".payOrder").children().eq(0).addClass("stepIcoActive");
            $(".payOrder").children().eq(1).addClass("stepDesActive");

            if (ObjectJS.Params.orderID == '') {
                ObjectJS.addClientOrder();
            }

        });


        //进入支付宝支付订单
        $("#btn_toPayOrder").click(function () {
            ObjectJS.toPayOrder();
        });

    }

    //获取产品列表
    ObjectJS.getList = function () {
        var _self = this;
        Global.post("/Auction/GetProductList",
            null,
            function (data) {
                var len = data.Items.length;
                var html5 = '<tr><td class="tdBG">5用户(增补)</td>';
                var html = '<tr><td class="tdBG">10用户</td>';
                var html2 = '<tr><td class="tdBG">20用户</td>';
                var html3 = '<tr><td class="tdBG">50用户</td>';
                var html4 = '<tr class="trLast"><td class="tdBG">100用户</td>';

                for (var i = 0; i < len; i++) {
                    var item = data.Items[i];
                    var price = ObjectJS.formatCurrency(item.Price);
                    if (item.UserQuantity == 5) {
                        html5 += '<td  name="productItem" data-id="1" data-productID="' + item.ProductID + '" data-usercount="5" data-price="' + item.Price + '" data-year="' + item.PeriodQuantity + '">' + price;

                        html += '</td>';
                    }
                    else if (item.UserQuantity == 10) {
                        html += '<td  name="productItem" data-id="2" data-productID="' + item.ProductID + '" data-usercount="10" data-price="' + item.Price + '" data-year="' + item.PeriodQuantity + '">' + price;

                        html += '</td>';
                    }
                    else if (item.UserQuantity == 20) {
                        html2 += '<td  name="productItem" data-id="3" data-productID="' + item.ProductID + '" data-usercount="20" data-price="' + item.Price + '" data-year="' + item.PeriodQuantity + '">' + price;

                        html += '</td>';
                    }
                    else if (item.UserQuantity == 50) {
                        html3 += '<td  name="productItem" data-id="4" data-productID="' + item.ProductID + '" data-usercount="50" data-price="' + item.Price + '" data-year="' + item.PeriodQuantity + '">' + price;

                        html += '</td>';
                    }
                    else if (item.UserQuantity == 100) {
                        html4 += '<td  name="productItem" data-id="5" data-productID="' + item.ProductID + '" data-usercount="100" data-price="' + item.Price + '" data-year="' + item.PeriodQuantity + '">' + price;

                        html += '</td>';
                    }
                }

                html += '</tr>';
                html2 += '</tr>';
                html3 += '</tr>';
                html4 += '</tr>';
                html5 += '</tr>';
                $(".productListTB").append(html5).append(html).append(html2).append(html3).append(html4);

                if (ObjectJS.Params.type == 3) {
                    ObjectJS.getBestWay();
                }

            }
        );
    }

    //获取最佳产品组合
    ObjectJS.getBestWay = function () {
        var _self = this;
        var quantity = $("#UserCount").val();

        if (quantity == '') return false;

        quantity = parseInt(quantity);
        var userYear = 1;
        if (_self.Params.type == 1) {
            if (quantity < 9) {
                alert("首次购买人数不能少于10人");
                return false;
            }
            if (quantity > 499) {
                alert("您公司人数超过500人，请联系我们，为您专业定制");
                return false;
            }

            userYear = $("#UserYear").val();
        }
        else if (_self.Params.type == 2) {
            if (quantity < 4) {
                alert("购买人数不能少于5人");
                return false;
            }
            if (quantity > 499) {
                alert("购买人数超过500人，请联系我们，为您专业定制");
                return false;
            }
        }
        else if (_self.Params.type == 3) {
            if (quantity < parseInt($("#txt-UserCount").val())) {
                alert("续约人数小于当前用户量总数，请先删除部分用户，然后再续费");
                return false;
            }
            if (quantity > 499) {
                alert("购买人数超过500人，请联系我们，为您专业定制");
                return false;
            }
        }

        Global.post("/Auction/GetBestWay",
            {
                quantity: quantity,
                years: userYear,
                type: _self.Params.type
            },
            function (data) {
                var yearCount = 1;
                if (ObjectJS.Params.type == 1 || ObjectJS.Params.type == 3) {
                    yearCount = $("#UserYear").val();
                }
                else {
                    yearCount = data.PeriodQuantity;
                }

                $(".productListTB td.tdBG2").removeClass("tdBG2").find("span").remove();

                for (var i = 0; len = data.Items.length, i < len; i++) {
                    var item = data.Items[i];
                    var productCountHtml = '';
                    if (item.count > 1)
                        productCountHtml = '<span class="mLeft15" style="font-size:16px;color:#fff;">×' + item.count + '</span>';
                    $(".productListTB td[data-productID='" + item.id + "'][data-year='" + yearCount + "']").addClass("tdBG2").append(productCountHtml).data("productCount", item.count);
                }

                //产品的人数、金额
                $("#UserCount").val(data.TotalQuantity);
                var RealAmount = 0;
                if (ObjectJS.Params.type == 1 || ObjectJS.Params.type == 3) {

                    RealAmount = data.TotalMoney;
                }
                else {
                    RealAmount = data.Amount;
                }
                RealAmount = parseFloat(RealAmount * data.Discount).toFixed(2);
                $("#Price").html(RealAmount);
                ObjectJS.Discount = data.Discount;

            });
    }

    //获取总金额、人数
    ObjectJS.getTotalPrice = function () {
        //统计产品的人数、金额
        var $arr = $(".productListTB td.tdBG2");
        var len = $arr.length;
        var userCount = 0;
        var yearCount = 0;
        var totalPrice = 0;
        for (var i = 0; i < len; i++) {
            userCount += (parseInt($arr.eq(i).data("usercount")) * parseInt($arr.eq(i).data("productCount")));
            totalPrice += (parseInt($arr.eq(i).data("price")) * parseInt($arr.eq(i).data("productCount")));
            yearCount = parseInt($arr.eq(i).data("year"));
        }
        $("#UserCount").val(userCount);
        $("#Price").html(totalPrice * ObjectJS.Discount);
    }

    //进入确认订单页
    ObjectJS.sureOrder = function (noSet) {
        var _self = this;

        if (!noSet) {
            if (!VerifyObject.isPass()) {
                return false;
            };

            var quantity = $("#UserCount").val();
            if (quantity == '') return false;

            quantity = parseInt(quantity);
            if (_self.Params.type == 1) {
                if (quantity < 9) {
                    alert("首次购买人数不能少于10人");
                    return false;
                }
                if (quantity > 499) {
                    alert("您公司人数超过500人，请联系我们，为您专业定制");
                    return false;
                }

            }
            else if (_self.Params.type == 2) {
                if (quantity < 4) {
                    alert("购买人数不能少于5人");
                    return false;
                }
                if (quantity > 499) {
                    alert("购买人数超过500人，请联系我们，为您专业定制");
                    return false;
                }
            }
            else if (_self.Params.type == 3) {
                if (quantity < parseInt($("#txt-UserCount").val())) {
                    alert("续约人数小于当前用户量总数，请先删除部分用户，然后再续费");
                    return false;
                }
                if (quantity > 499) {
                    alert("购买人数超过500人，请联系我们，为您专业定制");
                    return false;
                }
            }
        }


        $(".productList").hide();
        $(".productOrder").show();

        $(".selectProduct").css("background", "url('/modules/images/auction/bg-select-product.png') center center");
        $(".selectProduct").children().eq(0).addClass("stepIcoFinish");
        $(".selectProduct").children().eq(1).addClass("stepDesFinish");

        $(".sureOrder").css("background", "url('/modules/images/auction/bg-sure-order-active.png') center center");
        $(".sureOrder").children().eq(0).addClass("stepIcoActive");
        $(".sureOrder").children().eq(1).addClass("stepDesActive");

        if (!noSet) {
            $("#orderUserCount").html($("#UserCount").val());
            $("#orderYear").html($("#UserYear").val());
            $("#orderPrice").html($("#Price").html());
        }
    }

    //根据人数、年数生成客户订单
    ObjectJS.addClientOrder = function () {
        var _self = this;

        var UserYear = 1;
        if (_self.Params.type != 2)
            UserYear = $("#UserYear").val();

        Global.post("/Auction/AddClientOrder",
            {
                quantity: $("#UserCount").val(),
                years: UserYear,
                type: _self.Params.type
            },
            function (data) {
                if (data.ID) {
                    $("#orderAmount").html(data.RealAmount);
                    ObjectJS.RealAmount = data.RealAmount;
                    ObjectJS.Params.orderID = data.ID;
                }

            }
        );
    }

    //去支付宝进行支付
    ObjectJS.toPayOrder = function () {
        var html = "<div>支付完成前，请不要关闭此支付验证窗口。<br/>支付完成后，请根据您支付的情况点击下面按钮。</div><div class='pAll20'>";
        html += "<div class='btn left' style='background-color:#999;' onclick='location.href=\"/System/Client/4\"'>支付遇到问题</div>";
        html += "<div class='btn right' onclick='location.href=\"/System/Client/2\"'>支付完成</div>";
        html += "<div class='clear'></div></div>";
        Easydialog.open({
            container: {
                id: "",
                header: "支付提示",
                content: html,
                callback: function () {

                }
            }
        });

        var $alink = $("<a  id='alink' href='" + "/Auction/GoAlipayPay/" + ObjectJS.Params.orderID + "' target='_blank'></a>");
        $("body").append($alink);
        document.getElementById("alink").click();
    }

    //格式化金额
    ObjectJS.formatCurrency = function (num) {
        num = num.toString().replace(/\$|\,/g, '');
        if (isNaN(num))
            num = "0";
        sign = (num == (num = Math.abs(num)));
        num = Math.floor(num * 100 + 0.50000000001);
        cents = num % 100;
        num = Math.floor(num / 100).toString();
        if (cents < 10)
            cents = "0" + cents;
        for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3) ; i++)
            num = num.substring(0, num.length - (4 * i + 3)) + ',' +
            num.substring(num.length - (4 * i + 3));
        return (((sign) ? '' : '-') + num + '.' + cents);
    }

    module.exports = ObjectJS;
});