
define(function (require, exports, module) {
    var City = require("city"), BrandCity,
        Upload = require("upload"), BrandIco,
        Global = require("global"),
        Verify = require("verify"), VerifyObject,
        doT = require("dot"),
        Easydialog = require("easydialog");
    require("pager");
    require("switch");
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
        $("#addBrand").click(function () {
            _self.createModel();
        });

        //删除
        $("#deleteObject").click(function () {
            var _this = $(this);
            confirm("品牌删除后不可恢复,确认删除吗？", function () {
                Global.post("/Products/DeleteBrand", { brandID: _this.data("id") }, function (data) {
                    if (data.Status) {
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

            Global.post("/Products/GetBrandDetail", { id: _this.data("id") }, function (data) {
                var model = data.model;
                _self.createModel(model);
            });
        });
    }

    //添加/编辑弹出层
    ObjectJS.createModel = function (model) {
        var _self = this;

        doT.exec("template/products/brand-detail.html", function (template) {
            var html = template([]);
            Easydialog.open({
                container: {
                    id: "show-model-detail",
                    header: !model ? "新建品牌" : "编辑品牌",
                    content: html,
                    yesFn: function () {
                        if (!VerifyObject.isPass()) {
                            return false;
                        }
                        var entity = {
                            BrandID: model ? model.BrandID : "",
                            Name: $("#brandName").val().trim(),
                            AnotherName: $("#anotherName").val().trim(),
                            IcoPath: _self.IcoPath,
                            CountryCode: "0086",
                            CityCode: BrandCity.getCityCode(),
                            Status: $("#brandStatus").prop("checked") ? 1 : 0,
                            Remark: $("#description").val().trim(),
                            BrandStyle: $("#brandStyle").val().trim()
                        };
                        Global.post("/Products/SavaBrand", { brand: JSON.stringify(entity) }, function (data) {
                            if (data.ID.length > 0) {
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


            $("#brandName").focus();

            if (model) {

                $("#brandName").val(model.Name);
                $("#anotherName").val(model.AnotherName);
                $("#brandStyle").val(model.BrandStyle);
                $("#brandStatus").prop("checked", model.Status == 1);
                $("#description").val(model.Remark);
                $("#brandImg").attr("src", model.IcoPath);

                _self.IcoPath = model.IcoPath;

                BrandCity = City.createCity({
                    elementID: "brandCity",
                    cityCode: model.CityCode
                });

                BrandIco = Upload.createUpload({
                    element: "brandIco",
                    buttonText: "更换商标",
                    className: "",
                    data: { folder: '', action: 'edit', oldPath: model.IcoPath },
                    success: function (data, status) {
                        if (data.Items.length > 0) {
                            _self.IcoPath = data.Items[0];
                            $("#brandImg").attr("src", data.Items[0] + "?" + Global.guid());
                        } else {
                            alert("只能上传jpg/png/gif类型的图片，且大小不能超过5M！", 2);
                        }
                    }
                });
            } else {
                _self.IcoPath = "";
                BrandCity = City.createCity({
                    elementID: "brandCity"
                });
                BrandIco = Upload.createUpload({
                    element: "brandIco",
                    buttonText: "选择商标",
                    className: "",
                    data: { folder: '', action: 'add', oldPath: "" },
                    success: function (data, status) {
                        if (data.Items.length > 0) {
                            _self.IcoPath = data.Items[0];
                            $("#brandImg").attr("src", data.Items[0]);
                        } else {
                            alert("只能上传jpg/png/gif类型的图片，且大小不能超过5M！", 2);
                        }
                    }
                });
            }
        });
    }

    //获取品牌列表
    ObjectJS.getList = function () {
        var _self = this;
        $(".tr-header").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='8'><div class='data-loading'><div></td></tr>");

        Global.post("/Products/GetBrandList", Params, function (data) {
            $(".tr-header").nextAll().remove();
            if (data.Items.length > 0) {
                doT.exec("template/products/brands.html", function (templateFun) {
                    var innerText = templateFun(data.Items);
                    innerText = $(innerText);
                    $("#brand-items").after(innerText);

                    //下拉事件
                    innerText.find(".dropdown").click(function () {
                        var _this = $(this);

                        var position = _this.find(".ico-dropdown").position();
                        $(".dropdown-ul li").data("id", _this.data("id"));
                        $(".dropdown-ul").css({ "top": position.top + 20, "left": position.left - 55 }).show().mouseleave(function () {
                            $(this).hide();
                        });
                    });
                    //绑定启用插件
                    innerText.find(".status").switch({
                        open_title: "点击启用",
                        close_title: "点击禁用",
                        value_key: "value",
                        change: function (data, callback) {
                            _self.editStatus(data, data.data("id"), data.data("value"), callback);
                        }
                    });
                });
            }
            else {
                $(".tr-header").after("<tr><td colspan='8'><div class='nodata-txt' >暂无数据!<div></td></tr>");
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
    //更改品牌状态
    ObjectJS.editStatus = function (obj, id, status, callback) {
        var _self = this;
        Global.post("/Products/UpdateBrandStatus", {
            brandID: id,
            status: status ? 0 : 1
        }, function (data) {
            !!callback && callback(data.Status);
        });
    }

    module.exports = ObjectJS;
})