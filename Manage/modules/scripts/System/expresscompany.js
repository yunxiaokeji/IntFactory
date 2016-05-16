

define(function (require, exports, module) {

    require("jquery");
    require("pager");
    var Verify = require("verify"),
        Global = require("global"),
        doT = require("dot");
    var VerifyObject;

    var ExpressCompany = {};
   
    Params = {
        pageIndex: 1,
        id: '',
        keyWords:''
    };

    //详情初始化
    ExpressCompany.detailInit = function (id) {
        Params.id = id;

        ExpressCompany.detailEvent();

        if (id != '')
        {
            $("#pageTitle").html("设置产品");
            $("#saveExpressCompany").val("保存");
            $('#delExpressCompany').show();
            ExpressCompany.getDetail();
        }
    }
    //绑定事件
    ExpressCompany.detailEvent = function () {

        //验证插件
        VerifyObject = Verify.createVerify({
            element: ".verify",
            emptyAttr: "data-empty",
            verifyType: "data-type",
            regText: "data-text"
        });
        $('#delExpressCompany').click(function () {
            if (confirm("确定删除?")) {
                Global.post("/System/DeleteExpressCompany", { id: Params.id }, function (data) {
                    if (data.result == 1) {
                        location.href = "/System/ExpressCompanys";
                    }
                    else {
                        alert("删除失败");
                    }
                });
            }
        });
        //保存
        $("#saveExpressCompany").click(function () {
            if (!VerifyObject.isPass()) {
                return false;
            };

            var expressCompany = {
                ExpressID: Params.id,
                Name: $("#Name").val(),
                Website: $("#Website").val()
            };

            Global.post("/System/SaveExpressCompany", { expressCompany: JSON.stringify(expressCompany) }, function (data) {
                if (data.result == "1") {
                    location.href = "/System/ExpressCompanys";
                }
            });
        });
    };

    //详情
    ExpressCompany.getDetail = function () {
        Global.post("/System/GetExpressCompanyDetail", { id: Params.id }, function (data) {
            if (data.item) {
                var item = data.item;
                $("#Name").val(item.Name);
                $("#Website").val(item.Website);
            }
        });
    };

    //列表初始化
    ExpressCompany.init = function () {
        ExpressCompany.bindEvent();
        ExpressCompany.getList();
    };

    //绑定事件
    ExpressCompany.bindEvent = function () {
        //关键字查询
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                Params.pageIndex = 1;
                Params.keyWords = keyWords;
                ExpressCompany.getList();
            });
        });
    };

    //绑定数据
    ExpressCompany.getList = function () {
        $(".tr-header").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='4'><div class='data-loading'><div></td></tr>");

        Global.post("/System/GetExpressCompanys", Params, function (data) {
            $(".tr-header").nextAll().remove();
            doT.exec("template/system/expresscompany-list.html?3", function (templateFun) {
                var innerText = templateFun(data.items);
                innerText = $(innerText);
                $(".tr-header").after(innerText);
            });

            if (data.items.length == 0) {
                $(".tr-header").after("<tr><td colspan='4'><div class='nodata-txt' >暂无数据!<div></td></tr>");
            }

            $("#pager").paginate({
                total_count: data.totalCount,
                count: data.pageCount,
                start: Params.pageIndex,
                display: 5,
                border: true,
                rotate: true,
                images: false,
                mouse: 'slide',
                onChange: function (page) {
                    Params.pageIndex = page;
                    ExpressCompany.getList();
                }
            });

        });
    }

    module.exports = ExpressCompany;
});