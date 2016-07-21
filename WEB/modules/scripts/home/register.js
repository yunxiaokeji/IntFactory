define(function (require, exports, module) {
    var Global = require("global"),
        Easydialog = require("easydialog");

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
                $("#btnRegister").click();
            }
        });

        //公司名称
        $("#companyName").blur(function ()
        {
            if ($("#companyName").val() == '') {
                $("#companyName").next().fadeIn().find(".error-msg").html("公司名称不能为空");
            }
            else
            {
                $("#companyName").next().hide().find(".error-msg").html("");
            }
        });

        //用户名称
        $("#name").blur(function ()
        {
            if ($("#name").val() == '') {
                $("#name").next().fadeIn().find(".error-msg").html("姓名不能为空");
            }
            else 
            {
                $("#name").next().hide().find(".error-msg").html("");
            }
        });

        //密码
        $("#loginPWD").blur(function ()
        {
            if ($("#loginPWD").val() == '')
            {
                $("#txtBoxPassword").show();
                $("#loginPWD").next().fadeIn().find(".error-msg").html("密码不能为空");
            }
            else {
                if ($("#loginPWD").val().length < 6 || $("#loginPWD").val().length >25) {
                    $("#loginPWD").next().fadeIn().find(".error-msg").html("密码，6-25位");
                }
                else {
                    if (Global.passwordLevel($("#loginPWD").val()) == 1) {
                        $("#loginPWD").next().fadeIn().find(".error-msg").html("密码至少包含字母/数字/符号两种不同组合");
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
                        if (data.Result == 1) {
                            $("#code-error").fadeIn().find(".error-msg").html("手机号已被注册");
                            return;
                        }
                        else {
                            $("#code-error").hide().find(".error-msg").html("");

                            ObjectJS.SendMobileMessage("btnSendMsg", $("#loginName").val());
                        }
                    });
                }
                else {
                    $("#code-error").fadeIn().find(".error-msg").html("请输入正确手机号");
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
        if ($("#loginName").val() == ''){
            $("#loginName").next().fadeIn().find(".error-msg").html("手机号不能为空");
        }
        else {
            if (Global.validateMobilephone($("#loginName").val())){
                Global.post("/Home/IsExistLoginName", { loginName: $("#loginName").val() }, function (data){
                    if (data.Result == 1){
                        $("#loginName").next().fadeIn().find(".error-msg").html("手机号已被注册");
                    }
                    else{
                        $("#loginName").next().hide().find(".error-msg").html("");

                        //code
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

                                    //公司名称
                                    if ($("#companyName").val() == '') {
                                        $("#companyName").next().fadeIn().find(".error-msg").html("公司名称不能为空");
                                    }
                                    else {
                                        $("#companyName").next().hide().find(".error-msg").html("");

                                        //姓名
                                        if ($("#name").val() == '') {
                                            $("#name").next().fadeIn().find(".error-msg").html("姓名不能为空");
                                        }
                                        else {
                                            $("#name").next().hide().find(".error-msg").html("");

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
            }
            else{
                $("#loginName").next().fadeIn().find(".error-msg").html("输入正确手机号");
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
                var html = "<div>您已成功注册,是否立即完善公司信息？</div><div class='pAll20'>";
                html += "<div class='btn left' onclick='location.href=\"/Home/Index\"'>进入首页</div>";
                html += "<div class='btn right' onclick='location.href=\"/System/Client/2\"'>立即完善</div>";
                html += "<div class='clear'></div></div>";
                Easydialog.open({
                    container: {
                        id: "",
                        header: "注册提示",
                        content: html
                    }
                });

            }
            else if (data.Result == 0) {
                alert("注册失败");
            }
            else if (data.Result == 2) {
                $("#loginName").next().fadeIn().find(".error-msg").html("手机号已被注册");
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
                alert("验证码发送失败");
            }

        });
    }

    module.exports = ObjectJS;
});