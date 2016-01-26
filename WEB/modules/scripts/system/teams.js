define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Verify = require("verify"), VerifyObject,
        Easydialog = require("easydialog"),
        ChooseUser = require("chooseuser");

    var Model = {}, CacheChild = [];

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function (model) {
        var _self = this;

        _self.getList();
        _self.bindEvent();

    }
    //绑定事件
    ObjectJS.bindEvent = function (parentid) {
        var _self = this;

        $(document).click(function (e) {
            //隐藏下拉
            if (!$(e.target).hasClass("ddlteam")) {
                $("#ddlTeam").hide();
            }

            if (!$(e.target).hasClass("ddluser")) {
                $("#ddlUser").hide();
            }
        });

        //添加团队
        $("#addTeam").click(function () {
            _self.createModel();
        });

        //编辑团队
        $("#editTeam").click(function () {
            var _this = $(this);
            var model = {
                TeamID: _this.data("id"),
                TeamName: _this.data("name").trim()
            };
            _self.createModel(model);
        });

        //删除团队
        $("#deleteTeam").click(function () {
            var _this = $(this);

            confirm("团队删除后不可恢复,确认删除吗？", function () {
                Global.post("/System/DeleteTeam", { id: _this.data("id") }, function (data) {
                    if (data.status) {
                        _self.getList();
                    } else {
                        alert("团队删除失败!");
                    }
                });
            });

           
        });

        //添加下属
        $("#addUser").click(function () {
            var _this = $(this);
            ChooseUser.create({
                title: "添加团队成员",
                type: 4,
                single: false,
                callback: function (items) {
                    if (items.length > 0) {
                        var ids = "";
                        for (var i = 0; i < items.length; i++) {
                            ids += items[i].id + ",";
                        }
                        Global.post("/System/UpdateUserTeamID", {
                            ids: ids,
                            teamid: _this.data("id")
                        }, function (data) {
                            if (data.status) {
                                location.href = location.href;
                            }
                        });
                    }
                }
            });
        });

        //移除成员
        $("#removeObject").click(function () {
            var _this = $(this);
            confirm("确认把此成员移出团队吗?", function () {
                Global.post("/System/UpdateUserTeamID", {
                    ids: _this.data("id"),
                    teamid: ""
                }, function (data) {
                    if (data.status) {
                        location.href = location.href;
                    }
                });
            });
        });
    }

    ObjectJS.getList = function () {
        var _self = this;
        $("#teams").empty();
        Global.post("/System/GetTeams", {}, function (data) {
            _self.bindTeams(data.items);
        });
    }

    ObjectJS.createModel = function (model) {
        var _self = this;

        doT.exec("template/system/team-detail.html", function (template) {
            var html = template([]);
            Easydialog.open({
                container: {
                    id: "show-model-detail",
                    header: !model ? "新建团队" : "编辑团队名称",
                    content: html,
                    yesFn: function () {
                        if (!$("#teamname").val().trim()) {
                            alert("团队名称不能为空!");
                            return false;
                        }
                        var entity = {
                            TeamID: model ? model.TeamID : "",
                            TeamName: $("#teamname").val().trim()
                        }
                        _self.saveModel(entity);
                    },
                    callback: function () {

                    }
                }
            });
            $("#teamname").focus();
            if (model) {
                $("#teamname").val(model.TeamName);
            }

        });
    }

    ObjectJS.saveModel = function (model) {
        var _self = this;
        Global.post("/System/SaveTeam", { entity: JSON.stringify(model) }, function (data) {
            if (data.model.TeamID) {
                _self.getList();
            }
        })
    }

    ObjectJS.bindElement = function (element) {
        var _self = this;
        element.find(".ddlteam").click(function () {
            var _this = $(this);
            var offset = _this.offset();
            $("#ddlTeam li").data("id", _this.data("id")).data("name", _this.siblings().html());
            $("#ddlTeam").css({ "top": offset.top + 20, "left": offset.left }).show().mouseleave(function () {
                $(this).hide();
            });
        });

        element.find(".ddluser").click(function () {
            var _this = $(this);
            var offset = _this.offset();
            $("#ddlUser li").data("id", _this.data("id"));
            $("#ddlUser").css({ "top": offset.top + 20, "left": offset.left }).show().mouseleave(function () {
                $(this).hide();
            });
        });
    }
    //绑定下级
    ObjectJS.bindTeams = function (items) {
        var _self = this;
        for (var i = 0, j = items.length; i < j; i++) {
            var team = items[i];
            CacheChild[team.TeamID] = team.Users;
        }
        doT.exec("template/system/teams.html", function (template) {
            var innerHtml = template(items);
            innerHtml = $(innerHtml);
            $("#teams").append(innerHtml);

            //绑定事件
            _self.bindElement(innerHtml);

            //展开
            innerHtml.find(".openchild").each(function () {
                var _this = $(this);
                var _obj = _self.getChild(_this.attr("data-id"), _this.prevUntil("div").html(), _this.attr("data-eq"), CacheChild[_this.attr("data-id")]);
                _this.parent().after(_obj);
                _this.on("click", function () {
                    if (_this.attr("data-state") == "close") {
                        _this.attr("data-state", "open");
                        _this.removeClass("icoopen").addClass("icoclose");

                        $("#" + _this.attr("data-id")).show();

                    } else { //隐藏子下属
                        _this.attr("data-state", "close");
                        _this.removeClass("icoclose").addClass("icoopen");

                        $("#" + _this.attr("data-id")).hide();
                    }
                });
            });
        });
    }
    //展开下级
    ObjectJS.getChild = function (pid, provHtml, isLast, items) {
        var _self = this;
        var _div = $(document.createElement("div")).attr("id", pid).addClass("hide");
        doT.exec("template/system/teamuser.html", function (template) {

            for (var i = 0; i < CacheChild[pid].length; i++) {
                var _item = $(document.createElement("div")).addClass("child-item");

                //添加左侧背景图
                var _leftBg = $(document.createElement("div")).css("display", "inline-block").addClass("left");
                _leftBg.append(provHtml);
                if (isLast == "last") {
                    _leftBg.append("<span class='null left'></span>");
                } else {
                    _leftBg.append("<span class='line left'></span>");
                }
                _item.append(_leftBg);

                //是否最后一位
                if (i == CacheChild[pid].length - 1) {
                    _item.append("<span class='lastline left'></span>");

                } else {
                    _item.append("<span class='leftline left'></span>");
                }

                var innerTtml = template(CacheChild[pid][i]);
                _item.append(innerTtml);

                _div.append(_item);

                //绑定事件
                _self.bindElement(_item);
            }
        });
        return _div;
    }

    module.exports = ObjectJS;
});