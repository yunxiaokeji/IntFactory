define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Verify = require("verify"), VerifyObject,
        Easydialog = require("easydialog");

    var Model = {},
        cacheMenu = [];

    var Paras = {
        keyWords:"",
        pageIndex: 1
    }
    
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

        $("#createModel").click(function () {
            var _this = $(this);
            Model.RoleID = "";
            Model.Name = "";
            Model.Description = "";
            _self.createModel();
        });
        //删除
        $("#deleteObject").click(function () {
            var _self = $(this);
            if(confirm("角色删除后不可恢复,确认删除吗？"))
            {
                alert(111);
                //_self.deleteModel(_this.data("id"), function (status) {
                //    if (status == 1) {
                //        _self.getList();
                //    } else if (status == 10002) {
                //        alert("此角色存在员工，请移除员工后重新操作！");
                //    }
                //});
            }
        });
        //编辑
        $("#updateObject").click(function () {
            var _this = $(this);
            Global.post("/System/GetUserDetail", { id: _this.data("id") }, function (data) {
                var model = data.Item;
                //Model.RoleID = model.RoleID;
                Model.Name = model.Name;
                Model.Description = model.Description;
                _self.createModel();
            });
        });
    }
    //添加/编辑弹出层
    ObjectJS.createModel = function () {
        var _self = this;

        doT.exec("template/System/role-detail.html", function (template) {
            var html = template([]);
            Easydialog.open({
                container: {
                    id: "show-model-detail",
                    header: !Model.RoleID ? "新建角色" : "编辑角色",
                    content: html,
                    yesFn: function () {
                        if (!VerifyObject.isPass()) {
                            return false;
                        }
                        Model.Name = $("#modelName").val();
                        Model.Description = $("#modelDescription").val();
                        //Model.ParentID = "";
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
        $(".tr-header").after("<tr><td colspan='6'><div class='dataLoading'><img src='/modules/images/ico-loading.jpg'/><div></td></tr>");
        Global.post("/System/GetUsers", Paras, function (data) {
            _self.bindList(data.Items);
        });
    }

    //加载列表
    ObjectJS.bindList = function (items) {
        var _self = this;
        $(".tr-header").nextAll().remove();

        if (items.length > 0) {
            doT.exec("template/System/users.html", function (template) {
                var innerhtml = template(items);
                innerhtml = $(innerhtml);

                //操作
                innerhtml.find(".dropdown").click(function () {
                    var _this = $(this);
                    if (_this.data("type") != 1) {
                        var position = _this.find(".ico-dropdown").position();
                        $(".dropdown-ul li").data("id", _this.data("id"));
                        $(".dropdown-ul").css({ "top": position.top + 20, "left": position.left - 55 }).show().mouseleave(function () {
                            $(this).hide();
                        });
                    }
                });

                $(".tr-header").after(innerhtml);
            });
        }
        else {
            $(".tr-header").after("<tr><td colspan='6'><div class='noDataTxt' >暂无数据!<div></td></tr>");
        }
    }

    //保存实体
    ObjectJS.saveModel = function (model) {
        var _self = this;
        Global.post("/System/SaveRole", { entity: JSON.stringify(model) }, function (data) {
            if (data.model.RoleID.length > 0) {
                _self.getList();
                //_self.bindList([data.model]);
            }
        })
    }
    //删除
    ObjectJS.deleteModel = function (id, callback) {
        Global.post("/System/DeleteRole", { roleid: id }, function (data) {
            !!callback && callback(data.status);
        })
    }


    module.exports = ObjectJS;
});