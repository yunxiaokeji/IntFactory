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

        model = JSON.parse(model.replace(/&quot;/g, '"'));
        _self.bindEvent(model.UserID);
        _self.bindElement($(".user-item"));
        if (model.UserID) {
            _self.bindChild(model.ChildUsers);
        }
    }
    //绑定事件
    ObjectJS.bindEvent = function (parentid) {
        var _self = this;

        $(document).click(function (e) {
            //隐藏下拉
            if (!$(e.target).parents().hasClass("dropdown-ul") && !$(e.target).hasClass("ico-dropdown")) {
                $(".dropdown-ul").hide();
            }
        });

        //选择最高领导人
        if (!parentid) {
            $("#structure").hide();
            $(".create-first").show().click(function () {
                ChooseUser.create({
                    title: "选择最高领导人",
                    type: 3,
                    single: true,
                    callback: function (items) {
                        if (items.length == 1) {
                            Global.post("/Organization/UpdateUserParentID", {
                                ids: items[0].id,
                                parentid: "6666666666"
                            }, function (data) {
                                if (data.status) {
                                    location.href = location.href;
                                }
                            });
                        }
                    }
                });
            });
        }
        //添加下属
        $("#addChild").click(function () {
            var _this = $(this);
            ChooseUser.create({
                title: "添加下属",
                type: 3,
                single: false,
                callback: function (items) {
                    if (items.length > 0) {
                        var ids = "";
                        for (var i = 0; i < items.length; i++) {
                            ids += items[i].id + ",";
                        }
                        Global.post("/Organization/UpdateUserParentID", {
                            ids: ids,
                            parentid: _this.data("id")
                        }, function (data) {
                            if (data.status) {
                                location.href = location.href;
                            }
                        });
                    }
                }
            });
        });
        //移除下级
        $("#removeObject").click(function () {
            var _this = $(this);
            confirm("确认移除吗?", function () {
                Global.post("/Organization/ClearUserParentID", {
                    ids: _this.data("id"),
                    parentid: ""
                }, function (data) {
                    if (data.status) {
                        location.href = location.href;
                    }
                });
            });
        });
        //替换人员
        $("#changeObject").click(function () {
            var _this = $(this);
            ChooseUser.create({
                title: "替换人员",
                type: 3,
                single: true,
                callback: function (items) {
                    if (items.length == 1) {
                        Global.post("/Organization/ChangeUsersParentID", {
                            userid: items[0].id,
                            olduserid: _this.data("id")
                        }, function (data) {
                            if (data.status) {
                                location.href = location.href;
                            }
                        });
                    }
                }
            });
        });
    }
    ObjectJS.bindElement = function (element) {
        var _self = this;
        element.find(".ico-dropdown").click(function () {
            var _this = $(this);
            var offset = _this.offset();
            $(".dropdown-ul li").data("id", _this.data("id"));
            $(".dropdown-ul").css({ "top": offset.top + 20, "left": offset.left }).show().mouseleave(function () {
                $(this).hide();
            });
        });
    }
    //绑定下级
    ObjectJS.bindChild = function (items) {
        var _self = this;
        for (var i = 0, j = items.length; i < j; i++) {
            var menu = items[i];
            CacheChild[menu.UserID] = menu.ChildUsers;
        }
        doT.exec("template/organization/structure.html", function (template) {
            var innerHtml = template(items);
            innerHtml = $(innerHtml);
            $("#structure").append(innerHtml);

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
        doT.exec("template/organization/structureitem.html", function (template) {

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

                    //加载显示下属图标和缓存数据
                    if (CacheChild[pid][i].ChildUsers.length > 0) {
                        _item.append("<span data-id='" + CacheChild[pid][i].UserID + "' data-eq='last' data-state='close' class='icoopen openchild left'></span>");
                        if (!CacheChild[CacheChild[pid][i].UserID]) {
                            CacheChild[CacheChild[pid][i].UserID] = CacheChild[pid][i].ChildUsers;
                        }
                    }
                } else {
                    _item.append("<span class='leftline left'></span>");

                    //加载显示下属图标和缓存数据
                    if (CacheChild[pid][i].ChildUsers.length > 0) {
                        _item.append("<span data-id='" + CacheChild[pid][i].UserID + "' data-eq='' data-state='close' class='icoopen openchild left'></span>");
                        if (!CacheChild[CacheChild[pid][i].UserID]) {
                            CacheChild[CacheChild[pid][i].UserID] = CacheChild[pid][i].ChildUsers;
                        }
                    }
                }

                var innerTtml = template(CacheChild[pid][i]);
                _item.append(innerTtml);

                //_item.append("<div class='user-item left'><input type='checkbox' class='left'  value='" + CacheChild[pid][i].UserID + "' data-id='" + CacheChild[pid][i].UserID + "' /><span>" + CacheChild[pid][i].Name + "</span></div>");

                _div.append(_item);

                //绑定事件
                _self.bindElement(_item);

                //展开
                _item.find(".openchild").each(function () {
                    var _this = $(this);
                    var _obj = _self.getChild(_this.attr("data-id"), _leftBg.html(), _this.attr("data-eq"), items);
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
            }
        });
        return _div;
    }

    module.exports = ObjectJS;
});