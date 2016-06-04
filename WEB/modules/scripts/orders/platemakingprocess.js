define(function (require,exports,module) {

    var Objects = {};

    Objects.init = function (plate,img) {
        Objects.bindEvent(plate,img);
        Objects.removeTaskPlateOperate();
        Objects.getAmount();
    };

    Objects.bindEvent = function (plate,img) {
        if (plate == "") {
            $("#Platemak").html('<tr><td class="no-border" style="width:500px;font-size:15px;">暂无！</td></tr>')
        } else {
            $("#Platemak").html(decodeURI(plate));
        };

        

        $(".icon-delete").click(function () {
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

        $(".btn").click(function () {
            $(".preview").remove();
            $(".btn").remove();
            $(".iconfont").hide();

            if ($(".goods").hasClass("hover")) {
                $(".goosddoc").parent().parent().remove();
                $("#navsenddoc").parent().after('<td class="no-border-top locationimg" rowspan="2" style="width:158px"><span>前片图片</span><img src="'+img+'" /></td><td class="no-border-top locationimg" rowspan="2" style="width:158px"><span>后片图片</span><img src="'+img+'" /></td>');

            }
            if ($(".senddoc").hasClass("hover")) {
                $("#navsenddoc").parent().remove();
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

            $(".print").show().append('<span class="iconfont right font24 mTop5 mLeft20 color666" style="cursor:pointer;">&#xe658;</span>');
        });

        $(".print").click(function () {
            $(".print").remove();
            window.print();            
        });
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
    }

    module.exports = Objects;
});