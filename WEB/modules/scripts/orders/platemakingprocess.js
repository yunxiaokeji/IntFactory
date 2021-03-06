﻿define(function (require,exports,module) {
    var Global = require("global"),
    ChooseUser = require("chooseuser"),
    Upload = require("upload")
    
    var Objects = {};

    Objects.init = function (plate, orderid, OriginalID,ordertype) {
        Objects.bindEvent(plate);        
        Objects.getAmount();
        Objects.imgOrderTable();
        Objects.processPlate(orderid, OriginalID, ordertype);
    };

    Objects.bindEvent = function (plate) {
        if (plate == "") {
            $("#Platemak").html('<tr><td class="no-border" style="width:954px;font-size:15px;height:50px;"></td></tr>')
        } else {
            $("#Platemak").html(decodeURI(plate));
        };

        Objects.removeTaskPlateOperate();             

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
       
        $(".change-owner").click(function () {
            var _this = $(this);
            ChooseUser.create({
                title: "编辑负责人",
                type: 1,
                single: true,
                callback: function (items) {
                    if (items.length > 0) {
                        _this.prev().val(items[0].name);
                    }
                }
            });
        })
        
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
            $(".input").each(function () {
                var nameresponsible = $(this).val();
                $(this).next().hide();
                $(this).parent().find('.span').html(nameresponsible);
            });
            $(".span").show();
            $(".btn-ok").remove();
            $(".icon-delete").hide();
            $(".layer-upload").hide();

            $(".information").each(function () {
                _this=$(this);
                if (_this.hasClass("hover")) {                    
                    var id = _this.parent().parent().parent().parent().data("id");
                    if (id == "navgoods") {
                        $("#" + id).remove();
                        Objects.imgOrderTable();
                        $(".img-order tr td").removeClass("no-border-left");
                        $(".img-order img").removeAttr("style");
                        $(".img-order img").parent().addClass("no-border-bottom");
                        $(".navproducts tr:first td").removeClass("no-border-top")
                    } else {
                        $("." + id).remove();
                    };
                                       
                }
            });

            $(".processplates").each(function () {
                _this = $(this);
                if (_this.hasClass("hover")) {
                    var id = _this.parent().attr("id");
                    $("#" + id).remove();
                    $("." + id).remove();
                }
            });

            $(".operation").show();

            $('body,html').animate({ scrollTop: 0 }, 300);
        });

        $(".operation").find(".printico").click(function () {
            $(".operation").remove();
            window.print();            
        });

        $(".operation").find(".get-back").click(function () {
            location.href = location.href;
        })
    };

    
    //删除行操作按钮(制版工艺)
    Objects.removeTaskPlateOperate = function () {
        $("span.ico-dropdown").remove();
        $("#Platemak table").find("tr:first").addClass("fontbold");
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
            orderID: orderid
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
                    });
                });
            }            
        });    
    };

    module.exports = Objects;
});