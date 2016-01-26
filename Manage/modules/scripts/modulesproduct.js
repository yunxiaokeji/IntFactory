

define(function (require, exports, module) {

    require("jquery");
    require("pager");
    var Verify = require("verify"),
        Global = require("global"),
        doT = require("dot");
    var VerifyObject;

    var ModulesProduct = {};
   
    ModulesProduct.Params = {
        pageIndex: 1,
        id: '',
        keyWords:''
    };

    //模块产品详情初始化
    ModulesProduct.detailInit = function (id) {
        ModulesProduct.detailEvent();

        if (id != '') {
            $("#pageTitle").html("设置产品");
            $("#saveModulesProduct").val("保存");
            ModulesProduct.Params.id = id;

            ModulesProduct.getModulesProductDetail();
        }
    }
    //绑定事件
    ModulesProduct.detailEvent = function () {
       
        //验证插件
        VerifyObject = Verify.createVerify({
            element: ".verify",
            emptyAttr: "data-empty",
            verifyType: "data-type",
            regText: "data-text"
        });

        //保存模块产品
        $("#saveModulesProduct").click(function () {

            if (!VerifyObject.isPass()) {
                return false;
            };

            var Type = 1;
            if ($("#ModulesID").find("option:selected").text().indexOf("代理商")!=-1)
            {
                Type = 2;
            }

            var modulesProduct = {
                AutoID: ModulesProduct.Params.id,
                ModulesID: $("#ModulesID").val(),
                Period: $("#Period").val(),
                PeriodQuantity: $("#PeriodQuantity").val(),
                UserQuantity: $("#UserQuantity").val(),
                Price: $("#Price").val(),
                Description: $("#Description").val(),
                UserType: $("#UserType").val(),
                Type: Type
            };

            Global.post("/ModulesProduct/SaveModulesProduct", { modulesProduct: JSON.stringify(modulesProduct) }, function (data) {
                if (data.Result == "1") {
                    location.href = "/ModulesProduct/Index";
                }
            });
        });
    };

    //详情
    ModulesProduct.getModulesProductDetail = function (id) {
        Global.post("/ModulesProduct/GetModulesProductDetail", { id: ModulesProduct.Params.id }, function (data) {
            if (data.Result == "1") {
                var item = data.Item;

                $("#ModulesID").val(item.ModulesID);
                $("#Period").val(item.Period);
                $("#PeriodQuantity").val(item.PeriodQuantity);
                $("#UserQuantity").val(item.UserQuantity);
                $("#Price").val(item.Price);
                $("#Description").val(item.Description);

            }
        });
    };

    //列表初始化
    ModulesProduct.init = function () {
        ModulesProduct.bindEvent();
        ModulesProduct.bindData();
    };

    //绑定事件
    ModulesProduct.bindEvent = function () {
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                ModulesProduct.Params.pageIndex = 1;
                ModulesProduct.Params.keyWords = keyWords;
                ModulesProduct.bindData();
            });
        });
    };
    //绑定数据
    ModulesProduct.bindData = function () {
        var _self = this;
        $(".tr-header").nextAll().remove();

        Global.post("/ModulesProduct/GetModulesProducts", ModulesProduct.Params, function (data) {
            doT.exec("template/modulesproduct-list.html?3", function (templateFun) {
                var innerText = templateFun(data.Items);
                innerText = $(innerText);
                $(".tr-header").after(innerText);

                $(".table-list a.ico-del").bind("click", function () {
                    if (confirm("确定删除?"))
                    {
                        Global.post("/ModulesProduct/DeleteModulesProduct", { id: $(this).attr("data-id") }, function (data) {
                            if (data.Result == 1) {
                                location.href = "/ModulesProduct/Index";
                            }
                            else {
                                alert("删除失败");
                            }
                        });
                    }
                });
            });

            $("#pager").paginate({
                total_count: data.TotalCount,
                count: data.PageCount,
                start: ModulesProduct.Params.pageIndex,
                display: 5,
                border: true,
                rotate: true,
                images: false,
                mouse: 'slide',
                onChange: function (page) {
                    ModulesProduct.Params.pageIndex = page;
                    ModulesProduct.bindData();
                }
            });
        });
    }

    module.exports = ModulesProduct;
});