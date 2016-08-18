/* 
作者：Michaux
日期：2016-06-13
示例:
    $(...).markColor(options);
*/
define(function (require, exports, module) {
    require("plug/colormark/style.css");
    var Global = require("global");
    (function ($) {
        $.fn.markColor = function (options) {
            var opts = $.extend({}, $.fn.markColor.defaults, options);
            return this.each(function () {
                var _this = $(this);
                $.fn.drawmarkColor(_this, opts);
            });
        }
        $.fn.markColor.defaults = {
            isAll: false,
            data: [],
            xRepair: 0,
            dataValue: "ColorID",
            dataColor: "ColorValue",
            dataText: "ColorName",
            onChange: function () { }
        };
        $.fn.drawmarkColor = function (obj, opts) {
            obj.data("itemid", Global.guid());
            obj.addClass("mark-color");

            if (obj.data("value") >= 0) {
                obj.css("background-color", $.fn.getColor(obj.data("value"), opts));
                if (obj.data("value") == 0) {
                    obj.css("border", "solid 1px #ccc");
                    obj.css("background-color", "#fff");
                } else {
                    obj.css("border", "solid 1px " + $.fn.getColor(obj.data("value"), opts));
                }
            } else {
                obj.addClass("mark-color-all");
            }
            var width = 0;
            obj.click(function () {
                $(".mark-color-list").hide();
                var _this = $(this);
                var position = _this.offset();
                var top = position.top + 20;
                var left = position.left - 9, isBottom = $(document).height() - position.top < 197;

                if (opts.xRepair != 0) {
                    left = position.left + opts.xRepair;
                }
                if ($("#" + _this.data("itemid")).length == 0) {
                    var _colorBody = $("<ul id='" + _this.data("itemid") + "'  class='mark-color-list'></ul>");

                    if (opts.isAll) {
                        var _all = $("<li data-value='-1' title='全部' class='mark-color-item all'><span></span><div >全部</div></li>");
                        _colorBody.append(_all);
                        width += 10;
                    }
                    for (var i = 0; i < opts.data.length; i++) {
                        var tempguid = Global.guid();
                        var tempcolor = opts.data[i][opts.dataColor];
                        var _color = $("<li id='" + tempguid + "'    data-value='" + opts.data[i][opts.dataValue] + "' class='mark-color-item'><span></span><div >" + opts.data[i][opts.dataText] + "</div></li>");
                        _color.find("span").css("background-color", tempcolor);
                        _colorBody.append(_color);
                        _color.find("li").mouseover(function () { console.log(1) });
                    }
                    if ((opts.isAll && opts.data.length > 7) || (!opts.isAll && opts.data.length > 8)) {
                        if (isBottom) {
                            top = position.top - 217;
                        }
                        _colorBody.css("overflow-y", "scroll");
                    } else {
                        _colorBody.css("overflow-y", "auto");
                        if (isBottom) {
                            if (opts.isAll) {
                                top = position.top - (opts.data.length + 1) * 25 - 20;
                            } else {
                                top = position.top - opts.data.length * 25 - 20;
                            }
                        }
                    }
                    _colorBody.find(".mark-color-item").click(function () {
                        var _changeColor = $(this);
                        //更换才触发 
                        if (_changeColor.data("value") != _this.data("value")) {
                            _this.data("value", _changeColor.data("value"));
                            !!opts.onChange && opts.onChange(_this, function (status) {
                                if (status) {
                                    if (_changeColor.data("value") > 0) {
                                        if (_changeColor.data("value") == 0) {
                                            _this.css("border", "solid 1px #ccc");
                                        } else {
                                            _this.css("border", "solid 1px " + $.fn.getColor(_changeColor.data("value"), opts));
                                        }
                                        _this.css("background-color", $.fn.getColor(_changeColor.data("value"), opts));
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
                    _colorBody.css({ "top": top, "left": left }).show();
                    $("body").append(_colorBody);
                } else {
                    if ($(document).height() - position.top < 197) {
                        top = position.top - $("#" + _this.data("itemid")).height() - 20;
                    }
                    $("#" + _this.data("itemid")).css({ "top": top, "left": left }).show();
                }
                return false;
            });
            $(document).click(function (e) {
                if ($(e.target).data("itemid") != obj.data("itemid") && $(e.target).attr("id") != obj.data("itemid")) {
                    $("#" + obj.data("itemid")).hide();
                }
            });
        }
        $.fn.getColor = function (value, opts) {
            var color = "";
            for (var i = 0; i < opts.data.length; i++) {

                if (opts.data[i][opts.dataValue] == value) {
                    color = opts.data[i][opts.dataColor];
                    break;
                }
            }
            //if (!color) {
            //    color = "#ccc";
            //}
            return color;
        };
        $.fn.bindmouseover = function (colorvalue, li) {
            $('#' + li).css("background-color", colorvalue);
        };
        $.fn.bindmouseout = function (colorvalue, li) {
            $('#' + li).css("background-color", "");
        };
    })(jQuery);
    module.exports = jQuery;
});