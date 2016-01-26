define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Verify = require("verify"), VerifyObject,
        Easydialog = require("easydialog");

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
        //添加
        $("#createModel").click(function () {
            var _this = $(this);
            _self.createModel();
        });
        //删除
        $("#deleteObject").click(function () {
            var _this = $(this);
            confirm("机会阶段删除后不可恢复,确认删除吗？",function(){
                _self.deleteModel(_this.data("id"), function (status) {
                    if (status == 1) {
                        _self.getList();
                    } else {
                        alert("该阶段存在机会订单，删除失败！");
                    }
                });
            });
        });
        //编辑
        $("#updateObject").click(function () {
            var _this = $(this);
            Global.post("/System/GetOpportunityStageByID", { id: _this.data("id") }, function (data) {
                var model = data.model;
                _self.createModel(model);
            });
        });

    }
    //添加/编辑弹出层
    ObjectJS.createModel = function (model) {
        var _self = this;

        doT.exec("template/system/opportunitystage-detail.html", function (template) {
            var html = template([]);
            Easydialog.open({
                container: {
                    id: "show-model-detail",
                    header: !model ? "新建机会阶段" : "编辑机会阶段",
                    content: html,
                    yesFn: function () {
                        if (!VerifyObject.isPass()) {
                            return false;
                        }
                        var entity = {
                            StageID: model ? model.StageID : "",
                            StageName: $("#stagename").val().trim(),
                            Probability: $("#probability").val().trim() / 100
                        }
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
            $("#stagename").focus();
            if (model) {
                if (model.Mark == 2) {
                    $("#probability").prop("disabled", true);
                } else {
                    $("#probability").prop("disabled", false);
                }
                $("#stagename").val(model.StageName);
                $("#probability").val((model.Probability * 100).toFixed(2));
            }

        }); 
    }
    //获取列表
    ObjectJS.getList = function () {
        var _self = this;
        $(".tr-header").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='5'><div class='dataLoading'><img src='/modules/images/ico-loading.jpg'/><div></td></tr>");
        Global.post("/System/GetOpportunityStages", {}, function (data) {
            _self.bindList(data.items);
        });
    }
    //加载列表
    ObjectJS.bindList = function (items) {
        var _self = this;
        $(".tr-header").nextAll().remove();

        if (items.length > 0) {
            doT.exec("template/system/opportunitystages.html", function (template) {

                var innerhtml = template(items);
                innerhtml = $(innerhtml);

                //下拉事件
                innerhtml.find(".dropdown").click(function () {
                    var _this = $(this);

                    var position = _this.find(".ico-dropdown").position();
                    $(".dropdown-ul li").data("id", _this.data("id"));
                    $(".dropdown-ul").css({ "top": position.top + 20, "left": position.left - 55 }).show().mouseleave(function () {
                        $(this).hide();
                    });

                    if (_this.data("mark") == 0) {
                        $("#deleteObject").show();
                    } else {
                        $("#deleteObject").hide();
                    }

                });

                $(".tr-header").after(innerhtml);
            });
        }
        else {
            $(".tr-header").after("<tr><td colspan='5'><div class='noDataTxt' >暂无数据!<div></td></tr>");
        }
    }

    //保存实体
    ObjectJS.saveModel = function (model) {
        var _self = this;
        Global.post("/System/SaveOpportunityStage", { entity: JSON.stringify(model) }, function (data) {
            if (data.model.StageID) {
                _self.getList();
            }
        })
    }
    //删除
    ObjectJS.deleteModel = function (id, callback) {
        Global.post("/System/DeleteOpportunityStage", { id: id }, function (data) {
            !!callback && callback(data.status);
        });
    }

    module.exports = ObjectJS;
});