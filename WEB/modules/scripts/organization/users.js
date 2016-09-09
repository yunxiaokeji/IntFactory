﻿define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Verify = require("verify"), VerifyObject,
        City = require("city"), CityObject,
        Easydialog = require("easydialog"),
        ChooseUser = require("chooseuser");
    require("tiplayer");
    require("pager");

    var Model = {}, CacheRole = [];

    var ObjectJS = {};

    ObjectJS.Params = {
        PageIndex: 1,
        DepartID:"",
        RoleID:"",
        KeyWords: ""
    };

    ObjectJS.loginName = "";

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
            }, "注销");
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
        //重置密码
        $("#resetPassword").click(function () {
            var _this = $(this);
            var userid = _this.data("id");
            var tr = $(".list-item .dropdown[data-id=" + _this.data("id") + "]").parent();
            var html = "<div>";
            html += "<div><span style='display:inline-block;width:80px;'>密码：</span><input id='passLayer' maxlength='20' type='password' /></div>";
            html += "<div class='mTop5'><span style='display:inline-block;width:80px;'>确认密码：</span><input id='confirmPassLayer' maxlength='20' type='password' /></div>";
            html += "</div>";
            Easydialog.open({
                container: {
                    id: "resetPassLayer",
                    header: "重置密码",
                    content: html,
                    yesFn: function () {
                        console.log($("#resetPassLayer").val());
                        console.log($("#passLayer").val());

                        if (!$("#passLayer").val() || $("#passLayer").val().trim().length < 6) {
                            alert("密码不能小于6位数", 2);
                            return false;
                        } else if ($("#confirmPassLayer").val() != $("#passLayer").val()) {
                            alert("密码不一致,请重新输入", 2);
                            return false;
                        } else {
                            Global.post("/Organization/UpdateUserPwd", {
                                userID: userid,
                                loginPwd: $("#passLayer").val()
                            }, function (data) {
                                if (data.status) {
                                    alert("密码重置成功");
                                } else {
                                    alert("服务器繁忙！请稍后再试", 2);
                                    return false;
                                }
                            });
                        }
                    }
                }
            });
        });
        
        //解除手机号绑定
        $("#resetMobilePhone").click(function () {
            var _this = $(this);
            var userid = _this.data("id");
            var tr = $(".list-item .dropdown[data-id=" + _this.data("id") + "]").parent();
            var showmsg = "确认解除&nbsp;<span class='red'>" + tr.find('.name').html() + "<span>&nbsp;的绑定手机?";
            confirm(showmsg, function () {
                Global.post("/Organization/UpdateMobilePhone", {
                    userID: userid
                }, function (data) {
                    if (data.result == 1) {
                        tr.find('.mobile').html('');
                        alert("手机号解绑成功");
                    }
                    else if (data.result == 2) {
                        alert("手机号解绑成功");
                    }
                    else if (data.result == 3) {
                        alert("没有绑定手机号", 2);
                    }
                    else {
                        alert("服务器繁忙！请稍后再试", 2);
                    }
                });
            });
        });

        //编辑员工基本信息
        $("#editBaseInfo").click(function () {
            var _this = $(this);
            doT.exec("template/organization/staff-detail.html", function (template) {
                var tr = $(".list-item .dropdown[data-id=" + _this.data("id") + "]").parent();
                var staff = {
                    account: tr.find('.account').html(),
                    name: tr.find('.name').html(),
                    mobilePhone: tr.find('.mobile').html(),
                    email:tr.find('.email').html()
                };
                var innerHtml = template(staff);

                var departID = tr.find('.department').data('id');
                var departName = tr.find('.department').html();
                Easydialog.open({
                    container: {
                        id: "StaffInfo",
                        header: "编辑员工信息",
                        content: innerHtml,
                        yesFn: function () {
                            if (!VerifyObject.isPass()) {
                                return false;
                            }
                            var newParams = {
                                Name: $("#name").val(),
                                DepartID: departID,
                                MobilePhone: $("#mobile").val(),
                                Email: $("#email").val()
                            };
                            Global.post("/Organization/UpdateUserBaseInfo", { entity: JSON.stringify(newParams), userID: _this.data("id") }, function (data) {
                                if (data.result == 1) {
                                    tr.find('.staff-name').html(newParams.Name);
                                    tr.find('.mobile').html(newParams.MobilePhone);
                                    tr.find('.email').html(newParams.Email);
                                    tr.find('.department').html(departName);
                                    alert("修改成功");
                                }
                                else {
                                    alert("修改失败", 2);
                                }
                            })

                        },
                        callback: function () {

                        }
                    }

                });
                require.async("dropdown", function () {
                    var drop = $("#depart");
                    drop.css("width", "130px");

                    $("#depart").dropdown({
                        prevText: "部门-",
                        defaultText: departName,
                        defaultValue: departID,
                        data: departs,
                        dataValue: "DepartID",
                        dataText: "Name",
                        width: 130,
                        isposition:true,
                        onChange:function (data) {
                            departID = data.value;
                            departName = data.text;
                            tr.find('.department').data('id', data.value);
                        }
                    })
                })


                VerifyObject = Verify.createVerify({
                    element: ".verify",
                    emptyAttr: "data-empty",
                    verifyType: "data-type",
                    regText: "data-text"
                });



            })
        })

    }

    //获取列表
    ObjectJS.getList = function () {
        var _self = this;
        $(".tr-header").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='9'><div class='data-loading'><div></td></tr>");
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
                    $("#resetMobilePhone").show();
                    if (_this.data('status') == 0) {
                        $("#resetMobilePhone").hide();
                    }
                    ObjectJS.loginName = _this.data("name");
                    var position = _this.find(".ico-dropdown").position();
                    $(".dropdown-ul li").data("id", _this.data("id")).data("roleid", _this.data("roleid"));

                    $(".dropdown-ul").css({ "top": position.top + 20, "left": position.left - 80 }).show().mouseleave(function () {
                        $(this).hide();
                    });
                });

                $(".tr-header").after(innerhtml);
            });
        }
        else {
            $(".tr-header").after("<tr><td colspan='9'><div class='nodata-txt' >暂无数据!<div></td></tr>");
        }
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
                        $("#loginname").showTipLayer({ content: "账号已存在，请重新输入！" });
                        _this.focus().data("status", "1");
                    } else {
                        _this.data("status", "0");
                    }
                });
            }
        });

        $("#loginpass").change(function () {
            var _this = $(this);
            if (_this.val().length < 6) {
                _this.css({ "border-color": "red" });
                _this.showTipLayer({
                    content: "密码不能小于6位"
                });
            } else {
                _this.css({ "border-color": "#ccc" });
            }
        });

        $("#confirmpass").change(function () {
            var _this = $(this);
            if (_this.val() != $("#loginpass").val()) {
                _this.css({ "border-color": "red" });
                _this.showTipLayer({
                    content: "确认密码与原密码不一致"
                });
            } else {
                _this.css({ "border-color": "#ccc" });
            }
        });

        $("#btnSave").click(function () {
            var isContinue = true;
            if (!VerifyObject.isPass()) {
                return false;
            }

            if ($("#loginname").val().trim().length < 6) {
                $("#loginname").showTipLayer({ content: "账号长度不能少于6位！" });
                isContinue = false;
            }

            if ($("#loginpass").val().trim().length < 6) {
                $("#loginpass").showTipLayer({ content: "密码长度不能少于6位！" });
                isContinue = false;
            }

            if ($("#confirmpass").val() != $("#loginpass").val()) {
                $("#confirmpass").showTipLayer({ content: "确认密码与原密码不一致" });
                isContinue = false;
            }

            if ($("#loginname").data("status") == 1) {
                $("#loginname").showTipLayer({ content: "账号已存在，请重新输入！" });
                isContinue = false;
            }
            if (isContinue) {
                _self.saveUser();
            }
        });

    }

    //保存员工
    ObjectJS.saveUser = function () {
        var _self = this;

        var model = {
            LoginName: $("#loginname").val().trim(),
            LoginPWD: $("#loginpass").val().trim(),
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
                confirm("员工保存成功,是否继续添加员工?", function () {
                    location.href = location.href;
                }, "继续", function () {
                    location.href = "/Organization/Users";
                });
            } else if (data.result == 2) {
                alert("员工保存失败，登录账号已存在！", 2);
            } else if (data.result == 3) {
                alert("公司员工数已达到购买人数上限，请先选择购买人数！", 2);
            }
        }, "添加");
    }

    module.exports = ObjectJS;
});