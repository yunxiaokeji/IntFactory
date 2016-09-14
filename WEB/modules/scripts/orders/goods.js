define(function (require, exports, module) {
    var Global = require("global");
    var PricerangeSet = require("/modules/scripts/orders/pricerangeset.js");
    var ObjectJS = {};
    ObjectJS.init = function (orderid,goodsid, isPublic) {
        ObjectJS.goodsid = goodsid;
        ObjectJS.bindEvent();
        PricerangeSet.init(orderid);
        $("#navShare .ico-radiobox[data-value='" + isPublic + "']").addClass("hover");
    };

    ObjectJS.bindEvent = function () {
        $(".module-tab li").click(function () {
            var _self = $(this);
            var id = _self.data("id");
            if (!_self.hasClass("hover")) {
                _self.addClass("hover").siblings().removeClass("hover");

                $(".nav-partdiv").hide();
                $("#" + id).show();
                if (id == "navGoodsPrice") {
                    if (_self.data('isget') != 1) {
                        _self.data('isget', 1);
                        PricerangeSet.getPriceRange($("#goodsPrices"));
                    }
                }
            }
        });

        $("#navShare .ico-radiobox").click(function () {
            var _self = $(this);
            $("#navShare .ico-radiobox").removeClass("hover");
            if(_self.hasClass("hove")){
                _self.removeClass("hover");
            }else{
                _self.addClass("hover");
            }
            
        });

        $("#btnSaveShareInfo").click(function () {
            var hovers = $("#navShare .radiobox .hover");
            if (hovers.length != 1) {
                alert("请勾选保存项",2);
                return;
            }
            var publicStatus = hovers.eq(0).data("value");
            $("#btnSaveShareInfo").val("保存中...");
            Global.post("/orders/UpdateGoodsPublicStatus", {
                goodsid: ObjectJS.goodsid,
                publicStatus: publicStatus
            }, function (data) {
                $("#btnSaveShareInfo").val("保存");
                if (data.status) {
                    alert("保存成功");
                }
            });
        });
    };


    module.exports = ObjectJS;
});