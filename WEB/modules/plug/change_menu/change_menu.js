define(function (require, exports, module) {
    require("plug/change_menu/change_menu.css");
    (function () {
        var menuData = [];
        $.fn.changeMenu = function (option) {
            return this.each(function () {
                var _this = $(this);
                var options = $.extend({}, $.fn.changeMenu.default, option);
                for (var i = 1; i <= options.layer; i++) {
                    var data = {
                        layer:  i,
                        id: "",
                        name: ""
                    }
                    menuData.push(data);
                }
                $.fn.changeMenu.bindMenu(_this, options);
            });
        };
        $.fn.changeMenu.default = {
            width: 400,
            data: "",
            layer: 3,
            className:"product-change",
            defaults: {
                headerText: "全部分类",
                headerID: ""
            },
            onChange: function () {
            }
        };
        $.fn.changeMenu.bindMenu = function (obj, opts) {
            $.post("/Products/GetChildCategorysByID", {
                categoryid: ''
            }, function (data) {
                obj.click(function () {
                    var _this = $(this);
                    var offset = obj.offset();
                    if ($(".change-menu-body").length == 0) {
                        var _closeMenu = $("<div title='关闭' class='close-layer iconfont right mRight10 hand color999'>&#xe606;</div>");
                        var _menuBody = $("<div class='change-menu-body' style='width:" + opts.width + "px;left:" + offset.left + "px;top:" + (offset.top + 27) + "px;'></div>"),
                            _menuHeader = $("<div class='change-menu-header'><ul></ul></div>"),
                            _menuContent = $("<div class='change-menu-content'><ul></ul></div>");
                        var _clearFloat = "<div class='clear'></div>";
                        _menuHeader.append(_closeMenu).append(_clearFloat);
                        _menuContent.append(_clearFloat);
                        _menuBody.append(_menuHeader).append(_menuContent);
                        $('body').append(_menuBody);
                        bindObj(data.Items, _menuContent, obj, _menuHeader, opts.defaults);
                        _closeMenu.click(function () {
                            _menuBody.hide();
                        });
                    } else {
                        $(".change-menu-body").show();
                    }
                    return false;
                });
            });
        };

        var bindObj = function (data, _contentEle, obj, _headerEle, _headerData) {
            /*头部数据处理*/
            if (_headerData.headerText) {
                _headerEle.find('li').removeClass('hover');
                var _headerMenu = $('<li class="hand hover" data-id="' + _headerData.headerID + '" data-layer="' + (_headerData.headerLayer || 0) + '" >' + _headerData.headerText + '</li>');
                _headerEle.find('ul').append(_headerMenu);
                _headerMenu.click(function () {
                    if (!$(this).hasClass('hover')) {
                        $(this).siblings().removeClass('hover');
                        $(this).addClass('hover');
                        var id = $(this).data("id");
                        $.post("/Products/GetChildCategorysByID", {
                            categoryid: id
                        }, function (callBackData) {
                            _headerMenu.nextAll().remove();
                            var headerData = {
                                headerText: '',
                                headerID: ''
                            };
                            _contentEle.find('ul').empty();
                            bindObj(callBackData.Items, _contentEle, obj, _headerEle, headerData);
                        });
                    }
                });
            }

            /*子菜单数据处理*/
            for (var i = 0; i < data.length; i++) {
                var item = data[i];
                var _childMenu = $('<li class="hand" data-layer="' + item.Layers + '" data-name="' + item.CategoryName + '" data-id="' + item.CategoryID + '">' + item.CategoryName + '</li>');

                _contentEle.find('ul').append(_childMenu);
                _childMenu.click(function () {
                    var _this = $(this);
                    if (_this.data('layer') < 3) {
                        $.post("/Products/GetChildCategorysByID", {
                            categoryid: _this.data("id")
                        }, function (callBackData) {
                            var headerData = {
                                headerText: _this.text(),
                                headerID: _this.data("id"),
                                headerLayer: _this.data("layer")
                            };
                            _contentEle.find('ul').empty();
                            bindObj(callBackData.Items, _contentEle, obj, _headerEle, headerData);
                        });
                    } else {
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
                });
            }
        };

        $(document).click(function (e) {
            var ele=$(e.target);
            if (!ele.parents().hasClass('change-menu-body')) {
                $(".change-menu-body").hide();
            }
        });
    })(jQuery);
});