define(function (require, exports, module) {
    var Global = require("global"),
        Verify = require("verify"), VerifyObject,
        doT = require("dot"),
        Easydialog = require("easydialog");

    var ObjectJS = {};

    //初始化
    ObjectJS.init = function (departs,option) {
        var _self = this;

        _self.bindEvent();
    }

    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;

        $("#bindLogioName").click(function () {
            _self.setLoginname(function () {

            });
        });

        $("#bindLoginMobile").click(function () {
            if (!$("#S_LoginName").html()) {
                alert("请先设置账号", 2);
                return;
            }
            var S_BindMobile = $("#S_BindMobile").html();
            if (S_BindMobile == "") {
                $("#saveLoginMobile").val("绑定");
            } else {
                $("#saveLoginMobile").val("解绑");
            }
            $(this).hide();
            $(".bindloginmobile").show();

            if ($("#S_BindMobile").html()) {
                $("#mobilePhone").hide();
            }
        });

        //取消绑定
        $("#cancleLoginMobile").click(function () {
            $(".bindloginmobile").hide();
            $("#bindLoginMobile").show();

            $("#mobilePhone,#BindMobileCode").val("");
            $("#BindMobileCodeError,#BindMobileError").html("");
            $("#SendBindMobileCode").removeAttr("disabled");
        })

        //绑定手机
        $("#saveLoginMobile").click(function () {   
                _self.saveAccountBindMobile();
                //location.href = location.href + "?" + (new Date().getMilliseconds());          
        });

        //获取手机验证码
        $("#SendBindMobileCode").click(function () {
            var BindMobile = $("#mobilePhone").val();
            var S_BindMobile = $("#S_BindMobile").html();

            if (S_BindMobile == '') {
                if (BindMobile != '') {
                    if (Global.validateMobilephone(BindMobile)) {
                        Global.post("/MyAccount/IsExistLoginName", { loginName: BindMobile }, function (data) {
                            if (data.result) {
                                $("#BindMobileError").html("手机已存在");
                            }
                            else {
                                $("#BindMobileCodeError").html("");

                                ObjectJS.sendMobileMessage("SendBindMobileCode", BindMobile);
                            }
                        });
                    }
                    else {
                        $("#BindMobileError").html("手机格式有误");
                    }

                }
                else {
                    $("#BindMobileError").html("手机不能为空");
                }
            }
            else {
                ObjectJS.sendMobileMessage("SendBindMobileCode", S_BindMobile);
            }

        });

        //取消微信绑定
        $("#unBindWeiXin").click(function () {
            if (!$("#S_LoginName").html() && !$("#S_BindMobile").html()) {
                alert("您账号和手机都未设置,不能取消微信绑定", 2);
                return;
            }
            confirm("确定要取消绑定微信号?", function () {
                Global.post("/MyAccount/UnBindWeiXin", null, function (data) {
                    if (data.result) {
                        location.href = location.href + "?" + (new Date().getMilliseconds());
                    }
                    else {
                        alert("取消绑定失败", 2);
                    }
                });
            },"取消");
        });

        //微信绑定
        $("#bindWeiXin").click(function () {
            window.open("WXLogin", "", "height=500,width=500,left=" +(  ($(document).width()-500)/2 )+ ",toolbar=no,menubar=no,scrollbars=no, resizable=no,location=yes, status=no");
        });
    }

    //弹出层
    ObjectJS.setLoginname = function (callback) {
        var _self = this;
        doT.exec("template/myaccount/setloginname.html", function (templateFun) {

            var html = html = templateFun([]);

            Easydialog.open({
                container: {
                    id: "setloginname-add-div",
                    header: "设置账号",
                    content: html,
                    yesFn: function () {
                        if (!VerifyObject.isPass("#setloginname-add-div")) {
                            return false;
                        }
                        var _this = $("#LoginName");
                        if (  !(_this.val() && _this.val().length > 5 ) ) {
                            alert("账号长度不能低于6位！", 2);
                            return false;
                        } 

                        if (!$("#S_BindMobile").html().trim()) {
                            if ($("#LoginPWD").val() == "") {
                                alert("密码不能为空！", 2);
                                return false;
                            } else if ($("#LoginPWD").val().length < 6) {
                                alert("密码长度不能低于6位！", 2);
                                return false;
                            } else if (Global.passwordLevel($("#LoginPWD").val()) == 1) {
                                alert("密码至少包含字母大小写、数字、字符两种组合！", 2);
                                return false;
                            }

                            if ($("#LoginConfirmPWD").val() != $("#LoginPWD").val()) {
                                alert("确认密码输入不一致！", 2);
                                return false;
                            }
                        }

                        Global.post("/MyAccount/IsExistLoginName", { loginName: $("#LoginName").val() }, function (data) {
                            if (data.result) {
                                alert("账号已存在，请重新输入！", 2);
                                $("#LoginName").val("");
                            } else {
                                Global.post("/MyAccount/UpdateUserAccount", {
                                    loginName: $("#LoginName").val(),
                                    loginPwd: $("#LoginPWD").val()
                                }, function (data) {
                                    if (data.result) {
                                        alert("账号设置成功！");
                                        $("#S_LoginName").html($("#LoginName").val());
                                        $("#bindLogioName").hide();
                                    }
                                    else {
                                        alert("账号设置失败！", 2);
                                    }
                                });
                            }
                        });
                    },
                    callback: function () {

                    }
                }
            });

            $("#LoginName").focus();

            if ($("#S_BindMobile").html()) {
                $(".nologinname").hide();
            }

            VerifyObject = Verify.createVerify({
                element: ".verify",
                emptyAttr: "data-empty",
                verifyType: "data-type",
                regText: "data-text"
            });
        });
    }

    //绑定手机
    ObjectJS.saveAccountBindMobile = function () {

        if (!$("#S_LoginName").html()) {
            alert("请先设置账号！", 2);
            return;
        }

        var option = $("#S_BindMobile").html().trim() ? 2 : 1;
        
        var BindMobile = $("#mobilePhone").val();
        var BindMobileCode = $("#BindMobileCode").val();

        if (option == 1) {
            if (BindMobile != '') {
                if (Global.validateMobilephone(BindMobile)) {
                    Global.post("/MyAccount/IsExistLoginName", { loginName: BindMobile }, function (data) {

                        if (data.result) {
                            $("#BindMobileError").html("手机已存在");
                        }
                        else {
                            $("#BindMobileError").html("");

                            if (BindMobileCode == "") {
                                $("#BindMobileCodeError").html("验证码不能为空");
                            }
                            else {
                                Global.post("/Home/ValidateMobilePhoneCode", { mobilePhone: BindMobile, code: BindMobileCode }, function (data) {
                                    if (data.Result == 0) {
                                        $("#BindMobileCodeError").html("验证码有误");
                                    }
                                    else {
                                        $("#BindMobileCodeError").html("");

                                        var Paras =
                                        {
                                            bindMobile: BindMobile,
                                            code: $("#BindMobileCode").val(),
                                            option: option//1:绑定手机；2：解除绑定
                                        };

                                        Global.post("/MyAccount/SaveAccountBindMobile", Paras, function (data) {
                                            if (data.result == 1) {
                                                //$("#S_BindMobile").html(BindMobile);
                                                //$("#cancleLoginMobile").click();
                                                //$("#bindLoginMobile").val("解绑");
                                                location.href = location.href + "?" + (new Date().getMilliseconds());
                                            }
                                            else if (data.result == 2) {
                                                $("#BindMobileCodeError").html("验证码有误");
                                            }
                                            else if (data.result == 0) {
                                                alert("保存失败", 2);
                                            }
                                            else if (data.result == 3) {
                                                alert("请先设置账号", 2);
                                            }
                                        });
                                    }
                                });
                            }
                        }
                    });
                }
                else {
                    $("#BindMobileError").html("手机格式有误");
                }
            } else {
                $("#BindMobileError").html("手机不能为空");
            }
        }
        else {

            if (BindMobileCode == "") {
                $("#BindMobileCodeError").html("验证码不能为空");
            }
            else {
                Global.post("/Home/ValidateMobilePhoneCode", { mobilePhone: $("#S_BindMobile").html().trim(), code: BindMobileCode }, function (data) {
                    if (data.Result == 0) {
                        $("#BindMobileCodeError").html("验证码有误");
                    }
                    else {
                        $("#BindMobileCodeError").html("");

                        var Paras =
                        {
                            bindMobile: $("#S_BindMobile").html().trim(),
                            code: $("#BindMobileCode").val(),
                            option: option//1:绑定手机；2：解除绑定
                        };
                        Global.post("/MyAccount/SaveAccountBindMobile", Paras, function (data) {
                            if (data.result == 1) {
                                //$("#S_BindMobile").html("");
                                //$("#cancleLoginMobile").click();
                                //$("#bindLoginMobile").val("绑定");
                                location.href = location.href + "?" + (new Date().getMilliseconds());
                            }
                            else if (data.result == 2) {
                                $("#BindMobileCodeError").html("验证码有误");
                            }
                            else if (data.Result == 0) {
                                alert("保存失败", 2);
                            }
                            else if (data.result == 3) {
                                alert("请先设置账号", 2);
                            }
                        });
                    }
                });
            }
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
                $("#BindMobileError").html("");
                $("#BindMobileCode").focus();
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
                alert("验证码发送失败", 2);
                $btnSendCode.removeAttr("disabled");
            }

        });
    }

    module.exports = ObjectJS;
});