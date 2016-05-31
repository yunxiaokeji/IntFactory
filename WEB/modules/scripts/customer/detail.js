define(function (require, exports, module) {
    var Global = require("global"),
        City = require("city"), CityObject, CityContact,
        Verify = require("verify"), VerifyObject, VerifyContact,
        doT = require("dot"),
        ChooseUser = require("chooseuser"),
        Easydialog = require("easydialog");
    var CustomerReply = require("scripts/task/reply");       
    require("pager");
    require("mark");
    
    var ObjectJS = {}, CacheIems = [];
    
    //初始化
    ObjectJS.init = function (customerid, MDToken) {
        var _self = this;
        _self.guid = customerid;

        CustomerReply.initTalkReply(_self, "customer");

        Global.post("/Customer/GetCustomerByID", { customerid: customerid }, function (data) {
            if (data.model.CustomerID) {
                _self.bindCustomerInfo(data.model);
                _self.bindEvent(data.model);
            }
        });
        
        $("#addContact").hide();       

    }

    ObjectJS.isLoading = true;

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

        //if (model.Type == 0) {
        //    $("#lblType").html("人")
        //    $(".companyinfo").hide();
        //} else {
        //    $("#lblType").html("企")
        //    $(".companyinfo").show();
        //}

        //处理阶段
        //var stage = $(".stage-items li[data-id='" + model.StageID + "']");
        //stage.addClass("hover");
        //if (model.Stage) {
        //    CacheIems[model.StageID] = model.Stage.StageItem;
        //    if (model.Stage.StageItem) {
        //        _self.bindStageItems(model.Stage.StageItem);
        //    }
        //}

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
                });
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
                });
            });

            //切换阶段
            $(".stage-items li").click(function () {
                if (!ObjectJS.isLoading) {
                    return;
                }
                var _this = $(this);
                !_this.hasClass("hover") && confirm("确认客户切换到此阶段吗?", function () {
                    ObjectJS.isLoading = false;
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
                        ObjectJS.isLoading = true;
                    });
                });
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
                            alert("请选择不同人员进行更换!");
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


        
        //切换模块
        $(".module-tab li").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }           

            var _this = $(this);
            _this.siblings().removeClass("hover");
            _this.addClass("hover");
            $(".nav-partdiv").hide();
            $(".task-replys").hide();
            $("#" + _this.data("id")).show();

            $("#addContact").hide();
            $(".searth-module").hide();

            if (_this.data("id") == "navLog" && (!_this.data("first") || _this.data("first") == 0)) {                
                _this.data("first", "1");
                _self.getLogs(model.CustomerID, 1);
            }else if (_this.data("id") == "navContact") {
                $("#addContact").show();
                
                if ((!_this.data("first") || _this.data("first") == 0)) {
                    _this.data("first", "1");
                    _self.getContacts(model.CustomerID);
                }
            } else if (_this.data("id") == "navOrder") {
                $(".searth-module").show();
                $(".search-ipt,.search-ico").remove();
                if ((!_this.data("first") || _this.data("first") == 0)) {
                    _this.data("first", "1");
                    _self.getOrders(model.CustomerID, 1);
                }
                //关键字搜索
                require.async("search", function () {
                    $(".searth-module").searchKeys(function () {
                        _self.getOrders(model.CustomerID, 1);
                        
                    });
                });
            } else if (_this.data("id") == "navOppor") {
                $(".searth-module").show();
                $(".search-ipt,.search-ico").remove();
                if ((!_this.data("first") || _this.data("first") == 0)) {
                    _this.data("first", "1");
                    _self.getOpportunitys(model.CustomerID, 1);
                }
                //关键字搜索
                require.async("search", function () {
                    $(".searth-module").searchKeys(function () {
                        _self.getOrders(model.CustomerID, 1);

                    });
                });

            } else if (_this.data("id") == "navDHOrder") {
                $(".searth-module").show();
                $(".search-ipt,.search-ico").remove();
                if ((!_this.data("first") || _this.data("first") == 0)) {
                    _this.data("first", "1");
                    _self.getDHOrders(model.CustomerID, 1);
                }
                //关键字搜索
                require.async("search", function () {
                    $(".searth-module").searchKeys(function () {
                        _self.getDHOrders(model.CustomerID, 1);
                       
                    });
                });
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
                        alert("网络异常,请稍后重试!");
                    }
                    ObjectJS.isLoading = true;
                });
            });
        });
    }

    //获取日志
    ObjectJS.getLogs = function (customerid, page) {
        var _self = this;
        $("#customerLog").empty();
        $("#customerLog").nextAll().remove();
        $("#customerLog").after("<div class='data-loading' ><div>");
        ObjectJS.isLoading = false;
        Global.post("/Customer/GetCustomerLogs", {
            customerid: customerid,
            pageindex: page
        }, function (data) {
            if (data.items.length>0) {
                doT.exec("template/common/logs.html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);
                    $("#customerLog").append(innerhtml);
                });
                $(".data-loading").remove();
            } else {
                $(".data-loading").remove();
                $("#customerLog").after("<div class='nodata-txt' >暂无日志!<div>");
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

    //获取订单
    ObjectJS.getOrders = function (customerid, page) {
        var _self = this;
        $("#navOrder .tr-header").nextAll().remove();
        $("#navOrder .tr-header").after("<tr><td colspan='12'><div class='data-loading' ><div></td></tr>");
        ObjectJS.isLoading = false;
        Global.post("/Orders/GetOrdersByCustomerID", {
            customerid: customerid,
            ordertype: 1,
            pagesize: 10,
            pageindex: page
        }, function (data) {
            $("#navOrder .tr-header").nextAll().remove();
            if (data.items.length > 0) {
                doT.exec("template/orders/customerorders.html", function (template) {
                    var innerhtml = template(data.items);

                    innerhtml = $(innerhtml);

                    innerhtml.find(".mark").markColor({
                        isAll: false,
                        onChange: function (obj, callback) {
                            _self.markOrders(obj.data("id"), obj.data("value"), callback);
                        }
                    });

                    $("#navOrder .tr-header").after(innerhtml);
                });
            } else {
                $("#navOrder .tr-header").after("<tr><td colspan='12'><div class='nodata-txt' >暂无订单!<div></td></tr>");
            }
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
            ObjectJS.isLoading = true;
        });
    }

    //获取大货订单
    ObjectJS.getDHOrders = function (customerid, page) {
        var _self = this;
        $("#navDHOrder .tr-header").nextAll().remove();
        $("#navDHOrder .tr-header").after("<tr><td colspan='12'><div class='data-loading' ><div></td></tr>");
        ObjectJS.isLoading = false;
        Global.post("/Orders/GetOrdersByCustomerID", {
            customerid: customerid,
            ordertype: 2,
            pagesize: 10,
            pageindex: page
        }, function (data) {
            $("#navDHOrder .tr-header").nextAll().remove();
            if (data.items.length > 0) {
                doT.exec("template/orders/customerorders.html", function (template) {
                    var innerhtml = template(data.items);

                    innerhtml = $(innerhtml);
                    innerhtml.find(".mark").markColor({
                        isAll: false,
                        onChange: function (obj, callback) {
                            _self.markOrders(obj.data("id"), obj.data("value"), callback);
                        }
                    });

                    $("#navDHOrder .tr-header").after(innerhtml);
                });
            } else {
                $("#navDHOrder .tr-header").after("<tr><td colspan='12'><div class='nodata-txt' >暂无订单!<div></td></tr>");
            }
            $("#pagerDHOrders").paginate({
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
                    _self.getDHOrders(customerid, page);
                }
            });
            ObjectJS.isLoading = true;
        });
    }

    //获取需求
    ObjectJS.getOpportunitys = function (customerid, page) {
        var _self = this;
        $("#navOppor .tr-header").nextAll().remove();
        $("#navOppor .tr-header").after("<tr><td colspan='10'><div class='data-loading' ><div></td></tr>");
        ObjectJS.isLoading = false;
        Global.post("/Orders/GetNeedsOrderByCustomerID", {
            customerid: customerid,
            pagesize: 10,
            pageindex: page
        }, function (data) {
            $("#navOppor .tr-header").nextAll().remove();
            if (data.items.length > 0) {
                doT.exec("template/orders/customeroppors.html", function (template) {
                    var innerhtml = template(data.items);

                    innerhtml = $(innerhtml);

                    innerhtml.find(".mark").markColor({
                        isAll: false,
                        onChange: function (obj, callback) {
                            _self.markOrders(obj.data("id"), obj.data("value"), callback);
                        }
                    });
                    $("#navOppor .tr-header").after(innerhtml);
                });
            } else {
                $("#navOppor .tr-header").after("<tr><td colspan='10'><div class='nodata-txt' >暂无需求!<div></td></tr>");
            }
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
            ObjectJS.isLoading = true;
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
                alert("网络异常,请稍后重试!");
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

            //$("#extent").val(model.Extent);

            //$("#industry").val(model.IndustryID);


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
                alert("网络异常,请稍后重试!");
            }
            ObjectJS.isLoading = true;
        });
    }

    //标记订单
    ObjectJS.markOrders = function (ids, mark, callback) {
        if (mark < 0) {
            alert("不能标记此选项!");
            return false;
        }
        ObjectJS.isLoading = false;
        Global.post("/Orders/UpdateOrderMark", {
            ids: ids,
            mark: mark
        }, function (data) {
            if (data.result == "10001") {
                alert("您没有标记订单的权限！");
                callback && callback(false);
            } else {
                callback && callback(data.status);
            }
            ObjectJS.isLoading = true;
        });
    }


    module.exports = ObjectJS;
});