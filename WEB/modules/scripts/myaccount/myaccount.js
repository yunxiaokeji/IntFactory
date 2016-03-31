define(function (require, exports, module) {
    var Global = require("global"),
        Verify = require("verify"), VerifyObject;

    var ObjectJS = {};

    //初始化
    ObjectJS.init = function (departs) {
        var _self = this;

        _self.departs = JSON.parse(departs.replace(/&quot;/g, '"'));
        
        _self.bindEvent();

        _self.getDetail(departs);
    }

    //绑定事件
    ObjectJS.bindEvent = function () {
        VerifyObject = Verify.createVerify({
            element: ".verify",
            emptyAttr: "data-empty",
            verifyType: "data-type",
            regText: "data-text"
        });

        //tab切换
        $(".search-stages li").click(function () {
            var _this = $(this);
            _this.siblings().removeClass("hover");
            _this.addClass("hover");

            $(".content-body div[name='accountInfo']").hide().eq( parseInt(_this.data("id")) ).show();
        });


        //用户基本信息
        $("#btnSaveAccountInfo").click(function () {
            if (!VerifyObject.isPass("#accountInfo") ) {
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
                        data: ObjectJS.departs,
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

        Global.post("/MyAccount/SaveAccountInfo", {
            entity: JSON.stringify(model),
            departmentName: $("#DepartmentName").val()
        }, function (data) {
            if (data.result == 1) {
                alert("保存成功");
            }
        })
    }

    module.exports = ObjectJS;
});