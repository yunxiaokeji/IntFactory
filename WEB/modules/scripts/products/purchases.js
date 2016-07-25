
define(function (require, exports, module) {
    var Global = require("global"),
        Easydialog = require("easydialog"),
        doT = require("dot");
    require("pager");
    var moment = require("moment");
    require("daterangepicker");
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
    ObjectJS.init = function (type, wares) {
        var _self = this;
        wares = JSON.parse(wares.replace(/&quot;/g, '"'));
        Params.type = type;
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

        //日期插件
        $("#iptCreateTime").daterangepicker({
            showDropdowns: true,
            empty: true,
            opens: "right",
            ranges: {
                '今天': [moment(), moment()],
                '昨天': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                '上周': [moment().subtract(6, 'days'), moment()],
                '本月': [moment().startOf('month'), moment().endOf('month')]
            }
        }, function (start, end, label) {
            Params.pageIndex = 1;
            Params.begintime = start ? start.format("YYYY-MM-DD") : "";
            Params.endtime = end ? end.format("YYYY-MM-DD") : "";
            _self.getList();
        });

        //新建采购
        $("#btnCreate").click(function () {
            var _this = $(this);
            location.href = "/Products/ConfirmPurchase";
            //doT.exec("template/stock/chooseware.html", function (template) {
            //    var innerHtml = template(wares);
            //    Easydialog.open({
            //        container: {
            //            id: "show-model-chooseware",
            //            header: "选择采购仓库",
            //            content: innerHtml,
            //            yesFn: function () {
            //                var wareid = $(".ware-items .hover").data("id");
            //                if (!wareid) {
            //                    alert("请选择采购仓库！");
            //                    return false;
            //                } else {
            //                    location.href = "/Products/ConfirmPurchase/" + wareid;
            //                }
            //            },
            //            callback: function () {

            //            }
            //        }
            //    });

            //    $(".ware-items .ware-item").click(function () {
            //        $(this).siblings().removeClass("hover");
            //        $(this).addClass("hover");
            //    });
            ////});
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
        $(".table-header").nextAll().remove();
        $(".table-header").after("<tr><td colspan='7'><div class='data-loading'><div></td></tr>");
        var url = "/Purchase/GetPurchases",
            template = "template/purchase/purchases.html";

        Global.post(url, Params, function (data) {
            $(".table-header").nextAll().remove();

            if (data.items.length > 0) {
                doT.exec(template, function (templateFun) {
                    var innerText = templateFun(data.items);
                    innerText = $(innerText);
                    $(".table-header").after(innerText);

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
                $(".table-header").after("<tr><td colspan='10'><div class='nodata-txt' >暂无数据!<div></td></tr>");
            }

            /*有数据隐藏表头*/
            if (!$(".table-items-detail").find('div').hasClass('nodata-txt')) {
                $(".table-header").hide();
            } else {
                $(".table-header").show();
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
    ObjectJS.initDetail = function (docid, model) {
        var _self = this;
        _self.docid = docid;
        _self.model = JSON.parse(model.replace(/&quot;/g, '"'));

        Global.post("/System/GetDepotSeatsByWareID", { wareid: _self.model.WareID }, function (data) {
            CacheDepot = data.Items;
        });        
        //审核入库
        $("#btnconfirm").click(function () {
            _self.auditStorageIn();
        })

        $(".tab-nav-ul li").click(function () {
            var _this = $(this);
            _this.siblings().removeClass("hover");
            _this.addClass("hover");
            $(".nav-partdiv").hide();
            $("#" + _this.data("id")).show();

            if (_this.data("id") == "navLog" && (!_this.data("first") || _this.data("first") == 0)) {
                _this.data("first", "1");
                _self.getList();
            } else if (_this.data("id") == "navStorageIn" && (!_this.data("first") || _this.data("first") == 0)) {
                _this.data("first", "1");
                _self.getDocList();
            }
        });

        //删除单据
        $("#delete").click(function () {
            confirm("采购单删除后不可恢复,确认删除吗？", function () {
                Global.post("/Purchase/DeletePurchase", { docid: _self.docid }, function (data) {
                    if (data.Status) {
                        alert("采购单删除成功");
                        location.href = location.href;
                    } else {
                        alert("采购单删除失败");
                        location.href = location.href
                    }
                });
            });
        });

        //完成采购单
        $("#btnOver").click(function () {
            if (_self.model.Status == 0) {
                confirm("您尚未登记入库产品，完成采购单后不能再登记入库，确认操作吗？", function () {
                    Global.post("/Purchase/AuditPurchase", {
                        docid: _self.docid,
                        doctype: 101,
                        isover: 1,
                        details: "",
                        remark: ""
                    }, function (data) {
                        if (data.status) {
                            alert("操作成功!");
                            location.href = location.href;
                        } else if (data.result == "10001") {
                            alert("您没有操作权限!")
                        } else {
                            alert("操作失败！");
                        }
                    });
                });
            } else {
                var completeCount = 0;
                var docTotalCount = 0;
                var showMsg = "";
                for (var i = 0; i < _self.model.Details.length; i++) {
                    var item = _self.model.Details[i];
                    if ((item.Quantity * 1 - item.Complete * 1) > 0) {
                        showMsg += "有材料入库量小于单据采购数量，";
                        break;
                    }
                }
                showMsg += "完成采购单后不能再登记入库，确认操作吗？";
                confirm(showMsg, function () {
                    Global.post("/Purchase/AuditPurchase", {
                        docid: _self.docid,
                        doctype: 101,
                        isover: 1,
                        details: "",
                        remark: ""
                    }, function (data) {
                        if (data.status) {
                            alert("操作成功!");
                            location.href = location.href;
                        } else if (data.result == "10001") {
                            alert("您没有操作权限!")
                        } else {
                            alert("操作失败！");
                        }
                    });
                });
            }
        });
    }

    //获取入库明细
    ObjectJS.getDocList = function () {
        var _self = this;
        $(".log-body").empty();
        var url = "/Purchase/GetPurchasesDetails",
            template = "template/purchase/audit_details.html";

        $(".table-header").after("<tr><td colspan='10'><div class='data-loading' ><div></td></tr>");
        Global.post(url, {
            docid: _self.docid
        }, function (data) {
            $(".table-header").nextAll().remove();
            doT.exec(template, function (templateFun) {
                if (data.items.length > 0) {
                    var innerText = templateFun(data.items);
                    innerText = $(innerText);
                } else {
                    $(".table-header").after("<tr><td colspan='4'><div class='nodata-txt' >暂无数据!<div></td></tr>");
                }
                /*有数据隐藏表头*/
                if (!$(".table-items-detail").find('div').hasClass('nodata-txt')) {
                    $(".table-header").hide();
                } else {
                    $(".table-header").show();
                }
                $("#navStorageIn").append(innerText);
            });
        });
    }

    //审核入库
    ObjectJS.auditStorageIn = function () {
        var _self = this;
       
        doT.exec("template/purchase/audit_storagein.html", function (template) {
            var innerText = template(_self.model.Details);

            Easydialog.open({
                container: {
                    id: "showAuditStorageIn",
                    header: "采购单入库",
                    content: innerText,
                    yesFn: function () {
                        var details = ""
                        $("#showAuditStorageIn .list-item").each(function () {
                            var _this = $(this);
                            var quantity = _this.find(".quantity").val();
                            if (quantity > 0) {
                                details += _this.data("id") + "-" + quantity + ":" + _this.find("select").val() + ",";
                            }
                        });

                        if (details.length > 0 || $("#showAuditStorageIn .check").hasClass("ico-checked")) {

                            Global.post("/Purchase/AuditPurchase", {
                                docid: _self.docid,
                                doctype: 101,
                                isover: $("#showAuditStorageIn .check").hasClass("ico-checked") ? 1 : 0,
                                details: details,
                                remark: $("#expressRemark").val().trim()
                            }, function (data) {
                                if (data.status) {
                                    alert("入库成功!", location.href);
                                } else if (data.result == "10001") {
                                    alert("您没有操作权限!")
                                } else {
                                    alert("审核入库失败！");
                                }
                            });
                        } else {
                            alert("请输入采购入库数量！");
                            return false;
                        }
                    },
                    callback: function () {

                    }
                }
            });
            $("#showAuditStorageIn .check").click(function () {
                var _this = $(this);
                if (!_this.hasClass("ico-checked")) {
                    _this.addClass("ico-checked").removeClass("ico-check");
                } else {
                    _this.addClass("ico-check").removeClass("ico-checked");
                }
            });
            $("#showAuditStorageIn").find(".quantity").change(function () {
                var _this = $(this);
                if (!_this.val().isDouble() || _this.val() < 0) {
                    _this.val("0");
                }
                if (_this.val() == 0) {
                    return false;
                }
                if ((_this.parents('.list-item').find('.total').html()) * 1 - ((_this.parents('.list-item').find('.complete-count').html()) * 1 + _this.val() * 1) < 0) {
                    alert("该材料入库数已超出单据数量");
                }
            });

            $("#showAuditStorageIn").find("select").each(function () {
                var _this = $(this);
                _self.bindDepot(_this);
            });
        });
    };

    //绑定货位
    ObjectJS.bindDepot = function (depotbox) {
        var _self = this;

        for (var i = 0, j = CacheDepot.length; i < j; i++) {
            depotbox.append($("<option value='" + CacheDepot[i].DepotID + "' >" + CacheDepot[i].DepotCode + "</option>"))
        }

        depotbox.val(depotbox.data("id"));
    }

    module.exports = ObjectJS;
})