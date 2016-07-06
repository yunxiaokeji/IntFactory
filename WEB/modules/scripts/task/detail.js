define(function (require, exports, module) {
    var DoT = require("dot");
    var Global = require("global");
    var Upload = require("upload");
    var Easydialog = require("easydialog");
    var Pager = require("pager");
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

    ///taskid：任务id
    ///orderid:订单id
    ///stageid：订单阶段id
    ///mark:任务标记 11：材料 12 制版 21大货材料
    ///finishStatus：任务完成状态
    ///attrValues:订单品类属性
    ///orderType:订单类型
    ObjectJS.init = function (attrValues, orderimages, isWarn, task, originalID,orderPlanTime) {
        var task = JSON.parse(task.replace(/&quot;/g, '"'));
        if (attrValues != "")
            CacheAttrValues = JSON.parse(attrValues.replace(/&quot;/g, '"'));//制版属性缓存
        ObjectJS.orderid = task.OrderID;
        ObjectJS.guid = task.OrderID;
        ObjectJS.originalID = originalID;
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
        ObjectJS.isLoading = true;
        //材料任务
        if ($("#btn-addMaterial").length == 1) {
            ObjectJS.materialMark = 1;
            if (ObjectJS.mark == 21) {
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

        //判断制版任务是否已执行
        if (ObjectJS.mark == 12 || ObjectJS.mark == 22) {
            if ($("#platemakingBody .table-list").length == 0) {
                ObjectJS.isPlate = false;
            }
        }
        if (ObjectJS.mark === 23) {
            CutoutDoc = require("scripts/task/cutoutdoc");
            CutoutDoc.initCutoutDoc(ObjectJS.orderid,ObjectJS.taskid,Global,DoT,Easydialog);
        }
        else if (ObjectJS.mark === 24) {
            SewnDoc = require("scripts/task/sewndoc");
            SewnDoc.initSewnDoc(ObjectJS.orderid, ObjectJS.taskid, Global, DoT, Easydialog);
        }
        else if (ObjectJS.mark === 25) {
            SendOrders = require("scripts/task/sendorders");
            SendOrders.initSendOrders(ObjectJS.orderid, ObjectJS.taskid, Global, DoT, Easydialog);
        }
        else if (ObjectJS.mark === 15) {
            SendDYOrders = require("scripts/task/senddyorders");
            SendDYOrders.initSendDYOrders(ObjectJS.orderid, ObjectJS.taskid, Global, DoT, Easydialog);
        }
        else if (ObjectJS.mark === 16) {
            ProcessDYCosts = require("scripts/task/processcostdoc");
            ProcessDYCosts.initProcessCosts(ObjectJS.orderid, Global, DoT, ObjectJS.orderType);
        }

        TalkReply = require("scripts/task/reply");
        TalkReply.initTalkReply(ObjectJS, 'task',1);

        $(".part-btn").hide();

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
            $(".part-btn").hide();
            if (_this.data("btn")) {
                $("#" + _this.data("btn")).show();
            }

            if (_this.data("id") == "orderTaskLogs") {
                if (!_this.data("isget")) {                    
                    ObjectJS.getLogs(1);
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
                minDate = minDate + " 00:00:00"
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
                alert("材料没有添加,不能标记任务完成");
                return;
            }
        }
        else if (mark == 12) {
            if ($("#platemakingBody .table-list").length == 0) {
                alert("制版没有设置,不能标记任务完成");
                return;
            }
            else if (!ObjectJS.isPlate) {
                alert("制版没有设置,不能标记任务完成");
                return;
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
                    $("#orderImage").attr("src", images[i]);
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
                $("#orderImage").attr("src", _this.find("img").attr("src"));
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

                $(".enlarge-image-item").append('<img id="enlargeImage" src="' + $(this).attr("src") + '"/>');
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
                        $("#orderImage").attr("src", _img.attr("src"));
                        $(".enlarge-image-item").empty();
                        $(".enlarge-image-item").append('<img id="enlargeImage" src="' + _img.attr("src") + '"/>');
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
                        $("#orderImage").attr("src", _img.attr("src"));
                        $(".enlarge-image-item").empty();
                        $(".enlarge-image-item").append('<img id="enlargeImage" src="' + _img.attr("src") + '"/>');
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

    //获取任务日志
    ObjectJS.getLogs = function (page) {
        var _self = this;
        $("#taskLogList").empty();
        ObjectJS.isLoading=false
        Global.post("/Task/GetOrderTaskLogs", {
            id: _self.taskid,
            pageindex: page
        }, function (data) {

            if (data.items.length > 0) {
                DoT.exec("template/common/logs.html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);
                    $("#taskLogList").html(innerhtml);
                });
            }
            else {
                $("#taskLogList").html("<div class='nodata-txt'>暂无日志!</div>");
            }

            $("#pagerLogs").paginate({
                total_count: data.totalCount,
                count: data.pageCount,
                start: page,
                display: 5,
                border: true,
                rotate: true,
                images: false,
                mouse: 'slide',
                float: "left",
                onChange: function (page) {
                    _self.getLogs(page);
                }

            });
            ObjectJS.isLoading = true;
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
        $("#navProducts .loss-rate").change(function () {
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
            confirm("确认从清单中移除此材料吗？", function () {
                Global.post("/Orders/DeleteProduct", {
                    orderid: ObjectJS.orderid,
                    autoid: _this.data("id"),
                    name: _this.data("name")
                }, function (data) {
                    if (!data.status) {
                        alert("系统异常，请重新操作！");
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

    //更改损耗率
    ObjectJS.editLossRate = function (ele) {
        var _self = this;
        ObjectJS.isLoading = false;

        ele.data('value', ele.val());
        var loss = ((ele.val() * 1) * (ele.parents('tr').find('.tr-quantity').html() * 1)).toFixed(3);
        var amount = (loss*1) + (ele.parents('tr').find('.tr-quantity').html() * 1);
        ele.parents('tr').find('.tr-loss').html(loss);
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
                _this.html(((_this.prevAll(".tr-quantity").html() * 1 + _this.prevAll(".tr-loss").html() * 1) * _this.prevAll(".tr-price").find(".price").val()).toFixed(3));
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

        ObjectJS.bindContentClick();

        ObjectJS.bindAddColumn();
        ObjectJS.bindRemoveColumn();

        ObjectJS.bindAddRow();
        ObjectJS.bindRemoveRow();

        $("#btn-updateTaskRemark").click(function () {
            ObjectJS.updateOrderPlatemaking();
        });

        ObjectJS.initAddTaskPlate();
    };

    //文档点击的隐藏事件
    ObjectJS.bindDocumentClick = function () {
        $(document).bind("click", function (e) {
            //隐藏制版列操作下拉框
            if (!$(e.target).parents().hasClass("ico-dropdown") && !$(e.target).hasClass("ico-dropdown")) {
                $("#setPlateInfo").hide();
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

    //制版的内容点击
    ObjectJS.bindContentClick = function () {
        $("#platemakingBody .tr-content td").unbind().bind("click", function () {
            $(this).find('.tbContent').hide();
            $(this).find('.tbContentIpt').show().focus();
        });
    }

    //添加制版新列
    ObjectJS.bindAddColumn = function () {
        $("#btn-addColumn").unbind().bind("click", function () {
            $("#setTaskPlateAttrBox").remove();
            ObjectJS.columnnameid = $(this).data("columnname");

            var innerHtml = '<ul id="setTaskPlateAttrBox" class="role-items">';
            var noHaveLi = true;
            for (var i = 0; len = CacheAttrValues.length, i < len; i++) {
                var item = CacheAttrValues[i];
                if ($(".table-list td[data-columnname='columnname_" + item.ValueID + "']").length == 0) {
                    innerHtml += '<li class="role-item" data-id="' + item.ValueID + '">' + item.ValueName + '</li>';
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
                    header: "新增制版属性列",
                    content: innerHtml,
                    yesFn: function () {
                        var $hovers = $("#setTaskPlateAttrBox li.hover");
                        if ($hovers.length == 0) return;
                        var newColumnHeadr = '';
                        var newColumn = '';
                        $hovers.each(function () {
                            var columnnameid = $(this).data("id");
                            var columnnamename = $(this).html();

                            newColumnHeadr += '<td class="width100 tLeft columnHeadr" data-columnname="columnname_' + columnnameid + '" data-id="' + columnnameid + '">';
                            newColumnHeadr += '<span>' + columnnamename + '</span>';
                            newColumnHeadr += '<span class="ico-dropdown mRight10 right" data-columnname="columnname_' + columnnameid + '"></span>';
                            newColumnHeadr += '</td>';

                            newColumn += '<td class="tLeft width100" data-columnname="columnname_' + columnnameid + '">';
                            newColumn += '<span class="tbContent"></span>';
                            newColumn += '<input class="tbContentIpt" value="" type="text"/>';
                            newColumn += '</td>';
                        });

                        $("#platemakingBody td[data-columnname='" + ObjectJS.columnnameid + "']").eq(0).after(newColumnHeadr);
                        $("#platemakingBody td[data-columnname='" + ObjectJS.columnnameid + "']:gt(0)").after(newColumn).find("tbContentIpt").show();

                        ObjectJS.bindDropDown();
                        ObjectJS.bindContentClick();
                        ObjectJS.bindAddRow();
                        ObjectJS.bindRemoveRow();
                    }
                }

            });

            $("#setTaskPlateAttrBox .role-item").click(function () {
                if (!$(this).hasClass("hover"))
                    $(this).addClass("hover");
                else
                    $(this).removeClass("hover");
            });

        });
    }

    //删除制版列
    ObjectJS.bindRemoveColumn = function () {
        $("#btn-removeColumn").unbind().bind("click", function () {
            if ($("#platemakingBody .tr-header td").length == 3) {
                alert("只剩最后一列,不能删除");
                return;
            }

            $("#platemakingBody .table-list td[data-columnname='" + $(this).data("columnname") + "']").remove();
        });
    }

    //添加制版行
    ObjectJS.bindAddRow = function () {
        $("div.btn-addRow").unbind().bind('click', function () {
            var $newTR = $("<tr class='tr-content'>" + $(this).parent().parent().parent().html() + "</tr>");
            $newTR.find(".tbContentIpt").empty().attr("value", "").show();
            $newTR.find(".tbContent").empty();
            $(this).parent().parent().parent().after($newTR);

            ObjectJS.bindContentClick();
            ObjectJS.bindAddRow();
            ObjectJS.bindRemoveRow();
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
        });
    }

    //删除制版操作按钮
    ObjectJS.removeTaskPlateOperate = function () {
        $("span.ico-dropdown").remove();
        $("#platemakingBody table tr").each(function () {
            $(this).find("td:last").remove();
        });
    }

    //初始化制版属性行列
    ObjectJS.initAddTaskPlate = function () {
        if ($("#btn-addTaskPlate").length == 0) return;

        $("#btn-addTaskPlate").unbind().bind("click", function () {
            var noHaveLi = false;
            var innerHtml = '<ul id="setTaskPlateAttrBox" class="role-items">';
            for (var i = 0; len = CacheAttrValues.length, i < len; i++) {
                var item = CacheAttrValues[i];
                innerHtml += '<li class="role-item" data-id="' + item.ValueID + '">' + item.ValueName + '</li>';
            }
            innerHtml += '</ul>';

            if (CacheAttrValues.length == 0) {
                noHaveLi = true;
                innerHtml = '<div style="width:300px;">制版属性没有配置,请联系后台管理员配置</div>';
            }

            Easydialog.open({
                container: {
                    id: "show-model-initAddTaskPlate",
                    header: "新增制版属性列",
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

                        $hovers.each(function () {
                            var columnnameid = $(this).data("id");
                            var columnnamename = $(this).html();

                            newColumnHeadr += '<td class="width100 tLeft columnHeadr" data-columnname="columnname_' + columnnameid + '" data-id="' + columnnameid + '">';
                            newColumnHeadr += '<span>' + columnnamename + '</span>';
                            newColumnHeadr += '<span class="ico-dropdown mRight10 right" data-columnname="columnname_' + columnnameid + '"></span>';
                            newColumnHeadr += '</td>';

                            newColumn += '<td class="tLeft width100" data-columnname="columnname_' + columnnameid + '">';
                            newColumn += '<span class="tbContent"></span>';
                            newColumn += '<input class="tbContentIpt" value="" type="text"/>';
                            newColumn += '</td>';
                        });

                        newColumnHeadr += '<td class="width150 center">操作</td>';
                        newColumnHeadr += '</tr>';

                        newColumn += '<td class="width150 center">';
                        newColumn += '    <div class="platemakingOperate">';
                        newColumn += '        <div class="btn-addRow btn-create left" title="添加新行"></div>';
                        newColumn += '        <div class="btn-removeRow btn-remove mLeft10 left" title="删除此行"></div>';
                        newColumn += '        <div class="clear"></div>';
                        newColumn += '   </div>';
                        newColumn += '</td>';
                        newColumn += '</tr>';

                        tableHtml += newColumnHeadr + newColumn;
                        tableHtml += '</table>';

                        $("#platemakingBody").html(tableHtml).css({ "border-top": "1px solid #eee", "border-left": "1px solid #eee" }).show();

                        ObjectJS.bindDropDown();
                        ObjectJS.bindContentClick();
                        ObjectJS.bindAddRow();
                        ObjectJS.bindRemoveRow();

                        $("#btn-updateTaskRemark").show();
                        $("#btn-addTaskPlate").hide();
                    }
                }

            });

            $("#setTaskPlateAttrBox .role-item").click(function () {
                if (!$(this).hasClass("hover"))
                    $(this).addClass("hover");
                else
                    $(this).removeClass("hover");
            });

        });
    }

    //保存制版信息
    ObjectJS.updateOrderPlatemaking = function () {
        if ($("#platemakingBody").html() == "") { return; }

        //if ($(".tbContentIpt:visible").length == 0) { return; }

        $(".tbContentIpt:visible").each(function () {
            $(this).attr("value", $(this).val()).hide().prev().html($(this).val()).show();
        });

        //var valueIDs = '';
        //$("#platemakingBody .tr-header td.columnHeadr").each(function () {
        //    valueIDs += $(this).data("id") + '|';
        //});

        ObjectJS.isLoading = false;
        Global.post("/Task/UpdateOrderPlateRemark", {
            orderID: ObjectJS.orderid,
            taskID: ObjectJS.taskid,
            plateRemark: encodeURI($("#platemakingBody").html())
        }, function (data) {
            if (data.result == 1) {
                alert("保存成功");
                ObjectJS.isPlate = true;
            }
            else {
                alert("保存失败");
            }
            ObjectJS.isLoading = true;
        });
    }

    //制版工艺
    ObjectJS.initPlateMaking = function () {
        $("#btnAddPalte").click(function () {
            ObjectJS.addPlateMaking();            
        });

        $("#setObjectPlate").click(function () {
            var index = $(this).data("index");
            var item = PlateMakings[index];
            Plate = {
                PlateID: item.PlateID,
                Title: $("#plateTitle").val(),
                Remark: $("#plateRemark").val(),
                Icon: $("#plateIcon").val(),
                OrderID: ObjectJS.orderid,
                TaskID: ObjectJS.taskid,
                Type: $("#selectType").val()
            }
            
            ObjectJS.savePlateMaking(item);
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
            orderID:ObjectJS.orderid
        }, function (data) {
            $(".tb-plates").html('');

            if (data.items.length > 0) {
                DoT.exec("template/task/platemarting-list.html", function (template) {
                    PlateMakings = data.items;
                    var html = template(data.items);
                    html = $(html);
                    $(".tb-plates").append(html);

                    $(".typetitle").css({"background-color":"#eee","color":"#007aff"});
                    $(".typetitle:first").css("height", "46px");

                    if ($("#btnAddPalte").length == 1) {
                        html.find(".dropdown").click(function () {
                            var _this = $(this);
                            var position = _this.find(".ico-dropdown").position();
                            $("#setPlateMaking li").data("id", _this.data("id")).data("index", _this.data("index")).data("title", _this.data("title"));

                            $("#setPlateMaking").css({ "top": position.top + 20, "left": position.left - 80 }).show().mouseleave(function () {
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
                $(".tb-plates").html("<tr><td colspan='5'><div class='nodata-txt'>暂无数据!<div></td></tr>");
            }
        });
    }

    //新增工艺说明
    ObjectJS.addPlateMaking = function () {
        var item = {
            PlateID:"",
            Title: "",
            Remark: "",
            Icon: "",
            OrderID: "",
            TaskID: "",
            Type:1
        }

        ObjectJS.savePlateMaking(item);
    }

    //保存工艺说明
    ObjectJS.savePlateMaking = function (item) {
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

                        Plate = {
                            PlateID: item.PlateID,
                            Title: $("#plateTitle").val(),
                            Remark: $("#plateRemark").val(),
                            Icon: $("#plateIcon").val(),
                            OrderID: ObjectJS.orderid,
                            TaskID: ObjectJS.taskid,
                            Type: $("#selectType").val()
                        }
         
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

            $("#selectType").val(item.Type);

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
    //#endregion 任务制版相关事件

    module.exports = ObjectJS;
});