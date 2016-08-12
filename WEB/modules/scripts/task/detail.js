define(function (require, exports, module) {
    var DoT = require("dot");
    var Global = require("global");
    var Upload = require("upload");
    var Easydialog = require("easydialog");
    var Pager = require("pager");
    var Tip = require("tip");
    var TalkReply = null;
    var ChooseUser = null;
    var CutoutDoc = null;
    var SewnDoc = null;
    var SendOrders = null;
    var SendDYOrders = null;
    var ProcessDYCosts = null;
    
    var CacheAttrValues = [];//订单品类属性缓存
    var PlateMakings = [];//制版工艺说明
    var ObjectJS = {};
    var ChooseProduct = null;

    var plateMartingItem = [];//制版工艺类型缓存
    ///taskid：任务id
    ///orderid:订单id
    ///stageid：订单阶段id
    ///mark:任务标记 11：材料 12 制版 21大货材料
    ///finishStatus：任务完成状态
    ///attrValues:订单品类属性
    ///orderType:订单类型
    ObjectJS.init = function (attrValues, orderimages, isWarn, task, originalID, orderPlanTime, plateMarkItems, taskDescs) {
        var task = JSON.parse(task.replace(/&quot;/g, '"'));

        if (plateMarkItems) {
            plateMartingItem = JSON.parse(plateMarkItems.replace(/&quot;/g, '"'));
        } 
        /*任务模块别名*/
        var taskModeleDescs = JSON.parse(taskDescs.replace(/&quot;/g, '"'));

        if (attrValues != "")
            CacheAttrValues = JSON.parse(attrValues.replace(/&quot;/g, '"'));//制版属性缓存
        ObjectJS.orderid = task.OrderID;
        ObjectJS.guid = task.OrderID;
        ObjectJS.originalID = originalID;
        ObjectJS.processID = task.ProcessID;
        ObjectJS.taskid = task.TaskID;
        ObjectJS.stageid = task.StageID;
        ObjectJS.orderType = task.OrderType;
        ObjectJS.ownerid = task.OwnerID;
        ObjectJS.endTime = task.EndTime.toDate("yyyy/MM/dd hh:mm:ss");
        ObjectJS.finishStatus = task.FinishStatus;
        ObjectJS.status = task.Status;
        ObjectJS.maxHours = task.MaxHours;
        ObjectJS.planTime = orderPlanTime;
        ObjectJS.isWarn = isWarn;
        ObjectJS.orderimages = orderimages;
        ObjectJS.isPlate = true;//任务是否制版
        ObjectJS.mark = task.Mark;//任务标记 用于做标记任务完成的限制条件
        ObjectJS.materialMark = 0;//任务材料标记 用于算材料列表的金额统计
        ObjectJS.lockStatus = task.LockStatus;
        ObjectJS.isLoading = true;

        //材料任务
        if ($("#btn-addMaterial").length == 1) {
            ObjectJS.materialMark = 1;
            if (ObjectJS.mark == 11 && ObjectJS.orderType == 2) {
                ObjectJS.materialMark = 2;
            }
            ObjectJS.initTaskProduct();
        }
         //制版任务
        else if ($("#btn-updateTaskRemark").length == 1) {
            ObjectJS.bindPlatemakingEvent();
            ObjectJS.initPlateMaking();
        }
        else {
            if (ObjectJS.mark == 12 || ObjectJS.mark == 22) {
                ObjectJS.removeTaskPlateOperate();
            }
        }

        //统计材料列表总金额
        ObjectJS.getProductAmount();

        //获取订单所有任务阶段
        ObjectJS.getAllTaskStages();

        //判断制版任务是否已执行
        if (ObjectJS.mark == 12 || ObjectJS.mark == 22) {
            if ($("#platemakingBody .table-list").length == 0) {
                ObjectJS.isPlate = false;
            }
        }
        if (ObjectJS.mark === 13 && ObjectJS.orderType == 2) {
            var taskDesc = "裁剪";
            for (var i = 0; i < taskModeleDescs.length; i++) {
                var item = taskModeleDescs[i];
                if (item.Mark == 13) {
                    taskDesc = item.Name || '裁剪';
                    break;
                }
            }
            CutoutDoc = require("scripts/task/cutoutdoc");
            CutoutDoc.initCutoutDoc(ObjectJS.orderid, ObjectJS.taskid, Global, DoT, Easydialog, taskDesc);
        }
        else if (ObjectJS.mark === 14 && ObjectJS.orderType == 2) {
            var taskDesc="车缝";
            for (var i = 0; i < taskModeleDescs.length; i++) {
                var item = taskModeleDescs[i];
                if (item.Mark == 14) {
                    taskDesc = item.Name|| '车缝';
                    break;
                }
            }
            SewnDoc = require("scripts/task/sewndoc");
            SewnDoc.initSewnDoc(ObjectJS.orderid, ObjectJS.taskid, Global, DoT, Easydialog, taskDesc);
        }
        else if (ObjectJS.mark === 15 && ObjectJS.orderType == 2) {
            SendOrders = require("scripts/task/sendorders");
            SendOrders.initSendOrders(ObjectJS.orderid, ObjectJS.taskid, Global, DoT, Easydialog);
        }
        else if (ObjectJS.mark === 15 && ObjectJS.orderType == 1) {
            SendDYOrders = require("scripts/task/senddyorders");
            SendDYOrders.initSendDYOrders(ObjectJS.orderid, ObjectJS.taskid, Global, DoT, Easydialog);
        }
        else if (ObjectJS.mark === 16) {
            ProcessDYCosts = require("scripts/task/processcostdoc");
            ProcessDYCosts.initProcessCosts(ObjectJS.orderid, Global, DoT, ObjectJS.orderType);
        }

        TalkReply = require("replys");
        TalkReply.initTalkReply({
            element: "#taskReplys",
            guid: ObjectJS.taskid,
            type: 10, /*1 客户 2订单 10任务 */
            pageSize: 10,
            noGet: true
        });

        //事件绑定
        ObjectJS.bindBaseEvent();
    };

    //#region任务基本信息操作
    //绑定事件
    ObjectJS.bindBaseEvent = function () {
        //显示预警时间
        ObjectJS.showWarnTime();

        //绑定任务样式图
        ObjectJS.bindOrderImages();

        if ($("#btnChooseProduct").length == 1) {
            ChooseProduct = require("chooseproduct");
            //快捷添加产品
            $("#btnChooseProduct").click(function () {
                ChooseProduct.create({
                    title: "选择采购材料",
                    type: 11, //1采购 2出库 3报损 4报溢 5调拨
                    wareid: ObjectJS.guid,
                    callback: function (products) {
                        if (products.length > 0) {
                            var entity = {}, items = [];
                            entity.guid = ObjectJS.guid;
                            entity.type = 11;
                            for (var i = 0; i < products.length; i++) {
                                items.push({
                                    ProductID: products[i].pid,
                                    ProductDetailID: products[i].did,
                                    BatchCode: products[i].batch,
                                    DepotID: products[i].depotid,
                                    Description: products[i].remark,
                                });
                            }
                            entity.Products = items;
                            Global.post("/ShoppingCart/AddShoppingCartBatchIn", { entity: JSON.stringify(entity) }, function (data) {
                                if (data.status) {
                                    location.href = location.href;
                                }
                            });
                        }
                    }
                });
            });
        }
        //任务模块切换
        $(".module-tab li").click(function () {
            var _this = $(this);
            if (!ObjectJS.isLoading) {
                return;
            }
            if (_this.hasClass("hover")) {
                return;
            }
            _this.addClass("hover").siblings().removeClass("hover");
            $("#navTask").children().hide();
            $("#" + _this.data("id")).show();

            if (_this.data("id") == "orderTaskLogs") {
                if (!_this.data("isget")) {                    
                    _this.data("isget", "1");
                    require.async("logs", function () {
                        $("#orderTaskLogs").getObjectLogs({
                            guid: ObjectJS.taskid,
                            type: 10, /*1 客户 2订单 10任务 */
                            pageSize: 10
                        });
                    });
                }
            }
            else if (_this.data("id") == "navGoods") {
                if (!_this.data("isget")) {
                    ObjectJS.getOrderGoods();
                    _this.data("isget", "1");
                }
            }
            else if (_this.data("id") == "platemakingContent") {
                if (!_this.data("isget")) {
                    ObjectJS.getPlateMakings();
                    _this.data("isget", "1");
                }
            }
            else if (_this.data("id") == "taskReplys") {
                if (!_this.data("isget")) {
                    TalkReply.getTaskReplys(1);
                    _this.data("isget", "1");
                }
            }
            else if (_this.data("id") == "navCutoutDoc") {
                $("#btnMaterialOrder").show();
                //材料录入
                $("#btnMaterialOrder").unbind().click(function () {
                    Global.post("/Task/GetOrderDetailsByOrderID", { orderid: ObjectJS.orderid }, function (data) {
                        DoT.exec("template/task/material-add.html", function (template) {
                            var innerHtml = template(data.items);
                            Easydialog.open({
                                container: {
                                    id: "showMaterial",
                                    header: "用料登记",
                                    content: innerHtml,
                                    yesFn: function () {
                                        var details = "";
                                        var isContinue = false;
                                        $("#showMaterial .table-list .list-item").each(function () {
                                            var _thisTr = $(this);
                                            if (_thisTr.find('.quantity').val() * 1 > 0) {
                                                details += _thisTr.data('id') + '|' + _thisTr.find('.quantity').val() + ',';
                                            }
                                        });
                                        if (details.length > 0) {
                                            Global.post("/Task/CreateProductUseQuantity", {
                                                orderID: ObjectJS.orderid,
                                                details: details
                                            }, function (data) {
                                                if (data.result == 1) {
                                                    alert("登记成功");
                                                } else {
                                                    alert("网络异常，请重试");
                                                    return false;
                                                }
                                            });
                                        } else {
                                            isContinue = true;
                                            alert("登记数量必须大于0");
                                            return false;
                                        }
                                    }
                                }
                            });
                            $(".quantity").change(function () {
                                var _this = $(this);
                                if (!_this.val().isDouble() || _this.val() <= 0) {
                                    _this.val(0);
                                    return false;
                                }
                                if (_this.val() > _this.parents('tr').find('.purchase-count').text() * 1) {
                                    //alert("录入材料量大于采购量");
                                }
                            });
                        });
                    });
                });
                CutoutDoc.getCutoutDoc();
            }
            else if (_this.data("id") == "navSewnDoc") {
                SewnDoc.getSewnDoc();
            }
            else if (_this.data("id") == "navSendDoc") {
                SendOrders.getSendDoc();
            }
            else if (_this.data("id") == "navSendDYDoc") {
                SendDYOrders.getSendDYDoc();
            }
        });

        $(".module-tab li.default-check").click();

        //标记任务完成
        if ($("#FinishTask").length == 1) {
            $("#FinishTask").click(function () {
                if (!ObjectJS.isLoading) {
                    return;
                }

                ObjectJS.finishTask();
            });
        }

        //接受任务
        if ($("#AcceptTask").length == 1) {
            $("#AcceptTask").click(function () {
                if (!ObjectJS.isLoading) {
                    return;
                }
                ObjectJS.updateTaskEndTime();
            });
        }

        //锁定任务
        if ($("#LockTask").length == 1) {
            $("#LockTask").click(function () {
                ObjectJS.lockTask();
            })
        }
            
        if ($("#addTaskMembers").length == 1) {
            ChooseUser = require("chooseuser");
            //添加任务成员
            $("#addTaskMembers").click(function () {
                if (!ObjectJS.isLoading) {
                    return;
                }

                ChooseUser.create({
                    title: "添加任务成员",
                    type: 1,
                    single: false,
                    callback: function (items) {
                        var memberIDs = '';
                        for (var i = 0; i < items.length; i++) {
                            var item = items[i];
                            if (ObjectJS.ownerid == item.id) {
                                continue;
                            }
                            if ($(".memberlist" + " tr[data-id='" + item.id + "']").html()) {
                                continue;
                            }

                            ObjectJS.createTaskMember(item);
                            memberIDs += item.id + ",";
                        }

                        if (memberIDs != '') {
                            ObjectJS.addTaskMembers(memberIDs);
                        }
                    }
                });

            });

            //任务负责人更改成员权限
            ObjectJS.bindUpdateMemberPermission();

            //列表删除任务成员
            ObjectJS.bindRemoveTaskMember();
        }
    }

    //更改任务到期时间
    ObjectJS.updateTaskEndTime = function () {
        if (ObjectJS.maxHours == 0) {
            Easydialog = require("easydialog");

            DoT.exec("/template/task/set-endtime.html", function (template) {
                var innerHtml = template();
                Easydialog.open({
                    container: {
                        id: "show-model-setRole",
                        header: "设置任务到期时间",
                        content: innerHtml,
                        yesFn: function () {


                            var showMsg="任务到期时间不可逆，确定设置?";
                            var planTime = new Date(ObjectJS.planTime).getTime();
                            var endTime = new Date($("#UpdateTaskEndTime").val()).getTime();

                            //判断该任务的订单是否超期
                            var isExceed = new Date().getTime() < planTime ? true : false;
                           
                            if (planTime < endTime && isExceed) {
                                showMsg = "已超出订单交货时间,确定设置?";
                            }
                            if ($("#UpdateTaskEndTime").val() == "") {
                                alert("任务到期时间不能为空");
                                return;
                            }
                            confirm(showMsg, function () {
                                ObjectJS.isLoading = false;
                                Global.post("/Task/UpdateTaskEndTime", {
                                    id: ObjectJS.taskid,
                                    endTime: $("#UpdateTaskEndTime").val()
                                }, function (data) {
                                    if (data.result == 0) {
                                        alert("操作无效");
                                    }
                                    else if (data.result == 2) {
                                        alert("任务已接受,不能操作");
                                    }
                                    else if (data.result == 3) {
                                        alert("没有权限操作");
                                    }
                                    else {
                                        location.href = location.href;
                                    }
                                    ObjectJS.isLoading = true;
                                });
                            });

                        }
                    }
                });

                var myDate = new Date();
                var minDate = myDate.toLocaleDateString();
                minDate = minDate + " 23:59:59"
                //if (ObjectJS.planTime <= minDate) {
                //    ObjectJS.planTime = '';
                //}
                //更新任务到期日期
                var taskEndTime = {
                    elem: '#UpdateTaskEndTime',
                    format: 'YYYY/MM/DD hh:mm:ss',
                    min: minDate,
                    //max: ObjectJS.planTime,
                    istime: true,
                    istoday: false
                };
                laydate(taskEndTime);
               
            });

        }
        else {
            Global.post("/Task/UpdateTaskEndTime", {
                id: ObjectJS.taskid,
                endTime: ""
            }, function (data) {
                if (data.result == 0) {
                    alert("操作无效");
                }
                else if (data.result == 2) {
                    alert("任务已接受,不能操作");
                }
                else if (data.result == 3) {
                    alert("没有权限操作");
                }
                else {
                    location.href = location.href;
                }
                ObjectJS.isLoading = true;
            });
        }
    }

    //标记任务完成
    ObjectJS.finishTask = function () {
        var mark = ObjectJS.mark;
        var confirmMsg = '确定标记完成';
        if (mark == 11) {
            if ($("#navProducts .table-list tr").length == 2) {
                confirmMsg="材料没有添加,不能标记任务完成";
            }
        }
        else if (mark == 12) {
            if ($("#platemakingBody .table-list").length == 0) {
                confirmMsg="制版没有设置,不能标记任务完成";
            }
            else if (!ObjectJS.isPlate) {
                confirmMsg="制版没有设置,不能标记任务完成";
            }
        }
        else if (mark == 15 || mark == 25) {
            if ($(".nav-partdiv .list-item").length == 0) {
                confirmMsg = '还没发货,确定标记完成';
            }
        }
        else if (mark == 16) {
            if ($(".nav-partdiv .list-item").length == 0) {
                confirmMsg = '还没录入加工成本,确定标记完成';
            }
        }
        else if (mark == 23) {
            if ($(".nav-partdiv .list-item").length == 0) {
                confirmMsg = '还没裁剪,确定标记完成';
            }
        }
        else if (mark == 24) {
            if ($(".nav-partdiv .list-item").length == 0) {
                confirmMsg = '还没车缝,确定标记完成';
            }
        }
        confirm(confirmMsg+"?", function () {
            $("#FinishTask").val("完成中...").attr("disabled", "disabled");
            ObjectJS.isLoading = false;

            Global.post("/Task/FinishTask",
               {
                   id: ObjectJS.taskid
               },
               function (data) {
                   $("#FinishTask").val("标记完成").removeAttr("disabled");
                   if (data.result == 1) {
                       location.href = location.href;
                   }
                   else if (data.result == 2) {
                       alert("前面阶段任务有未完成,不能标记完成");
                   }
                   else if (data.result == 3) {
                       alert("无权限操作");
                   }
                   else if (data.result == 4) {
                       alert("任务没有接受，不能设置完成");
                   }
                   else if (data.result == -1) {
                       alert("保存失败");
                   }
                   ObjectJS.isLoading = true;
               });
        });
    }

    //锁定任务
    ObjectJS.lockTask = function () {
        confirm("锁定后不能对任务进行任何操作,确定要锁定?", function () {
            $("#LockTask").val("锁定中...").attr("disabled", "disabled");
            ObjectJS.isLoading = false;
            Global.post("/Task/LockTask", { taskID: ObjectJS.taskid }, function (data) {
                ObjectJS.isLoading = true;
                $("#LockTask").val("锁定任务").removeAttr("disabled");
                if (data == 1) {
                    location.href = location.href;
                } else {
                    alert("网络繁忙,解锁失败");
                }
            });
        })
    }

    //添加任务成员
    ObjectJS.addTaskMembers = function (memberIDs) {
        ObjectJS.isLoading = false;
        Global.post("/Task/AddTaskMembers", {
            id: ObjectJS.taskid,
            memberIDs: memberIDs
        }, function (data) {
            if (data.result == 0) {
                alert("添加失败");
            }
            else {
                //任务负责人更改成员权限
                ObjectJS.bindUpdateMemberPermission();

                //列表删除任务成员
                ObjectJS.bindRemoveTaskMember();
            }
            ObjectJS.isLoading = true;
        });
    }

    //删除任务成员
    ObjectJS.removeTaskMember = function (memberID) {
        ObjectJS.isLoading = false;

        Global.post("/Task/RemoveTaskMember", {
            id: ObjectJS.taskid,
            memberID: memberID
        }, function (data) {
            if (data.result == 0) {
                alert(memberIDs);
            }
            else {
                $(".memberlist tr[data-id='" + memberID + "']").remove();
            }

            ObjectJS.isLoading = true;
        });
    }

    //绑定任务更新成员权限
    ObjectJS.bindUpdateMemberPermission=function(){
        $('.check-lump').unbind().click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            var _this = $(this);
            var confirmMsg = "确定将" + _this.parents('tr').find('.membername').text() + "的权限设置为<span style='font-size:14px;color:red;'>" + (_this.data('type') == 1 ? "查看" : "编辑") + "</span>?";

            if (!_this.hasClass('hover')) {
                confirm(confirmMsg, function () {
                    Global.post("/Task/UpdateMemberPermission", {
                        taskID: _this.data('taskid'),
                        memberID: _this.data('memberid'),
                        type: _this.data('type')
                    }, function (data) {
                        if (data.result == 1) {
                            _this.parents('tr').find('.check-lump').removeClass('hover');
                            _this.addClass('hover');
                        } else {
                            alert('授权失败');
                        }
                    })
                });
            }
        });
    }

    //绑定任务删除任务成员
    ObjectJS.bindRemoveTaskMember=function(){
        $(".memberlist td.removeTaskMember").unbind().click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            var memberID = $(this).data("id");
            var confirmMsg = "确定删除成员<span style='color:red;font-size:14px;'>" + $(this).parents('tr').find('.membername').text() + "</span>?";

            confirm(confirmMsg, function () {
                ObjectJS.removeTaskMember(memberID);
            });
        });
    }

    //拼接任务成员html
    ObjectJS.createTaskMember = function (item) {
        var memberListHtml = '';
        memberListHtml += '<tr data-id="' + item.id + '" class="hide">';
        memberListHtml += '<td class="tLeft pLeft10"><i><img onerror="$(this).attr("src","/modules/images/defaultavatar.png"); src="' + (item.avatar == null ? "/modules/images/defaultavatar.png" : item.avatar) + '" /></i> <i class="membername">' + item.name + '</i></td>';
        memberListHtml += '<td><i class="hand ico-radiobox check-lump hover" data-taskid="' + ObjectJS.taskid + '" data-memberid="' + item.id + '" data-type=1 ><span></span></i></td>';
        memberListHtml += '<td><i class="hand ico-radiobox check-lump" data-taskid="' + ObjectJS.taskid + '" data-memberid="' + item.id + '" data-type=2 ><span></span></i></td>';
        memberListHtml += '<td class="removeTaskMember iconfont hand" data-id="' + item.id + '">&#xe651;</td>';
        $('.memberlist .member-items tr.member-nodata-txt').remove();
        memberListHtml = $(memberListHtml);
        $('.memberlist .member-items tbody').prepend(memberListHtml);
        memberListHtml.fadeIn(300);
    }

    //任务预警时间
    ObjectJS.showWarnTime = function () {
        if (ObjectJS.status == 8) {
            return;
        }
        if (ObjectJS.finishStatus!=1) {
            return;
        }

        var time_end = (new Date(ObjectJS.endTime)).getTime();
        var time_start = new Date().getTime(); //设定当前时间
        // 计算时间差 
        var time_distance = time_end - time_start;
        var overplusTime = false;

        if (time_distance < 0) {
            if (!overplusTime) {
                $("#overplusTime").html("超期时间");
                $(".taskBaseInfo .li-plustime .task-time").css("background-color", "red");
            }
            overplusTime = true;
            time_distance = time_start - time_end;
        }
        else
        {
            if (ObjectJS.isWarn==1) {
                if (!overplusTime) {
                    $(".taskBaseInfo .li-plustime .task-time").css({ "background-color": "orange", "color": "#fff" });
                }
                overplusTime = true;
            }
        }

        // 天
        var int_day = Math.floor(time_distance / 86400000)
        time_distance -= int_day * 86400000;
        // 时
        var int_hour = Math.floor(time_distance / 3600000)
        time_distance -= int_hour * 3600000;
        // 分
        var int_minute = Math.floor(time_distance / 60000)
        time_distance -= int_minute * 60000;
        // 秒 
        var int_second = Math.floor(time_distance / 1000)
        // 时分秒为单数时、前面加零 
        if (int_day < 10) {
            int_day = "0" + int_day;
        }
        if (int_hour < 10) {
            int_hour = "0" + int_hour;
        }
        if (int_minute < 10) {
            int_minute = "0" + int_minute;
        }
        if (int_second < 10) {
            int_second = "0" + int_second;
        }
        // 显示时间 
        $("#time-d").html(int_day);
        $("#time-h").html(int_hour);
        $("#time-m").html(int_minute);
        $("#time-s").html(int_second);

        // 设置定时器
        setTimeout(function () { ObjectJS.showWarnTime() }, 1000);
    }

    //绑定任务样式图
    ObjectJS.bindOrderImages = function () {
        var orderimages = ObjectJS.orderimages;
        var _self = this;
        var images = orderimages.split(",");
        _self.images = images;
        for (var i = 0; i < images.length; i++) {
            if (images[i]) {
                if (i == 0) {
                    $("#orderImage").attr("src", images[i].split("?")[0]);
                }
                var img = $('<li class="' + (i == 0 ? 'hover' : "") + '"><img src="' + images[i] + '" /></li>');
                $(".order-imgs-list").append(img);
            }
        }

        $(".order-imgs-list img").parent().click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                $("#orderImage").attr("src", _this.find("img").attr("src").split("?")[0]);
            }
        });

        //图片放大功能
        //图片放大功能
        var width = document.documentElement.clientWidth, height = document.documentElement.clientHeight;
        $("#orderImage").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            if ($(this).attr("src")) {

                $(".enlarge-image-bgbox,.enlarge-image-box").fadeIn();
                $(".right-enlarge-image,.left-enlarge-image").css({ "top": height / 2 - 80 })

                $(".enlarge-image-item").append('<img id="enlargeImage" src="' + $(this).attr("src").split("?")[0] + '"/>');
                $('#enlargeImage').smartZoom({ 'containerClass': 'zoomableContainer' });
                $(".left-enlarge-image").unbind().click(function () {
                    if (!ObjectJS.isLoading) {
                        return;
                    }
                    var ele = $(".order-imgs-list .hover").prev();
                    if (ele && ele.find("img").attr("src")) {
                        var _img = ele.find("img");
                        $(".order-imgs-list .hover").removeClass("hover");
                        ele.addClass("hover");
                        $("#orderImage").attr("src", _img.attr("src").split("?")[0]);
                        $(".enlarge-image-item").empty();
                        $(".enlarge-image-item").append('<img id="enlargeImage" src="' + _img.attr("src").split("?")[0] + '"/>');
                        $('#enlargeImage').smartZoom({ 'containerClass': 'zoomableContainer' });
                    }
                });

                $(".right-enlarge-image").unbind().click(function () {
                    if (!ObjectJS.isLoading) {
                        return;
                    }
                    var ele = $(".order-imgs-list .hover").next();
                    if (ele && ele.find("img").attr("src")) {
                        var _img = ele.find("img");
                        $(".order-imgs-list .hover").removeClass("hover");
                        ele.addClass("hover");
                        $("#orderImage").attr("src", _img.attr("src").split("?")[0]);
                        $(".enlarge-image-item").empty();
                        $(".enlarge-image-item").append('<img id="enlargeImage" src="' + _img.attr("src").split("?")[0] + '"/>');
                        $('#enlargeImage').smartZoom({ 'containerClass': 'zoomableContainer' });
                    }
                });
            }
        });

        $(".close-enlarge-image").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            $(".enlarge-image-bgbox,.enlarge-image-box").fadeOut();
            $(".enlarge-image-item").empty();
        });

        $(".enlarge-image-bgbox").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            $(".enlarge-image-bgbox,.enlarge-image-box").fadeOut();
            $(".enlarge-image-item").empty();
        });

        $(".zoom-botton").click(function (e) {
            if (!ObjectJS.isLoading) {
                return;
            }
            var scaleToAdd = 0.8;
            if (e.target.id == 'zoomOutButton')
                scaleToAdd = -scaleToAdd;
            $('#enlargeImage').smartZoom('zoom', scaleToAdd);
            return false;
        });


    }

    //汇总
    ObjectJS.getAmount = function () {
        //订单明细汇总
        $(".total-item td").each(function () {
            var _this = $(this), _total = 0;
            if (_this.data("class")) {
                $("." + _this.data("class")).each(function () {
                    _total += $(this).html() * 1;
                });
                if (_this.data("class") == "moneytotal") {
                    _this.html(_total.toFixed(2));
                } else {
                    _this.html(_total);
                }
            }
        });
    }
    //#endregion

    // #region任务材料操作
    //绑定事件
    ObjectJS.initTaskProduct = function () {

        //编辑价位
        $("#navProducts .price").change(function () {
            if (!ObjectJS.isLoading) {
                return;
            }

            if ($(this).val().isDouble() && $(this).val() >= 0) {
                ObjectJS.editPrice($(this));
            } else {
                $(this).val($(this).data("value"));
            }
        });

        //编辑数量
        $("#navProducts .quantity").change(function () {
            if (!ObjectJS.isLoading) {
                return;
            }

            if ($(this).val().isDouble() && $(this).val() > 0) {
                ObjectJS.editQuantity($(this));
            } else {
                $(this).val($(this).data("value"));
            }
        });

        //编辑损耗率
        $("#navProducts .loss").change(function () {
            if (!ObjectJS.isLoading) {
                return;
            }

            var lossRate = $(this).val();

            if (lossRate > -1) {
                ObjectJS.editLossRate($(this));
            } else {
                $(this).val($(this).data("value"));
            }
        });

        //删除产品
        $("#navProducts .ico-del").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            var _this = $(this);
            if (ObjectJS.orderType == 2) {
                var count = (_this.parents('tr').find('.purchase-count').text() * 1) + (_this.parents('tr').find('.inquantity').text() * 1);
                if (count > 0) {
                    alert("材料存在使用记录，无法删除！");
                    return false;
                }
            }
            confirm("确认从清单中移除此材料吗？", function () {
                Global.post("/Orders/DeleteProduct", {
                    orderid: ObjectJS.orderid,
                    autoid: _this.data("id"),
                    name: _this.data("name")
                }, function (data) {
                    if (!data.status) {
                        alert("材料存在使用记录，无法删除！");
                    }
                    else {
                        _this.parents("tr.item").remove();
                        ObjectJS.getProductAmount();
                    }
                });
            });
        });

        //生成采购单
        if ($("#btnEffectiveOrderProduct").length == 1) {
            $("#btnEffectiveOrderProduct").click(function () {
                if (!ObjectJS.isLoading) {
                    return;
                }

                ObjectJS.effectiveOrderProduct();
            });
        }

    };

    //更改材料单价
    ObjectJS.editPrice = function (ele) {
        var _self = this;
        ObjectJS.isLoading = false;

        Global.post("/Orders/UpdateOrderPrice", {
            orderid: _self.guid,
            autoid: ele.data("id"),
            name: ele.data("name"),
            price: ele.val()
        }, function (data) {
            if (!data.status) {
                ele.val(ele.data("value"));
                alert("当前订单状态,不能进行修改");
            }
            else {
                ele.data("value", ele.val());
                _self.getProductAmount();
            }
            ObjectJS.isLoading = true;
        });
    }

    //更改消耗量
    ObjectJS.editQuantity = function (ele) {
        var _self = this;
        ObjectJS.isLoading = false;
        ObjectJS.isLoading = false;
        ele.data('value', ele.val());
        var lossRata = ((ele.parents('tr').find('.tr-loss').find("input").val() * 1) / (ele.val() * 1)).toFixed(3);
        var amount = (ele.val() * 1) + (ele.parents('tr').find('.tr-loss').find("input").val() * 1);
        ele.parents('tr').find('.tr-lossrate').html(lossRata);
        ele.parents('tr').find('.amount-count').html(amount);
        Global.post("/Orders/UpdateProductQuantity", {
            orderid: _self.guid,
            autoid: ele.data("id"),
            name: ele.data("name"),
            quantity: ele.val()
        }, function (data) {
            if (!data.status) {
                ele.val(ele.data("value"));
                alert("当前订单状态,不能进行修改");
            }
            else {
                ele.data("value", ele.val());
                _self.getProductAmount();
            }
            ObjectJS.isLoading = true;
        });
    }

    //更改损耗量
    ObjectJS.editLossRate = function (ele) {
        var _self = this;
        ObjectJS.isLoading = false;
        ele.data('value', ele.val());
        var lossRata = ((ele.val() * 1) / (ele.parents('tr').find('.tr-quantity').find("input").val() * 1)).toFixed(3);
        var amount = (ele.val() * 1) + (ele.parents('tr').find('.tr-quantity').find("input").val() * 1);
        ele.parents('tr').find('.tr-lossrate').html(lossRata);
        ele.parents('tr').find('.amount-count').html(amount);

        Global.post("/Orders/UpdateProductLoss", {
            orderid: _self.guid,
            autoid: ele.data("id"),
            name: ele.data("name"),
            quantity: ele.val()
        }, function (data) {
            if (!data.status) {
                ele.val(ele.data("value"));
                alert("当前订单状态,不能进行修改");
            }
            else {
                ele.data("value", ele.val());
                _self.getProductAmount();
            }
            ObjectJS.isLoading = true;
        });
    }

    //生成采购单
    ObjectJS.effectiveOrderProduct = function () {
        ObjectJS.isLoading = false;
        confirm("生成采购单操作不可逆,确定要生成?", function () {
            Global.post("/Orders/EffectiveOrderProduct", {
                orderID: ObjectJS.orderid
            }, function (data) {
                if (data.result == 1) {
                    location.href = location.href;
                }
                ObjectJS.isLoading = true;
            });
        }, function () {
            ObjectJS.isLoading = true;
        })
      

    }

    //计算总金额
    ObjectJS.getProductAmount = function () {
        var amount = 0;
        $(".amount").each(function () {
            var _this = $(this);
            if (ObjectJS.materialMark == 0) {
                amount += _this.html() * 1;
            }
            else if (ObjectJS.materialMark == 1) {
                _this.html(((_this.prevAll(".tr-quantity").find("input").val() * 1) * _this.prevAll(".tr-price").find("label").text()).toFixed(3));
                amount += _this.html() * 1;
            }
            else if (ObjectJS.materialMark == 2) {
                _this.html(((_this.prevAll(".tr-quantity").find("input").val() * 1 ) * _this.prevAll(".tr-price").find(".price").val()).toFixed(3));
                amount += _this.html() * 1;
            }
        });

        $("#amount").text(amount.toFixed(3));
    }
    //#endregion

    //#region 任务制版相关事件
    //绑定
    ObjectJS.bindPlatemakingEvent = function () {
        ObjectJS.bindDocumentClick();
        ObjectJS.bindDropDown();

        ObjectJS.bindSetMarkValue();
        ObjectJS.bindSetVariation();
        ObjectJS.bindSetVariationColumn();

        ObjectJS.bindAddColumn();
        ObjectJS.bindRemoveColumn();

        ObjectJS.bindAddRow();
        ObjectJS.bindRemoveRow();

        ObjectJS.initAddTaskPlate();

        $("#btn-updateTaskRemark").click(function () {
            ObjectJS.updateOrderPlatemaking();
        });
    };

    //文档点击的隐藏事件
    ObjectJS.bindDocumentClick = function () {
        $(document).bind("click", function (e) {
            //隐藏制版列操作下拉框
            if (!$(e.target).parents().hasClass("ico-dropdown") && !$(e.target).hasClass("ico-dropdown")) {
                $("#setPlateInfo").hide();
                $("#setReturnSewn").hide();
            };
        });
    }

    //显示制版列操作下拉框
    ObjectJS.bindDropDown = function () {
        $(".ico-dropdown").unbind().bind("click", function () {
            var _this = $(this);
            var position = _this.position();
            $("#setPlateInfo li").data("columnname", _this.data("columnname"));
            $("#setPlateInfo").css({ "top": position.top + 20, "left": position.left - 70 }).show().mouseleave(function () {
                $(this).hide();
            });

            return false;
        });

    }

    //添加制版新列
    ObjectJS.bindAddColumn = function () {
        $("#btn-addColumn").unbind().bind("click", function () {
            $("#setTaskPlateAttrBox").remove();
            ObjectJS.columnnameid = $(this).data("columnname");

            var noHaveLi = true;
            var innerHtml = '<ul id="setTaskPlateAttrBox" class="role-items">';
            for (var i = 0; len = CacheAttrValues.length, i < len; i++) {
                var item = CacheAttrValues[i];                
                if ($(".table-list td[data-columnname='columnname_" + item.ValueID + "']").length == 0) {
                    innerHtml += '<li class="role-item" data-id="' + item.ValueID + '" data-sort="' + item.Sort + '">' + item.ValueName + '</li>';
                    noHaveLi = false;
                }
            }
            innerHtml += '</ul>';
            if (noHaveLi) {
                innerHtml = '<div style="width:300px;">制版属性列已全部添加设置了</div>';
            }

            Easydialog.open({
                container: {
                    id: "show-model-setRole",
                    header: "新增制版",
                    content: innerHtml,
                    yesFn: function () {
                        var $hovers = $("#setTaskPlateAttrBox li.hover");
                        if ($hovers.length == 0) return;

                        $hovers.each(function () {
                            var newColumnHeadr = '';
                            var newColumn = '';
                            var columnnameid = $(this).data("id");
                            var sort = $(this).data("sort");
                            var columnnamename = $(this).html();

                            newColumnHeadr += '<td class="width100 tLeft columnHeadr" data-columnname="columnname_' + columnnameid + '" data-id="' + columnnameid + '" data-sort="' + sort + '">';
                            newColumnHeadr += '<span>' + columnnamename + '</span>';
                            newColumnHeadr += '<span class="ico-dropdown mRight10 right" data-columnname="columnname_' + columnnameid + '"></span>';
                            newColumnHeadr += '</td>';

                            newColumn += '<td class="tLeft width100" data-columnname="columnname_' + columnnameid + '">';
                            newColumn += '<span class="tbContent"></span>';
                            newColumn += '<input class="tbContentIpt" value="" type="text"/>';
                            newColumn += '</td>';

                            var before = ObjectJS.getcolumnname(sort);
                            if (before == 0) {
                                $("#platemakingBody td[data-columnname='" + ObjectJS.columnname + "']").eq(0).after(newColumnHeadr);
                                $("#platemakingBody td[data-columnname='" + ObjectJS.columnname + "']:gt(0)").after(newColumn).find("tbContentIpt").show();
                            }
                            else {
                                $("#platemakingBody td[data-columnname='" + ObjectJS.columnname + "']").eq(0).before(newColumnHeadr);
                                $("#platemakingBody td[data-columnname='" + ObjectJS.columnname + "']:gt(0)").before(newColumn).find("tbContentIpt").show();
                            }

                            var marksort = $(".td-normal-plate").data("sort");
                            var $tds = $("#platemakingBody td[data-columnname='columnname_" + columnnameid + "']");
                            for (var i = 1; i < $tds.length; i++) {
                                var _self = $tds.eq(i);
                                var $td = _self.parent().find(".normal-plate");
                                var markvalue = $td.find(".tbContentIpt").val();
                                var variation = _self.parent().find(".normal-plate-ipt").val();
                                if (markvalue != "" && variation != "") {
                                    var value = (markvalue * 1) + ((variation * (sort - marksort)) * 1);
                                    if (!(value + "").isInt()) {
                                        value = value.toFixed(2);
                                    }
                                    _self.find(".tbContentIpt").val(value);
                                }

                            }
                        });

                        $(".tbContentIpt").each(function () {
                            $(this).html($(this).prev().html()).show().prev().hide();
                        });
                        $("#btn-updateTaskRemark").html("保存制版");
                        ObjectJS.bindSetMarkValue();
                        ObjectJS.bindDropDown();
                        ObjectJS.bindAddRow();
                        ObjectJS.bindRemoveRow();
                    }
                }

            });

            if (!noHaveLi) {
                $("#setTaskPlateAttrBox .role-item").click(function () {
                    if (!$(this).hasClass("hover"))
                        $(this).addClass("hover");
                    else
                        $(this).removeClass("hover");
                });
            }

        });
    }

    //删除制版列
    ObjectJS.bindRemoveColumn = function () {
        $("#btn-removeColumn").unbind().bind("click", function () {
            var $column = $("#platemakingBody .table-list td[data-columnname='" + $(this).data("columnname") + "']");
            var $td = $column.eq(0);
            if ($td.hasClass("td-normal-plate")) {
                alert("当前为标码列,不能删除");
                return;
            }
            if ($("#platemakingBody .tr-header td").length == 3) {
                alert("只剩最后一列,不能删除");
                return;
            }

            $column.remove();
            $(".tbContentIpt").each(function () {
                $(this).html($(this).prev().html()).show().prev().hide();
            });
            $("#btn-updateTaskRemark").html("保存制版");
        });
    }

    //获取插入列的列名
    ObjectJS.getcolumnname = function (slefsort) {
        var before = 0;
        $(".tr-header .columnHeadr").each(function () {
            ObjectJS.columnname = $(this).data("columnname");
            var sort = $(this).data("sort");
            if (sort) {
                if (slefsort < sort) {
                    before = 1;
                    return false;
                }
            }
        });

        return before;
    };

    //添加制版行
    ObjectJS.bindAddRow = function () {
        $("div.btn-addRow").unbind().bind('click', function () {
            var $newTR = $("<tr class='tr-content'>" + $(this).parent().parent().parent().html() + "</tr>");
            $newTR.find(".tbContentIpt").empty().attr("value", "").show();
            $newTR.find(".tbContent").empty();
            $(this).parent().parent().parent().after($newTR);

            $("#btn-updateTaskRemark").html("保存制版");
            ObjectJS.bindAddRow();
            ObjectJS.bindRemoveRow();
            ObjectJS.bindSetMarkValue();
            ObjectJS.bindSetVariation();
        });
    }

    //删除制版行
    ObjectJS.bindRemoveRow = function () {
        $("div.btn-removeRow").unbind().bind('click', function () {
            if ($("div.btn-removeRow").length == 1) {
                alert("只剩最后一行,不能删除");
                return;
            }

            $(this).parent().parent().parent().remove();
            $("#btn-updateTaskRemark").html("保存制版");
        });
    }

    //设置标码
    ObjectJS.bindSetMarkValue = function () {
        $(".normal-plate .tbContentIpt").unbind().bind("blur", function () {
            var markvalue = $(this).val();
            if (markvalue== '' || !markvalue.isDouble()) {
                return;
            }
            
            var variation =$(this).parent().parent().find(".normal-plate-ipt").val();
            if (variation=='' || !variation.isDouble()) {
                return;
            }

            var marksort = $(".td-normal-plate").data("sort");
            var $tbContentIpts = $(this).parent().parent().find(".tbContentIpt");
            var $columnHeadrs = $(".tr-header .columnHeadr");
            for (var i = 0; i < $columnHeadrs.length; i++) {
                var sort = $columnHeadrs.eq(i).data("sort");
                var value = (markvalue * 1) + ((variation * (sort - marksort)) * 1);
                if (!(value + "").isInt()) {
                    value = value.toFixed(2);
                }
                $tbContentIpts.eq(i + 1).val(value);
            }
        });
    };

    //设置档差
    ObjectJS.bindSetVariation = function () {
        $(".normal-plate-ipt").unbind().bind("blur", function () {
            var variation = $(this).val();
            if (variation == '' || !variation.isDouble()) {
                $(this).val('');
                return;
            }

            var $td = $(this).parent().parent().find(".normal-plate");
            var markvalue = $td.find(".tbContentIpt").val();
            if (markvalue == "" || !markvalue.isDouble() ) {
                alert("标码值有误");
                $(this).val('');
                return;
            }

            var marksort = $(".td-normal-plate").data("sort");
            var $tbContentIpts = $td.parent().find(".tbContentIpt");
            var $columnHeadrs=$(".tr-header .columnHeadr");
            for (var i = 0; i < $columnHeadrs.length; i++) {
                var sort = $columnHeadrs.eq(i).data("sort");
                var value = (markvalue*1) + ((variation * (sort - marksort))*1);
                if (!(value + "").isInt()) {
                    value = value.toFixed(2);
                }
                $tbContentIpts.eq(i + 1).val(value);
            }

        });
    }

    //设置标码列
    ObjectJS.bindSetVariationColumn = function () {
        $("#btn-setNormalColumn").unbind().bind("click", function () {
            var  $tdnormalplate=$(".td-normal-plate").removeClass("td-normal-plate").find("span:first");
            $tdnormalplate.html($tdnormalplate.html().replace("-标码", "").replace("-档差标准", "") );
            $(".normal-plate").removeClass("normal-plate");

            var columnname = $(this).data("columnname");
            var $tdnormalplatenow = $("#platemakingBody td[data-columnname='" + columnname + "']").eq(0).addClass("td-normal-plate");
            $tdnormalplatenow = $tdnormalplatenow.find("span:first");
            $tdnormalplatenow.html($tdnormalplatenow.html() + "-标码");
            $("#platemakingBody td[data-columnname='" + columnname + "']:gt(0)").addClass("normal-plate");
        });
    }

    //清除 制版操作
    ObjectJS.removeTaskPlateOperate = function () {
        $("span.ico-dropdown").remove();
        $("#platemakingBody table tr").each(function () {
            $(this).find("td:last").remove();
        });
    }

    //初始化制版列
    ObjectJS.initAddTaskPlate = function () {
        if ($("#btn-initAddTaskPlate").length == 0) return;

        $("#btn-initAddTaskPlate").unbind().bind("click", function () {
            var noHaveLi = false;
            if (CacheAttrValues.length > 0) {
                var innerHtml = '<ul id="setTaskPlateAttrBox" class="role-items">';
                for (var i = 0; len = CacheAttrValues.length, i < len; i++) {
                    var item = CacheAttrValues[i];
                    innerHtml += '<li class="role-item" data-id="' + item.ValueID + '" data-sort="' + item.Sort + '">' + item.ValueName + '</li>';
                }
                innerHtml += '</ul>';
            } else {
                noHaveLi = true;
                innerHtml = '<div style="width:300px;">制版属性没有配置,请联系后台管理员配置</div>';
            }

            Easydialog.open({
                container: {
                    id: "show-model-initAddTaskPlate",
                    header: "新增制版",
                    content: innerHtml,
                    yesFn: function () {
                        var $hovers = $("#setTaskPlateAttrBox li.hover");
                        if ($hovers.length == 0) return;

                        var tableHtml = '<table class="table-list">';
                        var newColumnHeadr = '<tr class="tr-header">';
                        newColumnHeadr += '<td class="width100 tLeft">部位/尺码</td>';

                        var newColumn = '<tr class="tr-content">';
                        newColumn += '<td class="tLeft width100">';
                        newColumn += '<span class="tbContent"></span>';
                        newColumn += '<input class="tbContentIpt" value="" type="text"/>';
                        newColumn += '</td>';

                        var i = 0;
                        $hovers.each(function () {
                            var columnnameid = $(this).data("id");
                            var sort = $(this).data("sort");
                            var columnnamename = $(this).html();
                            if (i == 0) {
                                newColumnHeadr += '<td class="width100 tLeft columnHeadr td-normal-plate" data-columnname="columnname_' + columnnameid + '" data-id="' + columnnameid + '"  data-sort="' + sort + '">';
                                newColumnHeadr += '<span>' + columnnamename + "-标码" + '</span>';
                            } else {
                                newColumnHeadr += '<td class="width100 tLeft columnHeadr" data-columnname="columnname_' + columnnameid + '" data-id="' + columnnameid + '" data-sort="' + sort + '">';
                                newColumnHeadr += '<span>' + columnnamename + '</span>';
                            }
                            newColumnHeadr += '<span class="ico-dropdown mRight10 right" data-columnname="columnname_' + columnnameid + '"></span>';
                            newColumnHeadr += '</td>';

                            if (i == 0) {
                                newColumn += '<td class="tLeft width100 normal-plate" data-columnname="columnname_' + columnnameid + '">';
                            } else {
                                newColumn += '<td class="tLeft width100" data-columnname="columnname_' + columnnameid + '">';
                            }
                            newColumn += '<span class="tbContent"></span>';
                            newColumn += '<input class="tbContentIpt" value="" type="text"/>';
                            newColumn += '</td>';
                            i++;
                        });

                        newColumnHeadr += '<td class="width150 tLeft">档差</td>';
                        newColumnHeadr += '<td class="width150 center">操作</td>';
                        newColumnHeadr += '</tr>';

                        newColumn += '<td class="tLeft width100" >';
                        newColumn += '<span class="tbContent"></span>';
                        newColumn += '<input class="tbContentIpt normal-plate-ipt" value="" type="text"/>';
                        newColumn += '</td>';
                        newColumn += '<td class="width150 center">';
                        newColumn += '    <div class="platemakingOperate">';
                        newColumn += '        <div class="btn-addRow btn-create left" title="添加新行"></div>';
                        newColumn += '        <div class="btn-removeRow btn-remove mLeft10 left" title="删除此行"></div>';
                        newColumn += '        <div class="clear"></div>';
                        newColumn += '   </div>';
                        newColumn += '</td>';
                        newColumn += '</tr>';

                        tableHtml += newColumnHeadr + newColumn+'</table>';

                        $("#platemakingBody").html(tableHtml).css({ "border-top": "1px solid #eee", "border-left": "1px solid #eee" }).show();
                        $("#btn-initAddTaskPlate").hide();
                        $("#btn-updateTaskRemark").show().html("保存制版");
                        $("#btn-addColumn").show();

                        ObjectJS.bindDropDown();
                        ObjectJS.bindAddRow();
                        ObjectJS.bindRemoveRow();
                        ObjectJS.bindSetMarkValue();
                        ObjectJS.bindSetVariation();
                    }
                }
            });

            if (!noHaveLi) {
                $("#setTaskPlateAttrBox .role-item").click(function () {
                    if (!$(this).hasClass("hover"))
                        $(this).addClass("hover");
                    else
                        $(this).removeClass("hover");
                });
            }

        });
    }

    //保存制版信息
    ObjectJS.updateOrderPlatemaking = function () {
        if ($("#platemakingBody").html() == "") { return; }

        if ($("#btn-updateTaskRemark").html() == "编辑制版") {
            $("#btn-updateTaskRemark").html("保存制版");
            $(".tbContentIpt").each(function () {
                $(this).html( $(this).prev().html() ).show().prev().hide();
            });
            return;
        } else {
            $(".tbContentIpt:visible").each(function () {
                $(this).attr("value", $(this).val()).hide().prev().html($(this).val()).show();
            });

            $("#btn-updateTaskRemark").html("编辑制版");
        }

        Global.post("/Task/UpdateOrderPlatehtml", {
            orderID: ObjectJS.orderid,
            taskID: ObjectJS.taskid,
            platehtml: encodeURI($("#platemakingBody").html())
        }, function (data) {
            if (data.result == 1) {
                ObjectJS.isPlate = true;
            }
            else {
                alert("保存失败");
            }
        });
    }

    //制版工艺说明
    ObjectJS.initPlateMaking = function () {
        if ($("#addPlateType").length > 0) {
            $("#addPlateType").click(function () {
                ObjectJS.choosePlateTypeAdd();
            });
        }   

        if ($("#btnAddPalte").length > 0) {
            $("#btnAddPalte").click(function () {
                if (btnAddPalte) {
                    ObjectJS.addPlateMaking();
                }
            });
        }
        $("#setObjectPlate").click(function () {
            var index = $(this).data("index");
            var item = PlateMakings[index];

            var _htmlTr = $(".tb-plates .dropdown[data-index='" + index + "']").parents('tr');
            DoT.exec("template/task/platemarting-quickly-add.html", function (templateFun) {
                var innerHtml = templateFun();
                innerHtml = $(innerHtml);
                innerHtml.find('.txt-name').val(item.Title);
                innerHtml.find('.desc').val(item.Remark);
                innerHtml.find('.plate-ico-img').data('src', item.Icon);
                innerHtml.find('.plate-ico-img').attr('src', item.Icon);
                innerHtml.find('.txt-name').val(item.Title);

                innerHtml.find('.save-plate').click(function () {
                    var _thisTr = $(this).parents('tr');
                    if (!_thisTr.find('.txt-name').val().trim()) {
                        alert("工艺名称不能为空");
                        return false;
                    }
                    var Plate = {
                        PlateID: item.PlateID,
                        Title: _thisTr.find('.txt-name').val() || '',
                        Remark: _thisTr.find('.desc').val() || '',
                        Icon: _thisTr.find('.plate-ico-img').data('src') || '',
                        OrderID: ObjectJS.orderType == 1 ? ObjectJS.orderid : ObjectJS.originalID,
                        TaskID: ObjectJS.taskid,
                        TypeName: item.TypeName
                    };
                    Global.post("/Task/SavePlateMaking", { plate: JSON.stringify(Plate) }, function (data) {
                        if (data.result == 0) {
                            alert("保存失败，请重试");
                        } else {
                            /*修改制版处理缓存*/
                            item.Icon = Plate.Icon;
                            item.Remark = Plate.Remark;
                            item.Title = Plate.Title;

                            var plateHtml = '<td class="tLeft item bBottom"><img style="width:30px;height:30px;text-indent:0;" src="' + (_thisTr.find('.plate-ico-img').data('src') || '') + '" /></td>';
                            plateHtml += '<td class="tLeft item bBottom">' + (_thisTr.find('.txt-name').val() || '') + '</td>';
                            plateHtml += '<td class="tLeft item show-all-txt bBottom" style="line-height:150%;">' + _thisTr.find('.desc').val() || '' + '</td>';
                            plateHtml += '<td class="center item width150 bBottom">' + (item.CreateTime ? item.CreateTime.toDate('yyyy-MM-dd hh:mm:ss') : new Date().toString('yyyy-MM-dd hh:mm:ss')) + '</td>';
                            plateHtml += '<td class="center item dropdown width150 bBottom" data-id="' + data.id + '" data-index="' + index + '" data-title="' + (_thisTr.find('.desc').val() || '') + '"><span class="ico-dropdown"></span></td>';
                            plateHtml = $(plateHtml);
                            _thisTr.attr('id', 'tr-plate-' + data.id);
                            _thisTr.html(plateHtml);
                            _thisTr.find(".dropdown").click(function () {
                                var _this = $(this);
                                var position = _this.find(".ico-dropdown").position();
                                $("#setPlateMaking li").data("id", _this.data("id")).data("index", _this.data("index")).data("title", _this.data("title"));
                                $("#setPlateMaking").css({ "top": position.top + 20, "left": position.left - 40 }).show().mouseleave(function () {
                                    $(this).hide();
                                });
                            });
                        }
                    });
                });

                innerHtml.find('.cencal-plate').click(function () {
                    var _thisTr = $(this).parents('tr');
                    var plateHtml = '<td class="tLeft item bBottom"><img style="width:30px;height:30px;text-indent:0;" src="' + item.Icon + '" /></td>';
                    plateHtml += '<td class="tLeft item bBottom">' + item.Title + '</td>';
                    plateHtml += '<td class="tLeft item show-all-txt bBottom" style="line-height:150%;">' + item.Remark + '</td>';
                    plateHtml += '<td class="center item width150 bBottom">' + item.CreateTime.toDate('yyyy-MM-dd hh:mm:ss') + '</td>';
                    plateHtml += '<td class="center item dropdown width150 bBottom" data-id="' + item.PlateID + '" data-index="' + index + '" data-title="' + item.Title + '"><span class="ico-dropdown"></span></td>';
                    plateHtml = $(plateHtml);
                    _thisTr.attr('id', 'tr-plate-' + item.PlateID);
                    _thisTr.html(plateHtml);
                    _thisTr.find(".dropdown").click(function () {
                        var _this = $(this);
                        var position = _this.find(".ico-dropdown").position();
                        $("#setPlateMaking li").data("id", _this.data("id")).data("index", _this.data("index")).data("title", _this.data("title"));
                        $("#setPlateMaking").css({ "top": position.top + 20, "left": position.left - 40 }).show().mouseleave(function () {
                            $(this).hide();
                        });
                    });

                    $(this).parents('tr').html(_htmlTr);
                });

                _htmlTr.html(innerHtml);

                if (Upload == null) {
                    Upload = require("upload");
                }
                //工艺说明录入上传附件
                var uploader = Upload.uploader({
                    browse_button: innerHtml.find('.add-img').attr('id'),
                    file_path: "/Content/UploadFiles/Product/",
                    multi_selection: false,
                    auto_callback: false,
                    fileType: 1,
                    init: {
                        "FileUploaded": function (up, file, info) {
                            var info = JSON.parse(info);
                            innerHtml.find('.plate-ico-img').attr("src", file.server + info.key);
                            innerHtml.find('.plate-ico-img').data("src", file.server + info.key);
                        }
                    }
                });
            });
        });

        $("#deleteObject").click(function () {
            var plateID = $(this).data("id");
            var title = $(this).data("title");
            ObjectJS.deletePlateMaking(plateID, title);
        });
    }

    //获取制版工艺说明
    ObjectJS.getPlateMakings = function () {
        
        $(".tb-plates").html('');
        $(".tb-plates").html("<tr><td colspan='5'><div class='data-loading'><div></td></tr>");
        Global.post("/Task/GetPlateMakings", {
            orderID: ObjectJS.orderType == 1 ? ObjectJS.orderid : ObjectJS.originalID
        }, function (data) {
            $(".tb-plates").html('');
            if (data.items.length > 0) {
                DoT.exec("template/task/platemarting-list.html", function (template) {
                    PlateMakings = data.items;
                    var html = template(data.items);
                    html = $(html);
                    if ($("#addPlateType").length == 0) {
                        html.find('.add-plate').remove();
                    }
                    $(".tb-plates").append(html);
                    $(".typetitle td").css({"background-color":"#eee","color":"#333","line-height":"30px"});
                    $(".typetitle:first td").css("line-height", "40px");
                    html.find(".add-plate").click(function () {
                        var _this = $(this);
                        ObjectJS.qulicklyAddPlateMarkings(_this);
                    });

                    if ($("#addPlateType").length == 1) {
                        html.find(".dropdown").click(function () {
                            var _this = $(this);
                            var position = _this.find(".ico-dropdown").position();
                            $("#setPlateMaking li").data("id", _this.data("id")).data("index", _this.data("index")).data("title", _this.data("title"));

                            $("#setPlateMaking").css({ "top": position.top + 20, "left": position.left-40 }).show().mouseleave(function () {
                                $(this).hide();
                            });
                        });
                    }
                    else {
                        html.find(".ico-dropdown").remove();
                    }
                });
            }
            else {
                if ($("#addPlateType").length > 0) {
                    var addPlateMartingHtml = $('<tr><td><div id="showPlateType" class="create-first"><span class="plus">+</span>添加工艺</div></td></tr>');
                    addPlateMartingHtml.bind("click", function () {
                        ObjectJS.choosePlateTypeAdd();
                    });
                    $(".tb-plates").html(addPlateMartingHtml);
                } else {
                    $(".tb-plates").html('<tr><td><div class="nodata-txt">暂无数据</div></td></tr>');
                }
            }
        });
    }

    //获取下单明细
    ObjectJS.getOrderGoods = function () {
        $("#navGoods .table-header").nextAll().remove();
        $("#navGoods .table-header").after($("<tr><td colspan='8'><div class='data-loading'></div></td></tr>"));
        Global.post("/Task/GetOrderGoods", { id: ObjectJS.orderid }, function (data) {
            $("#navGoods .table-header").nextAll().remove();
            if (data.list.length > 0) {
                DoT.exec("template/task/task-ordergoods.html", function (template) {
                    var innerHtml = template(data.list);
                    innerHtml = $(innerHtml);
                    $("#navGoods .table-header").after(innerHtml);
                    ObjectJS.getAmount();
                });
            } else {
                $("#navGoods .table-header").after($("<tr><td colspan='8'><div class='nodata-txt'>暂无明细</div></td></tr>"));
            }
        });
    };

    //获取订单所有任务流程
    ObjectJS.getAllTaskStages = function () {
        $(".process-stages .stage-items").append($("<div class='data-loading'></div>"));
        Global.post("/Task/GetOrderStages", { orderid: ObjectJS.orderid }, function (data) {
            if (data.items.length > 0) {
                DoT.exec("template/task/task-stage.html", function (template) {
                    data.items.stageID = ObjectJS.stageid;
                    var innerHtml = template(data.items);
                    innerHtml = $(innerHtml);
                    $(".process-stages .stage-items").html(innerHtml);
                });
            } else {
                $(".process-stages").remove();
            }
        });
    };

    //快捷添加一条制版工艺说明
    ObjectJS.qulicklyAddPlateMarkings = function (_this) {
        if (_this.parents().prev().find('.save-plate').length > 0) {
            alert("请先保存上一次编辑信息");
            return false;
        }
        DoT.exec("template/task/platemarting-quickly-add.html", function (templateFun) {
            var innerHtml = templateFun();
            innerHtml = $(innerHtml);
            innerHtml = $("<tr class='list-item'></tr>").append(innerHtml);
            innerHtml.find('.save-plate').click(function () {
                var _thisTr = $(this).parents('tr');
                if (!_thisTr.find('.txt-name').val().trim()) {
                    alert("工艺名称不能为空");
                    return false;
                }
                var Plate = {
                    PlateID: '',
                    Title: _thisTr.find('.txt-name').val() || '',
                    Remark: _thisTr.find('.desc').val() || '',
                    Icon: _thisTr.find('.plate-ico-img').data('src') || '',
                    OrderID: ObjectJS.orderType == 1 ? ObjectJS.orderid : ObjectJS.originalID,
                    TaskID: ObjectJS.taskid,
                    TypeName: _thisTr.next().find('.add-plate').data('typename') || ''
                };

                Global.post("/Task/SavePlateMaking", { plate: JSON.stringify(Plate) }, function (data) {
                    if (data.result == 0) {
                        alert("保存失败，请重试");
                    } else {
                        /*缓存制版信息*/
                        var cachePlate = {
                            CreateTime: '/Date(' + new Date().getTime() + ')/',
                            PlateID: data.id,
                            Title: (_thisTr.find('.txt-name').val() || ''),
                            Remark: _thisTr.find('.desc').val() || '',
                            Icon: (_thisTr.find('.plate-ico-img').data('src') || ''),
                            OrderID: ObjectJS.orderType == 1 ? ObjectJS.orderid : ObjectJS.originalID,
                            TaskID: ObjectJS.taskid,
                            TypeName: _thisTr.next().find('.add-plate').data('typename') || ''
                        };
                        var index = 0;
                        $(".tb-plates tr .dropdown").each(function () {
                            if ($(this).data('index') > index) {
                                index = $(this).data('index');
                            }
                        });
                        index += 1;
                        PlateMakings[index] = cachePlate;
                        var plateHtml = '<td class="tLeft item bBottom"><img style="width:30px;height:30px;text-indent:0;" src="' + (_thisTr.find('.plate-ico-img').data('src') || '') + '" /></td>';
                        plateHtml += '<td class="tLeft item bBottom">' + (_thisTr.find('.txt-name').val() || '') + '</td>';
                        plateHtml += '<td class="tLeft item show-all-txt bBottom" style="line-height:150%;">' + _thisTr.find('.desc').val() || '' + '</td>';
                        plateHtml += '<td class="center item width150 bBottom">' + cachePlate.CreateTime.toDate('yyyy-MM-dd hh:mm:ss') + '</td>';
                        plateHtml += '<td class="center item dropdown width150 bBottom" data-id="' + data.id + '" data-index="' + index + '" data-title="' + (_thisTr.find('.desc').val() || '') + '"><span class="ico-dropdown"></span></td>';
                        plateHtml = $(plateHtml);
                        _thisTr.attr('id', 'tr-plate-' + data.id);
                        _thisTr.html(plateHtml);
                        _thisTr.find(".dropdown").click(function () {
                            var _this = $(this);
                            var position = _this.find(".ico-dropdown").position();
                            $("#setPlateMaking li").data("id", _this.data("id")).data("index", _this.data("index")).data("title", _this.data("title"));
                            $("#setPlateMaking").css({ "top": position.top + 20, "left": position.left - 40 }).show().mouseleave(function () {
                                $(this).hide();
                            });
                        });
                    }
                });
            });

            innerHtml.find('.cencal-plate').click(function () {
                $(this).parents('tr').remove();
            });

            _this.parents('tr').before(innerHtml);

            if (Upload == null) {
                Upload = require("upload");
            }
            //工艺说明录入上传附件
            var uploader = Upload.uploader({
                browse_button: innerHtml.find('.add-img').attr('id'),
                file_path: "/Content/UploadFiles/Product/",
                multi_selection: false,
                auto_callback: false,
                fileType: 1,
                init: {
                    "FileUploaded": function (up, file, info) {
                        var info = JSON.parse(info);
                        innerHtml.find('.plate-ico-img').attr("src", file.server + info.key);
                        innerHtml.find('.plate-ico-img').data("src", file.server + info.key);
                    }
                }
            });
        });
    };

    //选择制版工艺类型并添加
    ObjectJS.choosePlateTypeAdd = function () {
        var innerHtml = '<ul id="setTaskPlateMarting" class="role-items">';
        for (var i = 0; len = plateMartingItem.length, i < len; i++) {
            var item = plateMartingItem[i];
            innerHtml += '<li class="role-item" data-text="' + item.Name + '">' + item.Name + '</li>';
        }
        innerHtml += '<li style="border:none !important;cursor:auto;"><input id="layoutPlateType" max-length=20 type="text" placeholder="自定义" style="width:60px;text-align:center;" /></li>';
        innerHtml += '</ul>';

        Easydialog.open({
            container: {
                id: "initAddPlateMarting",
                header: "选择工艺类型",
                content: innerHtml,
                yesFn: function () {
                   
                    var existsText="";
                    var items = [];
                    $("#setTaskPlateMarting li.hover").each(function () {
                        var _this = $(this);
                        var isContinue = true;
                        
                        $(".tb-plates .table-header").each(function () {
                            if (_this.text().trim() == $(this).find('.plate-name').data('name')) {
                                isContinue = false;
                                existsText=_this.text().trim();
                                return false;
                            }
                        });
                        if (isContinue) {
                            items.push({ type: _this.data('type'), text: _this.data('text') });
                        }
                    });

                    
                    if (items.length > 0) {
                        var innerAddHtml = "";
                        for (var i = 0; i < items.length; i++) {
                            var item = items[i];
                            innerAddHtml += '<tr class="table-header tr-header"><td class="font14 tLeft width150 bold plate-name"  data-name="' + item.text + '">工艺类型：' + item.text + '</td><td class="tLeft width150 bold">名称</td> <td class="tLeft bold">描述</td><td class="width150 bold">创建时间</td><td class="center width150 bold">操作</td></tr>';
                            innerAddHtml += '<tr class="list-item"><td colspan="5" class="center" style="padding:0 0 5px 0;"><div class="add-plate font16 hand hBlue" style="text-indent:0;line-height:50px;" data-typename="' + item.text + '">+添加' + item.text + '</div></td></tr>';
                        }
                        innerAddHtml = $(innerAddHtml);
                        innerAddHtml.find('.add-plate').click(function () {
                            ObjectJS.qulicklyAddPlateMarkings($(this));
                        });
                        var addPlateType = $('<div class="right btn-add mRight20" id="addPlateType">选择工艺类型</div>');
                        if ($("#addPlateType").length == 0) {
                            addPlateType.click(function () {
                                ObjectJS.choosePlateTypeAdd();
                            });
                            $("#btnAddPalte").after(addPlateType);
                        }

                        $("#showPlateType").parents('tr').remove();
                        $(".tb-plates").append(innerAddHtml);
                    }
                }
            }
        });

        $("#setTaskPlateMarting #layoutPlateType").change(function () {
            var _this = $(this);
            var isContinue = true;
            if (!_this.val().trim()) {
                alert("制版类型不能为空");
                _this.val('');
                return false;
            }

            $("#setTaskPlateMarting .role-item").each(function () {
                if (_this.val().trim() == $(this).text().trim()) {
                    isContinue = false;
                    return false;
                }
            });

            $(".tb-plates .table-header").each(function () {
                if (_this.val().trim() == $(this).find('.plate-name').data('name')) {
                    isContinue = false;
                    return false;
                }
            });
            if (!isContinue) {
                alert("制版类型已存在");
                return false;
            }
            var roleItem = $('<li class="role-item hover" data-type="10" data-text="' + _this.val().trim() + '">' + _this.val().trim() + '</li>');
            roleItem.click(function () {
                if (!$(this).hasClass("hover"))
                    $(this).addClass("hover");
                else
                    $(this).removeClass("hover");
            });
            _this.parent().before(roleItem);
            _this.val('');
        });

        $("#setTaskPlateMarting .role-item").click(function () {
            if (!$(this).hasClass("hover"))
                $(this).addClass("hover");
            else
                $(this).removeClass("hover");
        });
    };

    //新增工艺说明
    ObjectJS.addPlateMaking = function () {
        var item = {
            PlateID: "",
            Title: "",
            Remark: "",
            Icon: "",
            OrderID: "",
            TaskID: "",
            TypeName: plateMartingItem[0].Name
        }
        ObjectJS.savePlateMaking(item);
    }

    //保存工艺说明
    ObjectJS.savePlateMaking = function (item) {
        item.plateType = plateMartingItem;
        DoT.exec("template/task/platemarting-add.html", function (template) {
            var html = template([item]);
            Easydialog.open({
                container: {
                    id: "show-model-setPlate",
                    header: "工艺说明录入",
                    content: html,
                    yesFn: function () {
                        if ($("#plateTitle").val() == '') {
                            alert("工艺不能为空");
                            return false;
                        }
                        if ($("#plateRemark").val().length > 200) {
                            alert("描述不能超过200字");
                            return false;
                        }
                        var Plate = {
                            PlateID: item.PlateID,
                            Title: $("#plateTitle").val(),
                            Remark: $("#plateRemark").val(),
                            Icon: $("#plateIcon").val(),
                            OrderID: ObjectJS.orderType == 1 ? ObjectJS.orderid : ObjectJS.originalID,
                            TaskID: ObjectJS.taskid,
                            TypeName: $("#selectType").val()
                        };
                    
                        Global.post("/Task/SavePlateMaking", { plate: JSON.stringify(Plate) }, function (data) {
                            if (data.result == 0) {
                                alert("保存失败");
                            }
                            else {
                                ObjectJS.getPlateMakings();
                            }
                        });
                    }
                }
            });

            $("#selectType").val(item.TypeName);

            var icoUrl = item.Icon;
            if (icoUrl != '') {
                $(".plate-show-ico").show().find("img").attr("src", icoUrl);
            }


            if (Upload == null) {
                Upload = require("upload");
            }

            //工艺说明录入上传附件
            Upload.uploader({
                browse_button: 'selectPlateIcon',
                container: 'plateBox',
                drop_element: 'plateBox',
                file_path: "/Content/UploadFiles/Product/",
                picture_container: "plateBox",
                maxSize: 5,
                multi_selection: false,
                auto_callback: false,
                fileType: 1,
                init: {
                    "FileUploaded": function (up, file, info) {
                        var info = JSON.parse(info);
                        var src = file.server + info.key;
                        $(".plate-show-ico").show().find("img").attr("src", src);
                        $("#plateIcon").val(src);
                    }
                }
            });

        });
    }

    //删除工艺说明
    ObjectJS.deletePlateMaking = function (plateID,title) {
        confirm("确认删除工艺说明？", function () {
            Global.post("/Task/DeletePlateMaking", {
                plateID: plateID,
                taskID: ObjectJS.taskid,
                title: title
            }, function (data) {
                if (data.result == 0) {
                    alert("删除失败");
                }
                else {
                    $("#tr-plate-" + plateID).fadeOut(500);
                }
            });
        });
    }

    module.exports = ObjectJS;
});