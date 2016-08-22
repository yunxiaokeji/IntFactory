define(function (require,exports,module) {
    var ObjectJS = {},
        Global = require("global");
        Upload = require("upload");
    ObjectJS.init = function (plate, price, costprice, finalPrice, profitPrice,img,orderID) {
        ObjectJS.bindEvent(plate, price, costprice, finalPrice, profitPrice);
        ObjectJS.removeTaskPlateOperate();
        ObjectJS.getPriceRange(orderID);
        if (img!="") {
            var img = img.split("?")[0];
            $("#tabletoexcel tr td img").attr("src",img);
        } else {
            $("#tabletoexcel tr td img").attr("src", "/modules/images/none-img.png");
        }
    };

    ObjectJS.bindEvent = function (plate, price, costPrice, finalPrice, profitPrice) {
        if (plate != "") {
            $("#Platemak").html(decodeURI(plate));
        }

        //工艺说明录入上传附件
        Upload.uploader({
            browse_button: 'upLoadOneImg',
            file_path: "/Content/UploadFiles/Product/",
            picture_container: "OneImgBox",
            maxSize: 5,
            multi_selection: false,
            auto_callback: false,
            fileType: 1,
            init: {
                "FileUploaded": function (up, file, info) {
                    var info = JSON.parse(info);
                    var src = file.server + info.key;
                    $("#upLoadOneImg").prev().find("img").attr('src', src);
                }
            }
        });

        //工艺说明录入上传附件
        Upload.uploader({
            browse_button: 'upLoadTwoImg',
            file_path: "/Content/UploadFiles/Product/",
            picture_container: "TwoImgBox",
            maxSize: 5,
            multi_selection: false,
            auto_callback: false,
            fileType: 1,
            init: {
                "FileUploaded": function (up, file, info) {
                    var info = JSON.parse(info);
                    var src = file.server + info.key;
                    $("#upLoadTwoImg").prev().find("img").attr('src', src);
                }
            }
        });

        var conclusion = (Number(price) + Number(costPrice)) * (profitPrice / 100 + 1);        
        var finalPrice = Number(finalPrice);        
        if (finalPrice>0) {           
            $(".offer").val(finalPrice.toFixed(2));
        } else {
            $(".offer").val(conclusion.toFixed(2));            
        }

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

        $(".btn-ok").click(function () {
            var priceoffer = $(".offer").val();            
            $(".priceoffer").html(Number(priceoffer).toFixed(2));
            $(".offer").remove();


            if ($(".customer").hasClass("no-select")) {
                $(".customer").parent().parent().remove();
            }

            $(".information").each(function () {
                _this = $(this);
                if (_this.hasClass("no-select")) {
                    var id = _this.parent().data("id");
                    $("."+id).remove();
                }                
            });

            $(".btn-ok").remove();
            $(".icon-delete").hide();

            $(".export").show();
            $('body,html').animate({ scrollTop: 0 }, 300);
        });

        //打印
        $(".print").click(function () {
            $(".export").remove();
            window.print();
        });

        $(".get-back").click(function () {
            location.href = location.href+"?"+(new Date().getMilliseconds());
        })
    };

    //删除行操作按钮(制版工艺)
    ObjectJS.removeTaskPlateOperate = function () {
        $("span.ico-dropdown").remove();
        $("#Platemak table").find("tr:first").addClass("head");
        $("#Platemak table").find("tr:first").find("td").css({ "border-top": "0", "border-bottom": "1px solid", "font-size": "16px" });
        $("#Platemak table").find("tr").find("td").removeClass("tLeft");        
        $("#Platemak table tr").each(function () {
            $(this).find("td").find("span").css("font-size","14px");
            $(this).find("td:last").remove();            
            $(this).find("td:last").css("border-right", "0");
            $(this).find("td:first").css("border-left", "0");            
        });
        $("#Platemak table").css("border", "0").css("height", "100%");        
        $("#Platemak table").find("tr:first").find("td:last").css("margin-left","10%");
    };

    ObjectJS.getPriceRange = function (orderid) {
        Global.post("/Orders/GetOrderPriceRanges", { orderid: orderid }, function (data) {
            if (data.items.length>0) {
                for (var i = 0; i < data.items.length; i++) {
                    var item = data.items;
                    var nextitem = [];
                    if (item.length == 1 || i == item.length - 1) {
                        nextitem.MinQuantity = "无上限";
                    }else {
                        nextitem.MinQuantity = Number(item[i + 1].MinQuantity) - 1;
                    }

                    if (nextitem.MinQuantity == "无上限") {
                        $("#price-range").append("<div class='color000'>数量 大于" + item[i].MinQuantity + "，" + item[i].Price + "/件</div>");
                    } else {
                        $("#price-range").append("<div class='color000'>数量 在" + item[i].MinQuantity + "到" + nextitem.MinQuantity + "之间，" + item[i].Price + "/件</div>");
                    }                    
                }
            };
        });
    };
    
    module.exports = ObjectJS;
});