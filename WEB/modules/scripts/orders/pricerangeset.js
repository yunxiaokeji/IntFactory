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
        $(".price-range-set").click(function () {   
            ObjectJS.getPriceRange();
        });   
    };

    ObjectJS.getPriceRange = function () {
        ObjectJS.isLoading = false;
        $(".center-range").append('<div class="data-loading"><div>');
        Global.post("/Orders/GetOrderPriceRanges", { orderid: ObjectJS.orderID }, function (data) {
            $(".data-loading").remove();
                doT.exec("template/orders/pricerangge.html", function (template) {
                    var innerText = template(data.items);
                    Easydialog.open({
                        container: {
                            id: "show-model-detail",
                            header: "优惠设置",
                            content: innerText
                        }
                    });
                    
                    ObjectJS.addPriceRange();
                    ObjectJS.bindUpdatePriceRange(".update");
                    ObjectJS.deletePriceRange();
                });
            ObjectJS.isLoading = true;
        })
    }

    ObjectJS.addPriceRange = function () {
        $(".add-range").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            $(".no-data").remove();
            doT.exec("template/orders/addpricerange.html", function (template) {
                var innerText = template({});                
                var minNumber = 0;
                if ($(".center-range li").length > 0) {
                    minNumber = $(".center-range li:last").find(".min-number").val();
                }
                
                $(".center-range").append(innerText);
                $(".center-range li:last").find(".min-number").val(Number(minNumber) + 1);
                
                ObjectJS.bindUpdatePriceRange(".update");

                ObjectJS.bindUpdatePriceRange(".save-price-range");

                ObjectJS.deletePriceRange();                              

                $(".cancel-price-range").click(function () {
                    $(this).parent().parent().remove();
                });
            });
        });
    }   
       
    ObjectJS.bindUpdatePriceRange = function (save) {
        
        $(".min-number").change(function () {
            ObjectJS.validateData(this, ".min-number");
        });

        $(".max-number").change(function () {
            ObjectJS.validateData(this, ".max-number");
        });

        $(".price").change(function () {
            var pri = $(this).val();
            if (!pri.isDouble() || Number(pri) < 0) {
                alert("价格格式不正确");
                $(this).val($(this).data("num"));
                return;
            }
        });
        
        $(save).click(function () {   
            var _this = $(this).parent().parent();
            var rangeid = _this.data("rangeid");
            var minNumber = _this.find(".min-number").val().trim();
            var maxNumber = _this.find(".max-number").val().trim();
            var price = _this.find(".price").val().trim();
            
            if (minNumber == "" || maxNumber == "" || price == "") {
                alert("数字不能为空");
                return false;
            }
            
            var quantity = maxNumber;
            if ($(".center-range li").length == 1 || save == ".update") {
                quantity = minNumber;
            }
            var model = {
                RangeID: rangeid,
                MinQuantity: quantity,
                Price: price,
                OrderID: ObjectJS.orderID
            };

            Global.post("/Orders/SavePriceRange", {
                model: JSON.stringify(model)
            }, function (obj) {
                if (obj.id != "") {
                    if (obj.id == "1") {
                        _this.find(".min-number").val(minNumber).data("num", minNumber);
                        _this.find(".price").val(price).data("num", price);
                        _this.prev().find(".max-number").val(minNumber).data("num", minNumber);                            
                        alert("编辑成功");
                    } else {
                        _this.data("rangeid", obj.id);
                        if ($(".center-range li").length == 1) {
                            _this.find(".min-number").val(minNumber).data("num", minNumber);
                        } else {
                            _this.find(".min-number").val(maxNumber).data("num", maxNumber);
                        }                            
                        _this.find(".price").val(price).data("num", price);
                        _this.find(".max-number").val("无上限").attr("disabled", "disabled");
                        _this.prev().find(".max-number").val(maxNumber).data("num", maxNumber);
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

    ObjectJS.validateData = function (obj,save) {        
        var _parentlist = $(obj).parent().parent();
        var beforenum = 0;
        if (!_parentlist.data("id") == "0") {
            beforenum = _parentlist.prev().find(".min-number").val();
        }
                
        if (save==".update") {
            num = _parentlist.find(save).val();
        } else {
            num = _parentlist.find(save).val();
        }
        
        var afternum = _parentlist.nextAll().find(".min-number").val();        
        if (!num.isDouble() || !num.isInt() || Number(num) <= 0) {
            alert("数量格式不正确");
            $(obj).val($(obj).data("num"));
            return false;
        }

        if (Number(beforenum) >= Number(num)) {
            alert("不能小于其上一个数量");
            $(obj).val($(obj).data("num"));
            return false;
        }

        if (Number(num) >= Number(afternum)) {
            alert("不能大于其下一个数量");
            $(obj).val($(obj).data("num"));
            return false;
        }
    }

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