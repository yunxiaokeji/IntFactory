/*
调用方式
$(".dropdown").dropdown({
    prevText: "--",
    defaultText: "请选择",
    defaultValue: "0",
    data: data,
    onChange: function (data) {
        console.log(data);
    },
    postUrl: "/Home/GG"
});
*/
define(function (require, exports, module) {
    require("plug/dropdown_more/style.css");
    var Global = require("global");
    require("search");
    (function ($) {
        $.fn.dropdownSearch = function (options) {
            var opts = $.extend({}, $.fn.dropdownSearch.defaults, options);
            return this.each(function () {
                var _this = $(this);
                $.fn.drawDropdownSearch(_this, opts);
            })
        };

        $.fn.dropdownSearch.defaults = {
            dataValue: "ID",
            dataText: "Name",
            moreDataParams: "",/*{width:"",dataTexts:[""],dataSplitText:""}*/
            defaultsSearchParams: {},
            keyWordsName: "keyWords",
            PostUrl: "",
            PostParms: "",
            width: "180",
            searchWidth: "",
            offset:"",
            isposition: false,
            isCreate: true,
            showAll: false,
            onChange: function () { }
        };

        $.fn.drawDropdownSearch = function (obj, opts) {
            obj.data("itemid", Global.guid());
            obj.data("id", opts.defaultValue);
            if (opts.isCreate) {
                if (!obj.hasClass("dropdown-module")) {
                    obj.addClass("dropdown-module").css("width", opts.width);
                }
                var _input = $('<div class="dropdown-text long">' + (opts.prevText || '') + opts.defaultText + '</div>');
                _input.css("width", opts.width - 30);
                var _ico = $('<div class="dropdown-ico"><span class="top"></span><span class="bottom"></span></div>');
                obj.empty();
                obj.append(_input).append(_ico);
            }
            //处理事件
            obj.unbind().click(function () {
                var _this = $(this);

                if (_this.hasClass("hover")) {
                    $("#" + obj.data("itemid")).hide();
                    _this.removeClass("hover");
                } else {
                    $.fn.drawDropdownItemsSearch(obj, opts);
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
        };

        $.fn.drawDropdownItemsSearch = function (obj, opts) {
            var offset = obj.offset();
            var cacheData = [];
            if (opts.isposition) {
                offset = obj.position();
            }
            if ($("#" + obj.data("itemid")).length == 1) {
                $("#" + obj.data("itemid")).css({ "top": offset.top + 27, "left": (opts.offset == "left" ? offset.left - opts.width : opts.offset == "center" ? (offset.left - (opts.width*1/2)) : offset.left) }).show();
            } else {
                var _body = $("<div class='dropdown-search-body' id='" + obj.data("itemid") + "'></div>").css("width", opts.width);
                var _items = $("<ul class='dropdown-search-modules'><div class='data-loading'></div></ul>");

                var _search = $("<div data-width='" + (!opts.searchWidth ? (opts.width * 1 - 37) : opts.searchWidth) + "' style='line-height:25px;'></div>");
                var _httpPostObj;
                var searchList = function (keyword) {
                    _httpPostObj && _httpPostObj.abort();
                    var newParams = $.extend({}, opts.defaultsSearchParams, {
                        //keyWords: keyword || ''
                    });
                    newParams[opts.keyWordsName] = keyword || '';
                    _httpPostObj = Global.post(opts.PostUrl, newParams, function (data) {
                        _items.html('');
                        if (opts.showAll) {
                            _items.append("<li data-id=''><div class='item' style='padding:0 6px'>全部</div></li>");
                        }
                        if (opts.showDefault) {
                            _items.append("<li data-id='" + opts.defaultValue + "'><div class='item' style='padding:0 6px'>" + opts.defaultText + "</div></li>");
                        }
                        if (data.items && data.items.length > 0 || opts.showAll || opts.showDefault) {
                            for (var i = 0; i < data.items.length; i++) {
                                var item = data.items[i];
                                cacheData[item[opts.dataValue]] = item;
                                var _Li = $("<li class='clearfix' style='overflow:hidden' data-id='" + item[opts.dataValue] + "'>" +
                                    "<div class='long item' title='" + item[opts.dataText] + "' style='width:" + (opts.moreDataParams ? opts.width : opts.width - 10) + "px'>" + item[opts.dataText] + "</div>" +
                                    "</li>");
                                if (opts.moreDataParams) {
                                    var _width = opts.moreDataParams.width;
                                    var _text = "";
                                    for (var k = 0; k < opts.moreDataParams.dataTexts.length; k++) {
                                        var _data = item[opts.moreDataParams.dataTexts[k]];
                                        if (opts.moreDataParams.dataTexts[k] == "Price"
                                            && ((item.UnitID && item.UnitID == $("#specialUnits").data('id')) || item.Type == 4 || item.Type == 5)) {
                                            _data = _data * 1000000;
                                        }
                                        _text += _data;
                                        if (k != opts.moreDataParams.dataTexts.length - 1 && opts.moreDataParams.dataSplitText) {
                                            _text += opts.moreDataParams.dataSplitText;
                                        }
                                    }
                                    var _html = $("<div class='long tRight mRight5' style='width:" + (_width - 5) + "px'>" + _text + "</div>");

                                    _Li.append(_html);
                                }
                                _items.append(_Li);
                                var _LiWidth = opts.width * 1 - opts.moreDataParams.width * 1 - 5;
                                if (_items.find('li').length > 5) {
                                    _LiWidth -= 20;
                                }
                                _items.find('li .item').css({ width: _LiWidth });
                            }
                            _items.find("li").unbind().click(function () {
                                obj.find(".dropdown-text").html((opts.prevText || '') + $(this).find('.item').text());
                                obj.data("id", $(this).data("id"));
                                obj.removeClass("hover");
                                $("#" + obj.data("itemid")).hide();
                                opts.onChange({
                                    element: obj,
                                    item: cacheData[$(this).data("id")],
                                    value: $(this).data("id")
                                });
                            });
                        } else {
                            _items.html("<div class='nodata-txt'>暂无数据!</div>");
                        }
                    });
                };

                searchList();
                var _searchObj = _search.searchKeys(function (text) {
                    _items.html("<div class='data-loading'></div>");
                    searchList(text);
                });
                
                _searchObj.find('input.search-ipt').keyup(function () {
                    searchList($(this).val());
                });

                _body.append(_search).append(_items);
                _body.css({ "top": offset.top + 27, "left": (opts.offset == "left" ? offset.left - opts.width : opts.offset == "center" ? (offset.left - (opts.width * 1 / 2)) : offset.left) });
                obj.after(_body);
            }
            
        };

    })(jQuery)
});