define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot")

    var ObjectJS = {};

    ObjectJS.init = function (isExists) {
        ObjectJS.bindEvent(isExists);
    }

    ObjectJS.bindEvent = function (isExists) {

        //绑定手机
        $("#saveLoginMobile").click(function () {
            ObjectJS.saveAccountBindMobile(isExists);
        });

        $("#btnSubmit").click(function () {            
            Global.post("/Default/FinishInitSetting",{}, function (data) {               
                if (data.result) {
                    window.location = "/Home/Index";
                }
                else {
                    alert("网络出现异常,请稍后重试!");
                }
            })

        });

        //获取手机验证码
        $("#SendBindMobileCode").click(function () {
            var BindMobile = $("#mobilePhone").val();
            if (BindMobile != '') {
                if (Global.validateMobilephone(BindMobile)) {
                    Global.post("/Default/IsExistLoginName", { loginName: BindMobile }, function (data) {
                        if (data.result) {
                            $(".validation").html("手机已存在").css("color", "red");
                        }
                        else {
                            $(".validation").html("");

                            ObjectJS.sendMobileMessage("SendBindMobileCode", BindMobile);
                        }
                    });
                }
                else {
                    $(".validation").html("手机格式有误").css("color", "red");
                }
            }
            else {
                $(".validation").html("手机不能为空").css("color", "red");
            }
        });

        $("#pwd").blur(function () {
            if (!$(this).val().trim()) {
                $("#validationPwd").html("密码不能为空");
            } else if ($(this).val().trim().length < 6) {
                $("#validationPwd").html("密码不能低于六位");
            } else {
                $("#validationPwd").html("");
            }
        });

        $("#confirmPwd").blur(function () {
            if (!$(this).val().trim()) {
                $("#validationConfirmPwd").html("确认密码不能为空");
            } else if ($("#pwd").val().trim() != $("#confirmPwd").val().trim()) {
                $("#validationConfirmPwd").html("密码和确认密码不一致");
            } else {
                $("#validationConfirmPwd").html("");
            }
        });
    }

    //绑定手机
    ObjectJS.saveAccountBindMobile = function (isExists) {

        var BindMobile = $("#mobilePhone").val();
        var BindMobileCode = $("#BindMobileCode").val();
        
        if (BindMobile != '') {
            if (Global.validateMobilephone(BindMobile)) {

                if (BindMobileCode == "") {
                    $(".validation").html("验证码不能为空");
                    return;
                }
                if (isExists == 0) {
                    if (!$("#pwd").val().trim() || $("#pwd").val().trim().length < 6) {
                        $("input").blur();
                        return;
                    } else if ($("#pwd").val().trim() != $("#confirmPwd").val().trim()) {
                        $("input").blur();
                        return;
                    }

                }
                Global.post("/Default/IsExistLoginName", { loginName: BindMobile }, function (data) {
                    if (data.result) {
                        $(".validation").html("手机已存在");
                    } else {

                        $(".validation").html("");
                       
                        
                        Global.post("/Home/ValidateMobilePhoneCode", { mobilePhone: BindMobile, code: BindMobileCode }, function (data) {
                            if (data.Result == 0) {
                                $(".validation").html("验证码有误");
                            } else {
                                Global.post("/Default/AccountBindMobile", { BindMobile: BindMobile }, function (data) {
                                    if (data.result) {
                                        window.location = "/Default/SettingHelp";
                                    } else {
                                        alert("网络出现异常,请稍后重试!");
                                    }
                                });
                            }
                        });
                    }
                });
            } else {
                $(".validation").html("手机格式有误");
            }
        } else {
            $(".validation").html("手机不能为空");
        }
    }        
    

    //发送手机验证码
    var timeCount = 60;
    var interval = null;
    ObjectJS.sendMobileMessage = function (id, mobilePhone) {
        var $btnSendCode = $("#" + id);
        $btnSendCode.attr("disabled", "disabled");

        Global.post("/Home/SendMobileMessage", { mobilePhone: mobilePhone }, function (data) {
            if (data.Result == 1) {
                $("#BindMobileCode").focus();
                $("#" + id).css("background-color", "#aaa");
                interval = setInterval(function () {                    
                    timeCount--;
                    var $btnSendCode = $("#" + id);
                    $btnSendCode.val(timeCount + "秒后重发");

                    if (timeCount == 0) {
                        clearInterval(interval);
                        timeCount = 60;
                        $btnSendCode.val("获取验证码").css("background-color", "#4a98e7");
                        $btnSendCode.removeAttr("disabled");
                    }
                }, 1000);
            }
            else {
                var $btnSendCode = $("#" + id);
                alert("验证码发送失败");
                $btnSendCode.removeAttr("disabled");
            }

        });
    }


    module.exports = ObjectJS;
})