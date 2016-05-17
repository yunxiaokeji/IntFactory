

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
        keyWords: ''
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

    //绑定数据
    Clients.bindData = function () {
        var _self = this;
        $("#client-header").nextAll().remove();

        Global.post("/Client/GetClients", Clients.Params, function (data) {
            doT.exec("template/client/client-list.html?3", function (templateFun) {
                var innerText = templateFun(data.Items);
                innerText = $(innerText);
                $("#client-header").after(innerText);
            });

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