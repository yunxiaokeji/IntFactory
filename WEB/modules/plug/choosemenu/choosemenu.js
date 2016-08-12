define(function (require, exports, module) {
    require("plug/choosemenu/choosemenu.css");
    (function () {
        var menuData = [];
        var cacheData = [];
        var options;
        var _eleHeader;
        var _menuContent;

        $.fn.chooseMenu = function (option) {
            return this.each(function () {
                var _this = $(this);
                options = $.extend({}, $.fn.chooseMenu.default, option);
                if (options.isInit) {
                    $(".change-menu-body").remove();
                    menuData = [];
                }
                for (var i = 1; i <= options.layer; i++) {
                    var data = {
                        layer:  i,
                        id: "",
                        name: ""
                    }
                    menuData.push(data);
                }
                $.fn.chooseMenu.bindMenu(_this);
            });
        };

        $.fn.chooseMenu.default = {
            width: 400,
            data: "",
            layer: 3,
            isInit:false,/*是否初始化控件*/
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

            $.post("/Products/GetChildCategorysByID", {
                categoryid: ''
            }, function (data) {
                obj.click(function () {
                    var _this = $(this);
                    var offset = obj.offset();
                    if ($(".change-menu-body").length == 0) {
                        var _closeMenu = $("<div title='关闭' class='close-layer iconfont right mRight10 hand color999'>&#xe606;</div>");
                        var _menuBody = $("<div class='change-menu-body' style='width:" + options.width + "px;left:" + offset.left + "px;top:" + (offset.top + 27) + "px;'></div>");
                        _eleHeader = $("<div class='change-menu-header'><ul></ul></div>");
                        _menuContent = $("<div class='change-menu-content'><ul></ul></div>");
                        var _clearFloat = "<div class='clear'></div>";
                        _eleHeader.append(_closeMenu).append(_clearFloat);
                        _menuContent.append(_clearFloat);
                        _menuBody.append(_eleHeader).append(_menuContent);
                        $('body').append(_menuBody);
                        bindObj(data.Items, obj, options.defaults);

                        _closeMenu.click(function () {
                            _menuBody.hide();
                        });
                    } else {
                        $(".change-menu-body").css({ "left": offset.left + "px", "top": (offset.top + 27) + "px" }).show();
                    }
                    return false;
                });
            });
        };

        var bindObj = function (data, obj, _headerData) {
            /*头部数据处理*/
            if (_headerData.headerText) {
                _eleHeader.find('li').removeClass('hover');
                _eleHeader.find('li:last-child').html(_headerData.headerText);
                var _headerMenu = $('<li class="hand hover" data-id="' + _headerData.headerID + '" data-layer="' + (_headerData.headerLayer || 0) + '" >请选择</li>');
                _eleHeader.find('ul').append(_headerMenu);
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
                        _menuContent.find('ul').empty();
                        var id = $(this).data("id");
                        var item = cacheData[id];
                        if (!item) {
                            _menuContent.append('<div class="data-loading"></div>');
                            $.post("/Products/GetChildCategorysByID", {
                                categoryid: id
                            }, function (callBackData) {
                                _menuContent.find('.data-loading').remove();
                                cacheData[id] = callBackData.Items;
                                bindObj(callBackData.Items, obj, headerData);
                                !options.onHeaderChange || options.onHeaderChange(menuData);
                            });
                        } else {
                            bindObj(item, obj, headerData);
                            !options.onHeaderChange || options.onHeaderChange(menuData);
                        }
                    }
                });
            }

            /*子分类数据处理*/
            for (var i = 0; i < data.length; i++) {
                var item = data[i];
                var _childMenu = $('<li class="hand gategory-item" data-layer="' + item.Layers + '" data-name="' + item.CategoryName + '" data-id="' + item.CategoryID + '">' + item.CategoryName + '</li>');
                _menuContent.find('ul').append(_childMenu);
                _childMenu.click(function () {
                    var _this = $(this);
                    if (_this.data('layer') < 3) {
                        var id = $(this).data("id");
                        var item = cacheData[id];
                        var headerData = {
                            headerText: _this.text(),
                            headerID: _this.data("id"),
                            headerLayer: _this.data("layer")
                        };
                        _menuContent.find('ul').empty();
                        if (!item) {
                            _menuContent.append('<div class="data-loading"></div>');
                            $.post("/Products/GetChildCategorysByID", {
                                categoryid: _this.data("id")
                            }, function (callBackData) {
                                _menuContent.find('.data-loading').remove();
                                cacheData[id] = callBackData.Items;
                                bindObj(callBackData.Items, obj, headerData);
                            });
                        } else {
                            bindObj(item, obj, headerData);
                        }
                    } else {
                        _eleHeader.find('li:last-child').html(_this.text());
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
                    !options.onCategroyChange || options.onCategroyChange(menuData);
                });
            }
        };

        $(document).click(function (e) {
            var ele = $(e.target);
            if (!ele.parents().hasClass('change-menu-body') && !ele.hasClass('change-menu-body')
                && !ele.parents().hasClass('change-menu-content')&& !ele.hasClass('gategory-item')) {
                $(".change-menu-body").hide();
            }
        });
    })(jQuery);
});