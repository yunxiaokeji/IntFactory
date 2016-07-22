define(function (require, exports, module) {
    var Global = require("global");
    var Unit = {};
    //初始化
    Unit.init = function () {
        var _self = this;
        _self.bindEvent();
        _self.bindElementEvent($(".unit-item"));
    }

    //删除单位
    Unit.deleteUnit = function (unitid, callback) {
        Global.post("/Products/DeleteUnit", { unitID: unitid }, function (data) {
            !!callback && callback(data.Status);
        })
    }

    //绑定事件
    Unit.bindEvent = function () {
        var _self = this;
        $("#addUnit").click(function () {
            var _this = $(this);
            var _ele = $('<li class="unit-item"><input type="text" maxlength="5" data-id="" data-value="" value="" /><span data-id="" class="ico-delete"></span></li>');
            _self.bindElementEvent(_ele);
            _this.before(_ele);
            _ele.find("input").focus();
        });
    }

    //附加元素事件
    Unit.bindElementEvent = function (elments) {
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
                var unit = {
                    UnitID: _this.data("id"),
                    UnitName: _this.val(),
                    Description: ""
                };
                Global.post("/Products/SaveUnit", { unit: JSON.stringify(unit) }, function (data) {
                    if (data.result == "10001") {
                        alert("您没有此操作权限，请联系管理员帮您添加权限！");
                        return;
                    }
                    if (data.ID.length > 0) {
                        _this.data("id", data.ID);
                        _this.data("value", unit.UnitName);
                        _this.next().data("id", data.ID);
                    }
                })
            }
        });
        elments.find(".ico-delete").click(function () {
            var _this = $(this);
            if (_this.data("id") != "") {
                if (confirm("单位删除后不可恢复,确认删除吗？")) {
                    _self.deleteUnit(_this.data("id"), function (status) {
                        if (status) {
                            _this.parent().remove();
                        } else {
                            alert("单位已被使用，不能删除")
                        }
                    });
                }
            } else {
                _this.parent().remove();
            }
        })
    }

    module.exports = Unit;
});