define(function (require, exports, module) {
    var Global = require("global"),
        City = require("city"), CityObject, CityContact,
        Verify = require("verify"), VerifyObject, VerifyContact,
        doT = require("dot"),
        ChooseUser = require("chooseuser"),
        Easydialog = require("easydialog");
    var CustomerReply = require("replys");       
    require("pager");
    require("colormark");
    require("dropdownmore");
    
    var Params = {
        keyWords: "",
        customerid: "",
        filterType: 1,
        ordertype: 1,
        orderstatus: -1,
        archiving: 0,
        pagesize: 10,
        pageindex: 1
    };
    var ObjectJS = {}, CacheIems = [];
    ObjectJS.isLoading = true;
    ObjectJS.keyWords = "";
    //初始化
    ObjectJS.init = function (customerid, MDToken, navid) {
        var _self = this;
        _self.guid = customerid;
        Params.customerid = customerid;
        _self.ColorList = "";
        var replyId = "";

        var nav = $(".module-tab li[data-id='" + navid + "']");
        if (nav.length > 0) {
            nav.addClass("hover");
        } else {
            $(".module-tab li").first().addClass("hover").data("first", "1");
            $("#taskReplys").show();
            CustomerReply.initTalkReply({
                element: "#taskReplys",
                guid: _self.guid,
                type: 1, /*1 客户 2订单 10任务 */
                pageSize: 10
            });
        }

        Global.post("/System/GetLableColor", { lableType: 2 }, function (data) {
            if (data.items.length > 0) {
                _self.ColorList = data.items;
            }
        });

        Global.post("/Customer/GetCustomerByID", { customerid: customerid }, function (data) {
            if (data.model.CustomerID) {
                _self.bindCustomerInfo(data.model);
                _self.bindEvent(data.model, navid);
            }
        });

        $("#addContact").hide();

    }

    //基本信息
    ObjectJS.bindCustomerInfo = function (model) {

        var _self = this;

        $("#spCustomerName").html(model.Name);
        $("#lblMobile").text(model.MobilePhone || "--");
        $("#lblEmail").text(model.Email || "--");
        $("#lblIndustry").text(model.Industry ? model.Industry.Name : "--");
        $("#lblExtent").text(model.ExtentStr || "--");
        $("#lblCity").text(model.City ? model.City.Province + " " + model.City.City + " " + model.City.Counties : "--");
        $("#lblAddress").text(model.Address || "--");
        $("#lblTime").text(model.CreateTime.toDate("yyyy-MM-dd hh:mm:ss"));
        $("#lblUser").text(model.CreateUser ? model.CreateUser.Name : "--");

        $("#lblSource").text(model.SourceType == 1 ? "阿里客户" : model.SourceType == 2 ? "自助下单" : "手工创建");

        $("#lblOwner").text(model.Owner ? model.Owner.Name : "--");
        $("#changeOwner").data("userid", model.OwnerID);

        $("#lblReamrk").text(model.Description == "" ? "--" : model.Description);

        $(".module-tab li[data-id='navOppor']").html("需求列表（" + model.DemandCount + "）");
        $(".module-tab li[data-id='navOrder']").html("打样订单（" + model.DYCount + "）");
        $(".module-tab li[data-id='navDHOrder']").html("大货订单（" + model.DHCount + "）");

        for (var i = 0; i < model.Members.length; i++) {
            var member = model.Members[i];
            var element = $("<span class='pRight5 member-item' data-id='" + member.MemberID + "'>" + member.Name + "<span class='delete-member' data-id='" + member.MemberID + "'>×</span> </span>");
            ObjectJS.bindDeleteMember(element);
            $(".add-member").before(element);
        }

        //if (model.Type == 0) {
        //    $("#lblType").html("人")
        //    $(".companyinfo").hide();
        //} else {
        //    $("#lblType").html("企")
        //    $(".companyinfo").show();
        //}
    }

    //绑定事件
    ObjectJS.bindEvent = function (model, navid) {
        var _self = this;

        $(document).click(function (e) {
            //隐藏下拉
            if (!$(e.target).parents().hasClass("dropdown") && !$(e.target).hasClass("dropdown")) {
                $(".dropdown-ul").hide();
            }

            if (!$(e.target).parents().hasClass("order-layer") && !$(e.target).hasClass("order-layer")) {
                $(".order-layer").animate({ right: "-505px" }, 200);
                $(".object-item").removeClass('looking-view');
            }
        });

        //关键字搜索
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                Params.keyWords = keyWords;
                Params.pageindex = 1;
                _self.getList();

            });
        });

        //编辑客户信息
        $("#updateCustomer").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            _self.editCustomer(model);
        });

        if (model.Status == 1) {
            $("#lblStatus").text("正常").addClass("normal");

            $("#recoveryCustomer").hide();

            //丢失客户
            $("#loseCustomer").click(function () {
                if (!ObjectJS.isLoading) {
                    return;
                }
                confirm("确认更换客户状态为丢失吗?", function () {
                    ObjectJS.isLoading = false;
                    Global.post("/Customer/LoseCustomer", { ids: model.CustomerID }, function (data) {
                        if (data.status) {
                            location.href = location.href;
                        }
                        ObjectJS.isLoading = true;
                    });
                },"更换");
            });

            //关闭客户
            $("#closeCustomer").click(function () {
                if (!ObjectJS.isLoading) {
                    return;
                }
                confirm("确认关闭此客户吗?", function () {
                    ObjectJS.isLoading = false;
                    Global.post("/Customer/CloseCustomer", { ids: model.CustomerID }, function (data) {
                        if (data.status) {
                            location.href = location.href;
                        }
                        ObjectJS.isLoading = true;
                    });
                },"关闭");
            });

        } else if (model.Status == 2 || model.Status == 3) {
            $("#lblStatus").text(model.Status ? "已关闭" : "已丢失").addClass("red");

            $("#loseCustomer").hide();
            $("#closeCustomer").hide();
            //恢复客户
            $("#recoveryCustomer").click(function () {
                if (!ObjectJS.isLoading) {
                    return;
                }
                confirm("确认恢复此客户吗?", function () {
                    ObjectJS.isLoading = false;     
                    Global.post("/Customer/RecoveryCustomer", { ids: model.CustomerID }, function (data) {
                        if (data.status) {
                            location.href = location.href;
                        }
                        ObjectJS.isLoading = true;
                    });
                },"恢复");
            });

        } else if (model.Status == 9) {
            $("#lblStatus").text("已删除");

            $("#loseCustomer").hide();
            $("#closeCustomer").hide();
            $("#recoveryCustomer").hide();
        }
        //更换拥有者
        $("#changeOwner").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            var _this = $(this);
            ChooseUser.create({
                title: "更换负责人",
                type: 1,
                single: true,
                callback: function (items) {
                    if (items.length > 0) {
                        if (_this.data("userid") != items[0].id) {
                            ObjectJS.isLoading = false;
                            Global.post("/Customer/UpdateCustomOwner", {
                                userid: items[0].id,
                                ids: model.CustomerID
                            }, function (data) {
                                if (data.status) {
                                    _this.data("userid", items[0].id);
                                    $("#lblOwner").text(items[0].name);
                                }
                                ObjectJS.isLoading = true;
                            });
                        } else {
                            alert("请选择不同人员进行更换!", 2);
                        }
                    }
                }
            });
        });

        //企业客户
        $("#addContact").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            _self.addContact();
        });

        //切换订单状态
        $(".search-orderstatus .item").click(function () {
            var _this = $(this);

            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                Params.pageindex = 1;
                Params.orderstatus = _this.data("id");
                Params.archiving = _this.data("archiving");
                _self.getList();
            }
        });

        //切换模块
        $(".module-tab li").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }           
            var _this = $(this);
            _this.siblings().removeClass("hover");
            _this.addClass("hover");
            $(".nav-partdiv").hide();
            $("#" + _this.data("id")).show();
            $("#addContact,.search-orderstatus").hide();

            if (_this.data("id") == "navLog" && (!_this.data("first") || _this.data("first") == 0)) { //日志               
                _this.data("first", "1");
                require.async("logs", function () {
                    $("#navLog").getObjectLogs({
                        guid: _self.guid,
                        type: 1, /*1 客户 2订单 10任务 */
                        pageSize: 10
                    });
                });
            } else if (_this.data("id") == "taskReplys" && (!_this.data("first") || _this.data("first") == 0)) { //备忘
                _this.data("first", "1");
                CustomerReply.initTalkReply({
                    element: "#taskReplys",
                    guid: _self.guid,
                    type: 1, /*1 客户 2订单 10任务 */
                    pageSize: 10
                });
            } else if (_this.data("id") == "navContact") { //联系人
                $("#addContact").show();
                if ((!_this.data("first") || _this.data("first") == 0)) {
                    _this.data("first", "1");
                    _self.getContacts(model.CustomerID);
                }
            } else if (_this.data("id") == "navOppor") { //需求
                Params.filterType = 1;
                if ((!_this.data("first") || _this.data("first") == 0)) {
                    _this.data("first", "1");
                    Params.keyWords = "";
                    _self.getList();
                }
            } else if (_this.data("id") == "navOrder") { //订单
                Params.filterType = 2;
                $(".search-orderstatus").show();
                if ((!_this.data("first") || _this.data("first") == 0)) {
                    _this.data("first", "1");

                    Params.keyWords = "";
                    _self.getList();
                }
            } else if (_this.data("id") == "navDHOrder") { //大货单
                Params.filterType = 3;
                $(".search-orderstatus").show();
                if ((!_this.data("first") || _this.data("first") == 0)) {
                    _this.data("first", "1");

                    Params.keyWords = "";
                    _self.getList();
                }
            }
        });

        $("#editContact").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            var _this = $(this);
            ObjectJS.isLoading = false;
            Global.post("/Customer/GetContactByID", { id: _this.data("id") }, function (data) {
                _self.addContact(data.model);
                ObjectJS.isLoading = true;
            });
        });

        //删除联系人
        $("#deleteContact").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            var _this = $(this);
            ObjectJS.isLoading = false;
            confirm("确认删除此联系人吗？", function () {
                Global.post("/Customer/DeleteContact", { id: _this.data("id") }, function (data) {
                    if (data.status) {
                        _self.getContacts(_self.guid);
                    } else {
                        alert("网络异常,请稍后重试!", 2);
                    }
                    ObjectJS.isLoading = true;
                });
            },"删除");
        });

        //默认选中标签页
        if (navid) {
            $(".module-tab li[data-id='" + navid + "']").click();
        }
        $(".add-member").dropdownSearch({
            isCreate: false,
            PostUrl: "/Organization/GetSearchUsers",
            dataText: "Name",
            dataValue: "UserID",
            isposition: true,
            moreDataParams: {
                width: 85,
                dataTexts: ["MobilePhone"]
            },
            width: 200,
            onChange: function (data) {
                var _obj = data && data.item;
                if (_obj && _obj.UserID) {
                    var userid = _obj.UserID.toLowerCase();
                    if ($(".member-item[data-id='" + userid + "']").length > 0) {
                        alert("此员工已是成员");
                        return false;
                    }
                    if ($("#changeOwner").data("userid").toLowerCase() == userid) {
                        alert("此员工已是负责人");
                        return;
                    }
                    Global.post("/Customer/AddMembers", {
                        id: _self.guid,
                        userid: userid
                    }, function (data) {
                        if (data.result) {
                            var element = $("<span class='pRight5 member-item' data-id='" + userid + "'>" + _obj.Name + "<span class='delete-member' data-id='" + userid + "'>×</span> </span>");
                            ObjectJS.bindDeleteMember(element);
                            $(".add-member").before(element);
                        } else {
                            alert("添加失败");
                        }
                    });
                }
            }
        });
    }

    ObjectJS.bindDeleteMember = function (element) {
        var _self = this;
        element.find(".delete-member").click(function () {
            var _this = $(this);
            confirm("确认删除此成员吗？", function () {
                Global.post("/Customer/RemoveMember", {
                    id: _self.guid,
                    userid: _this.data("id")
                }, function (data) {
                    if (data.result) {
                        _this.parent().remove();
                    } else {
                        alert("添加失败");
                    }
                });
            });
        });
    }

    //获取日志
    ObjectJS.getLogs = function (customerid, page) {
        var _self = this;
        $("#customerLog").empty();
        $("#customerLog").append("<div class='data-loading' ><div>");
        ObjectJS.isLoading = false;
        Global.post("/Customer/GetCustomerLogs", {
            customerid: customerid,
            pageindex: page
        }, function (data) {

            $("#customerLog").empty();
            if (data.items.length>0) {
                doT.exec("template/common/logs.html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);
                    $("#customerLog").append(innerhtml);
                });
            } else {
                $("#customerLog").append("<div class='nodata-txt' >暂无日志!<div>");
            }
            
            $("#pagerLogs").paginate({
                total_count: data.totalCount,
                count: data.pageCount,
                start: page,
                display: 5,
                border: true,
                border_color: '#fff',
                text_color: '#333',
                background_color: '#fff',
                border_hover_color: '#ccc',
                text_hover_color: '#000',
                background_hover_color: '#efefef',
                rotate: true,
                images: false,
                mouse: 'slide',
                float: "left",
                onChange: function (page) {
                    _self.getLogs(customerid, page);
                }
            });
            ObjectJS.isLoading = true;
        });
    }

    //获取订单列表
    ObjectJS.getList = function () {
        var _self = this;
        ObjectJS.isLoading = false;
        var targetid = "navOppor";
        var action = "GetNeedsOrderByCustomerID";
        if (Params.filterType != 1) {
            action = "GetOrdersByCustomerID";
            if (Params.filterType == 3) {
                Params.ordertype = 2;
                targetid = "navDHOrder";
            } else {
                Params.ordertype = 1;
                targetid = "navOrder";
            }
        }
        
        var _target = $("#" + targetid + " .table-header")
        _target.nextAll().remove();
        _target.after("<tr><td colspan='10'><div class='data-loading' ><div></td></tr>");

        Global.post("/Orders/" + action, Params, function (data) {
            ObjectJS.isLoading = true;

            ObjectJS.bindList(data, _target);
        });
    }

    //加载列表
    ObjectJS.bindList = function (data, _target) {
        var _self = this;
        _target.nextAll().remove();

        if (data.items.length > 0) {
            doT.exec("template/orders/orders.html", function (template) {
                data.items.customerShow = 1;
                var innerhtml = template(data.items);
                innerhtml = $(innerhtml);

                _target.after(innerhtml);

                innerhtml.find(".view-detail").click(function () {
                    _self.getDetail($(this).data("id"), $(this).data('code'));
                    $('.object-item').removeClass('looking-view');
                    $(this).parents('.object-item').addClass('looking-view');
                    return false;
                });

                innerhtml.find('.order-progress-item').each(function () {
                    var _this = $(this);
                    _this.css({ "width": _this.data('width') });
                });
                innerhtml.find('.progress-tip,.top-lump').each(function () {
                    var _this = $(this);
                    _this.css({ "left": (_this.parent().width() - _this.width()) / 2 });
                });
                innerhtml.find('.layer-line').css({ width: 0, left: "160px" });

                innerhtml.find(".mark").markColor({
                    isAll: false,
                    data: _self.ColorList,
                    onChange: function (obj, callback) {
                        _self.markOrders(obj.data("id"), obj.data("value"), callback);
                    }
                });

            });
        }
        else {
            _target.after("<tr><td colspan='11'><div class='nodata-txt' >暂无数据!<div></td></tr>");
        }

        var pagerid = "pagerOppor";
        if (Params.filterType == 2) {
            pagerid = "pagerOrder";
        } else if (Params.filterType == 3) {
            pagerid = "pagerDHOrder";
        }
        $("#" + pagerid).paginate({
            total_count: data.totalCount,
            count: data.pageCount,
            start: Params.pageindex,
            display: 5,
            border: true,
            rotate: true,
            images: false,
            mouse: 'slide',
            onChange: function (page) {
                Params.pageindex = page;
                _self.getList();
            }
        });
    }

    ObjectJS.getContacts = function (customerid) {
        var _self = this;
        $("#navContact .tr-header").nextAll().remove();
        $("#navContact .tr-header").after("<tr><td colspan='10'><div class='data-loading' ></div></td></tr>");
        ObjectJS.isLoading = false;
        Global.post("/Customer/GetContacts", {
            customerid: customerid
        }, function (data) {
            if (data.items.length > 0) {
                doT.exec("template/customer/contacts.html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);

                    innerhtml.find(".dropdown").click(function () {
                        var _this = $(this);
                        var position = _this.find(".ico-dropdown").position();
                        $(".dropdown-ul li").data("id", _this.data("id"));
                        $(".dropdown-ul").css({ "top": position.top + 20, "left": position.left - 40 }).show().mouseleave(function () {
                            $(this).hide();
                        });
                        return false;
                    });
                    $("#navContact .tr-header").after(innerhtml);
                });
                $(".data-loading").parent().parent().remove();
            } else {
                $(".data-loading").parent().parent().hide();
                $("#navContact .tr-header").after("<tr><td colspan='10'><div class='nodata-txt' >暂无联系人!<div></td></tr>");
            }
            ObjectJS.isLoading = true;            
        });
    }

    ObjectJS.addContact = function (model) {
        var _self = this;
        $("#show-model-detail").empty();
        doT.exec("template/customer/contact-detail.html", function (template) {
            var innerText = template();
            Easydialog.open({
                container: {
                    id: "show-contact-detail",
                    header: !model ? "添加联系人" : "编辑联系人",
                    content: innerText,
                    yesFn: function () {
                        if (!VerifyContact.isPass()) {
                            return false;
                        }
                        var entity = {
                            ContactID: model ? model.ContactID : "",
                            CustomerID: _self.guid,                            
                            Name: $("#name").val().trim(),
                            CityCode: CityContact.getCityCode(),
                            Address: $("#address").val().trim(),
                            MobilePhone: $("#contactMobile").val().trim(),
                            Email: $("#email").val().trim(),
                            Jobs: $("#jobs").val().trim(),
                            Description: $("#remark").val().trim()
                        };
                        _self.saveContact(entity);
                    },
                    callback: function () {

                    }
                }
            });

            $("#name").focus();

            if (model) {
                $("#name").val(model.Name);
                $("#jobs").val(model.Jobs);
                $("#contactMobile").val(model.MobilePhone);
                $("#email").val(model.Email);
                $("#address").val(model.Address);
                $("#remark").val(model.Description);
            }

            CityContact = City.createCity({
                cityCode: model ? model.CityCode : "",
                elementID: "contactcity"
            });
            VerifyContact = Verify.createVerify({
                element: ".verify",
                emptyAttr: "data-empty",
                verifyType: "data-type",
                regText: "data-text"
            });
        });
    }

    ObjectJS.saveContact = function (model) {
        var _self = this;
        ObjectJS.isLoading = false;
        Global.post("/Customer/SaveContact", { entity: JSON.stringify(model) }, function (data) {
            if (data.model.ContactID) {
                _self.getContacts(model.CustomerID);
            } else {
                alert("网络异常,请稍后重试!", 2);
            }
            ObjectJS.isLoading = true;
        });
    }

    //编辑信息
    ObjectJS.editCustomer = function (model) {
        var _self = this;
        $("#show-contact-detail").empty();
        doT.exec("template/customer/customer-detail.html", function (template) {
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
                        var entity = {
                            CustomerID: model.CustomerID,
                            Name: $("#name").val().trim(),
                            Type: 0,//$("#companyCustom").hasClass("ico-checked") ? 1 : 0,
                            IndustryID: "",//$("#industry").val().trim(),
                            Extent: 0,//$("#extent").val().trim(),
                            CityCode: CityObject.getCityCode(),
                            Address: $("#address").val().trim(),
                            MobilePhone: $("#contactMobile").val().trim(),
                            Email: $("#email").val().trim(),
                            Description: $("#remark").val().trim()
                        };                        
                        _self.saveModel(entity);
                    },
                    callback: function () {

                    }
                }
            });

            CityObject = City.createCity({
                cityCode: model.CityCode,
                elementID: "city"
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

    //保存实体
    ObjectJS.saveModel = function (model) {
        var _self = this;
        ObjectJS.isLoading = false;
        Global.post("/Customer/SaveCustomer", { entity: JSON.stringify(model) }, function (data) {
            if (data.model.CustomerID) {                
                location.href = location.href;
                
            } else {
                alert("网络异常,请稍后重试!", 2);
            }
            ObjectJS.isLoading = true;
        });
    }

    //标记订单
    ObjectJS.markOrders = function (ids, mark, callback) {
        if (mark < 0) {
            alert("不能标记此选项!", 2);
            return false;
        }
        ObjectJS.isLoading = false;
        Global.post("/Orders/UpdateOrderMark", {
            ids: ids,
            mark: mark
        }, function (data) {
            if (data.result == "10001") {
                alert("您没有标记订单的权限！", 2);
                callback && callback(false);
            } else {
                callback && callback(data.status);
            }
            ObjectJS.isLoading = true;
        });
    }

    ObjectJS.getDetail = function (id, orderCode) {

        $(".order-layer-item").hide();
        if ($(".order-layer").css("right") == "-505px" || $(".order-layer").css("right") == "-505") {
            $(".order-layer").animate({ right: "0px" }, 200);
        }
        $(".order-layer").append("<div class='data-loading'><div>");

        if ($("#" + id).length > 0) {
            $(".order-layer").find(".data-loading").remove();
            $("#" + id).show();
        } else {
            $.get("/Orders/OrderLayer", { id: id }, function (html) {
                $(".order-layer").find(".data-loading").remove();
                $(".order-layer").append(html);

            });
        }
        var detail = "<a class='font14 mLeft5' href='/Orders/OrderDetail/" + id + "'>" + orderCode + "</a>";
        $(".order-layer").find('.layer-header').find('a').remove();
        $(".order-layer").find('.layer-header').append(detail);
    }

    module.exports = ObjectJS;
});