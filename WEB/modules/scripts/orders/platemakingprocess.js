define(function (require,exports,module) {
    var Global = require("global"),
    ChooseUser = require("chooseuser"),
    Upload = require("upload")
    
    var Objects = {};

    Objects.init = function (plate, orderid, OriginalID,ordertype,order) {
        Objects.bindEvent(plate);   
        Objects.processPlate(orderid, OriginalID, ordertype);        
        Objects.order = JSON.parse(order.replace(/&quot;/g, '"'));
        Objects.getOrderRemork(Objects.order);
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

            $(".order-information tr:not(:first-child) td").css({ "height": "29px", "line-height": "28px" });

            $(".information").each(function () {
                _this=$(this);
                if (_this.hasClass("hover")) {                    
                    var id = _this.parent().parent().parent().parent().data("id");                    
                    $("." + id).remove();                                  
                }
            });

            $(".processplates.hover").each(function () {
                _this = $(this);
                $(".processplate tr[data-name=" + _this.data('name') + "]").remove();
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
        //$("#Platemak table").find("tr td ").css("line-height","28px");
        $("#Platemak table").find("tr:first").addClass("fontbold");
        $("#Platemak table").find("tr:first").find("td").css({ "border-top": "0", "border-bottom": "1px solid", "font-size": "16px" });
        $("#Platemak table").find("tr").find("td").removeClass("tLeft");
        $("#Platemak table tr").each(function () {
            $(this).find("td:last").remove();
            $(this).find("td:last").css("border-right", "0");
            $(this).find("td:first").css("border-left", "0");
        });
        $("#Platemak table").css("border", "0");
        //$("#Platemak table").find("tr:first").find("td:last").css("margin-left", "10%");
    };

    Objects.getOrderRemork = function (order) {        
        var tableModel = {}, xattr = [], yattr = [],xyattr=[],xy=[];
        for (var i = 0; i < order.OrderGoods.length; i++) {            
            if ($.inArray(order.OrderGoods[i].XRemark, xattr) == -1 ) {
                xattr.push(order.OrderGoods[i].XRemark);                
            }
            if ($.inArray(order.OrderGoods[i].YRemark, yattr) == -1) {
                yattr.push(order.OrderGoods[i].YRemark);
            }
            xyattr.push({ xy: order.OrderGoods[i].XYRemark, quantity: order.OrderGoods[i].Quantity });
            xy.push(order.OrderGoods[i].XYRemark);
        }
        tableModel.xAttr = xattr;
        tableModel.yAttr = yattr;
            doT.exec("template/orders/plate-making-list.html", function (template) {
                var html = template(tableModel);
                html = $(html);
                $(".plate-list").append(html);
                html.find(".icon-delete").click(function () {
                    if (!$(this).hasClass("hover")) {
                        $(this).addClass("hover");
                    } else {
                        $(this).removeClass("hover");
                    }
                });
                
                $(".quantity").each(function () {
                    var _this = $(this), remark = _this.data("remark");
                    if ($.inArray(remark,xy)==-1) {
                        _this.html("");
                    } else {
                        for (var i = 0; i < xyattr.length; i++) {
                            if (remark==xyattr[i].xy) {
                                _this.html(xyattr[i].quantity);
                            }
                        }
                    }
                });

                $(".goosddoc tr").find("td:last").addClass("no-border-right");
                $(".goosddoc").find("tr:last td").addClass("no-border-bottom");
            });
        
    }

    Objects.processPlate = function (orderid, OriginalID, ordertype) {        
        Global.post("/Task/GetPlateMakings", {
            orderID: ordertype == 1 ? orderid : OriginalID
        }, function (data) {
            if (data.items.length > 0) {
                doT.exec("template/orders/processplate.html", function (template) {
                    var html = template(data.items);
                    html = $(html);
                    $(".processplate").append(html);
                    html.find(".icon-delete").click(function () {
                        if (!$(this).hasClass("hover")) {
                            $(this).addClass("hover");
                        } else {
                            $(this).removeClass("hover");
                        }
                    });

                    /*获取工艺下有几个步骤*/
                    var innerHtml=[], name = [], number = [];
                    html.find(".plate-name").each(function () {
                        _this = $(this);
                        innerHtml.push(_this);
                        name.push(_this.html());                      
                    });
                    for (var i = 0; i < name.length; i++) {                        
                        number.push($(".processplate tr[data-name=" + name[i] + "]").length);                        
                    }
                    for (var i = 0; i < innerHtml.length; i++) {
                        innerHtml[i].parent().attr("rowspan",number[i]);
                    }
                });
            } else {
                $(".processplate").append('<tr><td colspan="11"><div class="nodata-txt"></div></td></tr>');
            }
        });    
    };

    module.exports = Objects;
});