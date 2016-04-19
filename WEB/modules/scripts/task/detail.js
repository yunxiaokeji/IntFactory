define(function (require, exports, module) {
    var doT = require("dot");
    var Global = require("global");
    var Qqface= require("qqface");
    var Easydialog = null;
    var ChooseUser =null;
    require("pager");

    var ObjectJS = {};
    var CacheAttrValues = [];//订单品类属性缓存
    var Editor;

    ///taskid：任务id
    ///orderid:订单id
    ///stageid：订单阶段id
    ///mark:任务标记 1：材料 2 制版 3大货材料
    ///finishStatus：任务完成状态
    ///attrValues:订单品类属性
    ///orderType:订单类型
    ///um:富文本编辑器
    ///plateRemark:制版工艺描述
    ObjectJS.init = function ( attrValues, um, plateRemark, orderimages, endTime, task) {
        var task = JSON.parse(task.replace(/&quot;/g, '"'));

        ObjectJS.orderid = task.OrderID;
        ObjectJS.ownerid = task.OwnerID;
        ObjectJS.endTime = endTime;
        ObjectJS.finishStatus = task.FinishStatus;
        ObjectJS.stageid = task.StageID;
        ObjectJS.taskid = task.TaskID;
        ObjectJS.orderType = task.OrderType;
        ObjectJS.isPlate = true;//任务是否制版
        if(attrValues!="")
            CacheAttrValues=JSON.parse(attrValues.replace(/&quot;/g, '"'));//制版属性缓存
        Editor = um;
        ObjectJS.mark = 0;//任务讨论标记 用于获取讨论列表
        ObjectJS.taskMark = task.Mark;//任务标记 用于做标记任务完成的限制条件
        ObjectJS.materialMark = 0;//任务材料标记 用于算材料列表的金额统计
        ObjectJS.orderimages = orderimages;

        ObjectJS.bindEvent();

        if (task.Mark == 0)
            ObjectJS.getTaskReplys(1);

        //材料任务
        if ($("#btn-addMaterial").length == 1) {
            ObjectJS.materialMark = 1;
            if (ObjectJS.taskMark == 3) {
                ObjectJS.materialMark = 2;
            }

            ObjectJS.bindProduct();

            ObjectJS.removeTaskPlateOperate();
        }
       //制版任务
        else if ($("#btn-updateTaskRemark").length == 1) {
            if (Easydialog == null) {
                Easydialog = require("easydialog");
            }
            ObjectJS.bindPlatemakingEvent();
        }
        else{
            ObjectJS.removeTaskPlateOperate();
        }

        
        //统计材料总金额
        ObjectJS.getProductAmount();

        if (Editor) {
            //制版工艺描述 富文本
            Editor.ready(function () {
                Editor.setContent(decodeURI(plateRemark));

                $(".edui-container").click(function () {
                    $(".edui-body-container").animate({ height: "600px" }, 500);
                });


                $(document).click(function (e) {
                    //隐藏制版列操作下拉框
                    if (!$(e.target).parents().hasClass("replyBox") && !$(e.target).hasClass("replyBox")) {

                        $(".replyBox").removeClass("replybox-hover");
                    }


                    //隐藏制版列操作下拉框
                    if (!$(e.target).parents().hasClass("edui-container") && !$(e.target).hasClass("edui-container")) {
                        $(".edui-body-container").animate({ height: "100px" }, 500);
                    }
                });

            });
        }

        if ($("#PlateRemark").length == 1) {
            if (plateRemark != "") {
                $("#PlateRemark").html(decodeURI(plateRemark));
            }
            else {
                $("#PlateRemark").html(decodeURI("<div class='pAll10'>暂无工艺说明</div>"));
            }
        }

        if (ObjectJS.taskMark == 2) {
            if ($("#platemakingBody .table-list").length == 0) {
                ObjectJS.isPlate = false;
            }
        }

    };



    ///任务基本信息操作事件
    //绑定事件
    ObjectJS.bindEvent = function () {

        //切换模块
        $(".module-tab li").click(function () {
            var _this = $(this);
            if (_this.hasClass("hover")) return;

            _this.siblings().removeClass("hover");
            _this.addClass("hover");

            $("#navTask").children().hide();
            $("#" + _this.data("id")).show();

            if (_this.data("id") == "orderTaskLogs") {
                //任务日志列表
                ObjectJS.getLogs(1);
            }
            else if (_this.data("id") == "taskReplys") {
                //任务讨论列表
                ObjectJS.getTaskReplys(1);
            }

        });

        //任务讨论盒子点击
        $(".replyBox").click(function () {

            $(this).addClass("replybox-hover");
            $(this).find(".replyContent").focus();
        });

        //任务讨论盒子隐藏
        $(document).click(function (e) {
            if (!$(e.target).parents().hasClass("replyBox") && !$(e.target).hasClass("replyBox")) {
                $(".replyBox").removeClass("replybox-hover");
            }
        });


        //标记任务完成
        if ($("#FinishTask").length == 1) {
            $("#FinishTask").click(function () {
                ObjectJS.finishTask();
            });
        }

        //初始化任务讨论列表
        ObjectJS.initTalkReply();

        //绑定任务样式图
        ObjectJS.bindOrderImages();

        //添加任务成员
        if ($("#addTaskMembers").length == 1)
        {
            ChooseUser = require("chooseuser");

            $("#addTaskMembers").click(function () {
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

                            if ($("#taskMemberIDs" + " div[data-id='" + item.id + "']").html()) {
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

            //删除任务成员
            $("#taskMemberIDs a.removeTaskMember").unbind().click(function () {
                var memberID = $(this).data("id");
                confirm("确定删除任务成员?", function () {
                    ObjectJS.removeTaskMember(memberID);
                });
            });

        }

        //绑定讨论表情
        $('#btn-emotion').qqFace({
            assign: 'txtContent',
            path: '/modules/plug/qqface/arclist/'	//表情存放的路径
        });

        //显示剩余时间
        ObjectJS.showTime();

        //接受任务
        if ($("#AcceptTask").length == 1) {
            $("#AcceptTask").click(function () {
                ObjectJS.updateTaskEndTime();
            });
        }
    }

    //更改任务到期时间
    ObjectJS.updateTaskEndTime = function () {
        Easydialog = require("easydialog");
        var innerHtml = '<div class="pTop10 pBottom5"><span class="width80" style="display:inline-block;">到期时间:</span><input style="width:180px;" type="text" class="taskEndTime" id="UpdateTaskEndTime" placeholder="设置到期时间"/></div>';
        Easydialog.open({
            container: {
                id: "show-model-setRole",
                header: "设置任务到期时间",
                content: innerHtml,
                yesFn: function () {
                    if ($("#UpdateTaskEndTime").val() == "") {
                        alert("任务到期时间不能为空");
                        return;
                    }

                    confirm("任务到期时间不可逆，确定设置?", function () {
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
                        });
                    });

                }
            }
        });

        var myDate = new Date();
        var minDate = myDate.toLocaleDateString();
        minDate = minDate + " 23:59:59"
        //更新任务到期日期
        var taskEndTime = {
            elem: '#UpdateTaskEndTime',
            format: 'YYYY-MM-DD hh:mm:ss',
            min: minDate,
            max: '2099-06-16',
            istime: true,
            istoday: false
        };
        laydate(taskEndTime);


        
    }

    //标记任务完成
    ObjectJS.finishTask = function () {
        if (ObjectJS.taskMark == 1)
        {
            if ($("#navProducts .table-list tr").length == 2) {
                alert("材料没有添加,不能标记任务完成");
                return;
            }
            
        }
        else if (ObjectJS.taskMark == 2)
        {
            if ($("#platemakingBody .table-list").length == 0) {
                alert("制版没有设置,不能标记任务完成");
                return;
            }
            else if (!ObjectJS.isPlate) {
                alert("制版没有设置,不能标记任务完成");
                return;
            }
        }
        
        confirm("标记完成的任务不可逆,确定完成?", function () {
            $("#FinishTask").val("完成中...").attr("disabled", "disabled");
            
            Global.post("/Task/FinishTask",
               {
                 id: ObjectJS.taskid
               }, function (data){
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
            });
        });
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
        var width = document.documentElement.clientWidth, height = document.documentElement.clientHeight;
        $("#orderImage").click(function () {
            if ($(this).attr("src")) {
                $(".enlarge-image-bgbox,.enlarge-image-box").fadeIn();
                $("#enlargeImage").attr("src", $(this).attr("src")).css({ "height": height - 80, "max-width": width - 200 });
                $(".right-enlarge-image,.left-enlarge-image").css({ "top": height / 2 - 80 })
            }
        });
        
        $(".close-enlarge-image").click(function () {
            $(".enlarge-image-bgbox,.enlarge-image-box").fadeOut();
        });

        $(".enlarge-image-bgbox").click(function () {
            $(".enlarge-image-bgbox,.enlarge-image-box").fadeOut();
        });

        $(".left-enlarge-image").click(function () {
            var ele = $(".order-imgs-list .hover").prev();
            if (ele && ele.find("img").attr("src")) {
                var _img = ele.find("img");
                $(".order-imgs-list .hover").removeClass("hover");
                ele.addClass("hover");
                $("#enlargeImage").attr("src", _img.attr("src"));
                $("#orderImage").attr("src", _img.attr("src"));
            }
        });

        $(".right-enlarge-image").click(function () {
            var ele = $(".order-imgs-list .hover").next();
            if (ele && ele.find("img").attr("src")) {
                var _img = ele.find("img");
                $(".order-imgs-list .hover").removeClass("hover");
                ele.addClass("hover");
                $("#enlargeImage").attr("src", _img.attr("src"));
                $("#orderImage").attr("src", _img.attr("src"));
            }
        });
    }

    //添加任务成员
    ObjectJS.addTaskMembers = function (memberIDs) {
        Global.post("/Task/AddTaskMembers", {
            id: ObjectJS.taskid,
            memberIDs: memberIDs
        }, function (data) {
            if (data.result == 0) {
                alert("添加失败");
            }
            else {
                $("#taskMemberIDs a.removeTaskMember").unbind().click(function () {
                    var memberID = $(this).data("id");
                    confirm("确定删除任务成员?", function () {
                        ObjectJS.removeTaskMember(memberID);
                    });
                });
            }
        });
    }

    //删除任务成员
    ObjectJS.removeTaskMember = function (memberID) {
        Global.post("/Task/RemoveTaskMember", {
            id: ObjectJS.taskid,
            memberID: memberID
        }, function (data) {
            if (data.result == 0) {
                alert(memberIDs);
            }
            else {
                $("#taskMemberIDs" + " div[data-id='" + memberID + "']").remove();
            }
        });
    }

    //拼接任务成员html
    ObjectJS.createTaskMember = function (item) {
        var html = '';
        html += '<div class="task-member left" data-id="'+item.id+'">';
        html += '<div class="left pRight5"><span>'+item.name+'</span></div>';
        html += '<div class="left mRight10 pLeft5"><a class="removeTaskMember" href="javascript:void(0);" data-id="' + item.id + '" >×</a></div>';
        html+='<div class="clear"></div>';
        html += '</div>';

        $("#taskMemberIDs").append(html);
    }

    //任务到期时间倒计时
    ObjectJS.showTime = function () {
        if (ObjectJS.endTime == "未设置") {
            return;
        }

        if (ObjectJS.finishStatus==2) {
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
        setTimeout(function () { ObjectJS.showTime() }, 1000);
    }

    ///任务讨论
    //初始化任务讨论列表
    ObjectJS.initTalkReply = function () {
        $("#btnSaveTalk").click(function () {
            var txt = $("#txtContent");

            if (txt.val().trim()) {
                var model = {
                    GUID: ObjectJS.orderid,
                    StageID: ObjectJS.stageid,
                    mark:ObjectJS.mark,
                    Content: txt.val().trim(),
                    FromReplyID: "",
                    FromReplyUserID: "",
                    FromReplyAgentID: ""
                };
                ObjectJS.saveTaskReply(model,$(this));

                txt.val("");
            }

        });
    }

    //获取任务讨论列表
    ObjectJS.getTaskReplys = function (page) {
        var _self = this;
        $("#replyList").empty();
        $("#replyList").html("<tr><td colspan='2' style='border:none;'><div class='data-loading'><div></td></tr>");
        Global.post("/Opportunitys/GetReplys", {
            guid: ObjectJS.orderid,
            stageid: ObjectJS.stageid,
            mark:ObjectJS.mark,
            pageSize: 10,
            pageIndex: page
        }, function (data) {
            $("#replyList").empty();
            if (data.items.length > 0) {
                doT.exec("template/customer/replys.html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);

                    $("#replyList").html(innerhtml);

                    

                    innerhtml.find(".btn-reply").click(function () {
                        var _this = $(this), reply = _this.nextAll(".reply-box");
                        reply.slideDown(300);
                        reply.find("textarea").focus();
                        //reply.find("textarea").blur(function () {
                        //    if (!$(this).val().trim()) {
                        //        reply.slideUp(200);
                        //    }
                        //});
                    });

                    innerhtml.find(".save-reply").click(function () {
                        var _this = $(this);
                        if ($("#Msg_" + _this.data("replyid")).val().trim()) {
                            var entity = {
                                GUID: _this.data("id"),
                                StageID: _this.data("stageid"),
                                Mark: ObjectJS.mark,
                                Content: $("#Msg_" + _this.data("replyid")).val().trim(),
                                FromReplyID: _this.data("replyid"),
                                FromReplyUserID: _this.data("createuserid"),
                                FromReplyAgentID: _this.data("agentid")
                            };

                            ObjectJS.saveTaskReply(entity,_this);
                        }

                        $("#Msg_" + _this.data("replyid")).val('');
                        $(this).parent().slideUp(300);
                    });

                    innerhtml.find(".reply-content").each(function () {
                        $(this).html(Global.replaceQqface($(this).html()));
                    });

                    innerhtml.find('.btn-emotion').each(function(){
                        $(this).qqFace({
                            assign: $(this).data("id"),
                            path: '/modules/plug/qqface/arclist/'	//表情存放的路径
                        });
                    });

                    $(document).click(function (e) {
                        if (!$(e.target).parents().hasClass("reply-box") && !$(e.target).hasClass("reply-box") && !$(e.target).parents().hasClass("btn-reply") && !$(e.target).hasClass("btn-reply") && !$(e.target).parents().hasClass("qqFace") && !$(e.target).hasClass("qqFace")) {
                            
                            $(".reply-box").slideUp(300);
                        }
                    });

                });
            }
            else {
                $("#replyList").html("<tr><td colspan='2' style='border:none;'><div class='nodata-txt' >暂无评论!<div></td></tr>");
            }

            $("#pagerReply").paginate({
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
                    ObjectJS.getTaskReplys(page);
                }
            });
        });
    }

    //保存任务讨论
    ObjectJS.saveTaskReply = function (model, btnObject) {
        var _self = this;
        var btnname = "";
        if (btnObject) {
            btnname = btnObject.html();
            btnObject.html("保存中...").attr("disabled", "disabled");
        }

        Global.post("/Opportunitys/SavaReply", { entity: JSON.stringify(model) }, function (data) {
            if (btnObject) {
                btnObject.html(btnname).removeAttr("disabled");
            }

            doT.exec("template/customer/replys.html", function (template) {
                var innerhtml = template(data.items);
                innerhtml = $(innerhtml);
                innerhtml.hide();
                $("#replyList .nodata-txt").parent().parent().remove();

                $("#replyList").prepend(innerhtml);
                innerhtml.fadeIn(500);

                innerhtml.find(".reply-content").each(function () {
                    $(this).html(Global.replaceQqface($(this).html()));
                });

                innerhtml.find(".btn-reply").click(function () {
                    var _this = $(this), reply = _this.nextAll(".reply-box");
                    reply.slideDown(300);
                    reply.find("textarea").focus();
                    //reply.find("textarea").blur(function () {
                    //    if (!$(this).val().trim()) {
                    //        reply.slideUp(200);
                    //    }
                    //});
                });

                innerhtml.find(".save-reply").click(function () {
                    var _this = $(this);
                    if ($("#Msg_" + _this.data("replyid")).val().trim()) {
                        var entity = {
                            GUID: _this.data("id"),
                            StageID: _this.data("stageid"),
                            Mark: ObjectJS.mark,
                            Content: $("#Msg_" + _this.data("replyid")).val().trim(),
                            FromReplyID: _this.data("replyid"),
                            FromReplyUserID: _this.data("createuserid"),
                            FromReplyAgentID: _this.data("agentid")
                        };
                        ObjectJS.saveTaskReply(entity,_this);

                    }
                    $("#Msg_" + _this.data("replyid")).val('');
                    $(this).parent().slideUp(300);
                });

                innerhtml.find('.btn-emotion').each(function () {
                    $(this).qqFace({
                        assign: $(this).data("id"),
                        path: '/modules/plug/qqface/arclist/'	//表情存放的路径
                    });
                });

                $(document).click(function (e) {
                    if (!$(e.target).parents().hasClass("reply-box") && !$(e.target).hasClass("reply-box") && !$(e.target).parents().hasClass("btn-reply") && !$(e.target).hasClass("btn-reply") && !$(e.target).parents().hasClass("qqFace") && !$(e.target).hasClass("qqFace")) {

                        $(".reply-box").slideUp(300);
                    }
                });

            });
        });
    }

    //获取任务日志
    ObjectJS.getLogs = function (page) {
        var _self = this;
        $("#taskLogList").empty();

        Global.post("/Task/GetOrderTaskLogs", {
            id: _self.taskid,
            pageindex: page
        }, function (data) {

            if (data.items.length > 0) {
                doT.exec("template/common/logs.html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);
                    $("#taskLogList").html(innerhtml);
                });
            }
            else {
                $("#taskLogList").html("<div class='nodata-txt'>暂无数据!</div>");
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
        });
    }


    ///任务材料基本操作
    //绑定事件
    ObjectJS.bindProduct = function () {
        //编辑数量
        $(".quantity").change(function () {
            if ($(this).val().isDouble() && $(this).val() > 0) {
                ObjectJS.editQuantity($(this));
            } else {
                $(this).val($(this).data("value"));
            }
        });

        //编辑价位
        $(".price").change(function () {
            if ($(this).val().isDouble() && $(this).val() >= 0) {
                ObjectJS.editPrice($(this));
            } else {
                $(this).val($(this).data("value"));
            }
        });

        //编辑损耗
        $(".loss").change(function () {
            var loss = parseFloat($(this).val());
            if (!isNaN(loss)) {
                if (loss < 0)
                {
                    if (-loss>= parseFloat($(this).parent().prev().html()) ){
                        $(this).val($(this).data("value"));
                        return;
                    }
                }

                ObjectJS.editLoss($(this));
            } else {
                $(this).val($(this).data("value"));
            }
        });

        //删除产品
        $(".ico-del").click(function () {
            var _this = $(this);
            confirm("确认从清单中移除此材料吗？", function () {
                Global.post("/Orders/DeleteProduct", {
                    orderid: ObjectJS.orderid,
                    autoid: _this.data("id"),
                    name: _this.data("name")
                }, function (data) {
                    if (!data.status) {
                        alert("系统异常，请重新操作！");
                    } else {
                        _this.parents("tr.item").remove();
                        ObjectJS.getProductAmount();
                    }
                });
            });
        });

        if ($("#btnEffectiveOrderProduct").length== 1)
        {
            $("#btnEffectiveOrderProduct").click(function () {
                ObjectJS.effectiveOrderProduct();
            });
        }

    };

    //更改材料单价
    ObjectJS.editPrice = function (ele) {
        var _self = this;
        Global.post("/Orders/UpdateOrderPrice", {
            orderid: _self.orderid,
            autoid: ele.data("id"),
            name: ele.data("name"),
            price: ele.val()
        }, function (data) {
            if (!data.status) {
                ele.val(ele.data("value"));
                alert("当前订单状态,不能进行修改");
            } else {
                ele.data("value", ele.val());
                _self.getProductAmount();
            }
        });
    }

    //更改消耗量
    ObjectJS.editQuantity = function (ele) {
        var _self = this;
        Global.post("/Orders/UpdateProductQuantity", {
            orderid: _self.orderid,
            autoid: ele.data("id"),
            name: ele.data("name"),
            quantity: ele.val()
        }, function (data) {
            if (!data.status) {
                ele.val(ele.data("value"));
                alert("当前订单状态,不能进行修改");
            } else {
                ele.data("value", ele.val());
                _self.getProductAmount();
            }
        });
    }

    //更改损耗量
    ObjectJS.editLoss = function (ele) {
        var _self = this;
        Global.post("/Orders/UpdateProductLoss", {
            orderid: _self.orderid,
            autoid: ele.data("id"),
            name: ele.data("name"),
            quantity: ele.val()
        }, function (data) {
            if (!data.status) {
                ele.val(ele.data("value"));
                alert("当前订单状态,不能进行修改");
            } else {
                ele.data("value", ele.val());
                _self.getProductAmount();
            }
        });
    }

    //计算总金额
    ObjectJS.getProductAmount = function () {
        var amount = 0;
        $(".amount").each(function () {
            var _this = $(this);
            
            if (ObjectJS.materialMark == 0)
            {
                amount += _this.html() * 1;
            }
            else if (ObjectJS.materialMark == 1)
            {
                _this.html(((_this.prevAll(".tr-quantity").find("input").val() * 1) * _this.prevAll(".tr-price").find("label").text()).toFixed(2));
                amount += _this.html() * 1;
            }
            else if(ObjectJS.materialMark == 2)
            {
                _this.html(((_this.prevAll(".tr-quantity").html() * 1 + _this.prevAll(".tr-loss").find("input").val() * 1) * _this.prevAll(".tr-price").find(".price").val() ).toFixed(2));
                amount += _this.html() * 1;
            }
        });

        $("#amount").text(amount.toFixed(2));
    }

    //生成采购单
    ObjectJS.effectiveOrderProduct = function () {
        Global.post("/Orders/EffectiveOrderProduct", {
            orderID: ObjectJS.orderid
        }, function (data) {
            if (data.result == 1)
            {
                location.href = location.href;
            }
        });
    }


    
    ///任务制版相关事件
    //绑定
    ObjectJS.bindPlatemakingEvent = function () {
        ObjectJS.bindDocumentClick();
        ObjectJS.binddropdown();

        ObjectJS.bindContentClick();

        ObjectJS.bindAddColumn();
        ObjectJS.bindRemoveColumn();

        ObjectJS.bindAddRow();
        ObjectJS.bindRemoveRow();

        //$("#btn-platePrint").click(function () {
        //    ObjectJS.platePrint();
        //});

        $("#btn-updateTaskRemark").click(function () {
            ObjectJS.updateOrderPlatemaking();
        });

        $("#btn-updatePlateRemark").click(function () {
            ObjectJS.updateOrderPlateRemark();
        });

        ObjectJS.bindAddTaskPlate();
    };

    //文档点击的隐藏事件
    ObjectJS.bindDocumentClick = function () {
        $(document).unbind().bind("click", function (e) {
            //隐藏制版列操作下拉框
            if (!$(e.target).parents().hasClass("ico-dropdown") && !$(e.target).hasClass("ico-dropdown")) {
                $(".dropdown-ul").hide();
            }
        });


    }

    //显示制版列操作下拉框
    ObjectJS.binddropdown = function () {
        $(".ico-dropdown").unbind().bind("click", function () {
            var _this = $(this);
            var position = _this.position();
            $(".dropdown-ul li").data("columnname", _this.data("columnname"));
            $(".dropdown-ul").css({ "top": position.top + 20, "left": position.left - 70 }).show().mouseleave(function () {
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

    //添加新列
    ObjectJS.bindAddColumn = function () {
        $("#btn-addColumn").unbind().bind("click", function () {
            ObjectJS.columnnameid = $(this).data("columnname");
            var innerHtml = '<ul id="setTaskPlateAttrBox" class="role-items">';
            var noHaveLi = true;
            for (var i = 0;len = CacheAttrValues.length,i < len; i++)
            {
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

                        ObjectJS.binddropdown();
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

    //删除列
    ObjectJS.bindRemoveColumn = function () {
        $("#btn-removeColumn").unbind().bind("click", function () {

            if ($("#platemakingBody .tr-header td").length == 3) {
                alert("只剩最后一列,不能删除");
                return;
            }

            $("#platemakingBody .table-list td[data-columnname='" + $(this).data("columnname") + "']").remove();
        });
    }

    //添加行
    ObjectJS.bindAddRow = function () {
        $("div.btn-addRow").unbind().bind('click', function () {
            var $newTR = $("<tr class='tr-content'>" + $(this).parent().parent().parent().html() + "</tr>");
            $newTR.find(".tbContentIpt").empty().attr("value","").show();
            $newTR.find(".tbContent").empty();
            $(this).parent().parent().parent().after($newTR);

            ObjectJS.bindContentClick();
            ObjectJS.bindAddRow();
            ObjectJS.bindRemoveRow();
        });
    }

    //删除行
    ObjectJS.bindRemoveRow = function () {
        $("div.btn-removeRow").unbind().bind('click', function () {
            if ($("div.btn-removeRow").length == 1) {
                alert("只剩最后一行,不能删除");
                return;
            }

            $(this).parent().parent().parent().remove();
        });
    }

    //删除行操作按钮
    ObjectJS.removeTaskPlateOperate = function () {
        $("span.ico-dropdown").remove();
        $("#platemakingContent table tr").each(function () {
            $(this).find("td:last").remove();
        });
    }

    //新增制版属性列
    ObjectJS.bindAddTaskPlate = function () {
        if ($("#btn-addTaskPlate").length == 0) return;

        $("#btn-addTaskPlate").unbind().bind("click", function () {
            var noHaveLi = false;
            var innerHtml = '<ul id="setTaskPlateAttrBox" class="role-items">';
            for (var i = 0; len = CacheAttrValues.length, i < len; i++) {
                var item = CacheAttrValues[i];
                innerHtml += '<li class="role-item" data-id="' + item.ValueID + '">' + item.ValueName + '</li>';
            }
            innerHtml += '</ul>';

            if (CacheAttrValues.length==0) {
                noHaveLi = true;
                innerHtml = '<div style="width:300px;">制版属性没有配置,请联系后台管理员配置</div>';
            }


            Easydialog.open({
                container: {
                    id: "show-model-setRole",
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

                        $("#platemakingBody").html(tableHtml).css({ "border-top": "1px solid #eee", "border-left": "1px solid #eee" });

                        ObjectJS.binddropdown();
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
        if ($("#platemakingBody").html() == "") return;

        $(".tbContentIpt:visible").each(function () {
            $(this).attr("value", $(this).val() ).hide().prev().html($(this).val()).show();
        });

        var valueIDs = '';
        $("#platemakingBody .tr-header td.columnHeadr").each(function () {
            valueIDs += $(this).data("id") + '|';
        });

        Global.post("/Task/UpdateOrderPlateAttr", {
            orderID: ObjectJS.orderid,
            taskID: ObjectJS.taskid,
            platehtml: encodeURI($("#platemakingBody").html()),
            valueIDs: valueIDs
        }, function (data) {
            if (data.result == 1) {
                alert("保存成功");
                ObjectJS.isPlate = true;
            }
        });
    }

    //保存制版工艺说明
    ObjectJS.updateOrderPlateRemark = function () {
        Global.post("/Task/UpdateOrderPlateRemark", {
            orderID: ObjectJS.orderid,
            plateRemark: encodeURI(Editor.getContent())
        }, function (data) {
            if (data.result == 1) {
                $(".edui-body-container").animate({ height: "100px" }, 500);
            }
        });
    }

    ObjectJS.platePrint = function () {
        //var bdhtml = window.document.body.innerHTML;;
        //var docStr = $("#platemakingBody").html();
        //window.document.body.innerHTML=docStr;
        //window.print();
        //window.document.body.innerHTML = bdhtml;
        $("span.ico-dropdown").hide();
        $("#platemakingContent table tr").each(function () {
            $(this).find("td:last").hide();
        });

        var obj = $("#platemakingBody");
        //打开一个新窗口newWindow
        var newWindow = window.open("打印窗口", "_blank");
        //要打印的div的内容
        var docStr = "<style type='text/css'>.platemakingBody td {border-bottom:1px solid #eee;border-right:1px solid #eee;width:100px;}</style>";
        docStr += obj.html();

        $("span.ico-dropdown").show();
        $("#platemakingContent table tr").each(function () {
            $(this).find("td:last").show();
        });

        //打印内容写入newWindow文档
        newWindow.document.write(docStr);
        //关闭文档
        newWindow.document.close();
        //调用打印机
        newWindow.print();
        //关闭newWindow页面
        newWindow.close();

        
    }
    module.exports = ObjectJS;
});