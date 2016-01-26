

define(function (require, exports, module) {

    require("jquery");
    require("pager");
    var Verify = require("verify"),
        Global = require("global"),
        doT = require("dot");
    var VerifyObject;
    var ExpressCompany = {};
   
    //模块产品详情初始化
    ExpressCompany.detailInit = function (id) {
        ExpressCompany.detailEvent();

        if (id != 0) {
            $("#pageTitle").html("设置产品");
            $("#saveExpressCompany").val("保存");
            ExpressCompany.getExpressCompanyDetail(id);
        }
    }
    //绑定事件
    ExpressCompany.detailEvent = function () {
        ExpressCompany.Params = {
            pageIndex: 1,
            autoID: $("#id").val()
        };


        //验证插件
        VerifyObject = Verify.createVerify({
            element: ".verify",
            emptyAttr: "data-empty",
            verifyType: "data-type",
            regText: "data-text"
        });

        //保存模块产品
        $("#saveExpressCompany").click(function () {

            if (!VerifyObject.isPass()) {
                return false;
            };

            var expressCompany = {
                ExpressID: $("#ExpressID").val(),
                Name: $("#Name").val(),
                Website: $("#Website").val()
            };

            Global.post("/ExpressCompany/SaveExpressCompany", { expressCompany: JSON.stringify(expressCompany) }, function (data) {
                if (data.Result == "1") {
                    location.href = "/ExpressCompany/Index";
                } else if (data.Result == "2") {
                    //alert("登陆账号已存在!");
                }
            });
        });
    };

    //模块产品详情
    ExpressCompany.getExpressCompanyDetail = function (id) {
        Global.post("/ExpressCompany/GetExpressCompanyDetail", { id: id }, function (data) {
            if (data.Result == "1") {
                var item = data.Item;
                $("#Name").val(item.Name);
                $("#Website").val(item.Website);

            } else if (data.Result == "2") {
                alert("登陆账号已存在!");
                $("#loginName").val("");
            }
        });
    };

    //模块产品列表初始化
    ExpressCompany.init = function () {
        ExpressCompany.Params = {
            pageIndex: 1
        };
        ExpressCompany.bindEvent();
        ExpressCompany.bindData();
    };
    //绑定事件
    ExpressCompany.bindEvent = function () {

    };
    //绑定数据
    ExpressCompany.bindData = function () {
        var _self = this;
        $("#client-header").nextAll().remove();
        Global.post("/ExpressCompany/GetExpressCompanys", ExpressCompany.Params, function (data) {
            doT.exec("template/expresscompany_list.html?3", function (templateFun) {
                var innerText = templateFun(data.Items);
                innerText = $(innerText);
                $("#client-header").after(innerText);

                $(".table-list a.ico-del").bind("click", function () {
                    if (confirm("确定删除?"))
                    {
                        Global.post("/ExpressCompany/DeleteExpressCompany", { id: $(this).attr("data-id") }, function (data) {
                            if (data.Result == 1) {
                                location.href = "/ExpressCompany/Index";
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
                start: ExpressCompany.Params.pageIndex,
                display: 5,
                border: true,
                border_color: '#fff',
                text_color: '#333',
                background_color: '#fff',
                border_hover_color: '#ccc',
                text_hover_color: '#000',
                background_hover_color: '#efefef',
                rotate: true,
                images: false,
                mouse: 'slide',
                onChange: function (page) {
                    ExpressCompany.Params.pageIndex = page;
                    ExpressCompany.bindData();
                }
            });
        });
    }

    module.exports = ExpressCompany;
});