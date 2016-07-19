define(function (require, exports, module) {
    require("plug/change_menu/change_menu.css");
    (function () {
        $.fn.changeMenu = function (option) {
            return this.each(function () {
                var _this = $(this);
                var options = $.extend({}, $.fn.changeMenu.default, option);
                $.fn.changeMenu.bindMenu(_this, options);
            });
        };
        $.fn.changeMenu.default = {
            width: 400,
            data: "",
            layer:3,
            onChange: function () {
            }
        };
        $.fn.changeMenu.bindMenu = function (obj, opts) {
            var title = {
                bigMenu: "全部分类",
                menus:
                    [
                        { Name: "面料", ID: "213a" },
                        { Name: "辅料", ID: "FL" },
                        { Name: "工艺", ID: "GY" }
                    ]
            };
            obj.click(function () {
                var _this = $(this);
                if ($(".change-menu-body").length == 0) {
                    var _menuBody = $("<div class='change-menu-body' style='width:"+opts.width+"px;'></div>"),
                        _menuHeader = $("<div class='change-menu-header'><ul></ul></div>"),
                        _menuContent = $("<div class='change-menu-content'><ul></ul></div>");
                    var _clearFloat = "<div class='clear'></div>";
                    _menuHeader.append(_clearFloat);
                    _menuContent.append(_clearFloat);
                    _menuHeader.find('ul').append('<li class="hover hand">' + title.bigMenu + '</li>');
                    bindObj(title,_menuContent);
                    _menuBody.append(_menuHeader).append(_menuContent);
                    _this.after(_menuBody);
                    opts.onChange && opts.onChange();
                } else {
                    $(".change-menu-body").show();
                }
            });
        };

        var bindObj = function (data, ele) {
            for (var i = 0; i < data.menus.length; i++) {
                var item = data.menus[i];
                var _childMenu = $('<li class="hand" data-layer="2" data-id="' + item.ID + '">' + item.Name + '</li>');
                ele.find('ul').append(_childMenu);
                _childMenu.click(function () {
                    var _this = $(this);
                    $.post("/Products/GetChildCategorysByID", {
                        categoryid: _this.data("id")
                    }, function (callBackData) {
                        console.log(callBackData);
                    });
                });
            }
        };

    })(jQuery);
});