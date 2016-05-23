define(function (require, exports, module) {
    var Upload = require("upload"), ProductIco, ImgsIco,
        Global = require("global"),
        City = require("city"), CityObject,
        Verify = require("verify"), VerifyObject;


    var ObjectJS = {}, CacheCategory = [];
    //初始化
    ObjectJS.init = function (clientid, agentid, customerid, categoryitem) {
        var _self = this;
        _self.clientid = clientid;
        _self.agentid = agentid;
        var categoryitems = JSON.parse(categoryitem.replace(/&quot;/g, '"'));

        //_self.bigCategoryValue = "请选择";

        //_self.categoryValue = "请选择";

        if (categoryitem!=null)
        {
            ObjectJS.categoryitems = categoryitems;
        }

        if (customerid) {
            Global.post("/Customer/GetCustomerByID", { customerid: customerid }, function (data) {
                //console.log(data.model);
                if (data.model.CustomerID) {
                    $("#name").val(data.model.Name);
                    $("#contactMobile").val(data.model.MobilePhone);
                    $("#address").val(data.model.Address);
                    _self.bindEvent(data.model.CityCode);
                } else { }
            });
        } else {
            _self.bindEvent('');
        }
    }

    //绑定事件
    ObjectJS.bindEvent = function (citycode) {
        var _self = this;
        //var dropDownWidth = 96;
        //if ($(window).width() <= 600) {
        //    //下拉框的宽度等于(最大显示区域宽度-边框宽度-边距宽度/2)-下拉控件右边距4个像素
        //    dropDownWidth = (($('.pay-content').width() - 40) / 2) -4;
        //}

        ////大品类下拉
        //require.async("dropdown", function () {
        //    $(".bigcategory").dropdown({
        //        prevText: "",
        //        defaultText: "请选择",
        //        defaultValue:"请选择",
        //        data: ObjectJS.categoryitems,
        //        dataValue: "CategoryID",
        //        dataText: "CategoryName",
        //        width: dropDownWidth,
        //        onChange: function (data) {

        //            $('.ordercategory').empty();
        //            _self.bigCategoryValue = data.value;
        //            _self.categoryValue = "";
        //            alert(_self.categoryValue);
        //                Global.post("/Home/GetChildOrderCategorysByID", { categoryid: data.value, clientid: _self.clientid }, function (data) {
        //                    require.async("dropdown", function () {
        //                        $(".ordercategory").dropdown({
        //                            prevText: "",
        //                            defaultText: "请选择",
        //                            defaultValue:"请选择",
        //                            data: data.Items,
        //                            dataValue: "CategoryID",
        //                            dataText: "CategoryName",
        //                            width: dropDownWidth,
        //                            onChange: function (data) {
        //                                _self.categoryValue = data.value;
        //                            }
        //                        });
        //                    });
        //                    $(".ordercategory .dropdown-text").html(data.Items[0].CategoryName);
        //                });
        //        }
        //    });
        //});

        ////小品类下拉控件
        //require.async("dropdown", function () {
        //    $(".ordercategory").dropdown({
        //        prevText: "",
        //        defaultText: "请选择",
        //        defaultValue: "请选择",
        //        data: "",
        //        dataValue: "CategoryID",
        //        dataText: "CategoryName",
        //        width: dropDownWidth,
        //        onChange: function (data) {

        //        }
        //    });
        //});

        
        //收缩订单或客户信息
        $(".info-box").click(function () {
            var _this=$(this);
            var box = _this.data('type') == 1 ? ".orderinfo" : ".customer";
            var lump = _this.find('.lump');
            if (!$(box+"-box").is(":animated")) {
                if (lump.data('type') == 'open') {
                    lump.data('type', 'close');
                    lump.css({ 'border-bottom': 'none', 'border-top': '6px solid #4a98e7' });
                } else {
                    lump.data('type', 'open');
                    lump.css({ 'border-bottom': '6px solid #4a98e7', 'border-top': 'none' });
                }
                $(box+"-box").slideToggle();
            }
        })

        //保存
        $("#btnSave").click(function () {
            if (!VerifyObject.isPass()) {
                return false;
            }
            $(this).attr("disabled", true).html("正在下单...");
            
            _self.saveModel();
        });

        ProductIco = Upload.createUpload({
            element: "#productIco",
            buttonText: "+",
            className: "",
            multiple: true,
            data: { folder: '', action: 'add', oldPath: "" },
            success: function (data, status) {
                if (data.Items.length > 0) {
                    for (var i = 0; i < data.Items.length; i++) {

                            if ($("#orderImages li.is-img").length < 5) {
                                var img = $('<li class="is-img"><img src="' + data.Items[i] + '" /><span class="ico-delete"></span></li>');
                                
                                $("#orderImages li:first-child").before(img);

                                img.find(".ico-delete").click(function () {
                                    $(this).parent().remove();
                                });
                            }
                    }
                } else {
                    alert("只能上传jpg/png/gif类型的图片，且大小不能超过5M！");
                }
            }
        });

        VerifyObject = Verify.createVerify({
            element: ".verify",
            emptyAttr: "data-empty",
            verifyType: "data-type",
            regText: "data-text"
        });
        CityObject = City.createCity({
            cityCode: citycode,
            elementID: "city"
        });
        //切换类型
        $(".ico-radiobox").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                $(".ico-radiobox").removeClass("hover");
                _this.addClass("hover");
            }
        });

        $("#bigcategory").change(function () {
            var _this = $(this);
            $("#ordercategory").empty();
            if (CacheCategory[_this.val()]) {
                for (var i = 0; i < CacheCategory[_this.val()].length; i++) {
                    $("#ordercategory").append("<option value=" + CacheCategory[_this.val()][i].CategoryID + ">" + CacheCategory[_this.val()][i].CategoryName + "</option>")
                }
            } else {
                Global.post("/Home/GetChildOrderCategorysByID", { categoryid: _this.val(), clientid: _self.clientid }, function (data) {
                    CacheCategory[_this.val()] = data.Items;
                    for (var i = 0; i < CacheCategory[_this.val()].length; i++) {
                        $("#ordercategory").append("<option value=" + CacheCategory[_this.val()][i].CategoryID + ">" + CacheCategory[_this.val()][i].CategoryName + "</option>")
                    }
                });
            }
        });

        $("#bigcategory").change();
    }

    //保存实体
    ObjectJS.saveModel = function () {
        var _self = this;
        var images = "";
        var orderType;
        $("#orderImages img").each(function () {
            images += $(this).attr("src") + ",";
        });
        $(".ico-radiobox").each(function () {
            if ($(this).hasClass('hover')) {
                orderType = $(this).data('type');
            }
        })
        var model = {
            CustomerID: "",
            PersonName: $("#name").val().trim(),
            OrderType: orderType,
            BigCategoryID: $("#bigcategory").val().trim(),
            CategoryID: $("#ordercategory").val().trim(),
            CityCode: CityObject.getCityCode(),
            Address: $("#address").val().trim(),
            OrderImage: images,
            PlanPrice: $("#planPrice").val().trim(),
            PlanQuantity: $("#planQuantity").val().trim(),
            MobileTele: $("#contactMobile").val().trim(),
            Remark: $("#remark").val().trim(),
            AgentID: _self.agentid,
            ClientID: _self.clientid
        };

        Global.post("/Home/CreateOrder", { entity: JSON.stringify(model) }, function (data) {
            $("#btnSave").attr("disabled", false).html("确认下单");
            if (data.id) {
                location.href = "/Home/OrderSuccess/" + data.id;
                
            } else {
                alert("网络异常,请稍后重试!");
            }
        });
    }

    module.exports = ObjectJS;
});