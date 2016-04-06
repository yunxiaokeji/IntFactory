﻿define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Easydialog = require("easydialog"),
        ChooseCustomer = require("choosecustomer"),
        ChooseUser = require("chooseuser");
    require("pager");
    require("mark");

    var ColumnCount = 16;

    var Params = {
        SearchType: 1,
        TypeID: '',
        Status: -1,
        Mark: -1,
        PayStatus: -1,
        OrderStatus: 1,
        InvoiceStatus: -1,
        ReturnStatus: 0,
        SourceType: -1,
        UserID: "",
        AgentID: "",
        TeamID: "",
        Keywords: "",
        BeginTime: "",
        EndTime: "",
        PageIndex: 1,
        PageSize: 10,
        OrderBy: "o.CreateTime desc"
    };

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function (type, status) {
        var _self = this;
        Params.SearchType = type;
        if (status) {
            Params.Status = status;
        }
        _self.getList();
        _self.bindEvent(type);

        ObjectJS.getAliInfo();
    }

    //绑定事件
    ObjectJS.bindEvent = function (type) {
        var _self = this;
        $(document).click(function (e) {
            //隐藏下拉
            if (!$(e.target).parents().hasClass("dropdown") && !$(e.target).hasClass("dropdown")) {
                $(".dropdown-ul").hide();
            }
        });

        $("#btnSearch").click(function () {
            Params.PageIndex = 1;
            Params.BeginTime = $("#BeginTime").val().trim();
            Params.EndTime = $("#EndTime").val().trim();
            _self.getList();
        });

        //切换订单状态
        $(".search-status li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                Params.PageIndex = 1;
                Params.Status = _this.data("id");
                _self.getList();
            }
        });

        //切换退货状态
        $(".search-returnstatus li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                Params.PageIndex = 1;
                Params.OrderStatus = _this.data("id");
                _self.getList();
            }
        });
        //关键字搜索
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                Params.PageIndex = 1;
                Params.Keywords = keyWords;
                _self.getList();
            });
        });
        //订单类型
        require.async("dropdown", function () {
            var items = [
                { ID: 1, Name: "打样" },
                { ID: 2, Name: "大货" }
            ];
            $("#orderType").dropdown({
                prevText: "订单类型-",
                defaultText: "全部",
                defaultValue: "",
                data: items,
                dataValue: "ID",
                dataText: "Name",
                width: "120",
                onChange: function (data) {
                    Params.PageIndex = 1;
                    Params.TypeID = data.value;
                    _self.getList();
                }
            });
        });

        //支付状态
        require.async("dropdown", function () {
            var items = [
                { ID: 0, Name: "未付款" },
                { ID: 1, Name: "部分付款" },
                { ID: 2, Name: "已付款" }
            ];
            $("#payStatus").dropdown({
                prevText: "付款-",
                defaultText: "全部",
                defaultValue: "-1",
                data: items,
                dataValue: "ID",
                dataText: "Name",
                width: "90",
                onChange: function (data) {
                    Params.PageIndex = 1;
                    Params.PayStatus = data.value;
                    _self.getList();
                }
            });
        });
        //来源类型
        require.async("dropdown", function () {
            var items = [
                { ID: 1, Name: "工厂" },
                { ID: 2, Name: "自助" },
                { ID: 3, Name: "阿里" }
            ];
            $("#sourceType").dropdown({
                prevText: "来源-",
                defaultText: "全部",
                defaultValue: "-1",
                data: items,
                dataValue: "ID",
                dataText: "Name",
                width: "120",
                onChange: function (data) {
                    Params.PageIndex = 1;
                    Params.SourceType = data.value;
                    _self.getList();
                }
            });
        });

        if (type == 2) {
            require.async("choosebranch", function () {
                $("#chooseBranch").chooseBranch({
                    prevText: "下属-",
                    defaultText: "全部",
                    defaultValue: "",
                    userid: "",
                    isTeam: false,
                    width: "170",
                    onChange: function (data) {
                        Params.PageIndex = 1;
                        Params.UserID = data.userid;
                        Params.TeamID = data.teamid;
                        _self.getList();
                    }
                });
            });
        } else if (type == 3) {
            require.async("choosebranch", function () {
                $("#chooseBranch").chooseBranch({
                    prevText: "人员-",
                    defaultText: "全部",
                    defaultValue: "",
                    userid: "-1",
                    isTeam: true,
                    width: "170",
                    onChange: function (data) {
                        Params.PageIndex = 1;
                        Params.UserID = data.userid;
                        Params.TeamID = data.teamid;
                        _self.getList();
                    }
                });
            });
        }
        //全部选中
        $("#checkAll").click(function () {
            var _this = $(this);
            if (!_this.hasClass("ico-checked")) {
                _this.addClass("ico-checked").removeClass("ico-check");
                $(".table-list .check").addClass("ico-checked").removeClass("ico-check");
            } else {
                _this.addClass("ico-check").removeClass("ico-checked");
                $(".table-list .check").addClass("ico-check").removeClass("ico-checked");
            }
        });
        //转移拥有者
        $("#changeOwner").click(function () {
            var _this = $(this);
            ChooseUser.create({
                title: "更换负责人",
                type: 1,
                single: true,
                callback: function (items) {
                    if (items.length > 0) {
                        if (_this.data("userid") != items[0].id) {
                            _self.ChangeOwner(_this.data("id"), items[0].id);
                        } else {
                            alert("请选择不同人员进行更换!");
                        }
                    }
                }
            });
        });

        $("#auditOrReturn").click(function () {
            var _this = $(this);
            location.href = "/Orders/Detail/" + _this.data("id");
        });

        //过滤标记
        $("#filterMark").markColor({
            isAll: true,
            onChange: function (obj, callback) {
                callback && callback(true);
                Params.PageIndex = 1;
                Params.Mark = obj.data("value");
                _self.getList();
            }
        });

        //批量转移
        $("#batchChangeOwner").click(function () {
            var checks = $(".table-list .ico-checked");
            if (checks.length > 0) {
                ChooseUser.create({
                    title: "批量更换负责人",
                    type: 1,
                    single: true,
                    callback: function (items) {
                        if (items.length > 0) {
                            var ids = "", userid = items[0].id;
                            checks.each(function () {
                                var _this = $(this);
                                if (_this.data("userid") != userid) {
                                    ids += _this.data("id") + ",";
                                }
                            });
                            if (ids.length > 0) {
                                _self.ChangeOwner(ids, userid);
                            } else {
                                alert("请选择不同人员进行更换!");
                            }
                        }
                    }
                });
            } else {
                alert("您尚未选择客户!")
            }
        });

        //排序
        $(".orderby-column").click(function () {
            var _this = $(this);
            if (_this.data("orderby") && _this.data("orderby") == 1) {
                if (_this.data("isasc") == 1) {
                    _this.data("isasc", "0");
                    _this.find(".orderby-asc").removeClass("hover");
                    _this.find(".orderby-desc").addClass("hover");
                    Params.OrderBy = _this.data("column") + " desc ";
                } else {
                    _this.data("isasc", "1");
                    _this.find(".orderby-desc").removeClass("hover");
                    _this.find(".orderby-asc").addClass("hover");
                    Params.OrderBy = _this.data("column") + " asc ";
                }
            } else {
                $(".orderby-column").data("orderby", "0").find(".hover").removeClass("hover");
                _this.data("orderby", "1").data("isasc", "0");
                _this.find(".orderby-asc").removeClass("hover");
                _this.find(".orderby-desc").addClass("hover");
                Params.OrderBy = _this.data("column") + " desc ";
            }
            Params.PageIndex = 1;
            _self.getList();
        });

        //手动同步阿里订单
        $("#downAliOrders").click(function () {

            doT.exec("template/orders/downAliOrders.html", function (template) {

                var html = template([]);
                Easydialog.open({
                    container: {
                        id: "show-model-downAliOrders",
                        header: "同步阿里订单",
                        content: html
                    }
                });

                var nowDate = new Date();
                var maxDate = nowDate.toLocaleDateString();

                $("#downStartTime").val(ObjectJS.DownBeginTime);
                $("#downEndTime").val( nowDate.getFullYear()+"-"+(nowDate.getMonth()+1)+"-"+nowDate.getDate() );

                
                $("#btn-sureDown").click(function () {
                    if ($("#downStartTime").val() == "")
                    {
                        alert("请选择起始时间");
                        return;
                    }
                    else if ($("#downEndTime").val() == "")
                    {
                        alert("请选择截止时间");
                        return;
                    }

                    var dateFrom = new Date($("#downStartTime").val());
                    var dateTo = new Date($("#downEndTime").val());
                    var diff = dateTo.valueOf() - dateFrom.valueOf();
                    var diff_day = parseInt(diff / (1000 * 60 * 60 * 24));
                    if (diff_day > 15)
                    {
                        alert("最大下载15天内");
                        return;
                    }

                    $("#btn-sureDown").val("同步中...").attr("disabled", "disabled");
                    Global.post("/Orders/DownAliOrders", {
                        startTime: $("#downStartTime").val(),
                        endTime: $("#downEndTime").val(),
                        downOrderType: $("#downOrderType").val()
                    }, function (data) {
                        $("#btn-sureDown").val("确定同步").removeAttr("disabled");

                        if (data.result == 0) {
                            alert("同步失败");
                        }
                        else if (data.result == 1) {
                            Easydialog.close();

                            var html = '<div style="width:400px;border:1px solid #ccc;border-radius:4px;"><div id="downOrderBar" style="background-color:#06c;width:0px;height:10px;border-radius:4px;"></div></div>';
                            html += '<div style="text-align:center;margin-top:3px;"><span id="successOrderCount">0</span>(成功) / <span id="totalOrderCount">0</span>(总数)</div>';
                            Easydialog.open({
                                container: {
                                    id: "show-model-showDownAliOrders",
                                    header: "同步进度",
                                    content: html,
                                    yesFn: function () {
                                        location.href = "/Orders/Orders/Need";
                                    }
                                }

                            });

                            var successOrderCount = parseInt(data.successOrderCount);
                            $("#totalOrderCount").html(data.totalOrderCount);
                            var totalOrderCount = parseInt(data.totalOrderCount);

                            $("#successOrderCount").html(successOrderCount);
                            if (totalOrderCount>0)
                                $("#downOrderBar").css("width", (successOrderCount / totalOrderCount) * 400 + "px");
                            else
                                $("#downOrderBar").css("width",400 + "px");
                        }
                        else if (data.result == 2) {
                            alert("无权同步");
                        }
                        else if (data.result == 3) {
                            alert("最大下载15天内");
                        }
                        else if (data.result == 4) {
                            alert("今天已手动同步订单了,请稍后再试");
                        }
                    });

                });

                var start = {
                    elem: '#downStartTime',
                    format: 'YYYY-MM-DD',
                    max: maxDate,
                    min:ObjectJS.AliStartTime,
                    istime: false,
                    istoday: false,
                    choose: function (datas) {
                        end.min = datas; //开始日选好后，重置结束日的最小日期
                        end.start = datas //将结束日的初始值设定为开始日
                    }
                };
                laydate(start);

                var end = {
                    elem: '#downEndTime',
                    format: 'YYYY-MM-DD',
                    max: maxDate,
                    min:ObjectJS.AliStartTime,
                    istime: false,
                    istoday: true,
                    choose: function (datas) {
                        start.max = datas; //结束日选好后，重置开始日的最大日期
                    }
                };
                laydate(end);


            });

        });
    }

    //获取列表
    ObjectJS.getList = function () {
        var _self = this;
        $("#checkAll").addClass("ico-check").removeClass("ico-checked");
        $(".tr-header").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='" + ColumnCount + "'><div class='data-loading'><div></td></tr>");

        Global.post("/Orders/GetOrders", { filter: JSON.stringify(Params) }, function (data)
        {
            _self.bindList(data);
        });
    }

    //加载列表
    ObjectJS.bindList = function (data) {
        var _self = this;
        $(".tr-header").nextAll().remove();

        if (data.items.length > 0) {
            var url = "template/orders/orders.html";
            if (Params.SearchType == 4) {
                url = "template/orders/entrustorders.html";
            }
            doT.exec(url, function (template) {
                var innerhtml = template(data.items);
                innerhtml = $(innerhtml);

                //下拉事件
                innerhtml.find(".dropdown").click(function () {
                    var _this = $(this);
                    var position = _this.find(".ico-dropdown").position();
                    $(".dropdown-ul li").data("id", _this.data("id")).data("userid", _this.data("userid"));
                    $(".dropdown-ul").css({ "top": position.top + 20, "left": position.left - 90 }).show().mouseleave(function () {
                        $(this).hide();
                    });
                    return false;
                });
                innerhtml.find(".check").click(function () {
                    var _this = $(this);
                    if (!_this.hasClass("ico-checked")) {
                        _this.addClass("ico-checked").removeClass("ico-check");
                    } else {
                        _this.addClass("ico-check").removeClass("ico-checked");
                    }
                    return false;
                });
                innerhtml.find(".mark").markColor({
                    isAll: false,
                    onChange: function (obj, callback) {
                        _self.markOrders(obj.data("id"), obj.data("value"), callback);
                    }
                });
                $(".tr-header").after(innerhtml);
            });
        }
        else
        {
            $(".tr-header").after("<tr><td colspan='" + ColumnCount + "'><div class='nodata-txt' >暂无数据!<div></td></tr>");
        }

        $("#pager").paginate({
            total_count: data.totalCount,
            count: data.pageCount,
            start: Params.PageIndex,
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
                Params.PageIndex = page;
                _self.getList();
            }
        });
    }

    //转移客户
    ObjectJS.ChangeOwner = function (ids, userid) {
        var _self = this;
        Global.post("/Orders/UpdateOrderOwner", {
            userid: userid,
            ids: ids
        }, function (data) {
            if (data.status) {
                _self.getList();
            }
        });
    }

    //标记订单
    ObjectJS.markOrders = function (ids, mark, callback) {
        if (mark < 0) {
            alert("不能标记此选项!");
            return false;
        }
        Global.post("/Orders/UpdateOrderMark", {
            ids: ids,
            mark: mark
        }, function (data) {
            if (data.result == "10001") {
                alert("您没有标记订单的权限！");
                callback && callback(false);
            } else {
                callback && callback(data.status);
            }
        });
    }

    ObjectJS.getAliInfo = function () {
        Global.post("/Orders/GetAliInfo", null, function (data) {
            if (data.result == 1) {
                $("#downAliOrders").removeClass("nolimits");
                ObjectJS.AliStartTime = data.plan.CreateTime.toDate("yyyy-MM-dd");
                ObjectJS.DownBeginTime = data.downBeginTime.toDate("yyyy-MM-dd");
            }
        });
    }

    module.exports = ObjectJS;
});