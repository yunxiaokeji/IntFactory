﻿define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Verify = require("verify"), VerifyObject,
        City = require("city"), CityObject,
        Easydialog = require("easydialog"),
        ChooseUser = require("chooseuser");
    require("pager");

    var Model = {}, CacheRole = [];

    var ObjectJS = {};

    ObjectJS.Params = {
        PageIndex: 1,
        DepartID:"",
        RoleID:"",
        KeyWords: ""
    };

    //初始化
    ObjectJS.init = function (roles, departs, mdtoken) {
        var _self = this;
        roles = JSON.parse(roles.replace(/&quot;/g, '"'));
        departs = JSON.parse(departs.replace(/&quot;/g, '"'));
        CacheRole = roles;

        if (!mdtoken) {
            $("#addMDUser").hide();
        }

        _self.bindEvent(roles, departs);
        _self.getList();
    }
    //绑定事件
    ObjectJS.bindEvent = function (roles, departs) {
        var _self = this;

        //角色搜索
        require.async("dropdown", function () {
            $("#ddlRole").dropdown({
                prevText: "角色-",
                defaultText: "全部",
                defaultValue: "",
                data: roles,
                dataValue: "RoleID",
                dataText: "Name",
                width: "180",
                onChange: function (data) {
                    ObjectJS.Params.PageIndex = 1;
                    ObjectJS.Params.RoleID = data.value;
                    ObjectJS.getList();
                }
            });
        });

        //部门搜索
        require.async("dropdown", function () {
            $("#ddlDepart").dropdown({
                prevText: "部门-",
                defaultText: "全部",
                defaultValue: "",
                data: departs,
                dataValue: "DepartID",
                dataText: "Name",
                width: "180",
                onChange: function (data) {
                    ObjectJS.Params.PageIndex = 1;
                    ObjectJS.Params.DepartID = data.value;
                    ObjectJS.getList();
                }
            });
        });

        $(document).click(function (e) {
            //隐藏下拉
            if (!$(e.target).parents().hasClass("dropdown-ul") && !$(e.target).parents().hasClass("dropdown") && !$(e.target).hasClass("dropdown")) {
                $(".dropdown-ul").hide();
            }
        });

        //搜索框
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                ObjectJS.Params.PageIndex = 1;
                ObjectJS.Params.keyWords = keyWords;
                ObjectJS.getList();
            });
        });

        //添加明道用户
        $("#addMDUser").click(function () {
            ChooseUser.create({
                title: "明道用户导入",
                type: 2,
                single: false,
                callback: function (items) {
                    var ids = "";
                    for (var i = 0; i < items.length; i++) {
                        ids += items[i].id + ",";
                    }
                    if (ids.length > 0) {
                        Global.post("/Organization/SaveMDUser", {
                            parentid: "",
                            mduserids: ids
                        }, function (data) {
                            if (data.status) {
                                alert("明道用户导入成功，新用户需设置角色后才能正常使用系统！");
                                _self.getList();
                            }
                        });
                    }
                }
            });
        });
        //注销员工
        $("#deleteObject").click(function () {
            var _this = $(this);
            confirm("员工注销后不能再使用系统且不可恢复，确认注销吗?", function () {
                Global.post("/Organization/DeleteUserByID", {
                    userid: _this.data("id")
                }, function (data) {
                    if (data.status) {
                        _self.getList();
                    }
                });
            });
        });
        //设置角色
        $("#setObjectRole").click(function () {
            var _this = $(this);
            doT.exec("template/organization/setuserrole.html", function (template) {
                var innerHtml = template(CacheRole);
                Easydialog.open({
                    container: {
                        id: "show-model-setRole",
                        header: "设置员工角色",
                        content: innerHtml,
                        yesFn: function () {
                            $("#setUserRoleBox .role-item").each(function () {
                                var _role = $(this);
                                //保存角色
                                if (_role.hasClass("hover")) {
                                    if (_role.data("id") == _this.data("roleid")) {
                                        return;
                                    }
                                    Global.post("/Organization/UpdateUserRole", {
                                        userid: _this.data("id"),
                                        roleid: _role.data("id")
                                    }, function (data) {
                                        if (data.status) {
                                            _self.getList();
                                        }
                                    });
                                    return;
                                }
                            });
                        },
                        callback: function () {

                        }
                    }
                });
                //默认选中当前角色
                if (_this.data("roleid")) {
                    $("#setUserRoleBox .role-item[data-id=" + _this.data("roleid") + "]").addClass("hover");
                }

                $("#setUserRoleBox .role-item").click(function () {
                    $(this).siblings().removeClass("hover");
                    $(this).addClass("hover");
                });
            });
        });
    }

    //获取列表
    ObjectJS.getList = function () {
        var _self = this;
        $(".tr-header").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='8'><div class='dataLoading'><img src='/modules/images/ico-loading.jpg'/><div></td></tr>");
        Global.post("/Organization/GetUsers", { filter: JSON.stringify(ObjectJS.Params) }, function (data) {
            _self.bindList(data.items);

            $("#pager").paginate({
                total_count: data.totalCount,
                count: data.pageCount,
                start: ObjectJS.Params.PageIndex,
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
                    ObjectJS.Params.PageIndex = page;
                    _self.getList();
                }
            });

        });
    }
    //加载列表
    ObjectJS.bindList = function (items) {
        var _self = this;
        $(".tr-header").nextAll().remove();

        if (items.length > 0) {
            doT.exec("template/organization/users.html", function (template) {
                var innerhtml = template(items);
                innerhtml = $(innerhtml);

                //操作
                innerhtml.find(".dropdown").click(function () {
                    var _this = $(this);
                    var position = _this.find(".ico-dropdown").position();
                    $(".dropdown-ul li").data("id", _this.data("id")).data("roleid", _this.data("roleid"));

                    $(".dropdown-ul").css({ "top": position.top + 20, "left": position.left-80 }).show().mouseleave(function () {
                        $(this).hide();
                    });
                });

                $(".tr-header").after(innerhtml);
            });
        }
        else {
            $(".tr-header").after("<tr><td colspan='8'><div class='noDataTxt' >暂无数据!<div></td></tr>");
        }
    }

    //添加/编辑弹出层
    ObjectJS.createModel = function () {
        var _self = this;

        doT.exec("template/organization/department-detail.html", function (template) {
            var html = template([]);
            Easydialog.open({
                container: {
                    id: "show-model-detail",
                    header: !Model.DepartID ? "新建部门" : "编辑部门",
                    content: html,
                    yesFn: function () {
                        if (!VerifyObject.isPass()) {
                            return false;
                        }
                        Model.Name = $("#modelName").val();
                        Model.Description = $("#modelDescription").val();
                        Model.ParentID = "";
                        _self.saveModel(Model);
                    },
                    callback: function () {

                    }
                }
            });
            VerifyObject = Verify.createVerify({
                element: ".verify",
                emptyAttr: "data-empty",
                verifyType: "data-type",
                regText: "data-text"
            });
            $("#modelName").focus();
            $("#modelName").val(Model.Name);
            $("#modelDescription").val(Model.Description);

        });
    }
    //保存实体
    ObjectJS.saveModel = function (model) {
        var _self = this;
        Global.post("/Organization/SaveDepartment", { entity: JSON.stringify(model) }, function (data) {
            if (data.model.DepartID.length > 0) {
                _self.getList();
            }
        })
    }

    //新建员工逻辑和交互
    ObjectJS.initCreate = function () {
        var _self = this;
        _self.bindCreateEvent();
    }

    ObjectJS.bindCreateEvent = function () {
        var _self = this;

        VerifyObject = Verify.createVerify({
            element: ".verify",
            emptyAttr: "data-empty",
            verifyType: "data-type",
            regText: "data-text"
        });

        CityObject = City.createCity({
            elementID: "city"
        });

        $("#loginname").change(function () {
            var _this = $(this);
            if (_this.val().trim() && _this.val().trim().length >= 6) {
                Global.post("/Organization/IsExistLoginName", { loginname: _this.val() }, function (data) {
                    if (data.status) {
                        alert("账号已存在，请重新输入！");
                        _this.focus().data("status", "1");
                    } else {
                        _this.data("status", "0");
                    }
                });
            }
        });

        $("#btnSave").click(function () {
            if (!VerifyObject.isPass()) {
                return false;
            }

            if ($("#loginname").val().trim().length < 6) {
                alert("账号长度不能低于6位！");
                return false;
            }

            if ($("#loginname").data("status") == 1) {
                alert("账号已存在，请重新输入！");
                return false;
            }

            _self.saveUser();
        });

    }

    //保存员工
    ObjectJS.saveUser = function () {
        var _self = this;

        var model = {
            LoginName: $("#loginname").val().trim(),
            Name: $("#name").val().trim(),
            RoleID: $("#role").val().trim(),
            DepartID: $("#departments").val().trim(),
            CityCode: CityObject.getCityCode(),
            Address: $("#address").val().trim(),
            Email: $("#email").val().trim(),
            Jobs: $("#jobs").val().trim(),
            Description: $("#remark").val().trim()
        };
        Global.post("/Organization/SaveUser", { entity: JSON.stringify(model) }, function (data) {
            if (data.model && data.model.UserID) {
                confirm("员工保存成功,初始密码为登录账号,是否继续添加员工?", function () {
                    location.href = location.href;
                }, function () {
                    location.href = "/Organization/Users";
                });

            } else if (data.result == 2) {
                alert("员工保存失败，登录账号已存在！");
            } else if (data.result == 3) {
                alert("公司员工数已达到购买人数上限，请先选择购买人数！");
            }
        });
    }

    module.exports = ObjectJS;
});