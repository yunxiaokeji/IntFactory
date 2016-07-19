define(function (require,exports,module) {
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
            data:"",
            onChange: function () {
            }
        };
        $.fn.changeMenu.bindMenu = function (obj, opt) {
            var title = {

            };
            obj.click(function () {
                var _this = $(this);
                var _menuBody = $("<div class='change-menu-body'></div>"),
                    _menuHeader = $("<div class='change-menu-header'></div>"),
                    _menuContent = $("<div class='change-menu-content'></div>");
                _menuBody.append(_menuHeader).append(_menuContent);
                _this.after(_menuBody);
                opt.onChange && opt.onChange();
            });
        };
    })(jQuery);
});