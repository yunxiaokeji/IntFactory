define(function (require, exports, module) {
    require("plug/tip/style.css");

    (function ($) {
        var Option = {
            width: "",
            msg:""
        }

        $.fn.Tip = function (option) {
            Option = $.extend([], Option, option);
            var _this = $(this);
            
            _this.mouseover(function (event) {
                
                $(".content-tip").remove();
                var e = event || window.event;
                var tipDiv = createTip();
                tipDiv = $(tipDiv);
                $("body").append(tipDiv);

                var height = tipDiv.find(".tip-msg").height();
                tipDiv.css({ "top": (e.clientY + document.body.scrollTop - height - 30) + "px", "left": (e.clientX - Option.width / 2) + "px" });
                tipDiv.find(".tip-lump").css({ "top": (height+ 19) + "px", "left": (Option.width / 2 - 3) + "px" });
            });

        }

        var createTip = function () {
            var html = '<div class="content-tip">';
            html += '<div class="tip-msg" style="width:'+Option.width+'px">' + Option.msg + '</div>';
            html += '<div class="tip-lump"></div>';
            html += '</div>';
            return html;
        }
        
    })(jQuery)

    module.exports = jQuery;
});