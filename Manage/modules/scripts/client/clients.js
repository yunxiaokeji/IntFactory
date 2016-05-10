

define(function (require, exports, module) {

    require("jquery");
    require("pager");
    var Verify = require("verify"),
        Global = require("global"),
        doT = require("dot"),
        Easydialog = require("easydialog"),
        City = require("city");
    var VerifyObject, CityObject;
    

    var Clients = {};

    Clients.Params = {
        pageIndex: 1,
        pageSize: 20,
        status: -1,
        type: -1,
        beginDate: '',
        endDate: '',
        clientID: '',
        agentID:'',
        keyWords: ''
    };
    Clients.ActionParams = {
        pageIndex: 1,
        keyword: "",
        startDate: "",
        endDate: "",
        clientID:"?"
    };
    //新建客户初始化
    Clients.createInit = function (id) {
        Clients.createEvent();
        //行业为空
        if ($("#industry option").length == 1) $("#industry").change();
        
    }
    //绑定事件
    Clients.createEvent = function () {
        //验证插件
        VerifyObject = Verify.createVerify({
            element: ".verify",
            emptyAttr: "data-empty",
            verifyType: "data-type",
            regText: "data-text"
        });

        $("#authorizeType").change(function () {
            if ($("#authorizeType").val() == "1") {
                $(".contentnew li[name='authorizeType']").fadeIn();
            }
            else {
                $(".contentnew li[name='authorizeType']").fadeOut();
            }
        });       

       
        //判断账号是否存在
        $("#loginName").blur(function () {
            var value = $(this).val();
            if (!value) {
                return;
            }
            Global.post("/Client/IsExistLoginName", { loginName: value }, function (data) {
                if (data.Result) {
                    $("#loginName").val("");
                    alert("登录账号已存在!");
                }
            });
        });        
    };
    //客户详情初始化
    Clients.detailInit = function (id) {
        Clients.Params.clientID = id;
        Clients.detailEvent();
        //行业为空
        if ($("#industry option").length == 1) $("#industry").change();
        if (id) {
            Clients.getClientDetail(id);
        }
    }
    //绑定事件
    Clients.detailEvent = function () {
        //客户设置菜单
        $(".search-tab li").click(function () {
            $(this).addClass("hover").siblings().removeClass("hover");
            var index = $(this).data("index");
            $(".content-body div[name='navContent']").hide().eq(parseInt(index)).show();
            if (index == 1) {
                Clients.getClientAuthorizeData();
            }
            else if (index == 0) {
                Clients.getClientOrders();
            }else if(index == 2){
                Clients.bindActionReport();
            }
        });
        $("#SearchClientOrders").click(function () {
            if ($("#orderBeginTime").val() != '' || $("#orderEndTime").val() != '') {
                Clients.Params.pageIndex = 1;
                Clients.Params.beginDate = $("#orderBeginTime").val();
                Clients.Params.endDate = $("#orderEndTime").val();
                Clients.getClientOrders();
            }
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
                    $("#clientOrders").nextAll().remove();

                    Clients.Params.pageIndex = 1;
                    Clients.Params.status = parseInt(data.value);
                    Clients.getClientOrders();
                }
            });

            var OrderTypes = [
                {
                    ID: "4",
                    Name: "试用"
                },
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
                    $("#clientOrders").nextAll().remove();
                    Clients.Params.pageIndex = 1;
                    Clients.Params.type = parseInt(data.value);
                    Clients.getClientOrders();
                }
            });
        });
        //验证插件
        VerifyObject = Verify.createVerify({
            element: ".verify",
            emptyAttr: "data-empty",
            verifyType: "data-type",
            regText: "data-text"
        });
        
        $("#SearchList").click(function () {
            Clients.ActionParams.pageIndex = 1;
            Clients.ActionParams.startDate = $("#actionBeginTime").val();
            Clients.ActionParams.endDate = $("#actionEndTime").val();
            Clients.bindActionReport();

        });
        ////城市插件
        //CityObject = City.createCity({
        //    elementID: "citySpan"
        //});
       
    };
    //编辑信息
    Clients.editClient = function (model) {
        var _self = this;
        $("#show-contact-detail").empty();
        doT.exec("template/client/client-edit.html", function (template) {
            var innerText = template(model);
            Easydialog.open({
                container: {
                    id: "show-model-detail",
                    header: "编辑客户信息",
                    content: innerText,
                    yesFn: function () {
                        if (!VerifyObject.isPass()) {
                            return false;
                        }
                        var modules = [];
                        var entity = {
                            ClientID:model.ClientID,
                            CompanyName: $("#name").val(),
                            ContactName: $("#contact").val(),
                            MobilePhone: $("#mobile").val(),
                            Industry: $("#industry").val(),
                            CityCode: CityObject.getCityCode(),
                            OfficePhone:$('#officePhone').val(),
                            Address: $("#address").val(),
                            Description: $("#description").val(),
                            Modules: modules
                        };
                        Clients.saveModel(entity);
                    },
                    callback: function () {

                    }
                }
            });
            Clients.Setindustry(model);
            CityObject = City.createCity({
                cityCode: model.CityCode,
                elementID: "citySpan"
            });           
            VerifyObject = Verify.createVerify({
                element: ".verify",
                emptyAttr: "data-empty",
                verifyType: "data-type",
                regText: "data-text"
            });
            $(".edit-company").hide();
        });
    }
    Clients.Setindustry = function (model) {
        $('#industry').html($('#industrytemp').html());
        $('#industry').val(model.Industry || '');
        if ($('#industry').val() != '') { $("#otherIndustry").hide(); }
        //更换行业
        $("#industry").change(function () {
            $("#industryName").val("");
            if ($(this).val() == "") {
                $("#otherIndustry").show();
                $("#saveIndustry").hide();
            } else {
                $("#otherIndustry").hide();
            }
        });
        $("#industryName").blur(function () {
            if ($(this).val() == "") {
                $("#saveIndustry").hide();
            } else {
                var ele = $("#industry option[data-name='" + $(this).val() + "']");
                if (ele.length > 0) {
                    ele.prop("selected", "selected");
                    $("#otherIndustry").hide();
                } else {
                    $("#saveIndustry").show();
                }
            }
        });
        //保存行业
        $("#saveIndustry").click(function () {
            var name = $("#industryName").val();
            Global.post("/Client/CreateIndustry", { name: name }, function (data) {
                if (data.ID) {
                    var option = "<option value=\"" + data.ID + "\" selected=\"selected\" data-name=\"" + name + "\">" + name + "</option>";
                    $("#industry").prepend(option);
                    $("#otherIndustry").hide();
                }
            });
        });
       
    }
    Clients.saveModel = function (model) {
        var _self = this;

        Global.post("/Client/SaveClient", { client: JSON.stringify(model), loginName: $("#loginName").val() }, function (data) {
            if (data.Result == "1") {
                location.href = "/Client/Index";
            } else if (data.Result == "2") {
                alert("登陆账号已存在!");
                $("#loginName").val("");
            }
        });
    }
    Clients.editAuthorize = function (model) {
        var _self = this;
        $("#show-contact-detail").empty();
        doT.exec("template/client/client-authorize.html", function (template) {
            var innerText = template(model);
            Easydialog.open({
                container: {
                    id: "show-model-detail",
                    header: "授权",
                    content: innerText,
                    yesFn: function () {
                        if (!VerifyObject.isPass()) {
                            return false;
                        }                       
                        Clients.saveAuthorize();
                    },
                    callback: function () {

                    }
                }
            });
            Clients.SetAuthorize();
            VerifyObject = Verify.createVerify({
                element: ".verify",
                emptyAttr: "data-empty",
                verifyType: "data-type",
                regText: "data-text"
            });
            $(".edit-company").hide();
        });
        
    }
    Clients.SetAuthorize = function () {
        var endTime = {
            elem: '#endTime',
            format: 'YYYY-MM-DD',
            min: laydate.now(),
            max: '2099-06-16',
            istime: false,
            istoday: false
        };
        laydate(endTime);
        $("#span_giveSystem").click(function () {
            $(this).parent().next().show().next().hide();
        });
        $("#span_buySystem").click(function () {
            $(this).parent().next().hide().next().show();
        });
        $("#giveType").change(function () {
            if ($("#giveType").val() == "1") {
                $(this).parent().next().show().next().hide();
            }
            else {
                $(this).parent().next().hide().next().show();
            }
        });
        $("#buyType").change(function () {
            if ($("#buyType").val() == "2") {
                $(this).parent().next().show().next().hide();
            }
            else {
                $(this).parent().nextAll().show();
            }
        });
    }
    Clients.saveAuthorize = function () {
        //客户授权
        var paras =
        {
            clientID: Clients.Params.clientID,
            serviceType: $("#giveSystem").is(":checked") ? 1 : 2,
            giveType: $("#giveType").val(),
            userQuantity: $("#userQuantity").val(),
            endTime: $("#endTime").val(),
            buyType: $("#buyType").val(),
            buyUserQuantity: $("#buyUserQuantity").val(),
            buyUserYears: $("#buyUserYears").val()
        };
        Global.post("/Client/SaveClientAuthorize", paras, function (data) {
            if (data.Result == "1") {
                alert("保存成功");
            } else {
                alert("保存失败");
            }
        });
    }
    //客户详情
    Clients.getClientDetail = function (id) {
        Global.post("/Client/GetClientDetail", { id: id }, function (data) {
            if (data.Result == "1") {
                var item = data.Item;
                $("#customerName").html(item.CompanyName);
                $("#lblName").text(item.CompanyName);
                $("#lblContact").text(item.ContactName || '--');
                $("#lblMobile").text(item.MobilePhone || '--');               
                $("#lblAddress").text(item.Address || '--');
                $("#lblDescription").text(item.Description);
                $("#lblCity").text(item.City ? item.City.Province + " " + item.City.City + " " + item.City.Counties : "--");                
                $("#lblOfficePhone").text(item.OfficePhone);
                $('#industrytemp').val(item.Industry);
                $("#lblindustryName").text(item.Industry ? $("#industrytemp").find("option:selected").text() : "--");

                Clients.Params.clientID = item.ClientID;
                Clients.Params.agentID = item.AgentID;
                Clients.ActionParams.clientID = item.ClientID;
                //绑定编辑客户信息
                $("#updateClient").click(function () {
                    Clients.editClient(item);
                });
                //绑定授权信息
                $('#addAuthorize').click(function () {
                    Clients.editAuthorize(item);
                });
               
                Clients.getClientOrders();
            } else if (data.Result == "2")
            {
                alert("登陆账号已存在!");
                $("#loginName").val("");
            }
        });
    };

    //获取客户授权数据
    Clients.getClientAuthorizeData = function () {
        var _self = this;
        $("#client-header").nextAll().remove();
        Global.post("/Client/GetClientAuthorizeLogs", Clients.Params, function (data) {
            doT.exec("template/client/clientauthorizelog-list.html?3", function (templateFun) {
                var innerText = templateFun(data.Items);
                innerText = $(innerText);
                $("#client-header").after(innerText);

                //$(".table-list a.ico-del").bind("click", function () {
                //    if (confirm("确定删除?")) {
                //        Global.post("/Client/DeleteClient", { id: $(this).attr("data-id") }, function (data) {
                //            if (data.Result == 1) {
                //                location.href = "/Client/Index";
                //            }
                //            else {
                //                alert("删除失败");
                //            }
                //        });
                //    }
                //});
            });
            $("#pager").paginate({
                total_count: data.TotalCount,
                count: data.PageCount,
                start: Clients.Params.pageIndex,
                display: 5,
                border: true,
                rotate: true,
                images: false,
                mouse: 'slide',
                onChange: function (page) {
                    Clients.Params.pageIndex = page;
                    Clients.getClientAuthorizeData();
                }
            });
        });
    };

    //获取客户订单列表
    Clients.getClientOrders = function () {
        var _self = this;
        if ($("#clientOrders").nextAll().length>0) { return; }
        $("#clientOrders").nextAll().remove();
        Global.post("/Client/GetClientOrders", Clients.Params, function (data) {
            doT.exec("template/client/client-orders.html?3", function (templateFun) {
                var innerText = templateFun(data.Items);
                innerText = $(innerText);
                $("#clientOrders").after(innerText);

                $("#tb-clientOrders a.deleteOrder").bind("click", function () {
                    if (confirm("确定删除?")) {
                        Global.post("/Client/CloseClientOrder", { id: $(this).data("id") }, function (data) {
                            if (data.Result == 1) {
                                Clients.getClientOrders();
                            }
                            else {
                                alert("关闭失败");
                            }
                        });
                    }
                });

                $("#tb-clientOrders a.editOrder").bind("click", function () {
                    var id = $(this).data("id");
                    var amount = $(this).data("amount");
                    var html = "<input type='text' value='" + amount + "' id='txt-orderAmount' />";
                    Easydialog.open({
                        container: {
                            id: "show-model-feedback",
                            header: "修改订单支付金额",
                            content: html,
                            yesFn: function () {
   
                                Global.post("/Client/UpdateOrderAmount", { id: id, amount: $("#txt-orderAmount").val() }, function (data) {
                                    if (data.Result == 1) {
                                        Clients.getClientOrders();
                                    }
                                    else {
                                        alert("修改失败");
                                    }

                                });
                            },
                            callback: function () {
                            }
                        }
                    });
                });

                $("#tb-clientOrders a.examineOrder").bind("click", function () {
                    if (confirm("审核通过?")) {
                        Global.post("/Client/PayOrderAndAuthorizeClient", { id: $(this).data("id"), agentID: Clients.Params.agentID }, function (data) {
                            if (data.Result == 1) {
                                Clients.getClientOrders();
                            }
                            else {
                                alert("审核失败");
                            }
                        });
                    }
                });
            });

            $("#pager2").paginate({
                total_count: data.TotalCount,
                count: data.PageCount,
                start: Clients.Params.pageIndex,
                display: 5,
                border: true,
                rotate: true,
                images: false,
                mouse: 'slide',
                onChange: function (page) {
                    Clients.Params.pageIndex = page;
                    Clients.getClientOrders();
                }
            });
        });
    };

    //绑定数据
    Clients.bindActionReport = function () {
        $(".tr-header").nextAll().remove();
        Global.post("/Report/GetAgentActionReportsByClientID", Clients.ActionParams, function (data) {
            doT.exec("template/report/agentactionreport-list.html?3", function (templateFun) {
                var innerText = templateFun(data.Items);
                innerText = $(innerText);
                $(".tr-header").after(innerText);
            });
        });
    }
    //客户列表初始化
    Clients.init = function () {
        Clients.bindEvent();
        Clients.bindData();
    };

    //绑定事件
    Clients.bindEvent = function () {
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                Clients.Params.pageIndex = 1;
                Clients.Params.keyWords = keyWords;
                Clients.bindData();
            });
        });
    };

    //绑定数据
    Clients.bindData = function () {
        var _self = this;
        $("#client-header").nextAll().remove();

        Global.post("/Client/GetClients", Clients.Params, function (data) {
            doT.exec("template/client/client-list.html?3", function (templateFun) {
                var innerText = templateFun(data.Items);
                innerText = $(innerText);
                $("#client-header").after(innerText);

                $(".table-list a.ico-del").bind("click", function () {
                    if (confirm("确定删除?"))
                    {
                        Global.post("/Client/DeleteClient", { id: $(this).data("id") }, function (data) {
                            if (data.Result == 1) {
                                location.href = "/Client/Index";
                            }
                            else {
                                alert("删除失败");
                            }
                        });
                    }
                });
            });

            $("#pager").paginate({
                total_count: data.TotalCount,
                count: data.PageCount,
                start: Clients.Params.pageIndex,
                display: 5,
                border: true,
                rotate: true,
                images: false,
                mouse: 'slide',
                onChange: function (page) {
                    Clients.Params.pageIndex = page;
                    Clients.bindData();
                }
            });
        });
    }

    module.exports = Clients;
});