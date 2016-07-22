define(function (require, exports, module) {
    var Global = require("global"),
        Verify = require("verify"), VerifyObject,
        doT = require("dot"),
        Easydialog = require("easydialog");
        AttrPlug = require("scripts/products/attrplug");
    require("pager");
    var Value = {
        AttrID: "",
        ValueID: "",
        ValueName: "",
        Sort:0
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
                    } else {
                        alert("属性（规格）存在关联分类，不能删除");
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
        });

        $("#valueName").keydown(function (e) {
            var _this = $(this);
            if (e.keyCode == 13) {
                $(".ico-input-add").click();
            }
        });
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
            })
        }
        else {
            $(".tr-header").after("<tr><td colspan='6'><div class='noDataTxt' >暂无数据!<div></td></tr>");
        }

    }

    ObjectJS.initDetail = function (attrID) {
        Value.AttrID = attrID;
        ObjectJS.bindDetailEvent();

        ObjectJS.getAttrItems(attrID);
    }

    ObjectJS.bindDetailEvent = function () {
        var _self = this;
        $("#addAttrValue").click(function () {
            _self.showValue("", "", 1);
        });
    }

    ObjectJS.showValue = function (valuesid, name, sort) {
        var html = '<ul class="create-attr">' +
                        '<li><span class="width80 left">名称：</span><input type="text" id="valueName" maxlength="10" value="' + name + '" class="input verify " data-empty="必填" /></li>';
        html += '<li><span class="width80 left">排序：</span><input type="text" class="width80 verify" value="' + sort + '" data-type="number" data-text="格式不对" data-empty="必填" id="valueSort" ></li></ul>';

        Easydialog.open({
            container: {
                id: "show-add-attrvalue",
                header: "新增属性值",
                content: html,
                yesFn: function () {
                    //验证插件
                    VerifyObject = Verify.createVerify({
                        element: ".create-attr .verify",
                        emptyAttr: "data-empty",
                        verifyType: "data-type",
                        regText: "data-text"
                    });

                    if (!VerifyObject.isPass()) {
                        return false;
                    };

                    Value.ValueName = $("#valueName").val();
                    Value.ValueID = valuesid;
                    Value.Sort = $("#valueSort").val();
                    ObjectJS.saveAttrValue();
                },
                callback: function () {

                }
            }
        });
    }

    ObjectJS.getAttrItems = function (attrID) {
        var _self = this;
        Global.post("/Products/GetAttrByID", { attrID: attrID }, function (data) {
            $(".header-title").html(data.Item.AttrName + " 属性(规格)");
            document.title = data.Item.AttrName + " 属性(规格)详情";
            var items = data.Item.AttrValues;

            doT.exec("template/products/attr-detail.html", function (templateFun) {
                var inner = templateFun(items);
                inner = $(inner);
                $("#attrItems").after(inner);

                inner.find(".ico-del").click(function () {
                    var _this = $(this);
                    if (confirm("删除后不可恢复，确认删除此属性值吗？")) {
                        _self.deleteValue(_this.data("id"), function () {
                            location.href = location.href;
                        });
                    }
                });

                inner.find(".ico-edit").click(function () {
                    var _this = $(this);
                    _self.showValue(_this.data("id"), _this.data("name"), _this.data("sort"));
                });
            });
        });
    }

    ObjectJS.saveAttrValue = function () {
        Global.post("/Products/SaveAttrValue", { value: JSON.stringify(Value) }, function (data) {
            if (data.result == "10001") {
                alert("您没有此操作权限，请联系管理员帮您添加权限！");
                return;
            }
            if (data.ID.length > 0) {
                location.href = location.href;
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