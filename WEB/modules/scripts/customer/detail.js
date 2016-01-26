﻿define(function (require, exports, module) {
    var Global = require("global"),
        City = require("city"), CityObject, CityContact,
        Verify = require("verify"), VerifyObject, VerifyContact,
        doT = require("dot"),
        ChooseUser = require("chooseuser"),
        Easydialog = require("easydialog");
    require("pager");

    var ObjectJS = {}, CacheIems = [];
    //初始化
    ObjectJS.init = function (customerid, MDToken) {
        var _self = this;
        _self.customerid = customerid;
        _self.bindStyle();

        if (!MDToken) {
            $("#btn_shareMD").hide();
        }

        Global.post("/Customer/GetCustomerByID", { customerid: customerid }, function (data) {
            if (data.model.CustomerID) {
                _self.bindCustomerInfo(data.model);
                _self.bindEvent(data.model);
            }
        });

        _self.initTalk(customerid);

        $("#addContact").hide();
        //$(window).resize(function () {
        //    _self.bindStyle();
        //});
    }
    //样式
    ObjectJS.bindStyle = function () {

        var stages = $(".stage-items"), width = stages.width();

        stages.find("li .leftbg").first().removeClass("leftbg");
        stages.find("li .rightbg").last().removeClass("rightbg");
        stages.find("li").width(width / stages.find("li").length - 20);
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

        $("#lblSource").text(model.Source ? model.Source.SourceName : "--");

        $("#lblOwner").text(model.Owner ? model.Owner.Name : "--");
        $("#changeOwner").data("userid", model.OwnerID);

        $("#lblReamrk").text(model.Description);

        if (model.Type == 0) {
            $("#lblType").html("人")
            $(".companyinfo").hide();
        } else {
            $("#lblType").html("企")
            $(".companyinfo").show();
        }

        //处理阶段
        var stage = $(".stage-items li[data-id='" + model.StageID + "']");
        stage.addClass("hover");
        if (model.Stage) {
            CacheIems[model.StageID] = model.Stage.StageItem;
            if (model.Stage.StageItem) {
                _self.bindStageItems(model.Stage.StageItem);
            }
        }

    }
    //阶段行为项
    ObjectJS.bindStageItems = function (items) {
        $("#stageItems").empty();
        for (var i = 0; i < items.length; i++) {
            $("#stageItems").append("<li>" + items[i].ItemName + "</li>");
        }
    };
    //绑定事件
    ObjectJS.bindEvent = function (model) {
        var _self = this;

        $(document).click(function (e) {
            //隐藏下拉
            if (!$(e.target).parents().hasClass("dropdown") && !$(e.target).hasClass("dropdown")) {
                $(".dropdown-ul").hide();
            }
        })

        //编辑客户信息
        $("#updateCustomer").click(function () {
            _self.editCustomer(model);
        });

        if (model.Status == 1) {
            $("#lblStatus").text("正常").addClass("normal");

            $("#recoveryCustomer").hide();

            //丢失客户
            $("#loseCustomer").click(function () {
                confirm("确认更换客户状态为丢失吗?", function () {
                    Global.post("/Customer/LoseCustomer", { ids: model.CustomerID }, function (data) {
                        if (data.status) {
                            location.href = location.href;
                        }
                    });
                });
            });
            //关闭客户
            $("#closeCustomer").click(function () {
                confirm("确认关闭此客户吗?", function () {
                    Global.post("/Customer/CloseCustomer", { ids: model.CustomerID }, function (data) {
                        if (data.status) {
                            location.href = location.href;
                        }
                    });
                });
            });
            //切换阶段
            $(".stage-items li").click(function () {
                var _this = $(this);
                !_this.hasClass("hover") && confirm("确认客户切换到此阶段吗?", function () {
                    Global.post("/Customer/UpdateCustomStage", {
                        ids: model.CustomerID,
                        stageid: _this.data("id")
                    }, function (data) {
                        if (data.result == "10001") {
                            alert("您没有此操作权限，请联系管理员帮您添加权限！");
                            return;
                        }

                        if (data.status) {
                            _this.siblings().removeClass("hover");
                            _this.addClass("hover");
                            if (CacheIems[_this.data("id")]) {
                                _self.bindStageItems(CacheIems[_this.data("id")]);
                            } else {
                                Global.post("/Customer/GetStageItems", {
                                    stageid: _this.data("id")
                                }, function (data) {
                                    CacheIems[_this.data("id")] = data.items;
                                    _self.bindStageItems(CacheIems[_this.data("id")]);
                                });
                            }
                        }
                    });
                });
            });

        } else if (model.Status == 2 || model.Status == 3) {
            $("#lblStatus").text(model.Status ? "已关闭" : "已丢失").addClass("red");

            $("#loseCustomer").hide();
            $("#closeCustomer").hide();
            //恢复客户
            $("#recoveryCustomer").click(function () {
                confirm("确认恢复此客户吗?", function () {
                    Global.post("/Customer/RecoveryCustomer", { ids: model.CustomerID }, function (data) {
                        if (data.status) {
                            location.href = location.href;
                        }
                    });
                });
            });

        } else if (model.Status == 9) {
            $("#lblStatus").text("已删除");

            $("#loseCustomer").hide();
            $("#closeCustomer").hide();
            $("#recoveryCustomer").hide();
        }
        //更换拥有者
        $("#changeOwner").click(function () {
            var _this = $(this);
            ChooseUser.create({
                title: "更换拥有者",
                type: 1,
                single: true,
                callback: function (items) {
                    if (items.length > 0) {
                        if (_this.data("userid") != items[0].id) {
                            Global.post("/Customer/UpdateCustomOwner", {
                                userid: items[0].id,
                                ids: model.CustomerID
                            }, function (data) {
                                if (data.status) {
                                    _this.data("userid", items[0].id);
                                    $("#lblOwner").text(items[0].name);
                                }
                            });
                        } else {
                            alert("请选择不同人员进行转移!");
                        }
                    }
                }
            });
        });

        //企业客户
        if (model.Type == 1) {
            $("#addContact").click(function () {
                _self.addContact();
            });
        } else {
            $(".tab-nav-ul li[data-id='navContact']").remove();
        }

        //切换模块
        $(".tab-nav-ul li").click(function () {
            var _this = $(this);
            _this.siblings().removeClass("hover");
            _this.addClass("hover");
            $(".nav-partdiv").hide();
            $("#" + _this.data("id")).show();

            $("#addContact").hide();

            if (_this.data("id") == "navLog" && (!_this.data("first") || _this.data("first") == 0)) {
                _this.data("first", "1");
                _self.getLogs(model.CustomerID, 1);
            } else if (_this.data("id") == "navContact") {
                $("#addContact").show();
                if ((!_this.data("first") || _this.data("first") == 0)) {
                    _this.data("first", "1");
                    _self.getContacts(model.CustomerID);
                }
            } else if (_this.data("id") == "navOrder" && (!_this.data("first") || _this.data("first") == 0)) {
                _this.data("first", "1");
                _self.getOrders(model.CustomerID, 1);
            } else if (_this.data("id") == "navOppor" && (!_this.data("first") || _this.data("first") == 0)) {
                _this.data("first", "1");
                _self.getOpportunitys(model.CustomerID, 1);
            }
        });


        $("#editContact").click(function () {
            var _this = $(this);
            Global.post("/Customer/GetContactByID", { id: _this.data("id") }, function (data) {
                _self.addContact(data.model);
            });
        });

        //删除联系人
        $("#deleteContact").click(function () {
            var _this = $(this);
            confirm("确认删除此联系人吗？", function () {
                Global.post("/Customer/DeleteContact", { id: _this.data("id") }, function (data) {
                    if (data.status) {
                        _self.getContacts(_self.customerid);
                    } else {
                        alert("网络异常,请稍后重试!");
                    }
                });
            });
        });

        require.async("sharemingdao", function () {
            $("#btn_shareMD").sharemingdao({
                post_pars: {
                    content: model.Name,
                    groups: [],
                    share_type: 0
                },
                task_pars: {
                    name: model.Name,
                    end_date: "",
                    charger: model.Owner,
                    members: [model.Owner],
                    des: "",
                    url: "/Customer/Detail?id=" + model.CustomerID + "&source=md"
                },
                schedule_pars: {
                    name: model.Name,
                    start_date: "",
                    end_date: "",
                    members: [model.Owner],
                    address: model.Address,
                    des: "",
                    url: "/Customer/Detail?id=" + model.CustomerID + "&source=md"
                },
                callback: function (type, url) {
                    if (type == "Calendar") {
                        url = "<a href='" + url + "' target='_blank'>分享明道日程，点击查看详情</a>";
                    } else if (type == "Task") {
                        url = "<a href='" + url + "' target='_blank'>分享明道任务，点击查看详情</a>";
                    }

                    var entity = {
                        GUID: model.CustomerID,
                        Content: encodeURI(url),
                        FromReplyID: "",
                        FromReplyUserID: "",
                        FromReplyAgentID: ""
                    };
                    _self.saveReply(entity);
                }
            });
        });

    }
    //获取日志
    ObjectJS.getLogs = function (customerid, page) {
        var _self = this;
        $("#customerLog").empty();
        Global.post("/Customer/GetCustomerLogs", {
            customerid: customerid,
            pageindex: page
        }, function (data) {
           
            doT.exec("template/common/logs.html", function (template) {
                var innerhtml = template(data.items);
                innerhtml = $(innerhtml);
                $("#customerLog").append(innerhtml);
            });
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
        });
    }
    //获取订单
    ObjectJS.getOrders = function (customerid, page) {
        var _self = this;
        $("#navOrder .tr-header").nextAll().remove();
        Global.post("/Orders/GetOrdersByCustomerID", {
            customerid: customerid,
            pagesize: 10,
            pageindex: page
        }, function (data) {

            doT.exec("template/orders/cuatomerorders.html", function (template) {
                var innerhtml = template(data.items);
                console.log(innerhtml);

                innerhtml = $(innerhtml);
                $("#navOrder .tr-header").after(innerhtml);
            });
            $("#pagerOrders").paginate({
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
                    _self.getOrders(customerid, page);
                }
            });
        });
    }

    //获取订单
    ObjectJS.getOpportunitys = function (customerid, page) {
        var _self = this;
        $("#navOppor .tr-header").nextAll().remove();
        Global.post("/Orders/GetOpportunityaByCustomerID", {
            customerid: customerid,
            pagesize: 10,
            pageindex: page
        }, function (data) {

            doT.exec("template/orders/customeroppors.html", function (template) {
                var innerhtml = template(data.items);
                console.log(innerhtml);

                innerhtml = $(innerhtml);
                $("#navOppor .tr-header").after(innerhtml);
            });
            $("#pagerOppors").paginate({
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
                    _self.getOpportunitys(customerid, page);
                }
            });
        });
    }

    ObjectJS.getContacts = function (customerid) {
        var _self = this;
        $("#navContact .tr-header").nextAll().remove();
        Global.post("/Customer/GetContacts", {
            customerid: customerid
        }, function (data) {
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
        });
    }

    ObjectJS.addContact = function (model) {
        var _self = this;
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
                            CustomerID: _self.customerid,
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

        Global.post("/Customer/SaveContact", { entity: JSON.stringify(model) }, function (data) {
            if (data.model.ContactID) {
                _self.getContacts(model.CustomerID);
            } else {
                alert("网络异常,请稍后重试!");
            }
        });
    }
    //编辑信息
    ObjectJS.editCustomer = function (model) {
        var _self = this;
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
                            Type: $("#companyCustom").hasClass("ico-checked") ? 1 : 0,
                            IndustryID: $("#industry").val().trim(),
                            Extent: $("#extent").val().trim(),
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

            $("#extent").val(model.Extent);

            $("#industry").val(model.IndustryID);

            if (model.Type == 0) {
                $(".edit-company").hide();
            }
            //切换类型
            $(".customtype").click(function () {
                var _this = $(this);
                if (!_this.hasClass("ico-checked")) {
                    $(".customtype").removeClass("ico-checked").addClass("ico-check");
                    _this.addClass("ico-checked").removeClass("ico-check");
                    if (_this.data("type") == 1) {
                        $(".edit-company").show();
                    } else {
                        $(".edit-company").hide();
                    }
                }
            });
        });
    }
    //保存实体
    ObjectJS.saveModel = function (model) {
        var _self = this;

        Global.post("/Customer/SaveCustomer", { entity: JSON.stringify(model) }, function (data) {
            if (data.model.CustomerID) {
                location.href = location.href;
                
            } else {
                alert("网络异常,请稍后重试!");
            }
        });
    }
    //讨论备忘
    ObjectJS.initTalk = function (customerid) {
        var _self = this;

        $("#btnSaveTalk").click(function () {
            var txt = $("#txtContent");
            if (txt.val().trim()) {
                var model = {
                    GUID: customerid,
                    Content: txt.val().trim(),
                    FromReplyID: "",
                    FromReplyUserID: "",
                    FromReplyAgentID: ""
                };
                _self.saveReply(model);

                txt.val("");
            }
            
        });
        _self.getReplys(customerid, 1);

    }
    //获取备忘
    ObjectJS.getReplys = function (customerid, page) {
        var _self = this;
        $("#replyList").empty();
        Global.post("/Customer/GetReplys", {
            guid: customerid,
            pageSize: 10,
            pageIndex: page
        }, function (data) {
            doT.exec("template/customer/replys.html", function (template) {
                var innerhtml = template(data.items);
                innerhtml = $(innerhtml);

                $("#replyList").append(innerhtml);

                innerhtml.find(".btn-reply").click(function () {
                    var _this = $(this), reply = _this.nextAll(".reply-box");
                    reply.slideDown(500);
                    reply.find("textarea").focus();
                    reply.find("textarea").blur(function () {
                        if (!$(this).val().trim()) {
                            reply.slideUp(200);
                        }
                    });
                });
                innerhtml.find(".save-reply").click(function () {
                    var _this = $(this);
                    if ($("#Msg_" + _this.data("replyid")).val().trim()) {
                        var entity = {
                            GUID: _this.data("id"),
                            Content: $("#Msg_" + _this.data("replyid")).val().trim(),
                            FromReplyID: _this.data("replyid"),
                            FromReplyUserID: _this.data("createuserid"),
                            FromReplyAgentID: _this.data("agentid")
                        };

                        _self.saveReply(entity);
                    }

                    $("#Msg_" + _this.data("replyid")).val('');
                    $(this).parent().slideUp(100);
                });

                require.async("businesscard", function () {
                    innerhtml.find("img").businessCard();
                });
            });

            $("#pagerReply").paginate({
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
                    _self.getReplys(customerid, page);
                }
            });
        });
    }
    ObjectJS.saveReply = function (model) {
        var _self = this;

        Global.post("/Customer/SavaReply", { entity: JSON.stringify(model) }, function (data) {
            doT.exec("template/customer/replys.html", function (template) {
                var innerhtml = template(data.items);
                innerhtml = $(innerhtml);

                $("#replyList").prepend(innerhtml);

                innerhtml.find(".btn-reply").click(function () {
                    var _this = $(this), reply = _this.nextAll(".reply-box");
                    reply.slideDown(500);
                    reply.find("textarea").focus();
                    reply.find("textarea").blur(function () {
                        if (!$(this).val().trim()) {
                            reply.slideUp(200);
                        }
                    });
                });
                innerhtml.find(".save-reply").click(function () {
                    var _this = $(this);
                    if ($("#Msg_" + _this.data("replyid")).val().trim()) {
                        var entity = {
                            GUID: _this.data("id"),
                            Content: $("#Msg_" + _this.data("replyid")).val().trim(),
                            FromReplyID: _this.data("replyid"),
                            FromReplyUserID: _this.data("createuserid"),
                            FromReplyAgentID: _this.data("agentid")
                        };
                        _self.saveReply(entity);
                    }
                    $("#Msg_" + _this.data("replyid")).val('');
                    $(this).parent().slideUp(100);
                });

                require.async("businesscard", function () {
                    innerhtml.find("img").businessCard();
                });
            });
        });
    }
    module.exports = ObjectJS;
});