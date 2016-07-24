
/* 
作者：Allen
日期：2015-10-25
示例:
    $(...).dropdown(options);
*/

define(function (require, exports, module) {
    require("plug/dropdown/style.css");
    var Global = require("global");
    (function ($) {
        $.fn.dropdown = function (options) {
            var opts = $.extend({}, $.fn.dropdown.defaults, options);
            return this.each(function () {
                var _this = $(this);
                $.fn.drawDropdown(_this, opts);
            })
        }
        $.fn.dropdown.defaults = {
            prevText: "",//文本前缀
            defaultText: "",
            defaultValue: "",
            data: [],
            dataValue: "ID",
            dataText: "Name",
            width: "180",
            isposition: false,
            onChange: function () { }
        };
        $.fn.drawDropdown = function (obj, opts) {

            obj.data("itemid", Global.guid());
            obj.data("id", opts.defaultValue);

            if (!obj.hasClass("dropdown-module")) {
                obj.addClass("dropdown-module").css("width", opts.width);
            }


            var _input = $('<div class="dropdown-text long">' + opts.prevText + opts.defaultText + '</div>');
            _input.css("width", opts.width - 30);
            var _ico = $('<div class="dropdown-ico"><span class="top"></span><span class="bottom"></span></div>');
            obj.empty();
            obj.append(_input).append(_ico);


            
            //处理事件
            obj.unbind().click(function () {
                var _this = $(this);
                
                if (_this.hasClass("hover")) {
                    $("#" + obj.data("itemid")).hide();
                    _this.removeClass("hover");
                } else {
                    $.fn.drawDropdownItems(obj, opts);
                    _this.addClass("hover");
                }
            });

            $(document).click(function (e) {
                //隐藏下拉
                var bl = false;
                $(e.target).parents().each(function () {
                    var _this = $(this);
                    if (_this.data("itemid") == obj.data("itemid") || _this.attr("id") == obj.data("itemid")) {
                        bl = true;
                    }
                });
                if ($(e.target).data("itemid") == obj.data("itemid") || $(e.target).attr("id") == obj.data("itemid")) {
                    bl = true;
                }
                if (!bl) {
                    obj.removeClass("hover");
                    $("#" + obj.data("itemid")).hide();
                }
            });
        }
        $.fn.drawDropdownItems = function (obj, opts) {
            var offset = obj.offset();
            if (opts.isposition) {
                offset = obj.position();
            }

            if ($("#" + obj.data("itemid")).length == 1) {
                $("#" + obj.data("itemid")).css({ "top": offset.top + 27, "left": offset.left }).show();
            } else {
                var _items = $("<ul class='dropdown-items-modules' id='" + obj.data("itemid") + "'></ul>").css("width", opts.width);

                if (opts.defaultText) {
                    _items.append("<li data-id='" + opts.defaultValue + "'>" + opts.defaultText + "</li>");
                }
                for (var i = 0; i < opts.data.length; i++) {
                    if (opts.data[i][opts.dataValue] != opts.defaultValue) {
                        _items.append("<li data-id='" + opts.data[i][opts.dataValue] + "'>" + opts.data[i][opts.dataText] + "</li>");
                    }
                }
                _items.find("li").unbind().click(function () {
                    obj.find(".dropdown-text").html(opts.prevText + $(this).html());
                    obj.data("id", $(this).data("id"));
                    obj.removeClass("hover");
                    $("#" + obj.data("itemid")).hide();
                    opts.onChange({
                        value: $(this).data("id"),
                        text: $(this).html()
                    });
                });
                _items.css({ "top": offset.top + 27, "left": offset.left });
                obj.after(_items);
            }
        }
    })(jQuery)
    module.exports = jQuery;
});