define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        City = require("city"), CityObject, CityContact,
        Verify = require("verify"), VerifyObject, VerifyContact,
        ChooseUser = require("chooseuser"),
        moment = require("moment"),
        Easydialog = require("easydialog");
        Tip = require("tip");
    require("daterangepicker");
    require("pager");
    require("colormark");

    var Params = {
        SearchType: 1,
        Type: -1,
        SourceType: -1,
        SourceID: "",
        StageID: "",
        Status: 1,
        FirstName:"",
        Mark: -1,
        UserID: "",
        AgentID: "",
        TeamID: "",
        Keywords: "",
        BeginTime: "",
        EndTime: "",
        PageIndex: 1,
        OrderBy: "cus.CreateTime desc",
        PageSize: 15
    };

    var ObjectJS = {};
    ObjectJS.ColorList = [];

    ObjectJS.isLoading = true;

    //初始化
    ObjectJS.init = function (type,model) {
        var _self = this;
        _self.userid = $("#currentUserID").val();
        Params.SearchType = type;
        _self.ColorList = JSON.parse(model.replace(/&quot;/g, '"'));

        _self.getList();
        _self.bindEvent(Params.SearchType);
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

        //选择客户来源类型
        $(".customer-source .item").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");

                Params.SourceType = -1;
                var dataid = _this.data("idsource");
                Params.SourceType = dataid;
                ObjectJS.getList();
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
                _self.getList();
            }
        });

        $(".search-mark .item:gt(0)").each(function () {
            var _this = $(this);
            _this.Tip({
                width: 80,
                msg: _this.data("name")
            });
        });

        //选择字母
        $(".search-letter li").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            var _this = $(this);
            $(".data-loading").remove();
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");

                var datanum = _this.data("letter");
                if (datanum != "1") {
                    Params.FirstName = datanum;
                } else {
                    _this.css("font-size", "14px");
                    $(".search-letter li:eq(1)").addClass("hover");
                    Params.FirstName = "";
                }
                _self.getList();
            };
        });

        //切换状态
        $(".search-status li").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
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

        if (type != 3) {
            require.async("choosebranch", function () {
                $("#chooseBranch").chooseBranch({
                    prevText: "人员-",
                    defaultText: ["我的","我的下属"],
                    defaultValue: [_self.userid, ""],
                    defaultIndex:1,
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
                    defaultText: ["我的", "所有员工"],
                    defaultValue: [_self.userid, ""],
                    defaultIndex: 1,
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

        $("#batchMark").markColor({
            isAll: true,
            left: 10,
            data: _self.ColorList,
            onChange: function (obj, callback) {
                var checks = $(".table-list .checkbox.hover");
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
                    alert("您尚未选择客户", 2);
                }
            }
        });

        //全部选中
        $("#checkAll").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            var _this = $(this).find(".checkbox");
            if (!_this.hasClass("hover")) {
                _this.addClass("hover");
                $(".checkbox").addClass("hover");
            } else {
                _this.removeClass("hover");
                $(".checkbox").removeClass("hover");
            }
        });

        //转移拥有者
        $("#changeOwner").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
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
                            alert("请选择不同人员进行更换!", 2);
                        }
                    }
                }
            });
        });

        //批量转移
        $("#batchChangeOwner").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            var checks = $(".table-list .checkbox.hover");
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
                                alert("请选择不同人员进行更换!", 2);
                            }
                        }
                    }
                });
            } else {
                alert("您尚未选择客户!", 2)
            }
        });

        //排序
        $(".sort-item").click(function () {
            var _this = $(this);

            if (!ObjectJS.isLoading) {
                return;
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

        $("#createCustomer").click(function () {
            _self.createCustomer();
        });
    };

    //获取列表
    ObjectJS.getList = function () {
        var _self = this;
        $("#checkAll").removeClass("hover");
        $(".tr-header").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='15'><div class='data-loading'><div></td></tr>");

        ObjectJS.isLoading = false;
        Global.post("/Customer/GetCustomers", { filter: JSON.stringify(Params) }, function (data) {
            $(".tr-header").nextAll().remove();
            _self.bindCardList(data);            
            ObjectJS.isLoading = true;
        });
    }

    //加载卡片式列表
    ObjectJS.bindCardList = function (data) {
        var _self = this;

        if (data.items.length > 0) {
            doT.exec("template/customer/customers.html", function (template) {
                var innerhtml = template(data.items);
                innerhtml = $(innerhtml);

                innerhtml.find(".checkbox").click(function () {
                    var _this = $(this);
                    if (!_this.hasClass("hover")) {
                        _this.addClass("hover");
                    } else {
                        _this.removeClass("hover");
                    }
                    return false;
                });

                innerhtml.find(".mark").markColor({
                    isAll: false,
                    data: _self.ColorList,
                    onChange: function (obj, callback) {
                        _self.markCustomer(obj.data("id"), obj.data("value"), callback);
                    }
                });

                $(".tr-header").after(innerhtml);
            });
        } else {
            $(".tr-header").after("<tr><td colspan='15'><div class='nodata-txt'>暂无数据<div></td></tr>");
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
    };

    ObjectJS.createCustomer = function () {
        var _self = this;
        doT.exec("template/customer/customer-detail.html", function (template) {
            var innerText = template([]);
            Easydialog.open({
                container: {
                    id: "show-model-detail",
                    header: "新建客户",
                    content: innerText,
                    yesFn: function () {
                        if (!VerifyObject.isPass()) {
                            return false;
                        }
                        var entity = {
                            CustomerID: "",
                            Name: $("#name").val().trim(),
                            Type: 0,
                            IndustryID: "",
                            Extent: 0,
                            CityCode: CityObject.getCityCode(),
                            Address: $("#address").val().trim(),
                            MobilePhone: $("#contactMobile").val().trim(),
                            Email: $("#email").val().trim(),
                            Description: $("#remark").val().trim()
                        };
                        _self.saveModel(entity);
                    },
                    callback: function () {

                    }
                }
            });

            CityObject = City.createCity({
                cityCode: "",
                elementID: "city"
            });
            VerifyObject = Verify.createVerify({
                element: ".verify",
                emptyAttr: "data-empty",
                verifyType: "data-type",
                regText: "data-text"
            });
        });
    }

    ObjectJS.saveModel = function (entity) {
        var _self = this;

        Global.post("/Customer/SaveCustomer", { entity: JSON.stringify(entity) }, function (data) {
            if (data.model.CustomerID) {
                _self.getList();

            } else {
                alert("客户创建失败,联系电话已存在!", 2);
            }
        });
    }
    //标记客户
    ObjectJS.markCustomer = function (ids, mark, callback) {
        if (mark < 0) {
            alert("不能标记此选项!", 2);
            return false;
        }
        ObjectJS.isLoading = false;
        Global.post("/Customer/UpdateCustomMark", {
            ids: ids,
            mark: mark
        }, function (data) {
            if (data.result == "10001") {
                alert("您没有该操作权限，请联系管理员");
                return false;
            }
            callback && callback(data.status);
            ObjectJS.isLoading = true;
        });
    }

    //转移客户
    ObjectJS.ChangeOwner = function (ids, userid) {
        var _self = this;
        ObjectJS.isLoading = false;
        Global.post("/Customer/UpdateCustomOwner", {
            userid: userid,
            ids: ids
        }, function (data) {
            if (data.status) {
                _self.getList();
            }
            ObjectJS.isLoading = true;
        });
    }

    module.exports = ObjectJS;
});

