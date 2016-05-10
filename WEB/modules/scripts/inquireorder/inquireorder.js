define(function (require, exports, module) {
    var Global = require("global");

    var InquireOrder = {};

    InquireOrder.init = function () {
        InquireOrder.bindEvent();
    };

    InquireOrder.bindEvent = function () {
        
        //发送验证码
        $(".inquire-form-btn").click(function () {
            var phoneLogin=$(".inquire-form-phone").val();
            if (phoneLogin == '') {
                alert("手机号不能为空");
                return;
            }
            else {
                if (Global.validateMobilephone(phoneLogin)) {
                    InquireOrder.SendMobileMessage("inquire-form-btn", phoneLogin);
                }
                else {
                    alert("手机号格式有误");
                }
            }
        });

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
                $btnSendCode.text("发送中...");
                $(".inquire-form-button").css({"background-color":"#007aff","color":"#fff","font-size":"22px"});
                
                if (timeCount == 0) {
                    clearInterval(interval);
                    timeCount = 60;
                    $btnSendCode.val("获取验证码").css("background-color", "#4a98e7");
                    $btnSendCode.removeAttr("disabled");
                    $btnSendCode.text("获取验证码");
                    $(".inquire-form-button").css({ "background-color": "#d5d5d5", "color": "#666", "font-size": "12px" });
                    
                }

            }, 1000);

            $.post("/Inquire/SendMobileMessage", { mobilePhone: mobilePhone }, function (data) {
                if (data.Result == 0) {
                    alert("验证码发送失败");
                }
            });
        }

        //验证手机验证码
        $(".inquire-form-button").click(function () {
            var phoneLogin = $(".inquire-form-phone").val();
            var phonevalidation = $(".inquire-form-numb").val();
            if (phoneLogin==""||phonevalidation=="") {
                alert("手机号或验证码不能为空");
            } else {
                $.post("/Inquire/ValidateMobilePhoneCode", {mobilePhone:phoneLogin,code:phonevalidation}, function (data) {
                    if (data.Result == 0) {
                        alert("验证码有误");
                    } else {
                        alert("查询成功");
                        
                    }
                });
            }
        });
        
    }
    module.exports = InquireOrder;
});
