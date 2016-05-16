

define(function (require, exports, module) {
    require("jquery");
    require("pager");
    var Verify = require("verify"),
        Global = require("global"),
        doT = require("dot");
    var VerifyObject;

    var ModulesProduct = {};
   
    var Params = {
        id: '',
        keyWords: '',
        pageIndex: 1,
        pageSize: 5,
        periodType:-1
    };

    require.async("dropdown", function () {
        var PeriodQTY = [
           {
               ID: "1",
               Name: "1"
           },
           {
               ID: "2",
               Name: "2"
           },
           {
               ID: "3",
               Name: "3"
           }
        ];
        $("#PeriodQTY").dropdown({
            prevText: "周期量-",
            defaultText: "所有",
            defaultValue: "-1",
            data: PeriodQTY,
            dataValue: "ID",
            dataText: "Name",
            width: "120",
            onChange: function (data) {
                console.log(Params.pageIndex);
                Params.pageIndex = 1;
                Params.periodType = parseInt(data.value);
                ModulesProduct.getList();
            }
        });
    });


    //模块产品详情初始化
    ModulesProduct.detailInit = function (id) {
        Params.id = id;

        ModulesProduct.detailEvent();

        if (id != '0') {
            $("#pageTitle").html("设置产品收费");
            $("#saveModulesProduct").val("保存");
            $('#delModulesProduct').show();
            ModulesProduct.getDetail();
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
            var modulesProduct = {
                AutoID: Params.id,
                Period: $("#Period").val(),
                PeriodQuantity: $("#PeriodQuantity").val(),
                UserQuantity: $("#UserQuantity").val(),
                Price: $("#Price").val(),
                Description: $("#Description").val(),
                UserType: $("#UserType").val(),
                Type: 1
            };
            Global.post("/System/SaveModulesProduct", { modulesProduct: JSON.stringify(modulesProduct) }, function (data) {
                if (data.result == "1") {
                    location.href = "/System/Index";
                }
            });

        });
        $('#delModulesProduct').click(function () {
            if (confirm("确定删除?")) {
                Global.post("/System/DeleteModulesProduct", { id: Params.id }, function (data) {
                    if (data.result == 1) {
                        location.href = "/System/Index";
                    }
                    else {
                        alert("删除失败");
                    }
                });
            }
        });
    };

    //详情
    ModulesProduct.getDetail = function () {
        Global.post("/System/GetModulesProductDetail", { id: Params.id }, function (data) {
            if (data.item) {
                var item = data.item;
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
        ModulesProduct.getList();
    };

    //绑定事件
    ModulesProduct.bindEvent = function () {
    };

    //绑定数据
    ModulesProduct.getList = function () {
        $(".tr-header").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='6'><div class='data-loading'><div></td></tr>");
        Global.post("/System/GetModulesProducts", Params, function (data) {
            $(".tr-header").nextAll().remove();
            
            doT.exec("template/system/modulesproduct-list.html?3", function (templateFun) {
                var innerText = templateFun(data.items);
                innerText = $(innerText);
                $(".tr-header").after(innerText);
            });

            if (data.items.length == 0) {
                $(".tr-header").after("<tr><td colspan='6'><div class='nodata-txt' >暂无数据!<div></td></tr>");
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
                    ModulesProduct.getList();
                }
            });

        });
    }

    module.exports = ModulesProduct;
});