
define(function (require, exports, module) {

    require("jquery");
    var Verify = require("verify"),
        Global = require("global"),
        doT = require("dot");
    var VerifyObject;

    var Industry = {};
   
    Params = {
        id: "",
        pageIndex: 1,
        keyWords:""
    };

    //详情初始化
    Industry.detailInit = function (id) {
        Params.id = id;

        Industry.detailEvent();

        if (id !='')
        {
            $("#pageTitle").html("设置公司行业");
            $("#saveIndustry").val("保存");
            Industry.getDetail();
        }
    }
    //绑定事件
    Industry.detailEvent = function () {
       
        //验证插件
        VerifyObject = Verify.createVerify({
            element: ".verify",
            emptyAttr: "data-empty",
            verifyType: "data-type",
            regText: "data-text"
        });

        //保存
        $("#saveIndustry").click(function () {

            if (!VerifyObject.isPass()) {
                return false;
            };

            var industry = {
                IndustryID: Params.id,
                Name: $("#Name").val(),
                Description: $("#Description").val()
            };

            Global.post("/System/SaveIndustry", { industry: JSON.stringify(industry) }, function (data) {
                if (data.result == "1") {
                    location.href = "/System/Industrys";
                }
            });
        });
    };

    //详情
    Industry.getDetail = function () {
        Global.post("/System/GetIndustryDetail", { id: Params.id }, function (data) {
            if (data.item) {
                var item = data.item;
                $("#Name").val(item.Name);
                $("#Description").val(item.Description);

            }
        });
    };

    //列表初始化
    Industry.init = function () {
        Industry.bindEvent();
        Industry.getList();
    };

    //绑定事件
    Industry.bindEvent = function () {
        //关键字查询
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                Params.pageIndex = 1;
                Params.keyWords = keyWords;
                Industry.getList();
            });
        });
    };

    //绑定数据
    Industry.getList = function () {
        $(".tr-header").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='4'><div class='data-loading'><div></td></tr>");
        Global.post("/System/GetIndustrys", Params, function (data) {
            $(".tr-header").nextAll().remove();
            doT.exec("template/system/Industry-list.html?3", function (templateFun) {
                var innerText = templateFun(data.items);
                innerText = $(innerText);
                $(".tr-header").after(innerText);
            });

            if (data.items.length == 0) {
                $(".tr-header").after("<tr><td colspan='4'><div class='nodata-txt' >暂无数据!<div></td></tr>");
            }

        });
    }

    module.exports = Industry;
});