

define(function (require, exports, module) {
    require("jquery");
    require("pager");
    var Verify = require("verify"),
        Global = require("global"),
        doT = require("dot"),
        Easydialog = require("easydialog"),
        City = require("city");
    var VerifyObject, CityObject;    

    var Clients = {};

    Clients.Params = {
        pageIndex: 1,
        pageSize: 20,
        status: -1,
        type: -1,
        beginDate: '',
        endDate: '',
        clientID: '',
        agentID:'',
        keyWords: '',
        orderBy: 'CreateTime '
    };
    //新建客户初始化
    Clients.createInit = function (id) {
        Clients.createEvent();
        //行业为空
        if ($("#industry option").length == 1) $("#industry").change();
        
    }
    //绑定事件
    Clients.createEvent = function () {
        //验证插件
        VerifyObject = Verify.createVerify({
            element: ".verify",
            emptyAttr: "data-empty",
            verifyType: "data-type",
            regText: "data-text"
        });

        $("#authorizeType").change(function () {
            if ($("#authorizeType").val() == "1") {
                $(".contentnew li[name='authorizeType']").fadeIn();
            }
            else {
                $(".contentnew li[name='authorizeType']").fadeOut();
            }
        });       
        //判断账号是否存在
        $("#loginName").blur(function () {
            var value = $(this).val();
            if (!value) {
                return;
            }
            Global.post("/Client/IsExistLoginName", { loginName: value }, function (data) {
                if (data.Result) {
                    $("#loginName").val("");
                    alert("登录账号已存在!");
                }
            });
        });
        CityObject = City.createCity({
            elementID: "citySpan"
        });
        //保存客户端
        $("#saveClient").click(function () {
            if (!VerifyObject.isPass()) {
                return false;
            };

            if ($("#loginName").val().length < 6)
            {
                alert("登录账号不能低于六位");
                return;
            }
            var modules = [];
            $(".modules-item").each(function () {
                var _this = $(this);
                if (_this.hasClass("active")) {
                    modules.push({
                        ModulesID: _this.data("value")
                    });
                }
            });

            var client = {
                CompanyName: $("#name").val(),
                ContactName: $("#contact").val(),
                MobilePhone: $("#mobile").val(),
                Industry: $("#industry").val(),
                CityCode: CityObject.getCityCode(),
                Address: $("#address").val(),
                Description: $("#description").val(),
                Modules: modules
            };
            Global.post("/Client/SaveClient", { client: JSON.stringify(client), loginName: $("#loginName").val() }, function (data) {
                if (data.Result == "1") {
                    location.href = "/Client/Index";
                } else if (data.Result == "2") {
                    alert("登陆账号已存在!");
                    $("#loginName").val("");
                }
            });
        });
    };

    //客户列表初始化
    Clients.init = function () {
        Clients.bindEvent();
        Clients.bindData();
    };

    //绑定事件
    Clients.bindEvent = function () {
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                Clients.Params.pageIndex = 1;
                Clients.Params.keyWords = keyWords;
                Clients.bindData();
            });
        });
    };

    require.async("dropdown", function () {
        var ClientType = [
            { ID: "1", Name: "后台" },
            { ID: "2", Name: "自助" },
            { ID: "3", Name: "阿里" },
            { ID: "4", Name: "微信" }
        ];
        $("#ClientType").dropdown({
            prevText: "注册方式-",
            defaultText: "所有",
            defaultValue: "-1",
            data: ClientType,
            dataValue: "ID",
            dataText: "Name",
            width: "120",
            onChange: function (data) {
                $("#client-header").nextAll().remove();
                Clients.Params.pageIndex = 1;
                Clients.Params.type = parseInt(data.value);
                Clients.bindData();
            }
        });
    });
    $(".td-span").click(function () {
        var _this = $(this);
        if (_this.hasClass("hover")) {
            if (_this.find(".asc").hasClass("hover")) {
                $(".td-span").find(".asc").removeClass("hover");
                $(".td-span").find(".desc").removeClass("hover");
                _this.find(".desc").addClass("hover");
                Clients.Params.orderBy = _this.data("column") + " desc ";
            } else {
                $(".td-span").find(".desc").removeClass("hover");
                $(".td-span").find(".asc").removeClass("hover");
                _this.find(".asc").addClass("hover");
                Clients.Params.orderBy = _this.data("column") + " asc ";
            }
        } else {
            $(".td-span").removeClass("hover");
            $(".td-span").find(".desc").removeClass("hover");
            $(".td-span").find(".asc").removeClass("hover");
            _this.addClass("hover");
            _this.find(".desc").addClass("hover");
            Clients.Params.orderBy = _this.data("column") + " desc ";
        }
        Clients.Params.PageIndex = 1;
        Clients.bindData();
    });
    //绑定数据
    Clients.bindData = function () {
        var _self = this;
        $("#client-header").nextAll().remove();
        $("#client-header").after("<tr><td colspan='10'><div class='data-loading'><div></td></tr>");
        Global.post("/Client/GetClients", Clients.Params, function (data) {
            $("#client-header").nextAll().remove();
            if (data.Items.length > 0) {
                doT.exec("template/client/client-list.html", function (templateFun) {
                    var innerText = templateFun(data.Items);
                    innerText = $(innerText);
                    $("#client-header").after(innerText);
                });
            } else {
                $("#client-header").after("<tr><td colspan='10'><div class='nodata-txt'>暂无数据<div></td></tr>");
            }

            $("#pager").paginate({
                total_count: data.TotalCount,
                count: data.PageCount,
                start: Clients.Params.pageIndex,
                display: 5,
                border: true,
                rotate: true,
                images: false,
                mouse: 'slide',
                onChange: function (page) {
                    Clients.Params.pageIndex = page;
                    Clients.bindData();
                }
            });
        });
    }

    module.exports = Clients;
});