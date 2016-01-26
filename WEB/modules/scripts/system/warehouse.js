
define(function (require, exports, module) {
    var City = require("city"), CityObj,
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
            if (!$(e.target).parents().hasClass("dropdown-ul") && !$(e.target).parents().hasClass("dropdown") && !$(e.target).hasClass("dropdown")) {
                $(".dropdown-ul").hide();
            }
        });
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                Params.keyWords = keyWords;
                _self.getList();
            });
        });
        //添加仓库
        $("#addWarehouse").click(function () {
            _self.createModel();
        });

        //删除
        $("#deleteObject").click(function () {
            var _this = $(this);
            confirm("仓库删除后不可恢复,确认删除吗？", function () {
                Global.post("/System/DeleteWareHouse", { id: _this.data("id") }, function (data) {
                    if (data.Status) {
                        _self.getList();
                    } else {
                        alert("删除失败！");
                    }
                });
            });
        });
        //编辑
        $("#updateObject").click(function () {
            var _this = $(this);

            Global.post("/System/GetWareHouseByID", { id: _this.data("id") }, function (data) {
                var model = data.model;
                _self.createModel(model);
            });
        });
    }

    //添加/编辑弹出层
    ObjectJS.createModel = function (model) {
        var _self = this;

        doT.exec("template/system/warehouse-detail.html", function (template) {
            var html = template([]);
            Easydialog.open({
                container: {
                    id: "show-model-detail",
                    header: !model ? "新建仓库" : "编辑仓库",
                    content: html,
                    yesFn: function () {
                        if (!VerifyObject.isPass()) {
                            return false;
                        }
                        var entity = {
                            WareID: model ? model.WareID : "",
                            Name: $("#warehouseName").val().trim(),
                            WareCode: $("#warehouseCode").val().trim(),
                            ShortName: $("#shortName").val().trim(),
                            CityCode: CityObj.getCityCode(),
                            Status: $("#warehouseStatus").prop("checked") ? 1 : 0,
                            DepotCode: model ? "" : $("#depotCode").val().trim(),
                            DepotName: model ? "" : $("#depotName").val().trim(),
                            Description: $("#description").val().trim()
                        };
                        _self.saveModel(entity);
                    },
                    callback: function () {

                    }
                }
            });

            if (model) {
                $(".depot").remove();
            }

            VerifyObject = Verify.createVerify({
                element: ".verify",
                emptyAttr: "data-empty",
                verifyType: "data-type",
                regText: "data-text"
            });


            $("#warehouseName").focus();

            if (model) {
                $("#warehouseCode").val(model.WareCode);

                $("#warehouseName").val(model.Name);

                $("#shortName").val(model.ShortName);
                if (model.Status == 1) {
                    $("#warehouseStatus").prop("checked", "checked");
                }
                $("#description").val(model.Description);

                CityObj = City.createCity({
                    elementID: "warehouseCity",
                    cityCode: model.CityCode
                });
            } else {
                $("#warehouseStatus").prop("checked", "checked");
                CityObj = City.createCity({
                    elementID: "warehouseCity"
                });
            }
        });
    }

    //保存实体
    ObjectJS.saveModel = function (model) {
        var _self = this;
        Global.post("/System/SaveWareHouse", { ware: JSON.stringify(model) }, function (data) {
            if (data.ID) {
                location.href = "/System/WareHouse"
            }
        });
    }
    //获取仓库列表
    ObjectJS.getList = function () {
        var _self = this;
        $("#warehouse-items").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='8'><div class='dataLoading'><img src='/modules/images/ico-loading.jpg'/><div></td></tr>");
        Global.post("/System/GetWareHouses", Params, function (data) {
            $(".tr-header").nextAll().remove();

            if (data.Items.length > 0) {
                doT.exec("template/system/warehouses.html", function (templateFun) {
                    var innerText = templateFun(data.Items);
                    innerText = $(innerText);
                    $("#warehouse-items").after(innerText);

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
                $(".tr-header").after("<tr><td colspan='8'><div class='noDataTxt' >暂无数据!<div></td></tr>");
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
                    Brand.getList();
                }
            });
        });
    }
    //更改仓库状态
    ObjectJS.editStatus = function (obj, id, status, callback) {
        var _self = this;
        Global.post("/System/UpdateWareHouseStatus", {
            id: id,
            status: status ? 0 : 1
        }, function (data) {
            if (data.result == "10001") {
                alert("您没有此操作权限，请联系管理员帮您添加权限！");
                return;
            }
            !!callback && callback(data.Status);
        });
    }

    module.exports = ObjectJS;
})