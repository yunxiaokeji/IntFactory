define(function (require, exports, module) {
    var Global = require("global"),
        Verify = require("verify"), VerifyObject;

    var ObjectJS = {};

    //初始化
    ObjectJS.init = function (departs,option) {
        var _self = this;
        departs = JSON.parse(departs.replace(/&quot;/g, '"'));
        
        VerifyObject = Verify.createVerify({
            element: ".verify",
            emptyAttr: "data-empty",
            verifyType: "data-type",
            regText: "data-text"
        });

        _self.bindEvent();

        _self.getDetail(departs);

        if (option != "-1")
        {
            $(".search-stages li").removeClass("hover").eq(parseInt(option)).addClass("hover");
            $(".content-body div[name='accountInfo']").hide().eq(parseInt(option)).show();
        }
    }

    //绑定事件
    ObjectJS.bindEvent = function () {

        //tab切换
        $(".search-stages li").click(function () {
            var _this = $(this);
            _this.siblings().removeClass("hover");
            _this.addClass("hover");

            $(".content-body div[name='accountInfo']").hide().eq( parseInt(_this.data("id")) ).show();
        });


        //用户基本信息
        $("#btnSaveAccountInfo").click(function () {
            if (!VerifyObject.isPass("#accountInfo")) {
                return false;
            };

            ObjectJS.saveAccountInfo();
        });
  
    }
    //获取用户详情
    ObjectJS.getDetail = function (departs) {

        Global.post("/MyAccount/GetAccountDetail", null, function (data) {
            if (data) {
                var item = data;
                //基本信息
                $("#Name").val(item.Name);
                $("#Jobs").val(item.Jobs);
                $("#Birthday").val(item.Birthday.toDate("yyyy-MM-dd"));
                $("#Age").val(item.Age);
                //部门
                $("#DepartmentName").val(item.DepartmentName);
                $("#DepartID").val(item.DepartID);
                require.async("dropdown", function (item) {
                    $("#ddlDepart").dropdown({
                        prevText: "部门-",
                        defaultText: $("#DepartmentName").val(),
                        defaultValue: $("#DepartID").val(),
                        data: departs,
                        dataValue: "DepartID",
                        dataText: "Name",
                        width: "157",
                        onChange: function (data) {
                            $("#DepartID").val(data.value);
                        }
                    });
                });

                //联系信息
                $("#MobilePhone").val(item.MobilePhone);
                $("#OfficePhone").val(item.OfficePhone);
                $("#Email").val(item.Email);

                //绑定的手机号
                if (item.BindMobilePhone != '') {

                    $("#BindMobile").val(item.BindMobilePhone).attr("disabled", "disabled").hide();
                    $("#S_BindMobile").html(item.BindMobilePhone);

                    if (item.LoginName != '') {
                        $("#btnSaveAccountBindMobile").html("解绑").click(function () {
                            ObjectJS.SaveAccountBindMobile(2);
                        });
                    }
                    else {
                        $("#li-code").hide();
                        $("#div-mobile").hide();
                    }
                }
                else {
                    $("#btnSaveAccountBindMobile").html("绑定").click(function () {
                        ObjectJS.SaveAccountBindMobile(1);
                    });
                }

                //账户管理
                if (item.LoginName) {
                    //设置密码
                    $("#LoginName").val(item.LoginName).attr("disabled", "disabled").hide();
                    $("#S_LoginName").html(item.LoginName);

                    $("#LoginOldPWD").blur(function () {
                        if ($(this).val() != '') {
                            Global.post("/MyAccount/ConfirmLoginPwd", { loginName: $("#LoginName").val(), loginPwd: $(this).val() }, function (data) {

                                if (data.Result) {
                                    $("#LoginOldPWDError").html("");
                                }
                                else {
                                    $("#LoginOldPWDError").html("原密码有误");
                                }
                            });

                        } else {
                            $("#LoginOldPWDError").html("原密码不能为空");
                        }

                    });
                }
                else
                {
                    //新增账户
                    $("#li_loginOldPWD").hide();

                    $("#LoginName").blur(function () {
                        if ($(this).val() != '') {
                            if ($(this).val().length > 4) {
                                $("#LoginNameError").html("");
                                Global.post("/MyAccount/IsExistLoginName", { loginName: $(this).val() }, function (data) {

                                    if (data.Result) {
                                        $("#LoginNameError").html("账户已存在");
                                    }
                                    else {
                                        $("#LoginNameError").html("");
                                    }
                                });
                            }
                            else {
                                $("#LoginNameError").html("账户名称过短");
                            }

                        }
                        else {
                            $("#LoginNameError").html("账户不能为空");
                        }
                    });
                }

            }
        })
    }

    //保存基本信息
    ObjectJS.saveAccountInfo = function () {
        var _self = this;
        var model = {
            Name: $("#Name").val(),
            Jobs: $("#Jobs").val(),
            Birthday: $("#Birthday").val(),
            Age: 0,
            DepartID: $("#DepartID").val(),
            MobilePhone: $("#MobilePhone").val(),
            OfficePhone: $("#OfficePhone").val(),
            Email: $("#Email").val()
        };

        Global.post("/MyAccount/SaveAccountInfo", { entity: JSON.stringify(model), departmentName: $("#DepartmentName").val() }, function (data) {
            if (data.Result == 1) {
                alert("保存成功");
            }
        })
    }

    module.exports = ObjectJS;
});