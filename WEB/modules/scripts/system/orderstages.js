define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        ChooseUser = require("chooseuser"),
        Easydialog = require("easydialog");
    var Model = {};

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function (processid) {
        var _self = this;
        _self.bindEvent();
        _self.processid = processid;
        _self.bindElement($(".stages-item"));
        _self.bingStyle();
    }

    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;

        $(window).resize(function () {
            _self.bingStyle();
        });

        $(document).click(function (e) {
            //隐藏下拉
            if (!$(e.target).hasClass("operatestage")) {
                $("#ddlStage").hide();
            }

            if (!$(e.target).hasClass("operateitem")) {
                $("#ddlItem").hide();
            }
        });
        //添加新阶段
        $("#addObject").click(function () {

            var _this = $(this), input = $("#" + _this.data("id")), parent = input.parents(".stages-item").first();
            //复制并处理新对象
            var element = parent.clone();

            element.find(".name span").html("").hide();
            var _input = element.find(".name input");
            _input.attr("id", "").data("sort", _input.data("sort") + 1).show();
            
            element.find(".ico-dropdown").data("type", "0").data("id", "").hide();
            element.find(".child-items").empty();
            _self.bindElement(element);
            parent.after(element);
            _input.focus();
        });

        //删除阶段
        $("#deleteObject").click(function () {
            var _this = $(this);
            confirm("阶段状态删除后不可恢复,此阶段状态的订单自动回归到上个阶段状态,确认删除吗？", function () {
                _self.deleteModel(_this.data("id"), function (status) {
                    if (status) {
                        location.href = location.href;
                    } 
                });
            });
        });

        //编辑阶段名称
        $("#editObject").click(function () {

            var _this = $(this), input = $("#" + _this.data("id")), span = input.siblings("span");
            var input = $("#" + _this.data("id"));
            
            input.siblings().hide();
            input.parent().siblings(".ico-dropdown").hide();
            input.show();
            input.focus();

            input.val(span.html());
        });

        //转移拥有者
        $("#editOwner").click(function () {
            var _this = $(this);
            ChooseUser.create({
                title: "更换负责人",
                type: 1,
                single: true,
                callback: function (items) {
                    if (items.length > 0) {
                        Global.post("/System/UpdateOrderStageOwner", {
                            userid: items[0].id,
                            processid: _self.processid,
                            id: _this.data("id")
                        }, function (data) {
                            if (data.status) {
                                $("#owner" + _this.data("id")).text(items[0].name)
                            }
                        });
                    }
                }
            });
        });

        //编辑项目名称
        $("#editItem").click(function () {

            var _this = $(this), item = $("#" + _this.data("id"));

            item.hide();
            item.parent().next().show();

            item.parent().next().find("textarea").data("stageid", _this.data("stageid")).data("id", _this.data("id")).focus();

            item.parent().next().find("textarea").val(item.find(".itemname").html());
            
        });

        //删除阶段行为
        $("#deleteItem").click(function () {
            var _this = $(this);
            confirm("阶段行为删除后不可恢,确认删除吗？", function () {
                Global.post("/System/DeleteStageItem", {
                    id: _this.data("id"),
                    stageid: _this.data("stageid")
                }, function (data) {
                    if (data.status) {
                        $("#" + _this.data("id")).remove();
                    } else {
                        alert("系统异常!");
                    }
                })
            });
        });

    }
    //元素绑定事件
    ObjectJS.bindElement = function (items) {
        var _self = this;
        //下拉事件
        items.find(".operatestage").click(function () {
            var _this = $(this);
            if (_this.data("type") != 0) {
                $("#deleteObject").hide();
            } else {
                $("#deleteObject").show();
            }
            var offset = _this.offset();
            $("#ddlStage li").data("id", _this.data("id")).data("sort", _this.data("sort"));
            var left = offset.left;
            if (left > document.documentElement.clientWidth - 150) {
                left = left - 150;
            }
            $("#ddlStage").css({ "top": offset.top + 20, "left": left }).show().mouseleave(function () {
                $(this).hide();
            });
        });
        //阶段文本改变事件
        items.find(".name input").blur(function () {
            var _this = $(this), span = _this.siblings("span");
            if (_this.val() != span.html()) {
                var model = {
                    StageID: _this.attr("id"),
                    StageName: _this.val().trim(),
                    Sort: _this.data("sort")
                };
                _self.saveModel(model);
            } else {
                if (!_this.attr("id")) {
                    _this.parents(".stages-item").first().remove();
                } else {
                    span.html(_this.val()).show();
                    _this.val("").hide();
                    _this.parent().siblings(".ico-dropdown").show();
                }
            }
        });

        //添加行为项
        items.find(".create-child").click(function () {
            var _this = $(this);
            _this.prev().show();
            _this.prev().find("textarea").data("stageid", _this.data("id")).data("id", "").val("").focus();
        });

        //行为文本改变事件
        items.find(".create-action textarea").blur(function () {
            var _this = $(this);
            if (!_this.val().trim()) {
                _this.parent().hide();
                return;
            }
            if (_this.data("id") && _this.val().trim() == $("#" + _this.data("id")).find(".itemname").html().trim()) {
                _this.parent().hide();
                $("#" + _this.data("id")).show();
                return;
            }
            var model = {
                ItemID: _this.data("id"),
                ItemName: _this.val().trim(),
                StageID: _this.data("stageid")
            };
            Global.post("/System/SaveStageItem", { entity: JSON.stringify(model) }, function (data) {
                if (data.model.ItemID) {
                    if (model.ItemID) {
                        $("#" + _this.data("id")).find(".itemname").html(model.ItemName);
                        $("#" + _this.data("id")).show();
                    } else {
                        var ele = $('<li id="' + data.model.ItemID + '">' +
                                        '<span class="itemname width200 long">' + model.ItemName + '</span>' +
                                        '<span data-id="' + data.model.ItemID + '" data-stageid="' + model.StageID + '" class="ico-dropdown operateitem"></span>' +
                                    '</li> ');
                        _this.parent().prev(".child-items").append(ele);

                        _self.bindElement(ele);
                    }
                    _this.parent().hide();
                } else {
                    alert("系统异常!");
                }
            });

        });

        //行为项下拉事件
        items.find(".operateitem").click(function () {
            var _this = $(this);
            var offset = _this.offset();
            $("#ddlItem li").data("id", _this.data("id")).data("stageid", _this.data("stageid"));
            var left = offset.left;
            if (left > document.documentElement.clientWidth - 150) {
                left = left - 150;
            }
            $("#ddlItem").css({ "top": offset.top + 20, "left": left }).show().mouseleave(function () {
                $(this).hide();
            });
        });
    }
    //保存阶段实体
    ObjectJS.saveModel = function (model) {
        var _self = this;
        model.ProcessID = _self.processid;
        Global.post("/System/SaveOrderStage", { entity: JSON.stringify(model) }, function (data) {
            if (data.status == 1) {
                if (model.StageID) {
                    var _this = $("#" + model.StageID), span = _this.siblings("span");
                    span.html(_this.val()).show();
                    _this.val("").hide();
                    _this.parent().siblings(".operatestage").show();
                } else {
                    location.href = location.href;
                }
            } else {
                alert("系统异常!");
            }
        });
    }
    //删除
    ObjectJS.deleteModel = function (id, callback) {
        var _self = this;
        Global.post("/System/DeleteOrderStage", { id: id, processid: _self.processid }, function (data) {
            !!callback && callback(data.status);
        })
    }

    //高度控制
    ObjectJS.bingStyle = function () {
        var height = document.documentElement.clientHeight;
        $(".child-items").css("max-height", height - 330);
        $(".stages-box").css("height", height - 230);
    }

    module.exports = ObjectJS;
});