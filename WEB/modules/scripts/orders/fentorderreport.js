define(function (require,exports,module) {
    var ObjectJS = {};
    ObjectJS.init = function (plate,price,costprice) {
        ObjectJS.bindEvent(plate, price, costprice);
        ObjectJS.removeTaskPlateOperate();
        ObjectJS.addTaskPlateCss();
    };

    ObjectJS.bindEvent = function (plate, price, costprice) {
        if (plate == "") {
            $("#Platemak").html('<tr><td class="no-border" style="width:500px;font-size:15px;">暂无！</td></tr>')
        } else {
            $("#Platemak").html(decodeURI(plate));
        };

       
        var conclusion = Number(price) + Number(costprice);

        $(".conclusion").html(conclusion.toFixed(2));

        $(".offer").val(conclusion.toFixed(2));

        if (isFirefox = navigator.userAgent.indexOf("Firefox") > 0) {
            $(".style-information").parent().css("height","100%");
        }
          
        $(".icon-delete,.icon-delete-provider").click(function () {
            if ($(this).hasClass("no-select")) {
                $(this).removeClass("no-select");
            }
            else {
                $(this).addClass("no-select");
            };
            $(".nodata-txt").parent().css("width", "932px");
        });

        $(".btn").click(function () {
            var priceoffer = $(".offer").val();            
            $(".priceoffer").html(Number(priceoffer).toFixed(2));
            $(".offer").remove();

            if ($(".customer").hasClass("no-select")) {
                $(".customer").parent().parent().remove();
            }

            if ($(".providers").hasClass("no-select")) {
                $(".providers").parent().remove();
                $(".provider").remove();
            }

            if ($(".phones").hasClass("no-select")) {
                $(".phones").parent().remove();
                $(".phone").remove();
            }

            if ($(".addresses").hasClass("no-select")) {
                $(".addresses").parent().remove();
                $(".address").remove();
            }

            $(".btn,.preview").remove();
            $(".iconfont").hide();

            $(".export").show().append('<span class="iconfont mTop10 right" style="cursor:pointer;margin-right:-20px;">&#xe658;</span>');
        });

        //打印
        $(".export").click(function () {
            $(".export").remove();
            window.print();
        });
    };

    //删除行操作按钮(制版工艺)
    ObjectJS.removeTaskPlateOperate = function () {
        $("span.ico-dropdown").remove();
        
        $("#Platemak table").find("tr:first").find("td").css({ "border-top": "0", "border-bottom": "1px solid", "font-size": "16px" });
        $("#Platemak table").find("tr").find("td").removeClass("tLeft");        
        $("#Platemak table tr").each(function () {            
            $(this).find("td:last").remove();            
            $(this).find("td:last").css("border-right", "0");
            $(this).find("td:first").css("border-left", "0");            
        });
        $("#Platemak table").css("border", "0").css("height", "100%");        
        $("#Platemak table").find("tr:first").find("td:last").css("margin-left","10%");
    };

    ObjectJS.addTaskPlateCss = function () {
        $(".Processing").find("tr:last").find("td").addClass("no-border-bottom");
    }

    
    module.exports = ObjectJS;
});