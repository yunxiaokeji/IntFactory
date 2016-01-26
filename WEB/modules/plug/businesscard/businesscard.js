
/* 
作者：Allen
日期：2014-11-15
示例:
    $(...).searchKeys(callback);
*/

define(function (require, exports, module) {
    require("plug/businesscard/style.css");
    (function ($) {

        var Defaults = {
            element: "#"
        };

        var fadeInTimer = null;
        var fadeOutTimer = null;

        $.fn.businessCard = function (options) {
            $(this).each(function () {
                drawBusinessCard($(this));
            });

            $(document).click(function (e) {
                //隐藏下拉
                if (!$(e.target).parents().hasClass("businessCard")) {
                    $(".businessCard").hide();
                }
            });
        }

        var drawBusinessCard = function (obj) {
            var id = obj.data("id");
            var left = obj.offset().left-24;
            var top = obj.offset().top - 160;

            obj.unbind("mouseenter").mouseenter(function () {
                clearTimeout(fadeInTimer);
                clearTimeout(fadeOutTimer);
                $(".businessCard").hide();

                
                var $BusinessCardObj=$("#businessCard_"+id);
                if ($BusinessCardObj.length == 0) {

                    $.get("/Activity/GetUserDetail", { id: id }, function (data) {
                        var item = data.Item;
                        var _businessCardHtml = '<div class="businessCard" id="businessCard_' + item.UserID + '">';
                        _businessCardHtml += '<ul>';
                        _businessCardHtml += '<li style="height:45px;">';
                        var Avatar=item.Avatar;
                        if(Avatar=='')
                            Avatar = "/modules/images/defaultavatar.png";
                        _businessCardHtml += '<div class="left"><img class="userAvatar" src="' + Avatar + '" /></div>';
                        _businessCardHtml += '<div class="left mLeft10">' + item.Name + '</div>';
                        _businessCardHtml += '<div class="clear"></div>';
                        _businessCardHtml += '</li>';
                        _businessCardHtml += '<li>';
                        if (item.Department)
                            _businessCardHtml += item.Department.Name;
                        _businessCardHtml += '</li>';
                        _businessCardHtml += '<li>';
                        if (item.Role)
                            _businessCardHtml += item.Role.Name;
                        _businessCardHtml += '</li>';
                        _businessCardHtml += '<li>';
                        _businessCardHtml += item.MobilePhone;
                        _businessCardHtml += '</li>';
                        _businessCardHtml += '</ul>';
                        _businessCardHtml += '<span class="userArrowBox">';
                        _businessCardHtml += '<span class="userArrow"></span>';
                        _businessCardHtml += '<span class="userArrow2"></span>';
                        _businessCardHtml += '</span>';
                        _businessCardHtml += '</div>';
                        _businessCardHtml = $(_businessCardHtml);

                        _businessCardHtml.unbind("mouseenter").mouseenter(function () {
                            clearTimeout(fadeOutTimer);
                        });
                        _businessCardHtml.unbind("mouseleave").mouseleave(function () {
                            clearTimeout(fadeInTimer);
                            $(this).fadeOut();
                        });

                       
                        _businessCardHtml.css({ "left": left + "px", "top": top + "px" });  
                        fadeInTimer = setTimeout(function () { _businessCardHtml.fadeIn(); }, 50);

                        $("body").append(_businessCardHtml);
                        $BusinessCardObj = $("#businessCard_" + id);
                    });
                }
                else {

                    $BusinessCardObj.css({ "left": left + "px", "top": top + "px" });
                    fadeInTimer = setTimeout(function () { $BusinessCardObj.fadeIn(); }, 50);
                }
            });

            obj.unbind("mouseleave").mouseleave(function () {
                clearTimeout(fadeInTimer);

                var $BusinessCardObj = $("#businessCard_" + id);
                fadeOutTimer = setTimeout(function () { $BusinessCardObj.fadeOut(); }, 50);
            });


        }

    })(jQuery)
    module.exports = jQuery;
});