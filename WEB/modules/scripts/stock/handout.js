﻿
define(function (require, exports, module) {
    var Global = require("global"),
        Easydialog = require("easydialog"),
        doT = require("dot");
    require("pager");

    var Params = {
        keyWords: "",
        wareid: "",
        status: -1,
        pageIndex: 1,
        totalCount: 0,
        begintime: "",
        endtime: "",
        type: 7
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

        $(document).click(function (e) {
            //隐藏下拉
            if (!$(e.target).parents().hasClass("dropdown-ul") && !$(e.target).parents().hasClass("dropdown") && !$(e.target).hasClass("dropdown")) {
                $(".dropdown-ul").hide();
            }
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

        //新建出库
        $("#btnCreate").click(function () {
            var _this = $(this);
            doT.exec("template/stock/chooseware.html", function (template) {
                var innerHtml = template(wares);
                Easydialog.open({
                    container: {
                        id: "show-model-chooseware",
                        header: "选择出库仓库",
                        content: innerHtml,
                        yesFn: function () {
                            var wareid = $(".ware-items .hover").data("id");
                            if (!wareid) {
                                alert("请选择出库仓库！", 2);
                                return false;
                            } else {
                                location.href = "/Stock/CreateHandOut/" + wareid;
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
            location.href = "/Stock/HandOutDetail/" + _self.docid;
        });
        //作废
        $("#invalid").click(function () {
            location.href = "/Stock/HandOutDetail/" + _self.docid;
        });
        //删除
        $("#delete").click(function () {
            location.href = "/Stock/HandOutDetail/" + _self.docid;
        });

    }
    //获取单据列表
    ObjectJS.getList = function () {
        var _self = this;
        $(".tr-header").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='7'><div class='data-loading'><div></td></tr>");
        var url = "/Stock/GetStorageDocs",
            template = "template/stock/storagedocs.html";

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

    //审核页初始化
    ObjectJS.initDetail = function (docid) {
        var _self = this;
        _self.docid = docid;
        
        $("#btnInvalid").click(function () {
            confirm("手工出库单作废后不可恢复,确认作废吗？", function () {
                Global.post("/Stock/InvalidHandOutDoc", { docid: _self.docid }, function (data) {
                    if (data.status) {
                        location.href = "/Stock/HandOut";
                    } else {
                        alert("作废失败！", 2);
                    }
                });
            }, "作废");
        });

        $("#btnDelete").click(function () {
            confirm("手工出库单删除后不可恢复,确认删除吗？", function () {
                Global.post("/Stock/DeleteHandOutDoc", { docid: _self.docid }, function (data) {
                    if (data.status) {
                        location.href = "/Stock/HandOut";
                    } else {
                        alert("删除失败！", 2);
                    }
                });
            }, "删除");
        });
        //手工出库走报损逻辑
        $("#btnAudit").click(function () {
            confirm("确认审核手工出库单吗？", function () {
                Global.post("/Stock/AuditHandOutDoc", { docid: _self.docid }, function (data) {
                    if (data.result == 1) {
                        location.href = "/Stock/HandOut";
                    } else {
                        alert(data.errinfo, 2);
                    }
                });
            }, "审核");
        });
    }

    module.exports = ObjectJS;
})