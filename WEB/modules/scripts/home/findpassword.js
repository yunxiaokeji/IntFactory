define(function (require, exports, module) {
    var Global = require("global");

    var ObjectJS = {};

    //初始化
    ObjectJS.init = function () {
        var _self = this;
        _self.placeholderSupport();
        _self.bindEvent();
        
    }

    //绑定事件
    ObjectJS.bindEvent = function () {
        $(document).on("keypress", function (e) {
            if (e.keyCode == 13) {
                $("#btnUpdateUserPwd").click();
            }
        });

        //密码文本
        $("#txtBoxPassword").click(function () {
            $(this).hide();
            $("#loginPWD").focus();
        });

        //确认密码文本
        $("#txtBoxSurePassword").click(function () {
            $(this).hide();
            $("#loginSurePWD").focus();
        });

        //密码
        $("#loginPWD").blur(function () {
            if ($("#loginPWD").val() == '') {
                $("#txtBoxPassword").show();
                $("#loginPWD").next().fadeIn().find(".error-msg").html("密码不能为空");
            }
            else {
                if ($("#loginPWD").val().length < 6 || $("#loginPWD").val().length > 25) {
                    $("#loginPWD").next().fadeIn().find(".error-msg").html("密码，6-25位");
                }
                else {
                    if (Global.passwordLevel($("#loginPWD").val()) == 1) {
                        $("#loginPWD").next().fadeIn().find(".error-msg").html("密码至少两种不同组合");
                    }
                    else {
                        $("#loginPWD").next().hide().find(".error-msg").html("");
                    }
                }
            }
        });

        //确认密码
        $("#loginSurePWD").blur(function () {
            if ($("#loginSurePWD").val() == '') {
                $("#txtBoxSurePassword").show();
                $("#loginSurePWD").next().fadeIn().find(".error-msg").html("确认密码不能为空");
            }
            else {
                if ($("#loginSurePWD").val() != $("#loginPWD").val()) {
                    $("#loginSurePWD").next().fadeIn().find(".error-msg").html("密码不一致");
                }
                else {

                    $("#loginSurePWD").next().hide().find(".error-msg").html("");
                }
            }
        }).focus(function () {
            if ($("#loginSurePWD").val() == '') {
                $("#txtBoxSurePassword").hide();
            }
        });

        //发送验证码
        $("#btnSendMsg").click(function () {
            if ($("#loginName").val() == '') {
                $("#code-error").fadeIn().find(".error-msg").html("手机号不能为空");
                return;
            }
            else {
                if (Global.validateMobilephone($("#loginName").val())) {
                    Global.post("/Home/IsExistLoginName", { loginName: $("#loginName").val() }, function (data) {
                        if (data.Result == 0) {
                            $("#code-error").fadeIn().find(".error-msg").html("手机号不存在");
                        }
                        else {
                            $("#code-error").hide().find(".error-msg").html("");
                            ObjectJS.SendMobileMessage("btnSendMsg", $("#loginName").val());
                        }
                    });
                }
                else {
                    $("#code-error").fadeIn().find(".error-msg").html("手机号格式不对");
                    return;
                }
            }

        });

        //重置密码
        $("#btnUpdateUserPwd").click(function () {
            ObjectJS.validateData();
        });

    }

    //验证数据
    ObjectJS.validateData = function () {
        if ($("#loginName").val() == '') {
            $("#loginName").next().fadeIn().find(".error-msg").html("手机号不能为空");
        }
        else {
            if (Global.validateMobilephone($("#loginName").val())) {
                Global.post("/Home/IsExistLoginName", { loginName: $("#loginName").val() }, function (data) {
                    if (data.Result == 0) {
                        $("#loginName").next().fadeIn().find(".error-msg").html("手机号没有注册");
                    }
                    else {
                        $("#loginName").next().hide().find(".error-msg").html("");

                        //验证码
                        if ($("#code").val() == '') {
                            $("#code-error").fadeIn().find(".error-msg").html("验证码不能为空");
                        }
                        else {
                            Global.post("/Home/ValidateMobilePhoneCode", { mobilePhone: $("#loginName").val(), code: $("#code").val() }, function (data) {
                                if (data.Result == 0) {
                                    $("#code-error").fadeIn().find(".error-msg").html("验证码有误");
                                }
                                else {
                                    $("#code-error").hide().find(".error-msg").html("");

                                    //密码
                                    if ($("#loginPWD").val() == '') {
                                        $("#loginPWD").next().fadeIn().find(".error-msg").html("密码不能为空");
                                    }
                                    else {
                                        if ($("#loginPWD").val().length < 6 || $("#loginPWD").val().length > 25) {
                                            $("#loginPWD").next().fadeIn().find(".error-msg").html("密码，6-25位");
                                        }
                                        else {
                                            if (Global.passwordLevel($("#loginPWD").val()) == 1) {
                                                $("#loginPWD").next().fadeIn().find(".error-msg").html("密码至少两种不同组合");
                                            }
                                            else {
                                                $("#loginPWD").next().hide().find(".error-msg").html("");

                                                //确认密码
                                                if ($("#loginSurePWD").val() == '') {
                                                    $("#loginSurePWD").next().fadeIn().find(".error-msg").html("确认密码不能为空");
                                                }
                                                else {
                                                    if ($("#loginSurePWD").val() != $("#loginPWD").val()) {
                                                        $("#loginSurePWD").next().fadeIn().find(".error-msg").html("密码不一致");
                                                    }
                                                    else {
                                                        $("#loginSurePWD").next().hide().find(".error-msg").html("");
                                                        
                                                        ObjectJS.updateUserPwd();
                                                    }
                                                }

                                            }
                                        }
                                    }


                                }
                            });
                        }

                    }
                });
            }
            else {
                $("#loginName").next().fadeIn().find(".error-msg").html("输入正确手机号");
            }
        }

        return ObjectJS.isDataOk;
    };

    //保存实体
    ObjectJS.updateUserPwd = function () {
        $("#btnUpdateUserPwd").html("重置中...");

        var paras = {
            loginName: $("#loginName").val(),
            code: $("#code").val(),
            loginPwd: $("#loginPWD").val()

        };

        Global.post("/Home/UpdateUserPwd", paras, function (data) {
            $("#btnUpdateUserPwd").html("重置");

            if (data.Result == 1) {
                location.href = "/Home/Index";
            }
            else if (data.Result == 0) {
                alert("重置失败");
            }
            else if (data.Result == 2) {
                $("#loginName").next().fadeIn().find(".error-msg").html("手机号没有注册");
            }
            else if (data.Result == 3) {
                $("#code-error").fadeIn().find(".error-msg").html("验证码有误");
            }
        })
    }

    //属性placeholder IE兼容
    ObjectJS.placeholderSupport = function () {
        if (!('placeholder' in document.createElement('input'))) {   // 判断浏览器是否支持 placeholder
            $('[placeholder]').focus(function () {
                var input = $(this);
                input.css("color", "#333");
                if (input.val() == input.attr('placeholder')) {
                    input.val('');
                    input.removeClass('placeholder');
                }

            }).blur(function () {
                var input = $(this);
                if (input.val() == '' || input.val() == input.attr('placeholder')) {
                    input.addClass('placeholder');
                    input.val(input.attr('placeholder')).css("color", "#999");
                }
            }).blur();

        };
    }

    //发送手机验证码
    var timeCount = 60;
    var interval = null;
    ObjectJS.SendMobileMessage = function (id, mobilePhone) {
        var $btnSendCode = $("#" + id);
        $btnSendCode.attr("disabled", "disabled");

        Global.post("/Home/SendMobileMessage", { mobilePhone: mobilePhone }, function (data) {

            if (data.Result == 1) {
                $("#" + id).css("background-color", "#aaa");
                interval = setInterval(function () {
                    var $btnSendCode = $("#" + id);
                    timeCount--;
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
});