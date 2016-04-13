
define(function (require, exports, module) {
    require("plug/showtaskdetail/style.css");
    require("plug/showtaskdetail/style.css");
    var doT = require("dot");
    var Global = require("global"),
        ChooseUser = require("chooseuser");
    var Qqface = require("qqface");

    (function ($) {
        //默认参数
        var defaultParas = {
            Name: "tb-task-list",
            UpdateTaskEndTimeCallBack: function (endtime, taskid) { },
            FinishTaskCallBack: function () { }
        };

        var IsClickEventFinish = true;
        $.fn.showtaskdetail = function (options) {
            defaultParas = $.extend([], defaultParas, options);

            $(this).click(function () {
                if (IsClickEventFinish) {
                    IsClickEventFinish = false;
                    $(this).addClass("taskDetailContent");

                    var taskid = $(this).data("taskid");
                    var orderid = $(this).data("orderid");
                    var stageid = $(this).data("stageid");
                    var mark = $(this).data("mark");
                    var self = $(this).data("self");

                    var $taskDetailContent = $("#taskDetailContent");

                    //没有任务详情对象
                    if ($taskDetailContent.length == 0) {
                        drawTaskDetail(taskid, orderid, stageid, mark, self);
                    }
                    else {
                        //查询新的任务详情
                        if ($taskDetailContent.data("taskid") != taskid) {
                            drawTaskDetail(taskid, orderid, stageid, mark, self);
                        }
                        else//隐藏显示的任务详情
                        {
                            if ($taskDetailContent.css("width") == "480px") {
                                $taskDetailContent.animate({ width: '0px' }, 200);
                            }
                            else
                                $taskDetailContent.show().animate({ width: '480px' }, 200);
                        }
                    }
                    IsClickEventFinish = true;

                }

            });
        };

        //获取任务详情
        var drawTaskDetail = function (taskid, orderid, stageid, mark, self) {

            doT.exec("plug/showtaskdetail/task-detail.html", function (template) {
                Global.post("/task/GetTaskDetail", { id: taskid }, function (data) {
                    //获取任务详情内容
                    var arr = [];
                    arr.push(data.item);
                    var html = template(arr);

                    $("#taskDetailContent").remove();
                    $("body").append(html);


                    $("#taskDetailContent").animate({ width: '480px' }, 200);

                    //隐藏下拉
                    $(document).click(function (e) {
                        if (!$(e.target).parents().hasClass("task-layer-box") && !$(e.target).hasClass("task-layer-box")
                            && !$(e.target).parents().hasClass("easyDialog_wrapper") && !$(e.target).hasClass("easyDialog_wrapper") && !$(e.target).parents().hasClass("alert") && !$(e.target).hasClass("alert")
                            && !$(e.target).parents().hasClass("stage-items") && !$(e.target).hasClass("stage-items")) {
                            $("#taskDetailContent").animate({ width: '0px' }, 100);
                        }
                    });

                    //绑定讨论表情
                    $('#btn-emotion').qqFace({
                        assign: 'txtContent',
                        path: '/modules/plug/qqface/arclist/'	//表情存放的路径
                    });

                    if (self == 1) {
                        $("#changeTaskOwner").click(function () {
                            var _this = $(this);
                            ChooseUser.create({
                                title: "更换负责人",
                                type: 1,
                                single: true,
                                callback: function (items) {
                                    if (items.length > 0) {
                                        if (_this.data("userid") != items[0].id) {
                                            Global.post("/Task/UpdateTaskOwner", {
                                                userid: items[0].id,
                                                taskid: taskid
                                            }, function (data) {
                                                if (data.result) {
                                                    $("#taskOwnerID").text(items[0].name);
                                                    $("#changeTaskOwner").data("userid", items[0].id);
                                                }
                                            });
                                        } else {
                                            alert("请选择不同人员进行更换!");
                                        }
                                    }
                                }
                            });
                        });
                    } else {
                        $("#changeTaskOwner").hide();
                        $(".tast-link-controller").attr("href","javascript:void(0)")
                    }

                    initTalkReply(orderid, stageid, mark, self);

                });

            });
            
        }

        //更新任务到期日期
        var UpdateTaskEndTime = function (taskID) {
            Global.post("/Task/UpdateTaskEndTime", { id: taskID, endTime: $("#UpdateTaskEndTime").val() }, function (data) {
                if (data.result == 1)
                {
                    defaultParas.UpdateTaskEndTimeCallBack($("#UpdateTaskEndTime").val(), taskID);
                }
            });
        }

        //标记任务完成
        var FinishTask = function (taskID) {
            confirm("标记完成的任务不可逆,确定完成?", function () {
                Global.post("/Task/FinishTask", { id: taskID }, function (data) {
                    if (data.result == 1) {
                        alert("标记任务完成");
                        $("#FinishTask").addClass("btnccc").val("已完成").attr("disabled", "disabled");

                        defaultParas.FinishTaskCallBack();
                    }
                    else if (data.result == 2) {
                        alert("前面阶段任务有未完成,不能标记完成");
                    }
                    else if (data.result == 3) {
                        alert("无权限操作");
                    }
                    else if (data.result == -1) {
                        alert("保存失败");
                    }
                });
            });
        }

        //初始化任务讨论列表
        var initTalkReply = function (orderid, stageid, mark, isSelf) {
            var _self = this;

            $("#btnSaveTalk").click(function () {
                var txt = $("#txtContent");

                if (txt.val().trim()) {
                    var model = {
                        GUID: orderid,
                        StageID: stageid,
                        Mark: mark,
                        Content: txt.val().trim(),
                        FromReplyID: "",
                        FromReplyUserID: "",
                        FromReplyAgentID: ""
                    };
                    saveTaskReply(model);

                    txt.val("");
                }

            });

            if (isSelf == 1) {
                
            } else {
                //$(".talk-body .content-main").hide();
            }
            getTaskReplys(orderid, stageid, mark, 1, isSelf);

        }

        //获取任务讨论列表
        var getTaskReplys = function (orderid, stageid, mark, page, isSelf) {
            var _self = this;
            $("#replyList").empty();

            Global.post("/Opportunitys/GetReplys", {
                guid: orderid,
                stageid: stageid,
                mark: mark,
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
                                Mark: mark,
                                Content: $("#Msg_" + _this.data("replyid")).val().trim(),
                                FromReplyID: _this.data("replyid"),
                                FromReplyUserID: _this.data("createuserid"),
                                FromReplyAgentID: _this.data("agentid")
                            };

                            saveTaskReply(entity);
                        }

                        $("#Msg_" + _this.data("replyid")).val('');
                        $(this).parent().slideUp(300);
                    });

                    innerhtml.find(".reply-content").each(function () {
                        $(this).html(Global.replaceQqface($(this).html()));
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
                        getTaskReplys(orderid, stageid, mark, page, isSelf);
                    }
                });
            });
        }

        //保存任务讨论
        var saveTaskReply = function (model) {
            var _self = this;

            Global.post("/Opportunitys/SavaReply", { entity: JSON.stringify(model) }, function (data) {
                doT.exec("template/customer/replys.html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);

                    $("#replyList").prepend(innerhtml);

                    innerhtml.find(".btn-reply").click(function () {
                        var _this = $(this), reply = _this.nextAll(".reply-box");
                        reply.slideDown(500);
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
                                Content: $("#Msg_" + _this.data("replyid")).val().trim(),
                                FromReplyID: _this.data("replyid"),
                                FromReplyUserID: _this.data("createuserid"),
                                FromReplyAgentID: _this.data("agentid")
                            };
                            saveTaskReply(entity);

                        }
                        $("#Msg_" + _this.data("replyid")).val('');
                        $(this).parent().slideUp(300);
                    });

                    innerhtml.find(".reply-content").each(function () {
                        $(this).html(Global.replaceQqface($(this).html()));
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


    })(jQuery)

    module.exports = jQuery;
});