﻿define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        ChooseUser = require("chooseuser");
    require("pager");
    require("mark");

    var Params = {
        SearchType: 1,
        Type: -1,
        SourceID: "",
        StageID: "",
        Status: 1,
        Mark: -1,
        UserID: "",
        AgentID: "",
        TeamID: "",
        Keywords: "",
        BeginTime: "",
        EndTime: "",
        PageIndex: 1,
        PageSize: 20
    };

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function (type) {
        var _self = this;
        Params.SearchType = type;
        _self.getList();
        _self.bindEvent(type);
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

        //切换阶段
        $(".search-stages li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                Params.PageIndex = 1;
                Params.StageID = _this.data("id");
                _self.getList();
            }
        });
        //切换状态
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
        //关键字搜索
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                Params.PageIndex = 1;
                Params.Keywords = keyWords;
                _self.getList();
            });
        });
        //客户来源
        Global.post("/Customer/GetCustomerSources", { }, function (data) {
            require.async("dropdown", function () {
                $("#customerSource").dropdown({
                    prevText: "来源-",
                    defaultText: "全部",
                    defaultValue: "",
                    data: data.items,
                    dataValue: "SourceID",
                    dataText: "SourceName",
                    width: "140",
                    onChange: function (data) {
                        Params.PageIndex = 1;
                        Params.SourceID = data.value;
                        _self.getList();
                    }
                });
            });
        });
        //客户类型
        require.async("dropdown", function () {
            var items = [{ ID: 1, Name: "企业客户" }, { ID: 0, Name: "个人客户" }];
            $("#customerType").dropdown({
                prevText: "类型-",
                defaultText: "全部",
                defaultValue: "-1",
                data: items,
                dataValue: "ID",
                dataText: "Name",
                width: "120",
                onChange: function (data) {
                    Params.PageIndex = 1;
                    Params.Type = data.value;
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
                    width: "180",
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
                    width: "180",
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
                title: "更换拥有者",
                type: 1,
                single: true,
                callback: function (items) {
                    if (items.length > 0) {
                        if (_this.data("userid") != items[0].id) {
                            _self.ChangeOwner(_this.data("id"), items[0].id);
                        } else {
                            alert("请选择不同人员进行转移!");
                        }
                    }
                }
            });
        });
        //批量转移
        $("#batchChangeOwner").click(function () {
            var checks = $(".table-list .ico-checked");
            if (checks.length > 0) {
                ChooseUser.create({
                    title: "批量更换拥有者",
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
                                alert("请选择不同人员进行转移!");
                            }
                        }
                    }
                });
            } else {
                alert("您尚未选择客户!")
            }
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
        //批量标记
        $("#batchMark").markColor({
            isAll: true,
            onChange: function (obj, callback) {
                var checks = $(".table-list .ico-checked");
                if (checks.length > 0) {
                    var ids = "";
                    checks.each(function () {
                        var _this = $(this);
                        ids += _this.data("id") + ",";
                    });
                    _self.markCustomer(ids, obj.data("value"), function (status) {
                        _self.getList();
                        callback && callback(status);
                    });
                    
                } else {
                    alert("您尚未选择客户!")
                }
            }
        });
        
    }
    //获取列表
    ObjectJS.getList = function () {
        var _self = this;
        $("#checkAll").addClass("ico-check").removeClass("ico-checked");
        $(".tr-header").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='12'><div class='dataLoading' ><img src='/modules/images/ico-loading.jpg'/><div></td></tr>");

        Global.post("/Customer/GetCustomers", { filter: JSON.stringify(Params) }, function (data) {
            _self.bindList(data);
        });
    }
    //加载列表
    ObjectJS.bindList = function (data) {
        var _self = this;
        $(".tr-header").nextAll().remove();

        if (data.items.length > 0) {
            doT.exec("template/customer/customers.html", function (template) {
                var innerhtml = template(data.items);
                innerhtml = $(innerhtml);

                //下拉事件
                innerhtml.find(".dropdown").click(function () {
                    var _this = $(this);
                    var position = _this.find(".ico-dropdown").position();
                    $(".dropdown-ul li").data("id", _this.data("id")).data("userid", _this.data("userid"));
                    $(".dropdown-ul").css({ "top": position.top + 20, "left": position.left - 80 }).show().mouseleave(function () {
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

                //innerhtml.click(function () {
                //    var _this = $(this).find(".check");
                //    if (!_this.hasClass("ico-checked")) {
                //        _this.addClass("ico-checked").removeClass("ico-check");
                //    } else {
                //        _this.addClass("ico-check").removeClass("ico-checked");
                //    }
                //});

                innerhtml.find(".mark").markColor({
                    isAll: false,
                    onChange: function (obj, callback) {

                        _self.markCustomer(obj.data("id"), obj.data("value"), callback);

                    }
                });

                $(".tr-header").after(innerhtml);

            });
        }
        else
        {
            $(".tr-header").after("<tr><td colspan='12'><div class='noDataTxt' >暂无数据!<div></td></tr>");
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
    //标记客户
    ObjectJS.markCustomer = function (ids, mark, callback) {
        if (mark < 0) {
            alert("不能标记此选项!");
            return false;
        }
        Global.post("/Customer/UpdateCustomMark", {
            ids: ids,
            mark: mark
        }, function (data) {
            callback && callback(data.status);
        });
    }
    //转移客户
    ObjectJS.ChangeOwner = function (ids, userid) {
        var _self = this;
        Global.post("/Customer/UpdateCustomOwner", {
            userid: userid,
            ids: ids
        }, function (data) {
            if (data.status) {
                _self.getList();
            }
        });
    }

    module.exports = ObjectJS;
});