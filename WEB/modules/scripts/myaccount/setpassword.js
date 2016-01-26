define(function (require, exports, module) {
    var Global = require("global"),
        Verify = require("verify"), VerifyObject;

    var ObjectJS = {};

    //初始化
    ObjectJS.init = function () {
        var _self = this;

        VerifyObject = Verify.createVerify({
            element: ".verify",
            emptyAttr: "data-empty",
            verifyType: "data-type",
            regText: "data-text"
        });

        _self.bindEvent();
    }

    //绑定事件
    ObjectJS.bindEvent = function () {

        //tab切换
        $(".search-stages li").click(function () {
            var _this = $(this);
            _this.siblings().removeClass("hover");
            _this.addClass("hover");

            $(".content-body div[name='accountInfo']").hide().eq(parseInt(_this.data("id"))).show();
        });

        $("#LoginConfirmPWD").blur(function () {
            if ($(this).val() == "") {
                $("#LoginConfirmPWDError").html("确认密码不能为空");
            }
            else {
                if ($(this).val() != $("#LoginPWD").val()) {
                    $("#LoginConfirmPWDError").html("确认密码有误");
                }
                else {
                    $("#LoginConfirmPWDError").html("");
                }

            }
        });

        //修改密码
        $("#btnSaveAccountPwd").click(function () {

            if ($("#LoginOldPWD").val() != '') {
                $("#LoginOldPWDError").html("");

                if ($("#LoginPWD").val() == "") {
                    $("#LoginPWDError").html("密码不能为空");
                    return false;
                }
                else {
                    if ($("#LoginPWD").val().length < 5) {
                        $("#LoginPWDError").html("密码过短");
                        return false;
                    }
                    else {
                        if (Global.passwordLevel($("#LoginPWD").val()) == 1) {
                            $("#LoginPWDError").html("密码至少包含字母大小写、数字、字符两种组合");
                            return false;
                        }
                        else {
                            $("#LoginPWDError").html("");
                        }
                    }

                }

                if ($("#LoginConfirmPWD").val() == "") {
                    $("#LoginConfirmPWDError").html("确认密码不能为空");
                    return false;
                }
                else if ($("#LoginConfirmPWD").val() != $("#LoginPWD").val()) {
                    $("#LoginConfirmPWDError").html("确认密码有误");
                    return false;
                }
                else {
                    $("#LoginConfirmPWDError").html("");
                }

                Global.post("/MyAccount/ConfirmLoginPwd", { loginName: "", loginPwd: $("#LoginOldPWD").val() }, function (data) {
                    if (data.Result) {
                        $("#LoginOldPWDError").html("");

                        ObjectJS.UpdateUserAccount();

                    }
                    else {
                        $("#LoginOldPWDError").html("原密码有误");
                    }
                });

            }
            else {
                $("#LoginOldPWDError").html("原密码不能为空");
            }
        });
    }



    //账户管理
    ObjectJS.UpdateUserAccount = function () {
        Global.post("/MyAccount/UpdateUserPass", { loginPwd: $("#LoginPWD").val() }, function (data) {
            if (data.Result) {
                alert("保存成功");
                $("#LoginOldPWD").val("");
                $("#LoginPWD").val("");
                $("#LoginConfirmPWD").val("");
            }
            else {
                alert("保存失败");
            }
        });
    }

    module.exports = ObjectJS;
});