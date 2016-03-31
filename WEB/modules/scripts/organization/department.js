define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Verify = require("verify"), VerifyObject,
        Easydialog = require("easydialog");

    var Model = {};

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
            Model.DepartID = "";
            Model.Name = "";
            Model.Description = "";
            _self.createModel();
        });
        //删除
        $("#deleteObject").click(function () {
            var _this = $(this);
            confirm("部门删除后不可恢复,确认删除吗？",function(){
                _self.deleteModel(_this.data("id"), function (status) {
                    if (status == 1) {
                        _self.getList();
                    } else if (status == 10002) {
                        alert("此部门存在员工，请移除员工后重新操作！");
                    }
                });
            });
        });
        //编辑
        $("#updateObject").click(function () {
            var _this = $(this);
            Global.post("/Organization/GetDepartmentByID", { id: _this.data("id") }, function (data) {
                var model = data.model;
                Model.DepartID = model.DepartID;
                Model.Name = model.Name;
                Model.Description = model.Description;
                _self.createModel();
            });
        });

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
    //获取列表
    ObjectJS.getList = function () {
        var _self = this;
        $(".tr-header").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='5'><div class='data-loading'><div></td></tr>");
        Global.post("/Organization/GetDepartments", {}, function (data) {
            _self.bindList(data.items);
        });
    }
    //加载列表
    ObjectJS.bindList = function (items) {
        var _self = this;
        $(".tr-header").nextAll().remove();

        if (items.length > 0) {
            doT.exec("template/organization/departments.html", function (template) {
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

                });

                $(".tr-header").after(innerhtml);
            });
        }
        else {
            $(".tr-header").after("<tr><td colspan='5'><div class='nodata-txt' >暂无数据!<div></td></tr>");
        }
    }

    //保存实体
    ObjectJS.saveModel = function (model) {
        var _self = this;
        Global.post("/Organization/SaveDepartment", { entity: JSON.stringify(model) }, function (data) {
            if (data.model.DepartID.length > 0) {
                _self.getList();
                //_self.bindList([data.model]);
            }
        })
    }
    //删除
    ObjectJS.deleteModel = function (id, callback) {
        Global.post("/Organization/DeleteDepartment", { departid: id }, function (data) {
            !!callback && callback(data.status);
        })
    }

    module.exports = ObjectJS;
});