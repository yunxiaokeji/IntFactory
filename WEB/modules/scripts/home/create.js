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
        if (categoryitem!=null)
        {
            var categoryitems = JSON.parse(categoryitem.replace(/&quot;/g, '"'));
            ObjectJS.categoryitems = categoryitems;
            _self.bigCategoryValue = _self.categoryitems[0].CategoryID;
            _self.categoryValue = "";
        }

        if (customerid) {
            Global.post("/Customer/GetCustomerByID", { customerid: customerid }, function (data) {

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
        var dropDownWidth = 96;
        if ($(window).width() <= 600) {
            //下拉框的宽度等于(最大显示区域宽度-边框宽度-边距宽度/2)-下拉控件右边距4个像素
            dropDownWidth = (($('.pay-content').width() - 40) / 2) - 4;
        }

        //大品类下拉
        require.async("dropdown", function () {
            $(".bigcategory").dropdown({
                prevText: "",
                defaultText: _self.categoryitems[0].CategoryName,
                defaultValue: _self.categoryitems[0].CategoryID,
                data: _self.categoryitems,
                dataValue: "CategoryID",
                dataText: "CategoryName",
                width: dropDownWidth,
                onChange: function (data) {

                    ObjectJS.bigCategoryValue = data.value;

                    ObjectJS.bindCategory(data, dropDownWidth);
                    
                }
            });
        });

        ObjectJS.bindCategory({ value: _self.categoryitems[0].CategoryID }, dropDownWidth);

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
            maxQuantity: 5,
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
            
            console.log(!!_self.endTime?_self.endTime:"");
            
        });
    }

    //绑定小品类
    ObjectJS.bindCategory = function (item, dropDownWidth) {
        var _self = this;
        var isOnce = true;

        $('.ordercategory').empty();
        Global.post("/Home/GetChildOrderCategorysByID", { categoryid: item.value, clientid: _self.clientid }, function (data) {
            var items = data.Items;

            if (isOnce) {
                _self.categoryValue = items[0].CategoryID;
                isOnce = false;
            }
            
            require.async("dropdown", function () {
                $(".ordercategory").dropdown({
                    prevText: "",
                    defaultText:items[0].CategoryName,
                    defaultValue: items[0].CategoryID,
                    data:items ,
                    dataValue: "CategoryID",
                    dataText: "CategoryName",
                    width: dropDownWidth,
                    onChange: function (data) {
                        _self.categoryValue = data.value;
                    }
                });
            });

        });

    }

    //保存实体
    ObjectJS.saveModel = function () {
        var _self = this;
        var images = "";
        $("#orderImages img").each(function () {
            images += $(this).attr("src") + ",";
        });
        
        var model = {
            CustomerID: "",
            PersonName: $("#name").val().trim(),
            PlanTime: $("#iptCreateTime").val() == null ? "" : $("#iptCreateTime").val(),
            OrderType: $(".ico-radiobox.hover").data('type'),
            BigCategoryID: _self.bigCategoryValue.trim(),
            CategoryID: _self.categoryValue.trim(),
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