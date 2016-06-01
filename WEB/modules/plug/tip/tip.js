define(function (require, exports, module) {
    require("plug/tip/style.css");

    (function ($) {
        

        $.fn.Tip = function (option) {
            var Option = {
                width: "",
                msg: ""
            }

            Option = $.extend([], Option, option);
            var _this = $(this);
            
            _this.mouseover(function (event) {
                var e = event || window.event;
                var tipDiv = createTip(Option);
                var height = tipDiv.find(".tip-msg").height();
                var thisWidth = _this.width();

                tipDiv.css({ "top": (e.clientY + document.body.scrollTop - height - 30) + "px", "left": (_this.offset().left - ((Option.width - thisWidth) / 2)) + "px" });
                tipDiv.find(".tip-lump").css({ "top": (height + 19) + "px", "left": (Option.width / 2 - 3) + "px" });
            }).mouseout(function () {
                $(".content-tip").remove();
            });

        }

        var createTip = function (Option) {
            if ($(".content-tip").length == 1) {
                $(".content-tip").find(".tip-msg").html(Option.msg);

                return $(".content-tip");
            }
            else {
                var html = '<div class="content-tip">';
                html += '<div class="tip-msg" style="width:' + Option.width + 'px">' + Option.msg + '</div>';
                html += '<div class="tip-lump"></div>';
                html += '</div>';
                html = $(html);
                $("body").append(html);
                return html;
            }
        }
        
    })(jQuery)

    module.exports = jQuery;
});