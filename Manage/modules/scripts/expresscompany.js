

define(function (require, exports, module) {

    require("jquery");
    require("pager");
    var Verify = require("verify"),
        Global = require("global"),
        doT = require("dot");
    var VerifyObject;

    var ExpressCompany = {};
   
    ExpressCompany.Params = {
        pageIndex: 1,
        id: '',
        keyWords:''
    };

    //详情初始化
    ExpressCompany.detailInit = function (id) {
        ExpressCompany.detailEvent();

        if (id != '')
        {
            $("#pageTitle").html("设置产品");
            $("#saveExpressCompany").val("保存");
            ExpressCompany.Params.id = id;

            ExpressCompany.getExpressCompanyDetail();
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

        //保存
        $("#saveExpressCompany").click(function () {

            if (!VerifyObject.isPass()) {
                return false;
            };

            var expressCompany = {
                ExpressID: ExpressCompany.Params.id,
                Name: $("#Name").val(),
                Website: $("#Website").val()
            };

            Global.post("/ExpressCompany/SaveExpressCompany", { expressCompany: JSON.stringify(expressCompany) }, function (data) {
                if (data.Result == "1") {
                    location.href = "/ExpressCompany/Index";
                }
            });
        });
    };

    //详情
    ExpressCompany.getExpressCompanyDetail = function () {
        Global.post("/ExpressCompany/GetExpressCompanyDetail", { id: ExpressCompany.Params.id }, function (data) {
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

    //列表初始化
    ExpressCompany.init = function () {
        ExpressCompany.bindEvent();
        ExpressCompany.bindData();
    };

    //绑定事件
    ExpressCompany.bindEvent = function () {
        //关键字查询
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                ExpressCompany.Params.pageIndex = 1;
                ExpressCompany.Params.keyWords = keyWords;
                ExpressCompany.bindData();
            });
        });
    };

    //绑定数据
    ExpressCompany.bindData = function () {
        $(".tr-header").nextAll().remove();

        Global.post("/ExpressCompany/GetExpressCompanys", ExpressCompany.Params, function (data) {
            doT.exec("template/expresscompany-list.html?3", function (templateFun) {
                var innerText = templateFun(data.Items);
                innerText = $(innerText);
                $(".tr-header").after(innerText);

                $(".table-list a.ico-del").bind("click", function () {
                    if (confirm("确定删除?"))
                    {
                        Global.post("/ExpressCompany/DeleteExpressCompany", { id: $(this).data("id") }, function (data) {
                            if (data.Result == 1) {
                                location.href = "/ExpressCompany/Index";
                            }
                            else
                            {
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