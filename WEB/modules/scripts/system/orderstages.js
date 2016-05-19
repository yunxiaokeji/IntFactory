define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        ChooseUser = require("chooseuser"),
        Easydialog = require("easydialog");
    var Model = {};

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function (processid, processType, categoryType) {
        var _self = this;
        _self.bindEvent();
        _self.processid = processid;
        _self.processType = processType;
        _self.categoryType = categoryType;
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
            _self.showLayer("", "", "", $(this).data("sort") * 1 + 1);
        });

        //删除阶段
        $("#deleteObject").click(function () {
            var _this = $(this);
            confirm("阶段状态删除后不可恢复,确认删除吗？", function () {
                _self.deleteModel(_this.data("id"), function (status) {
                    if (status) {
                        location.href = location.href;
                    } 
                });
            });
        });

        //编辑阶段
        $("#editObject").click(function () {
            var _this = $(this);
            _self.showLayer(_this.data("id"), $("#" + _this.data("id")).html(), _this.data("mark"), _this.data("sort"));
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
                    stageid: _this.data("stageid"),
                    processid: _self.processid
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

    //添加、编辑浮层
    ObjectJS.showLayer = function (id, name, mark, sort) {
        var _self = this;
        doT.exec("template/system/orderstage-detail.html", function (template) {
            var innerText = template({ CategoryType: _self.categoryType, ProcessType: _self.processType });
            Easydialog.open({
                container: {
                    id: "addOrderStage",
                    header: id ? "编辑阶段" : "添加阶段",
                    content: innerText,
                    yesFn: function () {
                        if (!$("#iptStageName").val().trim()) {
                            alert("阶段名称不能为空");
                            return false;
                        }
                        var model = {
                            StageID: id,
                            StageName:$("#iptStageName").val().trim(),
                            Sort: sort,
                            Mark: $("#addOrderStage .stage-item.hover").data("id") || 0
                        };
                        _self.saveModel(model);
                    },
                    callback: function () {

                    }
                }
            });

            if (id) {
                $("#iptStageName").val(name);
                $("#addOrderStage .stage-item[data-id='" + mark + "']").addClass("hover");
            }

            $("#addOrderStage .stage-item").click(function () {
                var _this=$(this);
                if (_this.hasClass("hover")) {
                    _this.removeClass("hover");
                } else {
                    _this.siblings().removeClass("hover");
                    _this.addClass("hover");
                }
            });
        });
    }

    //元素绑定事件
    ObjectJS.bindElement = function (items) {
        var _self = this;
        //下拉事件
        items.find(".operatestage").click(function () {
            var _this = $(this);
            //if (_this.data("type") != 0) {
            //    $("#deleteObject").hide();
            //} else {
            //    $("#deleteObject").show();
            //}
            var offset = _this.offset();
            $("#ddlStage li").data("id", _this.data("id")).data("sort", _this.data("sort")).data("mark", _this.data("type"));
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
                StageID: _this.data("stageid"),
                ProcessID: _self.processid
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
                location.href = location.href;
            } else {
                alert("网络异常，请稍后重试");
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