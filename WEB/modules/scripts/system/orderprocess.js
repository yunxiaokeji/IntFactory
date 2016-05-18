define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Verify = require("verify"), VerifyObject,
        ChooseUser = require("chooseuser"),
        Easydialog = require("easydialog");
    require("switch");
    var Params = {
        Type: -1
    };

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function () {
        var _self = this;
        _self.bindEvent();
        _self.getList();
    }

    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;
        $(document).click(function (e) {
            //隐藏下拉
            if (!$(e.target).parents().hasClass("dropdown-ul") && !$(e.target).parents().hasClass("dropdown") && !$(e.target).hasClass("dropdown")) {
                $(".dropdown-ul").hide();
            }
        });

        $(".search-status li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                Params.Type = _this.data("id");
                _self.getList();
            }
        });
        //添加
        $("#createModel").click(function () {
            var _this = $(this);
            _self.createModel();
        });
        //删除
        $("#deleteObject").click(function () {
            var _this = $(this);
            confirm("阶段流程删除后不可恢复,确认删除吗？",function(){
                _self.deleteModel(_this.data("id"), function (status) {
                    if (status) {
                        _self.getList();
                    } else {
                        alert("系统异常，请稍后重试!");
                    }
                });
            });
        });
        //编辑
        $("#updateObject").click(function () {
            var _this = $(this);
            Global.post("/System/GetOrderProcessByID", { id: _this.data("id") }, function (data) {
                var model = data.model;
                _self.createModel(model);
            });
        });

        $("#setStages").click(function () {
            var _this = $(this);
            location.href = "/System/OrderStages/" +_this.data("id");
        });

        //转移拥有者
        $("#updateOwner").click(function () {
            var _this = $(this);
            ChooseUser.create({
                title: "更换负责人",
                type: 1,
                single: true,
                callback: function (items) {
                    if (items.length > 0) {
                        Global.post("/System/UpdateOrderProcessOwner", {
                            userid: items[0].id,
                            id: _this.data("id")
                        }, function (data) {
                            if (data.status) {
                                _self.getList();
                            }
                        });
                    }
                }
            });
        });

        $("#updateDefault").click(function () {
            var _this = $(this);
            Global.post("/System/UpdateOrderProcessDefault", { id: _this.data("id") }, function (data) {
                if (data.status) {
                    _self.getList();
                } else {
                    alert("系统异常，请稍后重试!");
                }
            })
        });

    }
    //添加/编辑弹出层
    ObjectJS.createModel = function (model) {
        var _self = this;

        doT.exec("template/system/orderprocess-detail.html", function (template) {
            var html = template([]);
            Easydialog.open({
                container: {
                    id: "show-model-detail",
                    header: !model ? "新建阶段流程" : "编辑阶段流程",
                    content: html,
                    yesFn: function () {
                        if (!VerifyObject.isPass()) {
                            return false;
                        }

                        var entity = {};
                        entity.ProcessID = model ? model.ProcessID : "";
                        entity.ProcessName = $("#processName").val().trim();
                        entity.ProcessType = $("#processType").find(".hover").val();
                        entity.CategoryType = $("#categoryType").find(".hover").val();
                        entity.PlanDays = 0;//$("#planDays").val().trim();
                        entity.IsDefault = 0;
                        _self.saveModel(entity);
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

            if (model && model.ProcessID) {
                $(".ico-radiobox").removeClass("hover");
                $("#processType").find(".ico-radiobox[data-value='" + model.ProcessType + "']").addClass("hover");
                $("#categoryType").find(".ico-radiobox[data-value='" + model.CategoryType + "']").addClass("hover");
            } else {
                $(".radiobox").click(function () {
                    var _this = $(this);
                    if (_this.find("hover").length == 0) {
                        _this.find(".ico-radiobox").addClass("hover");
                        _this.siblings().find(".ico-radiobox").removeClass("hover");
                    }
                });
            }

            $("#processName").focus();
            $("#processName").val(model.ProcessName);
            $("#planDays").val(model.PlanDays);

        }); 
    }
    //获取列表
    ObjectJS.getList = function () {
        var _self = this;
        $(".tr-header").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='6'><div class='data-loading'><div></td></tr>");
        Global.post("/System/GetOrderProcess", { type: Params.Type }, function (data) {
            _self.bindList(data.items);
        });
    }
    //加载列表
    ObjectJS.bindList = function (items) {
        var _self = this;
        $(".tr-header").nextAll().remove();

        if (items.length > 0) {
            doT.exec("template/system/orderprocess.html", function (template) {
                var innerhtml = template(items);
                innerhtml = $(innerhtml);

                //下拉事件
                innerhtml.find(".dropdown").click(function () {
                    var _this = $(this);
                    if (_this.data("type") == 1) {
                        $("#deleteObject").hide();
                        $("#updateDefault").hide();
                    } else {
                        $("#deleteObject").show();
                        $("#updateDefault").show();
                    }
                    var position = _this.find(".ico-dropdown").position();
                    $(".dropdown-ul li").data("id", _this.data("id"));
                    $(".dropdown-ul").css({ "top": position.top + 20, "left": position.left - 55 }).show().mouseleave(function () {
                        $(this).hide();
                    });
                });

                //绑定启用插件
                innerhtml.find(".status").switch({
                    open_title: "点击启用",
                    close_title: "点击禁用",
                    value_key: "value",
                    change: function (data, callback) {
                        _self.editIsChoose(data, data.data("id"), data.data("value"), callback);
                    }
                });

                $(".tr-header").after(innerhtml);
            });
        }
        else {
            $(".tr-header").after("<tr><td colspan='6'><div class='nodata-txt' >暂无数据!<div></td></tr>");
        }
    }

    //保存实体
    ObjectJS.saveModel = function (model) {
        var _self = this;
        Global.post("/System/SaveOrderProcess", { entity: JSON.stringify(model) }, function (data) {
            if (data.model && data.model.ProcessID) {
                _self.getList();
            } else {
                alert("网络异常，请稍后重试!");
            }
        })
    }
    //删除
    ObjectJS.deleteModel = function (id, callback) {
        Global.post("/System/DeleteOrderProcess", { id: id }, function (data) {
            !!callback && callback(data.status);
        })
    }

    module.exports = ObjectJS;
});