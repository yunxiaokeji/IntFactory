/* 
作者：Leo
日期：2016-08-24
示例:
    $(...).chooseMenu(options);
*/
define(function (require, exports, module) {
    require("plug/choosemenu/choosemenu.css");
    var DoT = require("dot");
    (function () {
        $.fn.chooseMenu = function (option) {
            return this.each(function () {
                var _this = $(this);
                _this.val('');
                var options = $.extend({}, $.fn.chooseMenu.default, option);
                if (options.isInit) {
                    $(".change-menu-body").remove();
                }
                $.fn.chooseMenu.bindMenu(_this, options);
            });
        };

        $.fn.chooseMenu.default = {
            width: 400,
            data: [],
            layer: 3,
            isInit: false,/*是否初始化控件*/
            headerDefaults: {
                headerText: "请选择",
                headerID: ""
            },
            dataKey: {
                CategoryID: "CategoryID",
                CategoryItems: "ChildCategory"
            },
            onHeaderChange: function () {
            },
            onCategroyChange: function () {
            }
        };

        $.fn.chooseMenu.bindMenu = function (obj, opts) {
            //绑定样式
            obj.css({
                "background": "url('/modules/images/ico-dropdown.png') no-repeat right 5px center"
            });
            //缓存信息
            var CacheChildCategory = [];
            var CacheCategory = [];
            var _contentBody;
            for (var i = 0; i < opts.data.length; i++) {
                CacheChildCategory[opts.data[i]["" + opts.dataKey.CategoryID + ""]] = opts.data[i]["" + opts.dataKey.CategoryItems + ""];
            }
            obj.click(function () {
                var _this = $(this);
                var offset = obj.offset();
                if (_contentBody) {
                    _contentBody.css({ "left": offset.left + "px", "top": (offset.top + 27) + "px" }).show();
                } else {
                    DoT.exec("plug/choosemenu/choosemenu.html", function (template) {
                        _contentBody = template();
                        _contentBody = $(_contentBody);
                        _contentBody.css({
                            "width": opts.width + "px",
                            "left": offset.left + "px",
                            "top": (offset.top + 27) + "px"
                        });
                        _contentBody.find('.close-layer').click(function () {
                            _contentBody.hide();
                        });
                        $('body').append(_contentBody);
                        bindObj(opts.data, obj, opts.headerDefaults);
                    });
                }
                return false;
            });
            //创建分类Html
            var bindObj = function (data, obj, _headerData) {
                //html
                var _headerMenu = _contentBody.find(".change-menu-header");
                var _contentMenu = _contentBody.find(".change-menu-content");

                /*头部数据处理*/
                if (_headerData.headerText) {
                    _headerMenu.find("li").removeClass('hover');
                    _headerMenu.find("li:last-child").html(_headerData.headerText);
                    var _headerObj = $('<li class="hand hover" data-id="' + _headerData.headerID + '">请选择</li>');
                    _headerMenu.find("ul").append(_headerObj);
                    _headerObj.click(function () {
                        if (!$(this).hasClass('hover')) {
                            $(this).siblings().removeClass('hover');
                            $(this).addClass('hover').html("请选择");
                            _headerObj.nextAll().remove();
                            var headerData = {
                                headerText: '',
                                headerID: ''
                            };
                            _contentMenu.find("ul").empty();
                            var id = $(this).data("id");
                            var item = getCacheDataByID(id);
                            if (!item) {
                                _contentBody.append('<div class="nodata-txt">暂无分类信息！</div>');
                                bindObj(item, obj, headerData);
                            } else {
                                _contentBody.find('.nodata-txt').remove();
                                bindObj(item, obj, headerData);
                                !opts.onHeaderChange || opts.onHeaderChange(CacheCategory[id] || "");
                            }
                        }
                    });
                }

                /*子分类数据处理*/
                if (data) {
                    for (var i = 0; i < data.length; i++) {
                        var item = data[i];
                        var _childMenu = $('<li class="hand categategory-item" data-layer="' + item.Layers + '" data-name="' + item.CategoryName + '" data-id="' + item.CategoryID + '">' + item.CategoryName + '</li>');
                        _contentMenu.find("ul").append(_childMenu);
                        _childMenu.click(function () {
                            var _this = $(this);
                            var id = $(this).data("id");
                            if (_this.data('layer') < opts.layer) {
                                var item = getCacheDataByID(id);
                                var headerData = {
                                    headerText: _this.text(),
                                    headerID: _this.data("id")
                                };
                                _contentMenu.find("ul").empty();
                                if (!item) {
                                    _contentBody.append('<div class="nodata-txt">暂无分类信息！</div>');
                                } else {
                                    _contentMenu.find(".nodata-txt").remove();
                                }
                                bindObj(item, obj, headerData);
                            } else {
                                _headerMenu.find("li:last-child").html(_this.text());
                                _contentBody.hide();
                            }

                            /*拼接文本框中的值*/
                            var _desc = "";
                            _headerMenu.find("li").each(function () {
                                var _this = $(this);
                                if (_this.text().trim() == "请选择") {
                                    return false;
                                }
                                _desc += _this.text().trim();
                                if (_this.index() < _headerMenu.find("li").length - 1) {
                                    _desc += "/";
                                }
                            });
                            obj.val(_desc);

                            !opts.onCategroyChange || opts.onCategroyChange(_this.data('layer') == opts.layer ? CacheCategory[id] : "");
                        });
                    }
                }
            };
            //获取子分类
            var getCacheDataByID = function (id) {
                if (!id)
                    return opts.data;
                if (CacheChildCategory[id]) {
                    //缓存分类和分类下的所有子分类
                    var items = CacheChildCategory[id];
                    for (var i = 0; i < items.length; i++) {
                        if (!CacheCategory[items[i]["" + opts.dataKey.CategoryID + ""]]) {
                            CacheCategory[items[i]["" + opts.dataKey.CategoryID + ""]] = items[i];
                            for (var key in items) {
                                var model = items[key];
                                if (model["" + opts.dataKey.CategoryItems + ""].length > 0) {
                                    CacheChildCategory[model["" + opts.dataKey.CategoryID + ""]] = model["" + opts.dataKey.CategoryItems + ""];
                                }
                            }
                        }
                    }
                    return CacheChildCategory[id];
                }
            };

            $(document).click(function (e) {
                var ele = $(e.target);
                if (!ele.parents().hasClass('change-menu-body') && !ele.hasClass('change-menu-body')
                    && !ele.parents().hasClass('change-menu-content') && !ele.hasClass('categategory-item')) {
                    if (_contentBody) {
                        _contentBody.hide();
                    }
                }
            });
        };
    })(jQuery);
});