define(function (require, exports, module) {
    var Global = require("global"),
        Easydialog = require("easydialog")

    var ObjectJS = {};

    //初始化
    ObjectJS.init = function (type,orderID) {
        var _self = this;
        ObjectJS.actionType = type;//1:付费购买；2：购买人数;3:续费
        ObjectJS.discount = 1;//购买折扣
        ObjectJS.orderID = orderID;
        
        _self.bindEvent();
        //处理未完成的订单
        if (orderID == "") {
            _self.getList();
        }

        $("#UserCount").focus();
    }

    //绑定事件
    ObjectJS.bindEvent = function () {
        
        //选择人数 获取最佳产品组合
        $("#UserCount").blur(function () {
            ObjectJS.getBestWay();
        });

        //选择购买年份
        $("#UserYear").change(function () {
            $(".product-list td.active").each(function () {
                var yearCount = $("#UserYear").val();

                var _self = $(this);
                var id = _self.data("id");
                var year = _self.data("year");
                var productCount = _self.data("productCount");
                if (year == yearCount) { return; }
               
                $(this).removeClass("active");

                var $span = $(this).find("span");
                $(this).find("span").remove();
                $(".product-list td[data-id='" + id + "'][data-year='" + yearCount + "']").addClass("active").append($span).data("productCount", productCount);

            });

            //统计产品的人数、金额
            ObjectJS.getTotalPrice();

        });

        //进入确认购买信息
        $("#btn-sureOrder").click(function () {
            if (!ObjectJS.validate(1)) { return; };

            $(this).parent().parent().hide().next().fadeIn(500);

            var $nav = $(".header-nav .nav-hover");
            $nav.removeClass("nav-hover").addClass("nav-finish");
            $nav.parent().next().next().find(".nav").addClass("nav-hover").next().addClass("nav-des-hover");

            $("#order-usercount").html($("#UserCount").val());
            if (ObjectJS.actionType == 2) {
                $("#order-useryear").html($("#UserYear").val() + "月");
            }
            else {
                $("#order-useryear").html($("#UserYear").val() + "年");
            }

            var totalmoney =parseFloat( $("#TotalMoney").html() );
            var trueamount = parseFloat(totalmoney / ObjectJS.discount);
            var freeamount = parseFloat(trueamount * (1 - ObjectJS.discount));
            $("#order-amount").html(totalmoney.toFixed(2));
            $("#order-trueamount").html("￥" + trueamount.toFixed(2));
            $("#order-freeamount").html("￥" + freeamount.toFixed(2));
        });

        //返回选择产品
        $("#btn-selectPrpduct").click(function () {
            $(this).parent().hide().prev().fadeIn(500);

            var $nav = $(".header-nav .nav-hover");
            $nav.removeClass("nav-hover").next().removeClass("nav-des-hover");
            $nav.parent().prev().prev().find(".nav").addClass("nav-hover").next().addClass("nav-des-hover");
        });

        //确认购买
        $("#btn-sureBuyOrder").click(function () {
            ObjectJS.sureAddOrder();
        });

        //去付款订单
        $("#btn-payorder").click(function () {
            ObjectJS.toPayOrder();
        });

    }

    //获取购买产品列表
    ObjectJS.getList = function () {
        $(".product-list .tb-thead").nextAll().remove();
        $(".product-list .tb-thead").after("<tr><td colspan='4'><div class='data-loading'><div></td></tr>");

        Global.post("/Auction/GetProductList",
            null,
            function (data) {
                $(".product-list .tb-thead").nextAll().remove();

                var len = data.Items.length;
                var html1 = '<tr class="tr-bg"><td><span class="user-box">5用户</span></td>';
                var html2 = '<tr><td><span class="user-box">10用户</span></td>';
                var html3 = '<tr class="tr-bg"><td><span class="user-box">20用户</span></td>';
                var html4 = '<tr><td><span class="user-box">50用户</span></td>';
                var html5 = '<tr class="tr-bg"><td><span class="user-box">100用户</span></td>';

                for (var i = 0; i < len; i++) {
                    var item = data.Items[i];
                    var price = ObjectJS.formatCurrency(item.Price);

                    if (item.UserQuantity == 5) {
                        html1 += '<td  name="productItem" data-id="1" data-productID="' + item.ProductID + '" data-usercount="5" data-price="' + item.Price + '" data-year="' + item.PeriodQuantity + '">' + price;
                        html1 += '</td>';
                    }
                    else if (item.UserQuantity == 10) {
                        html2 += '<td  name="productItem" data-id="2" data-productID="' + item.ProductID + '" data-usercount="10" data-price="' + item.Price + '" data-year="' + item.PeriodQuantity + '">' + price;
                        html2 += '</td>';
                    }
                    else if (item.UserQuantity == 20) {
                        html3 += '<td  name="productItem" data-id="3" data-productID="' + item.ProductID + '" data-usercount="20" data-price="' + item.Price + '" data-year="' + item.PeriodQuantity + '">' + price;
                        html3 += '</td>';
                    }
                    else if (item.UserQuantity == 50) {
                        html4 += '<td  name="productItem" data-id="4" data-productID="' + item.ProductID + '" data-usercount="50" data-price="' + item.Price + '" data-year="' + item.PeriodQuantity + '">' + price;
                        html4 += '</td>';
                    }
                    else if (item.UserQuantity == 100) {
                        html5 += '<td  name="productItem" data-id="5" data-productID="' + item.ProductID + '" data-usercount="100" data-price="' + item.Price + '" data-year="' + item.PeriodQuantity + '">' + price;
                        html5 += '</td>';
                    }
                }

                html1 += '</tr>';
                html2 += '</tr>';
                html3 += '</tr>';
                html4 += '</tr>';
                html5 += '</tr>';
                var html = html1 + html2 + html3 + html4 + html5;
                $(".product-list .tb-thead").after(html);

                //if (ObjectJS.Params.type == 3) {
                //    ObjectJS.getBestWay();
                //}

            }
        );
    }

    //获取最佳产品组合
    ObjectJS.getBestWay = function () {
        if (!ObjectJS.validate(0)) { return; }

        var _self = this;
        var userCount =  $("#UserCount").val();
        var userYear = 1;

        if (_self.actionType == 1 || _self.actionType == 3) {
            userYear = $("#UserYear").val();
        }

        Global.post("/Auction/GetBestWay",
            {
                quantity: userCount,
                years: userYear,
                type: _self.actionType
            },
            function (data) {
                var yearCount = 1;
                if (ObjectJS.actionType == 1 || ObjectJS.actionType == 3) {
                    yearCount = $("#UserYear").val();
                }
                else {
                    yearCount = data.PeriodQuantity;
                }

                $(".product-list td.active").removeClass("active").find("span").remove();
                for (var i = 0; len = data.Items.length, i < len; i++) {
                    var item = data.Items[i];

                    var productHtml = '';
                    if (item.count > 1) {
                        productHtml = '<span class="product-count">×' + item.count + '</span><span class="iconfont">&#xe613;</span>';
                    }
                    else {
                        productHtml = '<span class="iconfont">&#xe613;</span>';
                    }
                    $(".product-list td[data-productID='" + item.id + "'][data-year='" + yearCount + "']").addClass("active").append(productHtml).data("productCount", item.count);
                }

                //产品的人数、金额
                $("#UserCount").val(data.TotalQuantity);

                var RealAmount = 0;
                if (ObjectJS.actionType == 1 || ObjectJS.actionType == 3) {
                    RealAmount = data.TotalMoney;
                }
                else {
                    RealAmount = data.Amount;
                }
                ObjectJS.discount = data.Discount;
                RealAmount = parseFloat(RealAmount * data.Discount).toFixed(2);
                $("#TotalMoney").html(RealAmount);
                
            });
    }

    //验证数据
    ObjectJS.validate = function (option) {
        var _self = this;
        var userCount = $("#UserCount").val();
        
        if (userCount == '') {
            if (option == 1) {
                alert("人数不能为空");
            }
            return false;
        }

        if (!userCount.isInt()) {
            alert("人数填写有误");
            return false;
        }
        userCount = parseInt(userCount);
        if (userCount > 499) {
            alert("购买人数超过500人，请联系我们，为您专业定制");
            return false;
        }

        if (_self.actionType == 1) {
            if (userCount < 5) {
                alert("首次购买人数不能少于5人");
                return false;
            }
        }
        else if (_self.actionType == 2) {
            if (userCount < 5) {
                alert("购买人数不能少于5人");
                return false;
            }

        }
        else if (_self.actionType == 3) {
            if (userCount < parseInt($("#txt-userCount").val())) {
                alert("续约人数小于当前用户量总数，请先删除部分用户，然后再续费");
                return false;
            }
        }

        return true;
    }

    //获取总金额、人数
    ObjectJS.getTotalPrice = function () {
        //统计产品的人数、金额
        var $arr = $(".product-list td.active");
        var len = $arr.length;
        var userCount = 0;
        var totalAmount = 0;
        for (var i = 0; i < len; i++) {
            var $item = $arr.eq(i);
            userCount += ( parseInt($item.data("usercount")) * parseInt($item.data("productCount")) );
            totalAmount += (parseInt($item.data("price")) * parseInt($item.data("productCount")));
        }
        $("#UserCount").val(userCount);
        $("#TotalMoney").html( parseFloat( totalAmount * ObjectJS.discount ).toFixed(2) );
    }

    //确认购买 生成订单
    ObjectJS.sureAddOrder = function () {
        var _self = this;
        var userCount=$("#UserCount").val();
        var userYear = 1;
        if (_self.actionType != 2) {
            userYear = $("#UserYear").val();
        }

        Global.post("/Auction/AddClientOrder",
            {
                quantity: userCount,
                years: userYear,
                type: _self.actionType
            },
            function (data) {
                if (data.ID)
                {
                    $("#pay-amount").html(data.RealAmount);
                    ObjectJS.orderID = data.ID;

                    $("#btn-sureBuyOrder").parent().hide().next().fadeIn(500);

                    var $nav = $(".header-nav .nav-hover");
                    $nav.removeClass("nav-hover").addClass("nav-finish");
                    $nav.parent().next().next().find(".nav").addClass("nav-hover").next().addClass("nav-des-hover");
                    $nav.parent().next().addClass("nav-line-finish");
                }

            }
        );
    }

    //去支付宝进行支付
    ObjectJS.toPayOrder = function () {
        var html = "<div>支付完成前，请不要关闭此支付验证窗口</div><div style='padding:10px 0;'>";
        html += "<div class='btn right' id='btn-finishPay'>支付完成</div><div class='clear'></div>";
        html += "</div>";
        Easydialog.open({
            container: {
                id: "",
                header: "支付提示",
                content: html
            }
        });

        $("#btn-finishPay").unbind().bind("click", function () {
            ObjectJS.finishPay();
        });

        var $alink = $("<a id='alink' href='" + "/Auction/GoAlipayPay/" + ObjectJS.orderID + "' target='_blank'></a>");
        $("body").append($alink);
        document.getElementById("alink").click();
       
    }

    //完成支付
    ObjectJS.finishPay = function () {
        Easydialog.close();

        $("#btn-payorder").parent().hide().next().fadeIn(500);

        var $nav = $(".header-nav .nav-hover");
        $nav.removeClass("nav-hover").addClass("nav-finish");
        $nav.parent().next().next().find(".nav").addClass("nav-hover").next().addClass("nav-des-hover");
        $nav.parent().next().addClass("nav-line-finish");

        Global.post("/Auction/GetOrderInfo", { id: ObjectJS.orderID },
            function (data) {
                if (data.result == 1) {
                    $(".result-success").show();

                    $("#preUserCount").html(data.preUserCount);
                    $("#nowUserCount").html(data.nowUserCount);
                    $("#preEndTime").html(data.preEndTime);
                    $("#nowEndTime").html(data.nowEndTime);

                    if (data.type == 3) {
                        $(".pay-tb-info tr").each(function () {
                            $(this).find("td").eq(1).remove();
                        });

                        $(".pay-tb-info tr").eq(0).find("td:last").html("续费后");
                    }
                }
                else {
                    $(".result-error").show();
                }
            });

    }

    //格式化金额
    ObjectJS.formatCurrency = function (s) {
        s = parseFloat((s + "").replace(/[^\d\.-]/g, "")) + "";   
        var l = s.split(".")[0].split("").reverse();   
        t = "";   
        for(i = 0; i < l.length; i ++ )   
        {   
            t += l[i] + ((i + 1) % 3 == 0 && (i + 1) != l.length ? "," : "");   
        }   
        return t.split("").reverse().join("");   
    }

    module.exports = ObjectJS;
});