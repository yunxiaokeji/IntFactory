define(function (require, exports, module) {
    require("plug/tip_layer/tip_layer.css");
    (function () {
        $.fn.showTipLayer = function (options) {
            return this.each(function () {
                var opts = $.extend({}, $.fn.showTipLayer.defaults, options);
                var _this = $(this);
                $.fn.showMsg(_this, opts);
            });
        };
        $.fn.showTipLayer.defaults = {
            content: "",
            isposition: false,
            isAutoClose: true,
            closeTime: 5,
            zIndex: 1,
            callback: function () {

            }
        };
        $.fn.showMsg = function (obj, opts) {
            if (!obj.data('istip')) {
                obj.data('istip', 1);
                var _alertLayer = $('<div class="error-alert-layer hide"></div>');
                _alertContent = $('<span class="error-alert-content">' + opts.content + '</span>');
                _alertLump = $('<div class="error-alert-lump"></div>');
                _alertLayer.append(_alertContent).append(_alertLump);
                $(obj).after(_alertLayer);
                
                var offset = obj.offset();
                if (opts.isposition) {
                    offset = obj.position();
                }

                _alertLayer.css({
                    "left": (offset.left + obj.width() / 2 - _alertLayer.width()/2) + "px",
                    "top": (offset.top - 29) + "px",
                    "z-index": opts.zIndex
                });

                _alertLump.css({
                    "left": _alertLayer.width() / 2 + "px"
                });
                _alertLayer.fadeIn(700);
                _alertLayer.click(function () {
                    _alertLayer.fadeOut(700, function () {
                        _alertLayer.remove();
                        obj.data('istip', 0);
                    });
                });
                if (opts.isAutoClose) {
                    setTimeout(function () {
                        _alertLayer.fadeOut(700, function () {
                            _alertLayer.remove();
                            obj.data('istip', 0);
                        });
                    }, opts.closeTime * 1000);
                }
            }
        };
    })();
});