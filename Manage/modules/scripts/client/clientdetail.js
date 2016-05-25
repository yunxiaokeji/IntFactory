

define(function (require, exports, module) {

    var Verify = require("verify"),
    Global = require("global"),
    doT = require("dot"),
    Easydialog = require("easydialog"),
    City = require("city"),
    ec = require("echarts/echarts");

    require("jquery");
    require("pager");
    require("echarts/chart/pie");
    require("echarts/chart/line");
    require("echarts/chart/bar");   
    var VerifyObject, CityObject;

    var Clients = {};

    Clients.Params = {
        pageIndex: 1,
        pageSize: 20,
        status: -1,
        type: -1,
        userType:1,
        beginDate: '',
        endDate: '',
        clientID: '',
        agentID: '',
        keyWords: ''
    };
    Clients.ActionParams = {
        pageIndex: 1,
        keyword: "",
        startDate: "",
        endDate: "",
        clientID: "?"
    };

    var Params = {
        searchType: "clientdetailVitalityRPT",
        dateType: 3,
        beginTime: "",
        endTime: "",
        clientID:""
    };
    //客户详情初始化
    Clients.detailInit = function (id) {
        var _self = this;
        Clients.Params.clientID = id;
        _self.clientsChart = ec.init(document.getElementById('clientdetailVitalityRPT'));
        Clients.detailEvent();        
        //行业为空
        if ($("#industry option").length == 1) $("#industry").change();
        if (id) {
            Clients.getClientDetail(id);
        }
    }
    //绑定事件
    Clients.detailEvent = function () {
        var _self = this;       
        $("#btnSearch").click(function () {
            Params.beginTime = $("#reportBeginTime").val().trim();
            Params.endTime = $("#reportEndTime").val().trim();
            if (!Params.beginTime || !Params.endTime) {
                alert("开始日期与结束日期不能为空！");
                return;
            }
            if (Params.beginTime > Params.endTime) {
                alert("开始日期不能大于结束日期！");
                return;
            }
            _self.sourceDate()
            $(".search-type .hover").data("reportBeginTime", Params.beginTime).data("reportEndTime", Params.endTime);
        });       

        $(".search-type li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                Params.searchType = _this.data("id");
                Params.dateType = _this.data("type"); 
                $("#" + _this.data("id")).show();
                if (!_self.clientsChart) {
                    _self.clientsChart = ec.init(document.getElementById('clientdetailVitalityRPT'));
                }
                if (_this.data("begintime")) {
                    $("#reportBeginTime").val(_this.data("begintime"));
                } else {
                    if (Params.dateType == 3) {
                        $("#reportBeginTime").val(new Date().setFullYear(new Date().getFullYear() - 1).toString().toDate("yyyy-MM-dd"));
                    } else if (Params.dateType == 2) {
                        $("#reportBeginTime").val(new Date().setMonth(new Date().getMonth() - 3).toString().toDate("yyyy-MM-dd"));
                    }
                    else if (Params.dateType == 1) {
                        $("#reportBeginTime").val(new Date().setDate(new Date().getDay() - 15).toString().toDate("yyyy-MM-dd"));
                    }
                }
                if (_this.data("endtime")) {
                    $("#reportEndTime").val(_this.data("endtime"));
                } else {
                    $("#reportEndTime").val(Date.now().toString().toDate("yyyy-MM-dd"));
                }
                $("#btnSearch").click();
            }

        });
        //客户设置菜单
        $(".search-status li").click(function () {
            $(this).addClass("hover").siblings().removeClass("hover");
            var index = $(this).data("index");
            $(".content-body div[name='navContent']").hide().eq(parseInt(index)).show();
            if (index == 1) {
                Clients.getClientAuthorizeData();
                $('#addNewOrder').hide();
                $('#addAuthorize').show();
            }else if (index == 0) {
                Clients.getClientOrders();
                $('#addNewOrder').show();
                $('#addAuthorize').hide();
            } else if (index == 2) {
                $("#reportBeginTime").val(new Date().setMonth(new Date().getMonth() - 3).toString().toDate("yyyy-MM-dd"));
                $("#reportEndTime").val(Date.now().toString().toDate("yyyy-MM-dd"));
                $("#btnSearch").click();
                $('#addNewOrder').hide();
                $('#addAuthorize').hide();            
            }
        });
        $("#SearchClientOrders").click(function () {
            Clients.Params.pageIndex = 1;
            Clients.Params.beginDate = $("#orderBeginTime").val();
            Clients.Params.endDate = $("#orderEndTime").val();
            Clients.getClientOrders();
        });
        //搜索
        require.async("dropdown", function () {
            var OrderStatus = [
                { ID: "0", Name: "未支付" },
                { ID: "1", Name: "已支付" }
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
                {ID: "4", Name: "试用"},
                {ID: "1",Name: "购买系统"},
                {ID: "2",Name: "购买人数"},
                {ID: "3",Name: "续费"}
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

        $('#delClient').click(function () {
            if (confirm("确定删除?")) {
                Global.post("/Client/DeleteClient", { id: Clients.Params.clientID }, function (data) {
                    if (data.Result == 1) {
                        location.href = "/Client/Index";
                    }
                    else {
                        alert("删除失败");
                    }
                });
            }
        });
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
                            ClientID: model.ClientID,
                            CompanyName: $("#name").val(),
                            ContactName: $("#contact").val(),
                            MobilePhone: $("#mobile").val(),
                            Industry: $("#industry").val(),
                            CityCode: CityObject.getCityCode(),
                            OfficePhone: $('#officePhone').val(),
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
            Clients.setIndustry(model);
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
    Clients.setIndustry = function (model) {
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
    Clients.editAuthorize = function (model,type) {
        var _self = this;
        $("#show-contact-detail").empty();
        doT.exec("template/client/client-authorize.html", function (template) {
            var innerText = template(model);
            Easydialog.open({
                container: {
                    id: "show-model-detail",
                    header: type==2?'新建订单':'赠送授权',
                    content: innerText,
                    yesFn: function () {
                        if (!VerifyObject.isPass()) {
                            return false;
                        }
                        Clients.saveAuthorize(type);
                    },
                    callback: function () {

                    }
                }
            });            
            Clients.setAuthorize(type);
            VerifyObject = Verify.createVerify({
                element: ".verify",
                emptyAttr: "data-empty",
                verifyType: "data-type",
                regText: "data-text"
            });
            $(".edit-company").hide();
            
        });
    }
    Clients.setAuthorize = function (type) {
        var endTime = {
            elem: '#endTime',
            format: 'YYYY-MM-DD',
            min: laydate.now(),
            max: '2099-06-16',
            istime: false,
            istoday: false
        };
        laydate(endTime);
        if (type == 1) { $('#div_buy').hide(); $('#div_give').show(); }
        $("#giveType").change(function () {
            if ($("#giveType").val() == "1") {
                $(this).parent().next().show().next().hide();
            } else {
                $(this).parent().next().hide().next().show();
            }
        });
        $("#buyType").change(function () {
            if ($("#buyType").val() == "2") {
                $(this).parent().next().show().next().hide();
            } else {
                $(this).parent().nextAll().show();
            }
        });
    }
    Clients.saveAuthorize = function (type) {
        //客户授权
        var paras =
        {
            clientID: Clients.Params.clientID,
            serviceType: type,
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
                if (type == 2) {
                    Clients.getClientOrders();
                } else {
                    Clients.getClientAuthorizeData();
                }
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
                $("#lblUserQuantity").text(item.UserQuantity);
                $("#lblEndTime").text(item.EndTime.toDate("yyyy-MM-dd"));
                $('#industrytemp').val(item.Industry);
                $("#lblindustryName").text(item.Industry ? $("#industrytemp").find("option:selected").text() : "--");

                Clients.Params.clientID = item.ClientID;
                Clients.Params.agentID = item.AgentID;
                Clients.ActionParams.clientID = item.ClientID;
                Params.clientID = item.ClientID;
                //绑定编辑客户信息
                $("#updateClient").click(function () {
                    Clients.editClient(item);
                });
                //绑定授权信息
                $('#addNewOrder').click(function () {
                    Clients.editAuthorize(item, 2);
                });
                $('#addAuthorize').click(function () {
                    Clients.editAuthorize(item,1);
                });                
                Clients.getClientOrders();
            } else if (data.Result == "2") {
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
        $("#clientOrders").nextAll().remove();
        Global.post("/Client/GetClientOrders", Clients.Params, function (data) {
            doT.exec("template/client/client-orders.html?3", function (templateFun) {
                var innerText = templateFun(data.Items);
                innerText = $(innerText);
                $("#clientOrders").after(innerText);

                $("#tb-clientOrders a.deleteOrder").bind("click", function () {
                    if (confirm("确定删除?")) {
                        Global.post("/Client/CloseClientOrder", { id: $(this).data("id") }, function (data) {
                            Clients.validateResult(data);
                        });
                    }
                });
                $("#tb-clientOrders a.alipayOrder").bind("click", function () {
                    if (confirm("确定标记已支付吗?")) {
                        Global.post("/Client/PayClientOrder", { id: $(this).data("id") }, function (data) {
                            Clients.validateResult(data);
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
                                if (confirm("确定修改价格吗?")) {
                                    Global.post("/Client/UpdateOrderAmount", { id: id, amount: $("#txt-orderAmount").val() }, function (data) {
                                        Clients.validateResult(data);
                                    });
                                }
                            },
                            callback: function () {
                            }
                        }
                    });
                });

                $("#tb-clientOrders a.examineOrder").bind("click", function () {
                    if (confirm("确定审核通过吗?")) {
                        Global.post("/Client/PayOrderAndAuthorizeClient", { id: $(this).data("id"), agentID: Clients.Params.agentID }, function (data) {
                            Clients.validateResult(data);
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
     
    Clients.validateResult = function (data) {
        if (data.Result == 1) {
            Clients.getClientOrders();
        } else if (data.Result == 1001) {
            alert("订单已被处理,操作失败,请刷新页面查看.");
        } else if (data.Result == 1002) {
            alert("订单已被删除,操作失败,请刷新页面查看.");
        } else {
            alert("操作失败");
        }
    };



    Clients.sourceDate = function () {
        var _self = this; 
        _self.clientsChart.showLoading({
            text: "数据正在努力加载...",
            x: "center",
            y: "center",
            textStyle: {
                color: "red",
                fontSize: 14
            },
            effect: "spin"
        });
        Global.post("/Report/GetClientVitalityReport", Params, function (data) {
            var title = [], items = [], datanames = [];
            _self.clientsChart.clear(); 
            if (data.items.length == 0) {
                _self.clientsChart.hideLoading();
                _self.clientsChart.showLoading({
                    text: "暂无数据",
                    x: "center",
                    y: "center",
                    textStyle: {
                        color: "red",
                        fontSize: 14
                    },
                    effect: "bubble"
                });
                return;
            }
            console.log(Params);
            console.log(data);
            for (var i = 0, j = data.items.length; i < j; i++) {
                title.push(data.items[i].Name);
                var _items = [];
                for (var ii = 0, jj = data.items[i].Items.length; ii < jj; ii++) {
                    if (i == 0) {
                        datanames.push(data.items[i].Items[ii].Name);
                    }
                    _items.push(data.items[i].Items[ii].Value);
                }
                items.push({
                    name: data.items[i].Name,
                    type: 'line',
                    stack: '活跃度',
                    data: _items
                });
            } 
            option = {
                tooltip: {
                    trigger: 'axis'
                },
                legend: {
                    data: title
                },
                toolbox: {
                    show: true,
                    feature: {
                        dataView: {
                            show: true,
                            readOnly: false,
                            optionToContent: function (opt) {
                                var axisData = opt.xAxis[0].data;
                                var series = opt.series;
                                var table = '<table class="table-list"><tr class="tr-header">'
                                             + '<td>时间</td>';
                                for (var i = 0, l = series.length; i < l; i++) {
                                    table += '<td>' + series[i].name + '</td>'
                                }
                                table += '</tr>';
                                for (var i = 0, l = axisData.length; i < l; i++) {
                                    table += '<tr>'
                                    + '<td class="center">' + axisData[i] + '</td>'
                                    for (var ii = 0, ll = series.length; ii < ll; ii++) {
                                        table += '<td class="center">' + series[ii].data[i] + '</td>';
                                    }

                                    table += '</tr>';
                                }
                                table += '</table>';
                                return table;
                            }
                        },
                        magicType: { show: true, type: ['line', 'bar'] },
                        restore: { show: true },
                        saveAsImage: { show: true }
                    }
                },
                xAxis: [
                    {
                        type: 'category',
                        boundaryGap: false,
                        data: datanames
                    }
                ],
                yAxis: [
                    {
                        type: 'value'
                    }
                ],
                series: items
            };
            console.log(option);
            _self.clientsChart.hideLoading();
            _self.clientsChart.setOption(option);
        });
    }
    module.exports = Clients;
});