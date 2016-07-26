define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Tip = require("tip"),
        Easydialog = require("easydialog"),
        ChooseCustomer = require("choosecustomer"),
        ChooseUser = require("chooseuser"),
        moment = require("moment");
    require("daterangepicker");
    require("pager");
    require("colormark");

    var Params = {
        SearchType: 1,
        TypeID: '',
        Status: -1,
        Mark: -1,
        PayStatus: -1,
        OrderStatus: 1,
        InvoiceStatus: -1,
        ReturnStatus: -1,
        SourceType: -1,
        UserID: "",
        AgentID: "",
        TeamID: "",
        Keywords: "",
        BeginTime: "",
        EndTime: "",
        EntrustClientID: "",
        PageIndex: 1,
        PageSize: 10,
        OrderBy: "o.CreateTime desc"
    };

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function (type, status, model) {
        var _self = this;
        _self.ColorList = JSON.parse(model.replace(/&quot;/g, '"'));        
        Params.SearchType = type;
        if (status) {
            Params.OrderStatus = status;
        }

        _self.getList();
        _self.bindStyle();
        _self.bindEvent(type);

        ObjectJS.getAliInfo();
    }

    //绑定样式
    ObjectJS.bindStyle = function () {
        $(".search-status .item[data-type!=1]").hide();
        $(".search-status .item[data-id=-1]").show();
    }

    //绑定事件
    ObjectJS.bindEvent = function (type) {
        var _self = this;
        $(document).click(function (e) {
            //隐藏下拉
            if (!$(e.target).parents().hasClass("dropdown") && !$(e.target).hasClass("dropdown")) {
                $(".dropdown-ul").hide();
            }

            if (!$(e.target).parents().hasClass("order-layer") && !$(e.target).hasClass("order-layer")) {
                $(".order-layer").animate({ right: "-505px" }, 200);
                $(".object-item").removeClass('looking-view');
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
            Params.PageIndex = 1;
            Params.BeginTime = start ? start.format("YYYY-MM-DD") : "";
            Params.EndTime = end ? end.format("YYYY-MM-DD") : "";
            _self.getList();
        });

        //切换订单状态
        $(".search-status .item").click(function () {
            var _this = $(this);

            //快速点击屏蔽
            if (_self.isLoading) {
                return false;
            }

            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                Params.PageIndex = 1;
                Params.Status = _this.data("id");
                _self.getList();
            }
        });        

        //切换订单类型
        $(".search-ordertype .item").click(function () {
            var _this = $(this);

            //快速点击屏蔽
            if (_self.isLoading) {
                return false;
            }

            if (!_this.hasClass("hover")) {

                //隐藏状态
                var dataID = $(".search-orderstatus li.hover").data('id');
                var dataHide = _this.data('hide');
                if (dataID != '8'&&dataID != '-1') {
                    $(".search-status .item[data-type=" + dataID + "]").show();
                    $(".search-status .item[data-type!=" + dataID + "]").hide();
                }
                else {
                    $(".search-status .item").show();
                }
                $(".search-status .item[data-id='-1']").show();
                $(".search-status .item[data-hide=" + dataHide + "]").hide();

                if ($(".search-status .item.hover").data("hide") == _this.data("hide")) {
                    $(".search-status .item").removeClass("hover");
                    $(".search-status .item").first().addClass("hover")
                    Params.Status = -1;
                }

                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                Params.PageIndex = 1;
                Params.TypeID = _this.data("id");
                _self.getList();
            }
        });

        //来源类型
        $(".search-source .item").click(function () {
            var _this = $(this);

            //快速点击屏蔽
            if (_self.isLoading) {
                return false;
            }

            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                Params.PageIndex = 1;
                Params.SourceType = _this.data("id");
                _self.getList();
            }
        });

        /*标签过滤*/
        $("#filterMark").markColor({
            isAll: true,
            top: 30,
            left: 5,
            data: _self.ColorList,
            onChange: function (obj, callback) {
                callback && callback(true);
                Params.PageIndex = 1;
                Params.Mark = obj.data("value");
                ObjectJS.getList();
            }
        });

        //预警
        $(".search-warning .item").click(function () {
            var _this = $(this);

            //快速点击屏蔽
            if (_self.isLoading) {
                return false;
            }

            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                Params.PageIndex = 1;
                Params.InvoiceStatus = _this.data("id");
                _self.getList();
            }
        });

        //切换订单状态
        $(".search-orderstatus li").click(function () {
            var _this = $(this);

            //快速点击屏蔽
            if (_self.isLoading) {
                return false;
            }

            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
               
                var dataID=_this.data('id');
                var dataHide = $(".search-ordertype .item.hover").data('hide');
                if (dataID != '8' && dataID!='-1') {
                    $(".search-status .item[data-type=" + dataID + "]").show();
                    $(".search-status .item[data-type!=" + dataID + "]").hide();
                }
                else {
                    $(".search-status .item").show();
                }
                $(".search-status .item[data-id='-1']").show();
                $(".search-status .item[data-hide=" + dataHide + "]").hide();

                if ($(".search-status .item.hover").is(":hidden")) {
                    $(".search-status .item[data-id='-1']").addClass('hover').siblings().removeClass('hover');
                    Params.Status = -1;
                }

                Params.OrderStatus = _this.data("id");
                Params.PageIndex = 1;
                _self.getList();
            }
        });

        //切换订单来源类型
        $(".search-entrustclientid li").click(function () {
            var _this = $(this);

            //快速点击屏蔽
            if (_self.isLoading) {
                return false;
            }

            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                Params.PageIndex = 1;
                Params.EntrustClientID = _this.data("id");
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
                $(".table-items-detail .checkbox").addClass("hover");
            } else {
                _this.addClass("ico-check").removeClass("ico-checked");
                $(".table-items-detail .checkbox").removeClass("hover");
            }
        });

        //批量转移
        $("#batchChangeOwner").click(function () {
            var checks = $(".table-items-detail .checkbox.hover");
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
                alert("您尚未选择需要更换的订单")
            }
        });

        //排序
        $(".sort-item").click(function () {
            var _this = $(this);

            //快速点击屏蔽
            if (_self.isLoading) {
                return false;
            }

            if (_this.hasClass("hover")) {
                if (_this.find(".asc").hasClass("hover")) {
                    _this.find(".asc").removeClass("hover");
                    _this.find(".desc").addClass("hover");
                    Params.OrderBy = _this.data("column") + " desc ";
                } else {
                    _this.find(".desc").removeClass("hover");
                    _this.find(".asc").addClass("hover");
                    Params.OrderBy = _this.data("column") + " asc ";
                }
            } else {
                _this.addClass("hover").siblings().removeClass("hover");
                _this.siblings().find(".hover").removeClass("hover");
                _this.find(".desc").addClass("hover");
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
                    if (diff_day > 60)
                    {
                        alert("最大下载60天内");
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

        //关闭浮层
        $("#closeLayer").click(function () {
            $(".order-layer").animate({ right: "-505px" }, 200);
            $(".object-item").removeClass('looking-view');
        });
    }

    //获取列表
    ObjectJS.getList = function () {
        var _self = this;
        //加载中
        _self.isLoading = true;

        $("#checkAll").addClass("ico-check").removeClass("ico-checked");
        $(".table-header").nextAll().remove();
        $(".table-header").after("<tr><td colspan='11'><div class='data-loading'><div></td></tr>");

        Global.post("/Orders/GetOrders", { filter: JSON.stringify(Params) }, function (data) {
            _self.bindList(data);
        });
    }

    //加载列表
    ObjectJS.bindList = function (data) {
        
        var _self = this;
        $(".table-header").nextAll().remove();

        if (data.items.length > 0) {           
            doT.exec("template/orders/orders.html", function (template) {
                var innerhtml = template(data.items);
                innerhtml = $(innerhtml);

                innerhtml.find(".checkbox").click(function () {
                    var _this = $(this);
                    if (_this.hasClass("hover")) {
                        _this.removeClass("hover");
                    } else {
                        _this.addClass("hover");
                    }
                    return false;
                });

                innerhtml.find(".mark").markColor({
                    isAll: false,
                    data: _self.ColorList,
                    onChange: function (obj, callback) {
                        _self.markOrders(obj.data("id"), obj.data("value"), callback);
                    }
                });

                innerhtml.find(".view-detail").click(function () {
                    _self.getDetail($(this).data("id"), $(this).data('code'));
                    $('.object-item').removeClass('looking-view');
                    $(this).parents('.object-item').addClass('looking-view');
                    return false;
                });

                innerhtml.find('.order-progress-item').each(function () {
                    var _this = $(this);
                    _this.css({ "width": _this.data('width') });
                });
                innerhtml.find('.progress-tip,.top-lump').each(function () {
                    var _this = $(this);
                    _this.css({ "left": (_this.parent().width() - _this.width()) / 2 });
                });
                innerhtml.find('.layer-line').css({ width: 0, left: "160px" });

                $(".table-header").after(innerhtml);
            });
        }
        else
        {
            $(".table-header").after("<tr><td colspan='11'><div class='nodata-txt' >暂无数据!<div></td></tr>");
        }

        //加载完成
        _self.isLoading = false;

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

    ObjectJS.getDetail = function (id, orderCode) {

        $(".order-layer-item").hide();
        if ($(".order-layer").css("right") == "-505px" || $(".order-layer").css("right") == "-505") {
            $(".order-layer").animate({ right: "0px" }, 200);
        }
        $(".order-layer").append("<div class='data-loading'><div>");
       
        if ($("#" + id).length > 0) {
            $(".order-layer").find(".data-loading").remove();
            $("#" + id).show();
        } else {
            $.get("/Orders/OrderLayer", { id: id }, function (html) {
                $(".order-layer").find(".data-loading").remove();
                $(".order-layer").append(html);
               
            });
        }
        var detail = "<a class='font14 mLeft5' href='/Orders/OrderDetail/" + id + "'>" + orderCode + "</a>";
        $(".order-layer").find('.layer-header').find('a').remove();
        $(".order-layer").find('.layer-header').append(detail);
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