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
                        "layer": "layer" + i
                        
                    };

                }
                $.fn.changeMenu.bindMenu(_this, options);
            });
        };
        $.fn.changeMenu.default = {
            width: 400,
            data: "",
            layer: 3,
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
                    if ($(".change-menu-body").length == 0) {
                        var _menuBody = $("<div class='change-menu-body' style='width:" + opts.width + "px;'></div>"),
                            _menuHeader = $("<div class='change-menu-header'><ul></ul></div>"),
                            _menuContent = $("<div class='change-menu-content'><ul></ul></div>");
                        var _clearFloat = "<div class='clear'></div>";
                        _menuHeader.append(_clearFloat);
                        _menuContent.append(_clearFloat);
                        _menuBody.append(_menuHeader).append(_menuContent);
                        _this.after(_menuBody);
                        bindObj(data.Items, _menuContent, obj, _menuHeader, opts.defaults);
                        opts.onChange && opts.onChange();
                    } else {
                        $(".change-menu-body").show();
                    }
                });
            });

        };

        var bindObj = function (data, _contentEle, obj, _headerEle, _headerData) {
            if (_headerData.headerText) {
                _headerEle.find('li').removeClass('hover');
                var _headerMenu = $('<li class="hand hover" data-id=' + _headerData.headerID + '>' + _headerData.headerText + '</li>');
                _headerEle.find('ul').append(_headerMenu);
                _headerMenu.click(function () {
                    if (!$(this).hasClass('hover')) {
                        $(this).siblings().removeClass('hover');
                        $(this).addClass('hover');
                        var id = $(this).data("id");
                        if (id) {
                            obj.data('layer1', '');
                            obj.data('layer2', '');
                            obj.data('layer3', '');
                        }
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
                                headerID: _this.data("id")
                            };
                            _contentEle.find('ul').empty();
                            bindObj(callBackData.Items, _contentEle, obj, _headerEle, headerData);
                        });
                    }

                    if (_this.data('id')) {
                        var _layer = 'layer' + _this.data('layer');
                        obj.data(_layer, _this.data('id'));
                        var val = (obj.val() || '');
                        if (_this.data('layer') > 1) {
                            val += '/';
                        }
                        val += _this.data('name');
                        obj.val(val);
                    }
                });
            }
        };
    })(jQuery);
});