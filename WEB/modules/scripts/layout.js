/*
*布局页JS
*/
define(function (require, exports, module) {
    var $ = require("jquery"),
        doT = require("dot"),
        Global = require("global"),
        Easydialog = require("easydialog");

    var LayoutObject = {};
    //初始化数据
    LayoutObject.init = function () {
        
        LayoutObject.bindStyle();
        LayoutObject.bindEvent();
        LayoutObject.getAuthorizeInfo();
        //LayoutObject.bindUpcomings();
        LayoutObject.placeholderSupport();
    }

    //待办小红点
    LayoutObject.bindUpcomings = function () {
        Global.post("/Base/GetClientUpcomings", {}, function (data) {
            if (!data.items) { return; }

            for (var i = 0; i < data.items.length; i++) {
                var item = data.items[i];
                //采购
                if (item.DocType == 1) {
                    if ($("#modulesMenu li[data-code='103000000']").find(".point").length == 0) {
                        $("#modulesMenu li[data-code='103000000']").find(".name").after("<span class='point'></span>");
                    }

                    var controller = $("nav .controller[data-code='103010000']");
                    if (controller.find(".point").length == 0) {
                        controller.find(".controller-box .name").after("<span class='point'></span>");
                    }
                    if (controller.find("li[data-code='103020200']").find(".point").length == 0) {
                        controller.find("li[data-code='103020200']").find(".name").append("<span class='point'></span>");
                    }
                } else if (item.DocType == 21) { //订单
                    if ($("#modulesMenu li[data-code='102000000']").find(".point").length == 0) {
                        $("#modulesMenu li[data-code='102000000']").find(".name").after("<span class='point'></span>");
                    }

                    var controller = $("nav .controller[data-code='102010000']");
                    if (controller.find(".point").length == 0) {
                        controller.find(".controller-box .name").after("<span class='point'></span>");
                    }

                    if (item.ReturnStatus == 0 && item.SendStatus == 0) {
                        if (controller.find("li[data-code='102010100']").find(".point").length == 0) {
                            controller.find("li[data-code='102010100']").find(".name").append("<span class='point'></span>");
                        }
                    }
                } else if (item.DocType == 111) { //任务
                    if ($("#modulesMenu li[data-code='109000000']").find(".point").length == 0) {
                        $("#modulesMenu li[data-code='109000000']").find(".name").after("<span class='point'></span>");
                    }

                    var controller = $("nav .controller[data-code='109010000']");
                    if (controller.find(".point").length == 0) {
                        controller.find(".controller-box .name").after("<span class='point'></span>");
                    }

                    if (item.SendStatus == 1) {
                        if (controller.find("li[data-code='109010100']").find(".point").length == 0) {
                            controller.find("li[data-code='109010100']").find(".name").append("<span class='point'></span>");
                        }
                    }
                }
            }
        });
    }

    //绑定事件
    LayoutObject.bindEvent = function () {
        var _self = this;
        //调整浏览器窗体
        $(window).resize(function () {
            LayoutObject.bindStyle();
        });

        $(document).click(function (e) {

            if (!$(e.target).parents().hasClass("currentuser") && !$(e.target).hasClass("currentuser")) {
                $(".dropdown-userinfo").fadeOut("1000");
            }

            if (!$(e.target).parents().hasClass("companyname") && !$(e.target).hasClass("companyname")) {
                $(".dropdown-companyinfo").fadeOut("1000");
            }
        });

        $(".controller-box").click(function () {
            var _this = $(this).parent();
            if (!_this.hasClass("select")) {
                _self.setRotateR(_this.find(".open"), 0, 90);
                _this.addClass("select");
                _this.find(".action-box").slideDown(200);
            } else {
                _self.setRotateL(_this.find(".open"), 90, 0);
                _this.removeClass("select");
                _this.find(".action-box").slideUp(200);
            }
        });

        $(".ico-help").hover(function () {
            $(".wechat").css({ "bottom": "105px", "left": "70px" });

            $(".wechat img").css({ "width": "150px", "height": "150px" });
            
        }, function () {
            $(".wechat").css({ "bottom": "70px", "left": "44px" });

            $(".wechat img").css({ "width": "0", "height": "0" });
        })
        
        //登录信息展开
        $("#currentUser").click(function () {
            $(".dropdown-userinfo").fadeIn("1000");
        });

        //公司名称信息展开
        $("header .companyname").click(function () {
            $(".dropdown-companyinfo").fadeIn("1000");
        });

        //一级菜单图标事件处理
        $("#modulesMenu a").mouseenter(function () {
            var _this = $(this).find("img");
            _this.attr("src", _this.data("hover"));
        });

        $("#modulesMenu a").mouseleave(function () {
            if (!$(this).hasClass("select")) {
                var _this = $(this).find("img");
                _this.attr("src", _this.data("ico"));
            }
        });

        $("#modulesMenu .select img").attr("src", $("#modulesMenu .select img").data("hover"));

        //意见反馈浮层
        $(".help-feedback .ico-open").click(function () {
            var _this = $(this);
            if (_this.data("open") && _this.data("open") == "1") {
                _this.data("open", "0");
                _self.setRotateL(_this, 45, 0);

                _this.parent().animate({ "height": "45px" }, "fast");
            } else {
                _this.data("open", "1");
                _self.setRotateR(_this, 0, 45);

                _this.parent().animate({ "height": "135px" }, "fast");
            }
        });

        //关注微信号
        $(".help-feedback .ico-help").click(function () {
            Easydialog.open({
                container: {
                    id: "",
                    header: "厂盟科技官方微信号",
                    content: "<div class='center'><img src='/modules/images/wechat.jpg' /><br><span class='font14'>扫一扫关注微信号</span></div>",
                    yesfn: function () {

                    }
                }
            });
        })

    }

    //旋转按钮（顺时针）
    LayoutObject.setRotateR = function (obj, i, v) {
        var _self = this;
        if (i < v) {
            i += 3;
            setTimeout(function () {
                obj.css("transform", "rotate(" + i + "deg)");
                _self.setRotateR(obj, i, v);
            }, 5)
        }
    }

    //旋转按钮(逆时针)
    LayoutObject.setRotateL = function (obj, i, v) {
        var _self = this;
        if (i > v) {
            i -= 3;
            setTimeout(function () {
                obj.css("transform", "rotate(" + i + "deg)");
                _self.setRotateL(obj, i, v);
            }, 5)
        } 
    }

    //绑定元素定位和样式
    LayoutObject.bindStyle = function () {

    
    }

    //获取代理商授权信息
    LayoutObject.getAuthorizeInfo = function () {
        Global.post("/Home/GetAuthorizeInfo", null, function (data) {
            $("#remainderDays").html(data.remainderDays);
            if (data.authorizeType == 0) {
                $(".btn-buy").html("立即购买");
            }
            else {
                if (parseInt(data.remainderDays) < 31) {
                    $("#remainderDays").addClass("red");
                    $(".btn-buy").html("续费").attr("href", "/Auction/ExtendNow");
                }
                else {
                    $(".btn-buy").html("购买人数").attr("href", "/Auction/BuyUserQuantity");
                }
            }
        });
    }

    // 判断浏览器是否支持 placeholder
    LayoutObject.placeholderSupport = function () {
        if (! ('placeholder' in document.createElement('input')) ) {   
            $('[placeholder]').focus(function () {
                var input = $(this);
                if (input.val() == input.attr('placeholder')) {
                    input.val('');
                    input.removeClass('placeholder');
                }
            }).blur(function () {
                var input = $(this);
                if (input.val() == '' || input.val() == input.attr('placeholder')) {
                    input.addClass('placeholder');
                    input.val(input.attr('placeholder'));
                }
            }).blur();
        };
    }

    module.exports = LayoutObject;
})