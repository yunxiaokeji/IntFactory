define(function (require, exports, module) {
    var Global = require("global"),
        Verify = require("verify"), VerifyObject,
        doT = require("dot"),
        AttrPlug = require("scripts/products/attrplug");
    require("pager");
    var Value = {
        AttrID: "",
        ValueID: "",
        ValueName: ""
    },
    Params = {
        Index: 1,
        KeyWord: ""
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
            if (!$(e.target).hasClass("attr-values") && !$(e.target).parents().hasClass("attr-values") && !$(e.target).hasClass("attr-value-box") && !$(e.target).parents().hasClass("attr-value-box")) {
                _self.hideValues();
            }
        });

        $("#addAttr").click(function () {
            AttrPlug.init({
                attrid: "",
                categoryid: "",
                callback: function (Attr) {
                    _self.getList();
                }
            });
        });

        //搜索
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                Params.KeyWord = keyWords;
                ObjectJS.getList();
            });
        });

        //删除
        $("#deleteObject").click(function () {
            var _this = $(this);
            if(confirm("属性删除后不可恢复,确认删除吗？")){
                Global.post("/Products/DeleteProductAttr", { attrid: _this.data("id") }, function (data) {
                    if (data.Status) {
                        _self.getList();
                    }
                });
            }
        });

        //编辑
        $("#updateObject").click(function () {
            var _this = $(this);

            AttrPlug.init({
                attrid: _this.data("id"),
                callback: function (Attr) {
                    _self.getList();
                }
            });
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
        })
    }
    //获取属性列表
    ObjectJS.getList = function () {
        var _self = this;
        $(".tr-header").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='6'><div class='dataLoading'><img src='/modules/images/ico-loading.jpg'/><div></td></tr>");

        Global.post("/Products/GetAttrList", { index: Params.Index, keyWorks: Params.KeyWord }, function (data) {
            _self.innerItems(data.Items, true);

            $("#pager").paginate({
                total_count: data.TotalCount,
                count: data.PageCount,
                start: Params.Index,
                display: 5,
                border: true,
                border_color: '#fff',
                text_color: '#333',
                background_color: '#fff',
                border_hover_color: '#ccc',
                text_hover_color: '#000',
                background_hover_color: '#efefef',
                rotate: true,
                images: false,
                mouse: 'slide',
                onChange: function (page) {
                    Params.Index = page;
                    _self.getList();
                }
            });
        });
    }
    //加载属性数据
    ObjectJS.innerItems = function (items, clear) {
        var _self = this;
        if (clear) {
            $("#attrList").nextAll().remove();
        }

        if (items.length > 0) {
            doT.exec("template/products/attrs.html", function (templateFun) {
                var inner = templateFun(items);
                inner = $(inner);
                $("#attrList").after(inner);

                //下拉事件
                inner.find(".dropdown").click(function () {
                    var _this = $(this);

                    var position = _this.find(".ico-dropdown").position();
                    $(".dropdown-ul li").data("id", _this.data("id"));
                    $(".dropdown-ul").css({ "top": position.top + 20, "left": position.left - 55 }).show().mouseleave(function () {
                        $(this).hide();
                    });
                });

                inner.find(".attr-values").click(function () {
                    var _this = $(this);
                    _self.showValues(_this.data("id"));
                });
            })
        }
        else {
            $(".tr-header").after("<tr><td colspan='6'><div class='noDataTxt' >暂无数据!<div></td></tr>");
        }

    }
    //显示属性值悬浮层
    ObjectJS.showValues = function (attrID) {
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
    //隐藏属性值悬浮层
    ObjectJS.hideValues = function () {
        $("#attrValueBox").animate({ right: "-302px" }, "fast");
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
            _self.bindElementEvent(item);
            $("#attrValues").prepend(item);
        }
    }
    //元素绑定事件
    ObjectJS.bindElementEvent = function (elments) {
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
        Global.post("/Products/DeleteAttrValue", { valueid: valueid, attrid: Value.AttrID }, function (data) {
            if (data.result == "10001") {
                alert("您没有此操作权限，请联系管理员帮您添加权限！");
                return;
            }
            !!callback && callback(data.Status);
        });
    }
    module.exports = ObjectJS;
});