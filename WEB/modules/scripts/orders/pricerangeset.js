define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Easydialog = require("easydialog");

    var ObjectJS = {};
    ObjectJS.isLoading = true;

    ObjectJS.init = function (orderid) {
        ObjectJS.bindEvent(orderid);
    };

    ObjectJS.bindEvent = function (orderid) {

        $(".price-range-set").click(function () {            
            $(".center-range").empty();
            $("#bfe_overlay").show();
            $(".price-range").show();
            ObjectJS.getPriceRange(orderid);
        });

        $(".add-price-range").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            $(".no-data").remove();
            doT.exec("template/orders/addpricerange.html", function (template) {
                var innerText = template({});                
                innerText = $(innerText);

                var num = $(".center-range li:last").find(".min-number").val();
                innerText.find(".min-number").val(Number(num)+1);
                $(".center-range").append(innerText);
                
                ObjectJS.updateAndAddPriceRange(innerText, ".save-price-range", orderid);

                ObjectJS.updateAndAddPriceRange(innerText, ".update", orderid);

                ObjectJS.deletePriceRange(innerText);

                innerText.find(".cancel-price-range").click(function () {
                    $(this).parent().parent().remove();
                });                
            });
        });

        $(".close_btn,#bfe_overlay").click(function () {
            $("#bfe_overlay").hide();
            $(".price-range").hide();
        });
    };

    ObjectJS.getPriceRange = function (orderid) {
        ObjectJS.isLoading = false;
        $(".center-range").append('<div class="data-loading"><div>');
        Global.post("/Orders/GetOrderPriceRanges", { orderid: orderid }, function (data) {
            $(".data-loading").remove();
            if (data.items.length>0) {
                doT.exec("template/orders/pricerangge.html", function (template) {
                    var innerText = template(data.items);
                    innerText = $(innerText);
                    $(".center-range").append(innerText);    

                    ObjectJS.updateAndAddPriceRange(innerText, ".update", orderid);

                    ObjectJS.deletePriceRange(innerText);
                });
            } else {
                $(".center-range").append('<div class="center no-data mTop50">暂无数据</div>');
            }
            ObjectJS.isLoading = true;
        })
    }
    
    ObjectJS.updateAndAddPriceRange = function (innerText,save,orderid) {
        innerText.find(save).click(function () {
            var _this = $(this).parent().parent();
            
            var rangeid = _this.data("rangeid");
            
            if (!_this.data("id")=="0") {
                var beforenumber = _this.prev().find(".min-number").val().trim();
            } 
            var minnumber = _this.find(".min-number").val().trim();
            var maxnumber = _this.find(".max-number").val().trim();
            var price = _this.find(".price").val().trim();

            if (!minnumber.isDouble() || !minnumber.isInt() || Number(minnumber) <= 0) {
                alert("数量不正确");
                return;
            }
            if (!price.isDouble() || price < 0) {
                alert("价格不正确");
                return;
            }            
            if (Number(beforenumber) >= Number(minnumber)) {
                alert("数量不能小于等于其最小数量");
                return;
            }           
            if (Number(minnumber)>=Number(maxnumber)) {
                alert("数量不能大于等于其最大数量");
                return;
            }

            var model = {
                RangeID: rangeid,
                MinQuantity: minnumber,
                Price: price,
                OrderID: orderid
            };               
            Global.post("/Orders/OrderPriceRange", {
                model: JSON.stringify(model)
            }, function (obj) {               
                if (obj.id!=""||obj.id!="2") {
                    if (obj.id=="1") {
                        _this.find(".min-number").val(minnumber);
                        _this.find(".price").val(price);
                        _this.prev().find(".max-number").val(minnumber);
                        alert("编辑成功");                        
                    } else {
                        _this.data("rangeid", obj.id);                        
                        _this.find(".min-number").val(minnumber);
                        _this.find(".price").val(price);
                        _this.find(".max-number").val("无上限").attr("disabled", "disabled");
                        _this.prev().find(".max-number").val(minnumber);

                        $(".save-price-range,.cancel-price-range").hide();
                        $(".update,.delete").show();
                        
                        alert("添加成功");
                    }
                } else {
                    alert("添加失败");
                }
            });            
        });
    };

    ObjectJS.deletePriceRange = function (innerText) {
        innerText.find(".delete").click(function () {
            var _this = $(this).parent().parent();
            var rangeid = _this.data("rangeid");

            var confirmMsg = "确定删除此价格区间?";
            confirm(confirmMsg, function () {
                Global.post("/Orders/DeleteOrderPriceRange", { rangeid: rangeid }, function (data) {
                    if (data.status > 0) {
                        _this.fadeOut(400, function () {
                            _this.remove();
                        });
                        var numb = _this.find(".max-number").val();
                        if (numb == "无上限") {
                            _this.prev().find(".max-number").val("无上限");
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