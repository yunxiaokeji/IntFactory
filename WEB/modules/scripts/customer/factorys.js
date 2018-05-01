

define(function (require, exports, module) {
    require("jquery");
    require("pager");
    var Verify = require("verify"),
        Global = require("global"),
        doT = require("dot"),
        Easydialog = require("easydialog"),
        City = require("city");
    var VerifyObject, CityObject;    

    var Clients = {};

    Clients.Params = {
        pageIndex: 1,
        pageSize: 20,
        keyWords: '',
        orderBy: 'CreateTime '
    };

    var Params = {
        keyWords: "",
        clientId: "",
        filterType: 1,
        ordertype: 1,
        orderstatus: -1,
        pagesize: 10,
        pageindex: 1
    };
    //客户列表初始化
    Clients.init = function (isProvider) {
        Clients.bindEvent();
        Clients.bindData(isProvider);
    };

    //绑定事件
    Clients.bindEvent = function () {
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                Clients.Params.pageIndex = 1;
                Clients.Params.keyWords = keyWords;
                Clients.bindData();
            });
        });
    };

    $(".td-span").click(function () {
        var _this = $(this);
        if (_this.hasClass("hover")) {
            if (_this.find(".asc").hasClass("hover")) {
                $(".td-span").find(".asc").removeClass("hover");
                $(".td-span").find(".desc").removeClass("hover");
                _this.find(".desc").addClass("hover");
                Clients.Params.orderBy = _this.data("column") + " desc ";
            } else {
                $(".td-span").find(".desc").removeClass("hover");
                $(".td-span").find(".asc").removeClass("hover");
                _this.find(".asc").addClass("hover");
                Clients.Params.orderBy = _this.data("column") + " asc ";
            }
        } else {
            $(".td-span").removeClass("hover");
            $(".td-span").find(".desc").removeClass("hover");
            $(".td-span").find(".asc").removeClass("hover");
            _this.addClass("hover");
            _this.find(".desc").addClass("hover");
            Clients.Params.orderBy = _this.data("column") + " desc ";
        }
        Clients.Params.PageIndex = 1;
        Clients.bindData();
    });
    //绑定数据
    Clients.bindData = function (isProvider) {
        var _self = this;
        $("#client-header").nextAll().remove();
        $("#client-header").after("<tr><td colspan='12'><div class='data-loading'><div></td></tr>");
        var url = "GetFactoryCustomerByKeywords";
        if (isProvider) {
            url = "GetProviderClientByKeywords";
        }
        Global.post("/Customer/" + url, Clients.Params, function (data) {
            $("#client-header").nextAll().remove();
            if (data.items.length > 0) {
                doT.exec("template/customer/client-list.html", function (templateFun) {
                    var innerText = templateFun(data.items);
                    innerText = $(innerText);
                    if (isProvider) {
                        innerText.find(".detail").each(function () {
                            $(this).attr("href", "/Orders/ProviderDetail/" + $(this).data("id"));
                        });
                    } else {
                        innerText.find(".detail").each(function () {
                            $(this).attr("href", "/Customer/FactoryDetail/" + $(this).data("id"));
                        });
                    }
                    $("#client-header").after(innerText);
                });
            } else {
                $("#client-header").after("<tr><td colspan='10'><div class='nodata-txt'>暂无数据<div></td></tr>");
            }

            $("#pager").paginate({
                total_count: data.totalCount,
                count: data.pageCount,
                start: Clients.Params.pageIndex,
                display: 5,
                border: true,
                rotate: true,
                images: false,
                mouse: 'slide',
                onChange: function (page) {
                    Clients.Params.pageIndex = page;
                    Clients.bindData(isProvider);
                }
            });
        });
    }

    Clients.initDetail = function (isProvider) {
        Clients.bindDetailEvent(isProvider);
    }

    //绑定事件
    Clients.bindDetailEvent = function (isProvider) {
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

        Params.isProvider = isProvider;
        Params.clientId = $("#iptClientID").val();
        //关键字搜索
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                Params.keyWords = keyWords;
                Params.pageindex = 1;
                _self.getList();

            });
        });

        //切换模块
        $(".module-tab li").click(function () {

            var _this = $(this);
            _this.siblings().removeClass("hover");
            _this.addClass("hover");
            $(".nav-partdiv").hide();
            $("#" + _this.data("id")).show();
            if (_this.data("id") == "navOppor") { //需求
                Params.filterType = 1;
                if ((!_this.data("first") || _this.data("first") == 0)) {
                    _this.data("first", "1");
                    Params.keyWords = "";
                    _self.getList();
                }
            } else if (_this.data("id") == "navOrder") { //订单
                Params.filterType = 2;
                $(".search-orderstatus").show();
                if ((!_this.data("first") || _this.data("first") == 0)) {
                    _this.data("first", "1");

                    Params.keyWords = "";
                    _self.getList();
                }
            } else if (_this.data("id") == "navDHOrder") { //大货单
                Params.filterType = 3;
                $(".search-orderstatus").show();
                if ((!_this.data("first") || _this.data("first") == 0)) {
                    _this.data("first", "1");

                    Params.keyWords = "";
                    _self.getList();
                }
            }
        });
        $(".module-tab li").first().click();
    }

    //获取订单列表
    Clients.getList = function () {
        var _self = this;
        Clients.isLoading = false;
        var targetid = "navOppor";
        var action = "GetNeedsOrderByClientID";
        if (Params.filterType != 1) {
            action = "GetOrdersByClientID";
            if (Params.filterType == 3) {
                Params.ordertype = 2;
                targetid = "navDHOrder";
            } else {
                Params.ordertype = 1;
                targetid = "navOrder";
            }
        }

        var _target = $("#" + targetid + " .table-header")
        _target.nextAll().remove();
        _target.after("<tr><td colspan='10'><div class='data-loading' ><div></td></tr>");

        Global.post("/Orders/" + action, Params, function (data) {
            Clients.isLoading = true;

            Clients.bindList(data, _target);
        });
    }

    //加载列表
    Clients.bindList = function (data, _target) {
        var _self = this;
        _target.nextAll().remove();

        if (data.items.length > 0) {
            doT.exec("template/orders/orders.html", function (template) {
                data.items.customerShow = 1;
                var innerhtml = template(data.items);
                innerhtml = $(innerhtml);

                _target.after(innerhtml);

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

            });
        }
        else {
            _target.after("<tr><td colspan='11'><div class='nodata-txt' >暂无数据!<div></td></tr>");
        }

        var pagerid = "pagerOppor";
        if (Params.filterType == 2) {
            pagerid = "pagerOrder";
        } else if (Params.filterType == 3) {
            pagerid = "pagerDHOrder";
        }
        $("#" + pagerid).paginate({
            total_count: data.totalCount,
            count: data.pageCount,
            start: Params.pageindex,
            display: 5,
            border: true,
            rotate: true,
            images: false,
            mouse: 'slide',
            onChange: function (page) {
                Params.pageindex = page;
                _self.getList();
            }
        });
    }
    Clients.getDetail = function (id, orderCode) {

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
    module.exports = Clients;
});