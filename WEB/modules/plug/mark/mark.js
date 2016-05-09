
/* 
作者：Allen
日期：2015-11-6
示例:
    $(...).markColor(callback(obj,callback));
*/

define(function (require, exports, module) {
    require("plug/mark/style.css");
    var Global = require("global");
    (function ($) {
        $.fn.markColor = function (options) {
            var opts = $.extend({}, $.fn.markColor.defaults, options);
            return this.each(function () {
                var _this = $(this);
                $.fn.drawmarkColor(_this, opts);
            })
        }
        $.fn.markColor.defaults = {
            isAll: false,
            onChange: function () { }
        };
        $.fn.drawmarkColor = function (obj, opts) {
            var colors = ["#FFF", "#FF7C7C", "#3BB3FF", "#9F74FF", "#FFC85D", "#FFF65F"];
            obj.data("itemid", Global.guid());
            obj.addClass("mark-color");

            if (obj.data("value") >= 0) {
                obj.css("background-color", colors[obj.data("value")])
                if (obj.data("value") == 0) {
                    obj.css("border", "solid 1px #ccc");
                } else {
                    obj.css("border", "solid 1px " + colors[obj.data("value")]);
                }
            } else {
                obj.addClass("mark-color-all");
            }
            var width = colors.length * 13;
            obj.click(function () {
                $(".mark-color-list").hide();
                var _this = $(this);
                //var position = _this.position();
                var position = _this.offset();
                if ($("#" + _this.data("itemid")).length == 0) {
                    var _colorBody = $("<ul id='" + _this.data("itemid") + "' class='mark-color-list'></ul>");

                    if (opts.isAll) {
                        var _all = $("<li data-value='-1' title='全部' class='mark-color-item all'><span></span></li>");
                        _colorBody.append(_all);
                        width += 10;
                    }

                    for (var i = 0; i < colors.length; i++) {
                        var _color = $("<li data-value='" + i + "' class='mark-color-item'><span></span></li>");
                        if (i == 0) {
                            _color.addClass("first");
                        }
                        _color.find("span").css("background-color", colors[i]);
                        _colorBody.append(_color);
                    }

                    _colorBody.find(".mark-color-item").click(function () {
                        var _changeColor = $(this);
                        //更换才触发
                        if (_changeColor.data("value") != _this.data("value")) {
                            _this.data("value", _changeColor.data("value"));
                            !!opts.onChange && opts.onChange(_this, function (status) {
                                if (status) {
                                    if (_changeColor.data("value") >= 0) {
                                        if (_changeColor.data("value") == 0) {
                                            _this.css("border", "solid 1px #ccc");
                                        } else {
                                            _this.css("border", "solid 1px " + colors[_changeColor.data("value")]);
                                        }
                                        _this.css("background-color", colors[_changeColor.data("value")]);
                                        _this.removeClass("mark-color-all");
                                    } else {
                                        _this.css("border", "none");
                                        _this.addClass("mark-color-all");
                                    }
                                }
                            });
                        }
                        _colorBody.hide();
                    });
                    _colorBody.css({ "top": position.top + 20, "left": position.left - width + 10 }).show();
                    $("body").append(_colorBody);
                } else {
                    $("#" + _this.data("itemid")).css({ "top": position.top + 20, "left": position.left - colors.length * 13 + 10 }).show();
                }
                return false;
            });

            $(document).click(function (e) {
                if ($(e.target).data("itemid") != obj.data("itemid") && $(e.target).attr("id") != obj.data("itemid")) {
                    $("#" + obj.data("itemid")).hide();
                }
            });
            
        }
    })(jQuery)
    module.exports = jQuery;
});