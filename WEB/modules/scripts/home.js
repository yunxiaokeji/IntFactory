

define(function (require, exports, module) {

    require("jquery");
    var Global = require("global"),
        doT = require("dot");

    var Home = {};
    //登陆初始化
    Home.initLogin = function (status) {
        Home.placeholderSupport();
        if (status == 2) {
            alert("您的账号已在其它地点登录，如不是本人操作，请及时通知管理员对账号冻结！");
        } else if (status == 1) {
            alert("尊敬的明道用户，您尚未被管理员添加到智能工厂系统，请及时联系管理员！");
        }
        Home.bindLoginEvent();
    }
    //绑定事件
    Home.bindLoginEvent = function () {

        $(document).on("keypress", function (e) {
            if (e.keyCode == 13) {
                $("#btnLogin").click();
            }
        });

        //登录
        $("#btnLogin").click(function () {
            if (!$("#iptUserName").val()) {
                $(".registerErr").html("请输入账号").slideDown();
                return;
            }
            if (!$("#iptPwd").val()) {
                $(".registerErr").html("请输入密码").slideDown();
                return;
            }

            $(this).html("登录中...").attr("disabled", "disabled");
            Global.post("/Home/UserLogin", {
                userName: $("#iptUserName").val(),
                pwd: $("#iptPwd").val(),
                remember: $(".cb-remember-password").hasClass("ico-checked") ? 1 : 0
            },
            function (data)
            {
                $("#btnLogin").html("登录").removeAttr("disabled");

                if (data.result == 1)
                {
                    location.href = "/Home/Index";
                }
                else if (data.result == 0)
                {
                   $(".registerErr").html("账号或密码有误").slideDown();
                }
                else if (data.result == 2) {
                    $(".registerErr").html("密码输入错误超过3次，请2小时后再试").slideDown();
                }
                else if (data.result == 3) {
                    $(".registerErr").html("账号或密码有误,您还有" + (3 - parseInt( data.errorCount) ) + "错误机会").slideDown();
                }
                else if (data.result == -1)
                {
                    $(".registerErr").html("账号已冻结，请" + data.forbidTime + "分钟后再试").slideDown();
                }
            });
        });

        //if (!$("#iptUserName").val()) {
        //    $("#iptUserName").focus();
        //} else {
        //    $("#iptPwd").focus();
        //}

        //记录密码
        $(".cb-remember-password").click(function () {
            var _this = $(this);
            if (_this.hasClass("ico-check")) {
                _this.removeClass("ico-check").addClass("ico-checked");
            } else {
                _this.removeClass("ico-checked").addClass("ico-check");
            }
        });


        $(".txtBoxPassword").click(function () {
            $(this).hide();
            $("#iptPwd").focus();
        });

        $("#iptPwd").blur(function () {
            if ($(this).val() == '')
                $(".txtBoxPassword").show();
        }).focus(function () {
            if ($(this).val() == '')
                $(".txtBoxPassword").hide();
        });

    }


    //首页JS
    Home.initHome = function () {
        
        Home.bindStyle();
        Home.bindEvent();

        Global.post('/Home/GetAgentActions', {}, function (data) {
            for (var i = 0; i < data.model.Actions.length; i++) {
                var model = data.model.Actions[i];
                if (model.ObjectType == 0) {
                    $("#loginDay").text(model.DayValue.toFixed(0));
                    $("#loginWeek").text(model.WeekValue.toFixed(0));
                    $("#loginMonth").text(model.MonthValue.toFixed(0));
                } else if (model.ObjectType == 1) {
                    $("#customDay").text(model.DayValue.toFixed(0));
                    $("#customWeek").text(model.WeekValue.toFixed(0));
                    $("#customMonth").text(model.MonthValue.toFixed(0));
                } else if (model.ObjectType == 2) {
                    $("#orderDay").text(model.DayValue.toFixed(0));
                    $("#orderWeek").text(model.WeekValue.toFixed(0));
                    $("#orderMonth").text(model.MonthValue.toFixed(0));
                } else if (model.ObjectType == 3) {
                    $("#activeDay").text(model.DayValue.toFixed(0));
                    $("#activeWeek").text(model.WeekValue.toFixed(0));
                    $("#activeMonth").text(model.MonthValue.toFixed(0));
                } else if (model.ObjectType == 7) {
                    $("#opporDay").text(model.DayValue.toFixed(0));
                    $("#opporWeek").text(model.WeekValue.toFixed(0));
                    $("#opporMonth").text(model.MonthValue.toFixed(0));
                }
            }
        });

    }

    //首页样式
    Home.bindStyle = function () {

        $(".report-box").fadeIn();

        var width = document.documentElement.clientWidth - 300, height = document.documentElement.clientHeight - 200;

        var unit = 302;
        
        $(".report-box").css({
            width: unit * 3 + 120
        });

        $(".report-box").css({
            marginTop: (document.documentElement.clientHeight - 500) / 2
        });
       
    }

    Home.bindEvent = function () {
        //调整浏览器窗体
        $(window).resize(function () {
            Home.bindStyle();
        });

        $(document).click(function (e) {
            if (!$(e.target).parents().hasClass("currentuser") && !$(e.target).hasClass("currentuser")) {
                $(".dropdown-userinfo").fadeOut("1000");
            }
        });

        //折叠收藏
        $("#choosemodules").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.addClass("hover");
                $(".bottom-body").css("height", "0");
            } else {
                $(".bottom-body").css("height", "40px");
                _this.removeClass("hover");
            }
        });

        //登录信息展开
        $("#currentUser").click(function () {
            $(".dropdown-userinfo").fadeIn("1000");
        });
    }

    Home.placeholderSupport = function () {
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
    module.exports = Home;
});