

define(function (require, exports, module) {

    require("jquery");
    var Verify = require("verify"),
        Global = require("global"),
        doT = require("dot");
    var VerifyObject;

    var ObjectJS = {};
   
    var Object = {
        MenuCode:"",
        Name:"",
        Controller: "",
        View: "",
        Sort: 0,
        PCode: "",
        Layer:1
    };

    //详情初始化
    ObjectJS.detailInit = function (id) {
        ObjectJS.detailEvent();

    }
    //绑定事件
    ObjectJS.detailEvent = function () {
       
        //验证插件
        VerifyObject = Verify.createVerify({
            element: ".verify",
            emptyAttr: "data-empty",
            verifyType: "data-type",
            regText: "data-text"
        });

        //保存
        $("#saveMenu").click(function () {

            if (!VerifyObject.isPass()) {
                return false;
            };

            Object = {
                MenuCode:$("#MenuCode").val(),
                Name: $("#Name").val(),
                Controller: $("#Controller").val(),
                View: $("#View").val(),
                Sort: $("#Sort").val(),
                PCode: $("#PCode").val(),
                Layer: $("#Layer").val()
            };

            Global.post("/System/SaveSystemMenu", { menu: JSON.stringify(Object) }, function (data) {
                if (data.Result == "1") {
                    location.href = "/System/Menu";
                }
            });
        });
    };

    //详情
    ObjectJS.getIndustryDetail = function () {
        Global.post("/Industry/GetIndustryDetail", { id: ObjectJS.Params.id }, function (data) {
            if (data.Result == "1") {
                var item = data.Item;
                $("#Name").val(item.Name);
                $("#Description").val(item.Description);

            }
        });
    };

    //列表初始化
    ObjectJS.init = function () {
        ObjectJS.bindEvent();
        ObjectJS.bindData();
    };

    //绑定事件
    ObjectJS.bindEvent = function () {
        //关键字查询
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                ObjectJS.Params.pageIndex = 1;
                ObjectJS.Params.keyWords = keyWords;
                ObjectJS.bindData();
            });
        });
    };

    //绑定数据
    ObjectJS.bindData = function () {
        $(".tr-header").nextAll().remove();

        Global.post("/System/GetSystemMenus", ObjectJS.Params, function (data) {
            doT.exec("template/system/menu-list.html?3", function (templateFun) {
                var innerText = templateFun(data.Items);
                innerText = $(innerText);
                $(".tr-header").after(innerText);

                $("a.btn-delete").click(function () {
                    var id = $(this).data("id");
                    if (confirm("确认删除?"))
                    {
                        Global.post("/System/DeleteSystemMenu", { menuCode: id }, function (data) {
                            if (data.Result == 1) {
                                location.href = "/System/Menu";
                            }
                        });
                    }

                    //confirm("确认删除?", function () {
                    //    Global.post("/System/DeleteSystemMenu", { menuCode: id }, function (data) {
                    //        if (data.Result == 1)
                    //        {
                    //            location.href = "/System/Menu";
                    //        }
                    //    });
                    //});

                });

            });

        });
    }

    module.exports = ObjectJS;
});