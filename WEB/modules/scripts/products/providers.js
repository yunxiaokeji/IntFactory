
define(function (require, exports, module) {
    var City = require("city"), CityObject,
        Global = require("global"),
        Verify = require("verify"), VerifyObject,
        doT = require("dot"),
        Easydialog = require("easydialog");
    require("pager");
    var Params = {
        keyWords: "",
        pageSize: 20,
        pageIndex: 1,
        totalCount: 0
    };
    var ObjectJS = {};

    //列表页初始化
    ObjectJS.init = function () {
        var _self = this;
        _self.getList();
        _self.bindEvent();
    }
    //绑定列表页事件
    ObjectJS.bindEvent = function () {
        var _self = this;
        $(document).click(function (e) {
            //隐藏下拉
            if (!$(e.target).parents().hasClass("dropdown") && !$(e.target).hasClass("dropdown")) {
                $(".dropdown-ul").hide();
            }
        });

        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                Params.keyWords = keyWords;
                _self.getList();
            });
        });
        //添加
        $("#addObject").click(function () {
            _self.createModel();
        });

        //删除
        $("#deleteObject").click(function () {
            var _this = $(this);
            confirm("供应商删除后不可恢复,确认删除吗？", function () {
                Global.post("/Products/DeleteProvider", { id: _this.data("id") }, function (data) {
                    if (data.status) {
                        _self.getList();
                    } else {
                        alert("删除失败！", 2);
                    }
                });
            }, "删除");
        });
        //编辑
        $("#updateObject").click(function () {
            var _this = $(this);

            Global.post("/Products/GetProviderDetail", { id: _this.data("id") }, function (data) {
                var model = data.model;
                _self.createModel(model);
            });
        });
    }

    //添加/编辑弹出层
    ObjectJS.createModel = function (model) {
        var _self = this;

        doT.exec("template/products/provider_detail.html", function (template) {
            var html = template([]);
            Easydialog.open({
                container: {
                    id: "show-model-detail",
                    header: !model ? "添加供应商" : "编辑供应商",
                    content: html,
                    yesFn: function () {
                        if (!VerifyObject.isPass()) {
                            return false;
                        }
                        var entity = {
                            ProviderID: model ? model.ProviderID : "",
                            Name: $("#providerName").val().trim(),
                            Contact: $("#contact").val().trim(),
                            MobileTele: $("#mobiletele").val().trim(),
                            CityCode: CityObject.getCityCode(),
                            Address: $("#address").val().trim(),
                            Remark: $("#description").val().trim()
                        };
                        Global.post("/Products/SavaProviders", { entity: JSON.stringify(entity) }, function (data) {
                            if (data.status) {
                                alert(!model ? "添加供应商" : "编辑供应商" + "成功！");
                                _self.getList();
                            }
                        });                    },
                    callback: function () {

                    }
                }
            });
            VerifyObject = Verify.createVerify({
                element: ".verify",
                emptyAttr: "data-empty",
                verifyType: "data-type",
                regText: "data-text"
            });


            $("#providerName").focus();

            if (model) {
                $("#providerName").val(model.Name);
                $("#contact").val(model.Contact);
                $("#mobiletele").val(model.MobileTele);
                $("#address").val(model.Address)
                $("#description").val(model.Remark);

                CityObject = City.createCity({
                    elementID: "city",
                    cityCode: model.CityCode
                });
            } else {
                CityObject = City.createCity({
                    elementID: "city"
                });
            }
        });
    }

    //获取列表
    ObjectJS.getList = function () {
        var _self = this;
        $(".tr-header").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='7'><div class='data-loading'><div></td></tr>");

        Global.post("/Products/GetProviders", Params, function (data) {
            $(".tr-header").nextAll().remove();
            if (data.items.length > 0) {
                doT.exec("template/products/providers.html", function (templateFun) {
                    var innerText = templateFun(data.items);
                    innerText = $(innerText);
                    $(".tr-header").after(innerText);

                    //下拉事件
                    innerText.find(".dropdown").click(function () {
                        var _this = $(this);

                        var position = _this.find(".ico-dropdown").position();
                        $(".dropdown-ul li").data("id", _this.data("id"));
                        $(".dropdown-ul").css({ "top": position.top + 20, "left": position.left - 55 }).show().mouseleave(function () {
                            $(this).hide();
                        });
                    });
                });
            }
            else {
                $(".tr-header").after("<tr><td colspan='7'><div class='nodata-txt' >暂无数据!<div></td></tr>");
            }

            $("#pager").paginate({
                total_count: data.TotalCount,
                count: data.PageCount,
                start: Params.pageIndex,
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
                    Params.pageIndex = page;
                    _self.getList();
                }
            });
        });
    }
    module.exports = ObjectJS;
})