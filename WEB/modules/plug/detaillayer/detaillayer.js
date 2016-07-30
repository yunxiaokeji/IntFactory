define(function (require, exports, module) {
    var doT = require("dot");
    require("plug/detaillayer/detaillayer.css");
    var PlugJS = function (option) {
        var _this = this;
        _this.setting = $.fn.extend({}, _this.default, option);
        _this.init();
    };

    PlugJS.close = function () {
        $(".confim-dialog").remove();
    };

    PlugJS.prototype.default = {
        width: 400,
        content: "",
        title: "单据明细",
        close:PlugJS.prototype.close,
        yesFn: function () {

        }
    };
    PlugJS.prototype.init = function () {
        var _this = this;
        doT.exec("plug/detaillayer/detaillayer.html", function (template) {
            var innerHtml = template();
            innerHtml = $(innerHtml);
            innerHtml.find('.confim-title').html(_this.setting.title);
            innerHtml.find('.confim-body').append($(_this.setting.content));
            innerHtml.find('#btnSubmit').click(function () {
                _this.setting.yesFn && _this.setting.yesFn();
            });
            innerHtml.find('#btnClose').click(function () {
                PlugJS.close();
            });
            $('body').append(innerHtml);
            var height = ($(window).height() - $('.confim-main').height()) / 2;
            $('.confim-main').css("margin-top", height);
        });
    };

    $(document).click(function (e) {
        if (!$(e.target).parents().hasClass('confim-main') &&
            !$(e.target).parents().hasClass('confim-title') &&
            !$(e.target).parents().hasClass('confim-body') &&
            !$(e.target).parents().hasClass('confim-footer') &&
            !$(e.target).parents().hasClass('confim-dialog')
            ) {
            $(".confim-dialog").remove();
        }
    });

    exports.close = function () {
        return PlugJS.close();
    };

    exports.create = function (option) {
        return new PlugJS(option);
    };
});