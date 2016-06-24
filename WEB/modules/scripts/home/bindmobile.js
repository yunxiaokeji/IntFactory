define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot")

    var ObjectJS = {};

    ObjectJS.init = function () {
        ObjectJS.bindEvent();
    }

    ObjectJS.bindEvent = function () {

        //绑定手机
        $("#saveLoginMobile").click(function () {
            ObjectJS.saveAccountBindMobile();
        });

        $("#btnSubmit").click(function () {            
            Global.post("/Default/FinishInitSetting",{}, function (data) {               
                if (data.result) {
                    window.location = "/Default/SettingHelp";
                }
                else {
                    alert("跳转中,请稍后");                    
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
                            $(".validation").html("（*密码初始为手机号）");

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
    }

    //绑定手机
    ObjectJS.saveAccountBindMobile = function () {

        var BindMobile = $("#mobilePhone").val();
        var BindMobileCode = $("#BindMobileCode").val();
        
        if (BindMobile != '') {
            if (Global.validateMobilephone(BindMobile)) {
                Global.post("/Default/IsExistLoginName", { loginName: BindMobile }, function (data) {
                    if (data.result) {
                        $(".validation").html("手机已存在").css("color", "red");
                    }
                    else {
                        $(".validation").html("（*密码初始为手机号）");
                        if (BindMobileCode == "") {
                            $(".validation").html("验证码不能为空").css("color", "red");
                        }
                        else {
                            Global.post("/Home/ValidateMobilePhoneCode", { mobilePhone: BindMobile, code: BindMobileCode }, function (data) {
                                if (data.Result == 0) {
                                    $(".validation").html("验证码有误").css("color", "red");
                                }
                                else {                                    
                                    Global.post("/Default/AccountBindMobile", { BindMobile: BindMobile }, function (data) {
                                        if (data.result) {                                            
                                            window.location = "/Default/SettingHelp";
                                        } else {
                                            alert("正在跳转中,请稍等");
                                        }
                                       
                                    });
                                }
                            });
                        }
                    }
                });
            }
            else {
                $(".validation").html("手机格式有误").css("color", "red");
            }
        } else {
            $(".validation").html("手机不能为空").css("color", "red");
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