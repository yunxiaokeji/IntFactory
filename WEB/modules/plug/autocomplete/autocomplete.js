
/* 
作者：Allen
日期：2016-3-22
示例:
    $(...).autocomplete(options);
*/

define(function (require, exports, module) {
    require("plug/autocomplete/style.css");
    var Global = require("global");
    (function ($) {
        $.fn.autocomplete = function (options) {
            var opts = $.extend({}, $.fn.autocomplete.defaults, options);
            return this.each(function () {
                var _this = $(this);
                $.fn.drawAutoComplete(_this, opts);
            })
        }
        $.fn.autocomplete.defaults = {
            defaultText: "",
            defaultValue: "",
            width: "180",
            url: "",
            params: {},
            isposition: false,
            asyncCallback: function (response, data) { },
            select: function () { }
        };
        $.fn.drawAutoComplete = function (obj, opts) {

            obj.data("id", opts.defaultValue);
            obj.data("name", opts.defaultText);
            obj.data("itemid", Global.guid());
            if (!obj.hasClass("autocomplete-module")) {
                obj.addClass("autocomplete-module").css("width", opts.width);
            }
            var _input = $('<input type="text" class="autocomplete-text" value="' + opts.defaultText + '" />');
            _input.css("width", opts.width - 30);
            var _ico = $('<div class="dropdown-ico"></div>');
            obj.append(_input).append(_ico);
            
            _input.focus(function () {
                var _this = $(this);
                $("#" + obj.data("itemid")).remove();
                if (_this.val() && _this.val().length > 0) {
                    opts.params.keyWords = _this.val();
                    if (obj.data("status") != "1") {
                        obj.data("status", "1").data("val", _this.val());
                        Global.post(opts.url, opts.params, function (data) {
                            opts.asyncCallback(data, function (items) {
                                obj.data("status", "0");
                                $.fn.drawAutoCompleteItems(obj, opts, items)
                            })
                        });
                    }
                }
            });

            //处理事件
            _input.keyup(function () {
                var _this = $(this);
                $("#" + obj.data("itemid")).remove();
                if (_this.val() && _this.val().length > 0) {
                    opts.params.keyWords = _this.val();
                    if (obj.data("status") != "1" && obj.data("val") != _this.val()) {
                        obj.data("status", "1").data("val", _this.val());
                        obj.data("id", "");
                        obj.data("name", $(this).html())
                        Global.post(opts.url, opts.params, function (data) {
                            opts.asyncCallback(data, function (items) {
                                obj.data("status", "0");
                                $.fn.drawAutoCompleteItems(obj, opts, items)
                            })
                        });
                    }
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
                if (!bl) {
                    obj.removeClass("hover");
                    $("#" + obj.data("itemid")).hide();
                }
            });
        }
        $.fn.drawAutoCompleteItems = function (obj, opts, data) {
            var offset = obj.offset();
            if (opts.isposition) {
                offset = obj.position();
            }
            if ($("#" + obj.data("itemid")).length == 1) {
                $("#" + obj.data("itemid")).css({ "top": offset.top + 27, "left": offset.left }).show();
            } else {
                var _items = $("<ul class='autocomplete-items-modules' id='" + obj.data("itemid") + "'></ul>").css("width", opts.width);

                for (var i = 0; i < data.length; i++) {
                    _items.append("<li data-id='" + data[i]["id"] + "' data-name='" + data[i]["name"] + "'>" + data[i]["text"] + "</li>");
                }
                _items.find("li").click(function () {
                    obj.find(".autocomplete-text").val($(this).data("name"));
                    obj.data("id", $(this).data("id"));
                    obj.data("name", $(this).data("name"));
                    obj.removeClass("hover");
                    $("#" + obj.data("itemid")).hide();
                    opts.select({
                        value: $(this).data("id"),
                        text: $(this).data("name")
                    });
                });
                _items.css({ "top": offset.top + 27, "left": offset.left });
                obj.after(_items);
            }
        }
    })(jQuery)
    module.exports = jQuery;
});