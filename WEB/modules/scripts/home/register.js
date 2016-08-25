define(function (require, exports, module) {
    var Global = require("global"),
        Easydialog = require("easydialog");

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function (loginUrl) {
        ObjectJS.placeholderSupport();
        ObjectJS.bindEvent();
        ObjectJS.loginUrl = loginUrl;
    }

    //绑定事件
    ObjectJS.bindEvent = function () {
        $(document).on("keypress", function (e) {
            if (e.keyCode == 13) {
                $("#btnRegister").click();
            }
        });

        //手机号
        $("#loginName").blur(function () {
            var _self = $(this);
            if (_self.val() == '' || !Global.validateMobilephone(_self.val()) ) {
                _self.addClass("errIpt");
            } else {
                _self.removeClass("errIpt");
            }
        });

        //公司名称
        $("#companyName").blur(function () {
            var _self = $(this);
            if (_self.val() == '') {
                _self.addClass("errIpt");
            } else {
                _self.removeClass("errIpt");
            }
        });

        //用户名称
        $("#name").blur(function (){
            var _self = $(this);
            if (_self.val() == '') {
                _self.addClass("errIpt");
            } else {
                _self.removeClass("errIpt");
            }
        });

        //密码
        $("#loginPWD").blur(function () {
            var _self = $(this);
            if (_self.val() == '')
            {
                $("#txtBoxPassword").show();
                _self.addClass("errIpt");
            } else {
                if ($("#loginPWD").val().length < 6 || $("#loginPWD").val().length >25) {
                    _self.addClass("errIpt");
                }else {
                    if (Global.passwordLevel($("#loginPWD").val()) == 1) {
                        _self.addClass("errIpt");
                    }else {
                        _self.removeClass("errIpt");
                    }
                }
            }
        });

        //确认密码
        $("#loginSurePWD").blur(function () {
            var _self = $(this);
            if (_self.val() == '') {
                $("#txtBoxSurePassword").show();
                _self.addClass("errIpt");
            }else {
                if ($("#loginSurePWD").val() != $("#loginPWD").val()) {
                    _self.addClass("errIpt");
                }else {
                    _self.removeClass("errIpt");
                }
            }
        }).focus(function () {
            var _self = $(this);
            if (_self.val() == '') {
                $("#txtBoxSurePassword").hide();
            }
        });

        //发送验证码
        $("#btnSendMsg").click(function () {
            if ($("#loginName").val() == '') {
                $(".registerErr").html("手机号不能为空").slideDown();
                return;
            }
            else {
                if (Global.validateMobilephone($("#loginName").val())) {
                    Global.post("/Home/IsExistLoginName", { loginName: $("#loginName").val() }, function (data) {
                        if (data.Result == 1) {
                            $(".registerErr").html("手机号已被注册").slideDown();
                            return;
                        }
                        else {
                            $(".registerErr").html("").hide();
                            ObjectJS.SendMobileMessage("btnSendMsg", $("#loginName").val());
                        }
                    });
                }
                else {
                    $(".registerErr").html("请输入正确手机号").slideDown();
                    return;
                }
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

        //注册
        $("#btnRegister").click(function () {
            ObjectJS.validateData();
        });
    }

    //验证数据
    ObjectJS.validateData = function () {
        //手机号
        if ($("#loginName").val() == '') {
            $(".registerErr").html("手机号不能为空").slideDown();
        }
        else {
            if (Global.validateMobilephone($("#loginName").val())){
                Global.post("/Home/IsExistLoginName", { loginName: $("#loginName").val() }, function (data){
                    if (data.Result == 1){
                        $(".registerErr").html("手机号已被注册").slideDown();
                    }
                    else{
                        $("#loginName").removeClass("errIpt");
                        //code
                        if ($("#code").val() == '') {
                            $(".registerErr").html("验证码不能为空").slideDown();
                        }else {
                            Global.post("/Home/ValidateMobilePhoneCode", { mobilePhone: $("#loginName").val(), code: $("#code").val() }, function (data) {
                                if (data.Result == 0) {
                                    $(".registerErr").html("验证码有误").slideDown();
                                }else {
                                    $("#code-error").removeClass("errIpt");

                                    //公司名称
                                    if ($("#companyName").val() == '') {
                                        $(".registerErr").html("公司名称不能为空").slideDown();
                                    }else {
                                        $("#companyName").removeClass("errIpt");

                                        //姓名
                                        if ($("#name").val() == '') {
                                            $(".registerErr").html("姓名不能为空").slideDown();
                                        }else {
                                            $("#name").removeClass("errIpt");

                                            //密码
                                            if ($("#loginPWD").val() == '') {
                                                $(".registerErr").html("密码不能为空").slideDown();
                                            }else {
                                                if ($("#loginPWD").val().length < 6 || $("#loginPWD").val().length > 25) {
                                                    $(".registerErr").html("密码，6-25位").slideDown();
                                                }else {
                                                    if (Global.passwordLevel($("#loginPWD").val()) == 1) {
                                                        $(".registerErr").html("密码必须含字母+数字").slideDown();
                                                    }else {
                                                        $("#loginPWD").removeClass("errIpt");

                                                        //确认密码
                                                        if ($("#loginSurePWD").val() == '') {
                                                            $(".registerErr").html("确认密码不能为空").slideDown();
                                                        }else {
                                                            if ($("#loginSurePWD").val() != $("#loginPWD").val()) {
                                                                $(".registerErr").html("密码不一致").slideDown();
                                                            }else {
                                                                $("#loginSurePWD").removeClass("errIpt");
                                                                ObjectJS.registerClient();
                                                            }
                                                        }
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
            } else {
                $(".registerErr").html("输入正确手机号").slideDown();
            }
        }

    };

    //保存实体
    ObjectJS.registerClient = function () {
        $("#btnRegister").html("注册中...");
        var Paras = {
            loginName: $("#loginName").val(),
            code: $("#code").val(),
            companyName: $("#companyName").val(),
            name: $("#name").val(),
            loginPWD: $("#loginPWD").val()
        };

        Global.post("/Home/RegisterClient", Paras, function (data) {
            $("#btnRegister").html("注册");
            if (data.Result == 1) {
                location.href = ObjectJS.loginUrl;
            }else if (data.Result == 0) {
                alert("注册失败", 2);
            } else if (data.Result == 2) {
                $(".registerErr").html("手机号已被注册").slideDown();
            } else if (data.Result == 3) {
                $(".registerErr").html("验证码有误").slideDown();
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
                    input.val(input.attr('placeholder')).css("color","#999");
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

        Global.post("/Home/SendMobileMessage", { mobilePhone: mobilePhone }, function (data) {
            if (data.Result == 0) {
                alert("验证码发送失败", 2);
            }

        });
    }

    module.exports = ObjectJS;
});