define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Easydialog = require("easydialog");

    var ObjectJS = {};
    ObjectJS.isLoading = true;
    ObjectJS.orderID = "";

    ObjectJS.init = function (orderid) {
        ObjectJS.bindEvent();
        ObjectJS.orderID = orderid;
    };

    ObjectJS.bindEvent = function () {
        $("#setPriceRange").click(function () {
            var _this = $(this);
            if (_this.data('isget')==1) {
                _this.data('isget', 0);
                $(".pirce-range-box").fadeOut();
                _this.text('展开报价');
            } else {
                _this.data('isget', 1);
                _this.text('收起报价');
                $(".pirce-range-box").fadeIn();
                ObjectJS.getPriceRange();
            }
        });   
    };

    ObjectJS.getPriceRange = function () {
        ObjectJS.isLoading = false;
        $(".pirce-range-box").html('<div class="data-loading"><div>');
        Global.post("/Orders/GetOrderPriceRanges", { orderid: ObjectJS.orderID }, function (data) {
            doT.exec("template/orders/pricerangge.html", function (template) {
                var innerText = template(data.items);
                innerText = $(innerText);
                innerText.find(".close-range").click(function () {
                    $(".pirce-range-box").fadeOut();
                    $("#setPriceRange").data('isget', 0);
                    $("#setPriceRange").text('展开报价');
                });
                innerText.find('.add-range').click(function () {
                    if (!ObjectJS.isLoading) {
                        alert("数据提交中，请稍等");
                        return;
                    }
                    if ($(".center-range li:last").data("rangeid") == "") {
                        alert("请保存后再试.");
                        return;
                    }
                    $(".center-range .nodata-txt").remove();
                    doT.exec("template/orders/addpricerange.html", function (template) {
                        var innerHtml = template({});
                        innerHtml = $(innerHtml);
                        innerHtml.find(".cancel-price-range").click(function () {
                            $(this).parent().parent().remove();
                            $(".center-range li:last").find(".max-number").val('无上限');
                        });
                        var minNumber = 0;
                        if ($(".center-range li").length > 0) {
                            minNumber = $(".center-range li:last").find(".min-number").val() * 1;
                            $(".center-range li:last").find(".max-number").val(minNumber);
                        }
                        $(".center-range").append(innerHtml);
                        $(".center-range li:last").find(".min-number").val(minNumber + 1);

                        ObjectJS.bindUpdatePriceRange(innerHtml.find(".update,.save-price-range"));
                        ObjectJS.deletePriceRange();
                    });
                });

                $(".pirce-range-box").html(innerText);
                ObjectJS.bindUpdatePriceRange(innerText.find(".update"));
                ObjectJS.deletePriceRange();
            });
            ObjectJS.isLoading = true;
        });
    }
       
    ObjectJS.bindUpdatePriceRange = function (obj) {        
        $(".min-number").change(function () {
            var _this = $(this);
            if (!_this.val().isDouble()) {
                _this.val(_this.data('num'));
                return false;
            }
            var prevObj = _this.parent().parent().prev();
            if (prevObj.find('.min-number').val() && (_this.val() * 1 <= prevObj.find('.min-number').val() * 1)) {
                alert("数量必须大于上一区间启始值");
                _this.val(_this.data('num'));
            } else {
                //prevObj.find('.max-number').val((_this.val() * 1 - 1));
            }
        });

        $(".price").change(function () {
            var pri = $(this).val();
            if (!pri.isMoneyNumber()) {
                $(this).val($(this).data("num"));                
            }
        });
        
        obj.click(function () {   
            var _this = $(this).parent().parent();
            var rangeid = _this.data("rangeid");
            var minNumber = _this.find(".min-number").val().trim();
            var maxNumber = _this.find(".max-number").val().trim();
            var price = _this.find(".price").val().trim();
            
            if (minNumber == "" ||maxNumber=="") {
                alert("数字不能为空");
                return false;
            }
            if (price == "") {
                alert("价格不能为空");
                return false;
            }            
            var model = {
                RangeID: rangeid,
                MinQuantity: minNumber,
                Price: price,
                OrderID: ObjectJS.orderID
            };
            Global.post("/Orders/SavePriceRange", {model: JSON.stringify(model)}, function (obj) {
                if (obj.id) {
                    if (obj.id == "1") {
                        _this.find(".min-number").val(minNumber).data("num", minNumber);
                        _this.find(".price").val(price).data("num", price);
                        _this.prev().find(".max-number").val(Number(minNumber) - 1).data("num", Number(minNumber) - 1);
                    } else {
                        _this.data("rangeid", obj.id);
                        _this.find(".min-number").val(minNumber).data("num", minNumber);
                        _this.prev().find(".max-number").val(Number(minNumber) - 1).data("num", Number(minNumber) - 1);
                        _this.find(".price").val(price).data("num", price);
                        _this.find(".max-number").val(maxNumber).data("num", maxNumber).attr("disabled", "disabled");

                        $(".save-price-range,.cancel-price-range").hide();
                        $(".update,.delete").show();
                    }
                } else {
                    alert("网络连接失败");
                }
            });    
        });
    };

    ObjectJS.deletePriceRange = function () {
        $(".delete").unbind().click(function () {            
            var _this = $(this).parent().parent();
            var rangeid = _this.data("rangeid");
            var confirmMsg = "确定删除此价格区间?";            
            confirm(confirmMsg, function () {
                Global.post("/Orders/DeleteOrderPriceRange", { rangeid: rangeid }, function (data) {
                    if (data.status > 0) {
                        _this.fadeOut(400, function () {
                            _this.remove();
                        });                        
                        var nextNum = _this.next().find(".min-number").val();   
                        var numb = _this.find(".max-number").val();
                        if (numb == "无上限") {
                            _this.prev().find(".max-number").val("无上限");
                        } else {
                            _this.prev().find(".max-number").val(Number(nextNum) - 1);
                        }
                    } else {
                        alert("删除失败");
                    }
                })
            });
        });
    }

    module.exports = ObjectJS;
})