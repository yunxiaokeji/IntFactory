define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot");
    var InquireOrder = {};

    InquireOrder.init = function () {
        InquireOrder.bindEvent();        
    };

    InquireOrder.isloading = true;

    InquireOrder.bindEvent = function () {
        //发送验证码
        $(".inquire-form-btn").click(function () {
            var mobilePhone = $(".inquire-form-phone").val();
            if (mobilePhone == '') {
                alert("手机号不能为空");
                return;
            }
            else {
                if (Global.validateMobilephone(mobilePhone)) {
                    $(".inquire-form-btn").val("发送中...");
                    Global.post("/Inquire/SendMobileMessage", { mobilePhone: mobilePhone }, function (data) {
                        $(".inquire-form-btn").val("获取验证码");
                        if (data.Result == 0) {
                            alert("验证码发送失败");
                        }
                        else {
                            $(".inquire-form-button").css({ "background-color": "#4a98e7", "color": "#fff" }).removeAttr("disabled");
                            InquireOrder.SendMobileMessage("inquire-form-btn", mobilePhone);
                        }
                    });
                    
                }
                else {
                    alert("手机号格式有误");
                }
            }
        });

        //查询进度
        $(".inquire-form-button").click(function () {
            var phoneLogin = $(".inquire-form-phone").val();
            var phonevalidation = $(".inquire-form-numb").val();
            if (phoneLogin != "" || phonevalidation != null) {
                Global.post("/Inquire/ValidateMobilePhoneCode", { mobilePhone: phoneLogin, code: phonevalidation }, function (code) {
                    if (code.Result == 0) {
                        alert("验证码有误");                       
                    } else {
                        $(".img-loading").remove();
                        $(".info").remove();
                        InquireOrder.getOrderByPhone();
                    }
                });   

            } else {
                alert("手机号或验证码不能为空");               
            }
        });
        
    }
    
                        //$(".inquire-form-button").attr("disabled", "disabled").css("background", "#d5d5d5");
                //$(".inquire-form-button").click(function () {
                //    $(".img-loading").remove();
                //    $(".info").remove();
                //    InquireOrder.getOrderByPhone();

                //$(".inquire-form-button").attr("disabled", "disabled").css("background", "#d5d5d5");
    

             
        //$(document).keydown(function (e) {
        //    if (e.keyCode === 13) {
        //        $(".inquire-form-button").click();
        //    }
        //});
    }
    //发送手机验证码
    var timeCount = 60;
    var interval = null;
    InquireOrder.SendMobileMessage = function (id, mobilePhone) {
        var $btnSendCode = $("." + id);
        $btnSendCode.attr("disabled", "disabled");

        $("." + id).css("background-color", "#aaa");
        interval = setInterval(function () {
            var $btnSendCode = $("." + id);
            timeCount--;
            $btnSendCode.val(timeCount + "秒后重发");

            if (timeCount == 0) {
                clearInterval(interval);
                timeCount = 60;
                $btnSendCode.val("获取验证码").css("background-color", "#4a98e7");
                $btnSendCode.removeAttr("disabled");
                $btnSendCode.text("获取验证码");

            }

        }, 1000);

    }

    InquireOrder.getOrderByPhone = function () {
        if (InquireOrder.isloading)
        {
            InquireOrder.isloading = false;
            $(".inquire").after('<div class="img-loading"><img style="width:20px;" src="/modules/images/ico-loading.gif" /></div>');
            var mobilePhone = $(".inquire-form-phone").val();

            Global.post("/Inquire/InquireOrderByPhone", { mobilePhone: mobilePhone }, function (data) {
                if (data.items.length > 0) { 
                    doT.exec("/template/inquireorder/inquireorder.html", function (template) {
                        var innerhtml = template(data.items);
                        innerhtml = $(innerhtml);
                        $(".inquire").after(innerhtml);

                        for (var i = 0; i < data.items.length; i++) {
                            var item = data.items[i];
                            var $activeLi = $("#ul-" + item.OrderID).find("li[data-status='" + item.Status + "']");
                            $activeLi.find(".outerround").css({ "border": "2px solid #007aff", "margin-left": "-6px", "background-color": "#fff" });
                            $activeLi.find("span").addClass("complete");
                            $activeLi.find(".round").addClass("completebg");
                            $activeLi.prevAll().find(".connect").addClass("completecon");
                            $activeLi.prevAll().find(".outerround .round").addClass("completebg ");
                            $activeLi.prevAll().find("span").addClass("complete");
                            $activeLi.siblings().find("span").css("padding-left","4px");
                            for (var j = 0; j < item.StatusItems.length; j++) {
                                var statu = item.StatusItems[j];
                                var $statuLi = $("#ul-" + item.OrderID).find("li[data-status='" + statu.Status + "']");
                                $statuLi.find("span").css("float", "left");
                                $statuLi.find(".create-time").text(statu.CreateTime.toDate("yyyy-MM-dd hh:mm:ss")).css({ "float": "left", "margin-left": "15px" });
                                $statuLi.find(".create-time").after('<div class="clear"></div>');
                            }
                        }
                    });
                    InquireOrder.isloading = true;
                }
                else {
                    $(".inquire").after('<div class="info"><div class="infobox row"><div class="info-none"></div></div></div>');
                    InquireOrder.isloading = true;
                }
                $(".img-loading").remove();
            });
        }
        else {
            alert("数据正在加载，请稍等");
        }
         
    }

    module.exports = InquireOrder;
});
