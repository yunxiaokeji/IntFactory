define(function (require,exports,module) {
    var Global = require("global");
    var Objects = {};

    Objects.init = function (plate, img, orderid, OriginalID,ordertype) {
        Objects.bindEvent(plate,img);
        Objects.removeTaskPlateOperate();
        Objects.getAmount();
        Objects.imgOrderTable();
        Objects.processPlate(orderid, OriginalID, ordertype);
    };

    Objects.bindEvent = function (plate,img) {
        if (plate == "") {
            $("#Platemak").html('<tr><td class="no-border" style="width:954px;font-size:15px;height:50px;">暂无！</td></tr>')
        } else {
            $("#Platemak").html(decodeURI(plate));
        };

        if (img == "") {
            img = "/modules/images/none-img.png";
        };
        
        
        $('.icon-delete').click(function () {
            
            if (!$(this).hasClass("hover")) {
                $(this).addClass("hover");
            } else {
                $(this).removeClass("hover");
            }

        });        

        $("#navsenddoc .total-item td").each(function () {
            var _this = $(this), _total = 0;
            if (_this.data("class")) {
                $("#navsenddoc ." + _this.data("class")).each(function () {
                    _total += $(this).html() * 1;
                });
                _this.html(_total);               
            }            
        });
        

        $(".btn-ok").click(function () {
            $(".input").hide();
            $(".span").show();
            $(".input").each(function () {
                var nameresponsible = $(this).val();
                $(this).next().html(nameresponsible);
            });
            

            $(".preview").remove();
            $(".btn-ok").remove();
            $(".icon-delete").hide();

            if ($(".goods").hasClass("hover")) {
                $(".goosddoc").parent().parent().remove();
                Objects.imgOrderTable();                
                $(".img-order tr td").removeClass("no-border-left");
                $(".img-order img").removeAttr("style");
                $(".img-order img").parent().addClass("no-border-bottom");
                $(".navproducts tr:first td").removeClass("no-border-top");               
            }       
           
            if ($(".products").hasClass("hover")) {
                $(".navproducts").remove();
            }
            if ($(".raving").hasClass("hover")) {
                $(".navengraving").remove();
            }
            if ($(".pro").hasClass("hover")) {
                $(".proplate").remove();
            }
            if ($(".tailor").hasClass("hover")) {
                $(".tailor").remove();
            }
            if ($(".adhesive").hasClass("hover")) {
                $(".adhesive").remove();
            }
            if ($(".sewing-process").hasClass("hover")) {
                $(".sewing-process").remove();
            }
            if ($(".garment-finishing").hasClass("hover")) {
                $(".garment-finishing").remove();
            }
            if ($(".garment-inspection").hasClass("hover")) {
                $(".garment-inspection").remove();
            }
            if ($(".product-packaging").hasClass("hover")) {
                $(".product-packaging").remove();
            };
            $(".operation").show();
            $('body,html').animate({ scrollTop: 0 }, 300);
        });

        $(".operation").find(".printico").click(function () {
            $(".operation").remove();
            window.print();            
        });

        $(".operation").find(".get-back").click(function () {
            window.location = window.location;
        })
    };

    //删除行操作按钮(制版工艺)
    Objects.removeTaskPlateOperate = function () {
        $("span.ico-dropdown").remove();
        $("#Platemak table").find("tr:first").addClass("fontbold").addClass();
        $("#Platemak table").find("tr:first").find("td").css({ "border-top": "0", "border-bottom": "1px solid", "font-size": "16px" });
        $("#Platemak table").find("tr").find("td").removeClass("tLeft");
        $("#Platemak table tr").each(function () {
            $(this).find("td:last").remove();
            $(this).find("td:last").css("border-right", "0");
            $(this).find("td:first").css("border-left", "0");
        });
        $("#Platemak table").css("border", "0").css("height", "100%");
        $("#Platemak table").find("tr:first").find("td:last").css("margin-left", "10%");
    };

    //汇总
    Objects.getAmount = function () {
        //订单明细汇总
        $(" .total-items td").each(function () {
            var _this = $(this), _total = 0;
            if (_this.data("class")) {
                $("." + _this.data("class")).each(function () {
                    _total += $(this).html() * 1;
                });
                if (_this.data("class") == "moneytotal") {
                    _this.html(_total.toFixed(2));
                } else {
                    _this.html(_total);
                    _this.attr("title", _total);
                }
            }
        });
    };

    Objects.imgOrderTable = function () {
        var height = $(".navgoods").height();
        if (isFirefox = navigator.userAgent.indexOf("Firefox") > 0) {
            $(".img-order").height(parseInt(height) + 'px');
        } else {
            $(".img-order").height(parseInt(height) + 1 + 'px');
        }

        $(".img-order img").height(height / 2);
    };

    Objects.processPlate = function (orderid, OriginalID, ordertype) {        
        Global.post("/Task/GetPlateMakings", {
            orderID: ordertype == 2 ? OriginalID : orderid
        }, function (data) {
            if (data.items.length > 0) {
                doT.exec("template/orders/processplate.html", function (template) {                    
                    var html = template(data.items);
                    html = $(html);
                    $(".processplate").prepend(html);

                    html.find(".icon-delete").click(function () {
                        if (!$(this).hasClass("hover")) {
                            $(this).addClass("hover");
                        } else {
                            $(this).removeClass("hover");
                        }
                    })

                });
            } else {
                $(".processplate").prepend('<tr class="proplate"><td colspan="10" class="no-border-top"><div class="nodata-txt">暂无工艺</div></td></tr>');
            }
        });    
    };

    module.exports = Objects;
});