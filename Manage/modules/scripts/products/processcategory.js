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
    }

    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;

        //下拉事件
        $(".dropdown").click(function () {
            var _this = $(this);

            var position = _this.find(".ico-dropdown").position();
            $(".dropdown-ul li").data("id", _this.data("id")).data("name", _this.data("name")).data("desc", _this.data("desc"));
            $(".dropdown-ul").css({ "top": position.top + 20, "left": position.left - 55 }).show().mouseleave(function () {
                $(this).hide();
            });

        });

        $("#addCategory").click(function () {
            _self.showCategory("", "", "");
        });

        //删除
        $("#deleteObject").click(function () {
            var _this = $(this);
            if(confirm("品类删除后不可恢复,确认删除吗？")){
                Global.post("/Products/DeleteProcessCategory", { categoryid: _this.data("id") }, function (data) {
                    if (data.status) {
                        location.href = location.href;
                    } else {
                        alert("该品类存在关联数据，不能删除");
                    }
                });
            }
        });

        //编辑
        $("#updateObject").click(function () {
            var _this = $(this);

            _self.showCategory(_this.data("id"), _this.data("name"), _this.data("desc"));
        });
    }

    ObjectJS.showCategory = function (id, name, remark) {
        var _self = this;
        doT.exec("template/products/category-process.html", function (templateFun) {

            var html = templateFun([]);

            Easydialog.open({
                container: {
                    id: "category-add-div",
                    header: id ? "添加品类" : "编辑品类",
                    content: html,
                    yesFn: function () {
                        if (!$("#categoryName").val().trim()) {
                            alert("名称不能为空");
                            return false;
                        }

                        Global.post("/Products/SaveProcessCategory", {
                            categoryid: id,
                            name: $("#categoryName").val().trim(),
                            desc: $("#description").val().trim()
                        }, function (data) {
                            if (data.ID.length > 0) {
                                if (id) {
                                    location.href = location.href;
                                } else {
                                    location.href = "/Products/CategoryItems/" + data.ID;
                                }
                            } else {
                                alert("保存失败，请刷新页面后重试");
                            }
                        });
                    },
                    callback: function () {

                    }
                }
            });
            $("#categoryName").focus();
            if (id) {
                $("#categoryName").val(name);
                $("#description").val(remark);
            }
        });
    }

    ObjectJS.initDetail = function (categoryid) {
        var _self = this;
        _self.categoryid = categoryid;

        $(document).click(function (e) {
            if (!$(e.target).hasClass("module-item") && !$(e.target).parents().hasClass("module-item")) {
                $(".dropdown-ul").hide();
            }
        });

        $(".module-item li").click(function () {
            var _this = $(this);

            var position = _this.find(".ico-dropdown").offset();
            $(".dropdown-ul li").data("id", _this.data("id")).data("name", _this.data("name")).data("sort", _this.data("sort")).data("remark", _this.data("remark"));
            $(".dropdown-ul").css({ "top": position.top + 20, "left": position.left }).show().mouseleave(function () {
                $(this).hide();
            });
        });

        //编辑
        $("#updateObject").click(function () {
            var _this = $(this);

            _self.showCategoryItems(_this.data("id"), _this.data("name"), _this.data("sort"), _this.data("remark"));
        });
    }

    ObjectJS.showCategoryItems = function (id, name, sort, remark) {
        var _self = this;
        doT.exec("template/products/category-items.html", function (templateFun) {

            var html = templateFun([]);

            Easydialog.open({
                container: {
                    id: "category-add-div",
                    header:  "编辑模块",
                    content: html,
                    yesFn: function () {

                        if (!VerifyObject.isPass()) {
                            return false;
                        };

                        Global.post("/Products/UpdateCategoryItem", {
                            categoryid: _self.categoryid,
                            itemid:id,
                            name: $("#name").val().trim(),
                            sort: $("#sort").val().trim(),
                            remark: $("#description").val().trim()
                        }, function (data) {
                            if (data.status) {
                                location.href = location.href;
                            } else {
                                alert("保存失败，请刷新页面后重试");
                            }
                        });
                    },
                    callback: function () {

                    }
                }
            });
            $("#name").focus();
            $("#name").val(name);
            $("#sort").val(sort);
            $("#description").val(remark);
            VerifyObject = Verify.createVerify({
                element: ".verify",
                emptyAttr: "data-empty",
                verifyType: "data-type",
                regText: "data-text"
            });
        });
    }

    module.exports = ObjectJS;
});