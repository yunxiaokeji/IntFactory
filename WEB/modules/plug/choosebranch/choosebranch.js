
/* 
作者：Allen
日期：2015-11-6
示例:
    $(...).chooseBranch(options);
*/

define(function (require, exports, module) {
    require("plug/choosebranch/style.css");
    var Global = require("global"),
        doT = require("dot");
    require("search");
    (function ($) {
        $.fn.chooseBranch = function (options) {
            var opts = $.extend({}, $.fn.chooseBranch.defaults, options);
            return this.each(function () {
                var _this = $(this);
                $.fn.drawChooseBranch(_this, opts);
            })
        }
        $.fn.chooseBranch.defaults = {
            prevText: "",//文本前缀
            defaultText: "",
            defaultValue: "",
            userid: "",
            agentid: "",
            isTeam: false,
            width: "180",
            onChange: function () { }
        };
        $.fn.drawChooseBranch = function (obj, opts) {

            obj.data("itemid", "choosebranch" + opts.agentid + opts.userid);

            if (!obj.hasClass("choosebranch-module")) {
                obj.addClass("choosebranch-module").css("width", opts.width);
            }
            var _input = $('<div class="choosebranch-text">' + opts.prevText + opts.defaultText + '</div>');
            _input.css("width", opts.width - 30);
            var _ico = $('<div class="choosebranch-ico"><span></span></div>');
            obj.append(_input).append(_ico);

            //处理事件
            obj.click(function () {
                var _this = $(this);
                if (_this.hasClass("hover")) {
                    $("#" + obj.data("itemid")).hide();
                    _this.removeClass("hover");
                } else {
                    $.fn.drawChooseBranchItems(obj, opts);
                    _this.addClass("hover");
                }
            });

            $(document).click(function (e) {
                //隐藏下拉
                var bl = false;
                $(e.target).parents().each(function () {
                    var _this = $(this);
                    if (_this.data("itemid") == obj.data("itemid") || _this.attr("id") == obj.data("itemid")) {
                        bl = true;
                    }
                });
                if (!bl) {
                    obj.removeClass("hover");
                    $("#" + obj.data("itemid")).hide();
                }
            });
        }
        $.fn.drawChooseBranchItems = function (obj, opts) {
            var cacheChild = [];
            var offset = obj.offset();
            if ($("#" + obj.data("itemid")).length == 1) {
                $("#" + obj.data("itemid")).css({ "top": offset.top + 27, "left": offset.left }).show();
            } else {
                var _branch = $("<div style='min-width:" + opts.width + "px;' class='choosebranch-items-modules' id='" + obj.data("itemid") + "'></div>");

                var _search = $("<div data-width='" + (opts.width - 37) + "' class='search-branch'></div>");
                
                _branch.append(_search);

                if (opts.defaultText) {
                    _branch.append("<div class='default-item change-user' data-id='" + opts.defaultValue + "'>" + opts.defaultText + "</div>");
                }

                var _items = $("<div class='choosebranch-items'></div>");

                Global.post("/Plug/GetUserBranchs", {
                    userid: opts.userid,
                    agentid: opts.agentid
                }, function (data) {
                    for (var i = 0, j = data.items.length; i < j; i++) {
                        var user = data.items[i];
                        cacheChild[user.UserID] = user.ChildUsers;
                    }
                    doT.exec("plug/choosebranch/users.html", function (template) {
                        var innerHtml = template(data.items);
                        innerHtml = $(innerHtml);

                        _items.append(innerHtml);

                        //展开
                        innerHtml.find(".openchild").each(function () {
                            var _this = $(this);
                            var _obj = $.fn.drawChooseBranchChild(_this.attr("data-id"), _this.prevUntil("div").html(), _this.attr("data-eq"), cacheChild);
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

                        _branch.append(_items);

                        if (opts.isTeam) {
                            $.fn.drawChooseAllTeams(obj, _branch, opts);
                        }

                        _branch.find(".change-user").click(function () {
                            obj.find(".choosebranch-text").html(opts.prevText + $(this).html());
                            obj.data("id", $(this).data("id"));
                            obj.removeClass("hover");
                            $("#" + obj.data("itemid")).hide();
                            opts.onChange({
                                teamid: "",
                                userid: $(this).data("id"),
                                name: $(this).html()
                            });
                        });
                        _search.searchKeys(function (keyWords) {
                            var _ele = _items.find(".change-user[data-search*='" + keyWords + "']").first();
                            _branch.find(".change-user").css("color", "#333");
                            _ele.parents().prev().each(function () {
                                if ($(this).hasClass("branchitem")) {
                                    $(this).find(".openchild[data-state='close']").first().click();
                                }
                            })
                            _ele.css("color", "#4a98e7");
                        });

                    });
                    _branch.css({ "top": offset.top + 27, "left": offset.left });
                   
                    obj.after(_branch);

                   
                });
            }
        }
        $.fn.drawChooseAllTeams = function (obj, ele, opts) {
            var _teams = $(document.createElement("ul")).addClass("choosebranch-teams");
            Global.post("/Plug/GetTeams", {
                agentid: opts.agentid
            }, function (data) {
                for (var i = 0, j = data.items.length; i < j; i++) {
                    var team = data.items[i];
                    _teams.append("<li data-id='" + team.TeamID + "'>" + team.TeamName + "</li>")
                }

                _teams.find("li").click(function () {
                    obj.find(".choosebranch-text").html(opts.prevText + $(this).html());
                    obj.data("id", $(this).data("id"));
                    obj.removeClass("hover");
                    $("#" + obj.data("itemid")).hide();
                    opts.onChange({
                        teamid: $(this).data("id"),
                        userid: "",
                        name: $(this).html()
                    });
                });
            });

            ele.append(_teams);
        }
        $.fn.drawChooseBranchChild = function (pid, provHtml, isLast, cacheChild) {
            var _div = $(document.createElement("div")).attr("id", pid).addClass("hide").addClass("childbox");
            for (var i = 0; i < cacheChild[pid].length; i++) {
                var _item = $(document.createElement("div")).addClass("branchitem");

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
                if (i == cacheChild[pid].length - 1) {
                    _item.append("<span class='lastline left'></span>");

                    //加载显示下属图标和缓存数据
                    if (cacheChild[pid][i].ChildUsers.length > 0) {
                        _item.append("<span data-id='" + cacheChild[pid][i].UserID + "' data-eq='last' data-state='close' class='icoopen openchild left'></span>");
                        if (!cacheChild[cacheChild[pid][i].UserID]) {
                            cacheChild[cacheChild[pid][i].UserID] = cacheChild[pid][i].ChildUsers;
                        }
                    }
                } else {
                    _item.append("<span class='leftline left'></span>");

                    //加载显示下属图标和缓存数据
                    if (cacheChild[pid][i].ChildUsers.length > 0) {
                        _item.append("<span data-id='" + cacheChild[pid][i].UserID + "' data-eq='' data-state='close' class='icoopen openchild left'></span>");
                        if (!cacheChild[cacheChild[pid][i].UserID]) {
                            cacheChild[cacheChild[pid][i].UserID] = cacheChild[pid][i].ChildUsers;
                        }
                    }
                }

                _item.append("<span data-id='" + cacheChild[pid][i].UserID + "' data-search='" + cacheChild[pid][i].Name + "' class='left name change-user'>" + cacheChild[pid][i].Name + "</span>")

                _div.append(_item);

                //默认加载下级
                _item.find(".openchild").each(function () {
                    var _this = $(this);
                    var _obj = $.fn.drawChooseBranchChild(_this.attr("data-id"), _leftBg.html(), _this.attr("data-eq"), cacheChild);
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
            return _div;
        }
    })(jQuery)
    module.exports = jQuery;
});