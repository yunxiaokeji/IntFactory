

define(function (require, exports, module) {

    require("jquery");
    var Verify = require("verify"),
        Global = require("global"),
        doT = require("dot");
    var VerifyObject;

    var Industry = {};
   
    Industry.Params = {
        pageIndex: 1,
        id: "",
        keyWords:""
    };

    //详情初始化
    Industry.detailInit = function (id) {
        Industry.detailEvent();

        if (id !='')
        {
            $("#pageTitle").html("设置公司行业");
            $("#saveIndustry").val("保存");
            Industry.Params.id = id;

            Industry.getIndustryDetail();
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
                IndustryID: Industry.Params.id,
                Name: $("#Name").val(),
                Description: $("#Description").val()
            };

            Global.post("/Industry/SaveIndustry", { industry: JSON.stringify(industry) }, function (data) {
                if (data.Result == "1") {
                    location.href = "/Industry/Index";
                }
            });
        });
    };

    //详情
    Industry.getIndustryDetail = function () {
        Global.post("/Industry/GetIndustryDetail", { id: Industry.Params.id }, function (data) {
            if (data.Result == "1") {
                var item = data.Item;
                $("#Name").val(item.Name);
                $("#Description").val(item.Description);

            }
        });
    };

    //列表初始化
    Industry.init = function () {
        Industry.bindEvent();
        Industry.bindData();
    };

    //绑定事件
    Industry.bindEvent = function () {
        //关键字查询
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                Industry.Params.pageIndex = 1;
                Industry.Params.keyWords = keyWords;
                Industry.bindData();
            });
        });
    };

    //绑定数据
    Industry.bindData = function () {
        $(".tr-header").nextAll().remove();

        Global.post("/Industry/GetIndustrys", Industry.Params, function (data) {
            doT.exec("template/Industry-list.html?3", function (templateFun) {
                var innerText = templateFun(data.Items);
                innerText = $(innerText);
                $(".tr-header").after(innerText);
            });

        });
    }

    module.exports = Industry;
});