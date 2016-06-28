
define(function (require, exports, module) {

    require("jquery");
    require("pager");
    var Global = require("global"),
        doT = require("dot");

    var Admin = {};
   
    //模块产品详情初始化
    Admin.detailInit = function ()
    {
        Admin.detailEvent();

        Admin.getAdminDetail();
    }
    //绑定事件
    Admin.detailEvent = function () {
        $("#OldPwd").blur(function () {

            if ($("#OldPwd").val() == '')
            {
                $("#OldPwdError").html('原密码不能为空');
            }
            else
            {
                Global.post("/System/ConfirmAdminPwd", { pwd: _self.val() }, function (data)
                {
                    if (data.Result == 1) {
                        $("#OldPwdError").html('');
                    }
                    else
                    {
                        $("#OldPwdError").html('原密码有误');
                    }


                });
            }

        });

        $("#NewPwd").blur(function (){
            if ($("#NewPwd").val() == '') {
                $("#NewPwdError").html('新密码不能为空');
                return false;
            }
            else {
                $("#NewPwdError").html('');
            }
        });

        $("#NewConfirmPwd").blur(function () {
            if ($("#NewConfirmPwd").val() == '') {
                $("#NewConfirmPwdError").html('确认密码不能为空');
                return false;
            }
            else {
                if ($("#NewConfirmPwd").val() != $("#NewPwd").val()) {
                    $("#NewConfirmPwdError").html('确认密码有误');
                    return false;
                }
                else {
                    $("#NewConfirmPwdError").html('');
                }
            }
        });

        //保存
        $("#saveAdmin").click(function () {
            Admin.validateData(function () {
                Admin.saveAdmin();
            });
        });
    };

    Admin.validateData = function (callback)
    {
        if ($("#OldPwd").val() == '') {
            $("#OldPwdError").html('原密码不能为空');
            return false;
        }
        
        if ($("#NewPwd").val() == '') {
            $("#NewPwdError").html('新密码不能为空');
            return false;
        }
        else {
            $("#NewPwdError").html('');
        }

        if ($("#NewConfirmPwd").val() == '') {
            $("#NewConfirmPwdError").html('确认密码不能为空');
            return false;
        }
        else {
            if ($("#NewConfirmPwd").val() != $("#NewPwd").val()) {
                $("#NewConfirmPwdError").html('确认密码有误');
                return false;
            }
            else {
                $("#NewConfirmPwdError").html('');
            }
        }

        Global.post("/System/ConfirmAdminPwd", { pwd: $("#OldPwd").val() }, function (data) {
            if (data.Result == 1) {
                callback();
                $("#OldPwdError").html('');
            }
            else {
                $("#OldPwdError").html('原密码有误');
                return false;
            }
        });
    };

    Admin.saveAdmin = function () {
        Global.post("/System/SetAdminAccount", { loginName: $("#LoginName").val(), newPwd: $("#NewPwd").val() }, function (data)
        {
            if (data.Result == "1") {
                alert("保存成功");
            }
        });
    };

    //模块产品详情
    Admin.getAdminDetail = function () {
        Global.post("/System/GetAdminDetail", null, function (data) {
            var item = data;
            $("#LoginName").val(item.LoginName);
        });
    };

    module.exports = Admin;
});