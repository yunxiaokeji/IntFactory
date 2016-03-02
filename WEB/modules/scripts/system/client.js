define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Upload = require("upload"),
        City = require("city"), CityObject,
        Verify = require("verify"), VerifyObject;
    require("pager");

    var ObjectJS = {};

    ObjectJS.Params = {
        PageSize: 10,
        PageIndex: 1,
        Status: -1,
        Type: -1,
        BeginDate: '',
        EndDate:''
    };

    //初始化
    ObjectJS.init = function (option) {
        var _self = this;
        _self.bindEvent();

        VerifyObject = Verify.createVerify({
            element: ".verify",
            emptyAttr: "data-empty",
            verifyType: "data-type",
            regText: "data-text"
        });

        ObjectJS.getDetail();

        if (option == 3) {
            $(".search-stages li").eq(2).click();
        }
        else if (option == 2) {
            $(".search-stages li").eq(1).click();
        }
        else if (option == 4) {
            $(".search-stages li").eq(3).click();
        }
    }

    //绑定事件
    ObjectJS.bindEvent = function () {

        $(document).click(function (e) {
            //隐藏下拉
            if (!$(e.target).parents().hasClass("dropdown") && !$(e.target).hasClass("dropdown")) {
                $(".dropdown-ul").hide();
            }
        });

        //选择海报图片
        PosterIco = Upload.createUpload({
            element: "#Logo",
            buttonText: "选择LOGO",
            className: "",
            data: { folder: '/Content/tempfile/', action: 'add', oldPath: "" },
            success: function (data, status) {
                if (data.Items.length > 0) {
                    $("#PosterDisImg").show();
                    $("#PosterDisImg").attr("src", data.Items[0]);
                    $("#CompanyLogo").val(data.Items[0]);
                } else {
                    alert("只能上传jpg/png/gif类型的图片，且大小不能超过10M！");
                }
            }
        });

        //城市插件
        CityObject = City.createCity({
            elementID: "citySpan"
        });

        //切换
        $(".search-stages li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");

                $(".content-body div[name='clientInfo']").hide().eq(parseInt(_this.data("id"))).show();

                if (_this.data("id") == 3)
                {
                    ObjectJS.getClientOrders();
                }
            }
        });

        //保存公司基本信息
        $("#btnSaveClient").click(function () {
            if (!VerifyObject.isPass()) {
                return false;
            };

            ObjectJS.saveModel();
        });

        //搜索
        require.async("dropdown", function () {
            var OrderStatus = [
                {
                    ID: "0",
                    Name: "未支付"
                },
                {
                    ID: "1",
                    Name: "已支付"
                },
                {
                    ID: "9",
                    Name: "已关闭"
                }
            ];
            $("#OrderStatus").dropdown({
                prevText: "订单状态-",
                defaultText: "所有",
                defaultValue: "-1",
                data: OrderStatus,
                dataValue: "ID",
                dataText: "Name",
                width: "120",
                onChange: function (data) {
                    $(".tr-header").nextAll().remove();

                    ObjectJS.Params.PageIndex = 1;
                    ObjectJS.Params.Status =parseInt( data.value);
                    ObjectJS.getClientOrders();
                }
            });

            var OrderTypes = [
                {
                    ID: "1",
                    Name: "购买系统"
                },
                {
                    ID: "2",
                    Name: "购买人数"
                },
                {
                    ID: "3",
                    Name: "续费"
                }
            ];
            $("#OrderTypes").dropdown({
                prevText: "订单类型-",
                defaultText: "所有",
                defaultValue: "-1",
                data: OrderTypes,
                dataValue: "ID",
                dataText: "Name",
                width: "120",
                onChange: function (data) {
                    $(".tr-header").nextAll().remove();

                    ObjectJS.Params.PageIndex = 1;
                    ObjectJS.Params.Type = parseInt(data.value);
                    ObjectJS.getClientOrders();
                }
            });

        });

        $("#SearchClientOrders").click(function () {
            if ($("#orderBeginTime").val() != '' || $("#orderEndTime").val() != '') {
                ObjectJS.Params.PageIndex = 1;
                ObjectJS.Params.beginDate = $("#orderBeginTime").val();
                ObjectJS.Params.endDate = $("#orderEndTime").val();
                ObjectJS.getClientOrders();
            }
        });

        //继续支付客户端订单
        $("#PayClientOrder").click(function () {
            var id = $(this).data("id");
            var type = $(this).data("type");

            var url = "/Auction/BuyNow";
            if(type==2)
                url = "/Auction/BuyUserQuantity";
            else if(type==3)
                url = "/Auction/ExtendNow";
            url += "/" + id;

            location.href = url;
        });

        //关闭客户端订单
        $("#CloseClientOrder").click(function () {
            Global.post("/System/CloseClientOrder",{id:$(this).data("id")},function(data){
                if (data.Result == 1) {
                    ObjectJS.getClientOrders();
                }
                else {
                    alert("关闭失败");
                }
            });
        });
    }

    //获取详情
    ObjectJS.getDetail = function () {
        Global.post("/System/GetClientDetail", null, function (data) {
            if (data.Client) {
                var item = data.Client;
                //基本信息
                $("#CompanyName").val(item.CompanyName);
                $("#ContactName").val(item.ContactName);
                $("#MobilePhone").val(item.MobilePhone);
                $("#OfficePhone").val(item.OfficePhone);
                $("#Industry").val(item.Industry);
                if (item.City)
                    CityObject.setValue(item.City.CityCode);
                if (item.Logo)
                {
                    $("#PosterDisImg").show().attr("src", item.Logo);
                    $("#CompanyLogo").val(item.Logo);
                }
                $("#Address").val(item.Address);
                $("#Description").val(item.Description);

                //授权信息
                var agent = data.Agent;
                $("#cid").attr("href", '/Home/SelfOrder/' + item.ClientID).html('/Home/SelfOrder/' + item.ClientID);
                $("#ckey").html(item.ClientCode);

                $("#UserQuantity").html(agent.UserQuantity);
                $("#EndTime").html(agent.EndTime.toDate("yyyy-MM-dd"));
                $("#agentRemainderDays").html(data.Days);
                if (agent.AuthorizeType == 0)
                {
                    $(".btn-buy").html("立即购买");
                }
                else {
                    if (parseInt(data.Days) < 31)
                    {
                        $("#agentRemainderDays").addClass("red");
                        $(".btn-buy").html("续费").attr("href", "/Auction/ExtendNow");
                    }
                    else {
                        $(".btn-buy").html("购买人数").attr("href", "/Auction/BuyUserQuantity");
                    }
                }
            
            }
        })
    }

    //保存实体
    ObjectJS.saveModel = function () {
        var _self = this;
        var model = {
            CompanyName: $("#CompanyName").val(),
            Logo:$("#CompanyLogo").val(),
            ContactName: $("#ContactName").val(),
            MobilePhone: $("#MobilePhone").val(),
            OfficePhone: $("#OfficePhone").val(),
            CityCode: CityObject.getCityCode(),
            Industry: $("#Industry").val(),
            Address: $("#Address").val(),
            Description:$("#Description").val()
        };

        Global.post("/System/SaveClient", { entity: JSON.stringify(model) }, function (data) {
            if (data.Result == 1) {
                alert("保存成功");
            }
        })
    }

    //获取客户端的订单列表
    ObjectJS.getClientOrders = function () {
        var _self = this;
        $(".tr-header").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='8'><div class='dataLoading'><img src='/modules/images/ico-loading.jpg'/><div></td></tr>");

        Global.post("/System/GetClientOrders",
            {
                pageSize: ObjectJS.Params.PageSize,
                pageIndex: ObjectJS.Params.PageIndex,
                status: ObjectJS.Params.Status,
                type: ObjectJS.Params.Type,
                beginDate: ObjectJS.Params.BeginDate,
                endDate: ObjectJS.Params.EndDate
            },
            function (data) {
                $(".tr-header").nextAll().remove();

                if (data.Items.length > 0) {
                    doT.exec("template/system/client-orders.html", function (template) {
                        var innerhtml = template(data.Items);
                        innerhtml = $(innerhtml);

                        $(".tr-header").after(innerhtml);

                        //下拉事件
                        innerhtml.find(".dropdown").click(function () {
                            var _this = $(this);
                            var position = _this.find(".ico-dropdown").position();
                            $(".dropdown-ul li").data("id", _this.data("id")).data("type", _this.data("type"));
                            $(".dropdown-ul").css({ "top": position.top + 20, "left": position.left - 80 }).show().mouseleave(function () {
                                $(this).hide();
                            });
                            return false;
                        });
                    });
                }
                else {
                    $(".tr-header").after("<tr><td colspan='8'><div class='noDataTxt' >暂无数据!<div></td></tr>");
                }

                $("#pager").paginate({
                    total_count: data.TotalCount,
                    count: data.PageCount,
                    start: _self.Params.PageIndex,
                    display: 5,
                    images: false,
                    mouse: 'slide',
                    onChange: function (page) {
                        $(".tr-header").nextAll().remove();
                        _self.Params.PageIndex = page;
                        _self.getClientOrders();
                    }
                });

            }
        );
    }

    module.exports = ObjectJS;
});