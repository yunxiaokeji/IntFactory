/*
*布局页JS
*/
define(function (require, exports, module) {
    var $ = require("jquery"),
        Global = require("global"),
        doT = require("dot"),
        Verify = require("verify"), VerifyObject, CategoryVerifyObject,
        AttrPlug = require("scripts/products/attrplug"),
        Easydialog = require("easydialog");
    var Category = {
        CategoryID: "",
        PID: ""
    }, Value = {
        AttrID: "",
        ValueID: "",
        ValueName: ""
    };
    var CacheAttrs = [];

    var ObjectJS = {};
    //初始化数据
    ObjectJS.init = function () {
        ObjectJS.cache();
        ObjectJS.bindStyle();
        ObjectJS.bindEvent();
    }
    //缓存数据
    ObjectJS.cache = function () {
        //获取所有属性
        Global.post("/Products/GetAttrsByCategoryID", {
            categoryid: ""
        }, function (data) {
            CacheAttrs = data.Items;
        });
    }
    //绑定元素定位和样式
    ObjectJS.bindStyle = function () {

        var _height = document.documentElement.clientHeight - 270;
        $(".category-all").css("height", _height);
        $(".category-all").css("width", $(".category-layer").length * 200 + 650);
        $(".category-list").css("max-height", _height-100);

        $("div.content-body").scrollLeft($("div.content-body").width());
    }
    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;
        //调整浏览器窗体
        $(window).resize(function () {
            ObjectJS.bindStyle();
        });

        $(document).click(function (e) {
            if (!$(e.target).hasClass("attritem") && !$(e.target).parents().hasClass("attritem") && !$(e.target).hasClass("attr-value-box") && !$(e.target).parents().hasClass("attr-value-box")) {
                _self.hideValues();
            }

            if (!$(e.target).hasClass("ddlcategory")) {
                $("#ddlCategory").hide();
            }

            if (!$(e.target).hasClass("ddlattr")) {
                $("#ddlAttr").hide();
            }
        });

        //添加属性值
        $(".ico-input-add").click(function () {
            if ($("#valueName").val()) {
                Value.ValueID = "";
                Value.ValueName = $("#valueName").val();
                _self.saveValue(function () {
                    $("#valueName").val("");
                });
            }
        });
        $("#valueName").keydown(function (e) {
            var _this = $(this);
            if (e.keyCode == 13) {
                $(".ico-input-add").click();
            }
        });

        //删除分类
        $("#deleteCategory").click(function () {
            var _this = $(this);
            if (confirm("分类删除后不可恢复,确认删除吗?")) {
                Global.post("/Products/DeleteCategory", { id: _this.data("id") }, function (data) {
                    if (data.status == 1) {
                        $(".category-list li[data-id='" + _this.data("id") + "']").remove();
                    } else if (data.status == 10002) {
                        alert("存在下级分类,不能删除,可以选择不启用!");
                    } else {
                        alert("删除失败!");
                    }
                });
            }
        });
        //编辑分类
        $("#editCategory").click(function () {
            var _this = $(this);
            
            Global.post("/Products/GetCategoryByID", {
                categoryid: _this.data("id")
            }, function (data) {
                Category = data.Model;
                _self.showCategory(function (model) {
                    var _old = $(".category-list li[data-id='" + _this.data("id") + "']").find(".category-name");
                    _old.html(model.CategoryName);
                    if (model.Status == 1) {
                        _old.removeClass("color999");
                    } else {
                        _old.addClass("color999");
                    }
                    _old.parent().attr("title", model.Description);
                }, Category.Layers);
            });
            return false;
        });

        //删除属性
        $("#deleteAttr").click(function () {
            var _this = $(this);
            if (confirm("删除后不可恢复,确认删除吗?")) {
                Global.post("/Products/DeleteCategoryAttr", {
                    categoryid: _this.data("categoryid"),
                    attrid: _this.data("id"),
                    type: _this.data("type")
                }, function (data) {
                    if (data.Status) {
                        $(".category-attr-list li[data-id='" + _this.data("id") + "']").remove();

                        $(".common-attr-list li[data-id='" + _this.data("id") + "']").find(".addcommon").show();

                    }
                });
            }
        });
        //编辑属性
        $("#editAttr").click(function () {
            var _this = $(this);

            AttrPlug.init({
                attrid: _this.data("id"),
                categoryid: "",
                callback: function (Attr) {
                    var _old = $(".category-attr-list li[data-id='" + _this.data("id") + "']");
                    _old.attr("title", Attr.Description);
                    _old.find(".category-attr-name").html(Attr.AttrName);
                }
            });

            return false;
        });

        _self.addBindEvent($(".create-child"));

        _self.bindElementEvent($(".category-list li"));
        
    }

    //添加分类绑定事件并处理回调
    ObjectJS.addBindEvent = function (ele) {
        var _self = this;
        ele.click(function () {
            var _this = $(this);
            Category.CategoryID = "";
            Category.PID = _this.data("id");
            _self.showCategory(function (model) {
                var ele = $('<li data-id="' + model.CategoryID + '" title="' + model.Description + '" data-layer="' + _this.data("layer") + '">' +
                                '<span class="category-name long width150 left">' + model.CategoryName + '</span>' +
                                '<span data-id="' + model.CategoryID + '" class="ddlcategory right">></span>' +
                            '</li>');
                _self.bindElementEvent(ele);
                _this.prev("ul").append(ele);
            }, _this.data("layer"));
        });
    }

    //添加分类弹出层
    ObjectJS.showCategory = function (callback, Layers) {
        var _self = this;
        doT.exec("template/products/category_add.html", function (templateFun) {

            var html= templateFun([]);
            
            Easydialog.open({
                container: {
                    id: "category-add-div",
                    header: Category.CategoryID == "" ? "添加分类" : "编辑分类",
                    content: html,
                    yesFn: function () {

                        if (!CategoryVerifyObject.isPass("#category-add-div")) {
                            return false;
                        }

                        var model = {
                            CategoryID: Category.CategoryID,
                            CategoryCode: "",
                            CategoryName: $("#categoryName").val(),
                            CategoryType: 1,
                            PID: Category.PID,
                            Status: $("#categoryStatus").prop("checked") ? 1 : 0,
                            Description: $("#description").val()
                        };

                        var attrs = "", saleattrs = "";

                        _self.saveCategory(model, attrs, saleattrs, callback);
                    },
                    callback: function () {

                    }
                }
            });

            $("#categoryName").focus();

            //编辑填充数据
            if (Category.CategoryID) {
                $("#categoryName").val(Category.CategoryName);
                $("#categoryStatus").prop("checked", Category.Status == 1);
                $("#description").val(Category.Description);
            }

            CategoryVerifyObject = Verify.createVerify({
                element: ".verify",
                emptyAttr: "data-empty",
                verifyType: "data-type",
                regText: "data-text"
            });
        });
    }

    //元素绑定事件
    ObjectJS.bindElementEvent = function (element) {
        var _self = this;

        //下拉事件
        element.find(".ddlcategory").click(function () {
            var _this = $(this);

            var position = _this.offset();
            $("#ddlCategory li").data("id", _this.data("id"));
            $("#ddlCategory").css({ "top": position.top + 20, "left": position.left }).show().mouseleave(function () {
                $(this).hide();
            });
            return false;
        });

        //点击分类名称事件（展开下级和属性）
        element.click(function () {
            var _this = $(this), layer = _this.data("layer");
            _this.siblings().removeClass("hover");
            _this.addClass("hover");
            _this.parents(".category-layer").nextAll(".category-layer,.category-attr-layer,.common-attr-layer").remove();
            
            
            if (layer < 3) {
                //加载下级分类
                Global.post("/Products/GetChildCategorysByID", {
                    categoryid: _this.data("id"),
                    type: 1
                }, function (data) {
                    doT.exec("template/products/categorys.html", function (templateFun) {
                        var html = templateFun(data.Items);
                        html = $(html);
                        //绑定添加事件
                        html.find(".category-header").html(_this.find(".category-name").html() + " >>");

                        _self.addBindEvent(html.find(".create-child").data("id", _this.data("id")).data("layer", _this.data("layer") * 1 + 1));
                        _self.bindElementEvent(html.find("li"));

                        _this.parents(".category-layer").after(html);

                        _self.bindStyle();

                        _self.showAttrs(_this, true);
                    });
                });
            } else {
                _self.showAttrs(_this, false);
            }
        });
    }

    //显示分类属性
    ObjectJS.showAttrs = function (obj, isNext) {

        var _self = this;
        //属性设置
        Global.post("/Products/GetAttrsByCategoryID", {
            categoryid: obj.data("id")
        }, function (data) {
            console.log(data.Items);
            doT.exec("template/products/category-attrs.html", function (templateFun) {
                var html = templateFun(data.Items);
                html = $(html);
                //绑定添加事件
                html.find(".category-attr-header span").html(obj.find(".category-name").html() + "-属性列表");

                html.find(".ico-add").click(function () {
                    var _attrdiv = $(this);
                    AttrPlug.init({
                        attrid: "",
                        categoryid: obj.data("id"),
                        callback: function (Attr) {
                            _self.innerAttr(_attrdiv.parent().siblings("[data-type=" + Attr.Type + "]"), Attr);
                        }
                    });
                });

                _self.bindAttrElementEvent(html.find(".attritem"));

                if (isNext) {
                    obj.parents(".category-layer").next().after(html);
                } else {
                    obj.parents(".category-layer").after(html);
                }

                _self.bindCommonAttr(obj.data("id"), html);
            });
        });
    }

    //保存属性后添加到列表
    ObjectJS.innerAttr = function (parentele, Attr) {
        var _self = this;
        var ele = $('<li class="attritem" data-id="' + Attr.AttrID + '" data-category="' + Attr.CategoryID + '" title="' + Attr.Description + '">' +
                                                    '<span class="category-attr-name long">' + Attr.AttrName + '</span>' +
                                                    '<span data-id="' + Attr.AttrID + '" data-categoryid="' + Attr.CategoryID + '" data-type="' + Attr.Type + '" class="ico-dropdown ddlattr right"></span>' +
                                                '</li>');
        _self.bindAttrElementEvent(ele);
        parentele.append(ele);
    }

    //绑定通用属性
    ObjectJS.bindCommonAttr = function (categoryid, ele) {
        var _self = this;
        ele.next().remove();
        doT.exec("template/products/common-attrs.html", function (templateFun) {
            var html = templateFun(CacheAttrs);
            html = $(html);

            //隐藏已有属性添加按钮
            html.find(".addcommon").each(function () { //[data-type=" + $(this).data("type") + "]
                if ($(".category-attr-layer").find("ul li[data-id=" + $(this).data("id") + "]").length > 0) {
                    $(this).hide();
                }
            });

            html.find(".addcommon").click(function () {
                _self.addCommonAttr(categoryid, $(this));
            });

            ele.after(html);

            _self.bindStyle();
        });
    }

    //添加通用属性事件
    ObjectJS.addCommonAttr = function (categoryid, ele) {
        var _self = this;
        var _attrlist = $(".category-attr-layer").find("ul[data-type=" + ele.data("type") + "]");
        if (_attrlist.find(".category-attr-layer li[data-id=" + ele.data("id") + "]").length === 0) {
            Global.post("/Products/AddCategoryAttr", {
                categoryid: categoryid,
                attrid: ele.data("id"),
                type: ele.data("type")
            }, function (data) {
                if (data.Status) {
                    ele.parent().find(".addcommon").hide();
                    var _model = {
                        CategoryID: categoryid,
                        AttrID: ele.data("id"),
                        AttrName: ele.data("name"),
                        Type: ele.data("type"),
                        Description: ""
                    }
                    _self.innerAttr(_attrlist, _model);
                }
            });
            
        } else {
            ele.parent().find(".addcommon").hide();
            alert("此分类已存在该" + (ele.data("type") == 1 ? "属性" : "规格") + "，不能重复添加！");
        }
    }

    //绑定属性事件
    ObjectJS.bindAttrElementEvent = function (element) {
        var _self = this;

        //下拉事件
        element.find(".ddlattr").click(function () {
            var _this = $(this);

            var position = _this.offset();
            $("#ddlAttr li").data("id", _this.data("id")).data("type", _this.data("type")).data("categoryid", _this.data("categoryid"));
            $("#ddlAttr").css({ "top": position.top + 20, "left": position.left }).show().mouseleave(function () {
                $(this).hide();
            });
            return false;
        });

        element.click(function () {
            _self.showValues($(this).data("id"));
        })
    }

    //保存分类
    ObjectJS.saveCategory = function (category, attrs, saleattrs, callback) {
        Global.post("/Products/SavaCategory", {
            category: JSON.stringify(category),
            attrlist: attrs,
            saleattr: saleattrs
        }, function (data) {
            if (data.result == "10001") {
                alert("您没有此操作权限，请联系管理员帮您添加权限！");
                return;
            }
            if (data.ID) {
                category.CategoryID = data.ID;
                !!callback && callback(category);
            }
        });
    };

    //显示属性值悬浮层
    ObjectJS.showValues = function (attrID) {
        var height = document.documentElement.clientHeight - 53;
        $("#attrValueBox").css("height", height + "px");
        $("#attrValueBox").animate({ right: "0px" }, "fast");
        Value.AttrID = attrID;
        ObjectJS.getAttrDetail();
    }

    //获取属性明细
    ObjectJS.getAttrDetail = function () {
        Global.post("/Products/GetAttrByID", { attrID: Value.AttrID }, function (data) {
            $("#attrValueBox").find(".header-title").html(data.Item.AttrName);
            ObjectJS.innerValuesItems(data.Item.AttrValues, true);
        });
    }

    //加载值数据
    ObjectJS.innerValuesItems = function (items, clear) {
        var _self = this;
        if (clear) {
            $("#attrValues").empty();
        }
        for (var i = 0, j = items.length; i < j; i++) {
            var item = $('<li data-id="' + items[i].ValueID + '" class="item">' +
                               '<input type="text" data-id="' + items[i].ValueID + '" data-value="' + items[i].ValueName + '" value="' + items[i].ValueName + '" />' +
                               '<span data-id="' + items[i].ValueID + '" class="ico-delete"></span>' +
                         '</li>');
            _self.bindValueElementEvent(item);
            $("#attrValues").prepend(item);
        }
    }

    //隐藏属性值悬浮层
    ObjectJS.hideValues = function () {
        $("#attrValueBox").animate({ right: "-302px" }, "fast");
    }

    //元素绑定事件
    ObjectJS.bindValueElementEvent = function (elments) {
        var _self = this;
        elments.find("input").focus(function () {
            var _this = $(this);
            _this.select();
        });
        elments.find("input").blur(function () {
            var _this = $(this);
            //为空
            if (_this.val() == "") {
                if (_this.data("id") == "") {
                    _this.parent().remove();
                } else {
                    _this.val(_this.data("value"));
                }
            } else if (_this.val() != _this.data("value")) {

                Value.ValueID = _this.data("id");
                Value.ValueName = _this.val();
                //保存属性值
                _self.saveValue(function () {
                    _this.data("value", Value.ValueName);
                });
            }
        });
        elments.find(".ico-delete").click(function () {
            var _this = $(this);
            if (_this.data("id") != "") {
                if (confirm("删除后不可恢复,确认删除吗？")) {
                    _self.deleteValue(_this.data("id"), function (status) {
                        status && _this.parent().remove();
                    });
                }
            } else {
                _this.parent().remove();
            }
        })
    }

    //保存属性值
    ObjectJS.saveValue = function (editback) {
        var _self = this;
        Global.post("/Products/SaveAttrValue", { value: JSON.stringify(Value) }, function (data) {
            if (data.result == "10001") {
                alert("您没有此操作权限，请联系管理员帮您添加权限！");
                return;
            }
            if (data.ID.length > 0) {
                if (Value.ValueID == "") {
                    Value.ValueID = data.ID;
                    _self.innerValuesItems([Value], false);
                }
                !!editback && editback();
            } else {
                alert("操作失败,请稍后重试!");
            }
        });
    }

    //删除属性值
    ObjectJS.deleteValue = function (valueid, callback) {
        Global.post("/Products/DeleteAttrValue", { valueid: valueid }, function (data) {
            if (data.result == "10001") {
                alert("您没有此操作权限，请联系管理员帮您添加权限！");
                return;
            }
            !!callback && callback(data.Status);
        });
    }

    module.exports = ObjectJS;
})