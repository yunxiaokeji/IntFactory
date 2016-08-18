define(function (require, exports, module) {
    require("plug/chooseordercategory/chooseordercategory.css");
    var DoT = require("dot");
    (function () {
        var menuData = [];
        var cacheData = [];
        var CacheChildCategory = [];
        var CacheCategory = [];
        var options;
        var _eleHeader;
        var _menuContent;

        $.fn.chooseMenu = function (option) {
            return this.each(function () {
                var _this = $(this);
                options = $.extend({}, $.fn.chooseMenu.default, option);
                cacheData = options.data;
                if (options.isInit) {
                    $(".change-menu-body").remove();
                    menuData = [];
                }
                for (var i = 0; i < cacheData.length; i++) {
                    CacheChildCategory[cacheData[i].CategoryID] = cacheData[i].ChildCategory;
                }
                for (var i = 1; i <= options.layer; i++) {
                    var data = {
                        layer: i,
                        id: "",
                        name: ""
                    };
                    menuData.push(data);
                }
                $.fn.chooseMenu.bindMenu(_this);
            });
        };

        $.fn.chooseMenu.default = {
            width: 400,
            data: [],
            layer: 3,
            isInit: false,/*是否初始化控件*/
            defaults: {
                headerText: "请选择",
                headerID: ""
            },
            onHeaderChange: function () {
            },
            onCategroyChange: function () {
            }
        };

        $.fn.chooseMenu.bindMenu = function (obj) {
            //绑定样式
            obj.css({
                "background": "url('/modules/images/ico-dropdown.png') no-repeat right 5px center"
            });

            obj.click(function () {
                var _this = $(this);
                var offset = obj.offset();
                if ($(".change-menu-body").length == 0) {
                    DoT.exec("../modules/plug/chooseordercategory/chooseordercategory.html", function (template) {
                        var innerText = template();
                        innerText = $(innerText);
                        innerText.css({
                            "width": options.width + "px",
                            "left": offset.left + "px",
                            "top": (offset.top + 27) + "px"
                        });
                        innerText.find('.close-layer').click(function () {
                            $(".change-menu-body").hide();
                        });
                        $('body').append(innerText);
                        bindObj(cacheData, obj, options.defaults);
                    });
                } else {
                    $(".change-menu-body").css({ "left": offset.left + "px", "top": (offset.top + 27) + "px" }).show();
                }
                return false;
            });
        };

        var bindObj = function (data, obj, _headerData) {
            /*头部数据处理*/
            if (_headerData.headerText) {
                $(".change-menu-header").find('li').removeClass('hover');
                $(".change-menu-header").find('li:last-child').html(_headerData.headerText);
                var _headerMenu = $('<li class="hand hover" data-id="' + _headerData.headerID + '">请选择</li>');
                $(".change-menu-header ul").append(_headerMenu);
                _headerMenu.click(function () {
                    if (!$(this).hasClass('hover')) {
                        $(this).siblings().removeClass('hover');
                        $(this).addClass('hover');
                        _headerMenu.nextAll().remove();

                        var headerData = {
                            headerText: '',
                            headerID: ''
                        };
                        $(this).html("请选择");
                        $(".change-menu-content").find('ul').empty();
                        var id = $(this).data("id");
                        var item = getCacheDataByID(id);
                        if (!item) {
                            $(".change-menu-content").append('<div class="nodata-txt">暂无分类信息！</div>');
                        } else {
                            $(".change-menu-content").find('.nodata-txt').remove();
                            bindObj(item, obj, headerData);
                            !options.onHeaderChange || options.onHeaderChange(CacheCategory[id] || "");
                        }
                    }
                });
            }

            /*子分类数据处理*/
            if (data) {
                for (var i = 0; i < data.length; i++) {
                    var item = data[i];
                    var _childMenu = $('<li class="hand categategory-item" data-layer="' + item.Layers + '" data-name="' + item.CategoryName + '" data-id="' + item.CategoryID + '">' + item.CategoryName + '</li>');
                    $(".change-menu-content").find('ul').append(_childMenu);
                    _childMenu.click(function () {
                        var _this = $(this);
                        var id = $(this).data("id");
                        if (_this.data('layer') < options.layer) {
                            var item = getCacheDataByID(id);
                            var headerData = {
                                headerText: _this.text(),
                                headerID: _this.data("id"),
                                headerLayer: _this.data("layer")
                            };
                            $(".change-menu-content").find('ul').empty();
                            if (!item) {
                                $(".change-menu-content").append('<div class="nodata-txt">暂无分类信息！</div>');
                                bindObj(item, obj, headerData);
                            } else {
                                $(".change-menu-content").find('.nodata-txt').remove();
                                bindObj(item, obj, headerData);
                            }
                        } else {
                            $(".change-menu-header").find('li:last-child').html(_this.text());
                            $(".change-menu-body").hide();
                        }

                        /*保存文本框中填入值的层级、id、name*/
                        var _layer = _this.data('layer');
                        menuData[_layer - 1].id = _this.data('id');
                        menuData[_layer - 1].name = _this.data('name');
                        for (var k = 0; k < menuData.length; k++) {
                            var itemMD = menuData[k];
                            if (itemMD.layer <= _layer) {
                                continue;
                            }
                            itemMD.id = '';
                            itemMD.name = '';
                        }

                        /*拼接文本框中的值*/
                        var _desc = "";
                        for (var j = 0; j < menuData.length; j++) {
                            var itemMD = menuData[j];
                            if (!itemMD.id) {
                                continue;
                            }
                            if (_desc) {
                                _desc += "/";
                            }
                            _desc += itemMD.name;
                        }
                        obj.val(_desc);
                        !options.onCategroyChange || options.onCategroyChange(_this.data('layer') == options.layer ? CacheCategory[id] : "");
                    });
                }
            }
        };

        //获取子分类
        var getCacheDataByID = function (id) {
            if (!id)
                return cacheData;
            if (CacheChildCategory[id]) {
                getCategoryCache(CacheChildCategory[id]);
                return CacheChildCategory[id];
            }
        };

        //获取分类缓存
        var getCategoryCache = function (obj) {
            var items = obj;
            for (var i = 0; i < items.length; i++) {
                if (!CacheCategory[items[i].CategoryID]) {
                    CacheCategory[items[i].CategoryID] = items[i];
                    for (var key in items) {
                        var model = items[key];
                        if (model.ChildCategory) {
                            CacheChildCategory[model.CategoryID] = model.ChildCategory;
                        }
                    }
                }
            }
        };

        $(document).click(function (e) {
            var ele = $(e.target);
            if (!ele.parents().hasClass('change-menu-body') && !ele.hasClass('change-menu-body')
                && !ele.parents().hasClass('change-menu-content')&& !ele.hasClass('categategory-item')) {
                $(".change-menu-body").hide();
            }
        });
    })(jQuery);
});