﻿
define(function (require, exports, module) {
    var Global = require("global"),
        Easydialog = require("easydialog"),
        doT = require("dot");
    require("pager");

    //缓存货位
    var CacheDepot = [];

    var Params = {
        keyWords: "",
        wareid: "",
        status: -1,
        pageIndex: 1,
        totalCount: 0,
        begintime: "",
        endtime: "",
        providerid: "",
        type: 1
    };
    var ObjectJS = {};
    //初始化
    ObjectJS.init = function (type, wares, providers) {
        var _self = this;
        wares = JSON.parse(wares.replace(/&quot;/g, '"'));
        providers = JSON.parse(providers.replace(/&quot;/g, '"'));
        Params.type = type;
        _self.bindEvent(wares, providers);
        _self.getList();
    }

    //绑定事件
    ObjectJS.bindEvent = function (wares, providers) {
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

        //切换状态
        $(".search-status li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                Params.pageIndex = 1;
                Params.status = _this.data("id");
                _self.getList();
            }
        });


        $("#btnSearch").click(function () {
            Params.pageIndex = 1;
            Params.begintime = $("#BeginTime").val().trim();
            Params.endtime = $("#EndTime").val().trim();
            _self.getList();
        });

        require.async("dropdown", function () {
            $("#providers").dropdown({
                prevText: "供应商-",
                defaultText: "全部",
                defaultValue: "",
                data: providers,
                dataValue: "ProviderID",
                dataText: "Name",
                width: "180",
                onChange: function (data) {
                    Params.pageIndex = 1;
                    Params.providerid = data.value;
                    _self.getList();
                }
            });
        });

        //新建采购
        $("#btnCreate").click(function () {
            var _this = $(this);
            doT.exec("template/stock/chooseware.html", function (template) {
                var innerHtml = template(wares);
                Easydialog.open({
                    container: {
                        id: "show-model-chooseware",
                        header: "选择采购仓库",
                        content: innerHtml,
                        yesFn: function () {
                            var wareid = $(".ware-items .hover").data("id");
                            if (!wareid) {
                                alert("请选择采购仓库！");
                                return false;
                            } else {
                                location.href = "/Products/ConfirmPurchase/" + wareid;
                            }
                        },
                        callback: function () {

                        }
                    }
                });

                $(".ware-items .ware-item").click(function () {
                    $(this).siblings().removeClass("hover");
                    $(this).addClass("hover");
                });
            });
        });

        //审核
        $("#audit").click(function () {
            location.href = "/Products/AuditDetail/" + _self.docid;
        });
        //作废
        $("#invalid").click(function () {
            confirm("采购单作废后不可恢复,确认作废吗？", function () {
                Global.post("/Purchase/InvalidPurchase", { docid: _self.docid }, function (data) {
                    if (data.Status) {
                        Params.pageIndex = 1;
                        _self.getList();
                    } else {
                        alert("作废失败！");
                    }
                });
            });
        });

        $("#delete").click(function () {
            confirm("采购单删除后不可恢复,确认删除吗？", function () {
                Global.post("/Purchase/DeletePurchase", { docid: _self.docid }, function (data) {
                    if (data.Status) {
                        Params.pageIndex = 1;
                        _self.getList();
                    } else {
                        alert("删除失败！");
                    }
                });
            });
        });

    }

    //获取单据列表
    ObjectJS.getList = function () {
        var _self = this;
        $(".tr-header").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='7'><div class='dataLoading'><img src='/modules/images/ico-loading.jpg'/><div></td></tr>");
        var url = "/Purchase/GetPurchases",
            template = "template/purchase/purchases.html";

        Global.post(url, Params, function (data) {
            $(".tr-header").nextAll().remove();

            if (data.items.length > 0) {
                doT.exec(template, function (templateFun) {
                    var innerText = templateFun(data.items);
                    innerText = $(innerText);
                    $(".tr-header").after(innerText);

                    //下拉事件
                    $(".dropdown").click(function () {
                        var _this = $(this);
                        if (_this.data("status") == 0) {
                            $("#invalid").show();
                            $("#delete").show();
                        } else {
                            $("#invalid").hide();
                            $("#delete").hide();
                        }
                        var position = _this.find(".ico-dropdown").position();
                        $(".dropdown-ul").css({ "top": position.top + 15, "left": position.left - 40 }).show().mouseleave(function () {
                            $(this).hide();
                        });
                        _self.docid = _this.data("id");
                    });
                });
            }
            else {
                $(".tr-header").after("<tr><td colspan='7'><div class='noDataTxt' >暂无数据!<div></td></tr>");
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

    //审核页初始化
    ObjectJS.initDetail = function (docid, wareid) {
        var _self = this;
        _self.docid = docid;

        //Global.post("/System/GetDepotSeatsByWareID", { wareid: wareid }, function (data) {
        //    CacheDepot[wareid] = data.Items;
        //    $(".item").each(function () {
        //        var _this = $(this), depotbox = _this.find(".depot-li");
        //        _self.bindDepot(depotbox, data.Items, wareid, _this.data("id"));
        //    })
        //});        

        //审核入库
        $("#btnconfirm").click(function () {
            confirm("确认审核采购单入库吗？", function () {
                Global.post("/Purchase/AuditPurchase", { docid: docid }, function (data) {
                    if (data.Status) {
                        location.href = "/Products/Purchases";
                    };
                });
            });
        })
    }

    //绑定货位
    ObjectJS.bindDepot = function (depotbox, depots, wareid, autoid) {
        var _self = this;
        depotbox.empty();
        var depot = $("<select data-id='" + autoid + "' data-wareid='" + wareid + "'></select>");
        for (var i = 0, j = depots.length; i < j; i++) {
            depot.append($("<option value='" + depots[i].DepotID + "' >" + depots[i].DepotCode + "</option>"))
        }

        depot.val(depotbox.data("id"));

        //选择仓库
        depot.change(function () {
            Global.post("/Purchase/UpdateStorageDetailWare", {
                docid: _self.docid,
                autoid: autoid,
                wareid: wareid,
                depotid: depot.val()
            }, function (data) {
                if (!data.Status) {
                    alert("操作失败,请刷新页面重新操作！");
                };
            });
        });

        depot.prop("disabled", depotbox.data("status") == 1);

        depotbox.append(depot);
    }

    module.exports = ObjectJS;
})