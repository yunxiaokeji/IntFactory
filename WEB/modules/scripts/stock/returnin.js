
define(function (require, exports, module) {
    var Global = require("global"),
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
        type: 6
    };
    var ObjectJS = {};
    //初始化
    ObjectJS.init = function (wares) {
        var _self = this;
        wares = JSON.parse(wares.replace(/&quot;/g, '"'));
        _self.bindEvent(wares);
        _self.getList();
    }
    //绑定事件
    ObjectJS.bindEvent = function (wares) {
        var _self = this;
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                Params.keyWords = keyWords;
                _self.getList();
            });
        });
        $("#btnSearch").click(function () {
            Params.pageIndex = 1;
            Params.begintime = $("#BeginTime").val().trim();
            Params.endtime = $("#EndTime").val().trim();
            _self.getList();
        });
        require.async("dropdown", function () {
            $("#wares").dropdown({
                prevText: "仓库-",
                defaultText: "全部",
                defaultValue: "",
                data: wares,
                dataValue: "WareID",
                dataText: "Name",
                width: "180",
                onChange: function (data) {
                    Params.pageIndex = 1;
                    Params.wareid = data.value;
                    _self.getList();
                }
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

        $(document).click(function (e) {
            //隐藏下拉
            if (!$(e.target).parents().hasClass("dropdown-ul") && !$(e.target).parents().hasClass("dropdown") && !$(e.target).hasClass("dropdown")) {
                $(".dropdown-ul").hide();
            }
        });
        //审核
        $("#audit").click(function () {
            location.href = "/Stock/ReturnInDetail/" + _self.docid;
        });
    }
    //获取单据列表
    ObjectJS.getList = function () {
        var _self = this;
        $(".tr-header").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='8'><div class='data-loading'><div></td></tr>");

        var url = "/Stock/GetStorageDocs",
            template = "template/stock/storagedocs-original.html";

        Global.post(url, Params, function (data) {
            $(".tr-header").nextAll().remove();
            if (data.items.length > 0)
            {
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
            else
            {
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

    //审核页初始化
    ObjectJS.initDetail = function (docid, wareid) {
        var _self = this;
        _self.docid = docid;
        Global.post("/System/GetDepotSeatsByWareID", { wareid: wareid }, function (data) {
            CacheDepot[wareid] = data.Items;
            $(".detail-item").each(function () {
                var _this = $(this), depotbox = _this.find(".depot-li");
                _self.bindDepot(depotbox, data.Items, wareid, _this.data("id"));
            })
        });        

        $(".batch").change(function () {
            var _this = $(this);
            Global.post("/Stock/UpdateStorageDetailBatch", {
                docid: _self.docid,
                autoid: _this.data("id"),
                batch: _this.val().trim()
            }, function (data) {
                if (!data.status) {
                    alert("操作失败,请刷新页面重新操作！");
                };
            });
        });

        //审核入库
        $("#btnconfirm").click(function () {
            confirm("确认审核退货单入库吗？", function () {
                Global.post("/Stock/AuditReturnIn", { docid: docid }, function (data) {
                    if (data.result == 1) {
                        location.href = "/Stock/ReturnIn";
                    } else {
                        alert(data.errinfo);
                    }
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

        if (depotbox.data("id")) {
            depot.val(depotbox.data("id"));
        } else {
            Global.post("/Purchase/UpdateStorageDetailWare", {
                docid: _self.docid,
                autoid: autoid,
                wareid: wareid,
                depotid: depots[0].DepotID
            }, function (data) {

            });
        }

        //选择货位
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
        depotbox.append(depot);
    }


    module.exports = ObjectJS;
})