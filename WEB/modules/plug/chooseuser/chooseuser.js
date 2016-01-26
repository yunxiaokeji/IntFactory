
/*
    --选择用户插件--
    --引用
    chooseuser = require("chooseuser");
    chooseuser.create({});
*/
define(function (require, exports, module) {
    var $ = require("jquery"),
        Global = require("global"),
        doT = require("dot"),
        Easydialog = require("easydialog");

    require("plug/chooseuser/style.css");

    var PlugJS = function (options) {
        var _this = this;
        _this.setting = $.extend([], _this.default, options);
        _this.init();
    }

    //默认参数
    PlugJS.prototype.default = {
        title:"选择员工", //标题
        type: 1,  //类型 1：云销用户选择 2：明道用户导入 3：组织架构设置 4：销售团队成员
        single: false,
        callback: null   //回调
    };

    PlugJS.prototype.init = function () {
        var _self = this, url = "", templateUrl = "/plug/chooseuser/chooseuser.html";
        if (_self.setting.type == 1) {
            url = "/Organization/GetUserAll";
        } else if (_self.setting.type == 2) {
            url = "/Organization/GetMDUsers";
            templateUrl = "/plug/chooseuser/choosemduser.html";
        } else if (_self.setting.type == 3) {
            url = "/Organization/GetUsersByParentID";
        } else if (_self.setting.type == 4) {
            url = "/Organization/GetUserNoTeam";
        }
        Global.post(url, {}, function (data) {
            //从明道获取数据失败
            if (!data.status && _self.setting.type == 2) {
                alert("您还不是明道用户，不能从明道导入用户！");
                return;
            }
            doT.exec(templateUrl, function (template) {
                var innerHtml = template(data.items);

                Easydialog.open({
                    container: {
                        id: "choose-user-add",
                        header: _self.setting.title,
                        content: innerHtml,
                        yesFn: function () {
                            var list = [];
                            $("#userlistChoose li").each(function () {
                                var _this = $(this);
                                var model = {
                                    id: _this.data("id"),
                                    name: _this.find(".name").html(),
                                    avatar: _this.find("img").attr("src")
                                };
                                list.push(model);
                            })
                            _self.setting.callback && _self.setting.callback(list);
                        },
                        callback: function () {

                        }
                    }
                });
                //绑定事件
                _self.bindEvent();
            });
        });
        
    };
    //绑定事件
    PlugJS.prototype.bindEvent = function () {
        var _self = this;
        //搜索
        require.async("search", function () {
            $("#chooseuserSearch").searchKeys(function (keyWords) {
                _self.keywords = keyWords;
                if (_self.keywords) {
                    $(".userlist-items li").hide();
                    $(".userlist-items li[data-search*=" + _self.keywords.toLowerCase() + "]").show();
                    
                } else {
                    $(".userlist-items li").show();
                }
                var key = $("#letterfilter .hover").html();
                if (key != "全部") {
                    $(".userlist-items li[data-first!=" + key + "]").hide();
                }
            });
        });
        //字母筛选
        $("#letterfilter a").click(function () {
            var _this = $(this);
            _this.addClass("hover");
            _this.siblings().removeClass("hover");

            if (_self.keywords) {
                $(".userlist-items li").hide();
                $(".userlist-items li[data-search*=" + _self.keywords.toLowerCase() + "]").show();

            } else {
                $(".userlist-items li").show();
            }
            if (_this.html() != "全部") {
                $(".userlist-items li[data-first!=" + _this.html() + "]").hide();
            }
        });
        //选中成员
        $(".useradd").click(function () {
            
            if (_self.setting.single && $("#userlistChoose li").length > 0) {
                return;
            }

            var id = $(this).data("id");

            if ($("#userlistChoose li[data-id=" + id + "]").length > 0) {
                alert("此用户已在选中列表中！")
            } else {

                var ele = $(this).parent().clone();

                ele.find(".mobile").remove();
                ele.find("a").html("移除").click(function () {
                    ele.remove();
                });
                $("#userlistChoose").append(ele);
            }
        });
        //清空选中
        $("#clearChoose").click(function () {
            $("#userlistChoose").empty();
        });
    }
    exports.create = function (options) {
        return new PlugJS(options);
    }
});